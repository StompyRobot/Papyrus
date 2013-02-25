using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Studio.Framework.Services
{

	public interface IExceptionHandler
	{

		void HandleException(Exception e, Action callback);

	}

}
