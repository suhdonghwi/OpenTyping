using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenTyping
{
    public class KeyLayoutStats : INotifyPropertyChanged
    {
        public Dictionary<KeyPos, int> KeyIncorrectCount { get; set; }

        private KeyValuePair<KeyPos, int> mostIncorrect;
        public KeyValuePair<KeyPos, int> MostIncorrect
        {
            get => mostIncorrect;
            set => SetField(ref mostIncorrect, value);
        }

        public int SentencePracticeCount { get; set; } = 0;

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

        private static Dictionary<TK, TV> MergeBy<TK, TV>(IReadOnlyDictionary<TK, TV> lhs, IReadOnlyDictionary<TK, TV> rhs, Func<TV, TV, TV> mergeFunc)
        {
            var result = new Dictionary<TK, TV>();

            foreach (KeyValuePair<TK, TV> kvPair in lhs)
            {
                result[kvPair.Key] 
                    = rhs.ContainsKey(kvPair.Key) ? mergeFunc(lhs[kvPair.Key], rhs[kvPair.Key]) : kvPair.Value;
            }

            foreach (KeyValuePair<TK, TV> kvPair in rhs)
            {
                result[kvPair.Key] = kvPair.Value;
            }

            return result;
        }

        public void AddStats(KeyLayoutStats other)
        {
            int AddInt(int lhs, int rhs) => lhs + rhs;

            if (other.KeyIncorrectCount != null)
            {
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
