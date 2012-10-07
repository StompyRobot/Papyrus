using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Papyrus.Serialization
{

	internal class JsonSerializer : IDataSerializer
	{

		public static readonly string Extension = "jpp";

		public static Newtonsoft.Json.JsonSerializer GetPapyrusJsonSerializer()
		{
			var ser = new Newtonsoft.Json.JsonSerializer();
			ser.TypeNameHandling = TypeNameHandling.Auto;
			ser.ContractResolver = new PapyrusJsonContractResolver();
			ser.Converters.Add(new StringEnumConverter());
			return ser;
		}

		string IDataSerializer.Extension
		{
			get { return JsonSerializer.Extension; }
		}

		public string Serialize(RecordPlugin plugin, string directory, bool overwrite = true)
		{

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			var filePath = Path.Combine(directory, string.Format("{0}.{1}", plugin.Name, Extension));

			if (File.Exists(filePath) && !overwrite) {
				throw new Exception("File already exists and overwrite is specified as false.");
			}

			var ser = GetPapyrusJsonSerializer();

			using (var str = File.Open(filePath, FileMode.Create)) {
				using (var strWriter = new StreamWriter(str)) {
					using (var jsonWriter = new JsonTextWriter(strWriter)) {
						jsonWriter.Formatting = Formatting.Indented;

						ser.Serialize(jsonWriter, plugin);
						
					}
				}
			}

			return filePath;

		}

		public RecordPlugin Deserialize(string fileName)
		{

			var ser = GetPapyrusJsonSerializer();

			RecordPlugin plugin;

			using (FileStream file = File.OpenRead(fileName)) {
				using (StreamReader stream = new StreamReader(file)) {
					using (JsonReader reader = new JsonTextReader(stream)) {
						plugin = ser.Deserialize<Papyrus.RecordPlugin>(reader);
					}
				}
			}

			plugin.SourceFile = fileName;

			plugin.AfterDeserialization();

			return plugin;

		}

	}

}
