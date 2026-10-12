using ERD.Common;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.Git.Models
{
    public sealed class GitLocalBranches
    {
        [FieldInformation("Branch", IsRequired = true, Sort = 3)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("Branches")]
        public string BranchName { get; set; } = string.Empty;

        public DataItemModel[] Branches
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();
                IReadOnlyList<GitBranchInfo> branches = ERDGit.GetLocalBranches(General.ProjectModel.FileDirectory);

                result.Add(new DataItemModel { DisplayValue = "<Select>", ItemKey = string.Empty });

                foreach (GitBranchInfo item in branches)
                {
                    result.Add(new DataItemModel { DisplayValue = item.Name, ItemKey = item.Name });
                }

                return result.ToArray();
            }
        }
    }
}
