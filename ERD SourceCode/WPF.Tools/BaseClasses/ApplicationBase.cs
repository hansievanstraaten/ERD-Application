using System;
using System.Windows;

namespace WPF.Tools.BaseClasses
{
  public abstract class ApplicationBase : Application
  {
    public ApplicationBase()
    {
      Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri(@"/WPF.Tools;component/Styles/WindowStyles.xaml", UriKind.Relative)});
      Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary{Source = new Uri(@"/WPF.Tools;component/Styles/ApplicationColours.xaml", UriKind.Relative)});
      
    }
  }
}
