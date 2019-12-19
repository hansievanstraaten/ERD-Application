using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GeneralExtensions;
using Control = System.Windows.Forms.Control;
using FlowDirection = System.Windows.FlowDirection;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace WPF.Tools.CommonControls
{
  public class PasswordBoxBinding : TextBox
  {
    [DllImport("user32.dll")]
    static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
    
    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint idThread);

    public static readonly DependencyProperty SecuredTextProperty = DependencyProperty.Register("SecuredText", typeof(SecureString), typeof(PasswordBoxBinding), null);

    private SecureString securedText = new SecureString();

    private KeysConverter converter = new KeysConverter();
    
    public PasswordBoxBinding()
    {
      //base.PreviewKeyDown += this.OnKey_Press;

      //base.Text
    }

    public new string Text
    {
      private get
      {
        return base.Text;
      }

      set
      {
        base.Text = string.Empty.PadRight(value.Length, '*');
      }
    }

    public SecureString SecuredText
    {
      get
      {
        return this.securedText;
      }

      set
      {
        this.SecuredText = value;

        this.Text = value.ToString();
      }
    }

    
    public double TextLenght
    {
      get
      {
        return this.StringRenderLength(this.Text);
      }
    }
    
    public double StringRenderLength(string textValue)
    {
      var formattedText = new FormattedText(
        textValue,
        CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
        this.FontSize,
        Brushes.Black,
        new NumberSubstitution(),
        1);

      return formattedText.Width;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      if ((e.Key < Key.D0 || e.Key > Key.Z) &
          (e.Key < Key.NumPad0 || e.Key > Key.Divide) &
          (e.Key < Key.Oem1 || e.Key > Key.OemBackslash))
      {
        if ((e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Delete) ||
            Control.ModifierKeys == Keys.Shift && e.Key == Key.Home)
        {
          base.OnPreviewKeyDown(e);

          return;
        }
        
        e.Handled = true;

        return;
      }

      uint virtualKeyCode = (uint)KeyInterop.VirtualKeyFromKey(e.Key);

      uint scanCode = MapVirtualKey(virtualKeyCode, 0);

      IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

      StringBuilder result = new StringBuilder();

      ToUnicodeEx(virtualKeyCode, scanCode, new byte[255], result, (int)5, (uint)0, inputLocaleIdentifier);
      
      this.SecuredText.AppendChar(result.ToString().ToCharArray()[0]);

      e.Handled = true;

      base.Text += "*";

      //base.OnPreviewKeyDown(e);
    }

    private void OnKey_Press(object sender, KeyEventArgs e)
    {
      if ((e.Key < Key.D0 || e.Key > Key.Z) &
           (e.Key < Key.NumPad0 || e.Key > Key.Divide))
      {
        return;

      }

      uint virtualKeyCode = (uint)KeyInterop.VirtualKeyFromKey(e.Key);

      uint scanCode = MapVirtualKey(virtualKeyCode, 0);

      IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

      StringBuilder result = new StringBuilder();
      ToUnicodeEx(virtualKeyCode, scanCode, new byte[255], result, (int)5, (uint)0, inputLocaleIdentifier);


      var key = converter.ConvertToString(e.Key);

      this.SecuredText.AppendChar('c');
    }
  }
}
