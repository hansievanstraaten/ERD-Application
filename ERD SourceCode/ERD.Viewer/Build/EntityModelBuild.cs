using ERD.Common;
using ERD.Models;
using ERD.Models.BuildModels.EntityFrameworkModels;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ViSo.Common;
using WPF.Tools.Functions;

namespace ERD.Viewer.Build
{
  internal class EntityModelBuild
  {
    private EntityFrameworkSetup tempSetup;

    internal void SetupModelBrowse(object sender, string buttonKey)
    {
      try
      {
        bool browseResult = false;

        switch (buttonKey)
        {
          case "ServerOutputFolder":

            string serverFolder = this.FolderBrowse(out browseResult);

            if (browseResult)
            {
              if (EntityModelScript.Setup == null)
              {
                this.tempSetup.ServerOutputBaseDirectory = serverFolder;
              }
              else
              {
                EntityModelScript.Setup.ServerOutputBaseDirectory = serverFolder;
              }
            }

            break;

          case "ClientOutputFolder":

            string clientFolder = this.FolderBrowse(out browseResult);

            if (browseResult)
            {
              if (EntityModelScript.Setup == null)
              {
                this.tempSetup.ClientModelOutputDirectory = clientFolder;
              }
              else
              {
                EntityModelScript.Setup.ClientModelOutputDirectory = clientFolder;
              }

            }

            break;

            case "DataContextUsings":
              EntityModelScript.Setup.DataContextUsing = this.ShowTextEditor("Data Context", EntityModelScript.Setup.DataContextUsing);

            break;

          case "MappingClassUsing":

            EntityModelScript.Setup.MappingClassUsing = this.ShowTextEditor("Model Class", EntityModelScript.Setup.MappingClassUsing);

            break;

          case "ModelClassBaseUsing":

            EntityModelScript.Setup.ModelClassBaseUsing = this.ShowTextEditor("Model Class", EntityModelScript.Setup.ModelClassBaseUsing);

            break;

          case "ModelClassBaseString":

            EntityModelScript.Setup.ModelClassBaseString = this.ShowTextEditor("Model Class", EntityModelScript.Setup.ModelClassBaseString);

            break;

          case "ModelClassString":

            EntityModelScript.Setup.ModelClassString = this.ShowTextEditor("Model Class", EntityModelScript.Setup.ModelClassString);

            break;

          case "ModelPropertyString":

            EntityModelScript.Setup.ModelPropertyString = this.ShowTextEditor("Model Property", EntityModelScript.Setup.ModelPropertyString);

            break;

          case "RepositoryUsing":

            EntityModelScript.Setup.RepositoryUsing = this.ShowTextEditor("Repository Using", EntityModelScript.Setup.RepositoryUsing);

            break;
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
    }

    internal void BuildEntityModel(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      if (EntityModelScript.Setup == null)
      {
        this.tempSetup = new EntityFrameworkSetup();

        ObjectViewer viewer = new ObjectViewer("EF Setup", this.tempSetup);

        viewer.ModelViewItemBrowse += this.SetupModel_Browse;

        bool? result = viewer.ShowDialog();

        if (!result.HasValue || !result.Value)
        {
          return;
        }

        EntityModelScript.Setup = viewer.ModelObject.To<EntityFrameworkSetup>();
      }

      this.RunScriptBuilderAsync(canvases, dispatcher);
    }

    private void SetupModel_Browse(object sender, string buttonKey)
    {
      EventParser.ParseMessage(sender, new ParseMessageEventArguments {Title = "SetupModel_Browse", Arguments = new object[] {buttonKey}});
    }

    private async void RunScriptBuilderAsync(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      try
      {
        Exception resultError = null;

        await Task.Factory.StartNew(() =>
        {
          try
          {
            this.RunMappingScriptor(canvases, dispatcher);

            this.RunContextScriptor(canvases, dispatcher);

            this.BuildRepositoriesFiles(canvases, dispatcher);

            this.BuildModelFiles(canvases, dispatcher);

            dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments { Message = "Build Completed" }); });
          }
          catch (Exception err)
          {
            resultError = err;
          }
        });

        if (resultError != null)
        {
          MessageBox.Show(resultError.InnerExceptionMessage());
        }
      }
      catch
      {
        throw;
      }
    }

