using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class EngineItem : Item
	{

		public EngineItem()
		{
			VelocityMultiplier = AccelerationMultiplier = SecondAccelerationMultiplier = 1;
			_size = new IntSize(2, 2);
		}

		/// <summary>
		/// how much to multiply the ship's base speed by
		/// </summary>
		[ProtoMember(1), DefaultValue(1)]
		public float VelocityMultiplier { get; set; }

		/// <summary>
		/// how much to multiply the ship's base Acceleration by
		/// </summary>
		[ProtoMember(2), DefaultValue(1)]
		public float AccelerationMultiplier { get; set; }

		[ProtoMember(3), Description("The amount to multiply the side-thrusters acceleration by"), DefaultValue(1)]
		public float SecondAccelerationMultiplier { get; set; }

		[ReadOnly(true)]
		public override IntSize Size
		{
			get
			{
				return new IntSize(2, 2);
			}
		}

		public override Slot UpgradeSlot
		{
			get
			{
				return Slot.Engine;
			}
		}

		public override string ItemType
		{
			get { return "EngineItem"; }
		}


	}

}
