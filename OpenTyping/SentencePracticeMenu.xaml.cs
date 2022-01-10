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
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeMenu : PracticeMenuBase
    {
        public bool IsRandom { get; set; }

        public SentencePracticeMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPracticeData is null)
            {
                MessageBox.Show(LangStr.InfoMsg1,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            var sentencePracticeWindow = new SentencePracticeWindow(selectedPracticeData, IsRandom);
            sentencePracticeWindow.ShowDialog();
        }
    }
}
