using GeneralExtensions;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder.ReportComponents
{
    /// <summary>
    /// Interaction logic for ToolsMenuItem.xaml
    /// </summary>
    public partial class ToolsMenuItem : UserControlBase
    {
        //private bool allowDragDrop;

        public ToolsMenuItem()
        {
            this.InitializeComponent();
        }

        public Type ToolType { get; set; }

        public string Caption 
        { 
            get
            {
                return this.uxCaption.Content.ParseToString();
            }
            
            set
            {
                this.uxCaption.Content = value;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.Background = Brushes.LightSteelBlue;

            this.uxCaption.Foreground = Brushes.DarkSlateGray;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.Background = null;

            //this.uxCaption.Foreground = Brushes.LightGray;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //if (!this.allowDragDrop)
            //{
            //    return;
            //}

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();

                data.SetData(DataFormats.StringFormat, this.Caption);
                data.SetData(this);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            //if (!this.allowDragDrop)
            //{
            //    Mouse.SetCursor(Cursors.None);

            //    return;
            //}

            //if (e.Effects.HasFlag(DragDropEffects.Copy))
            //{
            //    Mouse.SetCursor(Cursors.Cross);
            //}
            //else 
            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
    }
}
