using ERD.Build.BuildEnums;
using ERD.Build.Models;
using System.Text;
using GeneralExtensions;
using System.Linq;
using ERD.Models;
using System.Collections.Generic;
using ERD.Build.DataMappings;
using System.Data;

namespace ERD.Build
{
    public class SampleScript
    {
        private ErdCanvasModel ErdCanvas;

        private TableModel SelectedTable
        {
            get;
            set;
        }

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

            //this.BuildTypeIteration(ref result, classOption.BuildTypes);

            return this.RemoveTrailingCharacters(result);
        }

        public string BuildSampleForeachCanvasScript(ErdCanvasModel sampleCanvas, OptionSetupModel classOption)
        {
            if (classOption.BuildTypes.Length == 0)
            {
                return string.Empty;
            }

            this.ErdCanvas = sampleCanvas;

            StringBuilder result = new StringBuilder();

            int minIndex = classOption.BuildTypes.Min(m => m.BuildTypeIndex);

            BuildTypeModel body = classOption.BuildTypes.FirstOrDefault(mi => mi.BuildTypeIndex == minIndex);

            result.Append(this.ReplaceParameters(body.Code));

            this.BuildTypeIteration(ref result, classOption.BuildTypes);

            return this.RemoveTrailingCharacters(result);
        }

        public string BuildSampleForeachTableScript(ErdCanvasModel sampleCanvas, TableModel table, OptionSetupModel classOption)
        {
            if (classOption.BuildTypes.Length == 0)
            {
                return string.Empty;
            }

            this.ErdCanvas = sampleCanvas;

            this.SelectedTable = table;

            StringBuilder result = new StringBuilder();

            int minIndex = classOption.BuildTypes.Min(m => m.BuildTypeIndex);

            BuildTypeModel body = classOption.BuildTypes.FirstOrDefault(mi => mi.BuildTypeIndex == minIndex);

            result.Append(this.ReplaceParameters(body.Code));

            this.BuildTypeIteration(ref result, classOption.BuildTypes);

            return this.RemoveTrailingCharacters(result);
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
                .Replace("[[PrimaryKey]]", this.PrimaryKeyColumnName)
                .Replace("[[ForeignKey]]", this.ForeignKeyColumnName)
                .Replace("[[NoKeyColumn]]", this.NoKeyColumnColumnName)
                .Replace("[[ColumnName]]", this.ColumnName)
                .Replace("[[ColumnFriendlyName]]", this.ColumnFriendlyName)
                .Replace("[[ColumnDescription]]", this.ColumnDescription)
                .Replace("[[DataType]]", this.ColumnDataType)
                .Replace("[[DatabaseDataType]]", this.SqlDataType)
                .Replace("[[DatabaseNullableType]]", this.SqlNullableDataType);

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
                if (this.SelectedColumn == null)
                {
                    return string.Empty;
                }

                switch (this.SelectedColumn.SqlDataType)
                {
                    case SqlDbType.VarChar:
                        if (this.SelectedColumn.MaxLength == 8016)
                        {
                            return "sql_variant";
                        }

                        goto default;
                        
                    case SqlDbType.Variant:
                        return "numeric";

                    default: 
                        return $"{this.SelectedColumn.SqlDataType} {this.FieldLength(this.SelectedColumn)}";
                }
            }
        }

        private string FieldLength(ColumnObjectModel column)
        {
            switch (column.SqlDataType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Time:
                    return $"({column.MaxLength})";

                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                    return $"({column.Scale})";

                case SqlDbType.VarChar:

                    if (column.MaxLength == 8016)
                    {
                        return string.Empty;
                    }

                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";

                case SqlDbType.VarBinary:
                case SqlDbType.NVarChar:
                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";


                case SqlDbType.Decimal:
                case SqlDbType.Variant:
                    return $"({column.Precision}, {column.Scale})";

                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.DateTime:
                case SqlDbType.Float:
                case SqlDbType.Image:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.NText:
                case SqlDbType.Real:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.SmallDateTime:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Text:
                case SqlDbType.Timestamp:
                case SqlDbType.TinyInt:
                case SqlDbType.Xml:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Date:
                default:
                    return string.Empty;
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
    }
}
