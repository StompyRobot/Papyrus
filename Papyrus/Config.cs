using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	public static class Config
	{

		/// <summary>
		/// Set to true to ignore reference resolution errors
		/// </summary>
		public static bool IgnoreReferenceErrors = false;

		/// <summary>
		/// The default record database to use for new data pointers. (For use in the editor)
		/// </summary>
		public static RecordDatabase DefaultRecordDatabase = null;

		public delegate bool ReferenceErrorDelegate(RecordReference recordReference);

		/// <summary>
		/// Method to call when a data pointer resolution error occurs. Return true from this callback
		/// to resume loading
		/// </summary>
		public static ReferenceErrorDelegate ReferenceErrorCallback;

	}

}
