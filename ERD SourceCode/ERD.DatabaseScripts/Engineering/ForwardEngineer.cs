using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
      Type window = Type.GetType("ERD.Viewer.Tools.Common.ProgressResponce,ERD.Viewer");

      this.progressWindow = Activator.CreateInstance(window, new object[] {tablesArray.Length + 1}) as WindowBase;
        //new ProgressResponce(tablesArray.Length + 1);

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

        string outPath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{Connections.DatabaseModel.DatabaseName}.sql");

        int outIndex = 1;

        while (File.Exists(outPath))
        {
          outPath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{Connections.DatabaseModel.DatabaseName} ({outIndex}).sql");

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
        DataAccess data = new DataAccess(Connections.DatabaseModel);

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
