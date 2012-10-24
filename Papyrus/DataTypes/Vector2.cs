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
using ProtoBuf;

namespace Papyrus.DataTypes
{
	[ProtoContract]
	[TypeConverter(typeof(Converters.Vector2TypeConverter))]
	public struct Vector2
	{

		/*public Vector2()
		{
			X = 0;
			Y = 0;
		}*/

		public Vector2(float val) : this()
		{
			X = Y = val;
		}

		public Vector2(float _X, float _Y) : this()
		{
			X = _X;
			Y = _Y;
		}

		[ProtoMember(1)]
		public float X { get; set; }

		[ProtoMember(2)]
		public float Y { get; set; }

		public override string ToString()
		{
			return X.ToString() + ", " + Y.ToString();
		}

#if XNA

		/// <summary>
		/// Implicit conversion from an xna vector to a data vector
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		static public implicit operator Vector2(Microsoft.Xna.Framework.Vector2 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		/// <summary>
		/// Implicit conversion from data vector to xna vector
		/// </summary>
		/// <param name="dataVec"></param>
		/// <returns></returns>
		static public implicit operator Microsoft.Xna.Framework.Vector2(Vector2 dataVec)
		{
			return new Microsoft.Xna.Framework.Vector2(dataVec.X, dataVec.Y);
		}

#endif

	}
}
