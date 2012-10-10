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
using System.Threading;
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using Papyrus.Serialization.Utilities;

namespace Papyrus.Serialization
{
	internal class DataProtoPiecemealSerializer : IDataSerializer
	{

		public static readonly string Extension = "sgpp";

		string IDataSerializer.Extension
		{
			get { return DataProtoPiecemealSerializer.Extension; }
		}

		private static MethodInfo _serializeRecordListMethod;
		private static MethodInfo _deserializeRecordListMethod;

		public string Serialize(RecordPlugin recordPlugin, string directory, bool overwrite)
		{

			var pluginPath = Path.Combine(directory, recordPlugin.Name);
			var headerPath = Path.Combine(directory, recordPlugin.Name + ".sgpp");

			if ((File.Exists(headerPath) || Directory.Exists(pluginPath)) && !overwrite)
				throw new Exception("Plugin already exists.");

			// Get a temporary file
			var tempPluginPath = Path.GetTempFileName();
			File.Delete(tempPluginPath); // Delete the file
			Directory.CreateDirectory(tempPluginPath); // Create a directory instead

			bool error = false;

			try {

				foreach (var recordList in recordPlugin.RecordLists) {

					SerializeRecordList(recordList.Value, Path.Combine(tempPluginPath, recordList.Key));

				}

				var header = SerializationHelper.PluginHeaderForPlugin(recordPlugin);
				header.DirectoryName = header.Name;

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
			PluginHeader header;

			using(var headerFile = File.OpenRead(fileName)) {
				header = ProtoBuf.Serializer.Deserialize<PluginHeader>(headerFile);
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
			plugin.ModuleDependencies = header.ModuleDependencies ?? new List<Guid>();

			var dataTypes = Directory.GetDirectories(pluginDir);
			var dataAssem = Assembly.GetAssembly(typeof (Record));
			foreach (var dataType in dataTypes) {

				//var typeName = Path.pa

				var dataTypeName = dataType.Split(Path.DirectorySeparatorChar).Last();

				Type type = null;

				foreach (var rootRecord in RecordDatabase.RootRecords) {

					type = rootRecord.Assembly.GetType(dataTypeName);

					if (type != null)
						break;
				}

				// If not found in a plugin assembly, search the papyrus assembly for built in types
				if(type == null)
					type = GetType().Assembly.GetType(dataTypeName);

				// Limit types to subtypes of record
				if(type == null || !typeof(Record).IsAssignableFrom(type))
					throw new PluginLoadException("Invalid Data Type");

				var recordList = DeserializeRecordList(type, dataType);

				plugin.RecordLists.Add(RecordPlugin.RecordListKeyForType(type), recordList);

			}

			plugin.AfterDeserialization();

			return plugin;

		}


		public PluginHeader ReadPluginHeader(string fileName)
		{

			using(var file = File.OpenRead(fileName)) {
				var header = (PluginHeader)ProtoBufUtils.TypeModel.Deserialize(file, null, typeof (PluginHeader));
				header.SourceFile = fileName;
				return header;
			}

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

					ProtoBufUtils.TypeModel.Serialize(file, recordContainer);

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

					var recordContainer = ProtoBufUtils.TypeModel.Deserialize(recordFile, null, typeof(RecordContainer<T>)) as RecordContainer<T>;
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
