using ERD.Build;
using ERD.Build.BuildEnums;
using ERD.Build.Models;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ViSo.Dialogs.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;

namespace ERD.Viewer.Build
{
	/// <summary>
	/// Interaction logic for BuildOption.xaml
	/// </summary>
	public partial class BuildOption : UserControlBase
    {
        private bool optionSetupAdded;
        private string lastLoadTableName = string.Empty;
        private TableModel selectedTable;
        private SampleScript scripting = new SampleScript();
        private OptionSetupModel optionSetup;
        private List<ErdCanvasModel> allErdCanvasModels;
        private CancellationTokenSource sampleScriptLoadCancellation;

        public BuildOption(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdCanvases)
        {
            InitializeComponent();

            this.SelectedCanvas = sampleCanvas;

            this.SelectedTable = this.SelectedCanvas.SegmentTables[0];

            this.allErdCanvasModels = allErdCanvases;

            this.Loaded += this.BuildOption_Loaded;
        }

        public ErdCanvasModel SelectedCanvas { get; set; }

        public TableModel SelectedTable 
        { 
            get
			{
                return this.selectedTable;
			}
            
            set
			{
                string currentTableName = this.selectedTable?.TableName;

                this.selectedTable = value;


                if (this.IsLoaded && this.selectedTable?.TableName != currentTableName)
				{
                    this.QueueLoadSampleScript();
				}
			}
        }

        public OptionSetupModel OptionSetup
        {
            get
            {
                return this.optionSetup;
            }

            set
            {
                this.optionSetup = value;
            }
        }

        private void BuildOption_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (base.WasFirstLoaded)
                {

                    if (this.SelectedTable?.TableName != this.lastLoadTableName)
					{
                        this.QueueLoadSampleScript();
					}

                    return;
                }

                if (this.OptionSetup == null)
                {
                    this.OptionSetup = new OptionSetupModel();

                    this.OptionSetup.OptionModelName = this.Title;
                }

                if (!this.optionSetupAdded)
                {
                    this.uxOptionSetup.Items.Add(this.OptionSetup);
                    this.optionSetupAdded = true;
                }

                this.optionSetup.PropertyChanged -= this.OptionSetup_Changed;
                this.optionSetup.PropertyChanged += this.OptionSetup_Changed;

                this.SetOptionSetup();

                foreach (BuildTypeModel buildType in this.OptionSetup.BuildTypes)
                {
                    BuildTypeControl control = new BuildTypeControl(buildType);

                    this.uxBuildTypes.Children.Add(control);

                    buildType.PropertyChanged += this.BuildType_Changed;
                }

                this.QueueLoadSampleScript(0);

                base.WasFirstLoaded = true;

                this.Loaded -= this.BuildOption_Loaded;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void OptionSetup_Changed(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case "RepeatOption":

                        this.SetOptionSetup();

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void AddBuildType_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (InputBox.ShowDialog("Build Type", "Name").IsFalse())
                {
                    return;
                }

                BuildTypeModel buildType = new BuildTypeModel
                {
                    BuildTypeName = InputBox.Result,
                    BuildTypeIndex = this.OptionSetup.BuildTypes.Length
                };

                this.OptionSetup.BuildTypes = this.OptionSetup.BuildTypes.Add(buildType);

                BuildTypeControl control = new BuildTypeControl(buildType);

                this.uxBuildTypes.Children.Add(control);

                buildType.PropertyChanged += this.BuildType_Changed;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void BuildType_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.QueueLoadSampleScript();
            }
        }

        private void OptionSetup_Browse(object sender, string buttonKey)
        {
            try
            {
                switch (buttonKey)
                {
                    case "OurputDirectoryKey":

                        System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

                        if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }

                        this.OptionSetup.OutputDirectory = folder.SelectedPath;

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SetOptionSetup()
        {
            switch (this.OptionSetup.RepeatOption)
            {
                case RepeatOptionEnum.ForeachCanvas:

                    this.uxOptionSetup[0, 0].Caption = "File Name E.g. '[[CanvasName]].cs'";

                    goto default;

                case RepeatOptionEnum.ForeachTableProject:

                    this.uxOptionSetup[0, 0].Caption = "File Name E.g. '[[TableName]].cs'";

                    goto default;

                case RepeatOptionEnum.SingleFile:

                    this.uxOptionSetup[0, 0].Caption = "File Name MyFile.cs'";

                    if (this.OptionSetup.BuildTypes.HasElements() && this.OptionSetup.BuildTypes.Length >= 1)
                    {
                        this.uxAddBuildType.IsEnabled = false;
                    }

                    break;

                default:

                    this.uxAddBuildType.IsEnabled = true;

                    break;
            }
        }
    
        private void QueueLoadSampleScript(int delayMilliseconds = 250)
        {
            try
            {
                this.sampleScriptLoadCancellation?.Cancel();
                this.sampleScriptLoadCancellation?.Dispose();

                this.sampleScriptLoadCancellation = new CancellationTokenSource();

                _ = this.LoadSampleScriptAsync(delayMilliseconds, this.sampleScriptLoadCancellation.Token);
            }
            catch
            {
                // DO NOTHING
            }
        }

        private async Task LoadSampleScriptAsync(int delayMilliseconds, CancellationToken cancellationToken)
        {
            try
            {
                if (delayMilliseconds > 0)
                {
                    await Task.Delay(delayMilliseconds, cancellationToken);
                }

                await Task.Run(() => this.LoadSampleScript(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // DO NOTHING
            }
        }

        private void LoadSampleScript()
		{
            try
            {
                if (this.OptionSetup == null || this.SelectedTable == null)
                {
                    return;
                }

                this.scripting.LanguageOption = this.OptionSetup.LanguageOption;

                this.lastLoadTableName = this.SelectedTable.TableName;

                string result = string.Empty;

                switch (this.OptionSetup.RepeatOption)
                {
                    case RepeatOptionEnum.ForeachCanvas:

                        result = this.scripting.BuildSampleForeachCanvasScript(this.SelectedCanvas, this.allErdCanvasModels, this.OptionSetup);

                        break;

                    case RepeatOptionEnum.ForeachTableProject:

                        result = this.scripting.BuildSampleForeachTableScript(this.SelectedCanvas, this.allErdCanvasModels, this.SelectedTable, this.OptionSetup);

                        break;

                    case RepeatOptionEnum.SingleFile:

                        result = this.scripting.BuildSingleFile(this.OptionSetup);

                        break;

                }

                Dispatcher.BeginInvoke(new Action(() => this.uxSampleScript.Text = result));

            }
            catch (Exception err)
            {
                // DO NOTHING
            }
        }
    }
}
