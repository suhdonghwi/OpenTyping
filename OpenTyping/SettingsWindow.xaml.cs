using System;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTyping.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

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

        private string keyLayoutDataDir = (string)Settings.Default[MainWindow.KeyLayoutDataDir];
        public string KeyLayoutDataDir
        {
            get => keyLayoutDataDir;
            private set => SetField(ref keyLayoutDataDir, value);
        } 

        private KeyLayout selectedKeyLayout;
        public KeyLayout SelectedKeyLayout
        {
            get => selectedKeyLayout;
            set => SetField(ref selectedKeyLayout, value);
        }

        public SettingsWindow()
        {
            InitializeComponent();

            this.Closing += this.OnClose;
            this.DataContext = this;

            KeyLayouts = new ObservableCollection<KeyLayout>(KeyLayout.LoadFromDirectory(KeyLayoutDataDir));

            var currentKeyLayout = (string)Settings.Default[MainWindow.KeyLayout];
            foreach (KeyLayout item in KeyLayouts)
            {
                if (item.Name == currentKeyLayout)
                {
                    SelectedKeyLayout = item;
                    break;
                }
            }
        }

        private void AddKeyLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDialog = new CommonOpenFileDialog();

            dataFileDialog.Filters.Add(new CommonFileDialogFilter("자판 데이터 파일", "*.json"));
            dataFileDialog.Multiselect = false;
            dataFileDialog.EnsureFileExists = true;
            dataFileDialog.EnsurePathExists = true;

            if (dataFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dataFileLocation = dataFileDialog.FileName;
                string dataFileName = Path.GetFileName(dataFileLocation);
                string destLocation =
                    Path.Combine((string)Settings.Default[MainWindow.KeyLayoutDataDir], dataFileName);

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

                this.Focus();
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

        private void KeyLayoutDataDirButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dataFileDirDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (dataFileDirDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                IList<KeyLayout> newKeyLayouts = KeyLayout.LoadFromDirectory(dataFileDirDialog.FileName);

                if (keyLayouts.Count == 0)
                {
                    MessageBox.Show("경로 " + (string)Settings.Default["KeyLayoutDataDir"] +
                                "에서 자판 데이터 파일을 찾을 수 없습니다. 해당 경로에 자판 데이터를 생성하고 다시 시도하세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                }
                else
                {
                    KeyLayoutDataDir = dataFileDirDialog.FileName;
                    KeyLayouts = new ObservableCollection<KeyLayout>(newKeyLayouts);
                    SelectedKeyLayout = KeyLayouts[0];
                }
            }

            this.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Settings.Default[MainWindow.KeyLayout] = SelectedKeyLayout.Name;
            Settings.Default[MainWindow.KeyLayoutDataDir] = KeyLayoutDataDir;

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