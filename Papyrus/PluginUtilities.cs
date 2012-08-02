using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;
using Papyrus.Serialization;

namespace Papyrus
{
	public static class PluginUtilities
	{

		/// <summary>
		/// Gets a list of plugins the the given directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static List<PluginInfo> PluginsInDirectory(string path)
		{

			var retList = new List<PluginInfo>();
			var errorList = new List<PluginInfo>();

			var plugins = Serialization.SerializationHelper.PluginsInDirectory(path);

			var pluginInstances = new List<RecordPlugin>(plugins.Count);

			foreach (var pluginPath in plugins) {

				var ser = Serialization.SerializationHelper.ResolveFromPath(pluginPath);

				try {

					pluginInstances.Add(ser.Deserialize(pluginPath));



				} catch(PluginLoadException) {
					
					errorList.Add(new PluginInfo() {
					                             	IsValid = false, Enabled = false,
													SourceFile = pluginPath, Name = Path.GetFileNameWithoutExtension(pluginPath), IsActive = false
					                             });

				}

			}

			PluginCollection.SortDependencies(pluginInstances);

			foreach (var plugin in pluginInstances) {

				retList.Add(new PluginInfo()
				{
					Dependencies = plugin.GetDependencies(),
					Description = plugin.Description,
					Name = plugin.Name,
					IsValid = true,
					SourceFile = plugin.SourceFile,
					Enabled = false,
					Author = plugin.Author,
					Format = SerializationHelper.DataFormatFromPath(plugin.SourceFile),
					LastModified = plugin.LastModified
				});

			}

			retList.AddRange(errorList);

			return retList;

		}

		/// <summary>
		/// Creates a new plugin with the given name in the given directory.
		/// </summary>
		/// <returns></returns>
		public static PluginInfo CreateNewPlugin(string name, string directory, DataFormat format = DataFormat.Proto)
		{
			
			RecordPlugin plugin = new RecordPlugin();
			plugin.Name = name;
			plugin.LastModified = DateTime.UtcNow;

			var ser = SerializationHelper.ResolveFromDataFormat(format);

			var file = ser.Serialize(plugin, directory, false);

			return new PluginInfo() {
			                        	Name = name, SourceFile = file, Format = format, LastModified = plugin.LastModified
			                        };

		}

		/// <summary>
		/// Opens a plugin and applies any changes to Author and Description
		/// </summary>
		/// <param name="pluginInfo"></param>
		public static void ApplyPluginInfo(PluginInfo pluginInfo)
		{

			var ser = SerializationHelper.ResolveFromPath(pluginInfo.SourceFile);

			var plugin = ser.Deserialize(pluginInfo.SourceFile);

			plugin.Author = pluginInfo.Author;
			pluginInfo.Description = pluginInfo.Description;
			plugin.LastModified = DateTime.UtcNow;

			ser.Serialize(plugin, Path.GetDirectoryName(pluginInfo.SourceFile), true);

		}

		/// <summary>
		/// Gets a list of file types for plugins.
		/// </summary>
		/// <returns></returns>
		public static List<DataSerializerInfo> PluginFileTypes()
		{
			return new List<DataSerializerInfo>(SerializationHelper._dataSerializers);
		} 

		/// <summary>
		/// Converts a plugin to a different format.
		/// </summary>
		/// <param name="pluginInfo">Plugin to convert</param>
		/// <param name="newFormat">Format to convert to</param>
		/// <param name="outputDirectory">Output directory for the new plugin.</param>
		/// <param name="overwrite">True to overwrite an existing plugin if it exists.</param>
		public static void ConvertPlugin(PluginInfo pluginInfo, DataFormat newFormat, string outputDirectory, bool overwrite)
		{

			// Serializer to load the plugin with
			var sourceSerializer = SerializationHelper.ResolveFromDataFormat(pluginInfo.Format);

			// Serializer to save the plugin with
			var destSerialier = SerializationHelper.ResolveFromDataFormat(newFormat);

			// Load the plugin from the source file
			var plugin = sourceSerializer.Deserialize(pluginInfo.SourceFile);

			// Serialize it out to the new file
			destSerialier.Serialize(plugin, outputDirectory, overwrite);

		}

	}
}
