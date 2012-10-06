using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Papyrus.Serialization
{

	internal class JsonSerializer : IDataSerializer
	{

		public static readonly string Extension = "jsonp";

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

			var ser = new Newtonsoft.Json.JsonSerializer();

			using (var str = File.Open(filePath, FileMode.Create)) {
				using (var strWriter = new StreamWriter(str)) {
					using (var jsonWriter = new JsonTextWriter(strWriter)) {
						
						ser.Serialize(jsonWriter, plugin);
						
					}
				}
			}

			return filePath;

		}

		public RecordPlugin Deserialize(string fileName)
		{
			throw new NotImplementedException();
		}

	}

}
