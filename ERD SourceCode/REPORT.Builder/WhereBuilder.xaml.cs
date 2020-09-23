using GeneralExtensions;
using REPORT.Builder.ReportComponents;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for WhereBuilder.xaml
    /// </summary>
    public partial class WhereBuilder : UserControlBase
    {
        public WhereBuilder()
        {
            this.InitializeComponent();

            this.SectionViewModel = new SelectViewModel();

            this.SectionViewModel.LookupValues = new DataItemModel[] { };

            this.SectionViewModel.PropertyChanged += this.SectionViewModel_Changed;

            this.uxSections.Items.Add(this.SectionViewModel);

            this.uxSections[0, 0].Caption = "Parent Section";

            this.SectionLinks = new List<SectionLinkModel>();
        }

        public SelectViewModel SectionViewModel { get; set;}

        public List<SectionLinkModel> SectionLinks;

        public void AddSectionOptions(ReportSection[] sections, int groupIndex)
        {
            this.reportSections = sections;

            List<DataItemModel> result = new List<DataItemModel>();

            result.Add(new DataItemModel { DisplayValue = "<Not Selected>", ItemKey = -1 });

            foreach(ReportSection section in sections.Where(gr => gr.SectionGroupIndex < groupIndex && gr.SectionType == SectionTypeEnum.TableData))
            {
                result.Add(new DataItemModel { DisplayValue = section.Title, ItemKey = section.SectionGroupIndex });
            }

            this.uxSections[0, 0].SetComboBoxItems(result.ToArray());
        }

        public void AddSectionLinks(SectionLinkModel[] links)
        {
            this.SectionLinks.Clear();

            this.sectionLinks = links;

            this.SectionLinks.AddRange(this.sectionLinks);

        }

        private void SectionViewModel_Changed(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                this.uxSectionLinks.Children.Clear();

                this.selectedSection = this.reportSections.FirstOrDefault(gi => gi.SectionGroupIndex == this.SectionViewModel.SelectedItem.ToInt32());

                foreach(ReportColumnModel column in this.selectedSection.ReportColumns)
                {
                    // TODO: Build a selectable option to add links to the canvase for the SQL
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private ReportSection selectedSection { get; set; }

        private ReportSection[] reportSections { get; set; }

        private SectionLinkModel[] sectionLinks { get; set; }
    }
}
