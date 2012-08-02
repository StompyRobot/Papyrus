using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	/// <summary>
	/// Marks a class as being a record
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RecordAttribute : Attribute
	{

		/// <summary>
		/// Should this record type be shown in the editor record list.
		/// </summary>
		public bool ShowInEditor { get; set; }

		public RecordAttribute()
		{
			ShowInEditor = true;
		}

	}
	
	/// <summary>
	/// Marks a class as being a subrecord, which means it can be serialized
	/// correctly inside a record.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SubRecordAttribute : Attribute
	{


		public SubRecordAttribute()
		{

		}

	}

	

}
