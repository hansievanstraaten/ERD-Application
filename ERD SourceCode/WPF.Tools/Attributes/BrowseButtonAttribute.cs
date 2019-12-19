using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Tools.Attributes
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class BrowseButtonAttribute : Attribute
  {
    private string itemKey;

    private string itemText;

    private string resourceImage;

    public BrowseButtonAttribute(string buttonKey, string buttonText, string resourceImageName)
    {
      this.itemKey = buttonKey;

      this.itemText = buttonText;

      this.resourceImage = resourceImageName;
    }

    public string ButtonKey
    {
      get
      {
        return this.itemKey;
      }
    }

    public string ButtonText
    {
      get
      {
        return this.itemText;
      }
    }

    public string ResourceImage
    {
      get
      {
        return this.resourceImage;
      }

      set
      {
        this.resourceImage = value;
      }
    }
  }
}
