using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
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

        private string currentInput;
        public string CurrentInput
        {
            get => currentInput;
            set => SetField(ref currentInput, value);
        }

        private PracticeData practiceData;
        private bool shuffle;

        private int currentWordIndex = -1;
        private readonly Brush currentWordBack = Brushes.LightGreen;
        private readonly Brush correctForeground = Brushes.Green;
        private readonly Brush incorrectForeground = Brushes.Red;

        public SentencePracticeWindow(PracticeData practiceData, bool shuffle)
        {
            InitializeComponent();

            this.practiceData = practiceData;
            this.shuffle = shuffle;

            CurrentText = "동해물과 백두산이 마르고 닳도록";
            NextWord();
        }

        private void NextWord()
        {
            if (currentWordIndex + 1 == currentWordSequence.Count)
            {
                return;
            }

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

                    int wordLength = Math.Min(targetWord.Length, currentWord.Length);
                    int j = 0;
                    for (; j < wordLength; j++)
                    {
                        var newRun = new Run(targetWord[j].ToString())
                        {
                            Foreground = targetWord[j] == currentWord[j] ? correctForeground : incorrectForeground
                        };
                        CurrentTextBlock.Inlines.Add(newRun);
                    }

                    for (; j < targetWord.Length; j++)
                    {
                        var newRun = new Run(targetWord[j].ToString())
                        {
                            Foreground = incorrectForeground
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
