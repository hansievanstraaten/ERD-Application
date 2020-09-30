using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WPF.Tools.Functions;

namespace ERD.Common
{
    public sealed class Connections : IDisposable
    {
        public delegate void ConnectionChangedEvent(object sender, DatabaseModel model);

        public event ConnectionChangedEvent ConnectionChanged;

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
                    lock (Connections.lockObject)
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

        public bool IsDefaultConnection { get; private set; }

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

        public DatabaseModel GetConnection(string connectionName)
        {
            if (this.DefaultConnectionName == connectionName)
            {
                return this.DefaultDatabaseModel;
            }

            DatabaseModel result = null;

            foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in this.AlternativeModels)
            {
                if (connectionKey.Key == connectionName)
                {
                    result = connectionKey.Value.CopyToObject(new DatabaseModel()) as DatabaseModel;
                }
            }

            return result;
        }

        public Dictionary<string, UserNameAndPasswordModel> SessionPasswords = new Dictionary<string, UserNameAndPasswordModel>();

        public Dictionary<string, AltDatabaseModel> AlternativeModels = new Dictionary<string, AltDatabaseModel>();

        public void SetDefaultDatabaseModel(DatabaseModel model, bool raiseConnectionChangeEvent)
        {
            this.DefaultDatabaseModel = model;

            this.IsDefaultConnection = true;

            if (raiseConnectionChangeEvent)
            {
                Connections.Instance.ConnectionChanged?.Invoke(Connections.Instance, model);
            }
        }

        public void SetConnection(MenuItem item, bool raiseConnectionChangeEvent)
        {
            if (item.Tag == null || item.Tag.ToString() == Connections.Instance.DefaultConnectionName)
            {
                Connections.Instance.DatabaseModel = Connections.Instance.DefaultDatabaseModel;

                EventParser.ParseMessage(Connections.Instance.DatabaseModel, "DatabaseConnection", Connections.Instance.DefaultConnectionName);

                this.IsDefaultConnection = true;

                if (raiseConnectionChangeEvent)
                {
                    Connections.Instance.ConnectionChanged?.Invoke(Connections.Instance, Connections.Instance.DatabaseModel);
                }
                
                return;
            }

            Connections.Instance.DatabaseModel = Connections.Instance.AlternativeModels[item.Tag.ToString()];

            EventParser.ParseMessage(Connections.Instance.DatabaseModel, "DatabaseConnection", item.Tag.ToString());

            this.IsDefaultConnection = false;

            if (raiseConnectionChangeEvent)
            {
                Connections.Instance.ConnectionChanged?.Invoke(Connections.Instance, Connections.Instance.DatabaseModel);
            }
        }

        public void Dispose()
        {
        }
    }
}
