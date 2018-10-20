using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    ///     SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : MetroWindow, INotifyPropertyChanged
    {
        private ObservableCollection<KeyLayout> keyLayouts;
        public ObservableCollection<KeyLayout> KeyLayouts
        {
            get => keyLayouts;
            private set => SetField(ref keyLayouts, value);
        }

        private string keyLayoutDataDir = (string)Settings.Default[MainWindow.KeyLayoutDataDirStr];
        public string KeyLayoutDataDir
        {
            get => keyLayoutDataDir;
            private set => SetField(ref keyLayoutDataDir, value);
        }

        private ObservableCollection<PracticeData> practiceDataList;
        public ObservableCollection<PracticeData> PracticeDataList
        {
            get => practiceDataList;
            private set => SetField(ref practiceDataList, value);
        }

        private string practiceDataDir = (string)Settings.Default[MainWindow.PracticeDataDirStr];
        public string PracticeDataDir
        {
            get => practiceDataDir;
            private set => SetField(ref practiceDataDir, value);
        } 

        private KeyLayout selectedKeyLayout;
        public KeyLayout SelectedKeyLayout
        {
            get => selectedKeyLayout;
            set => SetField(ref selectedKeyLayout, value);
        }

        private PracticeData selectedPracticeData;
        public PracticeData SelectedPracticeData
        {
            get => selectedPracticeData;
            set => SetField(ref selectedPracticeData, value);
        }

        public bool KeyLayoutUpdated { get; set; }
        public bool KeyLayoutDataDirUpdated { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();

            Closing += OnClose;

            KeyLayouts = new ObservableCollection<KeyLayout>(KeyLayout.LoadFromDirectory(KeyLayoutDataDir));

            var currentKeyLayout = (string)Settings.Default[MainWindow.KeyLayoutStr];
            foreach (KeyLayout item in KeyLayouts)
            {
                if (item.Name == currentKeyLayout)
                {
                    SelectedKeyLayout = item;
                    break;
                }
            }

            PracticeDataList = new ObservableCollection<PracticeData>(PracticeData.LoadFromDirectory(PracticeDataDir));
        }

        private void AddKeyLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDialog = new CommonOpenFileDialog()
            {
                Title = "자판 파일 열기"
            };

            dataFileDialog.Filters.Add(new CommonFileDialogFilter("자판 데이터 파일", "*.json"));
            dataFileDialog.Multiselect = false;
            dataFileDialog.EnsureFileExists = true;
            dataFileDialog.EnsurePathExists = true;

            if (dataFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dataFileLocation = dataFileDialog.FileName;
                string dataFileName = Path.GetFileName(dataFileLocation);
                string destLocation = Path.Combine(KeyLayoutDataDir, dataFileName);

                if (File.Exists(destLocation))
                {
                    MessageBox.Show("같은 이름의 파일이 이미 자판 데이터 경로에 존재합니다.",
                                    "열린타자",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                else
                {
                    File.Copy(dataFileLocation, destLocation);
                    KeyLayout keyLayout = KeyLayout.Load(destLocation);
                    KeyLayouts.Add(keyLayout);
                    SelectedKeyLayout = keyLayout;
                }

                Focus();
            }
        }

        private void RemoveKeyLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (KeyLayouts.Count == 1)
            {
                MessageBox.Show("자판 데이터가 한 개 존재하여 삭제할 수 없습니다.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result 
                = MessageBox.Show("선택된 자판 데이터 \"" + SelectedKeyLayout.Name + "\" 를 삭제하시겠습니까?",
                                  "열린타자",
                                  MessageBoxButton.OKCancel,
                                  MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                File.Delete(SelectedKeyLayout.Location);
                KeyLayouts.Remove(SelectedKeyLayout);
                SelectedKeyLayout = KeyLayouts[0];
            }
        }

        private void ClearStatButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result 
                = MessageBox.Show("선택된 자판 데이터 \"" + SelectedKeyLayout.Name + "\" 의 통계 정보를 삭제하시겠습니까?",
                                  "열린타자",
                                  MessageBoxButton.OKCancel,
                                  MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                SelectedKeyLayout.Stats = new KeyLayoutStats();
                KeyLayout.SaveKeyLayout(SelectedKeyLayout);

                KeyLayoutUpdated = true;
            }
        }

        private void KeyLayoutDataDirButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDirDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dataFileDirDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    KeyLayouts =
                        new ObservableCollection<KeyLayout>(KeyLayout.LoadFromDirectory(dataFileDirDialog.FileName));
                    KeyLayoutDataDir = dataFileDirDialog.FileName;
                    SelectedKeyLayout = KeyLayouts[0];
                }
                catch (Exception ex)
                {
                    if (ex is KeyLayoutLoadFail || ex is InvalidKeyLayoutDataException)
                    {
                        MessageBox.Show(ex.Message, "열린타자", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else throw;
                }
            }

            Focus();
        }

        private void AddPracticeDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDialog = new CommonOpenFileDialog();

            dataFileDialog.Filters.Add(new CommonFileDialogFilter("연습 데이터 파일", "*.json"));
            dataFileDialog.Multiselect = false;
            dataFileDialog.EnsureFileExists = true;
            dataFileDialog.EnsurePathExists = true;

            if (dataFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dataFileLocation = dataFileDialog.FileName;
                string dataFileName = Path.GetFileName(dataFileLocation);
                string destLocation =
                    Path.Combine(PracticeDataDir, dataFileName);

                if (File.Exists(destLocation))
                {
                    MessageBox.Show("같은 이름의 파일이 이미 연습 데이터 경로에 존재합니다.",
                                    "열린타자",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                else
                {
                    File.Copy(dataFileLocation, destLocation);
                    PracticeData practiceData = PracticeData.Load(destLocation);
                    PracticeDataList.Add(practiceData);
                }

                Focus();
            }
        }

        private void RemovePracticeDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (PracticeDataList.Count == 1)
            {
                MessageBox.Show("연습 데이터가 한 개 존재하여 삭제할 수 없습니다.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result 
                = MessageBox.Show("선택된 연습 데이터 \"" + SelectedPracticeData.Name + "\" 를 삭제하시겠습니까?",
                                  "열린타자",
                                  MessageBoxButton.OKCancel,
                                  MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                File.Delete(SelectedPracticeData.Location);
                PracticeDataList.Remove(SelectedPracticeData);
                SelectedPracticeData = null;
            }
        }

        private void PracticeDataDirButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDirDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dataFileDirDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    PracticeDataList =
                        new ObservableCollection<PracticeData>(
                            PracticeData.LoadFromDirectory(dataFileDirDialog.FileName));
                    PracticeDataDir = dataFileDirDialog.FileName;
                }
                catch (Exception ex)
                {
                    if (ex is PracticeDataLoadFail || ex is InvalidPracticeDataException)
                    {
                        MessageBox.Show(ex.Message, "열린타자", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            if ((string)Settings.Default[MainWindow.KeyLayoutStr] != SelectedKeyLayout.Name)
            {
                Settings.Default[MainWindow.KeyLayoutStr] = SelectedKeyLayout.Name;
                KeyLayoutUpdated = true;
            }

            if ((string)Settings.Default[MainWindow.KeyLayoutDataDirStr] != KeyLayoutDataDir)
            {
                Settings.Default[MainWindow.KeyLayoutDataDirStr] = KeyLayoutDataDir;
                KeyLayoutDataDirUpdated = true;
            }

            Settings.Default[MainWindow.PracticeDataDirStr] = PracticeDataDir;

            Settings.Default.Save();
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