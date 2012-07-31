using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class TurretGraphic : HardpointGraphic
	{

		public override bool AllowsType(HardpointType type)
		{

			// Only allow turrets, origin and weaponsource.
			if (type != HardpointType.Turret && type != HardpointType.Origin && type != HardpointType.WeaponSource)
				return false;

			return true;

		}

	}
}
