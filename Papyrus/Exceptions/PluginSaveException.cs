using System;

namespace Papyrus.Exceptions
{
	/// <summary>
	/// Indicates an error while attempting to resolve a pointer
	/// </summary>
	public class PluginSaveException : Exception
	{
		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginSaveException() { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginSaveException(string message) : base(message) { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public PluginSaveException(string message, Exception innerException) : base(message, innerException) { }

	}
}
