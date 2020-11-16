﻿using ERD.Base;
using ERD.Build;
using ERD.Build.Models;
using ERD.Models;
using GeneralExtensions;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
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

        private List<ErdCanvasModel> allErdCanvasModels;

        public BuildSetup(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdCanvases)
        {
            this.InitializeComponent();

            this.canvas = sampleCanvas;

            this.allErdCanvasModels = allErdCanvases;

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
                }
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

                foreach(OptionSetupModel optionModel in importSetup.BuildOptions)
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

        private void ShowAddTab()
        {
            try
            {
                if (InputBox.ShowDialog("Build Option Name", "Name").IsFalse())
                {
                    return;
                }

                BuildOption option = new BuildOption(this.canvas, this.allErdCanvasModels) {Title = InputBox.Result, ShowCloseButton = true};

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
            BuildOption option = new BuildOption(this.canvas, this.allErdCanvasModels)
            {
                Title = optionModel.OptionModelName,
                OptionSetup = optionModel,
                ShowCloseButton = true
            };

            this.uxTabs.Items.Add(option);

            this.uxTabs.Content.MaxHeight = this.ActualHeight - 65;
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

            foreach (BuildOption option in this.uxTabs.Items)
            {
                BuildScript.Setup.BuildOptions.Add(option.OptionSetup);
            }
        }

	}
}
