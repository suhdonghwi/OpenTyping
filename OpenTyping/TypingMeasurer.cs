using System;
using System.Diagnostics;

namespace OpenTyping
{
    public class TypingMeasurer
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private static bool IsHangulSyllable(char ch)
            => ch >= (char)0xAC00 && ch <= (char)0xD79F;

        private static int CountLetter(string text)
        {
            double count = 0;

            foreach (char ch in text)
            {
                if (IsHangulSyllable(ch)) count += 2.5;
                else count++;
            }

            return Convert.ToInt32(count);
        }

        public void Start() => stopwatch.Restart();
        public double Finish(string text)
        {
            double elapsed = stopwatch.Elapsed.TotalMinutes;
            int count = CountLetter(text);

            stopwatch.Reset();
            return count / elapsed;
        }
    }
}
