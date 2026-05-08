/* PSEUDOCODE / PLAN (detailed)
 - Replace MS SQL client usage with Npgsql (Postgres) equivalents.
 - Build a helper GetConnectionString() that returns a Postgres connection string.
   - If _trustedConnection is true, use Integrated Security (GSS/SSPI) in the connection string and ignore username/password.
   - Otherwise include Host, Database, Username, Password.
 - Replace SqlConnection/SqlCommand/SqlException with NpgsqlConnection/NpgsqlCommand/NpgsqlException.
 - Update TestConnection, ExecuteCommand, ExecuteNonQuery to use Npgsql types and GetConnectionString().
 - Keep IDataReader usage (NpgsqlDataReader implements IDataReader).
 - Update data type name handling in GetDataByType and GetDataByTypeDynamic to map common Postgres types:
   - "bytea" => binary data, convert to string via existing ConvertBytesToString
   - "boolean" / "bool" => convert to bool (XML) / 1 or 0 (dynamic) consistent with prior behavior
   - "uuid" => treat like uniqueidentifier (string)
   - "timestamp", "timestamp without time zone", "timestamp with time zone" => parse/return DateTime (XML wraps with XCData using ToStringCulture(true))
   - default => fallback to existing ParseToString / XCData behavior
 - Ensure CloseConnection closes and nulls NpgsqlConnection.
 - Preserve the rest of logic (dynamic results, XML building, password prompt flow).
*/

using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Xml.Linq;
using WPF.Tools.BaseClasses;
using Npgsql;

namespace ERD.DatabaseScripts.Postgres
{
    internal class PgDataAccess : IDataAccess
    {
        // Note: For Postgres we construct connection string dynamically.
        private NpgsqlConnection connection;
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

        public void Construct(Dictionary<string, string> setupValues)
        {
            this._server = setupValues["ServerName"];

            this._database = setupValues["DatabaseName"];

            this._username = setupValues["UserName"];

            this._password = setupValues["Password"];

            this._trustedConnection = setupValues["TrustedConnection"].ToBool();
        }

        private string GetConnectionString()
        {
            // Host and Database are required. Support Integrated Security for trusted connections.
            if (this._trustedConnection)
            {
                // Integrated Security for Npgsql (GSS/SSPI) - username/password ignored.
                return $"Host={this._server};Database={this._database};Integrated Security=true";
            }

            // Username/Password auth
            return $"Host={this._server};Database={this._database};Username={this._username};Password={this._password}";
        }

        public bool TestConnection()
        {
            try
            {
                this.connection = new NpgsqlConnection(this.GetConnectionString());

                this.connection.Open();

                return true;
            }
            catch (NpgsqlException)
            {
                return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public XDocument ExecuteQuery(string sqlQuery, int commandTimeout = 30)
        {
            StringBuilder resultString = new StringBuilder();

            resultString.Append("<QueryResults>");

            try
            {
                IDataReader reader = this.ExecuteCommand(sqlQuery, commandTimeout);

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
                            line.AppendLine($"<{columns[x].ToUpper()}></{columns[x].ToUpper()}>");

                            continue;
                        }

                        line.AppendLine($"<{columns[x].ToUpper()}>{this.GetDataByType(value, dataTypeName)}</{columns[x].ToUpper()}>");
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

        public List<dynamic> ExecuteQueryDynamic(string sqlQuery, int commandTimeout = 30)
        {
            List<dynamic> result = new List<dynamic>();

            try
            {
                IDataReader reader = this.ExecuteCommand(sqlQuery, commandTimeout);

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

        public int ExecuteNonQuery(string sqlQuery, int commandTimeout = 30)
        {
            try
            {
                this.connection = new NpgsqlConnection(this.GetConnectionString());

                this.connection.Open();

                using (var cmd = this.connection.CreateCommand())
                {
                    cmd.CommandTimeout = commandTimeout;
                    cmd.CommandText = sqlQuery;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                if (this.connection != null)
                {
                    if (this.connection.State == ConnectionState.Open)
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

            if (Connections.Instance.SessionPasswords.ContainsKey(sessionKey))
            {
                this._username = Connections.Instance.SessionPasswords[sessionKey].UserName;

                this._password = Connections.Instance.SessionPasswords[sessionKey].Password;

                return;
            }

            UserNameAndPasswordModel model = new UserNameAndPasswordModel
            {
                UserName = databaseModel.UserName,
                Password = databaseModel.Password
            };

            Type controlType = Type.GetType("ERD.Viewer.Database.UserNameAndPassword,ERD.Viewer");

            WindowBase userPassword = Activator.CreateInstance(controlType, new object[] { model }) as WindowBase;

            bool? result = this.InvokeMethod(userPassword, "ShowDialog", null).To<bool?>();

            if (!result.HasValue || !result.Value)
            {
                throw new ApplicationException("Operation canceled");
            }

            this._username = model.UserName;

            this._password = model.Password;

            Connections.Instance.SessionPasswords.Add(sessionKey, model);
        }

        private IDataReader ExecuteCommand(string sqlQuery, int commandTimeout)
        {
            try
            {
                this.connection = new NpgsqlConnection(this.GetConnectionString());

                this.connection.Open();

                var cmd = this.connection.CreateCommand();

                cmd.CommandTimeout = commandTimeout;

                cmd.CommandText = sqlQuery;

                return cmd.ExecuteReader();
            }
            catch
            {
                this.CloseConnection();

                throw;
            }
        }

        private object GetDataByType(object data, string type)
        {
            string typeLower = (type ?? string.Empty).ToLowerInvariant();

            // Handle common Postgres type names and fall back to default
            if (typeLower.Contains("bytea"))
            {
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                {
                    return data;
                }

                return new XCData(((byte[])data).ConvertBytesToString());
            }

            if (typeLower.Contains("boolean") || typeLower == "bool")
            {
                return Convert.ToBoolean(data);
            }

            if (typeLower.Contains("uuid"))
            {
                return new XCData(Convert.ToString(data));
            }

            if (typeLower.Contains("timestamp") || typeLower.Contains("date") || typeLower.Contains("time"))
            {
                DateTime date = DateTime.Now;

                DateTime.TryParse(Convert.ToString(data), out date);

                return new XCData(date.ToStringCulture(true));
            }

            // Fallback: convert to string wrapped in XCData
            return new XCData(data.ParseToString());
        }

        private object GetDataByTypeDynamic(object data, string type)
        {
            string typeLower = (type ?? string.Empty).ToLowerInvariant();

            if (typeLower.Contains("bytea"))
            {
                return ((byte[])data).ConvertBytesToString();
            }

            if (typeLower.Contains("boolean") || typeLower == "bool")
            {
                return Convert.ToBoolean(data) ? 1 : 0;
            }

            if (typeLower.Contains("uuid"))
            {
                return Convert.ToString(data);
            }

            if (typeLower.Contains("timestamp") || typeLower.Contains("date") || typeLower.Contains("time"))
            {
                DateTime date = DateTime.Now;

                DateTime.TryParse(Convert.ToString(data), out date);

                return date;
            }

            return data.ParseToString();
        }

        private void CloseConnection()
        {
            if (this.connection != null)
            {
                if (this.connection.State == ConnectionState.Open)
                {
                    this.connection.Close();
                }

                this.connection = null;
            }
        }
    }
}