using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Effects
{

	[ProtoContract]
	[ProtoInclude(1, typeof(WeaponMultiplyProperty))]
	[ProtoInclude(2, typeof(WeaponIncrementProperty))]
	[ProtoInclude(3, typeof(WeaponChangeProjectile))]
	[ProtoInclude(4, typeof(WeaponAddDamage))]
	[ProtoInclude(5, typeof(WeaponMultiplyDamage))]

	public abstract class WeaponUpgradeEffect : UpgradeEffect
	{
		
		public WeaponUpgradeEffect()
		{
			base.TargetSlot = Slot.Weapon;
		}

		[ReadOnly(true)]
		public override Slot TargetSlot
		{
			get
			{
				return base.TargetSlot;
			}
		}

	}

	[ProtoContract]
	public class WeaponMultiplyProperty : WeaponUpgradeEffect
	{

		public WeaponMultiplyProperty()
		{
			Multiplier = 1;
		}

		[ProtoMember(1)]
		public WeaponNumberProperties Property { get; set; }

		[ProtoMember(2)]
		public float Multiplier { get; set; }

		public override int Precedence { get { return 1; } }

		public override string ToString()
		{
			return string.Format("Multiply {0}*{1}", Property.ToString(), Multiplier.ToString());
		}

		public override void Apply(Item item)
		{

			Debug.Assert(item is WeaponItem);

			var prop = Property.ToString();
			Debug.Assert(item.HasProperty(prop));

			object initialValue = item.GetPropertyValue(prop);
			var result = Convert.ToDouble(initialValue) * Multiplier;

			item.SetPropertyValue(prop, Convert.ChangeType(result, initialValue.GetType()));

		}
	}

	[ProtoContract]
	public class WeaponIncrementProperty : WeaponUpgradeEffect
	{

		public WeaponIncrementProperty()
		{
			Amount = 0;
		}

		[ProtoMember(1)]
		public WeaponNumberProperties Property { get; set; }

		[ProtoMember(2)]
		public float Amount { get; set; }

		public override int Precedence { get { return 0; } }

		public override string ToString()
		{
			return string.Format("Increment {0}+{1}", Property.ToString(), Amount.ToString());
		}

		public override void Apply(Item item)
		{

			Debug.Assert(item is WeaponItem);

			var prop = Property.ToString();
			Debug.Assert(item.HasProperty(prop));

			object initialValue = item.GetPropertyValue(prop);
			var result = Convert.ToDouble(initialValue) + Amount;

			item.SetPropertyValue(prop, Convert.ChangeType(result, initialValue.GetType()));

		}

	}

	[ProtoContract]
	public class WeaponChangeProjectile : WeaponUpgradeEffect
	{

		[ProtoMember(2)]
		public DataPointer<ProjectileGraphic> Projectile { get; set; }

		public override int Precedence { get { return 0; } }

		public override string ToString()
		{
			return "ChangeProjectile";
		}

		public override void Apply(Item item)
		{
			Debug.Assert(item is WeaponItem);
			(item as WeaponItem).ProjectileGraphic = Projectile;
		}

	}

	[ProtoContract]
	public class WeaponAddDamage : WeaponUpgradeEffect
	{

		public WeaponAddDamage()
		{
			Damage = new Damage();
		}

		[ProtoMember(1)]
		public Damage Damage { get; set; }

		public override int Precedence { get { return 2; } }

		public override string ToString()
		{
			return string.Format("Add Damage ({0})", Damage.ToString());
		}

		public override void Apply(Item item)
		{
			Debug.Assert(item is WeaponItem);

			var witem = item as WeaponItem;
			var index = witem.Damage.FindIndex(p => p.Type == Damage.Type);

			if (index < 0)
				witem.Damage.Add(Damage);
			else
				witem.Damage[index] = new Damage() { Quantity = witem.Damage[index].Quantity + Damage.Quantity, Type = Damage.Type };

		}

	}

	[ProtoContract]
	public class WeaponMultiplyDamage : WeaponUpgradeEffect
	{

		public WeaponMultiplyDamage()
		{
			Damage = new Damage();
		}

		[ProtoMember(1)]
		public Damage Damage { get; set; }

		public override int Precedence { get { return 3; } }

		public override string ToString()
		{
			return string.Format("Multiply Damage ({0})", Damage.ToString());
		}

		public override void Apply(Item item)
		{
			Debug.Assert(item is WeaponItem);
			var witem = item as WeaponItem;
			var index = witem.Damage.FindIndex(p => p.Type == Damage.Type);

			if (index < 0)
				return;

			witem.Damage[index] = new Damage(){Quantity = witem.Damage[index].Quantity * Damage.Quantity, Type = Damage.Type};

		}

	}

}
