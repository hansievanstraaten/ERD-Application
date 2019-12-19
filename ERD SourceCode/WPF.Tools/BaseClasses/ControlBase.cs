using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Tools.BaseClasses
{
  public abstract class ControlBase : Control, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    
    public ControlBase() : 
      base()
    {
      this.Initialize();
    }

    protected void OnPropertyChanged() // All properties changed
    {
      OnPropertyChanged(null);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
    {
      OnPropertyChanged(Propertyname.GetProperty(expression).Name);
    }
    
    private void Initialize()
    {
      NameScope.SetNameScope(this, new NameScope());

      this.UseLayoutRounding = true;

      this.VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;

      this.SnapsToDevicePixels = true;

      this.VisualClearTypeHint = System.Windows.Media.ClearTypeHint.Enabled;

      
    }
  }
}
