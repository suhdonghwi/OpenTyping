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
                currentWordCount = currentText.Split(' ').Count();

                CurrentTextBlock.Inlines.Clear();
                foreach (char ch in value)
                {
                    CurrentTextBlock.Inlines.Add(new Run(ch.ToString()));
                }
            }
        }

        private int currentWordCount = 0;

        private string currentInput;
        public string CurrentInput
        {
            get => currentInput;
            set => SetField(ref currentInput, value);
        }

        private PracticeData practiceData;
        private bool shuffle;

        private int currentWordIndex = 0;
        private readonly Brush currentWordBack = Brushes.LightGreen;

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
            if (currentWordIndex == currentWordCount)
            {
                return;
            }

            var currentInlines = new List<Inline>(CurrentTextBlock.Inlines);
            CurrentTextBlock.Inlines.Clear();

            string tempCurrentText = currentText;
            for (int i = 0, wordIndex = 0; i < currentInlines.Count; i++)
            {
                if (((Run)currentInlines[i]).Text == " ")
                {
                    wordIndex++;
                    CurrentTextBlock.Inlines.Add(new Run(" "));
                }
                else
                {
                    CurrentTextBlock.Inlines.Add(new Run(tempCurrentText[i].ToString())
                    {
                        Background = wordIndex == currentWordIndex ? currentWordBack : Brushes.Transparent
                    });
                }
            }

            currentWordIndex++;
        }

        private void CurrentTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                CurrentInput = "";
                NextWord();
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
