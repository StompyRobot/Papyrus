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
		XML,
		Proto,

		ProtoPiecemeal,
		XMLPiecemeal
	}

	public class DataSerializerInfo
	{

		public Type Type;
		public string Extension;
		public string Name;
		public string Description;
		public DataFormat Format;

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

			SerializationHelper.RegisterFileFormat<DataProtoSerializer>("sgp", "SpaceGame Plugin", "Binary data format contained in one file.", DataFormat.Proto);
			SerializationHelper.RegisterFileFormat<DataProtoSerializer>("sgpx", "SpaceGame XML Plugin", "XML data format contained in one file.", DataFormat.XML);
			SerializationHelper.RegisterFileFormat<DataProtoPiecemealSerializer>("sgpp", "SpaceGame Piecemeal Plugin", "Binary data format spread across many files.", DataFormat.ProtoPiecemeal);
			SerializationHelper.RegisterFileFormat<DataXMLPieacemealSerializer>("sgppx", "SpaceGame XML Piecemeal Plugin", "XML data format spread across many files. Recommended if using source control.", DataFormat.XMLPiecemeal);

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

	}

}
