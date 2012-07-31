using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes.Hardpoints;

namespace Papyrus.Design
{
	public static class HardpointUtils
	{

		/// <summary>
		/// Resolves any pointers in this hardpoint. This is needed after adding a hardpoint without reloading the database.
		/// </summary>
		/// <param name="hardpoint"></param>
		/// <param name="database"></param>
		public static void ResolvePointers(Hardpoint hardpoint, RecordDatabase database)
		{
			hardpoint.ResolveDependencies(database);
		}

	}
}
