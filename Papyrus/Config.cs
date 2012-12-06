using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	public static class Config
	{

		/// <summary>
		/// Set to true to ignore data pointer resolution errors
		/// </summary>
		public static bool IgnoreDataPointerErrors = false;

		/// <summary>
		/// The default record database to use for new data pointers. (For use in the editor)
		/// </summary>
		public static RecordDatabase DefaultRecordDatabase = null;

		public delegate bool DataPointerErrorDelegate(DataPointer dataPointer);

		/// <summary>
		/// Method to call when a data pointer resolution error occurs. Return true from this callback
		/// to resume loading
		/// </summary>
		public static DataPointerErrorDelegate DataPointerErrorCallback;

	}

}
