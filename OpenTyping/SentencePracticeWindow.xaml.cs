using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        private string currentText;
        public string CurrentText
        {
            get => currentText;
            set => SetField(ref currentText, value);
        }

        private PracticeData practiceData;
        private readonly bool shuffle;

        private readonly TypingMeasurer typingMeasurer = new TypingMeasurer();

        private int typingSpeed;
        public int TypingSpeed
        {
            get => typingSpeed;
            private set => SetField(ref typingSpeed, value);
        }

        private int averageTypingSpeed;
        public int AverageTypingSpeed
        {
            get => averageTypingSpeed;
            private set => SetField(ref averageTypingSpeed, value);
        }

        private int typingAccuracy;
        public int TypingAccuracy
        {
            get => typingAccuracy;
            private set => SetField(ref typingAccuracy, value);
        }

        public ChartValues<int> TypingSpeedList { get; set; } = new ChartValues<int>();
        public ChartValues<int> AccuracyList { get; set; } = new ChartValues<int>();

        private int? currentSentenceIndex;
        private static readonly Random SentenceIndexRandom = new Random();

        private static readonly Differ Differ = new Differ();

        public SentencePracticeWindow(PracticeData practiceData, bool shuffle)
        {
            InitializeComponent();

            this.practiceData = practiceData;
            this.shuffle = shuffle;

            if (shuffle) this.practiceData.RemoveDuplicates();

            SpeedChart.AxisX[0].Separator.Step = 1;

            this.Loaded += SentencePracticeWindow_Loaded;
        }

        private void SentencePracticeWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            practiceData = PracticeData.FitPracticeData(practiceData, CurrentTextBlock);
            NextSentence();
        }

        private void SentencePracticeWindow_Closed(object sender, EventArgs e)
        {
            if (TypingSpeedList.Count > 0)
            {
                MainWindow.CurrentKeyLayout.Stats.AddStats(new KeyLayoutStats()
                {
                    SentencePracticeCount = TypingSpeedList.Count,
                    AverageTypingSpeed = Convert.ToInt32(TypingSpeedList.Average()),
                    AverageAccuracy = Convert.ToInt32(AccuracyList.Average())
                });
            }
        }

        private void NextSentence()
        {
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
                }

                double accuracy = Differ.CalculateAccuracy(diffs);
                TypingAccuracy = Convert.ToInt32(accuracy * 100);
                AccuracyList.Add(TypingAccuracy);

                TypingSpeed = Convert.ToInt32(typingMeasurer.Finish(CurrentTextBox.Text) * accuracy);
                TypingSpeedList.Add(TypingSpeed);
                AverageTypingSpeed = Convert.ToInt32(TypingSpeedList.Average());
            }

            if (!string.IsNullOrEmpty(CurrentTextBox.Text) || currentSentenceIndex is null) // 입력이 비어있지 않거나 첫 번째 호출인 경우
            {
                if (shuffle) // 무작위 순서 모드일 경우
                {
                    if (currentSentenceIndex is null) // 첫 호출인 경우
                    {
                        currentSentenceIndex = SentenceIndexRandom.Next(practiceData.TextData.Count);
                    }
                    else
                    {
                        int newIndex;
                        do
                        {
                            newIndex = SentenceIndexRandom.Next(practiceData.TextData.Count);
                        } while (newIndex == currentSentenceIndex); // 중복 방지

                        currentSentenceIndex = newIndex;
                    }
                }
                else
                {
                    if (currentSentenceIndex is null) currentSentenceIndex = 0; // 첫 호출인 경우
                    else if (currentSentenceIndex == practiceData.TextData.Count - 1) currentSentenceIndex = 0; // 마지막 인덱스인 경우, 순환
                    else currentSentenceIndex++;
                }

                CurrentText = practiceData.TextData[currentSentenceIndex.Value];
            }
        }

        private void CurrentTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                NextSentence();
                CurrentTextBox.Text = "";
                e.Handled = true;

                return;
            }
            if (CurrentTextBox.Text == "")
            {
                typingMeasurer.Start();
            }
        }

        private void CurrentTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string input = CurrentTextBox.Text;

            CurrentTextBlock.Inlines.Clear();
            var diffs
                = new List<Differ.DiffData>(Differ.Diff(CurrentText.Substring(0, Math.Min(input.Length, CurrentText.Length)),
                                                        CurrentTextBox.Text,
                                                        CurrentText));

            foreach (Differ.DiffData diff in diffs)
            {
                if (diff.State == Differ.DiffData.DiffState.Intermediate)
                {
                    diff.State = Differ.DiffData.DiffState.Unequal;
                }

                var run = new Run(diff.Text)
                {
                    Background = Differ.MapDiffState(diff.State)
                };
                CurrentTextBlock.Inlines.Add(run);
            }

            if (input.Length < CurrentText.Length)
            {
                CurrentTextBlock.Inlines.Add(new Run(CurrentText.Substring(input.Length)));
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
