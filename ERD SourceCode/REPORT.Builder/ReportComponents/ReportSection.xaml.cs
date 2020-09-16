﻿using GeneralExtensions;
using System;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder.ReportComponents
{
    /// <summary>
    /// Interaction logic for ReportSection.xaml
    /// </summary>
    public partial class ReportSection : UserControlBase
    {
        public delegate void ReportObjectSelectedEvent(object sender);

        public event ReportObjectSelectedEvent ReportObjectSelected;

        private double canvasHeight;

        private SectionTypeEnum sectionType;

        private PaperKind paperKind = PaperKind.A4;

        private PageMediaSize pageSize;

        public ReportSection()
        {
            this.InitializeComponent();

            this.SizeChanged += this.ReportSection_SizeChanged;

            this.uxSectionCanvas.ReportObjectSelected += this.ReportObject_Selected;
        }

        public XElement SectionXml
        {
            get
            {
                XElement result = new XElement("ReportSection");

                result.Add(new XAttribute("SectionType", (int)this.SectionType));
                result.Add(new XAttribute("SectionTitle", this.SectionTitle));
                result.Add(new XAttribute("PaperKind", this.PaperKind));
                result.Add(new XAttribute("SectionIndex", this.SectionIndex));
                result.Add(new XAttribute("CanvasHeight", this.uxMainGrid.RowDefinitions[1].Height.Value));

                result.Add(this.uxSectionCanvas.CanvasXml);

                return result;
            }

            set
            {
                foreach(XAttribute property in value.Attributes())
                {
                    this.SetPropertyValue(property.Name.LocalName, property.Value);
                }

                this.uxSectionCanvas.CanvasXml = value.Element("CanvasXml");
            }
        }

        public int SectionIndex { get; set; }

        public double CanvasHeight
        {
            get
            {
                return this.canvasHeight;
            }

            set
            {
                this.canvasHeight = value;

                this.uxMainGrid.RowDefinitions[1].Height = new GridLength(value);
            }
        }

        public bool IsDesignMode 
        { 
            get
            {
                return this.uxSectionCanvas.IsDesignMode;
            }
            
            set
            {
                this.uxSectionCanvas.IsDesignMode = value;
            }
        }

        public string SectionTitle
        {
            get
            {
                return this.uxSectionCaption.Content.ParseToString();
            }

            set
            {
                this.uxSectionCaption.Content = value;

                this.Title = value;
            }
        }

        public SectionTypeEnum SectionType 
        {
            get
            {
                return this.sectionType;
            }

            set
            {
                this.sectionType = value;

                this.SetPageAndCanvasSize();
            }
        }

        public PaperKind PaperKind
        {
            get
            {
                return this.paperKind;
            }

            set
            {
                this.paperKind = value;

                this.SetPageAndCanvasSize();
            }
        }

        public PageMediaSize PageSize
        {
            get
            {
                return this.pageSize;
            }

            private set
            {
                this.pageSize = value;
            }
        }

        private void ReportSection_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshRuler(this.uxMainGrid.RowDefinitions[1].ActualHeight);
        }

        private void Collapse_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                bool isCollapsing = this.uxCollapse.Direction == WPF.Tools.Specialized.DirectionsEnum.Down;

                if (isCollapsing)
                {
                    this.CanvasHeight = this.uxMainGrid.RowDefinitions[1].Height.Value;

                    this.uxSectionCanvas.Visibility = Visibility.Collapsed;
                    
                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(0);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(0);

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Right;
                }
                else
                {
                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(this.CanvasHeight);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(3);

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Down;

                    this.uxSectionCanvas.Visibility = Visibility.Visible;

                    this.RefreshRuler(this.CanvasHeight);
                }

                this.ReportObjectSelected?.Invoke(null);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportObject_Selected(object sender)
        {
            this.ReportObjectSelected?.Invoke(sender);
        }

        private void SetPageAndCanvasSize()
        {
            this.PageSize = PageSetupOptions.GetPageMediaSize(this.PaperKind);

            this.uxSectionCanvas.Width = this.PageSize.Width.Value;

            switch (this.SectionType)
            {
                case SectionTypeEnum.Page:

                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(this.PageSize.Height.Value);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(0);

                    this.uxBottomHandle.Visibility = Visibility.Collapsed;

                    this.uxSectionCanvas.Height = this.PageSize.Height.Value;

                    break;

                case SectionTypeEnum.Header:
                case SectionTypeEnum.Footer:
                default:

                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(150);
                    
                    break;

            }

            this.RefreshRuler(this.uxSectionCanvas.Height);
        }

        private void RefreshRuler(double heigh)
        {
            this.uxVerticalRuler.Refresh(25, heigh);
        }
    }
}