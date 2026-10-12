using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tools.Common
{
    /// <summary>
    /// Interaction logic for MonospacedMessageBox.xaml
    /// </summary>
    public partial class MonospacedMessageBox : UserControlBase
    {
        public MonospacedMessageBox()
        {
            this.InitializeComponent();
        }

        public string Message
        {
            get => this.uxMessageBox.Text;
            set => this.uxMessageBox.Text = value;
        }
    }
}
