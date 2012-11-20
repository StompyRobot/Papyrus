using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sample.RecordTypes
{

	[Papyrus.ChildRecord(1, typeof(GraphicAsset))]
	public abstract class SampleRootRecord : Papyrus.DataTypes.Record
	{
	}

}
