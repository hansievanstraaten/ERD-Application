using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Columns
{
  /// <summary>
  /// Interaction logic for ColumnsEdit.xaml
  /// </summary>
  public partial class ColumnsEdit : WindowBase
  {
    public ColumnsEdit(ColumnObjectModel column, string tableName)
    {
      this.InitializeComponent();

      this.TableName = tableName;

      this.Column = column;

      this.Column.PropertyChanged += this.ColumnProperty_Changed;

      this.uxColumnViewer.Items.Add(this.Column);

      if (this.Column.InPrimaryKey)
      {
        this.uxColumnViewer["Column Name"].IsEnabled = false;

        this.uxColumnViewer["In Primary Key"].IsEnabled = false;

        this.uxColumnViewer["Allow Nulls"].IsEnabled = false;
      }
    }

    public string TableName {get; private set; }

    public ColumnObjectModel Column {get; private set;}

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.CheckColumnIntegrity(string.Empty);

        if (this.uxColumnViewer.HasValidationError)
        {
          return;
        }

        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }

    private void ColumnProperty_Changed(object sender, PropertyChangedEventArgs e)
    {
      try
      {
        this.Column.PropertyChanged -= this.ColumnProperty_Changed; // Prevent Stack Overflow on PropertyChanged Events

        this.CheckColumnIntegrity(e.PropertyName);
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
      finally
      {
        this.Column.PropertyChanged += this.ColumnProperty_Changed;
      }
    }
    
    private void CheckColumnIntegrity(string propertyName)
    {
      if (!Integrity.KeepColumnsUnique)// || !Integrity.HasGlobalColumn(this.Column.ColumnName))
      {
        return;
      }

      if (Integrity.IsColumnInThisTable(this.TableName, this.Column.ColumnName))
      {
        return;
      }

      if (this.Column.Precision == 0)
      {
        this.Column.Precision = Integrity.GetGlobalPrecision(this.Column.ColumnName);
      }

      if (this.Column.Scale == 0)
      {
        this.Column.Scale = Integrity.GetGlobalScale(this.Column.ColumnName);
      }

      if (this.Column.MaxLength == 0)
      {
        this.Column.MaxLength = Integrity.GetGlobalMaxLength(this.Column.ColumnName);
      }

      SqlDbType? globalDataType = Integrity.GetGlobalColumnDataType(this.Column.ColumnName);

      if (globalDataType.HasValue)
      {
        this.Column.SqlDataType = globalDataType.Value;
      }

      if (this.Column.FriendlyName.IsNullEmptyOrWhiteSpace() || propertyName == "ColumnName")
      {
        this.Column.FriendlyName = Integrity.GetFriendlyName(this.Column.ColumnName);
      }

      if (this.Column.Description.IsNullEmptyOrWhiteSpace() || propertyName == "ColumnName")
      {
        this.Column.Description = Integrity.GetDescription(this.Column.ColumnName);
      }

      if ((Integrity.AllowDatabaseRelations || Integrity.AllowVertualRelations)
          && Integrity.IsPrimaryKeyColumn(this.Column.ColumnName))
      {
        //this.Column.IsForeignkey = true;

        //if (!Integrity.AllowDatabaseRelations && Integrity.AllowVertualRelations)
        //{
        //  this.Column.IsVertualRelation = Integrity.AllowVertualRelations;
        //}

        //this.Column.ForeignKeyTable = Integrity.GetPrimaryKeyTable(this.Column.ColumnName);

        //this.Column.ForeignConstraintName = Integrity.BuildForeighKeyName(this.Column.ForeignKeyTable, this.TableName);

        //this.Column.ForeignKeyColumn = this.Column.ColumnName;
      }
      else
      {
        this.Column.IsForeignkey = false;
        
        this.Column.ForeignKeyTable = string.Empty;

        this.Column.ForeignConstraintName = string.Empty;

        this.Column.ForeignKeyColumn = string.Empty;
      }

    }
  }
}
