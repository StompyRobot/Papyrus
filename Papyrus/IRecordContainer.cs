using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{

	internal interface IRecordContainer
	{

		RecordMode Mode { get; set; }

		/// <summary>
		/// The plugin that this record is located in.
		/// </summary>
		string Location { get; set; }

		/// <summary>
		/// The index that this record is located at in the plugin.
		/// </summary>
		int Index { get; set; }

		/// <summary>
		/// The plugin that this record will attempt to place itself at. Ignored when type is Append.
		/// </summary>
		string Destination { get; set; }

		Type RecordType { get; }

		void SetRecord(Record record);

		Record GetRecord();

	}

}