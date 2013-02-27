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
using System.Windows;

namespace Papyrus.Studio.Framework.Results
{
	public static class ShowExt
	{

		public static ShowModalResult Modal(object modal)
		{
			return new ShowModalResult(modal);
		}

		public static ShowExceptionResult Exception(Exception e)
		{
			return new ShowExceptionResult(e);
		}

	}
}
