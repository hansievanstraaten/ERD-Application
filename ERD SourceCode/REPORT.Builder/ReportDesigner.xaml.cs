using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using Microsoft.Win32;
using REPORT.Builder.Printing;
using REPORT.Builder.ReportComponents;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ViSo.Dialogs.Controls;
using ViSo.Dialogs.TextEditor;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.ColoutPicker;
using WPF.Tools.Exstention;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReportDesigner.xaml
	/// </summary>
	public partial class ReportDesigner : UserControlBase
	{
		#region FIELDS

		private readonly ReportTypeEnum reportDesignType;

		private ReportMasterModel reportMaster;

		private UIElement selectedReportObject;

		private DataSourceMasterModel dataSourceMainTable;

		private readonly List<ReportSection> dataReportSections = new List<ReportSection>();

		#endregion

		public ReportDesigner(ReportMasterModel masterModel)
		{
			InitializeComponent();

			InitializeToolsStack();

			SizeChanged += ReportDesigner_SizeChanged;

			reportDesignType = (ReportTypeEnum)masterModel.ReportTypeEnum;

			ReportMaster = masterModel;

			uxReportMasterModel.Items.Add(ReportMaster);

			if (ReportMaster.MasterReport_Id == 0)
			{   // New Report
				ReportMaster.ReportXMLVersion = 1;

				InitializeReportSections();
			}
			else
			{
				ReportTablesRepository repo = new ReportTablesRepository();

				//this.ReportMaster.ReportXMLVersion = repo.GetReportXMLVersion(masterModel.MasterReport_Id);

				ReportXMLModel reportXML = repo.GetReportXMLByPrimaryKey(ReportMaster.ReportXMLVersion, masterModel.MasterReport_Id);

				string reportXml = reportXML.BinaryXML.UnzipFile().ParseToString();

				XDocument report = XDocument.Parse(reportXml);

				reportDesignType = (ReportTypeEnum)report.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value.ToInt32();

				foreach (XElement sectionElement in report.Root.Element("ReportSettings").Elements("ReportSection").OrderBy(si => si.Attribute("SectionIndex").Value.ToInt32()))
				{
					ReportSection section = new ReportSection { IsDesignMode = true };

					section.SectionXml = sectionElement;

					uxReportSections.Children.Add(section);

					section.ReportObjectSelected += ReportObject_Selected;

					if (section.SectionType == SectionTypeEnum.TableData)
					{
						section.RequestNewDataSections += NewDataSection_Requested;

						section.ReportColumnAdded += ReportColumn_Added;
					}
					else if (reportDesignType == ReportTypeEnum.CoverPage
						|| reportDesignType == ReportTypeEnum.FinalPage)
					{
						section.RefreshPageStaticMarkers();
					}

					dataReportSections.Add(section);
				}
			}

			uxReportSections.MinWidth = CanvasWidth + 200;

			RefreshMarginMarkers();

			SetupDataReportsOptions();

			ReportMaster.PropertyChanged += ReportMaster_PropertyChanged;
		}

		#region PBLIC PROPERTIES

		public ReportMasterModel ReportMaster
		{
			get
			{
				return reportMaster;
			}

			set
			{
				reportMaster = value;
			}
		}

		public DataSourceMasterModel DataSourceMainTable
		{
			get
			{
				return dataSourceMainTable;
			}

			set
			{
				dataSourceMainTable = value;
			}
		}

		#endregion

		#region PUBLIC METHODS

		public bool Save()
		{
			try
			{
				if (uxReportMasterModel.HasValidationError)
				{
					return false;
				}

				ReportTablesRepository repo = new ReportTablesRepository();

				repo.UpdateReportMaster(ReportMaster);

				#region REPORT XML

				ReportXMLModel reportXml = repo.GetReportXMLByPrimaryKey(ReportMaster.ReportXMLVersion, ReportMaster.MasterReport_Id);

				if (reportXml == null)
				{
					reportXml = new ReportXMLModel
					{
						MasterReport_Id = ReportMaster.MasterReport_Id,
						ReportXMLVersion = ReportMaster.ReportXMLVersion,
						PrintCount = 0
					};
				}

				XDocument reportXmlDocument = GetReportXml();

				reportXml.BinaryXML = reportXmlDocument.ToString().ZipFile();

				repo.UpdateReportXML(reportXml);

				repo.DisableReportXMLPrintFilters(ReportMaster.MasterReport_Id, ReportMaster.ReportXMLVersion);

				List<ReportXMLPrintParameterModel> xxx = GetReportparameterFilters(reportXmlDocument);

				foreach (ReportXMLPrintParameterModel filter in GetReportparameterFilters(reportXmlDocument))
				{
					repo.UpdateReportXMLPrintParameter(filter);
				}

				#endregion

				#region REPORT CONNECTIONS

				repo.SetReportConnectionsSttus(ReportMaster.MasterReport_Id, false, false);

				DatabaseModel databaseModel = Connections.Instance.GetConnection(reportMaster.ProductionConnection);

				if (databaseModel != null)
				{
					ReportConnectionModel connection = new ReportConnectionModel
					{
						MasterReport_Id = ReportMaster.MasterReport_Id,
						ReportConnectionName = reportMaster.ProductionConnection,
						DatabaseTypeEnum = (int)databaseModel.DatabaseType,
						ServerName = databaseModel.ServerName,
						DatabaseName = databaseModel.DatabaseName,
						UserName = databaseModel.UserName,
						Password = databaseModel.Password.Encrypt(),
						TrustedConnection = databaseModel.TrustedConnection,
						IsProductionConnection = true,
						IsActive = true
					};

					repo.UpdateReportConnection(connection);
				}

				#endregion

				#region SAVE DATASOURCE SELECTION

				if (DataSourceMainTable != null)
				{
					DataSourceRepository sourceRepo = new DataSourceRepository();

					DataSourceMainTable.MasterReport_Id = ReportMaster.MasterReport_Id;

					sourceRepo.UpdateDataSourceMaster(DataSourceMainTable);

					sourceRepo.SetIsAvailable(ReportMaster.MasterReport_Id, false);

					foreach (DataSourceTableModel sourceModel in DataSourceMainTable.SelectedSourceTables)
					{
						sourceModel.MasterReport_Id = ReportMaster.MasterReport_Id;

						sourceRepo.UpdateDataSourceTable(sourceModel);
					}
				}

				#endregion

				return true;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());

				return false;
			}
		}

		#endregion

		#region OVERRIDEs

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			base.OnPreviewKeyUp(e);

			switch (e.Key)
			{
				case Key.Up:

					ResizeHandles.NotchUp(1);

					break;

				case Key.Down:

					ResizeHandles.NotchDown(1);

					break;

				case Key.Left:

					ResizeHandles.NotchLeft(1);

					break;

				case Key.Right:

					ResizeHandles.NotchRight(1);

					break;
			}
		}

		#endregion

		#region EVENT HANDLERS

		private void ReportSave_Cliked(object sender, RoutedEventArgs e)
		{
			try
			{
				Save();

				MessageBox.Show("Report Saved");
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			uxHorizontalRuler.Refresh(CanvasWidth, 20, CanvasHeight);
		}

		private void ReportObject_Selected(object sender, object reportObject)
		{
			try
			{
				uxProperties.Items.Clear();

				SelectedSectionGroupIndex = sender.GetPropertyValue("SectionGroupIndex").ToInt32();

				int sectionIndex = sender.GetPropertyValue("SectionIndex").ToInt32();

				if (SelectedSectionGroupIndex > 0 && reportObject == null)
				{
					uxCanvasSql.Text = sender == null ? string.Empty : sender.GetPropertyValue("SQLQuery").ParseToString();

					uxWhereBuilder.AddSectionOptions(dataReportSections.ToArray(), sender.GetPropertyValue("SectionGroupIndex").ToInt32());

					uxPropertiesCaption.Visibility = Visibility.Collapsed;

					uxProperties.Visibility = Visibility.Collapsed;

					uxWhereBuilder.Visibility = Visibility.Visible;

					uxWhereBuilderCaption.Visibility = Visibility.Visible;

					uxCanvasSql.Visibility = Visibility.Visible;

					uxCanvasSqlCaption.Visibility = Visibility.Visible;

					return;
				}
				else if (SelectedSectionGroupIndex == 0 && reportObject == null)
				{
					uxPropertiesCaption.Visibility = Visibility.Collapsed;

					uxProperties.Visibility = Visibility.Collapsed;

					uxWhereBuilder.Visibility = Visibility.Collapsed;

					uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

					uxCanvasSql.Visibility = Visibility.Collapsed;

					uxCanvasSqlCaption.Visibility = Visibility.Collapsed;
				}
				else if (reportObject != null)
				{
					uxPropertiesCaption.Visibility = Visibility.Visible;

					uxProperties.Visibility = Visibility.Visible;

					uxWhereBuilder.Visibility = Visibility.Collapsed;

					uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

					uxCanvasSql.Visibility = Visibility.Collapsed;

					uxCanvasSqlCaption.Visibility = Visibility.Collapsed;

					selectedReportObject = null;

					selectedReportObject = reportObject as UIElement;

					selectedReportObject.PreviewMouseRightButtonUp += SelecteReportObject_RightClick;

					uxProperties.Items.Add(reportObject);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportColumn_Added(object sender, ReportColumnModel column, int sectionGroupIndex)
		{
			try
			{
				RefreshSectionTitles();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void NewDataSection_Requested(object sender, ReportColumnModel column, int sectionGroupIndex)
		{
			try
			{
				ReportSection dataSection = dataReportSections
					.FirstOrDefault(rp => rp.SectionTableName == column.TableName
										&& rp.SectionType == SectionTypeEnum.TableData);

				if (dataSection == null)
				{
					int newGroupIndex = CreateDatareportSections();

					foreach (ReportSection section in dataReportSections.Where(si => si.SectionGroupIndex == newGroupIndex))
					{
						section.SectionTableName = column.TableName;
					}

					dataSection = dataReportSections
					.FirstOrDefault(rp => rp.SectionGroupIndex == newGroupIndex
										&& rp.SectionType == SectionTypeEnum.TableData);
				}

				dataSection.AddReportColumn(column);

				RefreshSectionTitles();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void SelecteReportObject_RightClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				ContextMenu menu = new ContextMenu();

				//if (KeyActions.IsCtrlPressed)
				if (ResizeHandles.SelectedElementCount > 1)
				{
					foreach (ReportAlignmentEnum alignment in Enum.GetValues(typeof(ReportAlignmentEnum)))
					{
						MenuItem alignmentItem = new MenuItem { Header = alignment.GetDescriptionAttribute(), Tag = alignment };

						alignmentItem.Click += Alignment_Click;

						menu.Items.Add(alignmentItem);
					}
				}

				if (ResizeHandles.SelectedElementCount == 1)
				{
					MenuItem delete = new MenuItem { Name = "uxDelete", Header = "Delete Object" };

					delete.Click += SelectedMenuItem_Click;

					menu.Items.Add(delete);
				}

				menu.IsOpen = true;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void Alignment_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				MenuItem menu = (MenuItem)sender;

				ReportAlignmentEnum alignment = menu.Tag.To<ReportAlignmentEnum>();

				ResizeHandles.AlignmentObjects(alignment);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void SelectedMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SectionCanvas canvas = (SectionCanvas)selectedReportObject.FindParentControlBase(typeof(SectionCanvas));

				ResizeHandles.RemoveElement(selectedReportObject);

				canvas.Children.Remove(selectedReportObject);

				selectedReportObject = null;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportMaster_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				switch (e.PropertyName)
				{
					case "PaperKindEnum":

						foreach (ReportSection section in uxReportSections.Children)
						{
							section.PaperKind = (PaperKind)ReportMaster.PaperKindEnum;
						}

						break;

					case "PageOrientationEnum":

						foreach (ReportSection section in uxReportSections.Children)
						{
							section.PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum;
						}

						break;

					case "PageMarginLeft":
					case "PageMarginRight":

					case "PageMarginTop":
					case "PageMarginBottom":

						if (reportDesignType == ReportTypeEnum.ReportContent
							|| reportDesignType == ReportTypeEnum.PageHeaderAndFooter)
						{
							break;
						}

						foreach (ReportSection section in uxReportSections.Children)
						{
							section.MarkerTopMargin = ReportMaster.PageMarginTop.ToInt32();

							section.MarkerBottomMargin = ReportMaster.PageMarginBottom.ToInt32();

							section.RefreshPageStaticMarkers();
						}

						break;

				}

				RefreshMarginMarkers();
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
				if (TextEditing.ShowDialog("Description", ReportMaster.DescriptionText).IsFalse())
				{
					return;
				}

				ReportMaster.DescriptionText = TextEditing.Text;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void PropertiesObject_Browse(object sender, string buttonKey)
		{
			try
			{
				switch (buttonKey)
				{
					case "ReportImageKey":

						OpenFileDialog dlg = new OpenFileDialog();

						bool? result = dlg.ShowDialog();

						if (!result.HasValue || !result.Value)
						{
							return;
						}

						selectedReportObject.SetPropertyValue("ImagePath", dlg.FileName);

						break;

					default:

						ColourPicker picker = new ColourPicker();

						if (picker.ShowDialog().IsFalse())
						{
							return;
						}

						selectedReportObject.SetPropertyValue(buttonKey, picker.SelectedColour);

						break;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void DataSourseSelect_Browse(object sender, RoutedEventArgs e)
		{
			try
			{
				DatasourceSelector selector = new DatasourceSelector(ReportMaster.MasterReport_Id);

				if (ControlDialog.ShowDialog("Data Source", selector, "Accept").IsFalse())
				{
					return;
				}

				DataSourceMainTable = selector.MainTable;

				LoadDataSource();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportPrint_Cliked(object sender, RoutedEventArgs e)
		{
			try
			{
				ContextMenu menu = new ContextMenu();


				MenuItem defaultItem = new MenuItem
				{
					Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})",
					Tag = Connections.Instance.DefaultConnectionName
				};

				defaultItem.Click += ReportPrintConnection_Cliked;

				menu.Items.Add(defaultItem);

				foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
				{
					MenuItem alternativeItem = new MenuItem
					{
						Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})",
						Tag = connectionKey.Key
					};

					alternativeItem.Click += ReportPrintConnection_Cliked;

					menu.Items.Add(alternativeItem);
				}

				menu.IsOpen = true;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportPrintConnection_Cliked(object sender, RoutedEventArgs e)
		{
			try
			{
				if (uxReportMasterModel.HasValidationError)
				{
					return;
				}

				MenuItem item = (MenuItem)e.Source;

				PrintPreview preview = new PrintPreview(ReportMaster.ReportName, GetPrintCanvases(item));

				ControlDialog.Show("Reports", preview, string.Empty, showOkButton: false, windowState: WindowState.Maximized);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ExportPDF_Cliked(object sender, RoutedEventArgs e)
		{
			try
			{
				ContextMenu menu = new ContextMenu();

				MenuItem defaultItem = new MenuItem
				{
					Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})",
					Tag = Connections.Instance.DefaultConnectionName
				};

				defaultItem.Click += ExportPdfConnection_Cliked;

				menu.Items.Add(defaultItem);

				foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
				{
					MenuItem alternativeItem = new MenuItem
					{
						Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})",
						Tag = connectionKey.Key
					};

					alternativeItem.Click += ExportPdfConnection_Cliked;

					menu.Items.Add(alternativeItem);
				}

				menu.IsOpen = true;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ExportPdfConnection_Cliked(object sender, RoutedEventArgs e)
		{
			try
			{
				if (uxReportMasterModel.HasValidationError)
				{
					return;
				}

				SaveFileDialog dlg = new SaveFileDialog();

				dlg.FileName = ReportMaster.ReportName;

				dlg.Filter = "PDF Files | *.pdf";

				dlg.OverwritePrompt = false;

				if (dlg.ShowDialog().IsFalse())
				{
					return;
				}

				MenuItem item = (MenuItem)e.Source;

				CanvasToPDF pdf = new CanvasToPDF();

				string result = pdf.ConvertToPdf(dlg.FileName, GetPrintCanvases(item));

				Process.Start(result);

			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		#endregion

		#region PRIVATE PROPERTIES

		private int SelectedSectionGroupIndex { get; set; }

		private double CanvasWidth
		{
			get
			{
				return ((ReportSection)uxReportSections.Children[0]).GetPropertyValue("PageWidth").ToDouble();
			}
		}

		private double CanvasHeight
		{
			get
			{
				return uxReportSections.ActualHeight + 25;
			}
		}

		#endregion

		#region PRIVATE METHODS

		private void InitializeReportSections()
		{
			uxReportSections.Children.Clear();

			switch (reportDesignType)
			{
				case ReportTypeEnum.CoverPage:
				case ReportTypeEnum.FinalPage:

					uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 0,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Page,
						IsDesignMode = true,
						PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					}); ;

					break;

				case ReportTypeEnum.ReportContent:

					CreateDatareportSections();

					break;

				case ReportTypeEnum.PageHeaderAndFooter:

					uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 0,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Header,
						IsDesignMode = true,
						PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					});

					uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 1,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Footer,
						IsDesignMode = true,
						PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					});

					break;
			}

			foreach (ReportSection section in uxReportSections.Children)
			{
				section.ReportObjectSelected += ReportObject_Selected;

				if (section.SectionType == SectionTypeEnum.TableData)
				{
					section.RequestNewDataSections += NewDataSection_Requested;

					section.ReportColumnAdded += ReportColumn_Added;
				}
			}
		}

		private void InitializeToolsStack()
		{
			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Label", ToolType = typeof(ReportLabel) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Border", ToolType = typeof(ReportBorder) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Current Date", ToolType = typeof(CurrentDate) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Image", ToolType = typeof(ReportImage) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Horizontal Line", ToolType = typeof(ReportHorizontalLine) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Vertical Line", ToolType = typeof(ReportVerticalLine) });

			uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Page Break", ToolType = typeof(ReportPageBreak) });
		}

		private void RefreshMarginMarkers()
		{
			double pageWidth = CanvasWidth;

			uxHorizontalRuler.ClearMarkers(true);

			uxHorizontalRuler.Refresh(pageWidth, 25, CanvasHeight);

			uxHorizontalRuler.AddMarker(ReportMaster.PageMarginLeft, true);

			uxHorizontalRuler.AddMarker((pageWidth - ReportMaster.PageMarginRight), true);
		}

		private void SetupDataReportsOptions()
		{
			if (reportDesignType != ReportTypeEnum.ReportContent)
			{
				return;
			}

			uxReportMasterModel["Cover Page"].Visibility = Visibility.Visible;

			uxReportMasterModel["Page Headers and Footers"].Visibility = Visibility.Visible;

			uxReportMasterModel["Final Page"].Visibility = Visibility.Visible;

			uxReportMasterModel["Production Connection"].Visibility = Visibility.Visible;

			uxReportMasterModel["Production Connection"].IsRequired = true;

			uxDataMenueBorder.Visibility = Visibility.Visible;

			uxDataMenue.Visibility = Visibility.Visible;

			uxTableTree.Visibility = Visibility.Visible;

			DataSourceRepository repo = new DataSourceRepository();

			DataSourceMasterModel sourceMaster = repo.GetDataSourceMasterByPrimaryKey(ReportMaster.MasterReport_Id);

			if (sourceMaster != null)
			{
				sourceMaster.SelectedSourceTables.AddRange(repo.GetDataSourceTableByForeignKeyMasterReport_Id(ReportMaster.MasterReport_Id));

				DataSourceMainTable = sourceMaster;

				LoadDataSource();
			}

			uxReportMasterModel.AllignAllCaptions();
		}

		private void LoadDataSource()
		{
			uxTableTree.Items.Clear();

			foreach (ReportSection section in uxReportSections.Children)
			{
				if (section.SectionGroupIndex != 0)
				{
					continue;
				}

				section.SectionTableName = DataSourceMainTable.MainTableName; // Do this to reserve the first section for the main data source

				section.RefresSectionTitle();
			}

			TreeViewItem mainTreeItem = new TreeViewItem { Header = DataSourceMainTable.MainTableName };

			foreach (DataItemModel column in Integrity.GetColumnsForTable(DataSourceMainTable.MainTableName))
			{
				ColumnObjectModel tableColumn = Integrity.GetObjectModel(DataSourceMainTable.MainTableName, column.DisplayValue);

				ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = DataSourceMainTable.MainTableName }) as ReportColumnModel;

				ToolsMenuItem treeMainColumn = new ToolsMenuItem
				{
					Caption = column.DisplayValue,
					ToolType = typeof(ReportDataObject),
					Tag = column.CopyToObject(reportColumn)
				};

				mainTreeItem.Items.Add(treeMainColumn);
			}

			uxTableTree.Items.Add(mainTreeItem);

			foreach (DataSourceTableModel sourceTable in DataSourceMainTable.SelectedSourceTables)
			{
				if (sourceTable.TableName == DataSourceMainTable.MainTableName)
				{
					continue;
				}

				TreeViewItem childTreeItem = new TreeViewItem { Header = sourceTable.TableName };

				foreach (DataItemModel column in Integrity.GetColumnsForTable(sourceTable.TableName))
				{
					ColumnObjectModel tableColumn = Integrity.GetObjectModel(sourceTable.TableName, column.DisplayValue);

					ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = sourceTable.TableName }) as ReportColumnModel;

					ToolsMenuItem treeChildColumn = new ToolsMenuItem
					{
						Caption = column.DisplayValue,
						ToolType = typeof(ReportDataObject),
						Tag = column.CopyToObject(reportColumn)
					};

					childTreeItem.Items.Add(treeChildColumn);
				}

				uxTableTree.Items.Add(childTreeItem);
			}
		}

		private XDocument GetReportXml()
		{
			XDocument result = new XDocument();

			XElement root = new XElement("Root");

			XElement report = new XElement("ReportSettings");

			report.Add(new XAttribute("ReportTypeEnum", (int)reportDesignType));

			foreach (ReportSection section in uxReportSections.Children)
			{
				report.Add(section.SectionXml);
			}

			root.Add(report);

			result.Add(root);

			//ReportXMLModel reportXml = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, this.ReportMaster.MasterReport_Id);

			//if (reportXml == null)
			//{
			//    reportXml = new ReportXMLModel
			//    {
			//        MasterReport_Id = this.ReportMaster.MasterReport_Id,
			//        ReportXMLVersion = this.ReportMaster.ReportXMLVersion,
			//        PrintCount = 0
			//    };
			//}

			return result;
		}

		private Dictionary<int, PrintCanvas> GetPrintCanvases(MenuItem item)
		{
			XDocument reportXml = GetReportXml();

			List<ReportXMLPrintParameterModel> parameterFilters = GetReportparameterFilters(reportXml);

			if (parameterFilters.Count > 0)
			{
				ReportFilterOptions filters = new ReportFilterOptions(parameterFilters);

				if (ControlDialog.ShowDialog("Parameter Filters", filters, "ValidateFilters", autoSize: true).IsFalse())
				{
					throw new ApplicationException("No Filters selected");
				}
			}

			BuildReportToCanvas reportPrint = new BuildReportToCanvas();

			DatabaseModel connection = Connections.Instance.GetConnection(item.Tag.ParseToString());

			BuildReportXML xmlBuild = new BuildReportXML();

			XDocument repotXml = xmlBuild.GetReport(reportXml, connection, ReportMaster.CopyToObject(new ReportMaster()) as ReportMaster, parameterFilters);

			reportPrint.PrintDocument(repotXml);

			return reportPrint.Pages;
		}

		private List<ReportXMLPrintParameterModel> GetReportparameterFilters(XDocument reportXml)
		{
			List<XElement> filterElements = reportXml
					.Root
					.Descendants("ReportObject")
					.Where(r => r.IsDataObject() && r.Attribute("UseAsPrintParameter").Value == "true")
					.ToList();

			List<ReportXMLPrintParameterModel> resut = new List<ReportXMLPrintParameterModel>();

			foreach (XElement item in filterElements)
			{
				resut.Add(new ReportXMLPrintParameterModel
				{
					TableName = item.Attribute("ObjectTable").Value,
					ColumnName = item.Attribute("ObjectColumn").Value,
					ReportXMLVersion = ReportMaster.ReportXMLVersion,
					MasterReport_Id = ReportMaster.MasterReport_Id,
					FilterCaption = item.Attribute("PrintParameterCaption").Value,
					DefaultValue = item.Attribute("PrintParameterDefaultValue").Value,
					IsActive = true
				});
			}

			return resut;
		}

		#endregion

		#region NEW DATA SECTION METHODS

		private int CreateDatareportSections()
		{
			int groupSectionId = NextGroupIndex();

			int[] sectionIndex = CalculateSectionIndexes();

			ReportSection header = new ReportSection
			{
				SectionIndex = sectionIndex[0],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableHeader,
				IsDesignMode = true,
				PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			uxReportSections.Children.Insert(sectionIndex[0], header);

			dataReportSections.Add(header);

			ReportSection data = new ReportSection
			{
				SectionIndex = sectionIndex[1],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableData,
				IsDesignMode = true,
				PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			uxReportSections.Children.Insert(sectionIndex[1], data);

			dataReportSections.Add(data);

			ReportSection footer = new ReportSection
			{
				SectionIndex = sectionIndex[2],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableFooter,
				IsDesignMode = true,
				PaperKind = (PaperKind)ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			uxReportSections.Children.Insert(sectionIndex[2], footer);

			dataReportSections.Add(footer);

			header.ReportObjectSelected += ReportObject_Selected;

			data.ReportObjectSelected += ReportObject_Selected;

			data.RequestNewDataSections += NewDataSection_Requested;

			data.ReportColumnAdded += ReportColumn_Added;

			//data.ReportSectionWhereClauseChanged += this.ReportSectionWhereClause_Changed;

			footer.ReportObjectSelected += ReportObject_Selected;

			return groupSectionId;
		}

		private void RefreshSectionTitles()
		{
			foreach (ReportSection section in dataReportSections)
			{
				section.RefresSectionTitle();
			}
		}

		private int NextGroupIndex()
		{
			return dataReportSections.Count == 0 ? 0 :
				(dataReportSections.Select(gr => gr.SectionGroupIndex).Max() + 1);
		}

		private int GetMaxSectionIndex()
		{   // Zero based index
			return dataReportSections.Count == 0 ? 2 :
				(dataReportSections.Select(gr => gr.SectionIndex).Max());
		}

		private int[] CalculateSectionIndexes()
		{
			int nextGroupIndex = NextGroupIndex();

			if (nextGroupIndex == 0)
			{
				return new int[] { 0, 1, 2 };
			}

			int maxSectionIndex = GetMaxSectionIndex();

			int insertIndexStart = ((maxSectionIndex + 1) / 3) + ((maxSectionIndex + 1) / 3);

			int[] result = new int[3];

			result[0] = insertIndexStart;
			result[1] = insertIndexStart + 1;
			result[2] = insertIndexStart + 2;

			int shiftIndex = result[2] + 1;

			ReportSection[] changeSections = dataReportSections
				.Where(si => si.SectionIndex >= insertIndexStart)
				.OrderBy(oi => oi.SectionIndex)
				.ToArray();

			foreach (ReportSection section in changeSections)
			{
				section.SectionIndex = shiftIndex;

				++shiftIndex;
			}

			return result;
		}

		#endregion
	}
}
