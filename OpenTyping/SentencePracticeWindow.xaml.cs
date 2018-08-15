using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                SetField(ref currentText, value);
                currentWordSequence = new List<string>(currentText.Split(' '));
            }
        }

        private string currentInput;
        public string CurrentInput
        {
            get => currentInput;
            set => SetField(ref currentInput, value);
        }

        private PracticeData practiceData;
        private bool shuffle;

        private List<string> currentWordSequence;
        private int currentWordIndex = 0;

        public SentencePracticeWindow(PracticeData practiceData, bool shuffle)
        {
            InitializeComponent();

            this.practiceData = practiceData;
            this.shuffle = shuffle;

            CurrentText = "동해물과 백두산이 마르고 닳도록";
            NextWord();
        }

        void NextWord()
        {
            if (currentWordIndex == currentWordSequence.Count)
            {
                return;
            }

            TextBlock newText = CurrentTextBlock;
            newText.Inlines.Clear();
            for (int i = 0; i < currentWordSequence.Count; i++)
            {
                var word = new Run(currentWordSequence[i]);
                if (currentWordIndex == i)
                {
                    word.Background = Brushes.LightGreen;
                }

                newText.Inlines.Add(word);
                newText.Inlines.Add(new Run(" "));
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
