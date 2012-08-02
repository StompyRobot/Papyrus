/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	/// <summary>
	/// Marks a property to be saved with this record
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public sealed class RecordPropertyAttribute : Attribute
	{

		/// <summary>
		/// Field number to mark this property. Must be unique - do not change after it has been set.
		/// </summary>
		public int FieldNumber { get; set; }

		/// <summary>
		/// Mark this property as a dynamic type which will be resolved on deserialization
		/// </summary>
		public bool DynamicType { get; set; }

		/// <summary>
		/// True to overwrite a list with the deserialized items
		/// </summary>
		public bool OverwriteList { get; set; }

		public RecordPropertyAttribute(int fieldNo)
		{
			FieldNumber = fieldNo;
		}

	}

}
