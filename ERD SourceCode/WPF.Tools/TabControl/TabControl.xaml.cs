using GeneralExtensions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;

namespace WPF.Tools.TabControl
{
  /// <summary>
  /// Interaction logic for TabControl.xaml
  /// </summary>
  public partial class TabControl : UserControl
  {
    public delegate void OnTabSelectedEvent(object sender, int itemIndex);

    public event OnTabSelectedEvent OnTabSelected;

    //private bool wasSizeReset = false;

    private int tabIndex;

    private int previousIndex = -1;

    private int maxTabls = -1;

    //private int sizeHitCount = 0;

    //private string maxMessage = "The maximum limit of tabs was reached.{1}This control allows for only {0} tables to be displayed.{1}{1}Please close some other tabs and try again";

    public TabControl()
    {
      this.InitializeComponent();

      this.Initialize();

      this.IsTabStop = false;

      this.Tabs.Focusable = false;
    }

    private WrapPanel Tabs
    {
      get
      {
        return this.uxTabs;
      }
    }

    public new UserControlBase Content
    {
      get
      {
        if (this.uxContent.Content == null)
        {
          return null;
        }

        return (UserControlBase) this.uxContent.Content;
      }

      set
      {
        this.uxContent.Content = null;

        this.uxContent.Content = value;
      }
    }
    
    public TabItemsCollection Items
    {
      get;

      set;
    }

    /// <summary>
    /// Gets or set the maximun tabls that can be displayed in the control
    /// <para>Defaults to -1</para>
    /// <para>-1 indicates no limit</para>
    /// </summary>
    public int MaxTabls
    {
      get
      {
        return this.maxTabls;
      }

      set
      {
        this.maxTabls = value;
      }
    }

    public int SelectedIndex
    {
      get;

      private set;
    }

    public object this[int index]
    {
      get
      {
        TabHeader header = this.TabItem(index);

        return this.Items[header.TabIndex];
      }
    }

    public object this[string headerText]
    {
      get
      {
        TabHeader tabItem = this.Tabs.Children.OfType<TabHeader>().FirstOrDefault(x => x.HeaderText == headerText);

        if (tabItem == null)
        {
          return null;
        }

        return this.Items[tabItem.TabIndex];
      }
    }

    //public bool InvalidateTabSize
    //{
    //  get;

    //  set;
    //}

    public object SelectedItem()
    {
      TabHeader header = this.TabItem(this.SelectedIndex);

      return this.Items[header.TabIndex];
    }

    public string SelectedTabHeaderText
    {
      get
      {
        TabHeader header = this.TabItem(this.SelectedIndex);

        if (header == null)
        {
          return string.Empty;
        }

        return header.HeaderText;
      }
    }

    public string GetTabHeaderText(int index)
    {
      TabHeader header = this.TabItem(index);

      if (header == null)
      {
        return string.Empty;
      }

      return header.HeaderText;
    }

