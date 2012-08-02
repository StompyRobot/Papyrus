using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

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

}
