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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Papyrus.Serialization.Utilities;

namespace Papyrus.Serialization
{

	internal class JsonSerializer : IDataSerializer
	{

		public static readonly string Extension = "jpp";

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

			var ser = JsonUtilities.GetPapyrusJsonSerializer();

			var json = JObject.FromObject(plugin, ser);
			// Insert dependencies into the json
			var deps = JArray.FromObject(plugin.GetDependencies(), ser);
			json.Add(new JProperty("PluginDependencies", deps));

			using (var str = File.Open(filePath, FileMode.Create)) {
				using (var strWriter = new StreamWriter(str)) {
					using (var jsonWriter = new JsonTextWriter(strWriter)) {

						jsonWriter.Formatting = Formatting.Indented;
						json.WriteTo(jsonWriter, ser.Converters.ToArray());
						
					}
				}
			}

			return filePath;

		}

		public RecordPlugin Deserialize(string fileName)
		{

			var ser = JsonUtilities.GetPapyrusJsonSerializer();

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

		public PluginHeader ReadPluginHeader(string fileName)
		{

			JObject jObject = JObject.Parse(File.ReadAllText(fileName));

			PluginHeader header = new PluginHeader();

			header.Name = (string)jObject.SelectToken("Name");
			header.Description = (string) jObject.SelectToken("Description");
			header.DirectoryName = null;
			header.Author = (string) jObject.SelectToken("Author");
			var moduleToken = (JArray)jObject.SelectToken("ModuleDependencies");
			header.ModuleDependencies = moduleToken.Select(p => new Guid((string) p)).ToList();
			var depsToken = jObject.SelectToken("PluginDependencies");
			header.PluginDependencies = depsToken == null ? new List<string>() : depsToken.Select(p => (string)p).ToList();
			header.SourceFile = fileName;
			header.LastModified = (DateTime)jObject.SelectToken("LastModified");

			return header;

		}

	}

}
