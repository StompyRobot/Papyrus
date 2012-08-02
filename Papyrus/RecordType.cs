using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	/// <summary>
	/// Represents a record type
	/// </summary>
	public class RecordType
	{

		/// <summary>
		/// Type object for the record
		/// </summary>
		public Type Type { get; internal set; }

		/// <summary>
		/// Should this record type be shown in the editor.
		/// </summary>
		public bool ShowInEditor { get; internal set; }

		/// <summary>
		/// True if this record type cannot be instantiated.
		/// </summary>
		public bool Abstract { get; internal set; }

		internal RecordType()
		{
			ShowInEditor = true;
			Type = null;
			Abstract = false;
		}

	}

}
