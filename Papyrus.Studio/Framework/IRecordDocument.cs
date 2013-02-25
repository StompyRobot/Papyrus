using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemini.Framework;
using Papyrus.DataTypes;

namespace Papyrus.Studio.Framework
{

	public interface IRecordDocument : IDocument
	{

		Record Record { get; }

	}

}
