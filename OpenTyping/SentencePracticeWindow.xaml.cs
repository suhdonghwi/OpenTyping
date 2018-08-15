using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeWindow : MetroWindow, INotifyPropertyChanged
    {
        private string currentText = "동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리나라 만세 무궁화 삼천리 화려강산";
        public string CurrentText
        {
            get => currentText;
            set => SetField(ref currentText, value);
        }

        private PracticeData practiceData;
        private bool shuffle;

        public SentencePracticeWindow(PracticeData practiceData, bool shuffle)
        {
            InitializeComponent();

            this.practiceData = practiceData;
            this.shuffle = shuffle;
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
