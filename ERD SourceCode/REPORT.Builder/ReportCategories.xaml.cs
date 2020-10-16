using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ViSo.Dialogs.Input;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Exstention;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportCategories.xaml
    /// </summary>
    public partial class ReportCategories : UserControlBase
    {
        private ReportHeaderFooters uxReports;

        public ReportCategories()
        {
            this.InitializeComponent();

            this.uxReports = new ReportHeaderFooters(ReportTypeEnum.ReportContent);

            Grid.SetRow(this.uxReports, 0);

            Grid.SetColumn(this.uxReports, 3);

            this.uxMainGrid.Children.Add(this.uxReports);

            this.LoadCategoryTree();
        }

        private void AddCategory_Cliked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string caption = this.SelectedCategory == null ? "Parent Category" : $"Child Category for {this.SelectedCategory.Header}";

                if (InputBox.ShowDialog(this.GetParentWindow(), true, "New Category", caption).IsFalse())
                {
                    return;
                }

                ReportTablesRepository repo = new ReportTablesRepository();

                long? nullLong = null;

                ReportCategoryModel model = new ReportCategoryModel
                {
                    CategoryName = InputBox.Result,
                    IsActive = true,
                    ParentCategoryId = this.SelectedCategory == null ? nullLong : this.SelectedCategory.Tag.ToInt64()
                };

                repo.UpdateReportCategory(model);

                if (this.SelectedCategory == null)
                {
                    this.uxCategoryTree.Items.Add(this.CreateTreeItem(model));
                }
                else
                {
                    this.SelectedCategory.Items.Add(this.CreateTreeItem(model));
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void DeleteCategory_Cliked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.SelectedCategory == null)
			{
                MessageBox.Show("Please Select a Category.");

                return;
			}

            try
            {
                long categoryId = this.SelectedCategory.Tag.ToInt64();

                ReportTablesRepository repo = new ReportTablesRepository();

                if (repo.CategoryHaveReports(categoryId))
                {
                    throw new ApplicationException("Cannot delete Category while reports are attached.");
                }

                string message = $"Are you sure that you want to delete '{this.SelectedCategory.Header}'?";

                MessageBoxResult boxResult = MessageBox.Show(message, "Delete", MessageBoxButton.YesNo);

                if (boxResult != MessageBoxResult.Yes)
				{
                    return;
				}

                repo.DeleteCategory(categoryId);

                if (this.SelectedCategory.Parent.GetType() == typeof(TreeViewItemTool))
                {
                    TreeViewItemTool parent = this.SelectedCategory.Parent as TreeViewItemTool;

                    parent.Items.Remove(this.SelectedCategory);
                }
                else
				{
                    this.uxCategoryTree.Items.Remove(this.SelectedCategory);
				}

            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void CategoryTreeMouse_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.SelectedCategory != null)
            {
                this.SelectedCategory.IsSelected = false;

                this.uxReports.SetReportTypeCategory(null);
            }
        }

        private void TreeViewItemSelection_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedCategory == null)
                {
                    this.uxReports.SetReportTypeCategory(null);

                    return;
                }

                this.uxReports.SetReportTypeCategory(this.SelectedCategory.Tag.ToInt64());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void LoadCategoryTree()
        {
            ReportTablesRepository repo = new ReportTablesRepository();

            List<ReportCategoryModel> categories = repo.GetActiveCategories();

            foreach(ReportCategoryModel parent in categories.Where(pc => pc.ParentCategoryId == null))
            {
                this.uxCategoryTree.Items.Add(this.BuildTree(parent.CategoryId, this.CreateTreeItem(parent), categories));
            }
        }

        private TreeViewItemTool BuildTree(long categoryId, TreeViewItemTool parentItem, List<ReportCategoryModel> categories)
        {
            foreach (ReportCategoryModel category in categories.Where(pc => pc.ParentCategoryId == categoryId))
            {
                parentItem.Items.Add(this.BuildTree(category.CategoryId, this.CreateTreeItem(category), categories));
            }

            return parentItem;
        }

        private TreeViewItemTool CreateTreeItem(ReportCategoryModel category)
        {
            TreeViewItemTool result = new TreeViewItemTool { Header = category.CategoryName, Tag = category.CategoryId };

            result.Selected += this.TreeViewItemSelection_Changed;

            return result;
        }

        private TreeViewItemTool SelectedCategory
        {
            get
            {
                return this.uxCategoryTree.SelectedItem as TreeViewItemTool;
            }
        }
    }
}
