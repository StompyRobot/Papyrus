using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.RecordTypes
{

	public class GraphicAsset : SampleRootRecord
	{

		[Papyrus.RecordProperty(1)]
		public string AssetPath { get; set; }
	
		[Papyrus.RecordProperty(2)]
		public string Owner { get; set; }

	}

}
