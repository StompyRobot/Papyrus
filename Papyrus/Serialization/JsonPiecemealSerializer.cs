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

namespace Papyrus.Serialization
{
	class JsonPiecemealSerializer : PiecemealSerializer
	{

		/// <summary>
		/// File extension for the header file
		/// </summary>
		public static readonly string HeaderExtension = "jpx";

		/// <summary>
		/// File extension for a single record
		/// </summary>
		public static readonly string ItemExtension = "json";

		public override string Extension
		{
			get { return HeaderExtension; }
		}

		protected override string RecordExtension
		{
			get { return ItemExtension; }
		}

		private Newtonsoft.Json.JsonSerializer _jsonSerializer;

		public JsonPiecemealSerializer()
		{

			_jsonSerializer = Utilities.JsonUtilities.GetPapyrusJsonSerializer();

		}

		protected override void SerializeRecord<T>(RecordContainer<T> record, System.IO.FileStream output)
		{

			using (var sWriter = new StreamWriter(output, Encoding.UTF8)) {
				using (var jWriter = new JsonTextWriter(sWriter)) {

					jWriter.Formatting = Formatting.Indented;

					_jsonSerializer.Serialize(jWriter, record);

				}
			}

		}

		protected override RecordContainer<T> ReadRecord<T>(System.IO.FileStream input)
		{

			using (var sReader = new StreamReader(input, Encoding.UTF8)) {
				using (var jReader = new JsonTextReader(sReader)) {

					return _jsonSerializer.Deserialize<RecordContainer<T>>(jReader);

				}
			}

		}

		protected override PluginHeader ReadHeader(System.IO.FileStream input)
		{

			using (var sReader = new StreamReader(input, Encoding.UTF8)) {
				using (var jReader = new JsonTextReader(sReader)) {

					return _jsonSerializer.Deserialize<PluginHeader>(jReader);

				}
			}

		}

		protected override void WriteHeader(PluginHeader header, System.IO.Stream stream)
		{

			using (var sWriter = new StreamWriter(stream, Encoding.UTF8)) {
				using (var jWriter = new JsonTextWriter(sWriter)) {

					jWriter.Formatting = Formatting.Indented;

					_jsonSerializer.Serialize(jWriter, header);

				}
			}

		}

	}
}
