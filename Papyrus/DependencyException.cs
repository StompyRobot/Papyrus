using System;


namespace Papyrus
{

	/// <summary>
	/// Indicates an error while attempting to resolve a dependency
	/// </summary>

	public class DependencyException : Exception
	{
		/// <summary>Creates a new ProtoException instance.</summary>
		public DependencyException() { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public DependencyException(string message) : base(message) { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public DependencyException(string message, Exception innerException) : base(message, innerException) { }

	}

}
