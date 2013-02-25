using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Papyrus;
using Papyrus.Studio.Framework.Controls;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework
{

	public class PapyrusPropertyControlFactory : DefaultPropertyControlFactory
	{

		public static Type FactoryType = typeof(PapyrusPropertyControlFactory);

		public static IPropertyControlFactory GetControlFactory()
		{
			return Activator.CreateInstance(FactoryType) as IPropertyControlFactory;
		}

		public PapyrusPropertyControlFactory()
		{
			
		}

		public override System.Windows.FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options)
		{

			if (typeof(RecordReference).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceControl(property);

			}

			if (typeof (IRecordReferenceList).IsAssignableFrom(property.ActualPropertyType)) {

				return CreateRecordReferenceListControl(property);

			}

			if (typeof (Papyrus.DataTypes.Color) == property.ActualPropertyType) {

				return CreateColorControl(property);

			}

			return base.CreateControl(property, options);

		}

		public FrameworkElement CreateRecordReferenceControl(PropertyItem item)
		{

			var c = new RecordReferenceItem();
			c.SetBinding(RecordReferenceItem.RecordReferenceProperty, item.CreateBinding());
			return c;

		}

		public FrameworkElement CreateRecordReferenceListControl(PropertyItem item)
		{

			var c = new RecordList();
			c.SetBinding(RecordList.SourceListProperty, item.CreateBinding());
			return c;

		}

	}

}
