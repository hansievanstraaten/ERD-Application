using System.Collections.Generic;
using WPF.Tools.ToolModels;

namespace ERD.Common
{
  public class IntegrityInvoker
  {
    public List<DataItemModel> GetSystemColumns()
    {
      return Integrity.GetSystemColumns();
    }
  }
}
