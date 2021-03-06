﻿using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using Microsoft.Win32;
using REPORT.Builder.Printing;
using REPORT.Builder.ReportComponents;
using REPORT.Builder.ReportTools;
using REPORT.Data.Common;
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
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReportDesigner.xaml
	/// </summary>
	public partial class ReportDesigner : UserControlBase
	{
		#region FIELDS

		private bool isDataObject;

		private bool IsReportSum;

		private ReportTypeEnum reportDesignType;

		private ReportMasterModel reportMaster;

		private UIElement selectedReportObject;

		private DataSourceMasterModel dataSourceMainTable;

		private readonly List<ReportSection> dataReportSections = new List<ReportSection>();

		#endregion

		public ReportDesigner(ReportMasterModel masterModel)
		{
			this.InitializeComponent();

			this.InitializeToolsStack();

			SizeChanged += this.ReportDesigner_SizeChanged;

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
				this.LoadReportXml();
			}

			this.uxReportSections.MinWidth = this.CanvasWidth + 200;

			this.RefreshMarginMarkers();

			this.SetupDataReportsOptions();

			this.CheckActiveVersion();

			this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
		}

		#region PBLIC PROPERTIES

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

		public DataSourceMasterModel DataSourceMainTable
		{
			get
			{
				return this.dataSourceMainTable;
			}

			set
			{
				this.dataSourceMainTable = value;
			}
		}

		#endregion

		#region PUBLIC METHODS

		public bool Save()
		{
			try
			{
				this.ReportMaster.PropertyChanged -= this.ReportMaster_PropertyChanged;

				if (this.uxReportMasterModel.HasValidationError)
				{
					return false;
				}

				ReportTablesRepository repo = new ReportTablesRepository();

				repo.UpdateReportMaster(this.ReportMaster);

				#region REPORT XML

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

				reportXml.IsActiveVersion = this.ReportMaster.IsActiveVesion;

				XDocument reportXmlDocument = this.GetReportXml();

				reportXml.BinaryXML = reportXmlDocument.ToString().ZipFile();

				repo.UpdateReportXML(reportXml);

				repo.DisableReportXMLPrintFilters(this.ReportMaster.MasterReport_Id, this.ReportMaster.ReportXMLVersion);

				foreach (ReportXMLPrintParameterModel filter in this.GetReportparameterFilters(reportXmlDocument))
				{
					repo.UpdateReportXMLPrintParameter(filter);
				}

				#endregion

				#region REPORT CONNECTIONS

				repo.SetReportConnectionsSttus(this.ReportMaster.MasterReport_Id, false, false);

				DatabaseModel databaseModel = Connections.Instance.GetConnection(this.reportMaster.ProductionConnection);

				if (databaseModel != null)
				{
					ReportConnectionModel connection = new ReportConnectionModel
					{
						MasterReport_Id = this.ReportMaster.MasterReport_Id,
						ReportConnectionName = this.reportMaster.ProductionConnection,
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

				if (this.DataSourceMainTable != null)
				{
					DataSourceRepository sourceRepo = new DataSourceRepository();

					this.DataSourceMainTable.MasterReport_Id = this.ReportMaster.MasterReport_Id;

					sourceRepo.UpdateDataSourceMaster(this.DataSourceMainTable);

					sourceRepo.SetIsAvailable(this.ReportMaster.MasterReport_Id, false);

					foreach (DataSourceTableModel sourceModel in this.DataSourceMainTable.SelectedSourceTables)
					{
						sourceModel.MasterReport_Id = this.ReportMaster.MasterReport_Id;

						sourceRepo.UpdateDataSourceTable(sourceModel);
					}
				}

				#endregion

				this.uxReportMasterModel["Report Version"]
					.SetComboBoxItems(repo.GetReportXmlVersions(this.ReportMaster.MasterReport_Id)
										  .Select(v => new DataItemModel { DisplayValue = v.ParseToString(), ItemKey = v })
										  .ToArray());

				this.uxReportMasterModel["Report Version"].SetValue(this.ReportMaster.ReportXMLVersion);

				return true;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());

				return false;
			}
			finally
			{
				this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
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
				this.Save();

				MessageBox.Show("Report Saved");
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.uxHorizontalRuler.Refresh(this.CanvasWidth, 20, this.CanvasHeight);
		}

		private void ReportObject_Selected(object sender, object reportObject)
		{
			try
			{
				this.uxProperties.Items.Clear();

				this.SelectedSectionGroupIndex = sender.GetPropertyValue("SectionGroupIndex").ToInt32();

				this.SelectedSectionIndex = sender.GetPropertyValue("SectionIndex").ToInt32();

				Type objectType = reportObject == null ? null : reportObject.GetType();

				this.isDataObject = (reportObject != null && objectType == typeof(ReportDataObject));

				if (this.selectedReportObject != null && this.isDataObject)
				{
					var xxx = this.selectedReportObject.GetPropertyValue("Text");

					ReportSection canvas = this.dataReportSections.FirstOrDefault(d => d.SectionIndex == this.SelectedSectionIndex);

					canvas.UpdateReplacementColumn(this.uxReplacceColumn.WhereHeader);

					canvas.UpdateInvokeReplaceModel(this.uxReplacceColumn.InvokeMethodSetup);

					canvas.UpdateUpdateStatement(this.uxUpdate.UpdateStatement);
				}

				if (this.selectedReportObject != null && this.IsReportSum)
				{
					ReportSum item = this.selectedReportObject.To<ReportSum>();

					item.DataSectionIndexChanged -= this.ReportSumDataSectionIndex_Changed;
				}
				
				this.IsReportSum = (reportObject != null && objectType == typeof(ReportSum));

				if (reportObject != null && this.IsReportSum)
				{
					this.SetSumDataSections(reportObject);
				}

				if (this.SelectedSectionGroupIndex > 0 && reportObject == null)
				{
					this.uxCanvasSql.Text = sender == null ? string.Empty : sender.GetPropertyValue("SQLQuery").ParseToString();

					this.uxWhereBuilder.AddSectionOptions(this.dataReportSections.ToArray(), sender.GetPropertyValue("SectionGroupIndex").ToInt32());

					this.uxPropertiesCaption.Visibility = Visibility.Collapsed;

					this.uxProperties.Visibility = Visibility.Collapsed;

					this.uxReplacceColumnCaption.Visibility = Visibility.Collapsed;

					this.uxReplacceColumn.Visibility = Visibility.Collapsed;

					this.uxUpdateCaption.Visibility = Visibility.Collapsed;

					this.uxUpdate.Visibility = Visibility.Collapsed;

					this.uxWhereBuilder.Visibility = Visibility.Visible;

					this.uxWhereBuilderCaption.Visibility = Visibility.Visible;

					this.uxCanvasSql.Visibility = Visibility.Visible;

					this.uxCanvasSqlCaption.Visibility = Visibility.Visible;

					return;
				}
				else if (this.SelectedSectionGroupIndex == 0 && reportObject == null)
				{
					this.uxPropertiesCaption.Visibility = Visibility.Collapsed;

					this.uxProperties.Visibility = Visibility.Collapsed;

					this.uxReplacceColumnCaption.Visibility = Visibility.Collapsed;

					this.uxReplacceColumn.Visibility = Visibility.Collapsed;

					this.uxUpdateCaption.Visibility = Visibility.Collapsed;

					this.uxUpdate.Visibility = Visibility.Collapsed;

					this.uxWhereBuilder.Visibility = Visibility.Collapsed;

					this.uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

					this.uxCanvasSql.Visibility = Visibility.Collapsed;

					this.uxCanvasSqlCaption.Visibility = Visibility.Collapsed;
				}
				else if (reportObject != null)
				{
					if (this.isDataObject)
					{
						ReportDataObject dataObject = reportObject.To<ReportDataObject>();

						this.uxReplacceColumnCaption.Visibility = Visibility.Visible;

						this.uxReplacceColumn.Visibility = Visibility.Visible;

						this.uxUpdateCaption.Visibility = Visibility.Visible;

						this.uxUpdate.Visibility = Visibility.Visible;

						ReportSection canvas = this.dataReportSections.FirstOrDefault(d => d.SectionIndex == this.SelectedSectionIndex);

						this.uxReplacceColumn.Clear();

						this.uxReplacceColumn.SetValueColumns(canvas.ReportColumns);

						this.uxReplacceColumn.WhereHeader = canvas.GetReplacementColumn(dataObject.ColumnModel.TableName, dataObject.ColumnModel.ColumnName);

						this.uxReplacceColumn.InvokeMethodSetup = canvas.GetInvokeMethod(dataObject.ColumnModel.TableName, dataObject.ColumnModel.ColumnName);

						this.uxReplacceColumn.RefreshCaptionWidth();

						this.uxUpdate.Clear();

						this.uxUpdate.SetValueColumns(canvas.ReportColumns);

						this.uxUpdate.UpdateStatement = canvas.GetUpdateStatement(dataObject.ColumnModel.TableName, dataObject.ColumnModel.ColumnName);
					}
					else
					{
						this.uxReplacceColumnCaption.Visibility = Visibility.Collapsed;

						this.uxReplacceColumn.Visibility = Visibility.Collapsed;

						this.uxUpdateCaption.Visibility = Visibility.Collapsed;

						this.uxUpdate.Visibility = Visibility.Collapsed;
					}

					this.uxPropertiesCaption.Visibility = Visibility.Visible;

					this.uxProperties.Visibility = Visibility.Visible;

					this.uxWhereBuilder.Visibility = Visibility.Collapsed;

					this.uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

					this.uxCanvasSql.Visibility = Visibility.Collapsed;

					this.uxCanvasSqlCaption.Visibility = Visibility.Collapsed;

					this.selectedReportObject = null;

					this.selectedReportObject = reportObject as UIElement;

					this.selectedReportObject.PreviewMouseRightButtonUp += this.SelecteReportObject_RightClick;

					this.uxProperties.Items.Add(reportObject);
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
				this.RefreshSectionTitles();
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
				ReportSection dataSection = this.dataReportSections
					.FirstOrDefault(rp => rp.SectionTableName == column.TableName
										&& rp.SectionType == SectionTypeEnum.TableData);

				if (dataSection == null)
				{
					int newGroupIndex = this.CreateDatareportSections();

					foreach (ReportSection section in this.dataReportSections.Where(si => si.SectionGroupIndex == newGroupIndex))
					{
						section.SectionTableName = column.TableName;
					}

					dataSection = this.dataReportSections
					.FirstOrDefault(rp => rp.SectionGroupIndex == newGroupIndex
										&& rp.SectionType == SectionTypeEnum.TableData);
				}

				dataSection.AddReportColumn(column);

				this.RefreshSectionTitles();
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

						alignmentItem.Click += this.Alignment_Click;

						menu.Items.Add(alignmentItem);
					}
				}

				if (ResizeHandles.SelectedElementCount == 1)
				{
					MenuItem delete = new MenuItem { Name = "uxDelete", Header = "Delete Object" };

					delete.Click += this.SelectedMenuItem_Click;

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
				SectionCanvas canvas = (SectionCanvas)this.selectedReportObject.FindParentControlBase(typeof(SectionCanvas));

				ResizeHandles.RemoveElement(this.selectedReportObject);

				canvas.Children.Remove(this.selectedReportObject);

				this.selectedReportObject = null;
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

						foreach (ReportSection section in this.uxReportSections.Children)
						{
							section.PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum;
						}

						break;

					case "PageOrientationEnum":

						foreach (ReportSection section in this.uxReportSections.Children)
						{
							section.PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum;
						}

						break;

					case "PageMarginLeft":
					case "PageMarginRight":

					case "PageMarginTop":
					case "PageMarginBottom":

						if (this.reportDesignType == ReportTypeEnum.ReportContent
							|| this.reportDesignType == ReportTypeEnum.PageHeaderAndFooter)
						{
							break;
						}

						foreach (ReportSection section in this.uxReportSections.Children)
						{
							section.MarkerTopMargin = this.ReportMaster.PageMarginTop.ToInt32();

							section.MarkerBottomMargin = this.ReportMaster.PageMarginBottom.ToInt32();

							section.RefreshPageStaticMarkers();
						}

						break;

					case "ReportXMLVersion":

						int version = this.ReportMaster.ReportXMLVersion;

						ReportTablesRepository repo = new ReportTablesRepository();

						if (repo.GetReportXmlVersions(this.ReportMaster.MasterReport_Id).Contains(version))
						{
							this.LoadReportXml();
						}
						else
						{
							this.InitializeReportSections();

							this.SetMainDataSource();
						}

						this.CheckActiveVersion();

						break;

				}

				this.RefreshMarginMarkers();
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

						this.selectedReportObject.SetPropertyValue("ImagePath", dlg.FileName);

						break;

					default:

						ColourPicker picker = new ColourPicker();

						if (picker.ShowDialog().IsFalse())
						{
							return;
						}

						this.selectedReportObject.SetPropertyValue(buttonKey, picker.SelectedColour);

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
				DatasourceSelector selector = new DatasourceSelector(this.ReportMaster.MasterReport_Id);

				if (ControlDialog.ShowDialog("Data Source", selector, "Accept").IsFalse())
				{
					return;
				}

				this.DataSourceMainTable = selector.MainTable;

				this.LoadDataSource();
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

				defaultItem.Click += this.ReportPrintConnection_Cliked;

				menu.Items.Add(defaultItem);

				foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
				{
					MenuItem alternativeItem = new MenuItem
					{
						Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})",
						Tag = connectionKey.Key
					};

					alternativeItem.Click += this.ReportPrintConnection_Cliked;

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
				this.ReportMaster.PropertyChanged -= this.ReportMaster_PropertyChanged;

				if (this.uxReportMasterModel.HasValidationError)
				{
					return;
				}

				MenuItem item = (MenuItem)e.Source;

				PrintPreview preview = new PrintPreview(this.ReportMaster.ReportName, this.GetPrintCanvases(item));

				ControlDialog.Show("Reports", preview, string.Empty, showOkButton: false, windowState: WindowState.Maximized);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
			finally
			{
				this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
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

				defaultItem.Click += this.ExportPdfConnection_Cliked;

				menu.Items.Add(defaultItem);

				foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
				{
					MenuItem alternativeItem = new MenuItem
					{
						Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})",
						Tag = connectionKey.Key
					};

					alternativeItem.Click += this.ExportPdfConnection_Cliked;

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
				this.ReportMaster.PropertyChanged -= this.ReportMaster_PropertyChanged;

				if (this.uxReportMasterModel.HasValidationError)
				{
					return;
				}

				SaveFileDialog dlg = new SaveFileDialog();

				dlg.FileName = this.ReportMaster.ReportName;

				dlg.Filter = "PDF Files | *.pdf";

				dlg.OverwritePrompt = false;

				if (dlg.ShowDialog().IsFalse())
				{
					return;
				}

				MenuItem item = (MenuItem)e.Source;

				CanvasToPDF pdf = new CanvasToPDF();

				string result = pdf.ConvertToPdf(dlg.FileName, this.GetPrintCanvases(item));

				Process.Start(result);

			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
			finally
			{
				this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
			}
		}

		private void ReportSumDataSectionIndex_Changed(object sender, Guid elemntId, int sectionIndex)
		{
			try
			{
				ModelViewItem viewerItem = this.uxProperties["Sum Column"];

				if (viewerItem == null)
				{
					return;
				}

				ReportSum item = sender.To<ReportSum>();

				ReportSection canvas = this.dataReportSections.FirstOrDefault(i => i.SectionIndex == sectionIndex
																			    && i.SectionType == SectionTypeEnum.TableData);

				viewerItem.SetComboBoxItems(new DataItemModel[] { });

				item.SumColumnValues = new DataItemModel[] { };
				
				if (canvas == null )
				{   // ToDo Clear columns
					return;
				}

				item.SectionGroupIndex = canvas.SectionGroupIndex;

				DataItemModel[] columnItems = canvas.ReportColumns
					.Select(rc => new DataItemModel { DisplayValue = rc.ColumnName, ItemKey = rc.ColumnName })
					.ToArray();

				item.SumColumnValues = columnItems;

				viewerItem.SetComboBoxItems(columnItems);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void ReportSectionDelete_Request(object sender, int sectionGroupIndex)
		{
			try
			{
				List<ReportSection> removeSection = this.dataReportSections
					.Where(s => s.SectionGroupIndex == sectionGroupIndex)
					.ToList();
				
				foreach(ReportSection section in removeSection)
				{
					this.uxReportSections.Children.Remove(section);

					this.dataReportSections.Remove(section);
				}

				int iterationGroupIndex = sectionGroupIndex + 1;

				int maxGroupIndex = this.GetMaxSectionGroupIndex();

				while(iterationGroupIndex <= maxGroupIndex)
				{
					foreach (ReportSection group in this.dataReportSections
						.Where(s => s.SectionGroupIndex == iterationGroupIndex))
					{
						group.SectionGroupIndex = (iterationGroupIndex - 1);
					}

					++iterationGroupIndex;
				}

				ReportSection[] orderBySections = this.dataReportSections
					.OrderBy(si => si.SectionIndex)
					.ToArray();

				for (int x = 0; x < this.dataReportSections.Count; ++x)
				{
					orderBySections[x].SectionIndex = x;
				}

				this.RefreshSectionTitles();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		#endregion

		#region PRIVATE PROPERTIES

		private int SelectedSectionIndex { get; set; }

		private int SelectedSectionGroupIndex { get; set; }

		private double CanvasWidth
		{
			get
			{
				return ((ReportSection)this.uxReportSections.Children[0]).GetPropertyValue("PageWidth").ToDouble();
			}
		}

		private double CanvasHeight
		{
			get
			{
				return this.uxReportSections.ActualHeight + 25;
			}
		}

		#endregion

		#region PRIVATE METHODS

		private void CheckActiveVersion()
		{
			ReportTablesRepository repo = new ReportTablesRepository();

			this.ReportMaster.IsActiveVesion = repo.IsActiveVersion(this.ReportMaster.MasterReport_Id, this.ReportMaster.ReportXMLVersion);
		}

		private void SetSumDataSections(object reportObject)
		{
			ReportSum item = reportObject.To<ReportSum>();

			List<DataItemModel> result = new List<DataItemModel>();

			result.Add(new DataItemModel { DisplayValue = Constants.None, ItemKey = -1 });

			foreach (ReportSection canvas in this.dataReportSections.Where(d => d.SectionType == SectionTypeEnum.TableData))
			{
				result.Add(new DataItemModel { DisplayValue = canvas.SectionTitle, ItemKey = canvas.SectionIndex });
			}

			item.DataSections = result.ToArray();

			item.ParentSectionGroupIndex = this.SelectedSectionGroupIndex;

			item.DataSectionIndexChanged += this.ReportSumDataSectionIndex_Changed;

			ReportSection sectionCanvas = this.dataReportSections.FirstOrDefault(i => i.SectionIndex == item.SectionIndex
																				&& i.SectionType == SectionTypeEnum.TableData);

			if (sectionCanvas == null)
			{
				return;
			}

			DataItemModel[] columnItems = sectionCanvas.ReportColumns
					.Select(rc => new DataItemModel { DisplayValue = rc.ColumnName, ItemKey = rc.ColumnName })
					.ToArray();


			item.SumColumnValues = columnItems;
		}

		private void ClearSections()
		{
			foreach(ReportSection section in this.dataReportSections)
			{
				section.ReportObjectSelected -= this.ReportObject_Selected;

				section.ReportSectionDeleteRequest -= this.ReportSectionDelete_Request;

				section.RequestNewDataSections -= this.NewDataSection_Requested;

				section.ReportColumnAdded -= this.ReportColumn_Added;
			}

			this.uxReportSections.Children.Clear();

			this.dataReportSections.Clear();
		}

		private void LoadReportXml()
		{
			this.ClearSections();

			ReportTablesRepository repo = new ReportTablesRepository();

			ReportXMLModel reportXML = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, this.ReportMaster.MasterReport_Id);

			string reportXml = reportXML.BinaryXML.UnzipFile().ParseToString();

			XDocument report = XDocument.Parse(reportXml);

			this.reportDesignType = (ReportTypeEnum)report.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value.ToInt32();

			foreach (XElement sectionElement in report.Root.Element("ReportSettings").Elements("ReportSection").OrderBy(si => si.Attribute("SectionIndex").Value.ToInt32()))
			{
				ReportSection section = new ReportSection { IsDesignMode = true };

				section.SectionXml = sectionElement;

				this.uxReportSections.Children.Add(section);

				section.ReportObjectSelected += this.ReportObject_Selected;

				if (section.SectionType == SectionTypeEnum.TableData)
				{
					section.RequestNewDataSections += this.NewDataSection_Requested;

					section.ReportColumnAdded += this.ReportColumn_Added;
				}
				else if (this.reportDesignType == ReportTypeEnum.CoverPage
					|| this.reportDesignType == ReportTypeEnum.FinalPage)
				{
					section.RefreshPageStaticMarkers();
				}

				if (section.SectionGroupIndex > 0)
				{
					section.ReportSectionDeleteRequest += this.ReportSectionDelete_Request;
				}

				this.dataReportSections.Add(section);
			}
		}

		private void InitializeReportSections()
		{
			this.ClearSections();

			switch (this.reportDesignType)
			{
				case ReportTypeEnum.CoverPage:
				case ReportTypeEnum.FinalPage:

					this.uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 0,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Page,
						IsDesignMode = true,
						PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					}); ;

					break;

				case ReportTypeEnum.ReportContent:

					this.CreateDatareportSections();

					break;

				case ReportTypeEnum.PageHeaderAndFooter:

					this.uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 0,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Header,
						IsDesignMode = true,
						PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					});

					this.uxReportSections.Children.Add(new ReportSection
					{
						SectionIndex = 1,
						SectionGroupIndex = 0,
						SectionType = SectionTypeEnum.Footer,
						IsDesignMode = true,
						PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
						PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
						MarkerBottomMargin = 100,
						MarkerTopMargin = 100
					});

					break;
			}

			foreach (ReportSection section in this.uxReportSections.Children)
			{
				section.ReportObjectSelected += this.ReportObject_Selected;

				if (section.SectionType == SectionTypeEnum.TableData)
				{
					section.RequestNewDataSections += this.NewDataSection_Requested;

					section.ReportColumnAdded += this.ReportColumn_Added;
				}

				section.RefresSectionTitle();
			}
		}

		private void InitializeToolsStack()
		{
			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Label", ToolType = typeof(ReportLabel) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Border", ToolType = typeof(ReportBorder) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Current Date", ToolType = typeof(CurrentDate) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Image", ToolType = typeof(ReportImage) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Horizontal Line", ToolType = typeof(ReportHorizontalLine) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Vertical Line", ToolType = typeof(ReportVerticalLine) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Page Break", ToolType = typeof(ReportPageBreak) });

			this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Report Sum", ToolType = typeof(ReportSum) });
		}

		private void RefreshMarginMarkers()
		{
			double pageWidth = this.CanvasWidth;

			this.uxHorizontalRuler.ClearMarkers(true);

			this.uxHorizontalRuler.Refresh(pageWidth, 25, this.CanvasHeight);

			this.uxHorizontalRuler.AddMarker(this.ReportMaster.PageMarginLeft, true);

			this.uxHorizontalRuler.AddMarker((pageWidth - this.ReportMaster.PageMarginRight), true);
		}

		private void SetupDataReportsOptions()
		{
			if (this.reportDesignType != ReportTypeEnum.ReportContent)
			{
				return;
			}

			this.uxReportMasterModel["Cover Page"].Visibility = Visibility.Visible;

			this.uxReportMasterModel["Page Headers and Footers"].Visibility = Visibility.Visible;

			this.uxReportMasterModel["Final Page"].Visibility = Visibility.Visible;

			this.uxReportMasterModel["Production Connection"].Visibility = Visibility.Visible;

			this.uxReportMasterModel["Production Connection"].IsRequired = true;

			this.uxDataMenueBorder.Visibility = Visibility.Visible;

			this.uxDataMenue.Visibility = Visibility.Visible;

			this.uxTableTree.Visibility = Visibility.Visible;

			DataSourceRepository repo = new DataSourceRepository();

			DataSourceMasterModel sourceMaster = repo.GetDataSourceMasterByPrimaryKey(this.ReportMaster.MasterReport_Id);

			if (sourceMaster != null)
			{
				sourceMaster.SelectedSourceTables.AddRange(repo.GetDataSourceTableByForeignKeyMasterReport_Id(this.ReportMaster.MasterReport_Id));

				this.DataSourceMainTable = sourceMaster;

				this.LoadDataSource();
			}

			this.uxReportMasterModel.AllignAllCaptions();
		}

		private void SetMainDataSource()
		{
			foreach (ReportSection section in this.uxReportSections.Children)
			{
				if (section.SectionGroupIndex != 0)
				{
					continue;
				}

				section.SectionTableName = this.DataSourceMainTable.MainTableName; // Do this to reserve the first section for the main data source

				section.RefresSectionTitle();
			}
		}

		private void LoadDataSource()
		{
			this.uxTableTree.Items.Clear();

			this.SetMainDataSource();

			TreeViewItem mainTreeItem = new TreeViewItem { Header = this.DataSourceMainTable.MainTableName };

			foreach (DataItemModel column in Integrity.GetColumnsForTable(this.DataSourceMainTable.MainTableName))
			{
				ColumnObjectModel tableColumn = Integrity.GetObjectModel(this.DataSourceMainTable.MainTableName, column.DisplayValue);

				ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = this.DataSourceMainTable.MainTableName }) as ReportColumnModel;

				ToolsMenuItem treeMainColumn = new ToolsMenuItem
				{
					Caption = column.DisplayValue,
					ToolType = typeof(ReportDataObject),
					Tag = column.CopyToObject(reportColumn)
				};

				mainTreeItem.Items.Add(treeMainColumn);
			}

			this.uxTableTree.Items.Add(mainTreeItem);

			foreach (DataSourceTableModel sourceTable in this.DataSourceMainTable.SelectedSourceTables)
			{
				if (sourceTable.TableName == this.DataSourceMainTable.MainTableName)
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

				this.uxTableTree.Items.Add(childTreeItem);
			}
		}

		private XDocument GetReportXml()
		{
			if (this.selectedReportObject != null && this.isDataObject)
			{
				ReportSection canvas = this.dataReportSections.FirstOrDefault(d => d.SectionIndex == this.SelectedSectionIndex);

				canvas.UpdateReplacementColumn(this.uxReplacceColumn.WhereHeader);

				canvas.UpdateInvokeReplaceModel(this.uxReplacceColumn.InvokeMethodSetup);

				canvas.UpdateUpdateStatement(this.uxUpdate.UpdateStatement);
			}

			XDocument result = new XDocument();

			XElement root = new XElement("Root");

			XElement report = new XElement("ReportSettings");

			report.Add(new XAttribute("ReportTypeEnum", (int)this.reportDesignType));

			foreach (ReportSection section in this.uxReportSections.Children)
			{
				report.Add(section.SectionXml);
			}

			root.Add(report);

			result.Add(root);

			return result;
		}

		private Dictionary<int, PrintCanvas> GetPrintCanvases(MenuItem item)
		{
			XDocument reportXml = this.GetReportXml();

			List<ReportXMLPrintParameterModel> parameterFilters = this.GetReportparameterFilters(reportXml);

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

			XDocument repotXml = xmlBuild.GetReport(reportXml, connection, this.ReportMaster.CopyToObject(new ReportMaster()) as ReportMaster, parameterFilters);

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
					ReportXMLVersion = this.ReportMaster.ReportXMLVersion,
					MasterReport_Id = this.ReportMaster.MasterReport_Id,
					FilterCaption = item.Attribute("PrintParameterCaption").Value,
					DefaultValue = item.Attribute("PrintParameterDefaultValue").Value,
					IsActive = true,
					IsRequired = item.Attribute("IsRequiredParameter").Value.TryToBool()
				});
			}

			return resut;
		}

		#endregion

		#region NEW DATA SECTION METHODS

		private int CreateDatareportSections()
		{
			int groupSectionId = this.NextGroupIndex();

			int[] sectionIndex = this.CalculateSectionIndexes();

			ReportSection header = new ReportSection
			{
				SectionIndex = sectionIndex[0],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableHeader,
				IsDesignMode = true,
				PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			this.uxReportSections.Children.Insert(sectionIndex[0], header);

			this.dataReportSections.Add(header);

			ReportSection data = new ReportSection
			{
				SectionIndex = sectionIndex[1],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableData,
				IsDesignMode = true,
				PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			this.uxReportSections.Children.Insert(sectionIndex[1], data);

			this.dataReportSections.Add(data);

			ReportSection footer = new ReportSection
			{
				SectionIndex = sectionIndex[2],
				SectionGroupIndex = groupSectionId,
				SectionType = SectionTypeEnum.TableFooter,
				IsDesignMode = true,
				PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
				PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
				MarkerBottomMargin = 100,
				MarkerTopMargin = 100
			};

			this.uxReportSections.Children.Insert(sectionIndex[2], footer);

			this.dataReportSections.Add(footer);

			header.ReportObjectSelected += this.ReportObject_Selected;

			header.ReportSectionDeleteRequest += this.ReportSectionDelete_Request;

			data.ReportSectionDeleteRequest += this.ReportSectionDelete_Request;

			footer.ReportSectionDeleteRequest += this.ReportSectionDelete_Request;

			data.ReportObjectSelected += this.ReportObject_Selected;

			data.RequestNewDataSections += this.NewDataSection_Requested;

			data.ReportColumnAdded += this.ReportColumn_Added;

			footer.ReportObjectSelected += this.ReportObject_Selected;

			return groupSectionId;
		}

		private void RefreshSectionTitles()
		{
			foreach (ReportSection section in this.dataReportSections)
			{
				section.RefresSectionTitle();
			}
		}

		private int NextGroupIndex()
		{
			return this.dataReportSections.Count == 0 ? 0 :
				(this.dataReportSections.Select(gr => gr.SectionGroupIndex).Max() + 1);
		}

		private int GetMaxSectionGroupIndex()
		{   // Zero based index
			return this.dataReportSections.Count == 0 ? 0 :
				(this.dataReportSections.Select(gr => gr.SectionGroupIndex).Max());
		}

		private int GetMaxSectionIndex()
		{   // Zero based index
			return this.dataReportSections.Count == 0 ? 2 :
				(this.dataReportSections.Select(gr => gr.SectionIndex).Max());
		}

		private int[] CalculateSectionIndexes()
		{
			int nextGroupIndex = this.NextGroupIndex();

			if (nextGroupIndex == 0)
			{
				return new int[] { 0, 1, 2 };
			}

			int maxSectionIndex = this.GetMaxSectionIndex();

			int insertIndexStart = ((maxSectionIndex + 1) / 3) + ((maxSectionIndex + 1) / 3);

			int[] result = new int[3];

			result[0] = insertIndexStart;
			result[1] = insertIndexStart + 1;
			result[2] = insertIndexStart + 2;

			int shiftIndex = result[2] + 1;

			ReportSection[] changeSections = this.dataReportSections
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
