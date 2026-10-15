using ERD.Build.BuildEnums;
using ERD.Build.Models;
using GeneralExtensions;
using System;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;

namespace ERD.Viewer.Build
{
    /// <summary>
    /// Interaction logic for BuildTypeControl.xaml
    /// </summary>
    public partial class BuildTypeControl : UserControlBase
    {
        private BuildTypeModel buildTypeObject;

        public BuildTypeControl(BuildTypeModel buildType)
        {
            this.InitializeComponent();

            this.InitializeValues();

            this.BuildType = buildType;

            this.uxHeader.Content = $"{buildType.BuildTypeName} = [[{buildType.BuildTypeIndex}]]";

            // Set this here to prevent the right pane from excessive refresh during loading
            this.uxRepeatType.SelectionChanged += this.RepeatType_Changed;
            this.uxCode.TextChanged += this.Code_Changed;
        }

        public BuildTypeModel BuildType
        {
            get
            {
                this.buildTypeObject.RepeatType = this.RepeatType;
                this.buildTypeObject.Code = this.Code;
                return this.buildTypeObject;
            }

            private set
            {
                this.buildTypeObject = value;
                this.RepeatType = this.buildTypeObject.RepeatType;
                this.Code = this.buildTypeObject.Code;
            }
        }

        public void ToggelCollaps(bool isColapsing)
        {
            try
            {
                if (!isColapsing)
                {
                    this.uxContent.Visibility = Visibility.Visible;

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Down;

                    this.Focus();
                }
                else
                {
                    this.uxContent.Visibility = Visibility.Collapsed;

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Right;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Collapse_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ToggelCollaps(this.uxCollapse.Direction != WPF.Tools.Specialized.DirectionsEnum.Right);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void RepeatType_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                this.buildTypeObject.RepeatType = this.uxRepeatType.SelectedItem == null ? RepeatTypeEnum.Once : (RepeatTypeEnum)((DataItemModel)this.uxRepeatType.SelectedItem).Tag;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Code_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                this.buildTypeObject.Code = this.uxCode.Text;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private RepeatTypeEnum RepeatType
        {
            get
            {
                return this.uxRepeatType.SelectedItem == null ? RepeatTypeEnum.Once : (RepeatTypeEnum)((DataItemModel)this.uxRepeatType.SelectedItem).Tag;
            }

            set
            {
                this.uxRepeatType.SelectedItem = ((DataItemModel[])this.uxRepeatType.ItemsSource).FirstOrDefault(x => (RepeatTypeEnum)x.Tag == value);
            }
        }
    
        private string Code
        {
            get
            {
                return this.uxCode.Text;
            }
            set
            {
                this.uxCode.Height = double.NaN;
                this.uxCode.Text = value;
            }
        }

        private void InitializeValues()
        {
            DataItemModel[] buildTypes = Enum.GetValues(typeof(RepeatTypeEnum))
                .Cast<RepeatTypeEnum>()
                .Select(x => new DataItemModel()
                {
                    DisplayValue = x.GetDescriptionAttribute(),
                    ItemKey = x,
                    Tag = x
                })
                .ToArray();

            this.uxRepeatType.ItemsSource = buildTypes;
        }        
    }
}
