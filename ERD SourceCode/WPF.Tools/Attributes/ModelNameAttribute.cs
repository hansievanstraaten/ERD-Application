using System;

namespace WPF.Tools.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ModelNameAttribute : Attribute
  {
    private bool allowCollapse;

    private string name;

    public ModelNameAttribute(string modelName, bool allowHeaderCollapse = false)
    {
      this.name = modelName;

      this.allowCollapse = allowHeaderCollapse;
    }

    public bool AllowHeaderCollapse
    {
      get
      {
        return this.allowCollapse;
      }

      set
      {
        this.allowCollapse = value;
      }
    }

    public string ModelName
    {
      get
      {
        return this.name;
      }

      set
      {
        this.name = value;
      }
    }
  }
}
