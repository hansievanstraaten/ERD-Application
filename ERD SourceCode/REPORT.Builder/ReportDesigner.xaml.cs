using GeneralExtensions;
using REPORT.Builder.ReportComponents;
using REPORT.Builder.ReportTools;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Linq;
using ViSo.Dialogs.TextEditor;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner : UserControlBase
    {
        private ReportTypeEnum reportDesignType;

        private ReportMasterModel reportMaster;

        public ReportDesigner(ReportMasterModel masterModel)
        {
            this.InitializeComponent();

            this.InitializeToolsStack();

            this.SizeChanged += this.ReportDesigner_SizeChanged;

            this.Loaded += this.ReportDesigner_Loaded;

            this.reportDesignType = (ReportTypeEnum)masterModel.ReportTypeEnum;

            this.ReportMaster = masterModel;

            this.uxReportMasterModel.Items.Add(this.ReportMaster);

            if (this.ReportMaster.MasterReport_Id == 0)
            {   // New Report
                this.ReportMaster.ReportXMLVersion = 1;

                this.InitializeReportSections();
            }
            else
            {
                ReportTablesRepository repo = new ReportTablesRepository();

                this.ReportMaster.ReportXMLVersion = repo.GetReportXMLVersion(masterModel.MasterReport_Id);

                ReportXMLModel reportXML = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, masterModel.MasterReport_Id);

                string reportXml = reportXML.BinaryXML.UnzipFile().ParseToString();

                XDocument report = XDocument.Parse(reportXml);

                this.reportDesignType = (ReportTypeEnum)report.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value.ToInt32();

                foreach (XElement sectionElement in report.Root.Element("ReportSettings").Elements("ReportSection").OrderBy(si => si.Attribute("SectionIndex").Value.ToInt32()))
                {
                    ReportSection section = new ReportSection { IsDesignMode = true };

                    section.SectionXml = sectionElement;

                    this.uxReportSections.Children.Add(section);

                    section.ReportObjectSelected += this.ReportObject_Selected;
                }
            }

            double pageWidth = ((ReportSection)this.uxReportSections.Children[0]).PageSize.Width.Value;

            this.uxReportSections.MinWidth = pageWidth + 200;
        }

        public ReportMasterModel ReportMaster
        {
            get
            {
                return this.reportMaster;
            }

            set
            {
                this.reportMaster = value;
            }
        }

        public bool Save()
        {
            try
            {
                if (this.uxReportMasterModel.HasValidationError)
                {
                    return false;
                }

                ReportTablesRepository repo = new ReportTablesRepository();

                repo.UpdateReportMaster(this.ReportMaster);

                XDocument result = new XDocument();

                XElement root = new XElement("Root");

                XElement report = new XElement("ReportSettings");

                report.Add(new XAttribute("ReportTypeEnum", (int)this.reportDesignType));

                foreach(ReportSection section in this.uxReportSections.Children)
                {
                    report.Add(section.SectionXml);
                }

                root.Add(report);

                result.Add(root);

                ReportXMLModel reportXml = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, this.ReportMaster.MasterReport_Id);

                if (reportXml == null)
                {
                    reportXml = new ReportXMLModel
                    {
                        MasterReport_Id = this.ReportMaster.MasterReport_Id,
                        ReportXMLVersion = this.ReportMaster.ReportXMLVersion,
                        PrintCount = 0
                    };
                }

                reportXml.BinaryXML = result.ToString().ZipFile();

                repo.UpdateReportXML(reportXml);
                //result.Save("C:\\temp\\TestReport.xml");

                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
        }

        private void ReportDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            //this.uxHorizontalRuler.Refresh(((ReportSection)this.uxReportSections.Children[0]).PageSize.Width.Value, 5);
        }

        private void ReportDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.uxHorizontalRuler.Refresh(((ReportSection)this.uxReportSections.Children[0]).PageSize.Width.Value, 20);
        }

        private void ReportObject_Selected(object sender)
        {
            try
            {
                this.uxProperties.Items.Clear();

                if (sender == null)
                {
                    return;
                }

                this.uxProperties.Items.Add(sender);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportMasterDescription_Browse(object sender, string buttonKey)
        {
            try
            {
                if (TextEditing.ShowDialog("Description", this.ReportMaster.DescriptionText).IsFalse())
                {
                    return;
                }

                this.ReportMaster.DescriptionText = TextEditing.Text;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void InitializeReportSections()
        {
            this.uxReportSections.Children.Clear();

            switch(this.reportDesignType)
            {
                case ReportTypeEnum.CoverPage:
                case ReportTypeEnum.FinalPage:

                    this.uxReportSections.Children.Add(new ReportSection
                    {
                        SectionTitle = this.reportDesignType.GetDescriptionAttribute(),
                        SectionIndex = 0,
                        SectionType = SectionTypeEnum.Page,
                        IsDesignMode = true
                    }); ;

                    break;

                case ReportTypeEnum.ReportContent:
                    break;

                case ReportTypeEnum.PageHeaderAndFooter:

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionTitle = "Page Header", 
                        SectionIndex = 0,
                        SectionType = SectionTypeEnum.Header,
                        IsDesignMode = true
                    });

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionTitle = "Page Footer", 
                        SectionIndex = 1,
                        SectionType = SectionTypeEnum.Footer,
                        IsDesignMode = true
                    });


                    break;
            }

            foreach(ReportSection section in this.uxReportSections.Children)
            {
                section.ReportObjectSelected += this.ReportObject_Selected;
            }
        }

        private void InitializeToolsStack()
        {
            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Label",  ToolType = typeof(ReportLabel) });
        }
    }
}
