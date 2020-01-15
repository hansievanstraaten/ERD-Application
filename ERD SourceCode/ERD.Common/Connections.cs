﻿using ERD.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WPF.Tools.Functions;

namespace ERD.Common
{
  public sealed class Connections : IDisposable
  {
    private static DatabaseModel databaseModel;

    private static DatabaseModel defaultDatabaseModel;

    private static Connections connectionsInstance;

    private static object lockObject = new object();

    public static Connections Instance
    {
      get
      {
        if (Connections.connectionsInstance == null)
        {
          lock(Connections.lockObject)
          {
            Connections.connectionsInstance = new Connections();
          }
        }

        return Connections.connectionsInstance;
      }
    }

    ~Connections()
    {
      try
      {
        this.Dispose();
      }
      catch
      {

      }
    }

    public string DefaultConnectionName = "Default";

    public string DefaultDatabaseName
    {
      get
      {
        if (Connections.Instance.DefaultDatabaseModel == null)
        {
          return "Not Connected";
        }

        return Connections.Instance.DefaultDatabaseModel.DatabaseName;
      }
    }

    public string ConnectionName
    {
      get
      {
        if (Connections.Instance.DatabaseModel == null)
        {
          return Connections.Instance.DefaultDatabaseName;
        }

        return Connections.Instance.DatabaseModel.DatabaseName;
      }
    }

    public DatabaseModel DefaultDatabaseModel
    {
      get
      {
        return Connections.defaultDatabaseModel;
      }

      private set
      {
        Connections.defaultDatabaseModel = value;

        EventParser.ParseMessage(Connections.Instance.DatabaseModel, "DatabaseConnection", Connections.Instance.DefaultConnectionName);

        Connections.Instance.DatabaseModel = value;
      }
    }

    public DatabaseModel DatabaseModel
    { 
      get
      {
        if (Connections.databaseModel == null)
        {
          return Connections.Instance.DefaultDatabaseModel;
        }

        return Connections.databaseModel;
      }
      
      set
      {
        Connections.databaseModel = value;
      }
    }

    public Dictionary<string, UserNameAndPasswordModel> SessionPasswords = new Dictionary<string, UserNameAndPasswordModel>();

    public Dictionary<string, AltDatabaseModel> AlternativeModels = new Dictionary<string, AltDatabaseModel>();

    public void SetDefaultDatabaseModel(DatabaseModel model)
    {
      this.DefaultDatabaseModel = model;
    }

    public void SetConnection(MenuItem item)
    {
      if (item.Tag == null || item.Tag.ToString() == Connections.Instance.DefaultConnectionName)
      {
        Connections.Instance.DatabaseModel = Connections.Instance.DefaultDatabaseModel;

        EventParser.ParseMessage(Connections.Instance.DatabaseModel, "DatabaseConnection", Connections.Instance.DefaultConnectionName);

        return;
      }

      Connections.Instance.DatabaseModel = Connections.Instance.AlternativeModels[item.Tag.ToString()];

      EventParser.ParseMessage(Connections.Instance.DatabaseModel, "DatabaseConnection", item.Tag.ToString());
    }

    public void Dispose()
    {
    }
  }
}
