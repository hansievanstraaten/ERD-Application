﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ERD.Viewer.Common;
using ERD.Viewer.Models;
using GeneralExtensions;

namespace ERD.Viewer.Database.MsSql
{
  internal class MsDataAccess : IDataAccess
  {
    private readonly string connectionString = "Server={0};Database={1};User ID={2};Password={3};Trusted_Connection={4}";

    private SqlConnection connection;
    private DataConverters converter = new DataConverters();

    private bool _trustedConnection;
    private string _server;
    private string _database;
    private string _username;
    private string _password;

    public void Construct(DatabaseModel databaseModel)
    {
      this._server = databaseModel.ServerName;

      this._database = databaseModel.DatabaseName;

      this._trustedConnection = databaseModel.TrustedConnection;

      if (databaseModel.UserName.IsNullEmptyOrWhiteSpace() || databaseModel.Password.IsNullEmptyOrWhiteSpace())
      {
        this.GetPasswordOptions(databaseModel);
      }
      else
      {
        this._username = databaseModel.UserName;

        this._password = databaseModel.Password;
      }
    }

    public XDocument ExecuteQuery(string sqlQuery)
    {
      StringBuilder resultString = new StringBuilder();

      resultString.Append("<QueryResults>");

      try
      {
        IDataReader reader = this.ExecuteCommand(sqlQuery);
        
        Dictionary<int, string> columns = new Dictionary<int, string>();

        int fieldCount = reader.FieldCount;

        for (int x = 0; x < reader.FieldCount; x++)
        {
          columns.Add(x, reader.GetName(x));
        }

        int readIndex = 0;

        while (reader.Read())
        {
          StringBuilder line = new StringBuilder();

          resultString.Append($"<Row_{readIndex}>");

          for (int x = 0; x < fieldCount; x++)
          {
            object value = reader.GetValue(x);

            string dataTypeName = reader.GetDataTypeName(x);

            if (value is DBNull)
            {
              line.AppendLine($"<{columns[x]}></{columns[x]}>");

              continue;
            }

            line.AppendLine($"<{columns[x]}>{this.GetDataByType(value, dataTypeName)}</{columns[x]}>");
          }

          resultString.Append(line);

          resultString.Append($"</Row_{readIndex}>");

          readIndex++;
        }

        resultString.Append("</QueryResults>");
      }
      catch
      {
        throw;
      }
      finally
      {
        this.CloseConnection();
      }

      return XDocument.Parse(resultString.ToString());
    }

    public List<dynamic> ExecuteQueryDynamic(string sqlQuery)
    {
      List<dynamic> result = new List<dynamic>();
      
      try
      {
        IDataReader reader = this.ExecuteCommand(sqlQuery);
        
        Dictionary<int, string> columns = new Dictionary<int, string>();

        int fieldCount = reader.FieldCount;

        for (int x = 0; x < reader.FieldCount; x++)
        {
          string originalName = reader.GetName(x);

          columns.Add(x, originalName);
        }

        while (reader.Read())
        {
          dynamic readItem = new ExpandoObject();
          
          var readItemDic = readItem as IDictionary<string, object>;

          foreach (KeyValuePair<int, string> column in columns)
          {
            object value = reader.GetValue(column.Key);

            string dataTypeName = reader.GetDataTypeName(column.Key);

            if (value is DBNull)
            {
              readItemDic.Add(column.Value, null);
              
              continue;
            }

            string columnName = column.Value;

            if (readItemDic.ContainsKey(columnName))
            {
              columnName = $"{column.Value}_{column.Key}";
            }

            readItemDic.Add(columnName, this.GetDataByTypeDynamic(value, dataTypeName));
          }
            
          result.Add(readItem);
        }
      }
      catch
      {
        throw;
      }
      finally
      {
        this.CloseConnection();
      }

      return result;
    }
    
    public int ExecuteNonQuery(string sqlQuery)
    {
      try
      {
        this.connection = new SqlConnection(string.Format(this.connectionString, this._server, this._database, this._username, this._password, this._trustedConnection));

        this.connection.Open();

        SqlCommand cmd = this.connection.CreateCommand();

        cmd.CommandText = sqlQuery;

        return cmd.ExecuteNonQuery();
      }
      catch
      {
        if (this.connection != null)
        {
          if (this.connection.State == System.Data.ConnectionState.Open)
          {
            this.connection.Close();
          }

          this.connection = null;
        }

        throw;
      }
    }

    private void GetPasswordOptions(DatabaseModel databaseModel)
    {
      string sessionKey = $"{databaseModel.ServerName}||{databaseModel.DatabaseName}";

      if (Connections.SessionPasswords.ContainsKey(sessionKey))
      {
        this._username = Connections.SessionPasswords[sessionKey].UserName;

        this._password = Connections.SessionPasswords[sessionKey].Password;

        return;
      }

      UserNameAndPasswordModel model = new UserNameAndPasswordModel
      {
        UserName = databaseModel.UserName,
        Password = databaseModel.Password
      };

      UserNameAndPassword userPassword = new UserNameAndPassword(model);

      bool? result = userPassword.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        throw new ApplicationException("Operation canceled");
      }

      this._username = model.UserName;

      this._password = model.Password;

      Connections.SessionPasswords.Add(sessionKey, model);
    }
    
    private IDataReader ExecuteCommand(string sqlQuery)
    {
      try
      {
        this.connection = new SqlConnection(string.Format(this.connectionString, this._server, this._database, this._username, this._password, this._trustedConnection));

        this.connection.Open();

        SqlCommand cmd = this.connection.CreateCommand();

        cmd.CommandText = sqlQuery;

        return cmd.ExecuteReader();
      }
      catch
      {
        if (this.connection != null)
        {
          if (this.connection.State == System.Data.ConnectionState.Open)
          {
            this.connection.Close();
          }

          this.connection = null;
        }

        throw;
      }
    }

    private object GetDataByType(object data, string type)
    {
      switch (type)
      {
        case "timestamp":
            
          byte[] bytes = (byte[])data;

          return new XCData(Convert.ToString(converter.ConvertTimeStamp(bytes)));

        case "bit":
          return Convert.ToBoolean(data);

        case "uniqueidentifier":

          return new XCData(Convert.ToString(data));

        case "varbinary":
          if (string.IsNullOrEmpty(data.ToString()))
          {
            return data;
          }

          return new XCData(((byte[])data).ConvertBytesToString());

        case "datetime":
          DateTime date = DateTime.Now;

          DateTime.TryParse(Convert.ToString(data), out date);

          //return date;
          return new XCData(date.ToStringCulture(true));

        default: return new XCData(data.ParseToString());
        //return data;
      }
    }
    
    private object GetDataByTypeDynamic(object data, string type)
    {
      switch (type)
      {
        case "timestamp":
            
          byte[] bytes = (byte[])data;

          return converter.ConvertTimeStamp(bytes);

        case "bit":
          return Convert.ToBoolean(data);

        case "uniqueidentifier":

          return Convert.ToString(data);

        case "varbinary":
          
          return ((byte[])data).ConvertBytesToString();

        case "datetime":
          DateTime date = DateTime.Now;

          DateTime.TryParse(Convert.ToString(data), out date);

          //return date;
          return date;

        default: return data.ParseToString();
        //return data;
      }
    }
    
    private void CloseConnection()
    {
      if (this.connection != null)
      {
        if (this.connection.State == System.Data.ConnectionState.Open)
        {
          this.connection.Close();
        }

        this.connection = null;
      } 
    }
  }
}
