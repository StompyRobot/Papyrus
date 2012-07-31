using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class Station : Record
	{

		public Station()
		{
			Name = "";
			Position = new Vector2(0,0);
			Graphic = DataPointer<Graphic>.Empty;
			ParallaxBackground = DataPointer<Graphic>.Empty;
			BackgroundLink = LinkType.None;
			SpawnList = new DataPointerList<Ship>();
			SpawnFrequency = 60;
		}

		[ProtoMember(1)]
		[Category("Station")]
		public string Name { get; set; }

		[ProtoMember(2)]
		[Category("Station")]
		public Vector2 Position { get; set; }

		[ProtoMember(3)]
		[Category("Station")]
		public DataPointer<Graphic> Graphic { get; set; }

		[ProtoMember(4)]
		[Category("Station")]
		public DataPointer<Graphic> ParallaxBackground { get; set; }

		[ProtoMember(5)]
		[Category("Station")]
		public LinkType BackgroundLink { get; set; }


		[ProtoMember(6)]
		[Category("Shipyard")]
		public bool IsShipyard { get; set; }

		[ProtoMember(7)]
		[Category("Shipyard")]
		public TechLevel ShipyardTechLevel { get; set; }

		[ProtoMember(8)]
		[Category("Outfitter")]
		public bool HasOutfitter { get; set; }

		[ProtoMember(9)]
		[Category("Outfitter")]
		public TechLevel OutfitterTechLevel { get; set; }

		[ProtoMember(10)]
		[Category("Shipyard"), Description("A list of ships that spawn at this shipyard.")]
		public DataPointerList<Ship> SpawnList { get; set; }

		[ProtoMember(11)]
		[Category("Shipyard"), Description("Time between ship spawns at this shipyard in seconds.")]
		public float SpawnFrequency { get; set; } 

	}
}
