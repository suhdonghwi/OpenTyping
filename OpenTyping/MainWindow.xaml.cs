using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static KeyLayout CurrentKeyLayout { get; private set; }

        public const string KeyLayoutDataDir = "KeyLayoutDataDir";
        public const string KeyLayout = "KeyLayout";
        public const string PracticeDataDir = "PracticeDataDir";

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

            if (string.IsNullOrEmpty((string)Settings.Default[KeyLayoutDataDir]))
            {
                string layoutsDirectory = Path.Combine(exeDirectory, "layouts");
                Settings.Default[KeyLayoutDataDir] = layoutsDirectory;
            }

            try
            {
                var keyLayouts =
                    new List<KeyLayout>(
                        OpenTyping.KeyLayout.LoadFromDirectory((string)Settings.Default[KeyLayoutDataDir]));

                var layoutName = (string)Settings.Default[KeyLayout];
                KeyLayout currentKeylayout = keyLayouts.FirstOrDefault(keyLayout => keyLayout.Name == layoutName);

                if (currentKeylayout == null)
                {
                    KeyLayout dubeolsikLayout = keyLayouts.Find(keyLayout => keyLayout.Name == "두벌식 표준");

                    if (dubeolsikLayout != null)
                    {
                        Settings.Default[KeyLayout] = dubeolsikLayout.Name;
                        CurrentKeyLayout = dubeolsikLayout;
                    }
                    else
                    {
                        Settings.Default[KeyLayout] = keyLayouts[0].Name;
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

            if (string.IsNullOrEmpty((string)Settings.Default[PracticeDataDir]))
            {
                string dataDirectory = Path.Combine(exeDirectory, "data");
                Settings.Default[PracticeDataDir] = dataDirectory;
            }
            
            InitializeComponent();
            Closed += MainWindow_Closed;
        }

        private static void SaveKeyLayout()
        {
            File.WriteAllText(CurrentKeyLayout.Location, JsonConvert.SerializeObject(CurrentKeyLayout, Formatting.Indented));
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            SaveKeyLayout();
            Settings.Default.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

            SaveKeyLayout();
            CurrentKeyLayout = settingsWindow.SelectedKeyLayout;

            KeyPracticeMenu.KeyLayoutBox.LoadKeyLayout();

            var binding = new Binding
            {
                Path = new PropertyPath("Stats.MostIncorrect.Key"),
                Source = CurrentKeyLayout,
                Converter = new KeyPosToKeyConverter()
            };
            HomeMenu.MostIncorrectKey.SetBinding(KeyBox.KeyProperty, binding);

            SentencePracticeMenu.LoadData();
        }
    }
}