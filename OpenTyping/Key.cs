using System.ComponentModel;

namespace OpenTyping
{
    [TypeConverter(typeof(KeyConverter))]
    public class Key
    {
        public Key(string keyData, string shiftKeyData)
        {
            KeyData = keyData;
            ShiftKeyData = shiftKeyData;
        }

        public string KeyData { get; }
        public string ShiftKeyData { get; }
    }
}