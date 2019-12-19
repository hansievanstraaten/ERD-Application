using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace WPF.Tools.BaseClasses
{
  public class ModelsBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool hasChanged;

    private List<Tuple<string, int, object, object>> changesList = new List<Tuple<string, int, object, object>>();

    [NotMapped]
    public bool HasModelChanged
    {
      get
      {
        return this.hasChanged;
      }

      set
      {
        this.hasChanged = value;

        if (!value)
        {
          this.changesList.Clear();
        }
      }
    }

    public Dictionary<string, Tuple<int, object, object>[]> Changes
    {
      get
      {
        return this.changesList.GroupBy(g => g.Item1)
          .ToDictionary(d => d.Key, d => d.Select(s => new Tuple<int, object, object>(s.Item2, s.Item3, s.Item4)).ToArray());
      }
    }

    public Tuple<object, object> this[string propertyName] 
    {
      get
      {
        Tuple<string, int, object, object>[] resultPack = this.changesList.Where(k => k.Item1 == propertyName).ToArray();

        if (resultPack.Length == 0)
        {
          return null;
        }

        int oldIndex = resultPack.Min(o => o.Item2);

        int newIndex = resultPack.Max(n => n.Item2);

        object oldValue = resultPack.First(oi => oi.Item2 == oldIndex).Item3;

        object newValue = resultPack.First(ni => ni.Item2 == newIndex).Item4;

        if (oldValue == newValue)
        {
          return null;
        }

        return new Tuple<object, object>(oldValue, newValue);
      }
    }

    protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
    {
      this.OnPropertyChanged(Propertyname.GetProperty(expression).Name);
    }

    protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression, ref T oldValue, T newValue) where T : IComparable<T>
    {

      if (oldValue == null && newValue == null)
      {
        return;
      }

      if (oldValue != null && (oldValue.Equals(newValue) || oldValue.CompareTo(newValue) == 0))
      {
        return;
      }

      if (oldValue == null || oldValue.ToString() != newValue.ToString())
      {
        if (oldValue != null)
        {
          this.AddChangs(Propertyname.GetProperty(expression).Name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        this.OnPropertyChanged(Propertyname.GetProperty(expression).Name);
      }
    }

    protected virtual void OnPropertyChanged(string name) //where T : EqualityComparer<T>
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected virtual void OnPropertyChangedStringCompair<T>(string name, ref T oldValue, T newValue) where T : IComparable<T>
    {
      if (oldValue == null && newValue == null)
      {
        return;
      }

      if (oldValue != null && (oldValue.Equals(newValue) || oldValue.CompareTo(newValue) == 0))
      {
        return;
      }

      if (oldValue == null || oldValue.ToString() !=  newValue.ToString())
      {
        if (oldValue != null)
        {
          this.AddChangs(name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }
    }

    protected virtual void OnPropertyChangedStringCompair<T>(string name, ref Nullable<T> oldValue, Nullable<T> newValue) where T : struct
    {
      if (oldValue == null && newValue == null)
      {
        return;
      }

      if (oldValue.HasValue && newValue.HasValue)
      {
        if (oldValue.Value.Equals(newValue.Value))
        {
          return;
        }
      }

      if (oldValue.HasValue != newValue.HasValue || oldValue.ToString() != newValue.ToString())
      {
        if (oldValue != null)
        {
          this.AddChangs(name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }
    }

    protected virtual void OnPropertyChanged<T>(string name, ref T oldValue, T newValue) where T :  IComparable<T>
    {
      if (oldValue == null && newValue == null)
      {
        return;
      }

      if (oldValue != null && (oldValue.Equals(newValue) || oldValue.CompareTo(newValue) == 0))
      {
        return;
      }
      
      if (oldValue == null || oldValue.CompareTo(newValue) != 0)
      {
        if (oldValue != null)
        {
          this.AddChangs(name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }
    }

    protected virtual void OnPropertyChanged(string name, ref byte[] oldValue, byte[] newValue)
    {
      if (oldValue == null && newValue == null)
      {
        return;
      }

      if (oldValue == null || oldValue.Length != (newValue == null ? 0 : newValue.Length))
      {
        if (oldValue != null)
        {
          this.AddChangs(name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }
    }

    protected virtual void OnPropertyChanged<T>(string name, ref Nullable<T> oldValue, Nullable<T> newValue) where T : struct, IComparable<T>
    {
      if (oldValue == null && newValue == null)
      {
        return;
      }
      
      if (oldValue != null && (oldValue.Equals(newValue) || Comparer<Nullable<T>>.Default.Compare(oldValue, newValue) == 0))
      {
        return;
      }

      if (oldValue.HasValue != newValue.HasValue || (newValue.HasValue && oldValue.Value.CompareTo(newValue.Value) != 0))
      {
        if (oldValue != null)
        {
          this.AddChangs(name, oldValue, newValue);
        }

        oldValue = newValue;

        this.HasModelChanged = true;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
      }
    }

    private void AddChangs<T>(string property, T oldValue, T newValue)
    {
      int indexCount = this.changesList.Count;

      this.changesList.Add(new Tuple<string, int, object, object>(property, indexCount, oldValue, newValue));
    }
  }
}
