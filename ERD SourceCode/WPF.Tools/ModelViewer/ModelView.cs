using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF.Tools.Attributes;
using WPF.Tools.Collections;
using WPF.Tools.CommonControls;

namespace WPF.Tools.ModelViewer
{
  public class ModelView : TreeView
  {
    private bool loadOnlyAttributedFields = true;
    
    private Dictionary<string, PropertyInfo> hidenItemsDict = new Dictionary<string, PropertyInfo>();

    private Dictionary<string, int> itemsDesiredIndexDic = new Dictionary<string, int>();
    
    public ModelView()
    {
      object windowStyle = this.FindResource("ModelViewStyle");

      if (windowStyle != null && windowStyle.GetType() == typeof(Style))
      {
        this.Style =  (Style)windowStyle;
      }

      this.Items = new ItemsCollection<dynamic>();

      this.Items.CollectionChanged += this.Items_Changed;

      this.DataContext = this;
      
      this.BorderThickness = new Thickness(0);

      this.BorderBrush = Brushes.Transparent;
      
      this.ContextMenu = new ContextMenu();
    }

    public bool LoadOnlyAttributedFields
    {
      get
      {
        return this.loadOnlyAttributedFields;
      }

      set
      {
        this.loadOnlyAttributedFields = value;
      }
    }

    public new object SelectedItem {get; private set; }

    public ModelViewItem SelectedModelItem {get; private set;}

    public new ItemsCollection<object> Items
    {
      get;

      set;
    }

    public void SetValue(int parentIndex, string propertyName, object value)
    {
      TreeViewItemTool parent = (TreeViewItemTool)base.Items[parentIndex];

      foreach (ModelViewItem item in parent.Items)
      {
        if (item.PropertyName != propertyName)
        {
          continue;
        }

        PropertyInfo inf = item.BindingExpression.DataItem.GetType().GetProperty(item.BindingExpression.ParentBinding.Path.Path);

        inf.SetValue(item.BindingExpression.DataItem, value, null);

        break;
      }
    }

    public void IsReadOnly(int parentIndex, string propertyName, bool isReadOnly)
    {
      TreeViewItemTool parent = (TreeViewItemTool)base.Items[parentIndex];

      foreach (ModelViewItem item in parent.Items)
      {
        if (item.PropertyName != propertyName)
        {
          continue;
        }

        item.IsReadOnly = isReadOnly;

        break;
      }
    }

    public void RefreshcaptionWidths()
    {
      foreach (TreeViewItemTool parent in base.Items)
      {
        double maxWidth = 0;

        foreach(ModelViewItem model in parent.Items)
        {
          if (model.CaptionTextWidth > maxWidth)
          {
            maxWidth = model.CaptionTextWidth;
          }
        }

        foreach(ModelViewItem model in parent.Items)
        {
          model.CaptionWidth = maxWidth;
        }
      }

      this.InvalidateVisual();
    }

    public bool HasValidationError
    {
      get
      {
        bool result = false;

        foreach (TreeViewItemTool parent in base.Items)
        {
          foreach (ModelViewItem view in parent.Items)
          {
            view.BindingExpression.UpdateSource();

            if (view.BindingExpression.HasError)
            {
              view.uxError.Visibility = Visibility.Visible;

              StringBuilder errors = new StringBuilder();

              foreach (ValidationError error in view.BindingExpression.ValidationErrors)
              {
                errors.AppendLine(error.ErrorContent.ToString());
              }

              view.uxError.Content = errors.ToString();

              result = true;
            }
            else
            {
              
              view.uxError.Visibility = Visibility.Collapsed;
            }
          }
        }

        return result;
      }
    }

    private void Items_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
          case NotifyCollectionChangedAction.Add:

            foreach (object item in e.NewItems)
            {
              this.AddItem(item);
            }

            break;

          case NotifyCollectionChangedAction.Reset:

            base.Items.Clear();

            break;

          case NotifyCollectionChangedAction.Remove:
          case NotifyCollectionChangedAction.Move:
          case NotifyCollectionChangedAction.Replace:

