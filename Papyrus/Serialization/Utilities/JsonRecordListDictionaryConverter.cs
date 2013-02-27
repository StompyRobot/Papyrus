/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Papyrus.Serialization.Utilities
{

	class JsonRecordListDictionaryConverter : JsonConverter
	{

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Dictionary<string,IRecordList>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
		{

			var recordTypes = RecordDatabase.GetRecordTypes();

			var instance = existingValue as Dictionary<string, IRecordList> ?? new Dictionary<String, IRecordList>();

			reader.Read();

			while (reader.TokenType == JsonToken.PropertyName) {

				// Type name of the record lists generic argument
				var typeName = reader.Value.ToString();

				// Type of the record lists generic argument

				var type = recordTypes.Single(p => p.Type.FullName == typeName).Type;

				// Type of the record list 
				var recordListType = typeof(RecordList<>).MakeGenericType(type);

				reader.Read();
				var result = serializer.Deserialize(reader, recordListType) as IRecordList;

				instance.Add(typeName, result);

				reader.Read();

			}

			return instance;

		}

		public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
		{

			var dict = value as Dictionary<string, IRecordList>;

			writer.WriteStartObject();

			foreach (var entry in dict) {

				writer.WritePropertyName(entry.Key);
				serializer.Serialize(writer, entry.Value);

			}

			writer.WriteEndObject();

		}

	}

}
