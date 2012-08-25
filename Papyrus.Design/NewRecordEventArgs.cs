using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus.Design
{

	public class NewRecordEventArgs : EventArgs
	{

		public Record NewRecord { get; private set; }

		public NewRecordEventArgs(Record newRecord)
		{
			NewRecord = newRecord;
		}


	}
}
