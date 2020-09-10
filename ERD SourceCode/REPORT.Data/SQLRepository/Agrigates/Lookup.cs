using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class Lookup : ModelsBase
    {
	private string _LookupGroup;
	private int _GroupKey;
	private string _GroupDescription;

	/// <summary>
	/// <para>Lookup Group</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Group Key</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Group Description</para>
	/// <para></para>
	/// </summary>
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