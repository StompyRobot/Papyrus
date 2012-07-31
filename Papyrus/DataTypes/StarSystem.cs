using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{
	//[Serializable]
	[ProtoContract]
	public class StarSystem : Record
	{

		public StarSystem()
		{
			Stations = new DataPointerList<Station>();
			Links = new DataPointerList<StarSystem>();
			Government = DataPointer<Government>.Empty;
			SunColour = new Color();
		}

		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public Vector2 MapPosition {get; set;}

		[ProtoMember(3)]
		public DataPointer<Government> Government { get; set; }

		[ProtoMember(4)]
		public bool HasSun { get; set; }

		[ProtoMember(5)]
		public Color SunColour { get; set; }

		[ProtoMember(6), DataRange(Minimum = 0.01f, Maximum = 1.0f)]
		public float SunSize { get; set; }

		[ProtoMember(7)]
		public DataPointerList<StarSystem> Links { get; set; }

		[ProtoMember(8)]
		public DataPointerList<Station> Stations { get; set; }

	}
}
