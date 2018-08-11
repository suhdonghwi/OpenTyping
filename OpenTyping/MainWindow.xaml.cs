﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private IList<KeyLayout> keyLayouts;

        public static KeyLayout CurrentKeyLayout { get; private set; }

        public MainWindow()
        {
            this.DataContext = this;

            if (string.IsNullOrEmpty((string)Settings.Default["KeyLayoutDataDir"]))
            {
                string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string layoutsDirectory = Path.Combine(exeDirectory, "layouts");

                Settings.Default["KeyLayoutDataDir"] = layoutsDirectory;
            }

            keyLayouts = KeyLayout.LoadFromDirectory((string)Settings.Default["KeyLayoutDataDir"]);

            if (keyLayouts.Count == 0)
            {
                MessageBox.Show("경로 " + (string)Settings.Default["KeyLayoutDataDir"] +
                                "에서 자판 데이터 파일을 찾을 수 없습니다. 해당 경로에 자판 데이터를 생성하고 다시 시도하세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (GetCurrentKeyLayout() == null)
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

            CurrentKeyLayout = GetCurrentKeyLayout();

            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private KeyLayout GetCurrentKeyLayout()
        {
            var layoutName = (string)Settings.Default["KeyLayout"];
            return keyLayouts.FirstOrDefault(keyLayout => keyLayout.Name == layoutName);
        }

        private static void SaveKeyLayout()
        {
            File.WriteAllText(CurrentKeyLayout.Location, JsonConvert.SerializeObject(CurrentKeyLayout));
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            SaveKeyLayout();
            Settings.Default.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(keyLayouts);
            settingsWindow.ShowDialog();

            keyLayouts = settingsWindow.KeyLayouts;

            SaveKeyLayout();
            CurrentKeyLayout = GetCurrentKeyLayout();

            KeyPracticeMenu.KeyLayoutBox.LoadKeyLayout();

            var binding = new Binding
            {
                Path = new PropertyPath("Stats.MostIncorrect.Key"),
                Source = CurrentKeyLayout,
                Converter = new KeyPosToKeyConverter()
            };

            HomeMenu.MostIncorrectKey.SetBinding(KeyBox.KeyProperty, binding);
        }
    }
}