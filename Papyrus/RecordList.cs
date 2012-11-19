/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{

	[ProtoContract]
	[DataContract]
	internal class RecordList<T> : IRecordList where T : Record
	{

		/// <summary>
		/// The underlying record type
		/// </summary>
		public override Type RecordType
		{
			get { return typeof (T); }
		}

		/// <summary>
		/// All the non-append records contained in this list.
		/// </summary>
		[ProtoMember(1, OverwriteList = true)]
		[DataMember]
		public List<RecordContainer<T>> Records { get; protected set; }

		public RecordList()
		{
			Records = new List<RecordContainer<T>>();
		}

		/// <summary>
		/// Returns a list of all the records in this list.
		/// </summary>
		/// <returns></returns>
		public override List<IRecordContainer> GetRecords()
		{
			return Records.Cast<IRecordContainer>().ToList();
		}

		/// <summary>
		/// Returns a list of data pointers that records in this list contain
		/// </summary>
		/// <returns></returns>
		public override List<DataPointer> GetDataPointers()
		{

			var dataPointers = new List<DataPointer>();

			foreach (var record in Records) {
				dataPointers.AddRange(record.Record.GetDataPointers().Except(dataPointers));
			}

			return dataPointers;

		}

		/// <summary>
		/// Gets a list of plugins that records in this list depend on
		/// </summary>
		/// <returns></returns>
		public override List<string> GetDependencies()
		{

			List<string> sources = new List<string>();

			foreach (var recordContainer in Records) {

				sources.AddRange(recordContainer.Record.GetDependencies().Except(sources));

			}

			return sources;
		}

		/// <summary>
		/// Returns a list of data pointer lists in this record list.
		/// </summary>
		/// <returns></returns>
		public override List<IDataPointerList> GetDataPointerLists()
		{

			var retList = new List<IDataPointerList>();

			foreach (var recordContainer in Records) {
				retList.AddRange(recordContainer.Record.GetDataPointerLists());
			}

			return retList;

		}
	}

}
