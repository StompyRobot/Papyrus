using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	[ProtoContract]
	public class ProjectileGraphic : Record
	{

		public ProjectileGraphic()
		{
			GraphicPath = ParticlePath = "";
			GraphicTint = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}

		[DisplayName("Graphic Path"),
		Description("Content path to the graphic.")]
		[ProtoMember(1)]
		public string GraphicPath { get; set; }

		[DisplayName("Particle Path"),
		Description("Content Path to the particle")]
		[ProtoMember(2)]
		public string ParticlePath { get; set; }

		[DisplayName("Projectile Size"),
		 Description("Size of the projectile in simulation units (0.1-10.0"), ProtoMember(3), DefaultValue(0.1f)]
		public float ProjectileSize { get; set; }

		[ProtoMember(4), Description("Color to tint the graphic")]
		public Color GraphicTint { get; set; }

	}
}
