using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Papyrus.Studio.Framework.Services
{
	public static class ResultExtensions
	{

		public static IEnumerator<IResult> AsCoroutine(this IResult result)
		{
			return (new [] { result } as IEnumerable<IResult>).GetEnumerator();
		}

	}
}
