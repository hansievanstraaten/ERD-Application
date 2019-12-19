using GeneralExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViSo.SharedEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.Dictionaries;
using WPF.Tools.Functions;
using WPF.Tools.ToolModels;

namespace WPF.Tools.DataGridObjects
{
  /// <summary>
  /// Interaction logic for ViSoDataGridPager.xaml
  /// </summary>
  public partial class ViSoDataGridPager : UserControlBase
  {
    public delegate void SelectedPageChangeEvent(object sender, object[] selectedItems);

    public event SelectedPageChangeEvent SelectedPageChange;

    private readonly int[] pageSizes = new int[] { 20, 30, 50, 75, 100 };

    private int pageIndex = 1;

    private int searchHitCount = 0;

    private bool showSearch = false;

    private DateTime lastSearched = DateTime.Now;

    private DataGridPagerObjectsCollection<object> itemsSource;

    private readonly List<object> searchList = new List<object>();

    private readonly List<object> holdList = new List<object>();

    public ViSoDataGridPager()
    {
      this.InitializeComponent();

      this.ItemsSource = new DataGridPagerObjectsCollection<object>(this.pageSizes[0]);

      this.ItemsSource.OnItemsCollectionChanged += this.ItesmSource_Changed;

      this.Loaded += this.ViSoDataGridPager_Loaded;

      this.uxPageSize.SelectionChanged -= this.PageSize_Changed;

      foreach (int sizeItem in this.pageSizes)
      {
        this.uxPageSize.Items.Add(new DataItemModel { DisplayValue = sizeItem.ToString(), ItemKey = sizeItem});
      }

      this.uxPageSize.SelectedValue = this.pageSizes[0];

      this.uxPageSize.SelectionChanged += this.PageSize_Changed;
    }

    public bool ShowSearch
    {
      get
      {
        return this.showSearch;
      }

      set
      {
        this.showSearch = value;

        if (this.IsLoaded)
        {
          this.uxSearchTextBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

          this.uxSearhLabel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
      }
    }
    
    public int PageSize
    {
      get
      {
        return this.ItemsSource.PageSize;
      }

      set
      {
        this.uxPageSize.SelectedItemKey = value;
      }
    }

    public int IndexOf(object item)
    {
      return this.ItemsSource.IndexOf(item);
    }

    public string SearchTextTitle
    {
      get
      {
        return this.uxSearhLabel.Content.ParseToString();
      }

      set
      {
        this.uxSearhLabel.Content = value;
      }
    }

    public string SearchText
    {
      get
      {
        return this.uxSearchTextBox.Text;
      }

      set
      {
        this.uxSearchTextBox.Text = value;
      }
    }

    public void InsertItem(object item, int atIndex)
    {
      this.ItemsSource.InsertItem(item, atIndex);
    }

    public void RemoveItem(object item)
    {
      this.ItemsSource.Remove(item);
    }

    public void UpdateItem(object item)
    {
      this.ItemsSource.UpdateItem(item);
    }

    public DataGridPagerObjectsCollection<object> ItemsSource
    {
      get
      {
        return this.itemsSource;
      }

      set
      {
        this.itemsSource = value;
      }
    }

    private void ViSoDataGridPager_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.uxSearhLabel.Visibility = this.ShowSearch ? Visibility.Visible : Visibility.Collapsed;

        this.uxSearchTextBox.Visibility = this.ShowSearch ? Visibility.Visible : Visibility.Collapsed;
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void LeftButton_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        this.pageIndex--;

        this.uxPageNumbers.SelectedIndex = this.pageIndex;
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
    
    private void RightButton_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        this.pageIndex++;

        this.uxPageNumbers.SelectedIndex = this.pageIndex;
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void ItesmSource_Changed(object sender, object[] changedItems)
    {
      try
      {
        int holdPage = this.uxPageNumbers.SelectedIndex;

        this.uxPageNumbers.Items.Clear();

        for (int x = 1; x <= this.itemsSource.TotalPageNumbers; x++)
        {
          this.uxPageNumbers.Items.Add(new DataItemModel { ItemKey = x, DisplayValue = $"Page {x}" });
        }

        if (holdPage < 0)
        {
          holdPage = 0;
        }

        if (holdPage >= this.uxPageNumbers.Items.Count)
        {
          holdPage = (this.uxPageNumbers.Items.Count - 1);
        }

        this.uxPageNumbers.SelectedIndex = holdPage;

        if (this.searchList.Count > 0)
        {
          this.uxSearchTextBox.Focus();
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void SearchText_Changed(object sender, TextChangedEventArgs e)
    {
      try
      {
        if (this.uxSearchTextBox.Text.IsNullEmptyOrWhiteSpace())
        {
          this.ItemsSource.Clear();

          this.itemsSource.AddRange(this.holdList.ToArray());

          this.holdList.Clear();

          this.searchList.Clear();

          this.SelectedPageChange?.Invoke(this, this.ItemsSource[this.pageIndex]);
          
          this.uxSearchTextBox.Focus();

          return;
        }

        if (this.holdList.Count == 0)
        {
          this.holdList.AddRange(this.ItemsSource.Items);

        }

        string searchText = this.uxSearchTextBox.Text;

        ConcurrentBag<object> searchBag = new ConcurrentBag<object>();

        Parallel.ForEach(this.holdList, row =>
        {
          if (row.ContainsValue(searchText, DataComparisonEnum.Contains))
          {
            searchBag.Add(row);
          }
        });

        this.searchList.Clear();

        this.searchList.AddRange(searchBag);

        this.lastSearched = DateTime.Now;

        this.ChangeItemSource();
      }
      catch(Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void PageNumbers_Changed(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        this.pageIndex = this.uxPageNumbers.SelectedIndex;

        if (this.pageIndex >= (this.ItemsSource.TotalPageNumbers - 1))
        {
          this.uxButtonRight.IsEnabled = false;
        }
        else
        {
          this.uxButtonRight.IsEnabled = true;
        }

        // NOTE: We do not do an If-Else cause the total page count can be 1
        if (this.pageIndex <= 0)
        {
          this.uxButtonLeft.IsEnabled = false;
        }
        else
        {
          this.uxButtonLeft.IsEnabled = true;
        }

        object[] result = this.ItemsSource[this.pageIndex]; // We need to do this to refresh the text

        this.uxRecordsLable.Content = $"{this.ItemsSource.ItemSourceSelectionText}";

        this.SelectedPageChange?.Invoke(this, result);
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void PageSize_Changed(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      try
      {
        this.ItemsSource.PageSize = this.uxPageSize.SelectedValue.ToInt32();

        this.ItemsSource.Refresh();
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private async void ChangeItemSource()
    {
      if (this.searchHitCount > 0)
      {
        return;
      }

      this.searchHitCount++;

      try
      {
        await Task.Factory.StartNew(() =>
        {
          TimeSpan time = this.lastSearched.DateDifference(DateTime.Now);

          while (time.TotalMilliseconds < 700)
          {
            time = this.lastSearched.DateDifference(DateTime.Now);

            Sleep.ThreadWait(100);
          }

          this.searchHitCount = 0;
        });

        if (this.holdList.Count > 0)
        {
          this.ItemsSource.Clear();

          this.ItemsSource.AddRange(this.searchList.ToArray());
        }
      }
      catch //(Exception err)
      {
        this.searchHitCount = 0;
        // DO NOTHING
      }
    }

  }
}
