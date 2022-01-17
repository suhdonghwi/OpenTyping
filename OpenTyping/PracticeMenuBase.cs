using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using OpenTyping.Properties;
using OpenTyping.Resources.Lang;

namespace OpenTyping
{
    public class PracticeMenuBase : UserControl, INotifyPropertyChanged
    {
        protected ObservableCollection<PracticeData> practiceDataList;
        public ObservableCollection<PracticeData> PracticeDataList
        {
            get => practiceDataList;
            private set => SetField(ref practiceDataList, value);
        }

        protected PracticeData selectedPracticeData;
        public PracticeData SelectedPracticeData
        {
            get => selectedPracticeData;
            set => SetField(ref selectedPracticeData, value);
        }

        public PracticeMenuBase()
        {
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                PracticeDataList =
                    new ObservableCollection<PracticeData>(
                        PracticeData.LoadFromDirectory((string)Settings.Default[MainWindow.PracticeDataDirStr], MainWindow.CurrentKeyLayout.Character));
            }
            catch (Exception ex)
            {
                if (ex is PracticeDataLoadFail || ex is InvalidPracticeDataException)
                {
                    MessageBox.Show(ex.Message, LangStr.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }

            SelectedPracticeData = null;
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
