using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using OpenTyping.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace OpenTyping
{
    /// <summary>
    ///     SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public ObservableCollection<KeyLayout> KeyLayouts { get; private set; }
        public string KeyLayoutDataDir { get; private set; } = (string)Settings.Default["KeyLayoutDataDir"];
        public KeyLayout SelectedKeyLayout { get; set; }

        public SettingsWindow(IEnumerable<KeyLayout> keyLayouts)
        {
            InitializeComponent();

            this.Closing += this.OnClose;

            this.DataContext = this;

            KeyLayouts = new ObservableCollection<KeyLayout>(keyLayouts);
            KeyLayoutsCombo.ItemsSource = KeyLayouts;

            var currentKeyLayout = (string)Settings.Default["KeyLayout"];
            foreach (KeyLayout item in KeyLayoutsCombo.Items)
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

            dataFileDialog.Filters.Add(new CommonFileDialogFilter("자판 데이터 파일", "*.kl"));
            dataFileDialog.Multiselect = false;
            dataFileDialog.EnsureFileExists = true;
            dataFileDialog.EnsurePathExists = true;

            if (dataFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dataFileName = Path.GetFileName(dataFileDialog.FileName);
                string destLocation =
                    (string)Settings.Default["KeyLayoutDataDir"] + "/" + dataFileName;

                if (File.Exists(destLocation))
                {
                    MessageBox.Show("같은 이름의 파일이 이미 자판 데이터 경로에 존재합니다.", 
                                    "열린타자",
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                }
                else
                {
                    File.Copy(dataFileName, destLocation);

                    KeyLayout keyLayout = KeyLayout.LoadKeyLayout(destLocation);
                    keyLayout.Location = destLocation;

                    KeyLayouts.Add(keyLayout);
                }
                
            }

            this.Focus();
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
                KeyLayoutsCombo.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
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
                KeyLayoutDataDir = dataFileDirDialog.FileName;
                KeyLayoutDataDirTxt.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

                KeyLayouts.Clear();

                foreach (KeyLayout keyLayout in KeyLayout.LoadKeyLayouts())
                {
                    KeyLayouts.Add(keyLayout);
                }

                SelectedKeyLayout = KeyLayouts[0];
                KeyLayoutsCombo.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
            }

            this.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Settings.Default["KeyLayout"] = SelectedKeyLayout.Name;
            Settings.Default["KeyLayoutDataDir"] = KeyLayoutDataDir;

            Settings.Default.Save();
        }
    }
}