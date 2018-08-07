using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTyping.Annotations;

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

        private static Dictionary<TK, TV> MergeBy<TK, TV>(Dictionary<TK, TV> lhs, Dictionary<TK, TV> rhs, Func<TV, TV, TV> mergeFunc)
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
            KeyIncorrectCount = MergeBy(KeyIncorrectCount, other.KeyIncorrectCount, AddInt);

            MostIncorrect = KeyIncorrectCount.FirstOrDefault(x => x.Value == KeyIncorrectCount.Values.Max());
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
