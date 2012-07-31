using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{
	/// <summary>
	/// Indicates an error while attempting to resolve a pointer
	/// </summary>

	public class DataPointerException : Exception
	{
		/// <summary>Creates a new ProtoException instance.</summary>
		public DataPointerException() { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public DataPointerException(string message) : base(message) { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public DataPointerException(string message, Exception innerException) : base(message, innerException) { }

	}
}
