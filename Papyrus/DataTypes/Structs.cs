using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Papyrus.DataTypes
{

	[ProtoContract]
	//[TypeConverterAttribute(typeof(DamageStructConverter))]
	public class Damage
	{
		[ProtoMember(1)]
		public DamageType Type { get; set; }

		[ProtoMember(2)]
		public float Quantity { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1})", Quantity.ToString(), Type.ToString());
		}
	}


	[ProtoContract/*, TypeConverterAttribute(typeof(IntSizeStructConverter))*/]
	public struct IntSize
	{

		

		public IntSize(int width, int height) : this()
		{
			Width = width;
			Height = height;
		}

		public IntSize(int size) : this(size, size) { }

		[ProtoMember(1)]
		public int Width { get; set; }
		[ProtoMember(2)]
		public int Height { get; set; }

		public override string ToString()
		{
			return string.Format("{0},{1}", Width, Height);
		}

	}

}
