using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IList<KeyLayout> keyLayouts;
        public static KeyLayout CurrentKeyLayout { get; private set; }

        private KeyLayout GetCurrentKeyLayout()
        {
            var layoutName = (string)Settings.Default["KeyLayout"];

            foreach (KeyLayout keyLayout in keyLayouts)
            {
                if (keyLayout.Name == layoutName)
                {
                    return keyLayout;
                }
            }

            return null;
        }

        public MainWindow()
        {
            this.DataContext = this;

            if (string.IsNullOrEmpty((string)Settings.Default["KeyLayoutDataDir"]))
            {
                string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string layoutsDirectory = Path.Combine(exeDirectory, "layouts");

                Settings.Default["KeyLayoutDataDir"] = layoutsDirectory;
            }

            keyLayouts = KeyLayout.LoadKeyLayouts();

            if (keyLayouts.Count == 0)
            {
                MessageBox.Show("경로 " + (string)Settings.Default["KeyLayoutDataDir"] +
                                "에서 자판 데이터 파일을 찾을 수 없습니다. 해당 경로에 자판 데이터를 생성하고 다시 시도하세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (string.IsNullOrEmpty((string) Settings.Default["KeyLayout"]))
            {
                if (keyLayouts.Any(kl => kl.Name == "두벌식 표준"))
                {
                    Settings.Default["KeyLayout"] = "두벌식 표준";
                }
                else
                {
                    Settings.Default["KeyLayout"] = keyLayouts[0].Name;
                }
            }

            Settings.Default.Save();
            CurrentKeyLayout = GetCurrentKeyLayout();

            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(keyLayouts);
            settingsWindow.ShowDialog();

            keyLayouts = settingsWindow.KeyLayouts;
            CurrentKeyLayout = GetCurrentKeyLayout();

            KeyPracticeMenu.keyLayoutBox.LoadKeyLayout();
        }
    }
}