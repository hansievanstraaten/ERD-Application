using ERD.Viewer.Models.ProjectModels;
using ERD.Viewer.Tools.BaseControls;
using System;

namespace ERD.Viewer;

public partial class ProjectSetup : ViewerWindowBase
{
    public ProjectSetup(ProjectModel project)
    {
        this.InitializeComponent();
    }

    private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {

        }
        catch (Exception err)
        {
            base.ShowError(err);
        }
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}