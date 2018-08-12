using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using OpenTyping.Properties;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeMenu : UserControl
    {
        public ObservableCollection<PracticeData> PracticeDataList { get; }

        public SentencePracticeMenu()
        {
            InitializeComponent();

            this.DataContext = this;

            PracticeDataList =
                new ObservableCollection<PracticeData>(
                    PracticeData.LoadFromDirectory((string)Settings.Default[MainWindow.PracticeDataDir]));
        }
    }
}
