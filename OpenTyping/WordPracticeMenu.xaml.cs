using System.Windows;

namespace OpenTyping
{
    /// <summary>
    /// WordPracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WordPracticeMenu : PracticeMenuBase
    {
        public WordPracticeMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            WordPracticeWindow wordPracticeWindow = new WordPracticeWindow();
            wordPracticeWindow.ShowDialog();
        }
    }
}
