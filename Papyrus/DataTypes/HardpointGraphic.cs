using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes.Hardpoints;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	public enum HardpointType
	{
		None,
		Turret, // A link to another hardpoint. Places the origin of the linked hardpoint at the turret position
		Origin, // The origin of the graphic
		WeaponSource, // A source for weapons to emit from (beams, projectiles)
		Engine // Indicates an engine is at this location
	}

	[ProtoContract]
	public abstract class HardpointGraphic : Record
	{

		[ProtoMember(1, DynamicType = true, OverwriteList = true)]
		public List<Hardpoints.Hardpoint> Hardpoints { get; set; }

		[ProtoMember(2)]
		public Vector2 Size { get; set; }

		[ProtoMember(3)]
		public string AssetPath { get; set; }

		protected HardpointGraphic()
		{
			Hardpoints = new List<Hardpoint>();
			Hardpoints.Add(new OriginHardpoint(){Name = "Origin", Position = new Vector2(0)});
			Size = new Vector2(1.0f);
		}

		[Obsolete("Weapon count now defined in Ship data object")]
		public int WeaponCount()
		{

			return 0;

		}

		public abstract bool AllowsType(HardpointType type);

		public override IEnumerable<DataPointer> GetDataPointers()
		{
			return Hardpoints.Aggregate(base.GetDataPointers().ToList(), (list, hardpoint) =>
			                                                             {
			                                                             	list.AddRange(hardpoint.GetDataPointers().ToList());
			                                                             	return list;
			                                                             });
		}

		internal override void ResolveDependencies(RecordDatabase database)
		{
			base.ResolveDependencies(database);

			foreach (var hardpoint in Hardpoints) {
				hardpoint.ResolveDependencies(database);
			}
		}

	}
}
