using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenTyping
{
    /// <summary>
    /// KeyPracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPracticeMenu : UserControl
    {
        public KeyPracticeMenu()
        {
            InitializeComponent();
            keyLayoutBox.PressKeys();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var keyPracticeWindow = new KeyPracticeWindow(keyLayoutBox.PressedKeys());
            keyPracticeWindow.ShowDialog();
        }
    }
}
