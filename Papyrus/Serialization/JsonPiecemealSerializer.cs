using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Serialization
{
	class JsonPiecemealSerializer : IDataSerializer
	{

		public static readonly string Extension = "jpx";

		string IDataSerializer.Extension
		{
			get { return Extension; }
		}

		public string Serialize(RecordPlugin plugin, string directory, bool overwrite = true)
		{
			throw new NotImplementedException();
		}

		public RecordPlugin Deserialize(string fileName)
		{
			throw new NotImplementedException();
		}

		public PluginHeader ReadPluginHeader(string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
