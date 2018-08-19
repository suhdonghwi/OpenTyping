using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
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

        private string currentInput;
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
            foreach (Inline inline in CurrentTextBlock.Inlines)
            {
                string temp = XamlWriter.Save(inline);
                Stream s = new MemoryStream(Encoding.UTF8.GetBytes(temp));

                Inline newInline = XamlReader.Load(s) as Inline;
                PreviousTextBlock.Inlines.Add(newInline);
            }

            string nextSentence = practiceData.TextData[currentSentenceIndex.Value];
            CurrentText = nextSentence;

            currentWordIndex = -1;
            NextWord();

            currentInputHistory.Clear();
        }

        private void NextWord()
        {
            currentWordIndex++;
            if (CurrentInput != null) currentInputHistory.Add(CurrentInput);

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
                    string currentWord = currentInputHistory[i];

                    int j = 0;
                    for (; j < currentWord.Length; j++)
                    {
                        Run newRun;

                        if (j >= targetWord.Length)
                        {
                            newRun = new Run(currentWord[j].ToString())
                            {
                                Foreground = incorrectForeground,
                                TextDecorations = TextDecorations.Underline
                            };
                        }
                        else
                        {
                            newRun = new Run(currentWord[j].ToString())
                            {
                                Foreground = targetWord[j] == currentWord[j] ? correctForeground : incorrectForeground
                            };
                        }

                        CurrentTextBlock.Inlines.Add(newRun);
                    }

                    for (; j < targetWord.Length; j++)
                    {
                        var newRun = new Run(targetWord[j].ToString())
                        {
                            Foreground = incorrectForeground,
                            TextDecorations = TextDecorations.Strikethrough
                        };
                        CurrentTextBlock.Inlines.Add(newRun);
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
                NextWord();
                CurrentInput = "";
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
