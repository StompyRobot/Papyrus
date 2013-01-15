/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;

namespace Papyrus.Exceptions
{
	/// <summary>
	/// Indicates an error while attempting to resolve a pointer
	/// </summary>

	public class ReferenceException : Exception
	{
		/// <summary>Creates a new ProtoException instance.</summary>
		public ReferenceException() { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public ReferenceException(string message) : base(message) { }

		/// <summary>Creates a new ProtoException instance.</summary>
		public ReferenceException(string message, Exception innerException) : base(message, innerException) { }

	}
}
