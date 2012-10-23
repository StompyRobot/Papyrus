/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus
{

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class PapyrusModuleAttribute : Attribute
	{
		public Guid ID { get; private set; }

		public Type RootRecord { get; private set; }

		public PapyrusModuleAttribute(string guid, Type rootRecord)
		{
			
			Debug.Assert(typeof(Record).IsAssignableFrom(rootRecord));

			RootRecord = rootRecord;
			
			ID = new Guid(guid);

		}

	}

}
