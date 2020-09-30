using GeneralExtensions;
using REPORT.Builder.ReportComponents;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ViSo.SharedEnums;
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
        private List<WhereParameterModel> SectionLinks;

        public WhereBuilder()
        {
            this.InitializeComponent();

            this.SectionViewModel = new SelectViewModel();

            this.SectionViewModel.LookupValues = new DataItemModel[] { };

            this.SectionViewModel.PropertyChanged += this.SectionViewModel_Changed;

            this.uxSections.Items.Add(this.SectionViewModel);

            this.uxSections[0, 0].Caption = "Parent Section";

            this.SectionLinks = new List<WhereParameterModel>();
        }

        public SelectViewModel SectionViewModel { get; set;}

        public void AddSectionOptions(ReportSection[] sections, int groupIndex)
        {
            this.CurrentGroupIndex = groupIndex;

            this.ReportSections = sections;

            this.uxSectionLinks.Children.Clear();

            List<DataItemModel> result = new List<DataItemModel>();

            result.Add(new DataItemModel { DisplayValue = "<Not Selected>", ItemKey = -1 });

            foreach(ReportSection section in sections.Where(gr => gr.SectionGroupIndex < groupIndex && gr.SectionType == SectionTypeEnum.TableData))
            {
                result.Add(new DataItemModel { DisplayValue = section.Title, ItemKey = section.SectionGroupIndex });
            }

            this.uxSections[0, 0].SetComboBoxItems(result.ToArray());

            this.SelectedSection = this.ReportSections
                    .FirstOrDefault(gi => gi.SectionGroupIndex == this.CurrentGroupIndex
                                        && gi.SectionType == SectionTypeEnum.TableData);

            this.SectionViewModel.PropertyChanged -= this.SectionViewModel_Changed;

            this.ForeignSection = this.ReportSections
                    .FirstOrDefault(gi => gi.SqlManager.HaveForeignGroupIndex(this.SelectedSection.SectionGroupIndex)
                                        && gi.SectionType == SectionTypeEnum.TableData);

            if (this.ForeignSection != null)
            {
                this.uxSections[0, 0].SetValue(this.ForeignSection.SectionGroupIndex);

                foreach (WhereParameterModel whereParameter in this.SelectedSection.SqlManager.WhereParameterModel.OrderBy(oi => oi.OperatorIndex))
                {
                    WhereParameter whereClause = new WhereParameter(whereParameter)
                    {
                        TableColumns = this.SelectedSection.ReportColumns.ToArray(),
                        ForeignColumns = this.ForeignSection.ReportColumns.ToArray(),
                        OperatorIndex = this.uxSectionLinks.Children.Count
                    };

                    whereClause.SqlOperatorChanged += this.SqlOperator_Changed;

                    whereClause.WhereClauseChanged += this.WhereClause_Changed;

                    this.uxSectionLinks.Children.Add(whereClause);
                }
            }

            this.SectionViewModel.PropertyChanged += this.SectionViewModel_Changed;
        }

        public void AddSectionLinks(WhereParameterModel[] links)
        {
            this.SectionLinks.Clear();

            this.SectionLinks.AddRange(links);

            this.SectionLinks.AddRange(this.SectionLinks);

        }

        private void SectionViewModel_Changed(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                this.SelectedSection.SqlManager.AddWhereModels(new WhereParameterModel[] { });

                foreach(ReportSection section in this.ReportSections)
                {
                    section.SqlManager.RemoveForeignGroupIndex(this.SelectedSection.SectionGroupIndex);
                }

                this.ForeignSection = this.ReportSections
                    .FirstOrDefault(gi => gi.SectionGroupIndex == this.SectionViewModel.SelectedItem.ToInt32()
                                        && gi.SectionType == SectionTypeEnum.TableData);

                if (this.ForeignSection != null)
                {
                    this.ForeignSection.SqlManager.AddForeignGroupIndex(this.SelectedSection.SectionGroupIndex);
                }

                this.uxSectionLinks.Children.Clear();

                this.AddNewWhereClause();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SqlOperator_Changed(object sender, int operatorIndex, SqlWhereOperatorsEnum option)
        {
            try
            {
                switch(option)
                {
                    case SqlWhereOperatorsEnum.None:

                        List<WhereParameter> removeChildren = new List<WhereParameter>();

                        foreach(WhereParameter clause in this.uxSectionLinks.Children)
                        {
                            if (clause.OperatorIndex > operatorIndex)
                            {
                                removeChildren.Add(clause);
                            }
                        }

                        foreach(WhereParameter clause in removeChildren)
                        {
                            this.uxSectionLinks.Children.Remove(clause);
                        }

                        this.SelectedSection.SqlManager.AddWhereModels(new WhereParameterModel[] { });

                        break;

                    default:

                        this.AddNewWhereClause();

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void WhereClause_Changed(object sender, int operatorIndex)
        {
            try
            {
                List<WhereParameterModel> result = new List<WhereParameterModel>();

                foreach(WhereParameter parameter in this.uxSectionLinks.Children)
                {
                    result.Add(parameter.WhereClause);
                }

                this.SelectedSection.SqlManager.AddWhereModels(result.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private int CurrentGroupIndex { get; set; }

        private ReportSection SelectedSection { get; set; }

        private ReportSection ForeignSection { get; set; }

        private ReportSection[] ReportSections { get; set; }

        private void AddNewWhereClause()
        {
            if (this.ForeignSection == null)
            {
                return;
            }

            WhereParameter whereClause = new WhereParameter
            {
                TableColumns = this.SelectedSection.ReportColumns.ToArray(),
                ForeignColumns = this.ForeignSection.ReportColumns.ToArray(),
                OperatorIndex = this.uxSectionLinks.Children.Count
            };

            whereClause.SqlOperatorChanged += this.SqlOperator_Changed;

            whereClause.WhereClauseChanged += this.WhereClause_Changed;

            this.uxSectionLinks.Children.Add(whereClause);
        }
    }
}
