using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPF.Tools.Collections;

namespace WPF.Tools.ModelViewer
{
  public class ModelViewer : StackPanel
  {
    public delegate void ModelViewItemBrowseEvent(object sender, string buttonKey);

    public event ModelViewItemBrowseEvent ModelViewItemBrowse;

    private bool loadOnlyAttributedFields = true;

    public ModelViewer()
    {
      this.Orientation = Orientation.Vertical;

      this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

      this.Items = new ItemsCollection<object>();

      this.Items.CollectionChanged += this.Items_Changed;

      this.Margin = new System.Windows.Thickness(1, 1, 10, 10);

      this.CanVerticallyScroll = true;
    }

    public bool HasValidationError
    {
      get
      {
        bool result = false;

        foreach (ModelViewObject parent in this.Children)
        {
          foreach (ModelViewItem view in parent.Items)
          {
            view.BindingExpression.UpdateSource();

            if (view.BindingExpression.HasError)
            {
              view.uxError.Visibility = System.Windows.Visibility.Visible;

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
              view.uxError.Visibility = System.Windows.Visibility.Collapsed;
            }
          }
        }

        return result;
      }
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

    public object SelectedItem
    {
      get;
      private set;
    }

    public ModelViewObject this[int classObjectIndex]
    {
      get
      {
        return (ModelViewObject) this.Children[classObjectIndex];
      }
    }

    public ModelViewItem this[string caption]
    {
      get
      {
        foreach (ModelViewObject child in this.Children)
        {
          ModelViewItem result = child[caption];

          if (result != null)
          {
            return result;
          }
        }

        return null;
      }
    }

    public ModelViewItem this[int classObjectIndex, int itemIndex]
    {
      get
      {
        ModelViewObject child = (ModelViewObject)this.Children[classObjectIndex];

        if (child == null)
        {
          return null;
        }

        ModelViewItem result = child[itemIndex];

        if (result != null)
        {
          return result;
        }

        return null;
      }
    }

    public ModelViewItem this[int classObjectIndex, string caption]
    {
      get
      {
        ModelViewObject child = (ModelViewObject)this.Children[classObjectIndex];

        if (child == null)
        {
          return null;
        }

        ModelViewItem result = child[caption];

        if (result != null)
        {
          return result;
        }

        return null;
      }
    }

    public ModelViewItem SelectedModelItem
    {
      get;
      private set;
    }

    public ItemsCollection<object> Items
    {
      get;
      set;
    }

    public void AllignAllCaptions()
    {
      double maxWidth = 0;

      foreach (ModelViewObject item in this.Children)
      {
        if (item.CaptionTextWidth > maxWidth)
        {
          maxWidth = item.CaptionTextWidth;
        }
      }

      foreach (ModelViewObject item in this.Children)
      {
        item.AllignAllCaptions(maxWidth);
      }
    }

    new public void Focus()
    {
      ((ModelViewObject)this.Children[0]).Focus();
    }
    
    private void Items_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:

          foreach (object item in e.NewItems)
          {
            ModelViewObject viewObject = new ModelViewObject(item, this.LoadOnlyAttributedFields);

            viewObject.ModelViewItemBrowse += this.ModelViewItem_Browse;

            viewObject.ModelViewItemGotFocus += this.ModelItem_Focus;

            this.Children.Add(viewObject);
          }

          break;

        case NotifyCollectionChangedAction.Reset:

          this.Children.Clear();

          break;

        case NotifyCollectionChangedAction.Remove:

          foreach (ModelViewObject oldItem in e.OldItems)
          {
            this.Children.Remove(oldItem);
          }

          break;

        case NotifyCollectionChangedAction.Move:
        case NotifyCollectionChangedAction.Replace:
          throw new NotImplementedException($"{e.Action} not Implemented");
      }
    }

    private void ModelViewItem_Browse(object sender, string buttonkey)
    {
      if (this.ModelViewItemBrowse != null)
      {
        this.ModelViewItemBrowse(sender, buttonkey);
      }
    }

    private void ModelItem_Focus(ModelViewItem sender, object focusedbject)
    {
      this.SelectedModelItem = sender;

      this.SelectedItem = focusedbject;
    }
  }
}
