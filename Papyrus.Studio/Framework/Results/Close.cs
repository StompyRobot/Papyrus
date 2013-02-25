using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemini.Framework;

namespace Papyrus.Studio.Framework.Results
{

	public static class Close
	{

		public static CloseDocumentResult Document(IDocument document)
		{
			return new CloseDocumentResult(document);
		}

	}

}
