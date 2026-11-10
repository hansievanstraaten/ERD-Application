using ERD.Base;
using ERD.Common;
using ERD.DatabaseScripts.Engineering;
using ERD.Models;
using ERD.Viewer.Tables;
using ERD.Viewer.Tools.Relations;
using GeneralExtensions;
using System;
using System.Collections.Generic;
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
        private const double relationClearance = 15.0;
        private const double relationTurnPenalty = 20.0;
        private const double routeBoundaryMargin = 50.0;
        private const double routeEpsilon = 0.001;
        
        private const double linePadding = 5;
        private const double anchorStartOffset = 3;
        private const double relationLeadOutLength = 20.0;

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

        public string NewTablePrefix
        {
            get;
            set;
        }

        public DatabaseModel DatabaseModel
        {
            get;
            set;
        }

        public void RemoveTableFromCanvas(TableModel tableModel)
        {
            try
            {
                TableObject[] tableObjectsArray = this.FindVisualControls(typeof(TableObject)).TryCast<TableObject>();

                TableObject tableObject = tableObjectsArray.FirstOrDefault(t => t.Table.TableName.ToLower() == tableModel.TableName.ToLower());

                if (tableObject == null)
                {
                    return;
                }


                this.Children.Remove(tableObject);

                if (this.RemoveTable != null)
                {
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
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
        
        public void RefreshTableRelations(TableModel tableModel)
        {
            TableObject[] tablesArray = this.FindVisualControls(typeof(TableObject)).TryCast<TableObject>();

            TableObject table = tablesArray.FirstOrDefault(t => t.Table.TableName == tableModel.TableName);

            if (table == null)
            {
                return;
            }

            KeyValuePair<string, DatabaseRelation>[] existingRelation = table.ColumnRelationModel.ToArray();

            foreach (KeyValuePair<string, DatabaseRelation> relation in existingRelation)
            {
                this.DatabaseRelation_Delete(relation.Value);

                if (this.columnRelationModel.ContainsKey(relation.Key))
                {
                    this.columnRelationModel.Remove(relation.Key);
                }
            }
            
            table.InitializeColumnRemations();

            UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

            foreach (KeyValuePair<string, DatabaseRelation> relation in table.ColumnRelationModel)
            {
                relation.Value.DatabaseRelationDelete += this.DatabaseRelation_Delete;

                this.columnRelationModel.Add(relation.Key, relation.Value);
                
                this.DrawRelation(table, tableControls, relation);
            }
            
            this.RedrawTableRelations(table);
        }

        public async void CreateTableObject(TableModel table)
        {
            if (!table.IsNewTable && (table.Columns == null || table.Columns.Count() == 0))
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ReverseEngineer reverseEngineer = new ReverseEngineer(this.Dispatcher);

                        EventParser.ParseMessage(this, this.Dispatcher, string.Empty, "Reading table information.");

                        table.Columns = table.Columns.AddRange(reverseEngineer.GetTableColumns(table.TableName).ToArray());

                        table.PrimaryKeyClusterConstraintName = reverseEngineer.GetTablePrimaryKeyCluster(table.TableName);
                    }
                    catch (Exception err)
                    {
                        EventParser.ParseError(this, err);
                    }
                });
            }

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

            tableControl.TableHeaderChanged += this.TableHeader_Changed;
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

                    this.AddNewRelation((DatabaseRelation) dataValue);

                    return;
                }

                TableModel table = (TableModel) dataValue;

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

                object[] dataValues = (object[]) e.Data.GetData(DataFormats.Serializable);

                if ((Type) dataValues[0] == typeof(TableObject))
                {
                    TableObject table = (TableObject) dataValues[1];

                    Point mousePos = e.GetPosition(this);

                    table.Location = mousePos;

                    table.StopDrag();

                    this.ResizeCanvas(mousePos, table.ActualHeight, table.ActualWidth);

                    this.RedrawTableRelations(table);
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
                TableObject table = (TableObject) sender;

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

        private void TableHeader_Changed(object sender)
        {
            this.CanvasChanged?.Invoke(this, sender);
        }

        private void DatabaseRelation_Delete(DatabaseRelation sender)
        {
            try
            {
                Integrity.RemoveForeighKey(sender);

                string dictionaryKey = $"{sender.ParentTable}||{sender.ChildTable}";

                UIElement tableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject) t).Table.TableName == sender.ChildTable);

                UIElement parentTableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject) t).Table.TableName == sender.ParentTable);

                if (tableControl != null)
                {
                    TableObject childTable = (TableObject) tableControl;

                    childTable.ColumnRelationModel.Remove(dictionaryKey);

                    childTable.LinkedRelations.Remove(dictionaryKey);
                }

                if (parentTableControl != null)
                {
                    TableObject childTable = (TableObject) parentTableControl;

                    childTable.ColumnRelationModel.Remove(dictionaryKey);

                    childTable.LinkedRelations.Remove(dictionaryKey);
                }

                foreach (Path relationPath in this.FindVisualControls(typeof(Path)).Where(p => ((Path) p).Name.EndsWith(sender.RelationshipName)))
                {
                    this.Children.Remove(relationPath);
                }

                this.columnRelationModel.Remove(dictionaryKey);

                this.CanvasChanged?.Invoke(this, sender);

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

            UIElement tableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject) t).Table.TableName == editor.DatabaseRelation.ChildTable);

            UIElement parentTableControl = this.FindVisualControls(typeof(TableObject)).FirstOrDefault(t => ((TableObject) t).Table.TableName == editor.DatabaseRelation.ParentTable);

            UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

            if (tableControl != null)
            {
                TableObject childTable = (TableObject) tableControl;

                string dictionaryKey = $"{editor.DatabaseRelation.ParentTable}||{editor.DatabaseRelation.ChildTable}";

                childTable.ColumnRelationModel.Add(dictionaryKey, editor.DatabaseRelation);
                
                this.columnRelationModel.Add(dictionaryKey, editor.DatabaseRelation);

                this.DrawRelation((TableObject) parentTableControl, tableControls, new KeyValuePair<string, DatabaseRelation>(dictionaryKey, editor.DatabaseRelation));
            }
        }

        private void RedrawTableRelations(TableObject table)
        {
            List<KeyValuePair<string, DatabaseRelation>> changedRelations = new List<KeyValuePair<string, DatabaseRelation>>();

            foreach (string mapedRelation in table.LinkedRelations)
            {
                if (!this.columnRelationModel.ContainsKey(mapedRelation))
                {
                    continue;
                }

                DatabaseRelation relationKeyValue = this.columnRelationModel[mapedRelation];

                foreach (Path relationPath in this.FindVisualControls(typeof(Path)).Where(p => ((Path)p).Name.EndsWith(relationKeyValue.RelationshipName)))
                {
                    this.Children.Remove(relationPath);
                }

                changedRelations.Add(new KeyValuePair<string, DatabaseRelation>(mapedRelation, relationKeyValue));
            }

            UIElement[] tableControls = this.FindVisualControls(typeof(TableObject));

            foreach (KeyValuePair<string, DatabaseRelation> relation in changedRelations)
            {
                string[] tablesUsed = relation.Key.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

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

        #region TABLE OBSTACLE ROUTING

        private sealed class RouteSearchNode
        {
            public int XIndex { get; set; }

            public int YIndex { get; set; }

            // 0 = Start, 1 = Horizontal, 2 = Vertical
            public int Direction { get; set; }

            public double G { get; set; }

            public double H { get; set; }

            public double F => G + H;

            public RouteSearchNode Parent { get; set; }

            public string Key => $"{XIndex}|{YIndex}|{Direction}";
        }

        private Point[] RouteAroundTables(Point start, Point end, TableObject parentTable, TableObject childTable, Point[] existingPoints,
            int parentLinkIndex, int childLinkIndex)
        {
            if (parentTable == childTable)
            {
                return existingPoints;
            }

            Point parentLeadOut = this.GetTableLeadOutPoint(parentTable, start, parentLinkIndex);

            Point childLeadOut = this.GetTableLeadOutPoint(childTable, end, childLinkIndex);

            // Preserve the existing crow's-foot decoration points.
            Point[] crowFootPoints = existingPoints
                .Skip(2)
                .ToArray();

            // Build the original route, but include the two lead-out points.
            List<Point> originalRoute = new List<Point>();

            originalRoute.Add(start);
            originalRoute.Add(parentLeadOut);
            originalRoute.AddRange(existingPoints.Take(2));
            originalRoute.Add(childLeadOut);
            originalRoute.Add(end);

            // If the route is clear, do NOT invoke A*.
            if (!this.RouteIntersectsObstacles(originalRoute, this.GetTableObstacles(parentTable, childTable)))
            {
                return new Point[] { parentLeadOut }
                    .Concat(existingPoints.Take(2))
                    .Concat(new Point[] { childLeadOut })
                    .Concat(crowFootPoints)
                    .ToArray();
            }

            // Only perform A* when another table obstructs the route.
            List<Rect> obstacles = this.GetTableObstacles(parentTable, childTable);

            Point[] newRoute = this.FindOrthogonalRoute(parentLeadOut, childLeadOut, obstacles);

            if (newRoute == null || newRoute.Length < 2)
            {
                return existingPoints;
            }

            return new Point[] { parentLeadOut }
                .Concat(newRoute.Skip(1).Take(newRoute.Length - 2))
                .Concat(new Point[] { childLeadOut })
                .Concat(crowFootPoints)
                .ToArray();
        }

        private List<Rect> GetTableObstacles(TableObject parentTable, TableObject childTable)
        {
            List<Rect> obstacles = new List<Rect>();

            TableObject[] tables = this
                .FindVisualControls(typeof(TableObject))
                .OfType<TableObject>()
                .ToArray();

            foreach (TableObject table in tables)
            {
                if (table == parentTable || table == childTable)
                {
                    continue;
                }

                double width = table.ActualWidth;
                double height = table.ActualHeight;

                if (width <= 0)
                {
                    width = table.DesiredSize.Width;
                }

                if (height <= 0)
                {
                    height = table.DesiredSize.Height;
                }

                if (width <= 0 || height <= 0)
                {
                    continue;
                }

                Rect obstacle = new Rect(table.Location.X - relationClearance,
                    table.Location.Y - relationClearance,
                    width + (relationClearance * 2),
                    height + (relationClearance * 2));

                obstacles.Add(obstacle);
            }

            return obstacles;
        }

        private bool RouteIntersectsObstacles(List<Point> route, List<Rect> obstacles)
        {
            for (int i = 0; i < route.Count - 1; i++)
            {
                Point start = route[i];
                Point end = route[i + 1];

                foreach (Rect obstacle in obstacles)
                {
                    if (this.SegmentIntersectsObstacle(start, end, obstacle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SegmentIntersectsObstacle(Point start, Point end, Rect obstacle)
        {
            bool horizontal = Math.Abs(start.Y - end.Y) < routeEpsilon;

            bool vertical = Math.Abs(start.X - end.X) < routeEpsilon;

            if (horizontal)
            {
                double y = start.Y;
                double left = Math.Min(start.X, end.X);
                double right = Math.Max(start.X, end.X);

                bool crossesVertically = y > obstacle.Top + routeEpsilon &&
                    y < obstacle.Bottom - routeEpsilon;

                bool crossesHorizontally = right > obstacle.Left + routeEpsilon &&
                    left < obstacle.Right - routeEpsilon;

                return crossesVertically && crossesHorizontally;
            }

            if (vertical)
            {
                double x = start.X;

                double top = Math.Min(start.Y, end.Y);
                double bottom = Math.Max(start.Y, end.Y);

                bool crossesHorizontally = x > obstacle.Left + routeEpsilon &&
                    x < obstacle.Right - routeEpsilon;

                bool crossesVertically = bottom > obstacle.Top + routeEpsilon &&
                    top < obstacle.Bottom - routeEpsilon;

                return crossesHorizontally && crossesVertically;
            }

            // The router only generates orthogonal lines.
            // Treat an unexpected diagonal as obstructed.
            return true;
        }

        private Point[] FindOrthogonalRoute(Point start, Point end, List<Rect> obstacles)
        {
            SortedSet<double> xCoordinates = new SortedSet<double>();
            SortedSet<double> yCoordinates = new SortedSet<double>();

            xCoordinates.Add(start.X);
            xCoordinates.Add(end.X);
            yCoordinates.Add(start.Y);
            yCoordinates.Add(end.Y);

            // Add the edges of every expanded obstacle.
            foreach (Rect obstacle in obstacles)
            {
                xCoordinates.Add(obstacle.Left);
                xCoordinates.Add(obstacle.Right);
                yCoordinates.Add(obstacle.Top);
                yCoordinates.Add(obstacle.Bottom);
            }

            // Add an outer routing corridor around all obstacles.
            // This gives A* an alternative when the direct route is blocked.
            double minX = Math.Min(start.X, end.X);
            double maxX = Math.Max(start.X, end.X);
            double minY = Math.Min(start.Y, end.Y);
            double maxY = Math.Max(start.Y, end.Y);

            foreach (Rect obstacle in obstacles)
            {
                minX = Math.Min(minX, obstacle.Left);
                maxX = Math.Max(maxX, obstacle.Right);
                minY = Math.Min(minY, obstacle.Top);
                maxY = Math.Max(maxY, obstacle.Bottom);
            }

            xCoordinates.Add(minX - routeBoundaryMargin);
            xCoordinates.Add(maxX + routeBoundaryMargin);
            yCoordinates.Add(minY - routeBoundaryMargin);
            yCoordinates.Add(maxY + routeBoundaryMargin);

            double[] xs = xCoordinates.ToArray();
            double[] ys = yCoordinates.ToArray();

            int startX = Array.IndexOf(xs, start.X);
            int startY = Array.IndexOf(ys, start.Y);

            int endX = Array.IndexOf(xs, end.X);
            int endY = Array.IndexOf(ys, end.Y);

            if (startX < 0 || startY < 0 ||
                endX < 0 || endY < 0)
            {
                return null;
            }

            List<RouteSearchNode> open = new List<RouteSearchNode>();

            Dictionary<string, RouteSearchNode> bestNodes = new Dictionary<string, RouteSearchNode>();

            HashSet<string> closed = new HashSet<string>();

            RouteSearchNode firstNode = new RouteSearchNode
            {
                XIndex = startX,
                YIndex = startY,
                Direction = 0,
                G = 0,
                H = Math.Abs(end.X - start.X) +
                    Math.Abs(end.Y - start.Y)
            };

            open.Add(firstNode);

            bestNodes.Add(firstNode.Key, firstNode);

            while (open.Count > 0)
            {
                // Select the node with the lowest estimated total cost.
                int bestIndex = 0;

                for (int i = 1; i < open.Count; i++)
                {
                    if (open[i].F < open[bestIndex].F)
                    {
                        bestIndex = i;
                    }
                }

                RouteSearchNode current = open[bestIndex];

                open.RemoveAt(bestIndex);

                if (closed.Contains(current.Key))
                {
                    continue;
                }

                closed.Add(current.Key);

                // Destination reached.
                if (current.XIndex == endX &&
                    current.YIndex == endY)
                {
                    return this.ReconstructRoute(current, xs, ys, start, end);
                }

                // Four possible movements:
                // Left, Right, Up and Down.
                int[,] movements =
                {
                    { -1,  0 },
                    {  1,  0 },
                    {  0, -1 },
                    {  0,  1 }
                };

                for (int i = 0; i < movements.GetLength(0); i++)
                {
                    int nextX = current.XIndex + movements[i, 0];
                    int nextY = current.YIndex + movements[i, 1];

                    if (nextX < 0 || nextX >= xs.Length ||
                        nextY < 0 || nextY >= ys.Length)
                    {
                        continue;
                    }

                    Point currentPoint = new Point(xs[current.XIndex], ys[current.YIndex]);

                    Point nextPoint = new Point(xs[nextX], ys[nextY]);

                    // Reject nodes inside obstacles.
                    if (this.IsPointInsideObstacle(nextPoint, obstacles))
                    {
                        continue;
                    }

                    // Reject segments crossing obstacles.
                    bool blocked = obstacles.Any(obstacle =>
                        this.SegmentIntersectsObstacle(
                            currentPoint,
                            nextPoint,
                            obstacle));

                    if (blocked)
                    {
                        continue;
                    }

                    int direction = movements[i, 0] != 0 ? 1 : 2;

                    double distance = Math.Abs(nextPoint.X - currentPoint.X) +
                        Math.Abs(nextPoint.Y - currentPoint.Y);

                    double turnCost = current.Direction != 0 && current.Direction != direction
                            ? relationTurnPenalty
                            : 0;

                    double tentativeG = current.G + distance + turnCost;

                    RouteSearchNode nextNode = new RouteSearchNode
                    {
                        XIndex = nextX,
                        YIndex = nextY,
                        Direction = direction,
                        G = tentativeG,
                        H = Math.Abs(end.X - nextPoint.X) +
                            Math.Abs(end.Y - nextPoint.Y),
                        Parent = current
                    };

                    if (closed.Contains(nextNode.Key))
                    {
                        continue;
                    }

                    if (bestNodes.TryGetValue(nextNode.Key, out RouteSearchNode existingNode))
                    {
                        if (tentativeG >= existingNode.G)
                        {
                            continue;
                        }

                        // Update an already discovered node.
                        existingNode.G = tentativeG;
                        existingNode.Parent = current;

                        continue;
                    }

                    bestNodes.Add(nextNode.Key, nextNode);

                    open.Add(nextNode);
                }
            }

            // No valid route was found.
            return null;
        }

        private bool IsPointInsideObstacle(Point point, List<Rect> obstacles)
        {
            return obstacles.Any(obstacle =>
                point.X > obstacle.Left + routeEpsilon &&
                point.X < obstacle.Right - routeEpsilon &&
                point.Y > obstacle.Top + routeEpsilon &&
                point.Y < obstacle.Bottom - routeEpsilon);
        }

        private Point[] ReconstructRoute(RouteSearchNode destination, double[] xs, double[] ys, Point start, Point end)
        {
            List<Point> route = new List<Point>();
            RouteSearchNode current = destination;

            while (current != null)
            {
                route.Add(new Point(xs[current.XIndex], ys[current.YIndex]));
                current = current.Parent;
            }

            route.Reverse();

            // Remove unnecessary intermediate points where the
            // route continues in the same direction.
            List<Point> simplified = new List<Point>();

            foreach (Point point in route)
            {
                simplified.Add(point);

                while (simplified.Count >= 3)
                {
                    Point a = simplified[simplified.Count - 3];
                    Point b = simplified[simplified.Count - 2];
                    Point c = simplified[simplified.Count - 1];

                    bool sameHorizontal = Math.Abs(a.Y - b.Y) < routeEpsilon 
                        && Math.Abs(b.Y - c.Y) < routeEpsilon;

                    bool sameVertical = Math.Abs(a.X - b.X) < routeEpsilon 
                        && Math.Abs(b.X - c.X) < routeEpsilon;

                    if (sameHorizontal || sameVertical)
                    {
                        simplified.RemoveAt(simplified.Count - 2);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Ensure the exact original endpoints are retained.
            simplified[0] = start;
            simplified[simplified.Count - 1] = end;

            return simplified.ToArray();
        }

        private Point GetTableLeadOutPoint(TableObject table, Point anchorPoint, int linkIndex)
        {
            double left = table.Location.X;
            double top = table.Location.Y;

            double width = table.DesiredSize.Width;
            double height = table.DesiredSize.Height;

            double right = left + width;
            double bottom = top + height;

            // Determine which table edge contains the anchor point.
            double distanceLeft = Math.Abs(anchorPoint.X - left);
            double distanceRight = Math.Abs(anchorPoint.X - right);
            double distanceTop = Math.Abs(anchorPoint.Y - top);
            double distanceBottom = Math.Abs(anchorPoint.Y - bottom);

            double shortestDistance = Math.Min(Math.Min(distanceLeft, distanceRight), Math.Min(distanceTop, distanceBottom));

            double leadOutOffset = (linkIndex * linePadding) + relationLeadOutLength;

            // Move directly away from the closest table edge.
            if (shortestDistance == distanceLeft)
            {
                return new Point(anchorPoint.X - leadOutOffset,
                    anchorPoint.Y);
            }

            if (shortestDistance == distanceRight)
            {
                return new Point(anchorPoint.X + leadOutOffset,
                    anchorPoint.Y);
            }

            if (shortestDistance == distanceTop)
            {
                return new Point( anchorPoint.X,
                    anchorPoint.Y - leadOutOffset);
            }

            return new Point(anchorPoint.X,
                anchorPoint.Y + leadOutOffset);
        }

        #endregion

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
                UIElement childItem = tableControls.FirstOrDefault(t => (((TableObject) t).Table.TableName == relationKeyValue.Value.ParentTable));

                if (childItem == null)
                {
                    this.DrawStubRelation(parentTable, relationKeyValue);

                    continue;
                }

                this.DrawRelation((TableObject) childItem, tableControls, relationKeyValue);
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

            relationKeyValue.Value.IntersectionPoints = new Point[] {selfPoint, selfStub};

            this.AddLineDrwaings(relationKeyValue, self, self, selfPoint, selfPoint);
        }

        private void DrawRelation(TableObject parentTable, UIElement[] tableControls, KeyValuePair<string, DatabaseRelation> relationKeyValue)
        {
            double xOffset = 15;

            double yOffset = 0;

            Point parentPoint = parentTable.Location;

            string[] relationTables = relationKeyValue.Key.Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);

            UIElement childItem = tableControls.FirstOrDefault(t => (((TableObject) t).Table.TableName == relationTables[1]));

            if (childItem == null)
            {
                this.DrawStubRelation(parentTable, relationKeyValue);

                return;
            }

            TableObject childTable = (TableObject) childItem;

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

            int childKeyLinkCount = childTable.Table.Columns.Where(cc => cc.ForeignKeys.Any(t => t.ForeignKeyTable == tablesArray[0]) && cc.InPrimaryKey).Count();

            int childKeyCount = childTable.Table.Columns.Where(ck => ck.InPrimaryKey).Count();

            int parentKeyLinkCount = parentTable.Table.Columns.Where(pc => pc.InPrimaryKey).Count();

            bool isStrongKey = (childKeyLinkCount == 0 && childKeyLinkCount != childKeyCount) ? false : childKeyLinkCount == parentKeyLinkCount;

            double parentBottom = parentTable.DesiredSize.Height + parentPoint.Y;

            double childBottom = childTable.DesiredSize.Height + childPoint.Y;

            double parentWidth = parentTable.DesiredSize.Width + parentPoint.X;

            double childWidth = childTable.DesiredSize.Width + childPoint.X;

            TablePlacementEnum placement = ParentPlacementCalculator.CalculatePlacement(
                new System.Drawing.Point((int)parentPoint.X, (int)parentPoint.Y),
                new System.Drawing.Point((int)childPoint.X, (int)childPoint.Y),
                parentWidth, childWidth, parentBottom, childBottom);

            var pN = parentTable.Table.TableName;
            var cN = childTable.Table.TableName;

            if (childTable.Table.TableName == parentTable.Table.TableName)
            {
                #region TABLE REFERENCE IT SELF

                parentPoint.X += parentTable.DesiredSize.Width;

                parentPoint.Y += yOffset;

                childPoint.X += parentTable.DesiredSize.Width;

                childPoint.Y += (yOffset + linePadding);

                result[0] = new Point(parentPoint.X + xOffset, parentPoint.Y);

                result[1] = new Point(childPoint.X + xOffset, childPoint.Y);

                yOffset += 30;

                #endregion
            }
            else if (placement == TablePlacementEnum.ParentAtLeft)
            {
                #region PARENT IS LEFT OF CHILD

                parentPoint.X += parentTable.DesiredSize.Width;

                double parentHeightCentre = parentPoint.Y + (parentTable.DesiredSize.Height / anchorStartOffset);

                double childHeightCentre = childPoint.Y + (childTable.DesiredSize.Height / anchorStartOffset);

                parentPoint.Y = parentHeightCentre + (parentLinkIndex * linePadding);

                childPoint.Y = childHeightCentre + (childLinkIndex * linePadding);

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

                double parentHeightCentre = parentPoint.Y + (parentTable.DesiredSize.Height / anchorStartOffset);

                double childHeightCentre = childPoint.Y + (childTable.DesiredSize.Height / anchorStartOffset);

                parentPoint.Y = parentHeightCentre + (parentLinkIndex * linePadding);

                childPoint.Y = childHeightCentre + (childLinkIndex * linePadding);

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

                double parentWidthCentre = parentPoint.X + (parentTable.DesiredSize.Width / anchorStartOffset);

                double childWidthCentre = childPoint.X + (childTable.DesiredSize.Width / anchorStartOffset);

                parentPoint.X = parentWidthCentre + (parentLinkIndex * linePadding);

                childPoint.X = childWidthCentre + (childLinkIndex * linePadding);

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

                double parentWidthCentre = parentPoint.X + (parentTable.DesiredSize.Width / anchorStartOffset);

                double childWidthCentre = childPoint.X + (childTable.DesiredSize.Width / anchorStartOffset);

                parentPoint.X = parentWidthCentre + (parentLinkIndex * linePadding);

                childPoint.X = childWidthCentre + (childLinkIndex * linePadding);

                double yOffP = (parentPoint.Y - childPoint.Y) / 2;

                result[0] = new Point(parentPoint.X, parentPoint.Y - yOffP);

                result[1] = new Point(childPoint.X, childPoint.Y + yOffP);

                if (!isStrongKey)
                {
                    result = this.AddDownCrowsFeet(result, childPoint);
                }

                #endregion
            }

            return this.RouteAroundTables(parentPoint, childPoint, parentTable, childTable, result, parentLinkIndex, childLinkIndex);
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
