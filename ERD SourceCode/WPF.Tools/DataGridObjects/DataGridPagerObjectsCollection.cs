using GeneralExtensions;
using System;
using System.Linq;
using WPF.Tools.Dictionaries;

namespace WPF.Tools.DataGridObjects
{
  public class DataGridPagerObjectsCollection<T>
  {
    public delegate void OnItemsCollectionChangedEvent(object sender, T[] changedItems);

    public event OnItemsCollectionChangedEvent OnItemsCollectionChanged;

    private bool addAsRange = false;

    private T[] itemsArray = new T[0];

    public DataGridPagerObjectsCollection(int pageSize)
    {
      this.SelectedItemIndex = -1;

      this.PageSize = pageSize;
    }

    public int PageSize { get; set; }

    public int PageIndex { get; private set; }

    public int TotalPageNumbers
    {
      get
      {
        if (this.itemsArray == null || this.itemsArray.Length == 0)
        {
          return 0;
        }

        return Math.Ceiling((this.itemsArray.Length.ToDecimal() / this.PageSize.ToDecimal())).ToInt32();
      }
    }

    public int SelectedItemIndex
    {
      get;

      private set;
    }

    public int IndexOf(T item)
    {
      return Array.IndexOf(this.itemsArray, item);
    }

    public T[] Items
    {
      get
      {
        return this.itemsArray;
      }
    }

    public T[] this[int pageIndex]
    {
      get
      {
        this.PageIndex = pageIndex;

        if (pageIndex < 0 || pageIndex >= this.itemsArray.Length)
        {
          this.SelectedItemIndex = -1;

          return new T[] { };
        }

        int startTake = this.PageSize * pageIndex;

        return this.itemsArray.Skip(startTake).Take(this.PageSize).ToArray();
      }
    }

    public string ItemSourceSelectionText
    {
      get
      {
        if (this.itemsArray == null || this.itemsArray.Length == 0)
        {
          return TranslationDictionary.Translate("No Records Found");
        }

        int startTake = this.PageSize * this.PageIndex;

        int stopTake = (startTake + this.PageSize);

        if (stopTake > this.itemsArray.Length)
        {
          stopTake = itemsArray.Length;
        }

        return $"{startTake} - {stopTake} of {this.itemsArray.Length}";
      }
    }

    public void Add(T item)
    {
      this.SelectedItemIndex = (this.itemsArray.Length + 1);

      Array.Resize(ref this.itemsArray, (this.itemsArray.Length + 1));

      this.itemsArray[(this.SelectedItemIndex - 1)] = item;

      if (!this.addAsRange && this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, new T[] { item });
      }
    }

    public void AddRange(T[] items)
    {
      this.addAsRange = true;

      foreach (T item in items)
      {
        this.Add(item);
      }

      this.addAsRange = false;

      if (this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, this[0]);
      }
    }

    public void InsertItem(T item, int atIndex)
    {
      if (atIndex < 0)
      {
        atIndex = 0;
      }

      Array.Resize(ref this.itemsArray, (this.itemsArray.Length + 1));

      for (int x = (this.itemsArray.Length - 1); x > atIndex; x--)
      {
        this.itemsArray[x] = this.itemsArray[(x - 1)];
      }

      this.itemsArray[atIndex] = item;

      if (this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, new T[] { item });
      }
    }

    public void Remove(T item)
    {
      int itemIndex = Array.IndexOf(this.itemsArray, item);

      for (int x = itemIndex; x < (this.itemsArray.Length - 1); x++)
      {
        this.itemsArray[x] = this.itemsArray[(x + 1)];
      }

      Array.Resize(ref this.itemsArray, (this.itemsArray.Length - 1));

      if (this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, new T[] { item });
      }
    }

    public void UpdateItem(T item)
    {
      int itemIndex = Array.IndexOf(this.itemsArray, item);

      this.itemsArray[itemIndex] = item;
    }

    public void Clear()
    {
      Array.Resize(ref this.itemsArray, 0);

      this.SelectedItemIndex = -1;

      if (this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, new T[] { });
      }
    }

    public void Refresh()
    {
      if (this.OnItemsCollectionChanged != null)
      {
        this.OnItemsCollectionChanged(this, new T[] { });
      }
    }
  }
}
