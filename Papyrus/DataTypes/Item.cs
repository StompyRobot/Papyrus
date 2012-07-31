using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public abstract class Item : Record
	{

		protected Item()
		{
			Purchasable = true;
			Icon = DataPointer<Graphic>.Empty;
		}

		[ProtoMember(1), Category("Item")]
		public string Name { get; set; }

		[ProtoMember(2), Category("Item")]
		public TechLevel TechLevel { get; set; }

		[ProtoMember(3), Category("Item")]
		public int Price { get; set; }

		[ProtoMember(4), Category("Item"), Description("The number of upgrade slots available for this item.")]
		[DefaultValue(1)]
		public uint SlotCount { get; set; }

		[ProtoMember(5), Category("Item"), Description("Can this item be purchased? Some examples of unpurchasable items are default engine/armour types for ships.")]
		[DefaultValue(true)]
		public bool Purchasable { get; set; }

		[ProtoMember(6), Category("Item"), Description("Icon")]
		public DataPointer<Graphic> Icon { get; set; }

		[ProtoMember(7)]
		protected IntSize _size = new IntSize(1);

		public abstract Slot UpgradeSlot { get; }

		public abstract string ItemType { get; }


		[Category("Item"), Description("The size of the item in the inventory.")]
		public virtual IntSize Size
		{
			get { return _size; }
			set { _size = value; }
		}

		/// <summary>
		/// Returns true if the specified property exists on this object
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public bool HasProperty(string prop)
		{
			return GetProperty(prop) != null;
		}

		/// <summary>
		/// Returns the property with the given name.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public PropertyInfo GetProperty(string prop)
		{
			return this.GetType().GetProperty(prop, BindingFlags.Instance | BindingFlags.Public);
		}

		/// <summary>
		/// Returns the value of the property with the given name
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public object GetPropertyValue(string prop)
		{
			 return GetProperty(prop).GetGetMethod().Invoke(this, null);
		}

		/// <summary>
		/// Sets the value of the given property
		/// </summary>
		/// <param name="prop"></param>
		/// <param name="value"></param>
		public void SetPropertyValue(string prop, object value)
		{
			GetProperty(prop).SetValue(this, value, null);
		}

	}
}
