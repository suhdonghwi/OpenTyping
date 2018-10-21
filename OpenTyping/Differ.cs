using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

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

        public static IEnumerable<char> DecomposeHangul(char ch)
        {
            var choseongTable = new List<string> { "ㄱ", "ㄱㄱ", "ㄴ", "ㄷ", "ㄷㄷ", "ㄹ", "ㅁ", "ㅂ", "ㅂㅂ", "ㅅ", "ㅅㅅ", "ㅇ", "ㅈ", "ㅈㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
            var jungseongTable = new List<string> { "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅗㅏ", "ㅗㅐ", "ㅗㅣ", "ㅛ", "ㅜ", "ㅜㅓ", "ㅜㅔ", "ㅜㅣ", "ㅠ", "ㅡ", "ㅡㅣ", "ㅣ" };
            var jongseongTable = new List<string> { " ", "ㄱ", "ㄱㄱ", "ㄱㅅ", "ㄴ", "ㄴㅈ", "ㄴㅎ", "ㄷ", "ㄹ", "ㄹㄱ", "ㄹㅁ", "ㄹㅂ", "ㄹㅅ", "ㄹㅌ", "ㄹㅍ", "ㄹㅎ", "ㅁ", "ㅂ", "ㅂㅅ", "ㅅ", "ㅅㅅ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
            var hangulCompatJamoTable = new List<string> { "ㄱ", "ㄱㄱ", "ㄱㅅ", "ㄴ", "ㄴㅈ", "ㄴㅎ", "ㄷ", "ㄷㄷ", "ㄹ", "ㄹㄱ", "ㄹㅁ", "ㄹㅂ", "ㄹㅅ", "ㄹㅌ", "ㄹㅍ", "ㄹㅎ", "ㅁ", "ㅂ", "ㅂㅂ", "ㅂㅅ", "ㅅ", "ㅅㅅ", "ㅇ", "ㅈ", "ㅈㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ", "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅗㅏ", "ㅗㅐ", "ㅗㅣ", "ㅛ", "ㅜ", "ㅜㅓ", "ㅜㅔ", "ㅜㅣ", "ㅠ", "ㅡ", "ㅡㅣ", "ㅣ" };

            if (ch >= (char)0x3131 && ch <= (char)0x3163) // ch가 Hangul Compatibility Jamo 유니코드 블럭에 있음
            {
                return hangulCompatJamoTable[ch - (char)0x3131];
            }
            if (ch < (char)0xAC00 || ch > (char)0xD79F) // ch가 Hangul Syllables 유니코드 블럭에 없음
            {
                return new List<char>();
            }

            int code = ch - (char)0xAC00;
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

        // text1, text2의 차이(Diff)를 구한다. 불일치하는 글자가 있을 경우 text1의 글자를 사용하여 불일치를 표현
        public IEnumerable<DiffData> Diff(string text1, string text2, string originalText1 /* text1의 잘리지 않은 원본, 도깨비불 확인용 */)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            {
                return new List<DiffData>();
            }

            int length = Math.Min(text1.Length, text2.Length), i = 0;
            var result = new List<DiffData>();
            DiffData.DiffState currentState = DiffData.DiffState.Unequal;
            string tempString = "";
            for (; i < length; i++)
            {
                char ch1 = text1[i], ch2 = text2[i];

                DiffData.DiffState state = ch1 == ch2 ? DiffData.DiffState.Equal : DiffData.DiffState.Unequal;

                if (i == length - 1)
                {
                    var decomposed1 = new List<char>(DecomposeHangul(ch1));
                    var decomposed2 = new List<char>(DecomposeHangul(ch2));

                    if (decomposed1.Any() && decomposed2.Any()) // ch1, ch2 둘 다 한글
                    {
                        if (decomposed1.SequenceEqual(decomposed2))
                        {
                            state = DiffData.DiffState.Equal;
                        }
                        else if (decomposed1.Count < decomposed2.Count &&
                                 i < originalText1.Length - 1 && // 다음 글자가 존재함
                                 decomposed1.SequenceEqual(decomposed2.Take(decomposed1.Count)) && // 부분 일치
                                 decomposed2.Count >= 3) // decomposed2 는 종성이 있어야 함
                        { // 도깨비불 현상 가능성
                            var nextDecomposed = new List<char>(DecomposeHangul(originalText1[i + 1]));
                            state = nextDecomposed.Any() && (decomposed2.Last() == nextDecomposed.First())
                                ? DiffData.DiffState.Equal
                                : DiffData.DiffState.Unequal;
                        }
                        else
                        {
                            state = decomposed2.SequenceEqual(decomposed1.Take(decomposed2.Count))
                                ? DiffData.DiffState.Intermediate
                                : DiffData.DiffState.Unequal;
                        }
                    }
                }

                if (i == 0) currentState = state;

                if (state == currentState)
                {
                    tempString += ch1;
                }
                else
                {
                    result.Add(new DiffData(tempString, currentState));
                    currentState = state;
                    tempString = ch1.ToString();
                }
            }

            if (tempString != "")
            {
                result.Add(new DiffData(tempString, currentState));
            }

            if (text1.Length == text2.Length) return result;

            result.Add(new DiffData((text1.Length < text2.Length ? text2 : text1).Substring(i),
                                    DiffData.DiffState.Unequal));
            return result;
        }

        private readonly Brush correctBackground = Brushes.LightGreen;
        private readonly Brush incorrectBackground = Brushes.Pink;
        private readonly Brush intermidiateBackground = new SolidColorBrush(Color.FromRgb(215, 244, 215));

        public Brush MapDiffState(DiffData.DiffState state) // Diff 상태를 그에 대응하는 색으로 변환
        {
            switch (state)
            {
                case DiffData.DiffState.Equal:
                    return correctBackground;
                case DiffData.DiffState.Unequal:
                    return incorrectBackground;
                case DiffData.DiffState.Intermediate:
                    return intermidiateBackground;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static double CalculateAccuracy(IList<DiffData> diffs)
        {
            double accuracy 
                = diffs.Sum(data => data.State == DiffData.DiffState.Equal ? data.Text.Length : 0) / // 입력 중 맞는 입력의 총 길이
                  (double)diffs.Sum(data => data.Text.Length); // 총 입력 길이

            return accuracy;
        }
    }
}
