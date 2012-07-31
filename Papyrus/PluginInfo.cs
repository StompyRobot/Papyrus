using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using DataFormat = Papyrus.Serialization.DataFormat;

namespace Papyrus
{

	/// <summary>
	/// A class describing a plugin.
	/// </summary>
	public class PluginInfo
	{

		/// <summary>
		/// Is this plugin enabled or not.
		/// </summary>
		public bool Enabled { get; set; } 

		/// <summary>
		/// False if this plugin cannot be loaded for one reason or another.
		/// </summary>
		public bool IsValid { get; set; }

		/// <summary>
		/// Name of this plugin
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// File this plugin is loaded from
		/// </summary>
		public string SourceFile { get; set; }

		/// <summary>
		/// Description for this plugin
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Author of this plugin
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// The time this plugin was last modified.
		/// </summary>
		public DateTime LastModified { get; set; }

		/// <summary>
		/// List of plugins this plugin depends on
		/// </summary>
		public List<string> Dependencies { get; set; }

		/// <summary>
		/// Used by the editor to determine if this plugin is currently
		/// active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Data format this plugin is saved into
		/// </summary>
		public Serialization.DataFormat Format { get; set; }

		public PluginInfo Copy()
		{
			var pluginInfo = (PluginInfo)this.MemberwiseClone();
			if(Dependencies != null)
				pluginInfo.Dependencies = new List<string>(Dependencies);
			return pluginInfo;
		}

	}

}
