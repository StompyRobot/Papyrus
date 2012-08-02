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
using Papyrus.DataTypes;

namespace Papyrus.Serialization
{
	internal class DataXMLSerializer : IDataSerializer
	{


		/*static readonly Type[] KnownDataTypes = new Type[] {
		                                   	//typeof(Data.DataTypes.DataPointer<>),
											typeof(Record),
											//typeof(Data.RecordList<>),
											typeof(RecordList<Government>),
											typeof(RecordList<StarSystem>),
											typeof(DataPointer<Government>), typeof(DataPointer<StarSystem>)
		                                   };*/

		public string Serialize(RecordPlugin database, string directory, bool overwrite)
		{

			throw new NotImplementedException("XML not yet implemented");

			/*if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			var fileNames = new List<string>();

			var ser = new System.Runtime.Serialization.DataContractSerializer(typeof(RecordPlugin), KnownDataTypes);

			foreach (var recordPlugin in database.Plugins) {

				var path = Path.Combine(directory, recordPlugin.Value.Name + ".xml");

				using (var stream = File.Open(path, FileMode.Create)) {
					
					ser.WriteObject(stream, recordPlugin.Value);
					fileNames.Add(path);

				}
			}

			return fileNames.ToArray();*/

		}

		public RecordPlugin Deserialize(string fileName)
		{

			try {

				var ser = new System.Runtime.Serialization.DataContractSerializer(typeof (RecordPlugin));


				using (var stream = File.OpenRead(fileName)) {

					var plugin = ser.ReadObject(stream) as RecordPlugin;
					plugin.SourceFile = fileName;
					return plugin;

				}

			} catch(Exception e) {

				throw new PluginLoadException("Could not load plugin (" + fileName + ")", e);

			}

		}

	}
}
