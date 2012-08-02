using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

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
