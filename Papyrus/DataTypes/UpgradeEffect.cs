using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Papyrus.DataTypes.Effects;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	[ProtoInclude(10, typeof(WeaponUpgradeEffect))]
	public abstract class UpgradeEffect
	{

		protected UpgradeEffect()
		{
			ShowInGUI = true;
		}

		[ProtoMember(1)] private Slot _slot;

		[ProtoMember(2)]
		[DefaultValue(true), Description("Should this upgrade effect be shown to the user in the GUI?")]
		public bool ShowInGUI { get; set; }

		/// <summary>
		/// The order in which this effect should be applied. eg Addition before Multiplication
		/// Smaller = Last, Higher = First
		/// </summary>
		[Browsable(false)]
		public abstract int Precedence { get; }

		[Description("The slot the upgrade must be in to trigger this effect.")]
		virtual public Slot TargetSlot
		{
			get { return _slot; }
			set
			{
				if (value.GetFlags().Count() > 1)
					throw new InvalidOperationException("Can only have one flag set on an effect.");
				_slot = value;
			}
		}

		public abstract void Apply(Item item);


	}

}
