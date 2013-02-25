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
