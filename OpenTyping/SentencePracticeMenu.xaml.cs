using System.Windows;
using MahApps.Metro.Controls;
using OpenTyping.Resources.Lang;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SentencePracticeMenu : PracticeMenuBase
    {
        private bool IsRandom { get; set; }

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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    this.IsRandom = true;
                }
                else
                {
                    this.IsRandom = false;
                }
            }
        }
    }
}
