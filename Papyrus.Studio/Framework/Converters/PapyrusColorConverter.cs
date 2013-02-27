using System;
using System.Globalization;
using System.Windows.Data;

namespace Papyrus.Studio.Framework.Converters
{

	public class PapyrusColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{


			if (value is Papyrus.DataTypes.Color && targetType == typeof (System.Windows.Media.Color)) {

				var color = (DataTypes.Color)value;
				return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

			}

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{

			if (targetType == typeof (DataTypes.Color) && value is System.Windows.Media.Color) {

				var color = (System.Windows.Media.Color) value;
				return new DataTypes.Color(color.R, color.G, color.B, color.A);

			}

			throw new NotSupportedException();
			//return Binding.DoNothing;

		}

	}
}
