using System;
using System.Windows;
using System.Windows.Controls;
using WPF.Tools.BaseClasses;
using GeneralExtensions;

namespace ViSo.Dialogs.Controls
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : WindowBase
    {
        private string boolUpdateMethodName;

        public ControlWindow(string windowTitle, UserControlBase control, string boolUpdateMethod, bool showOkButton, bool showCancelButton)
        {
            this.InitializeComponent();

            this.Title = windowTitle;

            this.uxContent.Content = control;

            this.boolUpdateMethodName = boolUpdateMethod;

            this.uxButtonOk.Visibility = showOkButton ? Visibility.Visible : Visibility.Collapsed;

            this.uxButtonCancel.Visibility = showCancelButton ? Visibility.Visible : Visibility.Collapsed;

            this.Loaded += this.ControlWindow_Loaded;
        }

        public UserControlBase UserControl
        {
            get
            {
                return this.uxContent.Content.To<UserControlBase>();
            }
        }

        private void ControlWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.AutoSize = true;
        }

        private void OkButton_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.boolUpdateMethodName.IsNullEmptyOrWhiteSpace())
                {
                    bool updateResult = this.InvokeMethod(this.uxContent.Content, this.boolUpdateMethodName, new object[] { }).TryToBool();

                    if (!updateResult)
                    {
                        return;
                    }
                }

                if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
                {
                    this.DialogResult = true;
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Cancel_Cliked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
