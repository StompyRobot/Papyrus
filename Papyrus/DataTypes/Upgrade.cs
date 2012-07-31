using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{


	[ProtoContract]
	public class Upgrade : Item
	{

		public Upgrade()
		{
			Effects = new List<UpgradeEffect>();
			Slots = Slot.None;
		}

		[ProtoMember(1)]
		public Slot Slots { get; set; }

		[ProtoMember(2, DynamicType = true)]
		[Editor("Data.Design.Controls.EffectCollectionEditor, Data.Design", "Data.Design.Controls.EffectCollectionEditor, Data.Design")]
		public List<UpgradeEffect> Effects { get; set; }

		public override Slot UpgradeSlot { 
			get {
				return Slot.None;
			} 
		}

		public override string ItemType
		{
			get { return "Upgrade"; }
		}

	}

}
