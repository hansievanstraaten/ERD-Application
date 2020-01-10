using ERD.Base;
using ERD.Common;
using ERD.DatabaseScripts.Engineering;
using ERD.Models;
using ERD.Viewer.Tables;
using ERD.Viewer.Tools.Relations;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WPF.Tools.Exstention;
using WPF.Tools.Functions;

namespace ERD.Viewer.Tools
{
  internal class TableWorkingCanvas : Canvas
  {
    internal delegate void TableMoveEvent(object sender, bool isDrag);

    internal delegate void TableAddedEvent(object sender, TableModel table);

    internal delegate void RemoveTableEvent(object sender, TableModel tableModel);

    internal delegate void CanvasChangedEvent(object sender, object changedItem);

    internal event TableMoveEvent TableMove;

    internal event TableAddedEvent TableAdded;

    internal event RemoveTableEvent RemoveTable;

    internal event CanvasChangedEvent CanvasChanged;

    private bool wasFirstLoaded = false;

    private Dictionary<string, DatabaseRelation> columnRelationModel = new Dictionary<string, DatabaseRelation>();

    public TableWorkingCanvas()
    {
      this.AllowDrop = true;
      
      this.Loaded += this.TableWorkingCanvas_Loaded;
    }

    public string NewTablePrefix {get; set;}

    public DatabaseModel DatabaseModel {get; set;}
    
    public async void CreateTableObject(TableModel table)
    {
      ReverseEngineer reverseEngineer = new ReverseEngineer(this.Dispatcher);

      await Task.Factory.StartNew(() => 
      {
        if (!table.IsNewTable && (table.Columns == null || table.Columns.Count() == 0))
        {
          EventParser.ParseMessage(this, this.Dispatcher, string.Empty, "Reading table information.");

          table.Columns = table.Columns.AddRange(reverseEngineer.GetTableColumns(table.TableName).ToArray());
        
          table.PrimaryKeyClusterConstraintName = reverseEngineer.GetTablePrimaryKeyCluster(table.TableName);
        }
      });

      
      table.IsNewTable = false;

      table.IsDeleted = false;

      Integrity.MapTable(table);

      TableObject tableControl = new TableObject(table);

      tableControl.TableMove += this.TableObject_Move;

      tableControl.RemoveTable += this.TableObject_Remove;

      tableControl.ForeignKeyColumnAdded += this.TableObject_ForeignKeyAdded;

      foreach (KeyValuePair<string, DatabaseRelation> relation in tableControl.ColumnRelationModel)
      {
        relation.Value.DatabaseRelationDelete += this.DatabaseRelation_Delete;

        this.columnRelationModel.Add(relation.Key, relation.Value);
      }

      this.Children.Add(tableControl);

      this.ResizeCanvas(tableControl.Location, tableControl.ActualHeight, tableControl.ActualWidth);

      if (this.IsLoaded)
      {
        tableControl.InvalidateVisual();

        this.AddRelationsDrawing(tableControl);
      }

      tableControl.TableColumnChanged += this.TableColumn_Changed;
    }
    
    protected override void OnDragOver(DragEventArgs e)
    {
      e.Handled = true;

      base.OnDragOver(e);

      if (e.Effects == DragDropEffects.Move)
      {
        if (e.Data.GetDataPresent(DataFormats.StringFormat))
        {
          object dataValue = e.Data.GetData(typeof(TableModel));

          if (dataValue == null || dataValue.GetType() != typeof(TableModel))
          {
            return;
          }

          if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
          {
            e.Effects = DragDropEffects.Copy;
          }
          else
          {
            e.Effects = DragDropEffects.Move;
          }
        }
      }
    }

