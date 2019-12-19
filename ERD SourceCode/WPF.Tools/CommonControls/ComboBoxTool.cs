using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Tools.CommonControls
{
  public class ComboBoxTool : ComboBox
  {
    public static DependencyProperty ItemKeyProperty = DependencyProperty.Register("SelectedItemKey", typeof(string), typeof(ComboBoxTool), new FrameworkPropertyMetadata(null, SelectedItem_Changed));
    
    private bool isSorting = false;

    private bool sortOff = false;
    
    private Type originalType;
    
    public ComboBoxTool()
    {
      base.SelectedValuePath = "ItemKey";

      base.DisplayMemberPath = "DisplayValue";

      this.UseLayoutRounding = true;

      this.VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;

      this.SnapsToDevicePixels = true;

      this.VisualClearTypeHint = System.Windows.Media.ClearTypeHint.Enabled;;
    }
    
    /// <summary>
    /// Gets or sets the sort capability
    /// <para>NOTE: Use where text only is applied</para>
    /// </summary>
    public bool IsSortOn
    {
      get
      {
        return this.sortOff;
      }

      set
      {
        this.sortOff = value;
      }
    }

    public object SelectedItemKey
    {
      get
      {
        object result = this.GetValue(ItemKeyProperty);

        if (result == null)
        {
          return null;
        }

        return Convert.ChangeType(result, this.originalType);
      }

      set
      {
        if (value == null)
        {
          return;
        }
        
        this.originalType = value.GetType();

        this.SetValue(ItemKeyProperty, value == null ? string.Empty : value.ToString());

        base.SelectedValue = value;
      }
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      if (this.isSorting || e.Action != NotifyCollectionChangedAction.Add || !this.IsSortOn)
      {
        return;
      }

      if (this.Items.Count > 0)
      {
        this.isSorting = true;

        try
        {
          List<string> collectionList = new List<string>();

          foreach (object item in this.Items)
          {
            if (item == null)
            {
              continue;
            }

            foreach (string val in this.ReturnValues(item))
            {
              collectionList.Add(val);
            }
          }

          this.Items.Clear();

          string[] itemsArray = collectionList.ToArray();

          Array.Sort(itemsArray);

          foreach (string value in itemsArray)
          {
            this.Items.Add(value);
          }
        }
        finally
        {
          this.isSorting = false;
        }
      }
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);

      this.SelectedItemKey = base.SelectedValue;
    }

    private static void SelectedItem_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ComboBoxTool box = (ComboBoxTool)sender;

      box.SelectedItemKey = e.NewValue;
    }

    private string[] ReturnValues(object item)
    {
      if (item == null)
      {
        return new string[] { };
      }

      if (item.GetType() == typeof(string))
      {
        return new string[] {Convert.ToString(item)};
      }

      if (item.GetType() == typeof(string[]))
      {
        return (string[]) item;
      }

      Type ttt = item.GetType();

      return new string[] { };
    }
  }
}
