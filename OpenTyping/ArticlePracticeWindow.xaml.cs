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
using MahApps.Metro.Controls.Dialogs;

namespace OpenTyping
{
    /// <summary>
    /// ArticlePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ArticlePracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        private PracticeData practiceData;
        private int currentSentenceIndex = 0;

        private int currentPage = 1;
        public int CurrentPage
        {
            get => currentPage;
            private set => SetField(ref currentPage, value);
        }

        private int totalPage;
        public int TotalPage
        {
            get => totalPage;
            private set => SetField(ref totalPage, value);
        }

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

        private bool freeze = false;

        private static readonly Differ Differ = new Differ();

        public ArticlePracticeWindow(PracticeData practiceData)
        {
            InitializeComponent();

            inputTextBoxes = new List<TextBox> { InputTextBox0, InputTextBox1, InputTextBox2 };
            targetTextBlocks = new List<TextBlock> { TargetTextBlock0, TargetTextBlock1, TargetTextBlock2 };

            this.practiceData = practiceData;
            this.Loaded += ArticlePracticeWindow_Loaded;
        }

        private void Next3Sentences()
        {
            for (int i = 0; i < 3; i++)
            {
                if (currentSentenceIndex + i == practiceData.TextData.Count) break;
                targetTextBlocks[i].Text = practiceData.TextData[currentSentenceIndex + i];
            }
        }

        private void EnableCurrentTextBox()
        {
            for (int i = 0; i < 3; i++) inputTextBoxes[i].IsEnabled = i == currentLine;
            inputTextBoxes[currentLine].Focus();
        }

        private void ArticlePracticeWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.practiceData = PracticeData.FitPracticeData(practiceData, TargetTextBlock0);
            TotalPage = practiceData.TextData.Count % 3 == 0 ? practiceData.TextData.Count / 3 : practiceData.TextData.Count / 3 + 1;

            Next3Sentences();
            EnableCurrentTextBox();
        }

        private void ArticlePracticeWindow_Closed(object sender, EventArgs e)
        {
            if (typingSpeedList.Count > 0)
            {
                MainWindow.CurrentKeyLayout.Stats.AddStats(new KeyLayoutStats()
                {
                    SentencePracticeCount = typingSpeedList.Count,
                    AverageTypingSpeed = Convert.ToInt32(typingSpeedList.Average()),
                    AverageAccuracy = Convert.ToInt32(accuracyList.Average())
                });
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

            for (int i = 0; i < diffs.Count() - 1; i++)
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

                currentTextBlock.Inlines.Add(run);
            }

            double accuracy = Differ.CalculateAccuracy(diffs);
            accuracyList.Add(Convert.ToInt32(accuracy * 100));
            typingSpeedList.Add(Convert.ToInt32(typingMeasurer.Finish(currentTextBox.Text) * accuracy));

            TypingAccuracy = Convert.ToInt32(accuracyList.Average());
            TypingSpeed = Convert.ToInt32(typingSpeedList.Average());

            if (currentLine == 2) // 다음 페이지로 이동
            {
                currentLine = 0;
                currentSentenceIndex++;
                if (currentSentenceIndex == practiceData.TextData.Count)
                {
                    FinishPracticeAsync();
                    return;
                }

                CurrentPage++;

                foreach (TextBox box in inputTextBoxes) box.Text = "";
                foreach (TextBlock block in targetTextBlocks) block.Text = "";

                Next3Sentences();
            }
            else
            {
                currentLine++;
                currentSentenceIndex++;
                if (currentSentenceIndex == practiceData.TextData.Count)
                {
                    FinishPracticeAsync();
                    return;
                }
            }

            EnableCurrentTextBox();
        }

        private async void FinishPracticeAsync()
        {
            freeze = true;
            await this.ShowMessageAsync("연습이 끝났습니다.",
                                        "최종 타속은 " + TypingSpeed + ", 정확도는 " + TypingAccuracy + "% 입니다.",
                                         MessageDialogStyle.Affirmative,
                                         new MetroDialogSettings{ AnimateHide = false });

            this.Close();
        }

        private void LineTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (freeze)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                NextLine();
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text == "") // 입력이 비어있는데 입력이 들어왔을 경우
            {
                typingMeasurer.Start(); // 타이머 (재)시작
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
            if (freeze)
            {
                e.Handled = true;
                return;
            }

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
                    Background = Differ.MapDiffState(diff.State)
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
