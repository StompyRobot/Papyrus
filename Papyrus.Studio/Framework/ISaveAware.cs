using System.Collections.Generic;
using Caliburn.Micro;

namespace Papyrus.Studio.Framework
{
	public interface ISaveAware {

		/// <summary>
		/// Save the record
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> Save();

	}
}