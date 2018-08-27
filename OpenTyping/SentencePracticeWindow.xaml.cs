using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media;
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

        private readonly PracticeData practiceData;
        private readonly bool shuffle;

        private readonly TypingMeasurer typingMeasurer = new TypingMeasurer();

        private int typingSpeed;
        public int TypingSpeed
        {
            get => typingSpeed;
            private set => SetField(ref typingSpeed, value);
        }

        private int? currentSentenceIndex;
        private static readonly Random SentenceIndexRandom = new Random();

        private readonly Brush correctForeground = Brushes.LightGreen;
        private readonly Brush incorrectForeground = Brushes.Pink;

        public SentencePracticeWindow(PracticeData practiceData, bool shuffle)
        {
            InitializeComponent();

            this.practiceData = practiceData;
            this.shuffle = shuffle;

            if (shuffle) this.practiceData.RemoveDuplicates();

            NextSentence();
        }

        private void NextSentence()
        {
            if (CurrentTextBox.Text != "")
            {
                string previousSentence = CurrentTextBox.Text;
                TypingSpeed = Convert.ToInt32(typingMeasurer.Finish(previousSentence));
            }

            if (shuffle)
            {
                if (currentSentenceIndex is null)
                {
                    currentSentenceIndex = SentenceIndexRandom.Next(practiceData.TextData.Count);
                }
                else
                {
                    int tempIndex = currentSentenceIndex.Value;
                    while ((currentSentenceIndex = SentenceIndexRandom.Next(practiceData.TextData.Count)) == tempIndex);
                }
            }
            else
            {
                if (currentSentenceIndex is null) currentSentenceIndex = 0;
                else if (currentSentenceIndex == practiceData.TextData.Count - 1) currentSentenceIndex = 0;
                else currentSentenceIndex++;
            }

            PreviousTextBlock.Inlines.Clear();
            foreach (var inline in CurrentTextBlock.Inlines)
            {
                var run = (Run)inline;
                Run newRun = new Run
                {
                    Text = run.Text,
                    Foreground = run.Foreground,
                    TextDecorations = run.TextDecorations
                };

                PreviousTextBlock.Inlines.Add(newRun);
            }

            string nextSentence = practiceData.TextData[currentSentenceIndex.Value];
            CurrentText = nextSentence;
        }

        private void HighlightDiff()
        {
            string input = CurrentTextBox.Text;

            var differ = new Differ();
            IEnumerable<Differ.DiffData> diffs = differ.Diff(CurrentText.Substring(0, input.Length), CurrentTextBox.Text);

            CurrentTextBlock.Inlines.Clear();
            foreach (Differ.DiffData diff in diffs)
            {
                var run = new Run(diff.Text);
                switch (diff.State)
                {
                    case Differ.DiffData.DiffState.Equal:
                        run.Background = correctForeground;
                        break;
                    case Differ.DiffData.DiffState.Unequal:
                        run.Background = incorrectForeground;
                        break;
                }

                CurrentTextBlock.Inlines.Add(run);
            }

            CurrentTextBlock.Inlines.Add(new Run(CurrentText.Substring(input.Length)));
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
            HighlightDiff();
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