    protected override void OnDrop(DragEventArgs e)
    {
      e.Handled = true;

      if (e.Data.GetDataPresent(DataFormats.StringFormat))
      { 
        #region TABLE/RElATION DROPED FROM LEFT MENU

        object dataValue = e.Data.GetData(typeof(TableModel));
          
        this.CanvasChanged?.Invoke(this, dataValue);

        if (dataValue == null || dataValue.GetType() != typeof(TableModel))
        {
          dataValue = e.Data.GetData(typeof(DatabaseRelation));

          if (dataValue == null)
          {
            return;
          }

          this.AddNewRelation((DatabaseRelation)dataValue);
          
          return;
        }

        TableModel table = (TableModel)dataValue;

        if (table.IsNewTable)
        {
          table = new TableModel();
            
          table.TableName = $"{this.NewTablePrefix}{this.Children.Count}";
          
          TableEdit tableEdit = new TableEdit(table);

          bool? result = tableEdit.ShowDialog();

          if (!result.HasValue || !result.Value)
          {
            return;
          }
        }

        if (!this.NewTablePrefix.IsNullEmptyOrWhiteSpace() && !table.TableName.StartsWith(this.NewTablePrefix))
        {
          table.TableName = $"{this.NewTablePrefix}{table.TableName}";
        }

        table.CanvasLocation = e.GetPosition(this);

        this.CreateTableObject(table);

        this.TableAdded?.Invoke(this, table);

        #endregion
      }
      else if (e.Data.GetDataPresent(DataFormats.Serializable))
      {
        #region TABLE MOVE ON CANVAS

        object[] dataValues = (object[])e.Data.GetData(DataFormats.Serializable);

        if ((Type) dataValues[0] == typeof(TableObject))
        {
          TableObject table = (TableObject) dataValues[1];

          Point mousePos = e.GetPosition(this);

          table.Location = mousePos;

          table.StopDrag();

          this.ResizeCanvas(mousePos, table.ActualHeight, table.ActualWidth);

          List<KeyValuePair<string, DatabaseRelation>> changedRelations = new List<KeyValuePair<string, DatabaseRelation>>();

          foreach (string mapedRelation in table.LinkedRelations)
          {
            if (!this.columnRelationModel.ContainsKey(mapedRelation))
            {
              continue;
            }

            DatabaseRelation relationKeyValue = this.columnRelationModel[mapedRelation];

            foreach (Path relationPath in this.FindVisualControls(typeof(Path)).Where(p => ((Path) p).Name.EndsWith(relationKeyValue.RelationshipName)))
            {
              this.Children.Remove(relationPath);
            }

            changedRelations.Add(new KeyValuePair<string, DatabaseRelation>(mapedRelation, relationKeyValue));
          }
          
          UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

          foreach (KeyValuePair<string, DatabaseRelation> relation in changedRelations)
          {
            string[] tablesUsed = relation.Key.Split(new string[] { "||"}, StringSplitOptions.RemoveEmptyEntries);

            UIElement parentTable = tableControls.FirstOrDefault(to => ((TableObject)to).Table.TableName == tablesUsed[0]);

            if (parentTable == null)
            {
              this.DrawStubRelation(table, relation);

              continue;
            }

            this.DrawRelation((TableObject)parentTable, tableControls, relation);
          }
          
          table.InvalidateVisual();
        }

        #endregion
      }

      base.OnDrop(e);
    }
    
    private void TableObject_Move(object sender, bool isdrag)
    {
      if (this.TableMove != null)
      {
        this.TableMove(sender, isdrag);
      }
    }
    
    private void TableObject_Remove(object sender, TableModel tableModel)
    {
      this.Children.Remove((TableObject) sender);

      if (this.RemoveTable != null)
      {
        TableObject tableObject = (TableObject) sender;
        
        foreach (KeyValuePair<string, DatabaseRelation> relation in tableObject.ColumnRelationModel)
        {
          this.Children.Remove(relation.Value.LinePath[0]);

          this.Children.Remove(relation.Value.LinePath[1]);

          this.columnRelationModel.Remove(relation.Key);
        }

        this.RemoveTable?.Invoke(this, tableModel);

        this.CanvasChanged?.Invoke(this, tableModel);

        Integrity.RemoveTableMapping(tableModel);
      }
    }

