using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static KeyLayout CurrentKeyLayout { get; private set; }

        public const string KeyLayoutDataDirStr = "KeyLayoutDataDir";
        public const string KeyLayoutStr = "KeyLayout";
        public const string PracticeDataDirStr = "PracticeDataDir";

        public MainWindow()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (exeDirectory is null)
            {
                MessageBox.Show("응용 프로그램 경로를 찾는 도중 에러가 발생했습니다.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (string.IsNullOrEmpty((string)Settings.Default[KeyLayoutDataDirStr]))
            {
                string layoutsDirectory = Path.Combine(exeDirectory, "layouts");
                Settings.Default[KeyLayoutDataDirStr] = layoutsDirectory;
            }

            try
            {
                var keyLayouts =
                    new List<KeyLayout>(KeyLayout.LoadFromDirectory((string)Settings.Default[KeyLayoutDataDirStr]));

                var layoutName = (string)Settings.Default[KeyLayoutStr];
                KeyLayout currentKeylayout = keyLayouts.FirstOrDefault(keyLayout => keyLayout.Name == layoutName);

                if (currentKeylayout == null)
                {
                    KeyLayout dubeolsikLayout = keyLayouts.Find(keyLayout => keyLayout.Name == "두벌식 표준");

                    if (dubeolsikLayout != null)
                    {
                        Settings.Default[KeyLayoutStr] = dubeolsikLayout.Name;
                        CurrentKeyLayout = dubeolsikLayout;
                    }
                    else
                    {
                        Settings.Default[KeyLayoutStr] = keyLayouts[0].Name;
                        CurrentKeyLayout = keyLayouts[0];
                    }
                }
                else
                {
                    CurrentKeyLayout = currentKeylayout;
                }
            }
            catch (Exception ex)
            {
                if (ex is KeyLayoutLoadFail || ex is InvalidKeyLayoutDataException)
                {
                    MessageBox.Show(ex.Message, "열린타자", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }

            if (string.IsNullOrEmpty((string)Settings.Default[PracticeDataDirStr]))
            {
                string dataDirectory = Path.Combine(exeDirectory, "data");
                Settings.Default[PracticeDataDirStr] = dataDirectory;
            }
            
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSyllablePractice();
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            KeyLayout.SaveKeyLayout(CurrentKeyLayout);
            Settings.Default.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            KeyLayout.SaveKeyLayout(CurrentKeyLayout);

            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

            if (settingsWindow.KeyLayoutUpdated || settingsWindow.KeyLayoutDataDirUpdated)
            {
                CurrentKeyLayout = settingsWindow.SelectedKeyLayout;

                KeyPracticeMenu.KeyLayoutBox.LoadKeyLayout();

                var currentKeyLayoutNameBinding = new Binding
                {
                    Path = new PropertyPath("Name"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.CurrentKeyLayoutName.SetBinding(TextBlock.TextProperty, currentKeyLayoutNameBinding);

                var currentKeyLayoutCharBinding = new Binding
                {
                    Path = new PropertyPath("Character"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.CurrentKeyLayoutChar.SetBinding(TextBlock.TextProperty, currentKeyLayoutCharBinding);

                var mostIncorrectBinding = new Binding
                {
                    Path = new PropertyPath("Stats.MostIncorrect.Key"),
                    Source = CurrentKeyLayout,
                    Converter = new KeyPosToKeyConverter()
                };
                HomeMenu.MostIncorrectKey.SetBinding(KeyBox.KeyProperty, mostIncorrectBinding);

                var averageSpeedBinding = new Binding
                {
                    Path = new PropertyPath("Stats.AverageTypingSpeed"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.AverageTypingSpeed.SetBinding(TextBlock.TextProperty, averageSpeedBinding);

                var averageAccuracyBinding = new Binding
                {
                    Path = new PropertyPath("Stats.AverageAccuracy"),
                    Source = CurrentKeyLayout,
                    StringFormat = "{0}%"
                };
                HomeMenu.AverageAccuracy.SetBinding(TextBlock.TextProperty, averageAccuracyBinding);

                var sentencePracticeCountBinding = new Binding
                {
                    Path = new PropertyPath("Stats.SentencePracticeCount"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.SentencePracticeCount.SetBinding(TextBlock.TextProperty, sentencePracticeCountBinding);

                CheckSyllablePractice();
            }

            SentencePracticeMenu.LoadData();
            ArticlePracticeMenu.LoadData();
        }

        private void CheckSyllablePractice()
        {
            if (CurrentKeyLayout.Character == "한글")
            {
                SyllablePracticeTabItem.Visibility = Visibility.Visible;
            }
            else
            {
                SyllablePracticeTabItem.Visibility = Visibility.Collapsed;

                if (SyllablePracticeTabItem.IsSelected)
                {
                    MenuTabControl.SelectedIndex = 0;
                }
            }
        }
    }
}