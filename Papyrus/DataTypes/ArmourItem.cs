using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class ArmourItem : Item
	{

		public ArmourItem()
		{
			ArmourMult = 1.0f;
		}

		[ProtoMember(1)]
		[DefaultValue(1.0f)]
		[DisplayName("Armour Multiplier")]
		[Description("By how much to multiply the base armour")]
		public float ArmourMult { get; set; }

		public override Slot UpgradeSlot
		{
			get { return Slot.Armour; }
		}

		[ReadOnly(true)]
		public override IntSize Size
		{
			get
			{
				return new IntSize(2,2);
			}
		}

		public override string ItemType
		{
			get { return "ArmourItem"; }
		}

	}
}
