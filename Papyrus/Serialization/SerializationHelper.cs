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

namespace Papyrus.Serialization
{

	public enum DataFormat
	{
		None,
		Proto,
		ProtoPiecemeal,

		Json,
		JsonPiecemeal
	}

	public class DataSerializerInfo
	{

		public Type Type { get; internal set; }
		public string Extension { get; internal set; }
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public DataFormat Format { get; internal set; }

	}

	internal static class SerializationHelper
	{

		internal static List<DataSerializerInfo> _dataSerializers = new List<DataSerializerInfo>(); 

		internal static bool RegisterFileFormat<T>(string ext, string name, string desc, DataFormat format = DataFormat.None) where T : IDataSerializer
		{

			_dataSerializers.Add(new DataSerializerInfo() {
			                                          	Description = desc,
														Extension = ext,
														Type = typeof(T),
														Name = name,
														Format = format
			                                          });

			return true;

		}

		static SerializationHelper()
		{

			SerializationHelper.RegisterFileFormat<DataProtoSerializer>(DataProtoSerializer.Extension, "Binary Plugin", "Binary data format contained in one file. (High performance; Recommended distribution format; Poor for source control.)", DataFormat.Proto);
			SerializationHelper.RegisterFileFormat<DataProtoPiecemealSerializer>(DataProtoPiecemealSerializer.HeaderExtension, "Binary Piecemeal Plugin", "Binary data format spread across many files. (Medium performace; Better than nothing for source control.)", DataFormat.ProtoPiecemeal);
			
			SerializationHelper.RegisterFileFormat<JsonSerializer>(JsonSerializer.Extension, "JSON Plugin", "JSON data format contained in one file (Low performance; useful for source control.)", DataFormat.Json);
			SerializationHelper.RegisterFileFormat<JsonPiecemealSerializer>(JsonPiecemealSerializer.HeaderExtension, "JSON Piecemeal Plugin", "JSON data format spread over many files. (Least performace; excellent for source control.)", DataFormat.JsonPiecemeal);

		}

		/// <summary>
		/// Returns an appropriate data serializer to load the file at the given path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IDataSerializer ResolveFromPath(string path)
		{

			string ext = Path.GetExtension(path);

			if (ext == null) {
				throw new Exception("Invalid Data Format");
			}

			if(ext.First() == '.') {
				ext = ext.Substring(1); // remove the .
			}

			var serializer = _dataSerializers.Find(p => p.Extension == ext);

			if(serializer == null)
				throw new Exception("No serializer could be found for plugin (" + path + ")");

			return Activator.CreateInstance(serializer.Type) as IDataSerializer;

		}

		/// <summary>
		/// From a given DataFormat returns a data serializer.
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static IDataSerializer ResolveFromDataFormat(DataFormat format)
		{

			var info = _dataSerializers.Find(p => p.Format == format);

			if (info == null) {
				throw new Exception("Invalid Data Format");
			}

			return Activator.CreateInstance(info.Type) as IDataSerializer;

		}

		/// <summary>
		/// Gets the data format from a given path
		/// </summary>
		/// <param name="path">Path of the plugin to get the data format from.</param>
		/// <returns></returns>
		public static DataFormat DataFormatFromPath(string path)
		{

			string ext = Path.GetExtension(path);

			if (ext == null)
			{
				throw new Exception("Invalid Data Format");
			}

			if (ext.First() == '.')
			{
				ext = ext.Substring(1); // remove the .
			}

			return _dataSerializers.Find(p => p.Extension == ext).Format;

		}

		/// <summary>
		/// Returns a list of plugins in the directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<string> PluginsInDirectory(string path)
		{

			var plugins = new List<string>();

			var files = Directory.GetFiles(path);

			foreach (var file in files) {

				foreach (var fileExtension in _dataSerializers.Select(p => p.Extension)) {

					if(Path.GetExtension(file).Substring(1) == fileExtension) {
						plugins.Add(file);
						break;
					}

				}

			}

			return plugins;

		} 

		/// <summary>
		/// Returns a plugin header for the given record plugin
		/// </summary>
		/// <param name="plugin"></param>
		/// <returns></returns>
		internal static PluginHeader PluginHeaderForPlugin(RecordPlugin plugin)
		{

			return new PluginHeader() {
			                                         	Author = plugin.Author,
			                                         	Description = plugin.Description,
			                                         	DirectoryName = null,
			                                         	ModuleDependencies = new List<Guid>(plugin.ModuleDependencies),
			                                         	Name = plugin.Name,
														PluginDependencies = plugin.GetDependencies(),
														LastModified = plugin.LastModified,
														SourceFile = plugin.SourceFile
			                                         };

		}

		/// <summary>
		/// Returns an empty record plugin for a given header
		/// </summary>
		/// <param name="header"></param>
		/// <returns></returns>
		internal static RecordPlugin RecordPluginForHeader(PluginHeader header)
		{
			
			return new RecordPlugin() {
			                          	Author = header.Author,
										Description = header.Description,
										LastModified = header.LastModified,
										ModuleDependencies = header.ModuleDependencies,
										Name = header.Name,
										SourceFile = header.SourceFile
			                          };

		} 

	}

}
