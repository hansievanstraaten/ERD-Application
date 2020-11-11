using System.Threading.Tasks;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
	/// <summary>
	/// Interaction logic for ReleaseNotes.xaml
	/// </summary>
	public partial class ReleaseNotes : UserControlBase
	{
		public ReleaseNotes()
		{
			this.InitializeComponent();
		}

		private async void ConnectToGitHub()
		{
			await Task.Run(() => 
			{
			});
		}
	}
}
