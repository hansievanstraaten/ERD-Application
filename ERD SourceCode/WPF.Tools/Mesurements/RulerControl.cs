using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Tools.Mesurements
{
    public class RulerControl : Canvas
    {
        private RulerImage ruler;

        private double lastTraceLenght = 100;

        private Point rightClick;

        private Marker selecteMarker;

        public RulerControl()
        {
            this.Initialize();
        }

        public RulerOrientationEnum Orientation
        {
            get;

            set;
        }

        public double WithValue
        {
            get;

            set;
        }

        public double TraceLength
        {
            get
            {
                return this.lastTraceLenght;
            }

            set
            {
                this.lastTraceLenght = value;

                foreach (UIElement item in this.Children)
                {
                    if (item.GetType() == typeof(Marker))
                    {
                        ((Marker)item).TraceLength = value;
                    }
                }
            }
        }

        public bool IsViewOnly
        {
            get;

            set;
        }

        public void Refresh(double pxWidth, double pxHeight, double traceLenght)
        {
            this.TraceLength = traceLenght;

            this.Width = pxWidth;

            this.Height = pxHeight;

            this.ruler.Refresh(pxWidth, pxHeight);
        }

        private void Initialize()
        {
            this.WithValue = 25;

            this.Loaded += Ruler_Loaded;

            this.ruler = new RulerImage();

            Canvas.SetZIndex(this.ruler, 0);

            this.Children.Add(this.ruler);
        }

        public void ClearMarkers(bool staticOnly)
        {
            List<Marker> clearList = new List<Marker>();

            foreach (UIElement item in this.Children)
            {
                if (item.GetType() != typeof(Marker))
                {
                    continue;
                }

                Marker mark = (Marker)item;

                if (!mark.IsStatic && staticOnly)
                {
                    continue;
                }

                clearList.Add(mark);
            }

            foreach (Marker item in clearList)
            {
                item.Dispose();

                this.Children.Remove(item);
            }
        }

        public void AddMarker(double atPoint, bool isStatic)
        {
            if (this.IsViewOnly)
            {
                return;
            }

            Marker mark = new Marker(isStatic);

            mark.Orientation = this.Orientation;

            Canvas.SetLeft(mark, this.Orientation == RulerOrientationEnum.Horizontal ? atPoint - 3 : 3);

            Canvas.SetTop(mark, this.Orientation == RulerOrientationEnum.Horizontal ? 3 : atPoint - 3);

            Canvas.SetZIndex(mark, 1);

            this.Children.Add(mark);
        }

        private void AddMarker()
        {
            if (this.IsViewOnly)
            {
                return;
            }

            Marker mark = new Marker();

            mark.Orientation = this.Orientation;

            Canvas.SetLeft(mark, this.Orientation == RulerOrientationEnum.Horizontal ? this.rightClick.X - 3 : 3);

            Canvas.SetTop(mark, this.Orientation == RulerOrientationEnum.Horizontal ? 3 : this.rightClick.Y - 3);

            Canvas.SetZIndex(mark, 1);

            this.Children.Add(mark);
        }

        private void DeleteMarker()
        {
            if (this.selecteMarker == null)
            {
                return;
            }

            if (this.selecteMarker.IsStatic)
            {
                return;
            }

            this.selecteMarker.Dispose();

            this.Children.Remove(this.selecteMarker);
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            UIElement item = System.Windows.Input.Mouse.DirectlyOver as UIElement;

            if (item.GetType() == typeof(Marker))
            {
                return;
            }

            this.rightClick = e.GetPosition(this);

            this.AddMarker();

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsViewOnly)
            {
                return;
            }

            UIElement item = System.Windows.Input.Mouse.DirectlyOver as UIElement;

            MenuItem mItem = (MenuItem)this.ContextMenu.Items[1];

            if (item.GetType() != typeof(Marker))
            {
                mItem.Visibility = System.Windows.Visibility.Collapsed;

                this.selecteMarker = null;
            }
            else
            {
                mItem.Visibility = System.Windows.Visibility.Visible;

                this.selecteMarker = (Marker)item;
            }

            this.rightClick = e.GetPosition(this);

            base.OnMouseRightButtonDown(e);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            switch (item.Name)
            {
                case "uxAdd":
                    this.AddMarker();

                    break;

                case "uxDelete":
                    this.DeleteMarker();

                    break;
            }
        }

        private void Ruler_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!this.IsViewOnly)
            {
                this.CreateContextMenu();
            }

            this.ruler.Orientation = this.Orientation;

            if (this.Orientation == RulerOrientationEnum.Horizontal)
            {
                this.ruler.Refresh(this.DesiredSize.Width, this.WithValue);

                return;
            }

            this.ruler.Refresh(this.WithValue, this.DesiredSize.Height);
        }

        private void CreateContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem add = new MenuItem { Name = "uxAdd", Header = "Add Marker" };

            MenuItem delete = new MenuItem { Name = "uxDelete", Header = "Delete Marker" };

            add.Click += MenuItem_Click;

            delete.Click += MenuItem_Click;

            menu.Items.Add(add);

            menu.Items.Add(delete);

            this.ContextMenu = menu;
        }
    }
}
