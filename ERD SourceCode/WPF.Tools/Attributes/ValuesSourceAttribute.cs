using System;

namespace WPF.Tools.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ValuesSourceAttribute : Attribute
  {
    private string propertyName;

    public ValuesSourceAttribute(string property)
    {
      this.propertyName = property;
    }

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }

      set
      {
        this.propertyName = value;
      }
    }
  }
}
