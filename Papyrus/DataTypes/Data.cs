using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ProtoBuf;
namespace Papyrus.DataTypes
{

	public class DataListAttribute : Attribute
	{

		public Type type;

		public DataListAttribute(Type _type)
		{
			type = _type;
		}

	}

	/// <summary>
	/// The root data object that is serialised into the database
	/// </summary>
	[ProtoContract(SkipConstructor = false)]
	public class Data
	{

		[ProtoMember(1)] public string DatabaseName;

		[ProtoMember(2), DataList(typeof(Ship))]
		public Dictionary<uint, Ship> Ships { get; set; }

		//[ProtoMember(5), DataList(typeof (ShipGraphic))]
		//public Dictionary<uint, ShipGraphic> ShipGraphics { get; set; }

		[ProtoMember(6), DataList(typeof(StarSystem))]
		public Dictionary<uint, StarSystem> Systems { get; set; }

		[ProtoMember(11), DataList(typeof(Graphic))]
		public Dictionary<uint, Graphic> Graphics { get; set; }

		[ProtoMember(12), DataList(typeof(Station))]
		public Dictionary<uint, Station> Stations { get; set; }

		[ProtoMember(13), DataList(typeof(Government))] 
		public Dictionary<uint, Government> Governments { get; set; }

		[ProtoMember(14), DataList(typeof(WeaponItem))]
		public Dictionary<uint, WeaponItem> WeaponItems { get; set; }

		[ProtoMember(15), DataList(typeof(Upgrade))]
		public Dictionary<uint, Upgrade> Upgrades { get; set; }

		[ProtoMember(16), DataList(typeof(EngineItem))]
		public Dictionary<uint, EngineItem> Engines { get; set; }

		[ProtoMember(17), DataList(typeof(ProjectileGraphic))]
		public Dictionary<uint, ProjectileGraphic> ProjectileGraphics { get; set; }

		[ProtoMember(18), DataList(typeof(ArmourItem))]
		public Dictionary<uint, ArmourItem> ArmourItems { get; set; }

		[ProtoMember(19), DataList(typeof(ShieldItem))]
		public Dictionary<uint, ShieldItem> ShieldItems { get; set; }

		[ProtoMember(20), DataList(typeof(ShipGraphic))]
		public Dictionary<uint, ShipGraphic> ShipGraphics { get; set; }
		
		[ProtoMember(21), DataList(typeof(TurretGraphic))]
		public Dictionary<uint, TurretGraphic> TurretGraphics { get; set; }

		Dictionary<string, Type> TypeLookup = new Dictionary<string, Type>(); 

		public Data()
		{
			DatabaseName = "New Database";
			Ships = new Dictionary<uint, Ship>();
			//ShipGraphics = new Dictionary<uint, ShipGraphic>();
			Systems = new Dictionary<uint, StarSystem>();

			Graphics = new Dictionary<uint, Graphic>();
			Stations = new Dictionary<uint, Station>();
			Governments = new Dictionary<uint, Government>();
			WeaponItems = new Dictionary<uint, WeaponItem>();
			Upgrades = new Dictionary<uint, Upgrade>();
			Engines = new Dictionary<uint, EngineItem>();
			ProjectileGraphics = new Dictionary<uint, ProjectileGraphic>();
			ArmourItems = new Dictionary<uint, ArmourItem>();
			ShieldItems = new Dictionary<uint, ShieldItem>();
			ShipGraphics = new Dictionary<uint, ShipGraphic>();
			TurretGraphics = new Dictionary<uint, TurretGraphic>();

			var props = GetType().GetProperties();
			foreach (var propertyInfo in props)
			{
				var attrib = Attribute.GetCustomAttribute(propertyInfo, typeof(DataListAttribute)) as DataListAttribute;

				if (attrib == null)
					continue;

				TypeLookup.Add(attrib.type.Name.ToLower(), attrib.type);
			}

		}


		#region Data Helper Methods (Slow, mainly for editor use)

		/// <summary>
		/// Returns the data list for the given type. (No type safety going on)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object GetDataList(Type type)
		{

			var props = GetType().GetProperties();
			foreach (var propertyInfo in props)
			{
				var attrib = Attribute.GetCustomAttribute(propertyInfo, typeof(DataListAttribute)) as DataListAttribute;

				if (attrib == null)
					continue;

				if (attrib.type == type)
				{

					return propertyInfo.GetValue(this, null);
				}

			}

			throw new Exception("Type " + type + " does not have a data list.");
		}

		/// <summary>
		/// Returns the data list of type T. Type safe.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public Dictionary<uint, T> GetDataList<T>() where T : Record
		{
			return GetDataList(typeof (T)) as Dictionary<uint, T>;
		} 


		/// <summary>
		/// Returns the item of type T with given id
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public T GetItem<T>(uint id) where T : Record
		{
			return GetDataList<T>()[id];
		}

		public bool ItemExists<T>(uint id) where T : Record
		{
			return GetDataList<T>().ContainsKey(id);
		}

		public Record GetItem(string type, uint id)
		{
			return GetItem(TypeLookup[type.ToLower()], id);
		}

		/// <summary>
		/// Takes a type and in id and returns the element corresponding to them
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public Record GetItem(Type type, uint id)
		{

			var methods = typeof(Data).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var method = methods.Single(p => p.Name == "GetItem" && p.IsGenericMethodDefinition);

			return method.MakeGenericMethod(type).Invoke(this, new object[]{id}) as Record;
		}

		public bool ItemExists(Type type, uint id)
		{

			var method = GetType().GetMethods(BindingFlags.Instance).Single(p => p.Name == "ItemExists" && p.IsGenericMethodDefinition);

			return (bool)method.MakeGenericMethod(type).Invoke(this, new object[] { id });

		}

		#endregion

	}

}
