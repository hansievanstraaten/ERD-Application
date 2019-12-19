using System.Windows;
using System.Windows.Controls;

namespace WPF.Tools.Specialized
{
  /// <summary>
  /// Interaction logic for PasswordBoxBindable.xaml
  /// </summary>
  public partial class PasswordBoxBindable : UserControl
  {
    public static readonly DependencyProperty PasswordTextProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxBindable), new FrameworkPropertyMetadata(null, Password_Changed));

    private bool isTyping = false;
    
    public PasswordBoxBindable(object dataContext)
    {
      this.InitializeComponent();

      this.DataContext = dataContext;

      this.uxPasswordBox.PasswordChanged += this.Password_Changed;
    }

    new public bool Focus()
    {
      return this.uxPasswordBox.Focus();
    }

    public string Password
    {
      get
      {
        return (string)this.GetValue(PasswordTextProperty);
      }

      set
      {
        this.SetValue(PasswordTextProperty, value);

        if (!this.isTyping)
        {
          this.uxPasswordBox.Password = value;
        }
      }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

      this.Focus();
    }

    private void Password_Changed(object sender, RoutedEventArgs e)
    {
      this.isTyping = true;

      try
      {
        this.Password = this.uxPasswordBox.Password;
      }
      finally
      {
        this.isTyping = false;
      }
    }

    private static void Password_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      PasswordBoxBindable box = (PasswordBoxBindable) sender;

      if (box.isTyping)
      {
        return;
      }
      
      box.uxPasswordBox.Password = e.NewValue.ToString();
    }
  }
}
