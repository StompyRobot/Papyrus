/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{
	/// <summary>
	/// Contains a Record item and the instructions on how to merge it into the main database.
	/// Keeps track of where a record came from and how to resolve a pointer back to it.
	/// </summary>
	[DataContract]
	[ProtoContract]
#if JSON
	[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
#endif
	internal class RecordContainer<T> : IRecordContainer where T : Record
	{

		[DataMember]
		[ProtoMember(1)]
#if JSON
		[Newtonsoft.Json.JsonProperty(DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate)]
#endif
		public RecordMode Mode { get; set; }

		/// <summary>
		/// The plugin that this record is located in.
		/// </summary>
		[DataMember]
		[ProtoMember(2)]
		public string Location { get; set; }

		/// <summary>
		/// The index that this record is located at in the plugin.
		/// </summary>
		[DataMember]
		[ProtoMember(3)]
		public int Index { get; set; }

		/// <summary>
		/// The plugin that this record will attempt to place itself at. Ignored when type is Append.
		/// </summary>
		[DataMember]
		[ProtoMember(4)]
		public string Destination { get; set; }

		/// <summary>
		/// The record this container 'contains'.
		/// </summary>
		[DataMember]
		[ProtoMember(5)]
		public T Record { get; set; }

		/// <summary>
		/// The type of record this container contains
		/// </summary>
#if JSON
		[Newtonsoft.Json.JsonIgnore]
#endif
		public Type RecordType { get { return typeof (T); } }
		
		[ProtoAfterDeserialization]
		private void PostDeserialization()
		{

			Record.Container = this;

			if (string.IsNullOrEmpty(Destination))
				Destination = Location;

		}

#if JSON

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			PostDeserialization();
		}

#endif

		public void SetRecord(Record record)
		{
#if DEBUG
			Debug.Assert(record is T, "Record must be of type " + typeof(T).Name);
#endif
			Record = record as T;
			Record.Container = this;
		}

		public Record GetRecord()
		{
			return Record;
		}

		public RecordContainer() {}


	}
}
