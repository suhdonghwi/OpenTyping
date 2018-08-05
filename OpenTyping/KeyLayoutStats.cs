using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTyping.Annotations;

namespace OpenTyping
{
    public class KeyLayoutStats : INotifyPropertyChanged
    {
        private IDictionary<KeyPos, int> keyIncorrectCount = new Dictionary<KeyPos, int>();

        private KeyValuePair<KeyPos, int> mostIncorrect = new KeyValuePair<KeyPos, int>();
        public KeyValuePair<KeyPos, int> MostIncorrect
        {
            get => mostIncorrect;
            set => SetField(ref mostIncorrect, value);
        }

        private static IDictionary<TK, TV> MergeBy<TK, TV>(IDictionary<TK, TV> lhs, IDictionary<TK, TV> rhs, Func<TV, TV, TV> mergeFunc)
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
            keyIncorrectCount = MergeBy(keyIncorrectCount, other.keyIncorrectCount, AddInt);
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