            break;
      }
    }

    private void ModelItem_Focus(ModelViewItem sender, object focuedobject)
    {
      this.SelectedModelItem = sender;

      this.SelectedItem = focuedobject;
    }

    private void VisibilityItem_Checked(object sender, RoutedEventArgs e)
    { // We need to do this as the Visibility set leaves a gap as big as the one in my teeth
      MenuItem item = (MenuItem) sender;
      
      if (item.IsChecked)
      {
        string itemKey = item.Tag.ToString();
        
        string[] itemKeysArray = itemKey.Split(new char[] { ':' });

        PropertyInfo propertyItem = this.hidenItemsDict[itemKey];
        
        int desiredIndex = itemsDesiredIndexDic[itemKey];

        int parentIndex = 0;

        TreeViewItemTool parent = this.FindParentItem(itemKeysArray[0], out parentIndex);

        ModelViewItem modelItem = new ModelViewItem(parent.Tag.ToString(), this.Items[parentIndex], propertyItem);
        
        if (parent.Items.Count == 0)
        {
          this.hidenItemsDict.Remove(itemKey);

          parent.Items.Add(modelItem);

          return;
        }

        modelItem.ModelViewItemGotFocus += this.ModelItem_Focus;

        string checkItemKey = $"{((ModelViewItem)parent.Items[0]).ParentObjectTypeName}:{((ModelViewItem)parent.Items[0]).PropertyName}";

        int itemsCollectionIndex = itemsDesiredIndexDic[checkItemKey];

        int insertAt = 0;

        while (itemsCollectionIndex < desiredIndex)
        {
          insertAt++;

          if (insertAt >= parent.Items.Count)
          {
            break;
          }

          checkItemKey = $"{((ModelViewItem)parent.Items[insertAt]).ParentObjectTypeName}:{((ModelViewItem)parent.Items[insertAt]).PropertyName}";

          itemsCollectionIndex = itemsDesiredIndexDic[checkItemKey];
        }

        modelItem.Visibility = Visibility.Visible;
        
        this.hidenItemsDict.Remove(itemKey);

        parent.Items.Insert(insertAt, modelItem);
      }
      else
      {
        int parentIndex = -1;

        string itemKey = item.Tag.ToString();

        ModelViewItem propertyItem = this.FindItem(itemKey, out parentIndex);

        propertyItem.ModelViewItemGotFocus -= this.ModelItem_Focus;

        TreeViewItemTool parent = ((TreeViewItemTool) base.Items[parentIndex]);

        this.hidenItemsDict.Add(itemKey, propertyItem.PropertyInfo);
        
        parent.Items.Remove(propertyItem);
      }
        
      this.RefreshcaptionWidths();
    }
    
    private void AddItem(object item)
    {
      Type itemType = item.GetType();

      TreeViewItemTool headerItem = this.CreateHeader(itemType);

      List<PropertyInfo> propertiesList = new List<PropertyInfo>(itemType.GetProperties());

      List<ModelViewItem> itemsList = new List<ModelViewItem>();
      
      double maxCaptionWidth = 100;

      foreach (PropertyInfo inf in propertiesList)
      {
        double itemWidth = 0;

        ModelViewItem viewItem = this.CreateField(headerItem.Tag.ToString(), item, inf, out itemWidth);

        if (!viewItem.HasFieldInformation && this.LoadOnlyAttributedFields)
        {
          continue;
        }
        
        viewItem.ModelViewItemGotFocus += this.ModelItem_Focus;

        itemsList.Add(viewItem);

        if (itemWidth > maxCaptionWidth)
        {
          maxCaptionWidth = itemWidth;
        }
      }
      
      int itemIndex = 0;

      foreach (ModelViewItem modelItem in itemsList.OrderBy(i => i.Sort))
      {
        modelItem.CaptionWidth = maxCaptionWidth;

        string itemKey = $"{headerItem.Tag}:{modelItem.PropertyName}";

        itemsDesiredIndexDic.Add(itemKey, itemIndex);

        this.CreateVisibilityMenuItem(modelItem, itemKey.ToString());

        itemIndex++;

        if (modelItem.Visibility != Visibility.Visible)
        {
          this.hidenItemsDict.Add(itemKey , modelItem.PropertyInfo);

          continue;
        }

        headerItem.Items.Add(modelItem);
      }

      base.Items.Add(headerItem);
    }

    private TreeViewItemTool CreateHeader(Type itemType)
    {
      ModelNameAttribute header = (ModelNameAttribute)itemType.GetCustomAttribute(typeof(ModelNameAttribute));

      return new TreeViewItemTool
      {
        Header = (header == null ? itemType.Name : header.ModelName), 
        IsExpanded = true,
        FontWeight = FontWeights.DemiBold,
        Tag = itemType.Name
      };
    }

    private ModelViewItem CreateField(string parentTypeName, object parentObject, PropertyInfo property, out double captionWidth)
    {
      ModelViewItem result = null;

      result = new ModelViewItem(parentTypeName, parentObject, property);
      
      captionWidth = result.CaptionTextWidth;

      return result;
    }

    private ModelViewItem FindItem(string propertyName, out int parentIndex)
    {
      parentIndex = -1;

      string[] propertyKeys = propertyName.Split(new char[] {':'});

      foreach (TreeViewItemTool item in base.Items)
      {
        parentIndex++;

        foreach (ModelViewItem propertyItem in item.Items)
        {
          if (propertyItem.ParentObjectTypeName == propertyKeys[0] && propertyItem.PropertyName == propertyKeys[1])
          {
            return propertyItem;
          }
        }
      }

      return null;
    }

    private TreeViewItemTool FindParentItem(string itemTypeName, out int parentIndex)
    {
      parentIndex = -1;

      foreach (TreeViewItemTool item in base.Items)
      {
        parentIndex++;

        if (item.Tag.ToString() == itemTypeName)
        {
          return item;
        }
      }


      return null;
    }

    private void CreateVisibilityMenuItem(ModelViewItem propertyItem, string parentName)
    {
      MenuItem visibilityMenuItem = new MenuItem
      {
        IsCheckable = true,
        IsChecked = propertyItem.Visibility == Visibility.Visible ? true : false,
        Header = propertyItem.Caption,
        Tag = parentName
      };

      visibilityMenuItem.Checked += this.VisibilityItem_Checked;

      visibilityMenuItem.Unchecked += this.VisibilityItem_Checked;

      this.ContextMenu.Items.Add(visibilityMenuItem);
    }
  }
}
