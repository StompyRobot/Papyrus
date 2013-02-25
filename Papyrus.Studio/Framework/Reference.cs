using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Papyrus.Studio.Framework
{

	[ContentProperty("Name")]
	public class Reference : System.Windows.Markup.Reference
	{
		public Reference() : base() { }
		public Reference(string name) : base(name) { }
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
				return null;
			return base.ProvideValue(serviceProvider);
		}
	}

}
