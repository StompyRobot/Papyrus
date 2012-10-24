/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

#if false

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Color = Papyrus.DataTypes.Color;

namespace Papyrus.Design
{
	public class ColorTypeConverter : TypeConverter
	{

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{

			if (sourceType == typeof(string))
				return true;
			if (sourceType == typeof(System.Drawing.Color))
				return true;
			if (sourceType == typeof(System.Windows.Media.Color))
				return true;
			
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{

			if (destinationType == typeof(System.Drawing.Color))
				return true;
			if (destinationType == typeof(string))
				return true;
			if (destinationType.IsAssignableFrom(typeof(SolidColorBrush)))
				return true;
			if(destinationType == typeof(System.Windows.Media.Color))
			return true;
			
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{

			if(value is string) {

				var stringValue = (string) value;
				var values = stringValue.Split(',').ToList().ConvertAll(p => p.Trim());
				
				if(values.Count != 3 && values.Count != 4)
					throw new FormatException("Invalid number of params");

				var newColor = new DataTypes.Color();
				newColor.R = Math.Min(255, Math.Max(int.Parse(values[0]), 0));
				newColor.G = Math.Min(255, Math.Max(int.Parse(values[1]), 0));
				newColor.B = Math.Min(255, Math.Max(int.Parse(values[2]), 0));

				if (values.Count == 4)
					newColor.A = Math.Min(255, Math.Max(int.Parse(values[3]), 0));
				else
					newColor.A = 255;

				return newColor;

			}

			if(value is System.Drawing.Color) {

				var colorValue = (System.Drawing.Color) value;

				return new DataTypes.Color(colorValue.R, colorValue.G, colorValue.B, colorValue.A);

			}

			if(value is System.Windows.Media.Color) {

				var colorValue = (System.Windows.Media.Color) value;

				return new DataTypes.Color(colorValue.R, colorValue.G, colorValue.B, colorValue.A);

			}
			
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{

			var color = value as DataTypes.Color;

			if(destinationType == typeof(string)) {

				return string.Format("{0}, {1}, {2}, {3}", color.R, color.G, color.B, color.A);

			}

			if(destinationType == typeof(System.Drawing.Color)) {

				return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

			}

			if (destinationType.IsAssignableFrom(typeof(SolidColorBrush))) {

				return new SolidColorBrush((System.Windows.Media.Color)ConvertTo(context, culture, color, typeof(System.Windows.Media.Color)));

			}

			if(destinationType == typeof(System.Windows.Media.Color)) {

				return System.Windows.Media.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);

			}
			
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
	}
}

#endif
