using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTyping
{
    class Differ
    {
        public class DiffData
        {
            public enum DiffState
            {
                Equal,
                Intermediate,
                Unequal,
            }

            public string Text { get; }
            public DiffState State { get; set; }

            public DiffData(string text, DiffState state)
            {
                Text = text;
                State = state;
            }
        }

        public delegate DiffData.DiffState Comparer(char ch1, char ch2);

        public IEnumerable<DiffData> Diff(string text1, string text2, Comparer comparer = null)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return new List<DiffData>();
            }

            if (comparer is null)
            {
                comparer += (ch1, ch2) => ch1 == ch2 ? DiffData.DiffState.Equal : DiffData.DiffState.Unequal;
            }

            int length = Math.Min(text1.Length, text2.Length);
            var result = new List<DiffData>();
            int i = 0;
            for (; i < length; i++)
            {
                result.Add(new DiffData(text1[i].ToString(), comparer(text1[i], text2[i])));
            }

            if (text1.Length == text2.Length) return result;

            result.Add(new DiffData((text1.Length < text2.Length ? text2 : text1).Substring(i),
                                    DiffData.DiffState.Unequal));
            return result;
        }
    }
}
