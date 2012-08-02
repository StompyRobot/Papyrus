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
