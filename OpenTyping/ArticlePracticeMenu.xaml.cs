using OpenTyping.Resources.Lang;
using System.Windows;

namespace OpenTyping
{
    /// <summary>
    /// SentencePracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ArticlePracticeMenu : PracticeMenuBase
    {
        public ArticlePracticeMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPracticeData == null)
            {
                MessageBox.Show(LangStr.InfoMsg1,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            var articlePracticeWindow = new ArticlePracticeWindow(selectedPracticeData);
            articlePracticeWindow.ShowDialog();
        }
    }
}
