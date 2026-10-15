using ERD.Base;
using ERD.Build;
using ERD.Build.Models;
using ERD.Models;
using GeneralExtensions;
using Microsoft.FSharp.Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ViSo.Dialogs.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.ToolModels;

namespace ERD.Viewer.Build
{
    /// <summary>
    /// Interaction logic for BuildSetup.xaml
    /// </summary>
    public partial class BuildSetup : WindowBase
    {
        private ErdCanvasModel canvas;

        private TableModel selectedTable;

        private List<ErdCanvasModel> allErdCanvasModels;

        private sealed class BuildOptionPlaceholder : UserControlBase
        {
            public ErdCanvasModel SampleCanvas { get; set; }

            public List<ErdCanvasModel> AllErdCanvases { get; set; }

            public OptionSetupModel OptionSetup { get; set; }

            public TableModel SelectedTable { get; set; }
        }

        public BuildSetup(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdCanvases)
        {
            InitializeComponent();

            this.canvas = sampleCanvas;

            this.allErdCanvasModels = allErdCanvases;

            this.SetSampleTableOptions();

            this.LoadBuildParamaters();

            this.SizeChanged += this.BuildSetup_SizeChanged;

            this.Loaded += this.BuildSetup_Loaded;
        }

        private void BuildSetup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BuildScript.Setup == null)
                {
                    BuildScript.Setup = new BuildSetupModel();

                    this.ShowAddTab();
                }
                else
                {
                    this.LoadTabOptions();

                    this.uxTabs.SetActive(0);

                    // Ensure the first tab's heavy UI is created so the UI doesn't look empty
                    if (this.uxTabs.Items.Count > 0)
                    {
                        EnsureBuildOptionAtIndex(this.uxTabs.SelectedIndex >= 0 ? this.uxTabs.SelectedIndex : 0);
                    }
                }

                // Lazy-load tabs' heavy UI when selected
                this.uxTabs.OnTabSelected += this.UxTabs_OnTabSelected;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void BuildSetup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (this.uxTabs.Content == null)
                {
                    return;
                }

