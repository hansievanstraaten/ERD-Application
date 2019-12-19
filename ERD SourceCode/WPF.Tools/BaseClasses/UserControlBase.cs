using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Tools.Dictionaries;

namespace WPF.Tools.BaseClasses
{
  public abstract class UserControlBase : UserControl, INotifyPropertyChanged
  {
    private string title;

    public event PropertyChangedEventHandler PropertyChanged;

    public UserControlBase()
    {
      this.Loaded += this.UserControlBase_Loaded;
    }

    public bool ShowCloseButton { get; set;}

    public bool WasFirstLoaded {get; set;}

    public string Title
    {
      get
      {
        return this.title;
      }

      set
      {
        this.title = value;
      }
    }

    public string AssemblyString
    {
      get
      {
        Type thisType = this.GetType();
        
        string result = $"{thisType.FullName},{thisType.Module.Name.Replace(".dll", string.Empty)}";

        return result;
      }
    }
    
    private void UserControlBase_Loaded(object sender, RoutedEventArgs e)
    {
      if (TranslationDictionary.TranslationLoaded)
      {
        this.Title = TranslationDictionary.Translate(this.Title);
      }
    }
    
    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
      base.OnPreviewKeyUp(e);

      switch (e.Key)
      {
        case Key.F12:

          string fullName = this.GetType().FullName;

          StringBuilder message = new StringBuilder();

          foreach (string item in this.GetType().FullName.Split(new char[] {'.'}))
          {
            message.AppendLine(item);
          }

          MessageBox.Show(message.ToString(), "You are at:");

          break;
      }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      try
      {
        PropertyChangedEventHandler eventHandler = this.PropertyChanged;

        if (eventHandler != null)
        {
          eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
      }
      catch
      {
        // DO NOTHING
      }
    }

    protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
    {
      this.OnPropertyChanged(Propertyname.GetProperty(expression).Name);
    }
  }
}
