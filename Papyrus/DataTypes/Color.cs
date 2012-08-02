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

namespace Papyrus.DataTypes
{

	//[Serializable]
	[ProtoBuf.ProtoContract]
	[TypeConverter("Papyrus.Design.ColorTypeConverter, Papyrus.Design")]
	//[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Color
	{
		[ProtoBuf.ProtoMember(1)]
		public int R { get; set; }

		[ProtoBuf.ProtoMember(2)]
		public int G { get; set; }

		[ProtoBuf.ProtoMember(3)]
		public int B { get; set; }

		[ProtoBuf.ProtoMember(4)]
		public int A { get; set; }

		public Color()
		{
			R = G = B = A = 0;
		}

		public Color(int r, int g, int b, int a = 255)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public Color(float r, float g, float b, float a = 1.0f)
		{
			R = (int)(r * 255f);
			G = (int)(g * 255f);
			B = (int)(b * 255f);
			A = (int)(a * 255f);
		}

#if XNA

		/// <summary>
		/// Implicit conversion from data color to xna color
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns></returns>
		static public implicit operator Microsoft.Xna.Framework.Color(Color color)
		{
			return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
		}

#endif

	}
}
