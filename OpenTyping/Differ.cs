using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTyping
{
    internal class Differ
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

        private static IEnumerable<char> DecomposeHangul(char hangul)
        {
            var choseongTable = new List<string>
            {
                "ㄱ",
                "ㄱㄱ",
                "ㄴ",
                "ㄷ",
                "ㄷㄷ",
                "ㄹ",
                "ㅁ",
                "ㅂ",
                "ㅂㅂ",
                "ㅅ",
                "ㅅㅅ",
                "ㅇ",
                "ㅈ",
                "ㅈㅈ",
                "ㅊ",
                "ㅋ",
                "ㅌ",
                "ㅍ",
                "ㅎ",
            };
            var jungseongTable = new List<string>
            {
                "ㅏ",
                "ㅐ",
                "ㅑ",
                "ㅒ",
                "ㅓ",
                "ㅔ",
                "ㅕ",
                "ㅖ",
                "ㅗ",
                "ㅗㅏ",
                "ㅗㅐ",
                "ㅗㅣ",
                "ㅛ",
                "ㅜ",
                "ㅜㅓ",
                "ㅜㅔ",
                "ㅜㅣ",
                "ㅠ",
                "ㅡ",
                "ㅡㅣ",
                "ㅣ"
            };
            var jongseongTable = new List<string>
            {
                " ",
                "ㄱ",
                "ㄱㄱ",
                "ㄱㅅ",
                "ㄴ",
                "ㄴㅈ",
                "ㄴㅎ",
                "ㄷ",
                "ㄹ",
                "ㄹㄱ",
                "ㄹㅁ",
                "ㄹㅂ",
                "ㄹㅅ",
                "ㄹㅌ",
                "ㄹㅍ",
                "ㄹㅎ",
                "ㅁ",
                "ㅂ",
                "ㅂㅅ",
                "ㅅ",
                "ㅅㅅ",
                "ㅇ",
                "ㅈ",
                "ㅊ",
                "ㅋ",
                "ㅌ",
                "ㅍ",
                "ㅎ"
            };

            if (hangul >= (char)0x3131 && hangul <= (char)0x3163) // The character is in Hangul Compatibility Jamo unicode block
            {
                return new List<char> { hangul };
            }
            if (hangul < (char)0xAC00 || hangul > (char)0xD79F) // The character is not in Hangul Syllables unicode block
            {
                return new List<char>();
            }

            int code = hangul - (char)0xAC00;
            var result = new List<char>();

            int choseongIndex = code / (21 * 28);
            result.AddRange(choseongTable[choseongIndex]);
            code %= 21 * 28;

            int jungseongIndex = code / 28;
            result.AddRange(jungseongTable[jungseongIndex]);
            code %= 28;

            int jongseongIndex = code;
            if (jongseongIndex != 0) result.AddRange(jongseongTable[jongseongIndex]);

            return result;
        }

        public IEnumerable<DiffData> Diff(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return new List<DiffData>();
            }

            int length = Math.Min(text1.Length, text2.Length);
            var result = new List<DiffData>();
            int i = 0;
            for (; i < length; i++)
            {
                char ch1 = text1[i], ch2 = text2[i];
                var decomposed1 = new List<char>(DecomposeHangul(ch1));
                var decomposed2 = new List<char>(DecomposeHangul(ch2));

                DiffData.DiffState state;

                if (decomposed1.Any() && decomposed2.Any())
                {
                    if (decomposed1.SequenceEqual(decomposed2))
                    {
                        state = DiffData.DiffState.Equal;
                    }
                    else if (decomposed1.Count < decomposed2.Count)
                    {
                        state = decomposed1.SequenceEqual(decomposed2.Take(decomposed1.Count))
                            ? DiffData.DiffState.Intermediate
                            : DiffData.DiffState.Unequal;
                    }
                    else
                    {
                        state = decomposed2.SequenceEqual(decomposed1.Take(decomposed2.Count))
                            ? DiffData.DiffState.Intermediate
                            : DiffData.DiffState.Unequal;
                    }
                }
                else
                {
                    state = ch1 == ch2 ? DiffData.DiffState.Equal : DiffData.DiffState.Unequal;
                }

                result.Add(new DiffData(text1[i].ToString(), state));
            }

            if (text1.Length == text2.Length) return result;

            result.Add(new DiffData((text1.Length < text2.Length ? text2 : text1).Substring(i),
                                    DiffData.DiffState.Unequal));
            return result;
        }
    }
}
