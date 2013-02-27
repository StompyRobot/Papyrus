/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Papyrus.Converters
{
	public class ColorTypeConverter : TypeConverter
	{

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{

			if (sourceType == typeof(string))
				return true;
			
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{

			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{

			if (!(value is string))
			{
				return base.ConvertFrom(context, culture, value);
			}

			var stringValue = (string) value;
			var values = stringValue.Split(',').ToList().ConvertAll(p => p.Trim());

			if (values.Count != 3 && values.Count != 4)
				throw new FormatException("Invalid number of params");

			var newColor = new DataTypes.Color();
			newColor.R = byte.Parse(values[0]);
			newColor.G = byte.Parse(values[1]);
			newColor.B = byte.Parse(values[2]);

			if (values.Count == 4)
				newColor.A = byte.Parse(values[3]);
			else
				newColor.A = 255;

			return newColor;

		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{

			var color = value as DataTypes.Color;

			if(color != null && destinationType == typeof(string)) {

				return string.Format("{0}, {1}, {2}, {3}", color.R, color.G, color.B, color.A);

			}

			return base.ConvertTo(context, culture, value, destinationType);

		}
		
	}
}