                this.uxTabs.Content.MaxHeight = e.NewSize.Height - 65;
            }
            catch
            {
                // DO NOTHING
            }
        }

        private void BuildSetup_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SetBuildOptions();
            }
            catch (Exception err)
            {
                e.Cancel = true;

                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SetBuildOptions();

                BuildScript.Save();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void TabAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowAddTab();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = $"(*.{FileTypes.estp})|*.{FileTypes.estp}";

                bool? result = dlg.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                string buildFile = File.ReadAllText(dlg.FileName);

                BuildSetupModel importSetup = JsonConvert.DeserializeObject(buildFile, typeof(BuildSetupModel)) as BuildSetupModel;

                int activeTab = this.uxTabs.SelectedIndex;

                foreach (OptionSetupModel optionModel in importSetup.BuildOptions)
                {
                    this.SetTab(optionModel);
                }

                this.uxTabs.SetActive(activeTab);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SampleTable_Changed(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                BuildTableOptonsModel optionModel = sender.To<BuildTableOptonsModel>();

                DataItemModel dataModel = optionModel.TablesSource.FirstOrDefault(tn => tn.ItemKey.ParseToString() == optionModel.TableName);

                ErdCanvasModel canvas = this.allErdCanvasModels.FirstOrDefault(c => c.ModelSegmentControlName == dataModel.Tag.ParseToString());

                this.selectedTable = canvas.SegmentTables.FirstOrDefault(t => t.TableName == optionModel.TableName);

                // Apply selection to both already-created BuildOption controls and placeholders
                foreach (var item in this.uxTabs.Items)
                {
                    if (item is BuildOption bo)
                    {
                        bo.SelectedTable = this.selectedTable;
                    }
                    else if (item is BuildOptionPlaceholder ph)
                    {
                        ph.SelectedTable = this.selectedTable;

                        if (ph.Content is BuildOption option)
                        {
                            option.SelectedTable = this.selectedTable;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected tab item type: {item.GetType().FullName}. Expected BuildOption or BuildOptionPlaceholder.");
                    }
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ShowAddTab()
        {
            try
            {
                if (InputBox.ShowDialog("Build Option Name", "Name").IsFalse())
                {
                    return;
                }

                BuildOption option = new BuildOption(this.canvas, this.allErdCanvasModels)
                {
                    Title = InputBox.Result,
                    OptionSetup = new OptionSetupModel { OptionModelName = InputBox.Result },
                    ShowCloseButton = true,
                    SelectedTable = this.selectedTable
                };

                this.uxTabs.Items.Add(option);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void LoadTabOptions()
        {
            try
            {
                foreach (OptionSetupModel optionModel in BuildScript.Setup.BuildOptions)
                {
                    this.SetTab(optionModel);
                }

                this.uxTabs.SetActive(0);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SetTab(OptionSetupModel optionModel)
        {
            BuildOptionPlaceholder placeholder = new BuildOptionPlaceholder
            {
                Title = optionModel.OptionModelName,
                OptionSetup = optionModel,
                ShowCloseButton = true,
                SampleCanvas = this.canvas,
                AllErdCanvases = this.allErdCanvasModels,
                SelectedTable = this.selectedTable
            };

            this.uxTabs.Items.Add(placeholder);

            if (this.uxTabs.Content != null)
            { 
                this.uxTabs.Content.MaxHeight = this.ActualHeight - 65;
            }
        }

        private void LoadBuildParamaters()
        {
            ResourceOption resourceOps = new ResourceOption();

            foreach (DataItemModel arg in resourceOps.ScriptParameterOptions())
            {
                TextBoxItem item = new TextBoxItem
                {
                    Text = arg.ItemKey.ToString(),
                    ToolTip = arg.DisplayValue,
                    IsReadOnly = true,
                    BorderThickness = new Thickness(0)
                };

                this.uxParametersList.Children.Add(item);
            }
        }

        private void SetBuildOptions()
        {
            BuildScript.Setup.BuildOptions.Clear();

            foreach (var item in this.uxTabs.Items)
            {
                if (item is BuildOption bo)
                {
                    BuildScript.Setup.BuildOptions.Add(bo.OptionSetup);
                }
                else if (item is BuildOptionPlaceholder ph)
                {
                    if (ph.Content is BuildOption option)
                    {
                        BuildScript.Setup.BuildOptions.Add(option.OptionSetup);
                    }
                    else
                    {
                        BuildScript.Setup.BuildOptions.Add(ph.OptionSetup);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected tab item type: {item.GetType().FullName}. Expected BuildOption or BuildOptionPlaceholder.");                    
                }
            }
        }

        private void SetSampleTableOptions()
        {
            BuildTableOptonsModel sampleTables = new BuildTableOptonsModel();

            List<DataItemModel> result = new List<DataItemModel>();

            foreach (ErdCanvasModel canvas in this.allErdCanvasModels)
            {
                result.AddRange(canvas.SegmentTables
                    .Select(t => new DataItemModel { DisplayValue = t.TableName, ItemKey = t.TableName, Tag = canvas.ModelSegmentControlName }));
            }

            sampleTables.TablesSource = result.ToArray();

            this.uxSampleTables.Items.Add(sampleTables);

            this.selectedTable = this.allErdCanvasModels[0].SegmentTables[0];

            sampleTables.TableName = this.selectedTable.TableName;

            sampleTables.PropertyChanged += this.SampleTable_Changed;
        }

        /// <summary>
        /// Ensure the tab at the provided index contains a real BuildOption control.
        /// If a placeholder exists it will be replaced with the constructed BuildOption.
        /// This method must run on the UI thread.
        /// </summary>
        private void EnsureBuildOptionAtIndex(int index)
        {
            if (index < 0 || index >= this.uxTabs.Items.Count)
            {
                return;
            }

            var item = this.uxTabs.Items[index];

            if (item is BuildOption)
            {
                return; // already created
            }

            if (item is BuildOptionPlaceholder ph)
            {
                try
                {
                    if (ph.Content is BuildOption)
                    {
                        return;
                    }

                    BuildOption option = new BuildOption(ph.SampleCanvas ?? this.canvas, ph.AllErdCanvases ?? this.allErdCanvasModels)
                    {
                        Title = ph.Title,
                        OptionSetup = ph.OptionSetup,
                        ShowCloseButton = ph.ShowCloseButton,
                        SelectedTable = ph.SelectedTable ?? this.selectedTable
                    };

                    ph.Content = option;

                    if (this.uxTabs.SelectedIndex == index)
                    {
                        this.uxTabs.Content = ph;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.InnerExceptionMessage());
                }
            }
        }

        private void UxTabs_OnTabSelected(object sender, int itemIndex)
        {
            try
            {
                if (itemIndex >= 0)
                {
                    this.EnsureBuildOptionAtIndex(itemIndex);
                }

                //int sel = this.uxTabs.SelectedIndex;

                //if (sel >= 0)
                //{
                //    // Create the tab content lazily; keep this quick on the UI thread.
                //    EnsureBuildOptionAtIndex(sel);
                //}
            }
            catch (Exception)
            {
                // swallow selection exceptions to keep UI responsive
            }
        }
    }
}
