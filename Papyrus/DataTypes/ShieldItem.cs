using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class ShieldItem : Item
	{

		public ShieldItem()
		{
			ShieldMult = 1.0f;
		}

		[ProtoMember(1)]
		[DefaultValue(1.0f)]
		[DisplayName("Shield Multiplier")]
		[Description("By how much to multiply the base shield strength.")]
		public float ShieldMult { get; set; }


		[ReadOnly(true)]
		public override IntSize Size
		{
			get
			{
				return new IntSize(2);
			}
		}

		public override Slot UpgradeSlot
		{
			get { return Slot.Shield; }
		}

		public override string ItemType
		{
			get { return "ShieldItem"; }
		}

	}
}
