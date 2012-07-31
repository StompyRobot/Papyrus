using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;


namespace Papyrus.DataTypes
{

	public enum ShipControlMode
	{
		Fighter, // ship rotates to face cursor
		ShipOfTheLine // turrets rotate to face cursor
	}

	public enum ShipAIMode
	{
		None,
		Fighter,
		Trader
	}

	//[Serializable]
	[ProtoContract]
	public class Ship : Record
	{

		public Ship()
		{
			Name = "";
			Graphic = DataPointer<ShipGraphic>.Empty;
			Mass = 10;
			Acceleration = 5;
			MaxVelocity = 4;
			SideAcceleration = 3;
			TurnSpeed = 90;
			Centre = new Vector2(0,0);
			TechLevel = TechLevel.Level1;
			DefaultWeapons = new DataPointerList<WeaponItem>();
			BaseArmourStrength = 100;
			BaseShieldStrength = 100;
			WeaponSlotCount = 0;
			DefaultEngine = DataPointer<EngineItem>.Empty;
			DefaultShield = DataPointer<ShieldItem>.Empty;
			DefaultArmour = DataPointer<ArmourItem>.Empty;
			ControlMode = ShipControlMode.Fighter;
		}

		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(8)]
		public DataPointer<ShipGraphic> Graphic { get; set; }


		[ProtoMember(2)]
		public float Mass { get; set; }

		[ProtoMember(3)]
		public float Acceleration { get; set; }

		[ProtoMember(6)]
		public float MaxVelocity { get; set; }

		/*[DataRange(Minimum = 1.0f, Maximum = 200.0f)]
		[ProtoMember(12)]
		public float SideVelocity { get; set; }*/

		[ProtoMember(13)]
		public float SideAcceleration { get; set; }

		/// <summary>
		/// Turn speed in degrees/s
		/// </summary>
		[DataRange(Minimum = 1.0f, Maximum = 360.0f)]
		[ProtoMember(9)]
		public float TurnSpeed { get; set; }

		[ProtoMember(4)]
		public Vector2 Centre { get; set; }

		[ProtoMember(5)]
		public TechLevel TechLevel { get; set; }

		[ProtoMember(10)]
		public float BaseShieldStrength { get; set; }

		[ProtoMember(11)]
		public float BaseArmourStrength { get; set; }

		[ProtoMember(12)]
		public DataPointerList<WeaponItem> DefaultWeapons { get; set; }

		[ProtoMember(14)]
		public DataPointer<EngineItem> DefaultEngine { get; set; }

		[ProtoMember(15)]
		public DataPointer<ShieldItem> DefaultShield { get; set; }

		[ProtoMember(16)]
		public DataPointer<ArmourItem> DefaultArmour { get; set; }

		[ProtoMember(17)]
		[Description("Number of weapon slots in this ship. Ensure that the ShipGraphic has enough weapon indexes to prevent weird graphical problems.")]
		public int WeaponSlotCount { get; set; }

		[ProtoMember(18),
		Description("How this ship should be controlled by the player.")]
		public ShipControlMode ControlMode { get; set; }

		[ProtoMember(19), Description("Which AI should this ship use.")]
		public ShipAIMode AIMode { get; set; }

	}
}
