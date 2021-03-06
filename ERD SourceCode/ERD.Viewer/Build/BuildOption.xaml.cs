﻿using ERD.Build;
using ERD.Build.BuildEnums;
using ERD.Build.Models;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private SampleScript scripting = new SampleScript();

        private OptionSetupModel optionSetup;

        private ErdCanvasModel canvas;

        private List<ErdCanvasModel> allErdCanvasModels;

        public BuildOption(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdCanvases)
        {
            this.InitializeComponent();

            this.canvas = sampleCanvas;

            this.allErdCanvasModels = allErdCanvases;

            this.Loaded += this.BuildOption_Loaded;
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
            if (base.WasFirstLoaded)
            {
                return;
            }

            try
            {
                if (this.OptionSetup == null)
                {
                    this.OptionSetup = new OptionSetupModel();

                    this.OptionSetup.OptionModelName = this.Title;
                }

                this.uxOptionSetup.Items.Add(this.OptionSetup);

                this.optionSetup.PropertyChanged += this.OptionSetup_Changed;

                this.SetOptionSetup();

                foreach (BuildTypeModel buildType in this.OptionSetup.BuildTypes)
                {
                    buildType.PropertyChanged += this.BuildType_Changed;

                    this.uxBuildTypes.Items.Add(buildType);

                    this.uxBuildTypes[buildType.BuildTypeIndex].Header = $"{buildType.BuildTypeName} = [[{buildType.BuildTypeIndex}]]";
                }

                base.WasFirstLoaded = true;
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

                buildType.PropertyChanged += this.BuildType_Changed;

                this.OptionSetup.BuildTypes = this.OptionSetup.BuildTypes.Add(buildType);

                this.uxBuildTypes.Items.Add(buildType);

                this.uxBuildTypes[buildType.BuildTypeIndex].Header = $"{InputBox.Result} = [[{buildType.BuildTypeIndex}]]";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void BuildType_Changed(object sender, PropertyChangedEventArgs e)
        {
            this.scripting.LanguageOption = this.OptionSetup.LanguageOption;

            try
            {
                switch (this.OptionSetup.RepeatOption)
                {
                    case RepeatOptionEnum.ForeachCanvas:

                        this.uxSampleScript.Text = this.scripting.BuildSampleForeachCanvasScript(this.canvas, this.allErdCanvasModels, this.OptionSetup);

                        break;

                    case RepeatOptionEnum.ForeachTableProject:

                        this.uxSampleScript.Text = this.scripting.BuildSampleForeachTableScript(this.canvas, this.allErdCanvasModels, this.canvas.SegmentTables[0], this.OptionSetup);

                        break;

                    case RepeatOptionEnum.SingleFile:

                        this.uxSampleScript.Text = this.scripting.BuildSingleFile(this.OptionSetup);

                        break;

                }

            }
            catch (Exception err)
            {
                // DO NOTHING
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

        private void BuildTypes_Browse(object sender, string buttonKey)
        {
            try
            {
                ModelViewObject viewObject = (ModelViewObject) sender;

                TextEditor editor = new TextEditor("Code", viewObject["Code"].GetValue().ParseToString());

                if (editor.ShowDialog().IsFalse())
                {
                    return;
                }

                viewObject["Code"].SetValue(editor.Text);

                viewObject["Code"].Focus();
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
    }
}
