/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Papyrus.Converters;
using ProtoBuf;
namespace Papyrus.DataTypes
{


	[ProtoContract, TypeConverterAttribute(typeof(IntSizeTypeConverter)), JsonObject(MemberSerialization.OptIn)]
	public struct IntSize
	{

		

		public IntSize(int width, int height) : this()
		{
			Width = width;
			Height = height;
		}

		public IntSize(int size) : this(size, size) { }

		[ProtoMember(1), JsonProperty]
		public int Width { get; set; }
		[ProtoMember(2), JsonProperty]
		public int Height { get; set; }

		public override string ToString()
		{
			return string.Format("{0},{1}", Width, Height);
		}

	}

}
