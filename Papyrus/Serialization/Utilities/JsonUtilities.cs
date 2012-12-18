using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Papyrus.Serialization.Utilities
{
	static class JsonUtilities
	{
		private static Newtonsoft.Json.JsonSerializer _serializer;

		public static Newtonsoft.Json.JsonSerializer GetPapyrusJsonSerializer()
		{

			if (_serializer == null) {
				_serializer = new Newtonsoft.Json.JsonSerializer();
				_serializer.TypeNameHandling = TypeNameHandling.Auto;
				_serializer.ContractResolver = new PapyrusJsonContractResolver();
				_serializer.Converters.Add(new StringEnumConverter());
			}

			return _serializer;

		}

		/// <summary>
		/// Overwrite oldRecord with record
		/// </summary>
		/// <param name="oldRecord"></param>
		/// <param name="record"></param>
		public static void OverWrite(DataTypes.Record oldRecord, DataTypes.Record record)
		{
			
			var ser = GetPapyrusJsonSerializer();
			
			using (MemoryStream str = new MemoryStream()) {

				using (var writer = new Newtonsoft.Json.Bson.BsonWriter(str)) {
					writer.CloseOutput = false;
					ser.Serialize(writer, record);
				}

				str.Seek(0, SeekOrigin.Begin);

				using (var reader = new Newtonsoft.Json.Bson.BsonReader(str)) {
					ser.Populate(reader, oldRecord);
				}
				
			}

		}

	}
}