using System.Windows.Input;
using MahApps.Metro.Controls;

namespace OpenTyping
{
    /// <summary>
    /// ArticlePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ArticlePracticeWindow : MetroWindow
    {
        public ArticlePracticeWindow()
        {
            InitializeComponent();
        }

        private void LineTextBox_PreviewExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
