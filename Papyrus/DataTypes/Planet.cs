using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;


namespace Papyrus.DataTypes
{

	//[Serializable]
	[ProtoContract]
	public class Planet : Record
	{

		public Planet()
		{
			Name = "";
			Graphic = DataPointer<PlanetGraphic>.Empty;
		}

		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public DataPointer<PlanetGraphic> Graphic { get; set; }

	}


}
