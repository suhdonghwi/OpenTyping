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
using OpenTyping.Resources.Lang;

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

        private string programLang = (string)Settings.Default[MainWindow.ProgramLang];
        public string ProgramLang
        {
            get => programLang;
            private set => SetField(ref programLang, value);
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
            InitLangUI();
           
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

        private void InitLangUI()
        {
            this.SetTextBylanguage();

            // 언어 RadioButton checked 표시   
            if (programLang == "uz")
            {
                uz.IsChecked = true;
            }
            else if (programLang == "en")
            {
                en.IsChecked = true;
            }
            else if (programLang == "ko")
            {
                ko.IsChecked = true;
            }
        }

        private void SetTextBylanguage()
        {
            SelfWindow.Title = LangStr.Setting;
            MainName.Text = LangStr.Setting;
            TabLbl1.Content = LangStr.SetKeyboard;
            TabLbl2.Content = LangStr.SetPracData;
            TabLbl3.Content = LangStr.SetProgramLang;
            ConfirmBtn.Text = LangStr.OK;

            // Layout
            CurKeyBoard.Text = LangStr.CurKeyboard;
            InitStat.Text = LangStr.InitStatInfo;
            KeyDataPath.Text = LangStr.KeyDataPath;

            // Sentence
            CurPracData.Text = LangStr.CurPracData;
            PracDataPath.Text = LangStr.PracDataPath;
            Add.Text = LangStr.Add;
            Del.Text = LangStr.Delete;
        }

        private void AddKeyLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDialog = new CommonOpenFileDialog()
            {
                Title = LangStr.KeyDataOpen
            };

            dataFileDialog.Filters.Add(new CommonFileDialogFilter(LangStr.PracDataFile, "*.json"));
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
                    MessageBox.Show(LangStr.ErrMsg2,
                                    LangStr.AppName,
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
                MessageBox.Show(LangStr.ErrMsg4,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result
                = MessageBox.Show(LangStr.WarnMsg1 +" \"" + SelectedKeyLayout.Name + "\" " + LangStr.WarnMsg3,
                                  LangStr.AppName,
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
                = MessageBox.Show(LangStr.WarnMsg1 + " \"" + SelectedKeyLayout.Name + "\" " + LangStr.WarnMsg4,
                                  LangStr.AppName,
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
                        MessageBox.Show(ex.Message, LangStr.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else throw;
                }
            }

            Focus();
        }

        private void AddPracticeDataButton_Click(object sender, RoutedEventArgs e)
        {
            var dataFileDialog = new CommonOpenFileDialog();

            dataFileDialog.Filters.Add(new CommonFileDialogFilter(LangStr.PracDataFile, "*.json"));
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
                    MessageBox.Show(LangStr.ErrMsg3,
                                    LangStr.AppName,
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
                MessageBox.Show(LangStr.ErrMsg5,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result
                = MessageBox.Show(LangStr.WarnMsg2 + " \"" + SelectedKeyLayout.Name + "\" " + LangStr.WarnMsg3,
                                  LangStr.AppName,
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
                        MessageBox.Show(ex.Message, LangStr.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var prevProgramLang = programLang;

            if ((bool)uz.IsChecked)
            {
                programLang = "uz";
            }
            else if ((bool)en.IsChecked)
            {
                programLang = "en";
            }
            else if ((bool)ko.IsChecked)
            {
                programLang = "ko";
            }

            if (prevProgramLang != programLang)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetTextBylanguage(programLang);
            }

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

            if ((string)Settings.Default[MainWindow.ProgramLang] != ProgramLang)
            {
                Settings.Default[MainWindow.ProgramLang] = ProgramLang;
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