    public void SetNotification(TabNotification notification)
    {
      try
      {
        foreach (TabHeader header in this.uxTabs.Children)
        {
          if (header.HeaderText == notification.TabHeaderToNotify)
          {
            header.ToolTip = notification.NotificationMessage;

            switch (notification.NotificationType)
            {
                case TabNotificationTypeEnum.Flash:

                  header.StartAnimation();

                  break;

                case TabNotificationTypeEnum.Lock:
                  
                header.IsEnabled = false;

                  break;

                case TabNotificationTypeEnum.Unlock:

                  header.IsEnabled = true;

                  break;

                case TabNotificationTypeEnum.PreventClose:

                  header.ShowCloseButton = false;

                  break;

              case TabNotificationTypeEnum.AllowClose:

                header.ShowCloseButton = true;

                break;
            }

            break;
          }
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }

    public void SetNotification(string tabName)
    {
      TabHeader tabItem = this.Tabs.Children.OfType<TabHeader>().FirstOrDefault(x => x.HeaderText == tabName);

      if (tabItem == null)
      {
        return;
      }

      tabItem.StartAnimation();
    }

    public void SetVisibility(string tabName, Visibility visibility)
    {
      TabHeader tabItem = this.Tabs.Children.OfType<TabHeader>().FirstOrDefault(x => x.HeaderText == tabName);

      if (tabItem == null)
      {
        return;
      }

      tabItem.Visibility = visibility;
    }

    public void SetVisibility(int index, Visibility visibility)
    {
      TabHeader tabItem = this.Tabs.Children[index] as TabHeader;

      if (tabItem == null)
      {
        return;
      }

      tabItem.Visibility = visibility;
    }

    public void SetEnabled(int index, bool isEnabled)
    {
      TabHeader tabItem = this.Tabs.Children[index] as TabHeader;

      if (tabItem == null)
      {
        return;
      }

      tabItem.IsTabEnabled = isEnabled;

      tabItem.IsEnabled = isEnabled;
    }

    public void SetHeaderName(int index, string name)
    {
      while (index > (this.Tabs.Children.Count - 1))
      {
        index--;

        if (index < 0)
        {
          return;
        }
      }

      TabHeader tabItem = this.Tabs.Children[index] as TabHeader;

      if (tabItem == null)
      {
        return;
      }
      
      tabItem.HeaderText = name;
    }

    public void SetTooltip(int index, string tooltip)
    {
      TabHeader tabItem = this.Tabs.Children[index] as TabHeader;

      if (tabItem == null)
      {
        return;
      }

      tabItem.ToolTip = tooltip;
    }

    public void SetActive(string tabHeader)
    {
      TabHeader tabItem = this.Tabs.Children.OfType<TabHeader>().FirstOrDefault(x => x.HeaderText == tabHeader);

      if (tabItem == null)
      {
        return;
      }

      this.UnSetPrevious();

      this.SetCurrent(tabItem.TabIndex);
    }

    public void SetActive(int index)
    {
      TabHeader tabItem = (TabHeader) this.Tabs.Children.GetByIndex(index);

      this.PerformItemSet(tabItem);
    }

    public void SetObjectId()
    {
    }

    private void SetCurrent(int index)
    {
      int indexCheck = this.previousIndex;

      TabHeader activeHeader = this.TabItem(indexCheck);

      if (activeHeader != null)
      {
        activeHeader.IsSelected = false;
      }

      TabHeader item = this.TabItem(index);

      if (item == null)
      {
        return;
      }

      item.IsSelected = true;

      UserControlBase control = this.Items[index];

      TabHeader nextHeader = this.TabItem(index);

      this.Content = control;

      this.previousIndex = index;

      this.SelectedIndex = index;

      control.InvalidateVisual();
    }
    
    public int GetNextInstanceIndex(int itemIndex, string headerText)
    {
      int result = 0;

      UserControlBase indexItem = this.Items[itemIndex];

      foreach (UserControlBase item in this.Items)
      {
        if (item.Name == indexItem.Name)
        {
          result++;
        }
      }

      // This is to slow, test it if you would like to, maybe it was my machine
      // UserControlBase[] items = this.Items.Where(i => i.InstanceID == indexItem.InstanceID).ToArray();
      if (result <= 1)
      {
        return 0;
      }

      UIElement[] elements = this.Tabs.FindVisualControls(typeof(TabHeader));

      foreach (UIElement item in elements)
      {
        if (((TabHeader) item).HeaderText.StartsWith(headerText))
        {
          string[] nameValArray = ((TabHeader) item).HeaderText.Split(' ');

          if (nameValArray.Length == 0)
          {
            return 0;
          }

          nameValArray[nameValArray.Length - 1] =
            nameValArray[nameValArray.Length - 1].Replace("(", string.Empty).Replace(")", string.Empty);

          int checkInt = 0;

          if (int.TryParse(nameValArray[nameValArray.Length - 1], out checkInt))
          {
            if (checkInt >= result)
            {
              result = checkInt + 1;
            }
          }
        }
      }

      return result;

    }

    internal void SetDisplayNameByIndex(int index, string headerText)
    {
      UIElement[] elements = this.Tabs.FindVisualControls(typeof(TabHeader));

      foreach (UIElement item in elements)
      {
        if (((TabHeader) item).TabIndex == index)
        {
          int indexVal = this.GetNextInstanceIndex(((TabHeader) item).TabIndex, headerText);

          ((TabHeader) item).HeaderText = indexVal == 0 ? headerText : string.Format("{0} ({1})", headerText, indexVal);

          break;
        }
      }
    }

    private void Initialize()
    {
      this.Items = new TabItemsCollection();

      this.Items.OnPropertyChangedEvent += this.Items_OnPropertyChangedEvent;

      this.VerticalAlignment = VerticalAlignment.Stretch;

      this.HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    private void ItemClick(object sender)
    {
      this.PerformItemSet((TabHeader) sender);
    }

    private void PerformItemSet(TabHeader item)
    {
      if (item == null)
      {
        return;
      }

      this.UnSetPrevious();

      if (!item.IsTabEnabled)
      {
        return;
      }

      this.SetCurrent(item.TabIndex);

      if (this.OnTabSelected != null)
      {
        this.OnTabSelected(item, item.TabIndex);
      }
    }

    private void CloseButtonClick(object sender)
    {
      TabHeader item = (TabHeader) sender;

      UserControlBase control = this.Items[item.TabIndex];

      this.Items.Remove(control);
    }

    //private void HandleNotifications(object sender, RoutedEventArgs e)
    //{
    //  e.Handled = true;

    //  ToolsNotificationItem item = (ToolsNotificationItem) e.OriginalSource;

    //  if (item.InstanceId == Guid.Empty)
    //  {
    //    return;
    //  }

    //  this.SetNotification(item);
    //}

    private void Items_OnPropertyChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:

          foreach (UserControlBase item in e.NewItems)
          {
            item.TabIndex = -1;
            
            if (this.MaxTabls > 0 && this.Items.Count > this.MaxTabls)
            {
              MessageBox.Show($"Maximum Tab Limit of {this.MaxTabls} reached");

              this.Items.Remove(item);

              return;
            }

            TabHeader header = new TabHeader();

            header.OnClick += this.ItemClick;

            header.OnCloseClick += this.CloseButtonClick;

            header.CanClose = item.ShowCloseButton;

            header.HeaderText = item.Title;

            header.TabIndex = this.tabIndex;

            //item.PropertyChanged += this.Item_PropertyChanged;

            //item.OnNotificationRequest += this.HandleNotifications;

            //item.SizeChanged += this.Control_SizeChanged;

            item.VerticalAlignment = VerticalAlignment.Stretch;

            item.HorizontalAlignment = HorizontalAlignment.Stretch;

            item.Tag = header;

            header.IsSelected = true;

            this.Tabs.Children.Add(header);

            item.TabIndex = this.tabIndex;

            this.UnSetPrevious();

            this.Content = item;

            this.previousIndex = this.tabIndex;

            this.tabIndex++;
          }

          break;

        case NotifyCollectionChangedAction.Remove:

          foreach (UserControlBase removeItem in e.OldItems)
          {
            if (this.Content == removeItem)
            {
              this.Content = null;

              if (removeItem.TabIndex == this.previousIndex)
              {
                this.SetNextTab();
              }
            }

            TabHeader tab = this.TabItem(removeItem.TabIndex);

            if (tab != null)
            {
              this.Tabs.Children.Remove(tab);
            }

            //removeItem.SizeChanged -= this.Control_SizeChanged;
          }

          break;

        case NotifyCollectionChangedAction.Reset:
          
          this.Items.Clear();

          this.Content = null;

          this.Tabs.Children.Clear();

          this.SelectedIndex = 0;

          this.tabIndex = 0;

          break;
      }
    }

    //private void Control_SizeChanged(object sender, SizeChangedEventArgs e)
    //{
    //  try
    //  {
    //    //if (this.uxContent.ActualHeight != 764)
    //    //{
          
    //    //}

    //    //this.uxContent.MaxHeight = this.ActualHeight;

    //    //this.uxContent.MaxWidth = this.ActualWidth;

    //    //UserControlBase senderControl = (UserControlBase) sender;

    //    //senderControl.Measure(e.NewSize);

    //    //senderControl.MaxHeight = this.ActualHeight;

    //    //senderControl.MaxWidth = this.ActualWidth;

    //    //senderControl.InvalidateVisual();

    //  }
    //  catch
    //  {
    //    // DO NOTHING
    //  }
    //}

    //private void Control_SizeChanged(object sender, SizeChangedEventArgs e)
    //{
    //  e.Handled = true;

    //  //if (!this.InvalidateTabSize || this.wasSizeReset)
    //  //{
    //  //  this.sizeHitCount++;

    //  //  if (this.sizeHitCount == 1)
    //  //  {
    //  //    return;
    //  //  }

    //  //  this.wasSizeReset = false;

    //  //  this.sizeHitCount = 0;

    //  //  return;
    //  //}

    //  try
    //  {
    //    this.SizeChanged -= this.Control_SizeChanged;

    //    this.uxContent.MaxWidth = this.ActualWidth;

    //    this.uxContent.MaxHeight = this.ActualHeight;

    //    //this.ResetSizes(this);

    //    //double height = 0;

    //    //double down = 0;

    //    //this.ForceChildTablsResize(this, 0, out height, out down);

    //    //this.wasSizeReset = true;
    //  }
    //  catch
    //  {
    //    throw;
    //  }
    //  finally
    //  {
    //    this.SizeChanged += this.Control_SizeChanged;
    //  }
    //}

    private void SetNextTab()
    {
      if (this.Tabs.Children.Count == 1)
      {
        return;
      }

      int checkIndex = this.previousIndex - 1;

      TabHeader item = null;

      while (checkIndex >= 0)
      {
        item = this.TabItem(checkIndex);

        if (item != null)
        {
          this.SetCurrent(checkIndex);

          return;
        }

        checkIndex--;
      }

      checkIndex = this.previousIndex + 1;

      while (checkIndex <= this.tabIndex)
      {
        item = this.TabItem(checkIndex);

        if (item != null)
        {
          this.SetCurrent(checkIndex);

          return;
        }

        checkIndex++;
      }
    }

    private void UnSetPrevious()
    {
      TabHeader current = this.TabItem(this.previousIndex);

      if (current != null)
      {
        current.IsSelected = false;
      }
    }

    private TabHeader TabItem(int index)
    {
      UIElement[] elements = this.Tabs.FindVisualControls(typeof(TabHeader));

      foreach (UIElement item in elements)
      {
        if (((TabHeader) item).TabIndex == index)
        {
          return item as TabHeader;
        }
      }

      return null;
    }

    //private void ResetSizes(TabControl control)
    //{
    //  UIElement parent = control.FindParentControlBase(typeof(TabControl));

    //  if (parent == null)
    //  {
    //    if (control.Content.MaxHeight == double.PositiveInfinity)
    //    {
    //      return;
    //    }

    //    control.Content.MaxHeight = double.PositiveInfinity;

    //    control.Content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

    //    control.InvalidateVisual();

    //    return;
    //  }

    //  this.ResetSizes(parent.To<TabControl>());

    //  if (control.Content.MaxHeight == double.PositiveInfinity)
    //  {
    //    return;
    //  }

    //  control.Content.MaxHeight = double.PositiveInfinity;

    //  control.Content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

    //  control.InvalidateVisual();

    //  return;
    //}

    //private void ForceChildTablsResize(TabControl control, int depth, out double controlHeight, out double downSize)
    //{
    //  controlHeight = double.PositiveInfinity;

    //  if (!control.IsLoaded)
    //  {
    //    downSize = double.PositiveInfinity;

    //    return;
    //  }

    //  if (control.Content == null || !control.Content.IsLoaded)
    //  {
    //    downSize = double.PositiveInfinity;

    //    return;
    //  }

    //  UIElement parent = control.FindParentControlBase(typeof(TabControl));

    //  if (parent == null)
    //  {
    //    control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

    //    controlHeight = control.ActualHeight;

    //    downSize = controlHeight - control.uxTabs.DesiredSize.Height;
        
    //    control.InvalidateVisual();

    //    return;
    //  }

    //  TabControl parentBase = parent.To<TabControl>();

    //  int nextDepth = depth + 1;

    //  this.ForceChildTablsResize(parent.To<TabControl>(), nextDepth, out controlHeight, out downSize);

    //  downSize = downSize - control.uxTabs.DesiredSize.Height - 60;

    //  if (downSize < 100)
    //  {
    //    return;
    //  }

    //  control.Content.MaxHeight = downSize;

    //  control.InvalidateVisual();
    //}
    
  }
}
