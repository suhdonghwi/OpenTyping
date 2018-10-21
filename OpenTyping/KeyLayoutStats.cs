using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenTyping
{
    public class KeyLayoutStats : INotifyPropertyChanged
    {
        public Dictionary<KeyPos, int> KeyIncorrectCount { get; set; } = new Dictionary<KeyPos, int>();

        private KeyValuePair<KeyPos, int> mostIncorrect;
        public KeyValuePair<KeyPos, int> MostIncorrect
        {
            get => mostIncorrect;
            set => SetField(ref mostIncorrect, value);
        }

        private int sentencePracticeCount;
        public int SentencePracticeCount
        {
            get => sentencePracticeCount;
            set => SetField(ref sentencePracticeCount, value);
        }

        private int averageTypingSpeed;
        public int AverageTypingSpeed
        {
            get => averageTypingSpeed;
            set => SetField(ref averageTypingSpeed, value);
        }

        private int averageAccuracy;

        public int AverageAccuracy
        {
            get => averageAccuracy;
            set => SetField(ref averageAccuracy, value);
        }

        // lhs와 rhs를 합친다. 중복된 key가 있을 경우 두 value를 mergeFunc에 넣어 나온 값을 value로 이용한다.
        private static Dictionary<TK, TV> MergeBy<TK, TV>(IReadOnlyDictionary<TK, TV> lhs, IReadOnlyDictionary<TK, TV> rhs, Func<TV, TV, TV> mergeFunc)
        { 
            Dictionary<TK, TV> result = lhs.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (KeyValuePair<TK, TV> kv in rhs)
            {
                result[kv.Key] = result.ContainsKey(kv.Key) ? mergeFunc(lhs[kv.Key], rhs[kv.Key]) : kv.Value;
            }

            return result;
        }

        public void AddStats(KeyLayoutStats other)
        {
            if (other.KeyIncorrectCount != null)
            {
                int AddInt(int lhs, int rhs) => lhs + rhs;

                KeyIncorrectCount = MergeBy(KeyIncorrectCount, other.KeyIncorrectCount, AddInt);
                MostIncorrect = KeyIncorrectCount.FirstOrDefault(x => x.Value == KeyIncorrectCount.Values.Max());
            }

            if (other.SentencePracticeCount > 0)
            {
                int newSpeedSum = (AverageTypingSpeed * SentencePracticeCount) +
                             (other.AverageTypingSpeed * other.SentencePracticeCount);
                int newAccuracySum = (AverageAccuracy * SentencePracticeCount) +
                                     (other.AverageAccuracy * other.SentencePracticeCount);

                SentencePracticeCount = SentencePracticeCount + other.SentencePracticeCount;

                AverageTypingSpeed = newSpeedSum / SentencePracticeCount;
                AverageAccuracy = newAccuracySum / SentencePracticeCount;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
