using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus
{

	/// <summary>
	/// Contains a list of plugins in the order they should be loaded.
	/// </summary>
	public class LoadOrder
	{

		public List<string> Plugins { get; protected set; }

		public LoadOrder()
		{
			Plugins = new List<string>();
		}

	}

}
