using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

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

        private readonly IList<KeyPos> keyList;

        private KeyInfo previousKey;
        public KeyInfo PreviousKey
        {
            get => previousKey;
            private set
            {
                previousKey = value;
                OnPropertyChanged("PreviousKey");
            }
        }

        private KeyInfo currentKey;
        public KeyInfo CurrentKey
        {
            get => currentKey;
            private set
            {
                currentKey = value;
                OnPropertyChanged("CurrentKey");
            }
        }

        private KeyInfo nextKey;
        public KeyInfo NextKey
        {
            get => nextKey;
            private set
            {
                nextKey = value;
                OnPropertyChanged("NextKey");
            }
        }

        private static readonly Random Randomizer = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public KeyPracticeWindow(IList<KeyPos> keyList)
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

                    if (InputMethod.Current != null)
                    {
                        InputMethod.Current.ImeState = InputMethodState.On;
                    }
                }
            }
        }

        private KeyInfo RandomKey()
        {
            KeyPos keyPos = keyList[Randomizer.Next(0, keyList.Count)];
            Key key = MainWindow.CurrentKeyLayout[keyPos];

            if (string.IsNullOrEmpty(key.ShiftKeyData))
            {
                return new KeyInfo(key.KeyData, keyPos, false);
            }

            bool isShift = Randomizer.Next(0, 2) == 0;
            return new KeyInfo(isShift ? key.ShiftKeyData : key.KeyData, keyPos, isShift);
        }

        private void MoveKey()
        {
            PreviousKey = CurrentKey;
            CurrentKey = NextKey;
            NextKey = RandomKey();
        }

        private void KeyPracticeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            KeyPos pos = KeyPos.FromKeyCode(e.Key);

            if (e.Key == System.Windows.Input.Key.LeftShift || 
                e.Key == System.Windows.Input.Key.RightShift || 
                pos == null) return;

            bool isShift = Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
            
            if (CurrentKey.KeyPos == pos && CurrentKey.IsShift == isShift)
            {
                Debug.Print("Correct!");
                MoveKey();
            }
            else
            {
                Debug.Print("Wrong!");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
