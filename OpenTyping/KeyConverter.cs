using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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

                switch (splitedValue.Length)
                {
                    case 1:
                        return new Key(splitedValue[0].Trim());
                    case 2:
                        return new Key(splitedValue[0].Trim(), splitedValue[1].Trim());
                    default:
                        throw new InvalidKeyDataException("잘못된 수의 키 데이터 문자열이 주어졌습니다.");
                }
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
