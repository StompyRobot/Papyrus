using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus.Design
{
	/// <summary>
	/// Some extension methods for records
	/// </summary>
	public static class RecordExtensions
	{

		/// <summary>
		/// Returns the name of the plugin this record is sourced from
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static string RecordLocation(this Record record)
		{
			return record.Container.Location;
		}

		/// <summary>
		/// Returns the name of the plugin this record is placed. ie the plugin
		/// that the record this record replaces resides in
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static string RecordDestination(this Record record)
		{
			return record.Container.Destination;
		}

		/// <summary>
		/// Serialises a record and compares the binary output to another record to detect
		/// differences
		/// </summary>
		/// <param name="record"></param>
		/// <param name="otherRecord"></param>
		/// <returns>True if the records are the same</returns>
		public static bool BinaryEquals(this Record record, Record otherRecord)
		{

			using (MemoryStream str = new MemoryStream()) {

				ProtoBuf.Serializer.Serialize(str, record);

				var recordBytes = str.GetBuffer();

				using (MemoryStream str2 = new MemoryStream()) {

					ProtoBuf.Serializer.Serialize(str2, otherRecord);

					var otherRecordBtyes = str2.GetBuffer();

					if (recordBytes.SequenceEqual(otherRecordBtyes))
						return true;

				}

			}

			return false;

		}

	}
}
