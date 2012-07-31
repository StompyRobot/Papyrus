using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.DataTypes
{

	public enum DamageType
	{
		
		Normal,
		EMP,
		Pierce

	}

	public enum TrackingType
	{
		None,
		Radar,
		IR
	}

	[Flags]
	public enum HardPointType
	{
		Turret = 0x1,
		Fixed = 0x2
	}

	public enum TechLevel
	{
		Level0 = 0,
		Level1 = 0x1,
		Level2 = 0x2,
		Level3 = 0x4,
		Level4 = 0x8,
		Level5 = 0x16
	}

	[Flags]
	public enum Slot
	{
		None	= 0x0,
		Weapon	= 0x1,
		Engine	= 0x2,
		Sensor	= 0x4,
		Shield	= 0x8,
		Armour	= 0x16,
		Misc	= 0x32
	}

	public enum LinkType
	{
		None,
		SpaceElevator,
		Ring
	}
}
