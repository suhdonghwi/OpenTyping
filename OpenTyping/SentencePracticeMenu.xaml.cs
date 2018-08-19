using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
            try
            {
                PracticeDataList =
                    new ObservableCollection<PracticeData>(
                        PracticeData.LoadFromDirectory((string)Settings.Default[MainWindow.PracticeDataDir], MainWindow.CurrentKeyLayout.Character));
            }
            catch (Exception ex)
            {
                if (ex is PracticeDataLoadFail || ex is InvalidPracticeDataException)
                {
                    MessageBox.Show(ex.Message, "열린타자", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }

            SelectedPracticeData = null;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPracticeData is null)
            {
                MessageBox.Show("연습하실 연습 데이터를 선택해주세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            var sentencePracticeWindow = new SentencePracticeWindow(selectedPracticeData, IsRandom);
            sentencePracticeWindow.ShowDialog();
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
