using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class ReportXMLBase : ModelsBase
	{
		private int _ReportXMLVersion;
		private Int64 _MasterReport_Id;
		private byte[] _BinaryXML;
		private Int64 _PrintCount;


		// Primary Keys
		[Key]
		 	
		public int ReportXMLVersion
		{
			get
			{
				return this._ReportXMLVersion;
			}

			set
			{
				base.OnPropertyChanged("ReportXMLVersion", ref this._ReportXMLVersion, value);
			}
		}
		[Key]
		 	
		public Int64 MasterReport_Id
		{
			get
			{
				return this._MasterReport_Id;
			}

			set
			{
				base.OnPropertyChanged("MasterReport_Id", ref this._MasterReport_Id, value);
			}
		}


		// Foreign Keys


		// Columns
		public byte[] BinaryXML
		{
			get
			{
				return this._BinaryXML;
			}

			set
			{
				base.OnPropertyChanged("BinaryXML", ref this._BinaryXML, value);
			}
		}
		public Int64 PrintCount
		{
			get
			{
				return this._PrintCount;
			}

			set
			{
				base.OnPropertyChanged("PrintCount", ref this._PrintCount, value);
			}
		}


	}
}