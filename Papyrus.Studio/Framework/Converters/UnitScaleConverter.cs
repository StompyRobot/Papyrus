using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Papyrus.Studio.Framework.Converters
{

	public class UnitScaleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double)
				return new Rect(0, 0, (double)value * 100, (double)value * 100);

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{

			throw new NotSupportedException();
			//return Binding.DoNothing;

		}

	}

}
