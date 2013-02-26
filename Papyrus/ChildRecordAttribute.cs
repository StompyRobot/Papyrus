/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;

namespace Papyrus
{

	/// <summary>
	/// Register a record type as a child of this record class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ChildRecordAttribute : Attribute
	{
		/// <summary>
		/// Type of the child record
		/// </summary>
		public Type ChildType { get; private set; }

		/// <summary>
		/// Field number to use for this child record
		/// </summary>
		public int FieldNo { get; private set; }

		public ChildRecordAttribute(int fieldNo, Type childType)
		{

			FieldNo = fieldNo;
			ChildType = childType;

		}

	}

}
