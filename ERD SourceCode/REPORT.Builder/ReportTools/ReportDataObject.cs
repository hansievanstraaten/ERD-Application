﻿using GeneralExtensions;
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

        public ReportDataObject()
        {
            this.ColumnModel = new ReportColumnModel();

            this.DataContext = this;

            base.Loaded += this.ReportLabel_Loaded;
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportDataObject"));
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

        public ReportColumnModel ColumnModel
        {
            get
            {
                return this.columnModel;
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
