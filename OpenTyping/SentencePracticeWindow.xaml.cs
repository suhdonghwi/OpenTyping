using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
            set
            {
                currentText = value;
                currentWordSequence = new List<string>(currentText.Split(' '));

                CurrentTextBlock.Inlines.Clear();
                foreach (char ch in value)
                {
                    CurrentTextBlock.Inlines.Add(new Run(ch.ToString()));
                }
            }
        }

        private List<string> currentWordSequence = new List<string>();
        private readonly List<string> currentInputHistory = new List<string>();

        private string currentInput = "";
        public string CurrentInput
        {
            get => currentInput;
            set => SetField(ref currentInput, value);
        }

        private readonly PracticeData practiceData;
        private readonly bool shuffle;

        private int? currentSentenceIndex = null;
        private static readonly Random SentenceIndexRandom = new Random();

        private int currentWordIndex = -1;
        private readonly Brush currentWordBack = Brushes.LightGreen;
        private readonly Brush correctForeground = Brushes.Green;
        private readonly Brush incorrectForeground = Brushes.Red;

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

            currentWordIndex = -1;
            NextWord();

            currentInputHistory.Clear();
        }

        private void PreviousWord()
        {
            if (currentWordIndex <= 0) return;

            currentWordIndex -= 2;
            CurrentInput = currentInputHistory[currentWordIndex + 1];
            CurrentTextBox.CaretIndex = CurrentInput.Length;
            currentInputHistory.RemoveAt(currentInputHistory.Count - 1);

            NextWord();
        }

        private void NextWord()
        {
            currentWordIndex++;

            CurrentTextBlock.Inlines.Clear();
            for (int i = 0; i < currentWordSequence.Count; i++)
            {
                string targetWord = currentWordSequence[i];

                if (i == currentWordIndex)
                {
                    var newRun = new Run(targetWord)
                    {
                        Background = currentWordBack
                    };

                    CurrentTextBlock.Inlines.Add(newRun);
                }
                else if (i < currentWordIndex)
                {
                    string inputWord = currentInputHistory[i];
                    int length = Math.Min(inputWord.Length, targetWord.Length);

                    string tempString = "";
                    bool correct = inputWord[0] == targetWord[0];
                    int j = 0;
                    for (; j < length; j++)
                    {
                        char inputChar = inputWord[j];
                        if ((inputChar == targetWord[j]) == correct)
                        {
                            tempString += inputChar;
                        }
                        else
                        {
                            CurrentTextBlock.Inlines.Add(new Run(tempString)
                            {
                                Foreground = correct ? correctForeground : incorrectForeground
                            });
                            correct = !correct;
                            tempString = inputChar.ToString();
                        }
                    }

                    if (tempString != "")
                    {
                        CurrentTextBlock.Inlines.Add(new Run(tempString)
                        {
                            Foreground = correct ? correctForeground : incorrectForeground
                        });
                    }

                    if (j < inputWord.Length)
                    {
                        CurrentTextBlock.Inlines.Add(new Run(inputWord.Substring(j))
                        {
                            Foreground = incorrectForeground,
                            TextDecorations = TextDecorations.Underline
                        });
                    }
                    else if (j < targetWord.Length)
                    {
                        CurrentTextBlock.Inlines.Add(new Run(targetWord.Substring(j))
                        {
                            Foreground = incorrectForeground,
                            TextDecorations = TextDecorations.Strikethrough
                        });
                    }
                }
                else
                {
                    var newRun = new Run(currentWordSequence[i]);
                    CurrentTextBlock.Inlines.Add(newRun);
                }

               CurrentTextBlock.Inlines.Add(new Run(" "));
            }

            if (currentWordIndex == currentWordSequence.Count)
            {
                NextSentence();
            }
        }

        private void CurrentTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                if (!string.IsNullOrEmpty(CurrentInput))
                {
                    currentInputHistory.Add(CurrentInput);
                    NextWord();
                    CurrentInput = "";
                }

                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Back || e.ImeProcessedKey == System.Windows.Input.Key.Back)
            {
                if (CurrentTextBox.Text == "")
                {
                    PreviousWord();
                    e.Handled = true;
                }
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