    private void RunMappingScriptor(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      try
      {
        string dispatchNote = "Build Mappings Script for: {0}";

        Paths.CreateDirectory(Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "Mappings"));
        
        foreach (ErdCanvasModel canvas in canvases)
        {
          foreach (TableModel table in canvas.SegmentTables)
          {
            dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments {Message = string.Format(dispatchNote, table.TableName)}); });

            string mapptingPath = Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "Mappings", $"{EntityModelScript.GetClassName(table)}Mapping.cs");

            string mappingResult = EntityModelScript.ScriptServerModelMapping(table);

            File.WriteAllText(mapptingPath, mappingResult);
          }
        }

      }
      catch (Exception err)
      {
        throw;
      }
    }

    private void RunContextScriptor(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      try
      {
        if (EntityModelScript.Setup.BuildContextForeachCanvas)
        {
          string dispatchNote = "Build Data Context File for: {0}";

          foreach (ErdCanvasModel canvas in canvases)
          {
            dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments { Message = string.Format(dispatchNote, canvas.ModelSegmentName) }); });
            
            string fileName = canvas.ModelSegmentName.MakeAlphaNumeric();

            this.BuildContextFile(fileName, canvas.SegmentTables.OrderBy(n => n.TableName).ToArray(), canvas.IncludeInContextBuild.ToArray());
          }
        }
        else
        {
          dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments { Message = "DataContext" }); });

          List<TableModel> tablesUsed = new List<TableModel>();

          foreach (List<TableModel> canvasTables in canvases.Select(t => t.SegmentTables))
          {
            tablesUsed.AddRange(canvasTables.ToArray());
          }

          this.BuildContextFile("DataContext", tablesUsed.OrderBy(n => n.TableName).ToArray(), new IncludeTableModel[] {});
        }
      }
      catch (Exception err)
      {
        throw;
      }
    }

    private void BuildContextFile(string fileName, TableModel[] tablesUsed, IncludeTableModel[] includeTables)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(EntityModelScript.Setup.DataContextUsing);

      result.AppendLine();

      result.AppendLine($"namespace {EntityModelScript.Setup.DataContextNamespace}");

      result.AppendLine("{");

      result.AppendLine($"    public class {fileName} : DbContext");
      result.AppendLine("    {");

      string getset = "{ get; set; }";

      List<string> tablesUsedList = new List<string>();

      foreach (TableModel table in tablesUsed)
      {
        string nameToUse = EntityModelScript.Setup.UseFriendlyNames ? table.FriendlyName.MakeAlphaNumeric() : table.TableName;

        result.AppendLine($"        public DbSet<{EntityModelScript.GetClassName(table)}> {nameToUse} {getset}");

        tablesUsedList.Add(table.TableName);
      }

      foreach (IncludeTableModel included in includeTables)
      {
        if (tablesUsedList.Contains(included.TableName))
        {
          continue;
        }

        string nameToUse = EntityModelScript.Setup.UseFriendlyNames ? included.FriendlyName.MakeAlphaNumeric() : included.TableName;

        result.AppendLine($"        public DbSet<{EntityModelScript.GetClassName(included)}> {nameToUse} {getset}");
      }

      result.AppendLine();
      result.AppendLine($"        {string.Format(EntityModelScript.Setup.DataContextConstructor, fileName)}");
      result.AppendLine("        {");
      result.AppendLine("        }");
      result.AppendLine();

      result.AppendLine("        protected override void OnModelCreating(DbModelBuilder modelBuilder)");
      result.AppendLine("        {");

      foreach (TableModel table in tablesUsed)
      {
        result.AppendLine($"            modelBuilder.Configurations.Add(new {EntityModelScript.GetClassName(table)}Mapping());");
      }

      foreach (IncludeTableModel included in includeTables)
      {
        if (tablesUsedList.Contains(included.TableName))
        {
          continue;
        }

        result.AppendLine($"            modelBuilder.Configurations.Add(new {EntityModelScript.GetClassName(included)}Mapping());");
      }

      result.AppendLine("        }");

      result.AppendLine("    }");
      result.AppendLine("}");
      
      Paths.CreateDirectory(Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "DataContext"));

      string outputPath = Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "DataContext", $"{fileName}.cs");

      File.WriteAllText(outputPath, result.ToString());
    }

    private void BuildRepositoriesFiles(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      string basePath = Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "Repositories", "Base");

      string userPath = Path.Combine(EntityModelScript.Setup.ServerOutputBaseDirectory, "Repositories", "User");

      Paths.CreateDirectory(basePath);

      Paths.CreateDirectory(userPath);
        
      dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments { Message = "Building Reposirories" }); });

      if (EntityModelScript.Setup.BuildContextForeachCanvas)
      {
        foreach (ErdCanvasModel canvas in canvases)
        {
          string fileName = canvas.ModelSegmentName.MakeAlphaNumeric();

          this.BuildRepositoryBase(fileName, fileName, basePath, canvas.SegmentTables.ToArray(), canvas.IncludeInContextBuild.ToArray());

          this.BuildRepository(fileName, userPath);
        }

      }
      else
      {
        List<TableModel> tablesUsed = new List<TableModel>();

        foreach (List<TableModel> canvasTables in canvases.Select(t => t.SegmentTables))
        {
          tablesUsed.AddRange(canvasTables.ToArray());
        }

        this.BuildRepositoryBase("Context", "DataContext", basePath, tablesUsed.ToArray(), new IncludeTableModel[] {});

        this.BuildRepository("Context", userPath);
      }
    }

    private void BuildRepositoryBase(string repositoryName, string dataContextName, string basePath, TableModel[] tables, IncludeTableModel[] includeTables)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(EntityModelScript.Setup.RepositoryUsing);

      result.AppendLine();

      result.AppendLine($"namespace {EntityModelScript.Setup.RepositoryNamespace}");
      result.AppendLine("{");

      result.AppendLine($"    public abstract class {repositoryName}Repository_Base");
      result.AppendLine("    {");
      result.AppendLine($"        public {dataContextName} dataContext;");

      result.AppendLine();
      result.AppendLine($"        public {repositoryName}Repository_Base()");
      result.AppendLine("        {");
      result.AppendLine($"            this.dataContext = new {dataContextName}();");
      result.AppendLine("        }");

      foreach (TableModel table in tables)
      {
        string tableClassName = EntityModelScript.GetClassName(table);

        StringBuilder argumentsString = new StringBuilder();

        StringBuilder linqString = new StringBuilder();

        foreach (ColumnObjectModel keyColumn in table.Columns.Where(pk => pk.InPrimaryKey))
        {
          string[] columnValues = EntityModelScript.GetColumnDotNetDescriptor(keyColumn);

          argumentsString.Append($"{columnValues[0]} {columnValues[1]}, ");

          linqString.Append($"pk.{columnValues[1]} == {columnValues[1]} && ");
        }

        if (linqString.Length > 4 && argumentsString.Length > 2)
        {
          result.AppendLine();
          result.AppendLine($"        public {tableClassName} Get{tableClassName}({(argumentsString.ParseToString().Substring(0, (argumentsString.Length - 2)))})");
          result.AppendLine("        {");
          result.AppendLine($"            return this.dataContext.{tableClassName}.FirstOrDefault(pk => {linqString.ParseToString().Substring(0, (linqString.Length - 4))});");
          result.AppendLine("        }");
        }
      }

      foreach (IncludeTableModel table in includeTables)
      {
        string tableClassName = EntityModelScript.GetClassName(table);

        StringBuilder argumentsString = new StringBuilder();

        StringBuilder linqString = new StringBuilder();

        List<ColumnObjectModel> tableColumns = Integrity.GetObjectModel(table.TableName);

        if (tableColumns == null)
        {
          continue;
        }

        foreach (ColumnObjectModel keyColumn in tableColumns.Where(pk => pk.InPrimaryKey))
        {
          string[] columnValues = EntityModelScript.GetColumnDotNetDescriptor(keyColumn);

          argumentsString.Append($"{columnValues[0]} {columnValues[1]}, ");

          linqString.Append($"pk.{columnValues[1]} == {columnValues[1]} && ");
        }

        if (linqString.Length > 4 && argumentsString.Length > 2)
        {
          result.AppendLine();
          result.AppendLine($"        public {tableClassName} Get{tableClassName}({(argumentsString.ParseToString().Substring(0, (argumentsString.Length - 2)))})");
          result.AppendLine("        {");
          result.AppendLine($"            return this.dataContext.{tableClassName}.FirstOrDefault(pk => {linqString.ParseToString().Substring(0, (linqString.Length - 4))});");
          result.AppendLine("        }");
        }

      }

      result.AppendLine("    }");
      result.AppendLine("}");

      File.WriteAllText(Path.Combine(basePath, $"{repositoryName}Repository_Base.cs"), result.ToString());
    }

    private void BuildRepository(string repositoryName, string userPath)
    {
      string userFilePath = Path.Combine(userPath, $"{repositoryName}Repository.cs");

      if (File.Exists(userFilePath))
      {
        return;
      }

      StringBuilder result = new StringBuilder();
      
      result.AppendLine($"namespace {EntityModelScript.Setup.RepositoryNamespace}");
      result.AppendLine("{");

      result.AppendLine($"    public class {repositoryName}Repository : {repositoryName}Repository_Base");
      result.AppendLine("    {");
      result.AppendLine();
      result.AppendLine($"        public {repositoryName}Repository()");
      result.AppendLine("        {");
      result.AppendLine("        }");

      result.AppendLine("    }");
      result.AppendLine("}");

      File.WriteAllText(userFilePath, result.ToString());
    }

    private void BuildModelFiles(ErdCanvasModel[] canvases, Dispatcher dispatcher)
    {
      string basePath = Path.Combine(EntityModelScript.Setup.ClientModelOutputDirectory, "Base");

      string userPath = Path.Combine(EntityModelScript.Setup.ClientModelOutputDirectory, "User");

      Paths.CreateDirectory(basePath);

      Paths.CreateDirectory(userPath);

      foreach (ErdCanvasModel canvas in canvases)
      {
        foreach (TableModel table in canvas.SegmentTables)
        {
          dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments { Message = string.Format("Building Model {0}", table.TableName) }); });

          string modelBase = EntityModelScript.ScriptServerModelBase(table);

          string className = EntityModelScript.GetClassName(table);

          File.WriteAllText(Path.Combine(basePath, $"{className}_Base.cs"), modelBase);

          string userFileName = Path.Combine(userPath, $"{className}.cs");

          if (!File.Exists(userFileName))
          {
            string model = EntityModelScript.ScriptServerModel(table);

            File.WriteAllText(userFileName, model);
            
          }
        }
      }
    }

    private string FolderBrowse(out bool dialogResult)
    {
      FolderBrowserDialog folder = new FolderBrowserDialog();

      if (folder.ShowDialog() != DialogResult.OK)
      {
        dialogResult = false;

        return string.Empty;
      }

      dialogResult = true;

      return folder.SelectedPath;
    }

    private string ShowTextEditor(string title, string originalText)
    {
      TextEditor editor = new TextEditor(title, originalText);

      bool? result = editor.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        return originalText;
      }

      return editor.Text;
    }

  }
}
