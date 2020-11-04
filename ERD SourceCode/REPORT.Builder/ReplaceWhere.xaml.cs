using GeneralExtensions;
using REPORT.Data.Common;
using System.Collections.Generic;
using System.Linq;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReplaceWhere.xaml
	/// </summary>
	public partial class ReplaceWhere : UserControlBase
	{
		public delegate void ColumnSelectionChangedEvent(object sender, int index, string selectedValue);

		public event ColumnSelectionChangedEvent ColumnSelectionChanged;

		private List<DataItemModel> columnslIst = new List<DataItemModel>();

		public ReplaceWhere()
		{
			this.InitializeComponent();
		}

		public int Index { get; set; }

		public bool IsValueFromColumnObject
		{
			get
			{
				return this.columnslIst.Any(a => 
				a.DisplayValue != Constants.None
				&& a.DisplayValue == this.WhereValue);
			}
		}

		public string WhereOption
		{
			get
			{
				return this.uxSelectedColumn.SelectedValue.ParseToString();
			}

			set
			{
				this.uxSelectedColumn.SelectedValue = value;
			}
		}

		public string WhereValue
		{
			get
			{
				return this.uxWhereValue.Text;
			}

			set
			{
				this.uxWhereValue.Text = value;
			}
		}

		public void SetSelectableColumns(List<DataItemModel> columns)
		{
			this.uxSelectedColumn.Items.Clear();

			foreach (DataItemModel column in columns)
			{
				this.uxSelectedColumn.Items.Add(column);
			}

			this.uxSelectedColumn.SelectedValue = Constants.None;
		}

		public void SetValueColumns(List<DataItemModel> columns)
		{
			this.uxWhereValue.Items.Clear();

			this.columnslIst.Clear();

			this.columnslIst.AddRange(columns);

			foreach (DataItemModel column in columns)
			{
				this.uxWhereValue.Items.Add(column);
			}

			this.uxWhereValue.SelectedValue = Constants.None;
		}

		private void SelectedColumn_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			this.ColumnSelectionChanged?.Invoke(this, this.Index, this.uxSelectedColumn.SelectedValue.ParseToString());
		}
	}
}
