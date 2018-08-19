using System;
using System.Collections.Generic;
using System.ComponentModel;
using Input = System.Windows.Input;

namespace OpenTyping
{
    [TypeConverter(typeof(KeyPosConverter))]
    public sealed class KeyPos : IEquatable<KeyPos>
    {
        public int Row { get; }
        public int Column { get; }

        public KeyPos(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Equals(KeyPos other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (Row == other.Row) && (Column == other.Column);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((KeyPos)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Row.GetHashCode();
                hashCode = (hashCode * 397) ^ Column.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator==(KeyPos lhs, KeyPos rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            if (lhs is null)
            {
                return false;
            }
            if (rhs is null)
            {
                return false;
            }

            return (lhs.Row == rhs.Row) && (lhs.Column == rhs.Column);
        }

        public static bool operator!=(KeyPos lhs, KeyPos rhs)
        {
            return !(lhs == rhs);
        }

        public static KeyPos FromKeyCode(Input.Key key)
        {
            var keyPosData = new List<List<Input.Key>>
            {
                new List<Input.Key> { Input.Key.Oem3, Input.Key.D1, Input.Key.D2, Input.Key.D3, Input.Key.D4, Input.Key.D5, Input.Key.D6, Input.Key.D7, Input.Key.D8, Input.Key.D9, Input.Key.D0, Input.Key.OemMinus, Input.Key.OemPlus },
                new List<Input.Key> { Input.Key.Q, Input.Key.W, Input.Key.E, Input.Key.R, Input.Key.T, Input.Key.Y, Input.Key.U, Input.Key.I, Input.Key.O, Input.Key.P, Input.Key.OemOpenBrackets, Input.Key.OemCloseBrackets, Input.Key.Oem5 },
                new List<Input.Key> { Input.Key.A, Input.Key.S, Input.Key.D, Input.Key.F, Input.Key.G, Input.Key.H, Input.Key.J, Input.Key.K, Input.Key.L, Input.Key.OemSemicolon, Input.Key.OemQuotes},
                new List<Input.Key> { Input.Key.Z, Input.Key.X, Input.Key.C, Input.Key.V, Input.Key.B, Input.Key.N, Input.Key.M, Input.Key.OemComma, Input.Key.OemPeriod, Input.Key.OemQuestion},
            };

            for (int i=0; i<keyPosData.Count; i++)
            {
                for (int j=0; j<keyPosData[i].Count; j++)
                {
                    if (keyPosData[i][j] == key)
                    {
                        return new KeyPos(i, j);
                    }
                }
            }

            return null;
        }
    }
}
