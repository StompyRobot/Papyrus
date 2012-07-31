using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Hardpoints
{
	[ProtoContract]
	[ProtoInclude(10, typeof(TurretHardpoint))]
	[ProtoInclude(11, typeof(WeaponSourceHardpoint))]
	[ProtoInclude(12, typeof(OriginHardpoint))]
	[ProtoInclude(13, typeof(EngineHardpoint))]
	public abstract class Hardpoint
	{

		[ProtoMember(1)]
		public Vector2 Position { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3), DefaultValue(0)]
		public float Rotation { get; set; }

		internal virtual List<DataPointer> GetDataPointers() { return new List<DataPointer>(); } 

		internal void ResolveDependencies(RecordDatabase database)
		{
			GetDataPointers().ForEach(p => (p as DataPointer).ResolvePointer(database));
		}

	}
}
