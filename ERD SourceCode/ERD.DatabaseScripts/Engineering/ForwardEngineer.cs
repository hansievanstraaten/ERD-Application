using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ViSo.Common;
using WPF.Tools.BaseClasses;

namespace ERD.DatabaseScripts.Engineering
{
  public class ForwardEngineer
  {
    public delegate void ForwardEngineeringCompletedEvent(object sender, bool completed);

    public event ForwardEngineeringCompletedEvent ForwardEngineeringCompleted;

    private BackgroundWorker buildWorker;
    
    private WindowBase progressWindow;
    
    public void BuildDatabase(TableModel[] tablesArray, bool asScriptOnly)
    {
      this.CheckConceptualValues(tablesArray);

      Type window = Type.GetType("ERD.Viewer.Tools.Common.ProgressResponce,ERD.Viewer");

      this.progressWindow = Activator.CreateInstance(window, new object[] {tablesArray.Length + 1}) as WindowBase;

      this.progressWindow.Show();

      this.buildWorker = new BackgroundWorker();

      this.buildWorker.WorkerReportsProgress = true;

      this.buildWorker.ProgressChanged += this.BildProgress_Changed;

      this.buildWorker.RunWorkerCompleted += this.BuildProgress_Completed;

      if (asScriptOnly)
      {
        this.buildWorker.DoWork += this.ScriptProgress_DoWork;
      }
      else
      {
        this.buildWorker.DoWork += this.BildProgress_DoWork;
      }
      
      this.buildWorker.RunWorkerAsync(tablesArray);
    }

    private void CheckConceptualValues(TableModel[] tablesArray)
    {
      if (!General.ProjectModel.KeepColumnsUnique)
      {
        return;
      }

      List<string> checkedList = new List<string>();

      foreach(TableModel table in tablesArray)
      {
        foreach(ColumnObjectModel column in table.Columns)
        {
          string lowerColumnName = column.ColumnName.ToLower();

          if (checkedList.Contains(lowerColumnName))
          {
            continue;
          }

          checkedList.Add(lowerColumnName);

          List<TableModel> referencedTables = tablesArray
            .Where(t => t.TableName != table.TableName
                        && t.Columns.Any(c => c.ColumnName.ToLower() == lowerColumnName))
            .ToList();

          if (referencedTables.Count == 0)
          {
            continue;
          }

          string columnDataType = column.DataType.Replace(" IDENTITY", string.Empty);

          foreach(TableModel otherTable in referencedTables)
          {
            ColumnObjectModel otherColumn = otherTable.Columns.First(f => f.ColumnName.ToLower() == lowerColumnName);

            if (otherColumn.DataType.Replace(" IDENTITY", string.Empty) != columnDataType)
            {
              string exceptionMessage = "The conceptual data values for Table {0}, Column {1} differs for the same column in Table {2}.{5}{5}" +
                                        "The data type in Table {0} is {3} and in Table {2} is {4}.";

              object[] messageParameters = new object[]
              {
                table.TableName,
                column.ColumnName,
                otherTable.TableName,
                columnDataType,
                otherColumn.DataType,
                Environment.NewLine
              };

              throw new ApplicationException(String.Format(exceptionMessage, messageParameters));
            }

          }

        }
      }
    }

    private void ScriptProgress_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        StringBuilder result = new StringBuilder();

        foreach (string contraint in Integrity.DropRelations)
        {
          result.Append(Scripting.DropForeignKey(contraint));

          result.AppendLine();
        }

        TableModel[] tablesArray = (TableModel[])e.Argument;
        
        for (int x = 0; x < tablesArray.Length; x++)
        {
          TableModel table = tablesArray[x];

          this.buildWorker.ReportProgress((x + 1), $"Executing: {table.TableName}");

          result.AppendLine(Scripting.ScriptTableCreate(table));

          result.AppendLine();
        }

        for (int x = 0; x < tablesArray.Length; x++)
        {
          TableModel table = tablesArray[x];

          this.buildWorker.ReportProgress((x + 1), $"Relations: {table.TableName}");

          string sqlQuery = Scripting.BuildForeignKey(table);

          if (sqlQuery.IsNullEmptyOrWhiteSpace())
          {
            continue;
          }

          result.AppendLine(sqlQuery);

          result.AppendLine();
        }

        string outPath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{Connections.Instance.DatabaseModel.DatabaseName}.sql");

        int outIndex = 1;

        while (File.Exists(outPath))
        {
          outPath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{Connections.Instance.DatabaseModel.DatabaseName} ({outIndex}).sql");

          ++outIndex;
        }

        File.WriteAllText(outPath, result.ToString());

        Process.Start(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads));
      }
      catch
      {
        throw;
      }
    }

    private void BildProgress_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        DataAccess data = new DataAccess(Connections.Instance.DatabaseModel);

        foreach (string contraint in Integrity.DropRelations)
        {
          data.ExecuteNonQuery(Scripting.DropForeignKey(contraint));
        }

        TableModel[] tablesArray = (TableModel[]) e.Argument;

        for(int x = 0; x < tablesArray.Length; x++)
        {
          TableModel table = tablesArray[x];

          this.buildWorker.ReportProgress((x + 1), $"Executing: {table.TableName}");
          
          data.ExecuteNonQuery(Scripting.ScriptTableCreate(table));
        }

        for(int x = 0; x < tablesArray.Length; x++)
        {
          TableModel table = tablesArray[x];

          this.buildWorker.ReportProgress((x + 1), $"Relations: {table.TableName}");

          string sqlQuery = Scripting.BuildForeignKey(table);

          if (sqlQuery.IsNullEmptyOrWhiteSpace())
          {
            continue;
          }

          data.ExecuteNonQuery(sqlQuery);
        }
      }
      catch
      {
        throw;
      }
    }

    private void BuildProgress_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Error != null)
        {
          throw e.Error;
        }
        
        this.progressWindow.Close();

        if (this.ForwardEngineeringCompleted != null)
        {
          this.ForwardEngineeringCompleted(this, true);
        }
      }
      catch (Exception err)
      {
        if (this.ForwardEngineeringCompleted != null)
        {
          this.ForwardEngineeringCompleted(this, false);
        }

        this.progressWindow.Close();

        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }

    private void BildProgress_Changed(object sender, ProgressChangedEventArgs e)
    {
      try
      {
        this.InvokeMethod(this.progressWindow, "UpdateProgress", new object[] { (double)e.ProgressPercentage });

        this.InvokeMethod(this.progressWindow, "UpdateMessage", new object[] { e.UserState });
      }
      catch
      {
        // DO NOTING
      }
    }
  }
}
