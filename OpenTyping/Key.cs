using System.ComponentModel;

namespace OpenTyping
{
    [TypeConverter(typeof(KeyConverter))]
    public class Key
    {
        public string KeyData { get; }
        public string ShiftKeyData { get; }

        public Key(string keyData, string shiftKeyData = null)
        {
            KeyData = keyData;
            ShiftKeyData = shiftKeyData;
        }
    }
}