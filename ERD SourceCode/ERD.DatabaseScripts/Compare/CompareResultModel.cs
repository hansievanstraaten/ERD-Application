using ERD.Models;
using WPF.Tools.BaseClasses;
using GeneralExtensions;
using WPF.Tools.Attributes;

namespace ERD.DatabaseScripts.Compare
{
  [ModelNameAttribute("Discrepancy")]
  public class CompareResultModel : ModelsBase
  {
    private string objectName;
    private string message;
    private ObjectTypeEnum objectType;
    private ObjectActionEnum objectAction;

    [FieldInformationAttribute("Action", Sort = 0)]
    public ObjectActionEnum ObjectAction
    {
      get
      {
        return this.objectAction;
      }

      set
      {
        this.objectAction = value;

        base.OnPropertyChanged(() => this.ObjectAction);

        base.OnPropertyChanged(() => this.ObjectAction_Value);
      }
    }
    
    [FieldInformationAttribute("Object Type", IsReadOnly = true, Sort = 1)]
    public string ObjectType_Value
    {
      get
      {
        return this.ObjectType.GetDescriptionAttribute();
      }
    }

    public ObjectTypeEnum ObjectType 
    { 
      get
      {
        return this.objectType;
      }
      
      set
      {
        this.objectType = value;

        base.OnPropertyChanged(() => this.ObjectType);

        base.OnPropertyChanged(() => this.ObjectType_Value);
      }
    }

    public TableModel TableObject { get; set; }

    [FieldInformationAttribute("From Table", IsReadOnly = true, Sort = 2)]
    public string TableName
    {
      get
      {
        return this.TableObject.TableName;
      }
    }

    [FieldInformationAttribute("Table / Column", IsReadOnly = true, Sort = 3)]
    public string ObjectName 
    { 
      get
      {
        return this.objectName;
      }
      
      set
      {
        this.objectName = value;

        base.OnPropertyChanged(() => this.ObjectName);
      }
    }

    [FieldInformationAttribute("Discrepancy Message", IsReadOnly = true, Sort = 4)]
    public string Message 
    { 
      get
      {
        return this.message;
      }
      
      set
      {
        this.message = value;

        base.OnPropertyChanged(() => this.Message);
      }
    }
    
    public string ObjectAction_Value 
    { 
      get
      {
        return this.ObjectAction.GetDescriptionAttribute();
      }
    }

  }
}
