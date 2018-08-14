using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenTyping
{
    [TypeConverter(typeof(KeyConverter))]
    public sealed class KeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                string[] splitedValue = Regex.Split(s, @"(?<!\\)[ ]");
                splitedValue = splitedValue.Select(str => str.Replace(@"\ ", " ")).ToArray();

                if (splitedValue.Count() == 1)
                {
                    return new Key(splitedValue[0].Trim());
                }

                return new Key(splitedValue[0].Trim(), splitedValue[1].Trim());
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var keyValue = (Key)value;
                
                if (string.IsNullOrEmpty(keyValue.ShiftKeyData))
                {
                    return keyValue.KeyData.Replace(" ", @"\ ");
                }

                return keyValue.KeyData.Replace(" ", @"\ ") + " " + keyValue.ShiftKeyData.Replace(" ", @"\ ");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
