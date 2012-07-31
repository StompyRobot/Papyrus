using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Papyrus.DataTypes
{

	//[Serializable]
	[ProtoContract]
	public class PlanetGraphic : Record
	{

		public PlanetGraphic()
		{
			Path = "";
		}

		[ProtoMember(1)]
		public string Path { get; set; }

	}

}
