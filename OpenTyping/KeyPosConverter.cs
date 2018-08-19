using System;
using System.ComponentModel;
using System.Globalization;

namespace OpenTyping
{
    [TypeConverter(typeof(KeyPosConverter))]
    public sealed class KeyPosConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                string[] splitedValue = s.Split(',');

                return new KeyPos(Int32.Parse(splitedValue[0]), Int32.Parse(splitedValue[1]));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var keyValue = (KeyPos)value;
                return keyValue.Row + "," + keyValue.Column;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
