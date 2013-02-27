/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Papyrus.DataTypes;
using Papyrus.Exceptions;

namespace Papyrus.Serialization
{
	abstract class PiecemealSerializer : IDataSerializer
	{

		private static MethodInfo _serializeRecordListMethod;
		private static MethodInfo _deserializeFilesMethod;

		protected abstract string RecordExtension { get; }

		protected abstract void SerializeRecord<T>(RecordContainer<T> record, FileStream output) where T : Record;

		protected abstract RecordContainer<T> ReadRecord<T>(FileStream input) where T : Record;

		protected abstract PluginHeader ReadHeader(FileStream input);

		protected abstract void WriteHeader(PluginHeader header, Stream stream);

		/// <summary>
		/// Returns a unique hash for a given container
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public string FileNameForContainer(IRecordContainer container)
		{

			string name = "";

			name += container.Mode.ToString();
			name += "_";
			name += container.Destination;
			name += "_";
			name += container.Index;
			name += ".";
			name += RecordExtension;

			return name;

		}

		public abstract string Extension { get; }

		public string Serialize(RecordPlugin plugin, string directory, bool overwrite = true)
		{

			var pluginPath = Path.Combine(directory, plugin.Name);
			var headerPath = Path.Combine(directory, string.Format("{0}.{1}", plugin.Name, Extension));

			if ((File.Exists(headerPath) || Directory.Exists(pluginPath)) && !overwrite)
				throw new PluginSaveException("Plugin already exists, overwrite is set to false.");

			string tempDirectory = GetTemporaryDirectory();

			bool error = false;

			try {

				SerializePluginToDirectory(plugin, tempDirectory);

				var header = SerializationHelper.PluginHeaderForPlugin(plugin);
				header.DirectoryName = header.Name;

				using (var headerFile = File.Open(headerPath, FileMode.Create)) {
					WriteHeader(header, headerFile);
				}
				

			} catch (Exception e) {

				error = true;

				throw new PluginSaveException("Error saving plugin.", e);

			} finally {

				// Clean up our mess if there was an error
				if (error) {
					Directory.Delete(tempDirectory, true);
				}

			}

			// Delete the plugin that already exists
			if (Directory.Exists(pluginPath)) {
				try {
					Directory.Delete(pluginPath, true);
				}
				catch (IOException) {
					Thread.Sleep(0);
					Directory.Delete(pluginPath, true);
				}
			}

			// Move the new plugin to the output directory
			try {
				Directory.Move(tempDirectory, pluginPath);
			}
			catch (IOException) {
				Thread.Sleep(0);
				Directory.Move(tempDirectory, pluginPath);
			}

			// return the file path
			return headerPath;

		}

		public RecordPlugin Deserialize(string fileName)
		{

			try {

				PluginHeader header = ReadPluginHeader(fileName);

				RecordPlugin plugin = SerializationHelper.RecordPluginForHeader(header);

				var rootPath = Path.Combine(Path.GetDirectoryName(fileName), header.DirectoryName);

				IterateRootDirectory(rootPath, plugin);

				plugin.AfterDeserialization();

				return plugin;

			}
			catch (Exception e)
			{
				throw new PluginLoadException("Error loading plugin", e);
			}

		}

		public PluginHeader ReadPluginHeader(string fileName)
		{

			using(var file = File.OpenRead(fileName)) {
				var header = ReadHeader(file);
				header.SourceFile = fileName;
				return header;
			}

		}

		private void SerializePluginToDirectory(RecordPlugin plugin, string path)
		{


			foreach (var recordList in plugin.RecordLists) {

				SerializeRecordList(recordList.Value, Path.Combine(path, recordList.Key));

			}

		}

		private void SerializeRecordList(IRecordList list, string directory)
		{

			if (_serializeRecordListMethod == null) {
				_serializeRecordListMethod = typeof(PiecemealSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(p => p.Name == "SerializeRecordListInternal" && p.IsGenericMethod);
			}

			_serializeRecordListMethod.MakeGenericMethod(list.RecordType).Invoke(this, new object[] { list, directory });

		}

		/// <summary>
		/// Serializes a record list into the directory
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="recordList"></param>
		/// <param name="directory"></param>
		private void SerializeRecordListInternal<T>(RecordList<T> recordList, string directory) where T : Record
		{

			Directory.CreateDirectory(directory);

			foreach (var recordContainer in recordList.Records) {

				var path = Path.Combine(directory, FileNameForContainer(recordContainer));

				using(var stream = File.Open(path, FileMode.Create)) {

					SerializeRecord(recordContainer, stream);

				}

			}

		}

		/// <summary>
		/// Iterate the given directory, each directory inside is resolved to a data type and deserialized into a record list
		/// </summary>
		/// <param name="path">Path to deserialize</param>
		/// <param name="plugin">plugin to deserialize into</param>
		private void IterateRootDirectory(string path, RecordPlugin plugin)
		{

			var directories = Directory.GetDirectories(path);

			foreach (var directory in directories) {
				
				var info = new DirectoryInfo(directory);

				Type dataType = ResolveDataType(info.Name);

				var list = DeserializeFiles(Directory.GetFiles(directory, string.Format("*.{0}", RecordExtension), SearchOption.TopDirectoryOnly), dataType);

				plugin.RecordLists.Add(RecordPlugin.RecordListKeyForType(dataType), list);

			}

		}

		/// <summary>
		/// Deserialize a list of files into a record list
		/// </summary>
		/// <param name="files">List of files to deserialize</param>
		/// <param name="dataType">Data type to attempt to deserialize</param>
		/// <returns>List of records deserialized</returns>
		private IRecordList DeserializeFiles(string[] files, Type dataType)
		{

			if (_deserializeFilesMethod == null) {
				_deserializeFilesMethod = typeof(PiecemealSerializer).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(p => p.Name == "DeserializeFilesInternal" && p.IsGenericMethod);
			}

			return _deserializeFilesMethod.MakeGenericMethod(dataType).Invoke(this, new object[] { files }) as IRecordList;

		}

		private RecordList<T> DeserializeFilesInternal<T>(string[] files) where T : Record
		{

			var recordList = new RecordList<T>();

			foreach (var file in files) {

				using(FileStream stream = File.OpenRead(file))
					recordList.Records.Add(this.ReadRecord<T>(stream) as RecordContainer<T>);

			}

			return recordList;

		}

		private string GetTemporaryDirectory()
		{

			// Get a temporary file
			var temp = Path.GetTempFileName();
			File.Delete(temp); // Delete the file
			Directory.CreateDirectory(temp); // Create a directory instead

			return temp;

		}

		/// <summary>
		/// Finds a data type with the given string as its typename
		/// </summary>
		/// <param name="typeString"></param>
		/// <returns></returns>
		private Type ResolveDataType(string typeString)
		{

			var dataTypeName = typeString.Split(Path.DirectorySeparatorChar).Last();

			Type type = null;

			foreach (var rootRecord in RecordDatabase.RootRecords) {

				type = rootRecord.Assembly.GetType(dataTypeName);

				if (type != null)
					break;
			}

			// If not found in a plugin assembly, search the papyrus assembly for built in types
			if (type == null)
				type = GetType().Assembly.GetType(dataTypeName);

			// Limit types to subtypes of record
			if (type == null || !typeof(Record).IsAssignableFrom(type))
				throw new PluginLoadException("Invalid Data Type");

			return type;

		}

	}
}