    private bool TableObject_ForeignKeyAdded(object sender, ColumnObjectModel column)
    {
      try
      {
        TableObject table = (TableObject)sender;

        List<string> exixtingRelations = table.ColumnRelationModel.Keys.ToList();

        table.InitializeColumnRemations();

        foreach (KeyValuePair<string, DatabaseRelation> relationPair in table.ColumnRelationModel)
        {
          if (this.columnRelationModel.ContainsKey(relationPair.Key))
          {
            continue;
          }

          this.columnRelationModel.Add(relationPair.Key, relationPair.Value);
        }

        UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

        string dictionaryKey = table.ColumnRelationModel.Keys.FirstOrDefault(ex => !exixtingRelations.Contains(ex));

        this.DrawRelation(table, tableControls, new KeyValuePair<string, DatabaseRelation>(dictionaryKey, table.ColumnRelationModel[dictionaryKey]));

        return true;
      }
      catch
      {
        return false;
      }
    }

    private void TableWorkingCanvas_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.wasFirstLoaded)
      {
        return;
      }

      try
      {
        foreach (UIElement tableControl in this.FindVisualControls(typeof(TableObject)))
        {
          this.AddRelationsDrawing((TableObject) tableControl);
        }
        
        this.wasFirstLoaded = true;
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }

    private void TableColumn_Changed(object sender, ColumnObjectModel column)
    {
      this.CanvasChanged?.Invoke(this, column);
    }
    
