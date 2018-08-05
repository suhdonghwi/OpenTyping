using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OpenTyping
{
    /// <summary>
    /// KeyPracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        public class KeyInfo
        {
            public KeyInfo(string keyData, KeyPos pos, bool isShift)
            {
                KeyData = keyData;
                Pos = pos;
                IsShift = isShift;
            }

            public string KeyData { get; set; }
            public KeyPos Pos { get; set; }
            public bool IsShift { get; set; }
        }

        private readonly IList<KeyPos> keyList;

        private KeyInfo previousKey;
        public KeyInfo PreviousKey
        {
            get => previousKey;
            private set => SetField(ref previousKey, value);
        }

        private KeyInfo currentKey;
        public KeyInfo CurrentKey
        {
            get => currentKey;
            private set => SetField(ref currentKey, value);
        }

        private KeyInfo nextKey;
        public KeyInfo NextKey
        {
            get => nextKey;
            private set => SetField(ref nextKey, value);
        }

        private int correctCount = 0;
        public int CorrectCount
        {
            get => correctCount;
            private set => SetField(ref correctCount, value);
        }

        private int incorrectCount = 0;
        public int IncorrectCount
        {
            get => incorrectCount;
            private set => SetField(ref incorrectCount, value);
        }

        private static readonly Random Randomizer = new Random();
        private static readonly ThicknessAnimationUsingKeyFrames ShakeAnimation = new ThicknessAnimationUsingKeyFrames();


        public KeyPracticeWindow(IList<KeyPos> keyList)
        {
            InitializeComponent();

            this.DataContext = this;
            this.keyList = keyList;

            CurrentKey = RandomKey();
            NextKey = RandomKey();

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => KeyLayoutBox.PressKey(CurrentKey.Pos)));
            this.KeyDown += KeyPracticeWindow_KeyDown;

            double shakiness = 30;
            const double shakeDiff = 3;
            var keyFrames = new ThicknessKeyFrameCollection();

            for(int timeSpan = 5; shakiness > 0;)
            {
                keyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 10, 0, 0))
                {
                    KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, timeSpan))
                });
                timeSpan += 5;

                keyFrames.Add(new EasingThicknessKeyFrame(new Thickness(shakiness, 10, 0, 0))
                {
                    KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, timeSpan))
                });
                timeSpan += 5;

                keyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 10, 0, 0))
                {
                    KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, timeSpan))
                });
                timeSpan += 5;

                keyFrames.Add(new EasingThicknessKeyFrame(new Thickness(-shakiness, 10, 0, 0))
                {
                    KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, timeSpan))
                });
                timeSpan += 5;

                keyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 10, 0, 0))
                {
                    KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, timeSpan))
                });
                timeSpan += 5;

                shakiness -= shakeDiff;
            }

            ShakeAnimation.KeyFrames = keyFrames;

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
            KeyLayoutBox.ReleaseKey(PreviousKey.Pos);

            CurrentKey = NextKey;
            KeyLayoutBox.PressKey(CurrentKey.Pos);

            NextKey = RandomKey();
        }

        private void KeyPracticeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            KeyPos pos = KeyPos.FromKeyCode(e.Key);

            if (e.Key == System.Windows.Input.Key.LeftShift || 
                e.Key == System.Windows.Input.Key.RightShift || 
                pos == null) return;

            bool isShift = Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
            
            if (CurrentKey.Pos == pos && CurrentKey.IsShift == isShift)
            {
                CorrectCount++;
                MoveKey();
            }
            else
            {
                IncorrectCount++;

                KeyGrid.BeginAnimation(MarginProperty, ShakeAnimation);
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
