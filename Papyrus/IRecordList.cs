/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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

		public abstract List<RecordReference> GetRecordReferences();

		public abstract List<string> GetDependencies();

		public abstract Type RecordType { get; }

		public abstract List<IRecordContainer> GetRecords();

		public abstract List<IRecordReferenceList> GetRecordReferenceLists();

	}

}
