using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{

	[ProtoContract]
	abstract class IRecordList
	{

		public abstract List<DataPointer> GetDataPointers();

		public abstract List<string> GetDependencies();

		public abstract Type RecordType { get; }

		public abstract List<IRecordContainer> GetRecords();

		public abstract List<IDataPointerList> GetDataPointerLists();

	}

}
