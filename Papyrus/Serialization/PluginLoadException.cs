using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Serialization
{

	/// <summary>
	/// Indicates an error while attempting to resolve a pointer
	/// </summary>
	public class PluginLoadException : Exception
	{
		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginLoadException() { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginLoadException(string message) : base(message) { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginLoadException(string message, Exception innerException) : base(message, innerException) { }

	}

}
