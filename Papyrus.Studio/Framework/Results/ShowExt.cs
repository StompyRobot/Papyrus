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
