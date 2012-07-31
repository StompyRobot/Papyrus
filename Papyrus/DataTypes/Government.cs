using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{
	[ProtoContract]
	public class Government : Record
	{

		public Government()
		{
			Name = "(No Name)";
			Color = new Color(1.0f,1.0f,1.0f,1.0f);
			HomeSystem = DataPointer<StarSystem>.Empty;
		}

		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public Color Color { get; set; }

		[ProtoMember(3)]
		public DataPointer<StarSystem> HomeSystem { get; set; }

	}
}
