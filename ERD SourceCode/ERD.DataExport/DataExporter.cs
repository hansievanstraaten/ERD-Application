using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DataExport.Models;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ERD.DataExport
{
    public class DataExporter
    {
        public string Export(string connectionName, TableModel tableModel, char delimiter, string outputDirectory, int topX)
        {
            if (!Directory.Exists(outputDirectory))
            {
                throw new Exception("Output directory does not exist.");
            }

            DataAccess dataAccess = new DataAccess(Connections.Instance.GetConnection(connectionName));

            string selectQuery = SQLQueries.DatabaseQueries.BuildSelectTop(tableModel, topX);

            List<dynamic> resultList = dataAccess.ExecuteQueryDynamic(selectQuery);

            if (resultList.Count == 0)
            {
                throw new ApplicationException("Nothing to export.");
            }

            StringBuilder result = new StringBuilder();

            string[] columnsCollection = ((IDictionary<string, object>)resultList[0]).Keys.ToArray();

            int columnCount = columnsCollection.Length;

            // Build Header Row
            for (int x = 0; x < columnCount; ++x)
            {
                string column = columnsCollection[x];
                
                result.Append(column);

                if (x == columnCount - 1)
                {
                    result.AppendLine();
                }
                else
                {
                    result.Append(delimiter);
                }
            }
                  
            // Build Data
            foreach (IDictionary<string, object> dataRow in resultList)
            {
                object[] boxedRowValues = dataRow.Values.ToArray();

                for (int x = 0; x < columnCount; ++x)
                {
                    string columnValue = boxedRowValues[x].ParseToString();

                    if(columnValue.Contains(delimiter))
                    {
                        throw new ArgumentException("Data contains the delimiter. Please change the delimiter.");
                    }

                    result.Append(columnValue);

                    if (x == columnCount - 1)
                    {
                        result.AppendLine();
                    }
                    else
                    {
                        result.Append(delimiter);
                    }
                }
            }

            string resultFileName = $"{tableModel.TableName}_Data.csv";

            string filePath = Path.Combine(outputDirectory, resultFileName);

            File.WriteAllText(filePath, result.ToString());

            return filePath;
        }    
    }
}
