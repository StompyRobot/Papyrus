using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus.Serialization
{

	[ProtoContract]
	struct ProtoPiecemealHeader
	{

		[ProtoMember(1)]
		public string Name;

		[ProtoMember(2)]
		public string DirectoryName;

		[ProtoMember(3)]
		public string Description;

		[ProtoMember(4)]
		public string Author;

	}

	internal class DataProtoPiecemealSerializer : IDataSerializer
	{
		private static MethodInfo _serializeRecordListMethod;
		private static MethodInfo _deserializeRecordListMethod;

		public DataProtoPiecemealSerializer()
		{
			// Initialise any protobuffer model stuff.
			DataProtoSerializer.InitProtoSystem();
		}

		public string Serialize(RecordPlugin recordPlugin, string directory, bool overwrite)
		{

			var pluginPath = Path.Combine(directory, recordPlugin.Name);
			var headerPath = Path.Combine(directory, recordPlugin.Name + ".sgpp");

			if ((File.Exists(headerPath) || Directory.Exists(pluginPath)) && !overwrite)
				throw new Exception("Plugin already exists.");

			var tempPluginPath = Path.GetTempFileName();
			File.Delete(tempPluginPath);
			Directory.CreateDirectory(tempPluginPath);

			bool error = false;

			try {

				foreach (var recordList in recordPlugin.RecordLists) {

					SerializeRecordList(recordList.Value, Path.Combine(tempPluginPath, recordList.Key));

				}

				var header = new ProtoPiecemealHeader()
				             {Name = recordPlugin.Name, DirectoryName = recordPlugin.Name, Description = recordPlugin.Description, Author = recordPlugin.Author};

				using(var headerFile = File.Open(headerPath, FileMode.Create)) {
					ProtoBuf.Serializer.Serialize(headerFile, header);
				}


			} catch(Exception) {

				error = true;
				throw;

			} finally {
				
				// Clean up our mess if there was an error
				if (error) {
					Directory.Delete(tempPluginPath);
				}

			}

			if (error)
				return null;

			// Delete the plugin that already exists
			if (Directory.Exists(pluginPath)) {
				try {
					Directory.Delete(pluginPath, true);
				} catch (IOException) {
					Thread.Sleep(0);
					Directory.Delete(pluginPath, true);
				}
			}

			// Move the new plugin to the output directory
			Directory.Move(tempPluginPath, pluginPath);

			// return the file path
			return headerPath;

		}

		public RecordPlugin Deserialize(string fileName)
		{
			ProtoPiecemealHeader header;

			using(var headerFile = File.OpenRead(fileName)) {
				header = ProtoBuf.Serializer.Deserialize<ProtoPiecemealHeader>(headerFile);
			}

			var pluginDir = Path.Combine(Path.GetDirectoryName(fileName), header.DirectoryName);

			if(!Directory.Exists(pluginDir))
				throw new Exception("Plugin Directory not found");

			var plugin = new RecordPlugin();
			plugin.Name = header.Name;
			plugin.SourceFile = fileName;
			plugin.Description = header.Description;
			plugin.Author = header.Author;
			plugin.RecordLists = new Dictionary<string, IRecordList>();

			var dataTypes = Directory.GetDirectories(pluginDir);
			var dataAssem = Assembly.GetAssembly(typeof (Record));
			foreach (var dataType in dataTypes) {

				//var typeName = Path.pa

				var dataTypeName = dataType.Split(Path.DirectorySeparatorChar).Last();

				var type = dataAssem.GetType("Papyrus.DataTypes." + dataTypeName);

				// Limit data types to the DataType namespace
				if(type == null || type.Namespace != typeof(Record).Namespace)
					throw new Exception("Invalid Data Type");

				var recordList = DeserializeRecordList(type, dataType);

				plugin.RecordLists.Add(RecordPlugin.RecordListKeyForType(type), recordList);

			}

			return plugin;

		}

		/// <summary>
		/// Serializes a record list into a series of files in a directory.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="directory"></param>
		private void SerializeRecordList(IRecordList list, string directory)
		{
			if (_serializeRecordListMethod == null) {
				_serializeRecordListMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(p => p.Name == "SerializeRecordList" && p.IsGenericMethod);
			}

			var method = _serializeRecordListMethod.MakeGenericMethod(list.RecordType);
			method.Invoke(this, new object[]{list, directory});

		}

		/// <summary>
		/// Generic method to serialize a record list into a series of files in a directory.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="directory"></param>
		private void SerializeRecordList<T>(RecordList<T> list, string directory) where T : Record
		{

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			for (int i = 0; i < list.Records.Count; i++) {

				var recordContainer = list.Records[i];

				using (var file = File.Open(Path.Combine(directory,FileNameForContainer(recordContainer) + ".rec"), FileMode.Create)) {
					
					ProtoBuf.Serializer.Serialize(file, recordContainer);

				}
				
			}

		}

		/// <summary>
		/// Deserializes a directory of records into a record list.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="directory"></param>
		private IRecordList DeserializeRecordList(Type type, string directory)
		{

			if (_deserializeRecordListMethod == null)
			{
				_deserializeRecordListMethod = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(p => p.Name == "DeserializeRecordList" && p.IsGenericMethod);
			}

			var method = _deserializeRecordListMethod.MakeGenericMethod(type);

			return method.Invoke(this, new object[] { directory }) as IRecordList;

		}

		/// <summary>
		/// Generic method to serialize a record list into a series of files in a directory.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="directory"></param>
		private RecordList<T> DeserializeRecordList<T>(string directory) where T : Record
		{

			if (!Directory.Exists(directory))
				throw new FileNotFoundException("No directory found for data type.");

			var recordList = new RecordList<T>();

			var files = Directory.GetFiles(directory);

			foreach (var file in files) {
				
				using(var recordFile = File.OpenRead(file)) {

					var recordContainer = ProtoBuf.Serializer.Deserialize<RecordContainer<T>>(recordFile);
					recordList.Records.Add(recordContainer);

				}

			}

			return recordList;

		}

		/// <summary>
		/// Returns a unique hash for a given container
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static string FileNameForContainer(IRecordContainer container)
		{

			string name = "";

			name += container.Mode.ToString();
			name += "_";
			name += container.Destination;
			name += "_";
			name += container.Index;

			return name;

		}

	}
}
