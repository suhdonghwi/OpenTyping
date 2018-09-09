using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace OpenTyping
{
    /// <summary>
    /// ArticlePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ArticlePracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        private readonly PracticeData practiceData;
        private int currentSentenceIndex = 0;

        private int currentLine = 0;

        private readonly TypingMeasurer typingMeasurer = new TypingMeasurer();

        private int typingSpeed;
        public int TypingSpeed
        {
            get => typingSpeed;
            private set => SetField(ref typingSpeed, value);
        }

        private int typingAccuracy;
        public int TypingAccuracy
        {
            get => typingAccuracy;
            private set => SetField(ref typingAccuracy, value);
        }

        private readonly List<int> typingSpeedList = new List<int>();
        private readonly List<int> accuracyList = new List<int>();

        private readonly IList<TextBox> inputTextBoxes;
        private readonly IList<TextBlock> targetTextBlocks;

        private readonly Brush correctBackground = Brushes.LightGreen;
        private readonly Brush incorrectBackground = Brushes.Pink;
        private readonly Brush intermidiateBackground = new SolidColorBrush(Color.FromRgb(215, 244, 215));

        private static readonly Differ Differ = new Differ();

        public ArticlePracticeWindow(PracticeData practiceData)
        {
            InitializeComponent();

            inputTextBoxes = new List<TextBox> { InputTextBox0, InputTextBox1, InputTextBox2 };
            targetTextBlocks = new List<TextBlock> { TargetTextBlock0, TargetTextBlock1, TargetTextBlock2 };

            this.practiceData = practiceData;

            for (int i = 0; i < 3; i++)
            {
                if (currentSentenceIndex + i == practiceData.TextData.Count) break;
                targetTextBlocks[i].Text = practiceData.TextData[currentSentenceIndex + i];
            }

            for (int i = 0; i < 3; i++) inputTextBoxes[i].IsEnabled = i == currentLine;
        }

        private Brush MapDiffState(Differ.DiffData.DiffState state)
        {
            switch (state)
            {
                case Differ.DiffData.DiffState.Equal:
                    return correctBackground;
                case Differ.DiffData.DiffState.Unequal:
                    return incorrectBackground;
                case Differ.DiffData.DiffState.Intermediate:
                    return intermidiateBackground;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NextLine()
        {
            if (string.IsNullOrEmpty(inputTextBoxes[currentLine].Text)) return;

            var currentTextBox = inputTextBoxes[currentLine];
            var currentTextBlock = targetTextBlocks[currentLine];
            string currentText = currentTextBlock.Text;

            currentTextBlock.Inlines.Clear();
            var diffs = new List<Differ.DiffData>(Differ.Diff(currentTextBox.Text, currentText, currentTextBox.Text));

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
                    Background = MapDiffState(diff.State)
                };
                currentTextBlock.Inlines.Add(run);
            }

            double accuracy = diffs.Sum(data => data.State == Differ.DiffData.DiffState.Equal ? data.Text.Length : 0) /
                              (double)diffs.Sum(data => data.Text.Length);
            accuracyList.Add(Convert.ToInt32(accuracy * 100));
            typingSpeedList.Add(Convert.ToInt32(typingMeasurer.Finish(currentTextBox.Text) * accuracy));

            TypingAccuracy = Convert.ToInt32(accuracyList.Average());
            TypingSpeed = Convert.ToInt32(typingSpeedList.Average());

            if (currentLine == 2) // 다음 페이지로 이동
            {
                currentLine = 0;
                currentSentenceIndex++;

                foreach (TextBox box in inputTextBoxes) box.Text = "";
                foreach (TextBlock block in targetTextBlocks) block.Text = "";

                for (int i = 0; i < 3; i++)
                {
                    if (currentSentenceIndex + i == practiceData.TextData.Count) break;
                    targetTextBlocks[i].Text = practiceData.TextData[currentSentenceIndex + i];
                }
            }
            else
            {
                currentLine++;
                currentSentenceIndex++;
            }

            for (int i = 0; i < 3; i++) inputTextBoxes[i].IsEnabled = i == currentLine;

            inputTextBoxes[currentLine].Focus();
        }

        private void LineTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                NextLine();
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text == "")
            {
                typingMeasurer.Start();
            }
        }

        private void LineTextBox_PreviewExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void LineTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = inputTextBoxes[currentLine];
            var currentTextBlock = targetTextBlocks[currentLine];
            string input = currentTextBox.Text;
            string currentText = practiceData.TextData[currentSentenceIndex];

            var diffs
                = new List<Differ.DiffData>(Differ.Diff(currentText.Substring(0, Math.Min(input.Length, currentText.Length)),
                                                        currentTextBox.Text,
                                                        currentText));

            for (int i = 0; i < diffs.Count() - 1; i++)
            {
                if (diffs[i].State == Differ.DiffData.DiffState.Intermediate)
                {
                    diffs[i].State = Differ.DiffData.DiffState.Unequal;
                }
            }
            
            currentTextBlock.Inlines.Clear();
            foreach (Differ.DiffData diff in diffs)
            {
                var run = new Run(diff.Text)
                {
                    Background = MapDiffState(diff.State)
                };
                currentTextBlock.Inlines.Add(run);
            }

            if (input.Length < currentText.Length)
            {
                currentTextBlock.Inlines.Add(new Run(currentText.Substring(input.Length)));
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
