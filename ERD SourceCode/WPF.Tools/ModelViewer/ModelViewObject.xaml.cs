using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using WPF.Tools.Attributes;
using WPF.Tools.Collections;
using System.Windows;
using GeneralExtensions;
using WPF.Tools.Dictionaries;

namespace WPF.Tools.ModelViewer
{
    /// <summary>
    /// Interaction logic for ModelViewObject.xaml
    /// </summary>
    public partial class ModelViewObject : UserControl
    {
        public delegate void ModelViewItemGotFocusEvent(ModelViewItem sender, object focuedObject);

        public delegate void ModelViewItemBrowseEvent(object sender, string buttonKey);

        public event ModelViewItemGotFocusEvent ModelViewItemGotFocus;

        public event ModelViewItemBrowseEvent ModelViewItemBrowse;

        public ModelViewObject(object classObject, bool loadOnlyAttributedFlag)
        {
            this.InitializeComponent();

            this.Loaded += this.ModelViewObject_Loaded;

            this.LoadOnlyAttributedFields = loadOnlyAttributedFlag;

            this.ClassObject = classObject;

            this.ClassObjectType = this.ClassObject.GetType();

            this.Items = new ItemsCollection<ModelViewItem>();

            object headerObj = this.ClassObjectType.GetCustomAttribute(typeof(ModelNameAttribute));

            ModelNameAttribute header = headerObj == null ? new ModelNameAttribute(this.ClassObjectType.Name) : (ModelNameAttribute)headerObj;

            this.Header = header.ModelName;

            this.uxCollapse.Visibility = header.AllowHeaderCollapse ? Visibility.Visible : Visibility.Collapsed;

            if (header.AllowHeaderCollapse)
            {
                this.uxContent.Margin = new Thickness(27, 0, 0, 0);
            }

            this.LoadPropertyObjects();
        }

        public ModelViewObject(object classObject) : this(classObject, true)
        {

        }

        public bool LoadOnlyAttributedFields { get; set; }

        /// <summary>
        /// RUNTIME PROPETY ONLY
        /// </summary>
        public bool AllowHeaderCollapse
        {
            get
            {
                return this.uxCollapse.Visibility == Visibility.Visible;
            }
        }

        public double CaptionTextWidth
        {
            get
            {
                double result = 0;

                foreach (ModelViewItem item in this.uxContent.Children)
                {
                    if (item.CaptionTextWidth > result)
                    {
                        result = item.CaptionTextWidth;
                    }
                }

                return result;
            }
        }

        public object ClassObject { get; private set; }

        public object Header
        {
            get
            {
                return this.uxHeader.Content;
            }

            set
            {
                this.uxHeader.Content = value;
            }
        }

        public Type ClassObjectType { get; set; }

        public ModelViewItem this[string caption]
        {
            get
            {
                foreach (ModelViewItem item in this.uxContent.Children)
                {
                    if (item.Caption.ParseToString() == caption)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public ModelViewItem this[int itemIndex]
        {
            get
            {
                return (ModelViewItem)this.uxContent.Children[itemIndex];
            }
        }

        public ItemsCollection<ModelViewItem> Items { get; set; }

        new public void Focus()
        {
            ((ModelViewItem)this.uxContent.Children[0]).Focus();
        }

        public void SetAllowHeaderCollapse(bool allow)
        {
            this.uxCollapse.Visibility = allow ? Visibility.Visible : Visibility.Collapsed;
        }

        public void HideHeader(bool hide)
        {
            this.uxHeaderStack.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ToggelCollaps(bool isColapsing)
        {
            try
            {
                if (!isColapsing)
                {
                    this.uxContent.Visibility = Visibility.Visible;

                    this.uxCollapse.Direction = Specialized.DirectionsEnum.Down;

                    this.Focus();
                }
                else
                {
                    this.uxContent.Visibility = Visibility.Collapsed;

                    this.uxCollapse.Direction = Specialized.DirectionsEnum.Right;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void AllignAllCaptions(double maxWidth)
        {
            foreach (ModelViewItem item in this.uxContent.Children)
            {
                item.CaptionWidth = maxWidth;
            }
        }

        private void ModelViewObject_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                double maxCaptionWidth = 100;

                foreach (ModelViewItem viewerItem in this.Items)
                {
                    if (viewerItem.CaptionTextWidth > maxCaptionWidth)
                    {
                        maxCaptionWidth = viewerItem.CaptionTextWidth;
                    }
                }

                foreach (ModelViewItem viewerItem in this.Items)
                {
                    viewerItem.CaptionWidth = maxCaptionWidth;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
            }
        }

        private void ModelItem_Focus(ModelViewItem sender, object focusedbject)
        {
            if (this.ModelViewItemGotFocus != null)
            {
                this.ModelViewItemGotFocus(sender, focusedbject);
            }
        }

        private void ModelViewItem_Browse(object sender, string buttonkey)
        {
            if (this.ModelViewItemBrowse != null)
            {
                this.ModelViewItemBrowse(this, buttonkey);
            }
        }

        private void Collapse_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                this.ToggelCollaps(this.uxCollapse.Direction != Specialized.DirectionsEnum.Right);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void LoadPropertyObjects()
        {
            List<PropertyInfo> propertiesList = new List<PropertyInfo>(this.ClassObjectType.GetProperties());

            foreach (PropertyInfo inf in propertiesList)
            {
                if (this.LoadOnlyAttributedFields)
                {
                    object fieldValues = inf.GetCustomAttribute(typeof(FieldInformationAttribute));

                    if (fieldValues == null)
                    {
                        continue;
                    }
                }

                ModelViewItem viewItem = this.CreateField(this.ClassObjectType.Name, this.ClassObject, inf);

                viewItem.ModelViewItemGotFocus += this.ModelItem_Focus;

                this.Items.Add(viewItem);
            }

            foreach (ModelViewItem child in this.Items.OrderBy(s => s.Sort))
            {
                this.uxContent.Children.Add(child);
            }

            if (this.Items.Count > 0)
            {
                ((ModelViewItem)this.uxContent.Children[0]).Focus();
            }
        }

        private ModelViewItem CreateField(string parentTypeName, object parentObject, PropertyInfo property)
        {
            ModelViewItem result = null;

            result = new ModelViewItem(parentTypeName, parentObject, property);

            result.ModelViewItemBrowse += this.ModelViewItem_Browse;

            //captionWidth = result.CaptionTextWidth;

            return result;
        }

    }
}
