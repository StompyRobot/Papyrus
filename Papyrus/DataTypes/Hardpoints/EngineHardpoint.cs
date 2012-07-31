using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Hardpoints
{

	[Flags]
	public enum EngineDirection
	{
		Forward, Reverse, Left, Right
	}

	[ProtoContract]
	public sealed class EngineHardpoint : Hardpoint
	{

		public EngineHardpoint()
		{
			Name = "Engine";
			Size = new Vector2(0.25f, 0.5f);
			Rotation = 3.14159f;
		}

		[ProtoMember(1)]
		public Vector2 Size { get; set; }

		[ProtoMember(2)]
		public EngineDirection Direction { get; set; }

	}
}
