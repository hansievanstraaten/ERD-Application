using ERD.Common;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.Git.Models.GitModels
{
    public class GitRemoteSelect
    {
        [FieldInformation("Remote Name", IsRequired = true, Sort = 3)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("RemoteItems")]
        public string RemoteName { get; set; } = string.Empty;

        public DataItemModel[] RemoteItems
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();
                IReadOnlyList<GitRemoteInfo> remotes = ERDGit.GetRemotes(General.ProjectModel.FileDirectory);

                result.Add(new DataItemModel { DisplayValue = "<Select>", ItemKey = string.Empty });

                foreach (GitRemoteInfo item in remotes)
                {
                    result.Add(new DataItemModel { DisplayValue = item.Name, ItemKey = item.Name });
                }

                return result.ToArray();
            }
        }
    }
}
