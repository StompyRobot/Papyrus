using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus
{

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class PapyrusModuleAttribute : Attribute
	{
		public Guid ID { get; private set; }

		public Type RootRecord { get; private set; }

		public PapyrusModuleAttribute(string guid, Type rootRecord)
		{
			
			Debug.Assert(typeof(Record).IsAssignableFrom(rootRecord));

			RootRecord = rootRecord;
			ID = Guid.Parse(guid);

		}

	}

}
