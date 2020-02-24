using ERD.Build.Models;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPF.Tools.Functions;

namespace ERD.Build
{
    public class BuildClasses
    {
        private SampleScript scriptor = new SampleScript();

        private ErdCanvasModel[] canvases;

        private Dispatcher dispatcher;

        public void ScriptFiles(ErdCanvasModel[] canvasObjects, Dispatcher windowDispatcher)
        {
            this.canvases = canvasObjects;

            this.dispatcher = windowDispatcher;

            this.BuildScripts();
        }

        private async void BuildScripts()
        {
            Exception resultError = null;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    this.ScriptSingleFile();

                    this.ScriptForeachCanvas();

                    this.ScriptForeachTable();

                    this.ParseBuildMessage("Build Completed");
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

        private void ScriptSingleFile()
        {
            foreach (OptionSetupModel option in BuildScript.Setup.BuildOptions.Where(o => o.RepeatOption == BuildEnums.RepeatOptionEnum.SingleFile))
            {
                string fileName = option.OutputFileName;

                string filePath = Path.Combine(option.OutputDirectory, fileName);

                if (!Directory.Exists(option.OutputDirectory))
                {
                    Directory.CreateDirectory(option.OutputDirectory);
                }

                this.ParseBuildMessage($"Scripting: {fileName}");

                if (!option.OverrideIfExists && File.Exists(filePath))
                {
                    continue;
                }

                string result = this.scriptor.BuildSingleFile(option);

                File.WriteAllText(filePath, result);
            }
        }

        private void ScriptForeachCanvas()
        {
            List<TableModel> allTablesList = new List<TableModel>();

            foreach (ErdCanvasModel model in this.canvases)
            {
                allTablesList.AddRange(model.SegmentTables);
            }

            foreach (OptionSetupModel option in BuildScript.Setup.BuildOptions.Where(o => o.RepeatOption == BuildEnums.RepeatOptionEnum.ForeachCanvas))
            {
                this.scriptor.LanguageOption = option.LanguageOption;

                foreach (ErdCanvasModel canvas in this.canvases)
                {
                    string filePath = this.OutputFileName(option, canvas, null);

                    this.ParseBuildMessage($"Scripting: {Path.GetFileName(filePath)}");

                    if (!option.OverrideIfExists && File.Exists(filePath))
                    {
                        continue;
                    }

                    string[] havingTables = canvas.SegmentTables.Select(i => i.TableName).ToArray();

                    string[] includeTables = canvas.IncludeInContextBuild.Select(i => i.TableName).ToArray();

                    string[] selectTheseTables = includeTables.Except(havingTables).ToArray();

                    canvas.SegmentTables.AddRange(allTablesList.Where(all => selectTheseTables.Contains(all.TableName)));

                    string result = this.scriptor.BuildSampleForeachCanvasScript(canvas, this.canvases.ToList(), option);

                    File.WriteAllText(filePath, result);

                    foreach (TableModel table in allTablesList.Where(all => selectTheseTables.Contains(all.TableName)))
                    {
                        canvas.SegmentTables.Remove(table);
                    }
                }
            }
        }

        private void ScriptForeachTable()
        {
            List<TableModel> allTablesList = new List<TableModel>();

            foreach (ErdCanvasModel model in this.canvases)
            {
                allTablesList.AddRange(model.SegmentTables);
            }

            foreach (OptionSetupModel option in BuildScript.Setup.BuildOptions.Where(o => o.RepeatOption == BuildEnums.RepeatOptionEnum.ForeachTableProject))
            {
                foreach (ErdCanvasModel canvas in this.canvases)
                {
                    foreach (TableModel table in canvas.SegmentTables)
                    {
                        string filePath = this.OutputFileName(option, canvas, table);

                        this.ParseBuildMessage($"Scripting: {Path.GetFileName(filePath)}");

                        if (!option.OverrideIfExists && File.Exists(filePath))
                        {
                            continue;
                        }

                        string result = this.scriptor.BuildSampleForeachTableScript(canvas, this.canvases.ToList(), table, option);

                        File.WriteAllText(filePath, result);
                    }
                }
            }
        }

        private void ParseBuildMessage(string message)
        {
            this.dispatcher.Invoke(() => { EventParser.ParseMessage(this, new ParseMessageEventArguments {Message = message}); });
        }

        private string OutputFileName(OptionSetupModel option, ErdCanvasModel canvas, TableModel table)
        {
            if (table == null)
            {
                string canvasFileName = option.OutputFileName.Replace("[[CanvasName]]", canvas.ModelSegmentControlName);

                string canvasFilePath = option.OutputDirectory.Replace("[[CanvasName]]", canvas.ModelSegmentControlName);

                if (!Directory.Exists(canvasFilePath))
                {
                    Directory.CreateDirectory(canvasFilePath);
                }

                return Path.Combine(canvasFilePath, canvasFileName);
            }

            string fileName = option.OutputFileName
                .Replace("[[TableName]]", table.TableName)
                .Replace("[[CanvasName]]", canvas.ModelSegmentControlName);

            string filePath = option.OutputDirectory
                .Replace("[[TableName]]", table.TableName)
                .Replace("[[CanvasName]]", canvas.ModelSegmentControlName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            return Path.Combine(filePath, fileName);
        }
    }
}
