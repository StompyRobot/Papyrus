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
using System.Reflection;
using Papyrus.DataTypes;
using ProtoBuf.Meta;

namespace Papyrus.Serialization.Utilities
{

	public static class ProtoBufUtils
	{
		public static RuntimeTypeModel TypeModel;

		private static List<Type> _handled;

		private static int _recordListFieldNo = 0;

		public static void Initialise()
		{

			_handled = new List<Type>(100);
			TypeModel = ProtoBuf.Meta.TypeModel.Create();

			var types = RecordDatabase.GetRecordTypes();
			_recordListFieldNo = 1;
			var fieldNo = 50;

			TagRecordClass(TypeModel, typeof(Record));
			var rType = TypeModel.Add(typeof (Record), false);

			RecordDatabase.RootRecords.Sort(TypeAlphaSort);

			foreach (var rootRecord in RecordDatabase.RootRecords) {

				ScanSubRecords(TypeModel, rootRecord.Assembly);
				TagRecordClass(TypeModel, rootRecord);
				rType.AddSubType(fieldNo, rootRecord);
				++fieldNo;

			}

		}

		/// <summary>
		/// Scans an assembly and updates the type model with any SubRecords found
		/// </summary>
		/// <param name="assembly"></param>
		private static void ScanSubRecords(RuntimeTypeModel model, Assembly assembly)
		{

			var subRecords = assembly.GetTypes().Where(p => Attribute.IsDefined((MemberInfo) p, typeof (SubRecordAttribute))).ToList();
			subRecords.Sort(TypeAlphaSort);

			foreach (var subRecord in subRecords) {
				TagRecordClass(model, subRecord);
			}

		}

		/// <summary>
		/// Tags a class with 
		/// </summary>
		/// <param name="type"></param>
		public static void TagRecordClass(RuntimeTypeModel model, Type type)
		{

			if (_handled.Contains(type))
				return;

			_handled.Add(type);

			var rType = model.Add(type, false);

			var children = ((ChildRecordAttribute[]) type.GetCustomAttributes(typeof (ChildRecordAttribute), false)).ToList();
			children.Sort((a1, a2) => System.String.CompareOrdinal(a1.ChildType.Name, a2.ChildType.Name));	

			// Tag any subtypes
			foreach (var attrib in children)
			{


				TagRecordClass(model, attrib.ChildType); // Tell protobuf how to serialize the child object
				rType.AddSubType(attrib.FieldNo, attrib.ChildType); // Then add it as a subtype of this

			}

			// Tag any fields
			foreach (var memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{

				if(memberInfo.DeclaringType != type)
					continue;
				

				if (Attribute.IsDefined(memberInfo, typeof(RecordPropertyAttribute)))
				{

					var recordProp = (RecordPropertyAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(RecordPropertyAttribute));

					var member = rType.AddField(recordProp.FieldNumber, memberInfo.Name);

					member.DynamicType = recordProp.DynamicType;
					member.OverwriteList = recordProp.OverwriteList;

				}

			}

			// If it is a full record, create a record list type for it.
			if (typeof(Record).IsAssignableFrom(type)) {

				var recordListType = typeof (RecordList<>).MakeGenericType(type);

				TypeModel.Add(typeof (IRecordList), true).AddSubType(++_recordListFieldNo, recordListType);

			}



		}


		/// <summary>
		/// Overwrites record with the values from with.
		/// </summary>
		public static void OverWrite<T>(T record, T with)
		{

			using (MemoryStream str = new MemoryStream()) {
				
				TypeModel.Serialize(str, with);
				str.Seek(0, SeekOrigin.Begin);
				TypeModel.Deserialize(str, record, typeof (T));

			}

		}

		public static int TypeAlphaSort(Type t1, Type t2)
		{
			return System.String.CompareOrdinal(t1.Name, t2.Name);
		}


	}

}
