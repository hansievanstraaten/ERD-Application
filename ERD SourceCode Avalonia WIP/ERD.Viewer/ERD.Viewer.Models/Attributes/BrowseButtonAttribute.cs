using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BrowseButtonAttribute : Attribute
    {
        private string itemKey;

        private string itemText;

        private string resourceImage;

        public BrowseButtonAttribute(string buttonKey, string buttonText, string resourceImageName)
        {
            itemKey = buttonKey;

            itemText = buttonText;

            resourceImage = resourceImageName;
        }

        public string ButtonKey
        {
            get
            {
                return itemKey;
            }
        }

        public string ButtonText
        {
            get
            {
                return itemText;
            }
        }

        public string ResourceImage
        {
            get
            {
                return resourceImage;
            }

            set
            {
                resourceImage = value;
            }
        }
    }
}
