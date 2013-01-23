/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Papyrus.DataTypes;
using Papyrus.Serialization.Utilities;

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
		/// Returns true if this record can be pointed to. Generally returns false if a record
		/// has not been saved at least once.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static bool CanGetReference(this Record record)
		{
			return !string.IsNullOrEmpty(record.Container.Location) && record.Container.Index >= 0;
		}

		/// <summary>
		/// Returns true if this record is in the current active plugin (plugin being edited).
		/// </summary>
		/// <param name="record"></param>
		/// <returns>True if record is located in the active plugin.</returns>
		public static bool InActivePlugin(this Record record)
		{

			if(!(record.Database is MutableRecordDatabase))
				return false;

			return record.RecordLocation() == (record.Database as MutableRecordDatabase).ActivePluginName;

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

				ProtoBufUtils.TypeModel.Serialize(str, record);

				var recordBytes = str.GetBuffer();

				using (MemoryStream str2 = new MemoryStream()) {

					ProtoBufUtils.TypeModel.Serialize(str2, otherRecord);

					var otherRecordBtyes = str2.GetBuffer();

					if (recordBytes.SequenceEqual(otherRecordBtyes))
						return true;

				}

			}

			return false;

		}

		/// <summary>
		/// Serialises a record and compares the json output to another record to detect
		/// differences
		/// </summary>
		/// <param name="record"></param>
		/// <param name="otherRecord"></param>
		/// <returns>True if the records are the same</returns>
		public static bool JsonEquals(this Record record, Record otherRecord)
		{

			using (var str = new StringWriter()) {

				JsonUtilities.GetPapyrusJsonSerializer().Serialize(str, record);

				var output = str.GetStringBuilder().ToString();

				str.GetStringBuilder().Length = 0;

				JsonUtilities.GetPapyrusJsonSerializer().Serialize(str, otherRecord);

				var output2 = str.GetStringBuilder().ToString();

				if (output == output2)
					return true;
	
			}

			return false;

		}

		public static Record Duplicate(this Record record)
		{

			if (!(record.Database is MutableRecordDatabase))
				return null;

			var database = record.Database as MutableRecordDatabase;

			var newRecord = database.NewRecord(record.GetType());

			ProtoBufUtils.OverWrite(newRecord, record);

			return newRecord;

		}

		/// <summary>
		/// Returns an editable copy of a record
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static Record GetEditableCopy(this Record record)
		{

			if (!(record.Database is MutableRecordDatabase))
				return null;

			return (record.Database as MutableRecordDatabase).GetEditableCopy(record);
		}

		/// <summary>
		/// Saves an editable record
		/// </summary>
		/// <param name="record"></param>
		public static void SaveEditableCopy(this Record record)
		{
			
			if(record.ReadOnly)
				throw new Exception("Attempted to save a read only record.");

			(record.Database as MutableRecordDatabase).SaveRecord(record);

		}

	}
}
