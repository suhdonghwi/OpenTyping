using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTyping
{
    public sealed class KeyPos : IEquatable<KeyPos>
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public KeyPos(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Equals(KeyPos other)
        {
            return (Row == other.Row) && (Column == other.Column);
        }
    }
}
