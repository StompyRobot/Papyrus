using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Hardpoints
{

	/// <summary>
	/// Essentially a container of more hardpoints
	/// </summary>
	[ProtoContract]
	public class TurretHardpoint : Hardpoint
	{

		public TurretHardpoint()
		{
			Name = "Turret";
			TurretGraphic = DataPointer<TurretGraphic>.Empty;
			Scale = 1;
		}

		[ProtoMember(1)]
		public DataPointer<TurretGraphic> TurretGraphic { get; set; }

		[ProtoMember(2)]
		[Description("The range that this turret can turn. X = Left bound, Y = Right bound")]
		public Vector2 Range { get; set; }

		[ProtoMember(3)]
		[Description("Any weapon sources will have their id incremented by this amount.")]
		public int WeaponIDInc { get; set; }

		[ProtoMember(4), Description("Value to scale this turret by.")]
		public float Scale { get; set; }


		internal override List<DataPointer> GetDataPointers()
		{
			return new List<DataPointer>(){TurretGraphic};
		}

	}

}
