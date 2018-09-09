using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ArticlePracticeMenu : PracticeMenuBase
    {
        public bool IsRandom { get; set; }

        public ArticlePracticeMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPracticeData is null)
            {
                MessageBox.Show("연습하실 연습 데이터를 선택해주세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            var articlePracticeWindow = new ArticlePracticeWindow(selectedPracticeData);
            articlePracticeWindow.ShowDialog();
        }
    }
}
