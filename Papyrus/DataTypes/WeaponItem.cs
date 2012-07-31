using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	/// <summary>
	/// The properties modifiable from WeaponEffects. These should match the property names exactly.
	/// </summary>
	public enum WeaponNumberProperties
	{
		WindUp, Cooldown, EnergyUsage, TurningSpeed, Velocity, TrackingEnd, TrackingDelay, Lifetime, Length, Width, ActiveTime
	}

	public enum WeaponType
	{
		Beam, Projectile, Field, RailGun
	}

	[ProtoContract]
	public class WeaponItem : Item
	{

		public WeaponItem()
		{
			Damage = new List<Damage>();
			_size = new IntSize(3, 2);
			DamageBuild = 0f;
			//Beam = DataPointer<Beam>.Empty;
			ProjectileGraphic = DataPointer<ProjectileGraphic>.Empty;
			Knockback = 0.5f;
		}

		[ProtoMember(1), Category("Weapon"),
		Description("The type of weapon. (Beam, RailGun, Projectile, Field) " +
			"\n Beam - A constant beam of energy doing dps " + "" +
			"\n Projectile - A physical projectile which does damage when it hits a ship, optionally tracking it's target." + 
			"\n Field - Causes damage in a radius around the weapon per second." +
			"\n RailGun - Causes a single burst of damage in one direction, optionally penetrating targets")]
		public WeaponType WeaponType { get; set; }

		[ProtoMember(2), Category("Weapon"), Description("Time after the fire button is pressed for the first round to fire. (Seconds)")]
		public float WindUp { get; set; }

		[ProtoMember(3), Category("Weapon"), Description("Time after firing before the windup starts again. (Seconds)")]
		public float Cooldown { get; set; }

		[ProtoMember(4), Category("Weapon"), Description("Energy usage per round (projectile) or per second (beam).")]
		public float EnergyUsage { get; set; }

		[ProtoMember(5), Category("Weapon"), Description("Damage per hit (projectile) or damage per second (beam)")]
#if WINDOWS
		//[Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemsSource(typeof(SubclassItemSource<Damage>))] // Add the item source for the collection editor
#endif
		public List<Damage> Damage { get; set; }

		[ProtoMember(10), Category("Projectile"), Description("Tracking type used by this projectile.")]
		public TrackingType TrackingType { get; set; }

		[ProtoMember(11), DataRange(Minimum = 1.0f, Maximum = 360.0f), Category("Projectile")]
		public float TurningSpeed { get; set; }

		[ProtoMember(12), DataRange(Minimum = 1, Maximum = 600), Category("Projectile")]
		public float Velocity { get; set; }

		[ProtoMember(13), DataRange(Minimum = 0.1f, Maximum = 60.0f), Category("Projectile"), Description("Time that passes before tracking targets ceases.")]
		public float TrackingEnd { get; set; }

		[ProtoMember(14), DataRange(Minimum = 0.1f, Maximum = 60.0f), Category("Projectile"), Description("Time to wait until starting to track a target")]
		public float TrackingDelay { get; set; }

		[ProtoMember(15), Category("Projectile"), Description("Duration this projectile should stay active.")]
		public float Lifetime { get; set; }

		[ProtoMember(16), Category("Projectile")]
		public DataPointer<ProjectileGraphic> ProjectileGraphic { get; set; }

		//[ProtoMember(20), Category("Beam")]
		//public DataPointer<Beam> Beam { get; set; }

		[ProtoMember(21), Category("Beam"), Description("Length of the beam. Negative value for infinite.")]
		public float Length { get; set; }

		[ProtoMember(22), Category("Beam"), Description("Width of the beam.")]
		public float Width { get; set; }

		[ProtoMember(23), Category("Beam"), Description("How long the beam can be active for before deactivating automatically. Negative value for infinite.")]
		public float ActiveTime { get; set; }

		[ProtoMember(24), Category("Weapon"), Description("How many rounds are fired per firing round")]
		public int RoundCount { get; set; }

		[ProtoMember(25), Category("Weapon"), Description("Time between seperate projectiles of a round")]
		public float RoundTime { get; set; }

		[ProtoMember(26), Category("Beam"), Description("Multiply contact time by this value and add to damage."), DefaultValue(0f)]
		public float DamageBuild { get; set; }

		[ProtoMember(27), Category("Ray"), Description("Should this weapon do a fixed amount of damage, not constant."), DefaultValue(false)]
		public bool FixedDamage { get; set; }

		[ProtoMember(28), Category("Weapon"), Description("If the weapon firing cycle is halted before cooldown is reached, the cooldown is reduced by the % of the firing cycle unused.")]
		public bool PartialDischarge { get; set; }

		[ProtoMember(29), Category("Projectile"), Description("Sound to play when firing."), DefaultValue(null)]
		public string FireSound { get; set; }

		[ProtoMember(30), Category("Weapon"), Description("Force to apply to a ship when hit by this weapon."), DefaultValue(0.5f)]
		public float Knockback { get; set; }

		[ReadOnly(true)]
		public override IntSize Size
		{
			get
			{
				return new IntSize(3, 2);
			}
		}

		public override Slot UpgradeSlot
		{
			get
			{
				return Slot.Weapon;
			}
		}

		public override string ItemType
		{
			get { return "WeaponItem"; }
		}

	}


}
