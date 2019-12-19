using ERD.Models;
using System;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Database
{
  /// <summary>
  /// Interaction logic for UserNameAndPassword.xaml
  /// </summary>
  public partial class UserNameAndPassword : WindowBase
  {
    public UserNameAndPassword(UserNameAndPasswordModel model)
    {
      this.InitializeComponent();

      this.Loaded += this.UserNameAndPassword_Loaded;

      this.PasswordModel = model;
    }

    public UserNameAndPasswordModel PasswordModel {get; set;}

    private void UserNameAndPassword_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.uxViewer.Items.Add(this.PasswordModel);
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
    }

    private void Accept_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.uxViewer.HasValidationError)
        {
          return;
        }

        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
    }
  }
}
