using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{
	//[Serializable]
	[ProtoContract]
	public class Graphic : Record
	{

		public Graphic()
		{
			Path = "";
			Scale = 1.0f;
		}

		[ProtoMember(1)]
		public string Path { get; set; }

		[ProtoMember(2)]
		public float Scale { get; set; }

	}
}
