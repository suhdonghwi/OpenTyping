using MahApps.Metro.Controls;
using OpenTyping.Properties;
using OpenTyping.Resources.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OpenTyping
{
    /// <summary>
    /// WordPracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WordPracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        // Key pressing
        private KeyPos currKeyPos;
        private static readonly ThicknessAnimationUsingKeyFrames ShakeAnimation = new ThicknessAnimationUsingKeyFrames();

        // Practice data
        private PracticeData practiceData;
        private int? currentSentenceIndex;

        private string currentText;
        public string CurrentText
        {
            get => currentText;
            set => SetField(ref currentText, value);
        }

        private string nextText;
        public string NextText
        {
            get => nextText;
            set => SetField(ref nextText, value);
        }

        // Maeasurement
        private static readonly Differ Differ = new Differ();

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

        private readonly TypingMeasurer typingMeasurer = new TypingMeasurer();

        private int typingSpeed;

        private int averageTypingSpeed;
        public int AverageTypingSpeed
        {
            get => averageTypingSpeed;
            private set => SetField(ref averageTypingSpeed, value);
        }

        private int typingAccuracy;

        private int averageAccuracy;
        public int AverageAccuracy
        {
            get => averageAccuracy;
            private set => SetField(ref averageAccuracy, value);
        }

        public List<int> TypingSpeedList { get; set; } = new List<int>();
        public List<int> AccuracyList { get; set; } = new List<int>();

        // Sound
        private readonly MediaPlayer playMedia = new MediaPlayer();
        private readonly Uri wrongPressedUri = new Uri("pack://siteoforigin:,,,/Resources/Sounds/WrongPressed.mp3");

        private readonly SoundPlayer playPressedSound = new SoundPlayer(Properties.Resources.Pressed);
        private readonly SoundPlayer playBellSound = new SoundPlayer(Properties.Resources.Bell);

        // Timer
        private System.Windows.Threading.DispatcherTimer timer;
        private int elapsedTime;
        private int second = 0;
        private int minute = 0;
        private int hour = 0;
        private int milisecond = 0;

        // Event for returning UserRecord value
        public event Action<User> RtnNewUser;

        public WordPracticeWindow()
        {
            InitializeComponent();
            this.SetTextBylanguage();
            CurrentTextBox.Focus();

            this.ShuffleWords();
            this.Loaded += WordPracticeWindow_Loaded;

            double shakiness = 30;
            const double shakeDiff = 3;
            var keyFrames = new ThicknessKeyFrameCollection();

            for (int timeSpan = 5; shakiness > 0;)
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
        }

        private void SetTextBylanguage()
        {
            SelfWindow.Title = LangStr.AppName;
            Speed.Text = LangStr.Speed;
            Accuracy.Text = LangStr.Accuracy;
        }

        private void ShuffleWords()
        {
            practiceData = new PracticeMenuBase().loadWordData();
            var wordIndexRandom = new Random();
            
            // UI code for progress bar
            ProgressBar.Maximum = practiceData.TextData.Count;
            MaxCnt.Text = practiceData.TextData.Count.ToString();

            practiceData.RemoveDuplicates();
            practiceData.TextData = practiceData.TextData.OrderBy(s => wordIndexRandom.Next()).ToList();
        }

        private void WordPracticeWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            NextWord();
        }

        private void WordPracticeWindow_Closed(object sender, EventArgs e)
        {
            if (TypingSpeedList.Count > 0)
            {
                MainWindow.CurrentKeyLayout.Stats.AddStats(new KeyLayoutStats()
                {
                    AverageTypingSpeed = Convert.ToInt32(TypingSpeedList.Average()),
                    AverageAccuracy = Convert.ToInt32(AccuracyList.Average())
                });
            }

            if ((string)Settings.Default["Name"] == "")
            {
                Settings.Default["Name"] = LangStr.Anonymous;
            }
            if ((string)Settings.Default["Org"] == "")
            {
                Settings.Default["Org"] = LangStr.Anonymous;
            }

            User user = new User(
                (string)Settings.Default["Name"],
                (string)Settings.Default["Org"],
                averageAccuracy,
                averageTypingSpeed,
                (int)currentSentenceIndex,
                (double)elapsedTime / (double)10 // Unit change: 100ms -> 1s
            );
            this.RtnNewUser(user);
        }

        private void NextWord()
        {
            bool isWrongWord = false;

            if (!string.IsNullOrEmpty(CurrentTextBox.Text)) // Diff 구하고 하이라이트, 타속, 정확도 계산 : 첫 호출인 경우 수행하지 않음
            {
                PreviousTextBlock.Inlines.Clear();
                var diffs = new List<Differ.DiffData>(
                    Differ.Diff(CurrentTextBox.Text, CurrentText, CurrentTextBox.Text));

                for (int i = 0; i < diffs.Count(); i++)
                {
                    if (diffs[i].State == Differ.DiffData.DiffState.Intermediate)
                    {
                        diffs[i].State = Differ.DiffData.DiffState.Unequal;
                    }
                }

                foreach (var diff in diffs)
                {
                    var run = new Run(diff.Text)
                    {
                        Background = Differ.MapDiffState(diff.State)
                    };
                    PreviousTextBlock.Inlines.Add(run);

                    if (run.Background == Differ.incorrectBackground)
                    {
                        isWrongWord = true;
                    }
                }

                if (isWrongWord)
                {
                    KeyGrid.BeginAnimation(MarginProperty, ShakeAnimation);
                    playMedia.Open(wrongPressedUri);
                    playMedia.Play();
                    IncorrectCount++;
                }
                else
                {
                    playBellSound.Play();
                    CorrectCount++;
                }
                isWrongWord = false;

                double accuracy = Differ.CalculateAccuracy(diffs);
                typingAccuracy = Convert.ToInt32(accuracy * 100);
                AccuracyList.Add(typingAccuracy);
                AverageAccuracy = Convert.ToInt32(AccuracyList.Average());

                typingSpeed = Convert.ToInt32(typingMeasurer.Finish(CurrentTextBox.Text) * accuracy);
                TypingSpeedList.Add(typingSpeed);
                AverageTypingSpeed = Convert.ToInt32(TypingSpeedList.Average());
            }

            if (!string.IsNullOrEmpty(CurrentTextBox.Text) || currentSentenceIndex is null) // 입력이 비어있지 않거나 첫 번째 호출인 경우
            {
                if (currentSentenceIndex is null) currentSentenceIndex = 0; // 첫 호출인 경우
                else currentSentenceIndex++;

                if (currentSentenceIndex == practiceData.TextData.Count)
                {
                    // UI code for progress bar
                    ProgressBar.Value = (int)currentSentenceIndex;
                    CurrCnt.Text = currentSentenceIndex.ToString();

                    timer.Stop();
                    this.Close();
                    return;
                }

                CurrentText = practiceData.TextData[currentSentenceIndex.Value];
                NextText = "";
                if (currentSentenceIndex != practiceData.TextData.Count - 1)
                {
                    NextText = practiceData.TextData[currentSentenceIndex.Value + 1];
                }

                // UI code for progress bar
                ProgressBar.Value = (int)currentSentenceIndex;
                CurrCnt.Text = currentSentenceIndex.ToString();
            }
        }

        private async void CurrentTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            currKeyPos = KeyPos.FromKeyCode(e.Key == System.Windows.Input.Key.ImeProcessed ? e.ImeProcessedKey : e.Key);

            if (e.Key == System.Windows.Input.Key.LeftShift ||
                e.Key == System.Windows.Input.Key.RightShift ||
                (e.Key != System.Windows.Input.Key.Enter && currKeyPos == null))
                    return;

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                NextWord();
                CurrentTextBox.Text = "";
                e.Handled = true;

                return;
            } 
            else
            {
                if (CurrentTextBox.Text == "")
                {
                    typingMeasurer.Start();
                    StartStopWatch();
                }

                KeyLayoutBox.PressCorrectKey(currKeyPos);
                playPressedSound.Play();
                await Task.Delay(20);
                KeyLayoutBox.ReleaseKey(currKeyPos);
            }
        }

        private void CurrentTextBox_PreviewExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void StartStopWatch()
        {
            if (timer != null) return;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100ms
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            elapsedTime++;
            milisecond = elapsedTime % 10;

            TBstopWatch.Text = string.Format("{0:D2}", hour) + ":" + string.Format("{0:D2}", minute)
                                + ":" + string.Format("{0:D2}", second) + "." + string.Format("{0:D2}", milisecond);

            if (milisecond == 0)
            {
                second++;
                milisecond = 0;
            }
            if (second == 60)
            {
                minute++;
                second = 0;
            }
            if (minute == 60)
            {
                hour++;
                minute = 0;
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
