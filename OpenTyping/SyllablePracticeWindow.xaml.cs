using MahApps.Metro.Controls;

namespace OpenTyping
{
    /// <summary>
    /// SyllablePracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SyllablePracticeWindow : MetroWindow
    {
        private readonly string syllablesList;

        public SyllablePracticeWindow(string syllablesList)
        {
            InitializeComponent();

            this.syllablesList = syllablesList;
        }
    }
}
