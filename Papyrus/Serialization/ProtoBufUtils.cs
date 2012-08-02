using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf.Meta;

namespace Papyrus.Serialization
{

	internal static class ProtoBufUtils
	{
		public static RuntimeTypeModel TypeModel;

		private static List<Type> _handled; 

		public static void Initialise()
		{

			_handled = new List<Type>(100);
			TypeModel = ProtoBuf.Meta.TypeModel.Create();

			var types = RecordDatabase.GetRecordTypes();

			var fieldNo = 50;

			TagRecordClass(TypeModel, typeof(Record));
			var rType = TypeModel.Add(typeof (Record), false);

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

			var subRecords = assembly.GetTypes().Where(p => Attribute.IsDefined(p, typeof (SubRecordAttribute)));

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

			// Tag any subtypes
			foreach (var attrib in (ChildRecordAttribute[])type.GetCustomAttributes(typeof(ChildRecordAttribute), false))
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


	}

}
