using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for CategorySelector.xaml
	/// </summary>
	public partial class CategorySelector : UserControlBase
	{
		public CategorySelector()
		{
			this.InitializeComponent();

			this.LoadCategoryTree();
		}

		public long SelectedCategoryId
		{
			get
			{
				if (this.SelectedCategory == null)
				{
					return -1;
				}

				return this.SelectedCategory.Tag.ToInt64();
			}
		}

		public TreeViewItemTool SelectedCategory
		{
			get
			{
				return this.uxCategoryTree.SelectedItem == null ? null : this.uxCategoryTree.SelectedItem as TreeViewItemTool;
			}
		}

		private void LoadCategoryTree()
		{
			ReportTablesRepository repo = new ReportTablesRepository();

			List<ReportCategoryModel> categories = repo.GetActiveCategories();

			foreach (ReportCategoryModel parent in categories.Where(pc => pc.ParentCategoryId == null))
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

			return result;
		}

		
	}
}
