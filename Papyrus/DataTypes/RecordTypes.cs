using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.DataTypes
{
	/// <summary>
	/// Contains a list of record types.
	/// </summary>
	static public class RecordTypes
	{

		public class RecordType
		{

			/// <summary>
			/// The record which this record type inherits from
			/// </summary>
			public Type ParentRecordType { get; set; }

			/// <summary>
			/// The type of the record.
			/// </summary>
			public Type Type { get; set; }

			/// <summary>
			/// The index to start protocol field numbers at. DO NOT CHANGE ONCE SET. Will break backwards
			/// compatability!
			/// </summary>
			public int IndexStart = 10;

			public RecordType(Type type, Type parentType = null)
			{
				this.Type = type;
				this.ParentRecordType = parentType ?? typeof (Record);
			}

		}

		/// <summary>
		/// List of record types. Mainly used for protocol buffers.
		/// If removing a record type, replace it with null to maintain backwards
		/// compatability. eg:
		/// <code>new RecordType(null, typeof(Item));</code>
		/// </summary>
		public static readonly RecordType[] Types = new RecordType[] 
		{

		    new RecordType(typeof(Ship)),

			new RecordType(typeof(StarSystem)),
			new RecordType(typeof(Planet)),
			new RecordType(null/*,typeof(Beam)*/),
			new RecordType(typeof(PlanetGraphic)),

			new RecordType(typeof(Graphic)),
			new RecordType(typeof(Station)),
			new RecordType(typeof(Government)),

			new RecordType(typeof(Item)) { IndexStart = 10 },
				new RecordType(typeof(WeaponItem),	typeof(Item)),
				new RecordType(typeof(Upgrade),		typeof(Item)),
				new RecordType(typeof(EngineItem),	typeof(Item)),
				new RecordType(typeof(ArmourItem),	typeof(Item)),
				new RecordType(typeof(ShieldItem),	typeof(Item)),

			new RecordType(typeof(ProjectileGraphic)),

			new RecordType(typeof(HardpointGraphic)),
				new RecordType(typeof(TurretGraphic), typeof(HardpointGraphic)),
				new RecordType(typeof(ShipGraphic), typeof(HardpointGraphic))
                                            	
		};

	}
}
