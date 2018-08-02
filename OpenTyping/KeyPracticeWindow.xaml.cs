using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenTyping
{
    /// <summary>
    /// KeyPracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<KeyPos> keyList;

        private string previousKey;
        public string PreviousKey
        {
            get { return previousKey; }
            private set
            {
                previousKey = value;
                OnPropertyChanged("PreviousKey");
            }
        }

        private string currentKey;
        public string CurrentKey
        {
            get { return currentKey; }
            private set
            {
                currentKey = value;
                OnPropertyChanged("CurrentKey");
            }
        }
        private KeyPos currentPos;


        private string nextKey;
        public string NextKey
        {
            get { return nextKey; }
            private set
            {
                nextKey = value;
                OnPropertyChanged("NextKey");
            }
        }
        private KeyPos nextPos;

        private static readonly Random randomizer = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public KeyPracticeWindow(List<KeyPos> keyList)
        {
            InitializeComponent();

            this.DataContext = this;
            this.keyList = keyList;

            var currentKey = RandomKey();
            CurrentKey = currentKey.Item1;
            currentPos = currentKey.Item2;

            var nextKey = RandomKey();
            NextKey = nextKey.Item1;
            nextPos = nextKey.Item2;

            this.KeyDown += KeyPracticeWindow_KeyDown;
        }

        private Tuple<string, KeyPos> RandomKey()
        {
            KeyPos keyPos = keyList[randomizer.Next(0, keyList.Count)];
            Key key = MainWindow.CurrentKeyLayout[keyPos];

            return Tuple.Create(randomizer.Next(0, 1) == 0 ? key.KeyData : key.ShiftKeyData, keyPos); 
        }

        private void MoveKey()
        {
            PreviousKey = CurrentKey;

            CurrentKey = NextKey;
            currentPos = nextPos;

            var nextKey = RandomKey();
            NextKey = nextKey.Item1;
            nextPos = nextKey.Item2;
        }

        private void KeyPracticeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MoveKey();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
