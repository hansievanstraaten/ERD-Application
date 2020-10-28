using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class LookupBase : ModelsBase
	{
		private string _LookupGroup;
		private int _GroupKey;
		private string _GroupDescription;


		// Primary Keys
		[Key]
		 	
		public string LookupGroup
		{
			get
			{
				return this._LookupGroup;
			}

			set
			{
				base.OnPropertyChanged("LookupGroup", ref this._LookupGroup, value);
			}
		}
		[Key]
		 	
		public int GroupKey
		{
			get
			{
				return this._GroupKey;
			}

			set
			{
				base.OnPropertyChanged("GroupKey", ref this._GroupKey, value);
			}
		}


		// Foreign Keys


		// Columns
		public string GroupDescription
		{
			get
			{
				return this._GroupDescription;
			}

			set
			{
				base.OnPropertyChanged("GroupDescription", ref this._GroupDescription, value);
			}
		}


	}
}