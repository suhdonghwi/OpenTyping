using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public class KeyInfo
        {
            public KeyInfo(string keyData, KeyPos keyPos, bool isShift)
            {
                KeyData = keyData;
                KeyPos = keyPos;
                IsShift = isShift;
            }

            public string KeyData { get; set; }
            public KeyPos KeyPos { get; set; }
            public bool IsShift { get; set; }
        }

        private List<KeyPos> keyList;

        private KeyInfo previousKey;
        public KeyInfo PreviousKey
        {
            get { return previousKey; }
            private set
            {
                previousKey = value;
                OnPropertyChanged("PreviousKey");
            }
        }

        private KeyInfo currentKey;
        public KeyInfo CurrentKey
        {
            get { return currentKey; }
            private set
            {
                currentKey = value;
                OnPropertyChanged("CurrentKey");
            }
        }

        private KeyInfo nextKey;
        public KeyInfo NextKey
        {
            get { return nextKey; }
            private set
            {
                nextKey = value;
                OnPropertyChanged("NextKey");
            }
        }

        private static readonly Random randomizer = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public KeyPracticeWindow(List<KeyPos> keyList)
        {
            InitializeComponent();

            this.DataContext = this;
            this.keyList = keyList;

            CurrentKey = RandomKey();
            NextKey = RandomKey();

            this.KeyDown += KeyPracticeWindow_KeyDown;

            foreach (System.Windows.Forms.InputLanguage lang in System.Windows.Forms.InputLanguage.InstalledInputLanguages)
            {
                if (lang.LayoutName == "English")
                {
                    System.Windows.Forms.InputLanguage.CurrentInputLanguage = lang;
                    InputMethod.Current.ImeState = InputMethodState.On;
                }
            }
        }

        private KeyInfo RandomKey()
        {
            KeyPos keyPos = keyList[randomizer.Next(0, keyList.Count)];
            Key key = MainWindow.CurrentKeyLayout[keyPos];
            bool isShift = randomizer.Next(0, 1) == 0;

            return new KeyInfo(isShift ? key.KeyData : key.ShiftKeyData, keyPos, isShift);
        }

        private void MoveKey()
        {
            PreviousKey = CurrentKey;
            CurrentKey = NextKey;
            NextKey = RandomKey();
        }

        private void KeyPracticeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.LeftShift || e.Key == System.Windows.Input.Key.RightShift)
            {
                return;
            }

            MoveKey();

            if (Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift))
            {
                Debug.Print(e.Key.ToString() + " with Shift");
            }
            else
            {
                Debug.Print(e.Key.ToString());
            }   
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