    private void DatabaseRelation_Delete(DatabaseRelation sender)
    {
      try
      {
        Integrity.RemoveForeighKey(sender);
        
        string dictionaryKey = $"{sender.ParentTable}||{sender.ChildTable}";

        UIElement tableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject)t).Table.TableName == sender.ChildTable);

        UIElement parentTableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject)t).Table.TableName == sender.ParentTable);

        if (tableControl != null)
        {
          TableObject childTable = (TableObject)tableControl;

          childTable.ColumnRelationModel.Remove(dictionaryKey);

          childTable.LinkedRelations.Remove(dictionaryKey);
        }

        if (parentTableControl != null)
        {
          TableObject childTable = (TableObject)parentTableControl;

          childTable.ColumnRelationModel.Remove(dictionaryKey);

          childTable.LinkedRelations.Remove(dictionaryKey);
        }

        foreach (Path relationPath in this.FindVisualControls(typeof(Path)).Where(p => ((Path)p).Name.EndsWith(sender.RelationshipName)))
        {
          this.Children.Remove(relationPath);
        }
          
        this.columnRelationModel.Remove(dictionaryKey);

        sender.Dispose();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }

    private void ResizeCanvas(Point dropPoint, double tableHeight, double tableWidth)
    {
      if (tableHeight == 0)
      {
        tableHeight = 1000;
      }

      if (tableWidth == 0)
      {
        tableWidth = 500;
      }

      if (this.Width.IsNan() || this.Width < (dropPoint.X + tableWidth))
      {
        this.Width = dropPoint.X + tableWidth + 100;
      }

      if (this.Height.IsNan() || this.Height < (dropPoint.Y + tableHeight))
      {
        this.Height = dropPoint.Y + tableHeight + 100;
      }
    }

    private void AddNewRelation(DatabaseRelation relation)
    {
      RelationEditor editor = new RelationEditor(relation);

      bool? result = editor.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        return;
      }

      relation.DatabaseRelationDelete += this.DatabaseRelation_Delete;

      UIElement tableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject)t).Table.TableName == editor.DatabaseRelation.ChildTable);

      UIElement parentTableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject)t).Table.TableName == editor.DatabaseRelation.ParentTable);
      
      UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

      if (tableControl != null)
      {
        TableObject childTable = (TableObject)tableControl;
        
        string dictionaryKey = $"{editor.DatabaseRelation.ParentTable}||{editor.DatabaseRelation.ChildTable}";

        childTable.ColumnRelationModel.Add(dictionaryKey, editor.DatabaseRelation);
        
        this.columnRelationModel.Add(dictionaryKey, editor.DatabaseRelation);

        this.DrawRelation((TableObject)parentTableControl, tableControls, new KeyValuePair<string, DatabaseRelation>(dictionaryKey, editor.DatabaseRelation));
      }
    }

    #region LINE MAINTENANCE

    private void AddRelationsDrawing(TableObject parentTable)
    {
      string tableStartKey = $"{parentTable.Table.TableName}||";

      string tableEndKey = $"||{parentTable.Table.TableName}";

      UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));
      
      foreach (KeyValuePair<string, DatabaseRelation> relationKeyValue in this.columnRelationModel.Where(k => k.Key.StartsWith(tableStartKey)))
      {
        this.DrawRelation(parentTable, tableControls, relationKeyValue);
      }

      foreach (KeyValuePair<string, DatabaseRelation> relationKeyValue in this.columnRelationModel.Where(k => k.Key.EndsWith(tableEndKey)))
      {
        UIElement childItem = tableControls.FirstOrDefault(t => (((TableObject)t).Table.TableName == relationKeyValue.Value.ParentTable));

        if (childItem == null)
        {
          this.DrawStubRelation(parentTable, relationKeyValue);

          continue;
        }

        this.DrawRelation((TableObject)childItem, tableControls, relationKeyValue);
      }
    }

    private void DrawStubRelation(TableObject self, KeyValuePair<string, DatabaseRelation> relationKeyValue)
    {
      string[] keysIndex = self.ColumnRelationModel.Select(k => k.Key).ToArray();

      int xOffset = Array.IndexOf(keysIndex, relationKeyValue.Key) * 5;
      
      Point selfPoint = self.Location;

      Point selfStub = self.Location;

      selfPoint.Y += xOffset;

      selfStub.Y += xOffset;

      selfStub.X -= 50;
      
      if (!self.LinkedRelations.Contains(relationKeyValue.Key))
      {
        self.LinkedRelations.Add(relationKeyValue.Key);
      }

      relationKeyValue.Value.ClearPath();

      relationKeyValue.Value.IntersectionPoints = new Point[] { selfPoint, selfStub };

      this.AddLineDrwaings(relationKeyValue, self, self, selfPoint, selfPoint);
    }
    
    private void DrawRelation(TableObject parentTable, UIElement[] tableControls, KeyValuePair<string, DatabaseRelation> relationKeyValue)
    {
      double xOffset = 15;

      double yOffset = 0;

      Point parentPoint = parentTable.Location;

      string[] relationTables = relationKeyValue.Key.Split(new string[] { "||"}, StringSplitOptions.RemoveEmptyEntries);
        
      UIElement childItem = tableControls.FirstOrDefault(t => (((TableObject)t).Table.TableName == relationTables[1]));
        
      if (childItem == null)
      {
        this.DrawStubRelation(parentTable, relationKeyValue);

        return;
      }

      TableObject childTable = (TableObject)childItem;

      Point childPoint = childTable.Location;

      if (!parentTable.LinkedRelations.Contains(relationKeyValue.Key))
      {
        parentTable.LinkedRelations.Add(relationKeyValue.Key);
      }

      if (!childTable.LinkedRelations.Contains(relationKeyValue.Key))
      {
        childTable.LinkedRelations.Add(relationKeyValue.Key);
      }

      relationKeyValue.Value.ClearPath();

      relationKeyValue.Value.IntersectionPoints = this.CalculateIntersectionPoints(parentTable, childTable, relationKeyValue.Key, ref xOffset, ref yOffset, ref parentPoint, ref childPoint);

      this.AddLineDrwaings(relationKeyValue, parentTable, childTable, parentPoint, childPoint);

      parentPoint.X -= parentTable.DesiredSize.Width;
    }

    #endregion

    #region LINE SETUP AND DRAWING AREA

    private void AddLineDrwaings(
      KeyValuePair<string, DatabaseRelation> relationKeyValue, 
      TableObject parentTable, 
      TableObject childTable,
      Point parentPoint,
      Point childPoint)
    {
      relationKeyValue.Value.ParentTablePoint = parentPoint;

      relationKeyValue.Value.ChildTablePoint = childPoint;

      this.Children.Add(relationKeyValue.Value.LinePath[0]);

      this.Children.Add(relationKeyValue.Value.LinePath[1]); 
    }

    private Point[] CalculateIntersectionPoints(
      TableObject parentTable,
      TableObject childTable,
      string relationKey,
      ref double xOffset,
      ref double yOffset,
      ref Point parentPoint, 
      ref Point childPoint)
    {
      Point[] result = new Point[2];
      
      int parentLinkIndex = parentTable.LinkedRelations.IndexOf(relationKey);

      int childLinkIndex = childTable.LinkedRelations.IndexOf(relationKey);

      string[] tablesArray = relationKey.Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);

      int childKeyLinkCount = childTable.Table.Columns.Where(cc => cc.ForeignKeyTable == tablesArray[0] && cc.InPrimaryKey).Count();

      int childKeyCount = childTable.Table.Columns.Where(ck => ck.InPrimaryKey).Count();

      int parentKeyLinkCount = parentTable.Table.Columns.Where(pc => pc.InPrimaryKey).Count();

      bool isStrongKey = (childKeyLinkCount == 0 && childKeyLinkCount != childKeyCount) ? false : childKeyLinkCount == parentKeyLinkCount;

      double parentBottom = parentTable.DesiredSize.Height + parentPoint.Y;

      double childBottom = childTable.DesiredSize.Height + childPoint.Y;

      double parentWidth = parentTable.DesiredSize.Width + parentPoint.X;

      double childWidth = childTable.DesiredSize.Width + childPoint.X;

      TablePlacementEnum placement = ParentPlacementCalculator.CalculatePlacement(parentPoint, childPoint, parentWidth, childWidth, parentBottom, childBottom);
      
      if (childTable.Table.TableName == parentTable.Table.TableName)
      {
        #region TABLE REFERENCE IT SELF

        parentPoint.X += parentTable.DesiredSize.Width;

        parentPoint.Y += yOffset;

        childPoint.X += parentTable.DesiredSize.Width;

        childPoint.Y += (yOffset + 20);
          
        result[0] = new Point(parentPoint.X + xOffset, parentPoint.Y);

        result[1] = new Point(childPoint.X + xOffset, childPoint.Y);
          
        yOffset += 30;

        #endregion
      }
      else if (placement == TablePlacementEnum.ParentAtLeft)
      {
        #region PARENT IS LEFT OF CHILD

        parentPoint.X += parentTable.DesiredSize.Width;
        
        double parentHeightCentre = parentPoint.Y + (parentTable.DesiredSize.Height / 2);

        double childHeightCentre = childPoint.Y + (childTable.DesiredSize.Height / 2);
          
        parentPoint.Y = parentHeightCentre + (parentLinkIndex * 20);

        childPoint.Y = childHeightCentre + (childLinkIndex * 20);
        
        double xOffP = (parentPoint.X - childPoint.X) / 2;

        result[0] = new Point(parentPoint.X - xOffP, parentPoint.Y);

        result[1] = new Point(childPoint.X + xOffP, childPoint.Y);

        if (!isStrongKey)
        {
          result = this.AddRightCrowsFeet(result, childPoint);
        }

        #endregion
      }
      else if (placement == TablePlacementEnum.ParentAtRight)
      {
        #region PARENT IS RIGHT OF CHILD

        childPoint.X += childTable.DesiredSize.Width;

        double parentHeightCentre = parentPoint.Y + (parentTable.DesiredSize.Height / 2);

        double childHeightCentre = childPoint.Y + (childTable.DesiredSize.Height / 2);
        
        parentPoint.Y = parentHeightCentre + (parentLinkIndex * 20);

        childPoint.Y = childHeightCentre + (childLinkIndex * 20);
        
        double xOffP = (parentPoint.X - childPoint.X) / 2;
        
        result[0] = new Point(parentPoint.X - xOffP, parentPoint.Y);

        result[1] = new Point(childPoint.X + xOffP, childPoint.Y);

        if (!isStrongKey)
        {
          result = this.AddLeftCrowsFeet(result, childPoint);
        }

        #endregion
      }
      else if (placement == TablePlacementEnum.ParentAtBottom)
      {
        #region PARENT IS BOTTOM OF CHILD

        childPoint.Y += childTable.DesiredSize.Height;

        double parentWidthCentre = parentPoint.X + (parentTable.DesiredSize.Width / 2);

        double childWidthCentre = childPoint.X + (childTable.DesiredSize.Width / 2);
        
        parentPoint.X = parentWidthCentre + (parentLinkIndex * 20);

        childPoint.X = childWidthCentre + (childLinkIndex * 20);
        
        double yOffP = (parentPoint.Y - childPoint.Y) / 2;
        
        result[0] = new Point(parentPoint.X, parentPoint.Y - yOffP);

        result[1] = new Point(childPoint.X, childPoint.Y + yOffP);

        if (!isStrongKey)
        {
          result = this.AddUpCrowsFeet(result, childPoint);
        }

        #endregion
      }
      else
      {
        #region PARENT IS TOP OF CHILD

        parentPoint.Y += parentTable.DesiredSize.Height;

        double parentWidthCentre = parentPoint.X + (parentTable.DesiredSize.Width / 2);

        double childWidthCentre = childPoint.X + (childTable.DesiredSize.Width / 2);
        
        parentPoint.X = parentWidthCentre + (parentLinkIndex * 20);

        childPoint.X = childWidthCentre + (childLinkIndex * 20);
        
        double yOffP = (parentPoint.Y - childPoint.Y) / 2;
        
        result[0] = new Point(parentPoint.X, parentPoint.Y - yOffP);

        result[1] = new Point(childPoint.X, childPoint.Y + yOffP);

        if (!isStrongKey)
        {
          result = this.AddDownCrowsFeet(result, childPoint);
        }

        #endregion
      }

      return result;
    }

    private Point[] AddLeftCrowsFeet(Point[] intersectionPoints, Point childPoint)
    {
      Point[] result = new Point[7];

      result[0] = intersectionPoints[0];

      result[1] = intersectionPoints[1];

      result[2] = new Point(childPoint.X + 12, childPoint.Y);
      
      result[3] = new Point(childPoint.X + 8, childPoint.Y - 5);
      
      result[4] = new Point(childPoint.X + 12, childPoint.Y);
      
      result[5] = new Point(childPoint.X + 8, childPoint.Y + 5);
      
      result[6] = new Point(childPoint.X + 12, childPoint.Y);

      return result;
    }

    private Point[] AddRightCrowsFeet(Point[] intersectionPoints, Point childPoint)
    {
      Point[] result = new Point[7];

      result[0] = intersectionPoints[0];

      result[1] = intersectionPoints[1];

      result[2] = new Point(childPoint.X - 12, childPoint.Y);

      result[3] = new Point(childPoint.X - 8, childPoint.Y - 5);

      result[4] = new Point(childPoint.X - 12, childPoint.Y);
      
      result[5] = new Point(childPoint.X - 8, childPoint.Y + 5);
      
      result[6] = new Point(childPoint.X - 12, childPoint.Y);

      return result;
    }

    private Point[] AddUpCrowsFeet(Point[] intersectionPoints, Point childPoint)
    {
      Point[] result = new Point[7];

      result[0] = intersectionPoints[0];

      result[1] = intersectionPoints[1];

      result[2] = new Point(childPoint.X, childPoint.Y + 12);

      result[3] = new Point(childPoint.X + 5, childPoint.Y + 8);

      result[4] = new Point(childPoint.X, childPoint.Y + 12);
      
      result[5] = new Point(childPoint.X - 5, childPoint.Y + 8);
      
      result[6] = new Point(childPoint.X, childPoint.Y + 12);

      return result;
    }

    private Point[] AddDownCrowsFeet(Point[] intersectionPoints, Point childPoint)
    {
      Point[] result = new Point[7];

      result[0] = intersectionPoints[0];

      result[1] = intersectionPoints[1];

      result[2] = new Point(childPoint.X, childPoint.Y - 12);

      result[3] = new Point(childPoint.X + 5, childPoint.Y - 8);

      result[4] = new Point(childPoint.X, childPoint.Y - 12);
      
      result[5] = new Point(childPoint.X - 5, childPoint.Y - 8);
      
      result[6] = new Point(childPoint.X, childPoint.Y - 12);

      return result;
    }


    #endregion
  }
}
