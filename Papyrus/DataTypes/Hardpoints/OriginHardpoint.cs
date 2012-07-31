using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes.Hardpoints
{

	/// <summary>
	/// The origin of the hardpoint collection
	/// </summary>
	[ProtoContract]
	public sealed class OriginHardpoint : Hardpoint
	{

		public OriginHardpoint()
		{
			Name = "Origin";
		}

	}

}
