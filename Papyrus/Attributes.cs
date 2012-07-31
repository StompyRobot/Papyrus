using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{


	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DataRangeAttribute : Attribute
	{
		public float Minimum = 0, Maximum = -1;
	}


}