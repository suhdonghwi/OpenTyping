using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeMenu : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<PracticeData> practiceDataList;
        public ObservableCollection<PracticeData> PracticeDataList
        {
            get => practiceDataList;
            private set => SetField(ref practiceDataList, value);
        }

        private PracticeData selectedPracticeData;
        public PracticeData SelectedPracticeData
        {
            get => selectedPracticeData;
            set => SetField(ref selectedPracticeData, value);
        }

        public bool IsRandom { get; set; } = false;

        public SentencePracticeMenu()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            PracticeDataList =
                new ObservableCollection<PracticeData>(
                    PracticeData.LoadFromDirectory((string)Settings.Default[MainWindow.PracticeDataDir]));
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
