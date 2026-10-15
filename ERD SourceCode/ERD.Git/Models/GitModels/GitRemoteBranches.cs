using ERD.Common;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.Git.Models.GitModels
{
    public class GitRemoteBranches
    {
        [FieldInformation("Branch", IsRequired = true, Sort = 3)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("Branches")]
        public string BranchName { get; set; } = string.Empty;

        public DataItemModel[] Branches { get; set; } = new DataItemModel[0];
    }
}
