using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus.Design
{

	public class RecordEventArgs : EventArgs
	{

		public Record Record { get; private set; }

		public RecordEventArgs(Record record)
		{
			Record = record;
		}


	}
}
