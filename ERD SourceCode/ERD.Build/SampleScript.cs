using ERD.Build.BuildEnums;
using ERD.Build.Models;
using System.Text;
using GeneralExtensions;
using System.Linq;
using ERD.Models;
using System.Collections.Generic;
using ERD.Build.DataMappings;
using System.Data;
using ERD.DatabaseScripts;


namespace ERD.Build
{
    public class SampleScript
    {
        private ErdCanvasModel ErdCanvas;

        private List<ErdCanvasModel> AllErdCanvasModels;

        private TableModel SelectedTable
        {
            get;
            set;
        }

        private TableModel SelectedReferencedTable { get; set; }

        private ColumnObjectModel SelectedColumn
        {
            get;
            set;
        }

        public LanguageOptionEnum LanguageOption
        {
            get;
            set;
        }

        public string BuildSingleFile(OptionSetupModel classOption)
        {
            if (classOption.BuildTypes.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            int minIndex = classOption.BuildTypes.Min(m => m.BuildTypeIndex);

            BuildTypeModel body = classOption.BuildTypes.FirstOrDefault(mi => mi.BuildTypeIndex == minIndex);

            result.Append(body.Code);

            this.ExecuteIfStatment(ref result, 0);

            return this.RemoveTrailingCharacters(result);
        }

        public string BuildSampleForeachCanvasScript(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdModels, OptionSetupModel classOption)
        {
            if (classOption.BuildTypes.Length == 0)
            {
                return string.Empty;
            }

            this.ErdCanvas = sampleCanvas;

            this.AllErdCanvasModels = allErdModels;

            StringBuilder result = new StringBuilder();

            int minIndex = classOption.BuildTypes.Min(m => m.BuildTypeIndex);

            BuildTypeModel body = classOption.BuildTypes.FirstOrDefault(mi => mi.BuildTypeIndex == minIndex);

            result.Append(this.ReplaceParameters(body.Code));

            this.BuildTypeIteration(ref result, classOption.BuildTypes);

            this.ExecuteIfStatment(ref result, 0);

            return this.RemoveTrailingCharacters(result);
        }

        public string BuildSampleForeachTableScript(ErdCanvasModel sampleCanvas, List<ErdCanvasModel> allErdModels, TableModel table, OptionSetupModel classOption)
        {
            if (classOption.BuildTypes.Length == 0)
            {
                return string.Empty;
            }

            this.ErdCanvas = sampleCanvas;

            this.AllErdCanvasModels = allErdModels;

            this.SelectedTable = table;

            StringBuilder result = new StringBuilder();

            int minIndex = classOption.BuildTypes.Min(m => m.BuildTypeIndex);

            BuildTypeModel body = classOption.BuildTypes.FirstOrDefault(mi => mi.BuildTypeIndex == minIndex);

            result.Append(this.ReplaceParameters(body.Code));

            this.BuildTypeIteration(ref result, classOption.BuildTypes);

            this.ExecuteIfStatment(ref result, 0);

            return this.RemoveTrailingCharacters(result);
        }

        public void ExecuteIfStatment(ref StringBuilder result, int passedIndex)
        {
            string startIfString = "[[IF{{"; //0}}]]";

            string endIfString = "}}]]";

            int startIfIndex = result.IndexOf(startIfString, passedIndex);

            if (startIfIndex <= 0)
            {
                return;
            }

            int endIfIndex = result.IndexOf(endIfString, startIfIndex) + endIfString.Length;

            int indexLength = endIfIndex - startIfIndex;

            string checkValue = result.Substring(startIfIndex, indexLength);

            string[] checkValues = checkValue
                .Replace(startIfString, string.Empty)
                .Replace(endIfString, string.Empty)
                .Split(new string[] { "==", "then" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (checkValues[0].Trim() == checkValues[1].Trim())
            {
                result.Replace(checkValue, checkValues[2], startIfIndex, indexLength);
            }
            else
            {
                result.Replace(checkValue, string.Empty, startIfIndex, indexLength);
            }

            this.ExecuteIfStatment(ref result, startIfIndex);
        }

        public string RemoveTrailingCharacters(StringBuilder value)
        {
            string result = value.ToString();

            int lastIndex = result.LastIndexOf("[[?");

            if (lastIndex <= 0)
            {
                return value.ToString();
            }

            while (lastIndex > 0)
            {
                // Get the value to replace
                StringBuilder getString = new StringBuilder("[[?");

                StringBuilder replaceString = new StringBuilder();

                int endIndex = lastIndex + 3;

                #region GET THE ITEM TO REMOVE AND REPLACE

                while (result.Substring(endIndex, 2) != "]]")
                {
                    getString.Append(result.Substring(endIndex, 1));

                    if (result.Substring(endIndex, 1) != "]")
                    {
                        replaceString.Append(result.Substring(endIndex, 1));
                    }

                    ++endIndex;
                }

                getString.Append("]]");

                #endregion

                result = result.Remove(lastIndex, ((endIndex + 2) - lastIndex))
                    .Replace(getString.ToString(), replaceString.ToString());

                lastIndex = result.LastIndexOf("[[?");
            }

            return result;
        }

        private void BuildTypeIteration(ref StringBuilder result, BuildTypeModel[] buildTypesArray)
        {
            for (int x = 1; x < buildTypesArray.Length; ++x)
            {
                BuildTypeModel buildType = buildTypesArray[x];

                string buildTypeParameter = $"[[{buildType.BuildTypeIndex}]]";

                StringBuilder injectionText = new StringBuilder();

                switch (buildType.RepeatType)
                {
                    case RepeatTypeEnum.ForeachTableInCanvas:

                        List<BuildTypeModel> nextTypesList = new List<BuildTypeModel>();

                        int xIndex = x;

                        while (xIndex < buildTypesArray.Length)
                        {
                            nextTypesList.Add(buildTypesArray[xIndex]);

                            ++xIndex;
                        }

                        for (int table = 0; table < this.ErdCanvas.SegmentTables.Count; ++table)
                        {
                            this.SelectedTable = this.ErdCanvas.SegmentTables[table];

                            injectionText.Append(this.ReplaceParameters(buildType.Code));

                            this.BuildTypeIteration(ref injectionText, nextTypesList.ToArray());
                        }

                        break;

                    case RepeatTypeEnum.ForeachTableInProject:

                        List<BuildTypeModel> nextTypesListB = new List<BuildTypeModel>();

                        int xIndexB = x;

                        while (xIndexB < buildTypesArray.Length)
                        {
                            nextTypesListB.Add(buildTypesArray[xIndexB]);

                            ++xIndexB;
                        }

                        foreach (ErdCanvasModel canvas in this.AllErdCanvasModels)
                        {
                            foreach (TableModel table in canvas.SegmentTables)
                            {
                                this.SelectedTable = table;

                                injectionText.Append(this.ReplaceParameters(buildType.Code));

                                this.BuildTypeIteration(ref injectionText, nextTypesListB.ToArray());
                            }
                        }

                        break;

                    default:

                        this.BuildTypeSwitch(buildType, ref injectionText);

                        break;
                }

                result.Replace(buildTypeParameter, this.RemoveTrailingCharacters(injectionText));
            }
        }

        private void BuildTypeSwitch(BuildTypeModel buildType, ref StringBuilder injectionText)
        {
            switch (buildType.RepeatType)
            {
                case RepeatTypeEnum.ForeachPrimaryKeyInTable:

                    #region FOREACH PRIMARY KEY IN TABLE  

                    ColumnObjectModel[] keyColumnsArray = this.SelectedTable.Columns.Where(pk => pk.InPrimaryKey).ToArray();

                    for (int keyCol = 0; keyCol < keyColumnsArray.Length; ++keyCol)
                    {
                        this.SelectedColumn = keyColumnsArray[keyCol];

                        injectionText.Append(this.ReplaceParameters(buildType.Code));
                    }

                    break;

                #endregion

                case RepeatTypeEnum.ForeachForeignKeyInTable:

                    #region FOREACH FOREIGN KEY IN TABLE  

                    ColumnObjectModel[] foreignKeyColumnsArray = this.SelectedTable.Columns.Where(pk => pk.IsForeignkey).ToArray();

                    for (int keyCol = 0; keyCol < foreignKeyColumnsArray.Length; ++keyCol)
                    {
                        this.SelectedColumn = foreignKeyColumnsArray[keyCol];

                        injectionText.Append(this.ReplaceParameters(buildType.Code));
                    }

                    break;

                #endregion


                case RepeatTypeEnum.ForeachNonColumnInTable:

                    #region FOREACH NON-KEY COLUMN IN TABLE

                    ColumnObjectModel[] nonKeyColumnsArray = this.SelectedTable.Columns.Where(nk => !nk.InPrimaryKey && !nk.IsForeignkey).ToArray();

                    for (int keyCol = 0; keyCol < nonKeyColumnsArray.Length; ++keyCol)
                    {
                        this.SelectedColumn = nonKeyColumnsArray[keyCol];

                        injectionText.Append(this.ReplaceParameters(buildType.Code));
                    }

                    break;

                #endregion

                case RepeatTypeEnum.ForeachColumnInTable:

                    #region FOREACH COLUMN IN TABLE

                    foreach (ColumnObjectModel column in this.SelectedTable.Columns)
                    {
                        this.SelectedColumn = column;

                        injectionText.Append(this.ReplaceParameters(buildType.Code));
                    }

                    #endregion

                    break;

                case RepeatTypeEnum.ForeachReferencedTable:

                    #region FOREACH REFERENCED TABLE

                    Dictionary<string, TableModel> referenceTables = new Dictionary<string, TableModel>();

                    foreach (ErdCanvasModel canvas in this.AllErdCanvasModels)
                    {
                        foreach (TableModel table in canvas.SegmentTables)
                        {
                            if (table.Columns.Any(col => col.ForeignKeyTable == this.SelectedTable.TableName)
                                                         && !referenceTables.ContainsKey(table.TableName))
                            {
                                referenceTables.Add(table.TableName, table);
                            }
                        }
                    }

                    foreach(TableModel table in referenceTables.Values)
                    {
                        this.SelectedReferencedTable = table;

                        injectionText.Append(this.ReplaceParameters(buildType.Code));
                    }

                    #endregion

                    break;                

                case RepeatTypeEnum.Once:
                default:
                    injectionText.Append(this.ReplaceParameters(buildType.Code));

                    break;
            }
        }

        private string ReplaceParameters(string rawText)
        {
            if (rawText.IsNullEmptyOrWhiteSpace())
            {
                return rawText;
            }

            rawText = rawText
                .Replace("[[CanvasName]]", this.CanvasName)

                .Replace("[[TableName]]", this.TableName)
                .Replace("[[TableFriendlyName]]", this.TableFriendlyName)
                .Replace("[[TablePluralName]]", this.TablePluraName)
                .Replace("[[TableDescription]]", this.TableDescription)

                .Replace("[[PrimaryKey]]", this.PrimaryKeyColumnName)
                .Replace("[[ForeignKey]]", this.ForeignKeyColumnName)
                .Replace("[[NoKeyColumn]]", this.NoKeyColumnColumnName)

                .Replace("[[ForeignTableName]]", this.ForeignTableName)
                .Replace("[[ForeignTableFriendlyName]]", this.ForeignTableFriendlyName)
                .Replace("[[ForeignTablePluralName]]", this.ForeignTablePluralName)
                .Replace("[[ForeignTableDescription]]", this.ForeignTableDescription)
                
                .Replace("[[ColumnName]]", this.ColumnName)
                .Replace("[[ColumnFriendlyName]]", this.ColumnFriendlyName)
                .Replace("[[ColumnDescription]]", this.ColumnDescription)

                .Replace("[[DataType]]", this.ColumnDataType)
                .Replace("[[DatabaseColumnType]]", this.SqlDataType)
                .Replace("[[DatabaseColumnLength]]", this.FieldLength)
                .Replace("[[DatabaseNullableType]]", this.SqlNullableDataType) 
                .Replace("[[DatabaseColumnTypeIDENTITY]]", this.SqlColumnIDENTITY);

            return rawText;
        }

        private string CanvasName
        {
            get
            {
                return this.ErdCanvas.ModelSegmentControlName;
            }
        }

        private string TableName
        {
            get
            {
                if (this.SelectedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedTable.TableName;
            }
        }
        
        private string TablePluraName
        {
            get
            {
                if (this.SelectedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedTable.PluralName;
            }
        }

        private string TableFriendlyName
        {
            get
            {
                if (this.SelectedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedTable.FriendlyName;
            }
        }

        private string TableDescription
        {
            get
            {
                if (this.SelectedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedTable.Description;
            }
        }
        
        private string ForeignTableName
        {
            get
            {
                if (this.SelectedReferencedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedReferencedTable.TableName;
            }
        }

        private string ForeignTableFriendlyName
        {
          get
          {
            if (this.SelectedReferencedTable == null)
            {
              return string.Empty;
            }

            return this.SelectedReferencedTable.FriendlyName;
          }
        }

        private string ForeignTablePluralName
        {
            get
            {
                if (this.SelectedReferencedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedReferencedTable.PluralName;
            }
        }

        private string ForeignTableDescription
        {
            get
            {
                if (this.SelectedReferencedTable == null)
                {
                    return string.Empty;
                }

                return this.SelectedReferencedTable.Description;
            }
        }

        private string PrimaryKeyColumnName
        {
            get
            {
                if (this.SelectedColumn == null || !this.SelectedColumn.InPrimaryKey)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.ColumnName;
            }
        }

        private string NoKeyColumnColumnName
        {
            get
            {
                if (this.SelectedColumn == null
                    || this.SelectedColumn.InPrimaryKey
                    || this.SelectedColumn.IsForeignkey)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.ColumnName;
            }
        }

        private string ForeignKeyColumnName
        {
            get
            {
                if (this.SelectedColumn == null || !this.SelectedColumn.IsForeignkey)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.ColumnName;
            }
        }

        private string ColumnName
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.ColumnName;
            }
        }

        private string ColumnFriendlyName
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.FriendlyName;
            }
        }

        private string ColumnDescription
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                return this.SelectedColumn.Description;
            }
        }

        private string ColumnDataType
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                return DataTypesConverter.GetStringDataType(this.SelectedColumn, this.LanguageOption);
            }
        }
    
        private string SqlDataType
        {
            get
            {
                return Scripting.DatabaseDataType(this.SelectedColumn);
            }
        }

        private string FieldLength
        {
            get
            {
                return Scripting.DatafieldLength(this.SelectedColumn);
            }
        }

        private string SqlNullableDataType
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                if (this.SelectedColumn.AllowNulls)
                {
                    return "NULL";
                }

                return "NOT NULL";
            }
        }

        private string SqlColumnIDENTITY
        {
            get
            {
                if (this.SelectedColumn == null)
                {
                    return "false";
                }

                if (this.SelectedColumn.DataType.EndsWith(" IDENTITY"))
                {
                    return "true";
                }

                return "false";
            }
        }
    }
}
