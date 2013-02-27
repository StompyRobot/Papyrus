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

namespace Papyrus.Studio.Modules.RecordBrowser.ViewModels
{
	public class RecordTypeViewModel
	{

		public RecordTypeViewModel(Type type)
		{
			SubTypes = new List<RecordTypeViewModel>();
			Type = type;
		}

		public Type Type { get; set; }

		public List<RecordTypeViewModel> SubTypes { get; set; }

		public bool Visible { get; set; }

		public RecordTypeViewModel Parent { get; set; }

	}
}
