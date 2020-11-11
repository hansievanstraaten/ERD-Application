using GeneralExtensions;
using System;
using System.Diagnostics;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : UserControlBase
	{
		public About()
		{
			this.InitializeComponent();

			this.uxLogo.Source = IconSet.IconSets.ResourceImageSource("ViSo_nice", 150);

			this.uxAboutText.Text = this.AboutText();
		}
		private string AboutText()
		{
			return "Thank you for using ViSo-Nice." + Environment.NewLine + Environment.NewLine +
				   "The product is supplied totally free of charge and no information is required from yourself or " +
				   "your PC." + Environment.NewLine + Environment.NewLine +
				   "The product is developed as a production ready application, solving real life situation that " +
				   "I encountered during database and C# development. Features implemented was done so purely to " +
				   "resolve situation that I encountered." + Environment.NewLine + Environment.NewLine +
				   "The Reporting Solution was developed just for fun, and I hope you will enjoy it." + Environment.NewLine + Environment.NewLine +
				   "Your support is appreciated, and some feedback would be appreciated." + Environment.NewLine +
				   "Info.ViSoNice @gmail.com" + Environment.NewLine + Environment.NewLine +
				   "NS: For this release the help files and release notes are in the GitHub repository under the 'ERD Msi' Folder. This may change in later versions.";
		}

		private void GoToGitHub_Cliked(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				Process.Start("https://github.com/hansievanstraaten/ERD-Application");
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}
	}
}
