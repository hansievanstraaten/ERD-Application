using System.Collections.ObjectModel;

namespace WPF.Tools.Collections
{
  public class ItemsCollection<T> : ObservableCollection<T>
  {
    public void AddRange(T[] items)
    {
      foreach (T item in items)
      {
        base.Add(item);
      }
    }
  }
}
