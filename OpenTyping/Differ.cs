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
                Unequal,
            }

            public string Text { get; }
            public DiffState State { get; }

            public DiffData(string text, DiffState state)
            {
                Text = text;
                State = state;
            }
        }

        public IEnumerable<DiffData> Diff(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return new List<DiffData>();
            }

            int length = Math.Min(text1.Length, text2.Length);
            string temp = "";
            bool equalState = text1[0] == text2[0];
            var result = new List<DiffData>();
            int i = 0;
            for (; i < length; i++)
            {
                if (text1[i] == text2[i] == equalState)
                {
                    temp += text1[i];
                }
                else
                {
                    result.Add(new DiffData(temp, equalState ? DiffData.DiffState.Equal : DiffData.DiffState.Unequal));
                    temp = text1[i].ToString();
                    equalState = !equalState;
                }
            }

            result.Add(new DiffData(temp, equalState ? DiffData.DiffState.Equal : DiffData.DiffState.Unequal));
            if (text1.Length == text2.Length) return result;

            result.Add(new DiffData((text1.Length < text2.Length ? text2 : text1).Substring(i),
                                    DiffData.DiffState.Unequal));
            return result;
        }
    }
}
