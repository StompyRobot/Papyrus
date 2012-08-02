using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Papyrus.DataTypes;
using ProtoBuf.Meta;

namespace Papyrus.Serialization
{
	internal class DataProtoSerializer : IDataSerializer
	{
		private static bool _protoInitialised;

		internal static void InitProtoSystem()
		{

			if (_protoInitialised)
				return;

			_protoInitialised = true;

			// Use the static init to tell protobuf-net about ProtoIncludes

			Dictionary<Type, int> fieldNum = new Dictionary<Type, int>();
			fieldNum.Add(typeof(Record), 10); // leave a bit of room for more fields in case we need them later.
			fieldNum.Add(typeof(IRecordList), 1);

			/*var dataTypes = new List<TypeDefinition>(RecordTypes.Types);

			foreach (var recordType in dataTypes)
			{

				var parentRecordType = recordType.ParentRecordType;

				if (parentRecordType == null)
					parentRecordType = typeof(Record);

				// Make sure that the parent record has an index counter present.
				if (!fieldNum.ContainsKey(parentRecordType))
				{

					// If it doesn't exist, we'll need to create it now.
					var parentType = dataTypes.Find(p => p.Type == recordType.ParentRecordType);

					if (parentType == null)
						throw new Exception("Could not find parent type " + recordType.ParentRecordType.Name + " in Data.DataTypes.RecordTypes.Type");

					fieldNum.Add(parentType.Type, parentType.IndexStart);

				}

				// Check if the type is null. If it is null, it is there to simply keep backwards compatablity.
				if (recordType.Type != null)
				{

					// Check if this record needs to be added to the index counter
					if (!fieldNum.ContainsKey(recordType.Type))
						fieldNum.Add(recordType.Type, recordType.IndexStart);

					// Add this type to the runtime type model as a subtype of the parent type, using the field number
					ProtoBuf.Meta.RuntimeTypeModel.Default[parentRecordType].AddSubType(fieldNum[parentRecordType], recordType.Type);

					// Also add this to the runtime type model of the IRecordList
					ProtoBuf.Meta.RuntimeTypeModel.Default[typeof(IRecordList)].AddSubType(fieldNum[typeof(IRecordList)],
																							typeof(RecordList<>).MakeGenericType(
																								recordType.Type));

				}

				// Increment the field number. Always increment this because otherwise breaks backwards compatability
				fieldNum[parentRecordType] = fieldNum[parentRecordType] + 1;
				fieldNum[typeof(IRecordList)] = fieldNum[typeof(IRecordList)] + 1;

			}
			*/
			
			// Temp hack to allow loading of old data files (none remain, so this is just here for reference)
			/*ProtoBuf.Meta.RuntimeTypeModel.Default.DynamicTypeFormatting += (sender, args) =>
			                                                                {
																				if (args.Type == null && !string.IsNullOrEmpty(args.FormattedName) && args.FormattedName.Contains("Data.DataTypes")) {
																					args.Type = Type.GetType(args.FormattedName.Replace("Data.DataTypes", "Papyrus.DataTypes").Replace(", Data", ", Papyrus"));
																					//args.FormattedName = args.FormattedName.Replace("Data.DataTypes", "Papyrus.DataTypes");
																				}
			                                                                };*/
			//ProtoBuf.
			
		}


		public DataProtoSerializer()
		{
			if(!_protoInitialised)
				InitProtoSystem();
		}

		public string Serialize(RecordPlugin recordPlugin, string directory, bool overwrite)
		{

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			var path = Path.Combine(directory, recordPlugin.Name + ".sgp");

			// Check if the file exists before overwriting
			if(File.Exists(path) && !overwrite)
				throw new Exception("File already exists and overwrite is specified as false.");

			using (var tmpStream = new MemoryStream(512)) {

				//Serialize to a memory buffer before the file in case something goes wrong.
				ProtoBufUtils.TypeModel.Serialize(tmpStream, recordPlugin);

				using (var stream = File.Open(path, FileMode.Create)) {

					// Reset mem stream to beginning
					tmpStream.Seek(0, SeekOrigin.Begin);

					// Copy to the file
					tmpStream.CopyTo(stream);

				}

			}

			// return the file path
			return path;

		}

		public RecordPlugin Deserialize(string fileName)
		{

			try {

				using (var stream = File.Open(fileName, FileMode.Open)) {
					RecordPlugin recordPlugin = ProtoBufUtils.TypeModel.Deserialize(stream, null, typeof(RecordPlugin)) as RecordPlugin;
					recordPlugin.SourceFile = fileName;
					recordPlugin.AfterDeserialization();
					return recordPlugin;
				}

			} catch(Exception e) {

				throw new PluginLoadException("Could not load plugin (" + fileName + ")", e);

			}

		}
	}
}
