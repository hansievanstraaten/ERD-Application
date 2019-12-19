using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Database
{
  /// <summary>
  /// Interaction logic for BrowseData.xaml
  /// </summary>
  public partial class BrowseData : WindowBase
  {
    private DataTable queryResults;

    private MenuItem connectionItem;

    public BrowseData(string sqlQuery, string title)
    {
      this.InitializeComponent();

      this.Title = $"Browse - {title}";

      this.DataContext = this;
      
      this.uxSqlQuery.ActionKeys = new Key[] { Key.F5 };

      if (!sqlQuery.IsNullEmptyOrWhiteSpace())
      {
        this.uxSqlQuery.Text = sqlQuery;
        
        this.ActionSQLQuery();
      }
    }

    public BrowseData(TableModel tableModel, MenuItem connectionMenue) : this(string.Empty, string.Empty)
    {
      this.connectionItem = connectionMenue;

      this.uxColumn0.MaxHeight = 250;

      this.uxColumn0.Height = new GridLength(250, GridUnitType.Auto);

      this.uxColumn1.Height = new GridLength(3, GridUnitType.Pixel);

      this.Title = $"Browse - {Connections.DatabaseModel.ServerName} - {Connections.DatabaseModel.DatabaseName} - {tableModel.TableName}";
      
      string selectQuery = TableQueryBuilder.BuildSelectTop(tableModel);
      
      if (!selectQuery.IsNullEmptyOrWhiteSpace())
      {
        this.uxSqlQuery.Text = selectQuery;

        this.ActionSQLQuery();
      }
    }

    public DataTable QueryResults
    {
      get
      {
        return this.queryResults;
      }

      set
      {
        this.queryResults = value;

        base.OnPropertyChanged(() => this.QueryResults);
      }
    }

    private void SqlQuert_Action(object sender, KeyEventArgs e)
    {
      this.ActionSQLQuery();
    }

    private void ActionSQLQuery()
    {
      try
      {
        if (this.connectionItem != null)
        {
          Connections.SetConnection(this.connectionItem);
        }
        else
        {
          Connections.SetDefaultConnection();
        }

        this.uxMessage.Content = "Executing Query";

        DataAccess dataAccess = new DataAccess(Connections.DatabaseModel);

        List<dynamic> resultList = dataAccess.ExecuteQueryDynamic(this.uxSqlQuery.Text);

        DataTable result = new DataTable();

        if (resultList.Count == 0)
        {
          this.QueryResults = result;

          return;
        }

        foreach (string column in ((IDictionary<string, object>)resultList[0]).Keys)
        {
          result.Columns.Add(column);
        }

        foreach (var dataItem in resultList)
        {
          result.Rows.Add(((IDictionary<string, object>)dataItem).Values.ToArray());
        }

        this.uxRowCount.Content = $"{result.Rows.Count} Rows";

        this.QueryResults = result;

        this.uxMessage.Content = "Query Completed";
      }
      catch (Exception err)
      {
        this.uxRowCount.Content = "ERROR";

        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }
  }
}
