using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Hardpoints
{

	/// <summary>
	/// The source of projectiles/beams/railguns/fields
	/// </summary>
	[ProtoContract]
	public class WeaponSourceHardpoint : Hardpoint
	{

		public WeaponSourceHardpoint()
		{
			Name = "WeaponSource";
			WeaponIndex = 0;
			Size = new Vector2(0.25f, 0.08f);
		}

		[ProtoMember(1)]
		[Description("The slot index this weapon source represents.")]
		public int WeaponIndex { get; set; }


		[ProtoMember(2)]
		public Vector2 Size { get; set; }

	}

}
