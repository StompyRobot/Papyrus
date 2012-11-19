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
using System.Reflection;
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using Papyrus.Extensions;
using Papyrus.Serialization.Utilities;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Papyrus.Serialization
{
	internal class DataProtoSerializer : IDataSerializer
	{

		public static readonly string Extension = "sgp";

		string IDataSerializer.Extension
		{
			get { return DataProtoSerializer.Extension; }
		}

		public string Serialize(RecordPlugin recordPlugin, string directory, bool overwrite)
		{

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			var path = Path.Combine(directory, recordPlugin.Name + ".sgp");

			// Check if the file exists before overwriting
			if(File.Exists(path) && !overwrite)
				throw new Exception("File already exists and overwrite is specified as false.");

			PluginHeader header = SerializationHelper.PluginHeaderForPlugin(recordPlugin);

			using (var tmpStream = new MemoryStream(1024)) {

				ProtoBufUtils.TypeModel.SerializeWithLengthPrefix(tmpStream, header, typeof(PluginHeader), PrefixStyle.Fixed32, 0);

				//Serialize to a memory buffer before the file in case something goes wrong.
				ProtoBufUtils.TypeModel.Serialize(tmpStream, recordPlugin);

				using (var stream = File.Open(path, FileMode.Create)) {

					// Reset mem stream to beginning
					tmpStream.Seek(0, SeekOrigin.Begin);

					// Copy to the file
					tmpStream.CopyTo(stream);

				}

			}

			// return the file path
			return path;

		}

		public RecordPlugin Deserialize(string fileName)
		{

			try {

				using (var stream = File.Open(fileName, FileMode.Open)) {

					var header = ProtoBufUtils.TypeModel.DeserializeWithLengthPrefix(stream, null, typeof (PluginHeader), PrefixStyle.Fixed32, 0);

					RecordPlugin recordPlugin = ProtoBufUtils.TypeModel.Deserialize(stream, null, typeof(RecordPlugin)) as RecordPlugin;
					recordPlugin.SourceFile = fileName;
					recordPlugin.AfterDeserialization();
					return recordPlugin;

				}

			} catch(Exception e) {

				throw new PluginLoadException("Error loading plugin (" + fileName + ")", e);

			}

		}

		public PluginHeader ReadPluginHeader(string fileName)
		{

			try {

				using (var stream = File.Open(fileName, FileMode.Open)) {

					var header = (PluginHeader)ProtoBufUtils.TypeModel.DeserializeWithLengthPrefix(stream, null, typeof(PluginHeader), PrefixStyle.Fixed32, 0);
					header.SourceFile = fileName;

					return header;

				}

			}
			catch (Exception e) {

				throw new PluginLoadException("Error loading plugin (" + fileName + ")", e);

			}

		}
	}
}
