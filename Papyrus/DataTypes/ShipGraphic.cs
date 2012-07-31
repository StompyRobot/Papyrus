using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public sealed class ShipGraphic : HardpointGraphic
	{

		public ShipGraphic()
		{
			AdvancedPhysics = true;
		}

		/// <summary>
		/// True to use advanced texture->fixture and fixture deformation in the game.
		/// </summary>
		[ProtoMember(1)]
		[DefaultValue(true), Description("Use Advanced Physics on a ship with this graphic. (Texture->Fixture conversions, deformations)")]
		public bool AdvancedPhysics { get; set; }

		/// <summary>
		/// Allow anything for a ship graphic
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override bool AllowsType(HardpointType type)
		{
			return true;
		}

	}
}
