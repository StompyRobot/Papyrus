using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Papyrus.Serialization.Utilities
{
	static class JsonUtilities 
	{

		public static Newtonsoft.Json.JsonSerializer GetPapyrusJsonSerializer()
		{
			var ser = new Newtonsoft.Json.JsonSerializer();
			ser.TypeNameHandling = TypeNameHandling.Auto;
			ser.ContractResolver = new PapyrusJsonContractResolver();
			ser.Converters.Add(new StringEnumConverter());
			return ser;
		}

	}
}