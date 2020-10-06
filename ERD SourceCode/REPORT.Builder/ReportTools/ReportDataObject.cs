using GeneralExtensions;
using REPORT.Data.Models;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Data Object")]
    public class ReportDataObject : LabelBase
    {
        private ReportColumnModel columnModel;

        private bool suppressed;

        public ReportDataObject()
        {
            //this.ColumnModel = new ReportColumnModel();

            this.DataContext = this;

            base.Loaded += this.ReportLabel_Loaded;
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportDataObject"));
                result.Add(new XAttribute("ObjectTable", this.ColumnModel.TableName));
                result.Add(new XAttribute("ObjectColumn", this.ColumnModel.ColumnName));
                result.Add(new XAttribute("Suppressed", this.Suppressed));
                result.Add(new XAttribute("ColumnModel", this.ColumnModel.ZipFile().ConvertBytesToString()));

                return result;
            }

            set
            {
                foreach (XAttribute item in value.Attributes())
                {
                    if (item.Name.LocalName == "ColumnModel")
                    {
                        this.ColumnModel = item.Value.ConvertStringToBytes().UnzipFile() as ReportColumnModel;

                        continue;
                    }

                    this.SetPropertyValue(item.Name.LocalName, item.Value);
                }
            }
        }

        [FieldInformation("Is Suppressed", Sort = 500)]
        public bool Suppressed
        {
            get
            {
                return this.suppressed;
            }

            set
            {
                this.suppressed = value;

                if  (this.IsDesignMode && value)
                {
                    this.Foreground = Brushes.Gray;
                }
                else if (value)
                {
                    this.Visibility = Visibility.Hidden;
                }
            }
        }

        public ReportColumnModel ColumnModel
        {
            get
            {
                return this.columnModel == null ? new ReportColumnModel() : this.columnModel;
            }

            set
            {
                this.columnModel = value;

                base.Text = value.ColumnName;
            }
        }

        private void ReportLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsDesignMode)
            {
                this.Background = Brushes.WhiteSmoke;
            }
            else
            {
                this.Background = Brushes.Transparent;
            }
        }    
    }
}
