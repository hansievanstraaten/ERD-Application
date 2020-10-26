using System;
using System.Windows;
using WPF.Tools.BaseClasses;
using GeneralExtensions;
using System.Windows.Input;

namespace ViSo.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewerWindow.xaml
    /// </summary>
    public partial class ViewerWindow : WindowBase
    {
        public delegate void OnItemBrowseEvent(object sender, object model, string buttonKey);

        public event OnItemBrowseEvent OnItemBrowse;

        public ViewerWindow(string title, object[] models)
        {
            this.InitializeComponent();

            this.Title = title;

            foreach (object model in models)
            {
                this.uxViewer.Items.Add(model);
            }

            this.AutoSize = true;

            this.Loaded += this.ViewerWindow_Loaded;
        }

        public ViewerWindow(string title, object model) : this(title, new object[] { model })
        {
        }

        public ViewerWindow(string title, string fieldCaption, object model) : this(title, model)
        {
            this.uxViewer[0, 0].Caption = fieldCaption;

            this.uxViewer.AllignAllCaptions();
        }

        public WPF.Tools.ModelViewer.ModelViewer ModelView
        {
            get
            {
                return this.uxViewer;
            }
        }

        private void ViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.uxViewer.Focus();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void OkButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (this.uxViewer.HasValidationError)
            {
                return;
            }

            this.DialogResult = true;

            this.Close();
        }

        private void Cancel_Cliked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }

        private void ModelItem_Browse(object sender, string buttonKey)
        {
            try
            {
                this.OnItemBrowse?.Invoke(sender, this.uxViewer.Items[0], buttonKey);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            switch (e.Key)
            {
                case Key.Enter:

                    this.DialogResult = true;

                    break;
            }
        }
    }
}
