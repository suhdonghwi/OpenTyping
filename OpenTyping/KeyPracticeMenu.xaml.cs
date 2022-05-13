using MahApps.Metro.Controls;
using OpenTyping.Resources.Lang;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OpenTyping
{
    /// <summary>
    /// KeyPracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPracticeMenu : UserControl
    {
        private bool NoShiftMode { get; set; } = true;

        private KeyPos[] finger1 = {
                new KeyPos(4,0)
        };

        private KeyPos[] finger2 = {
                new KeyPos(0,4), 
                new KeyPos(0,5),
                new KeyPos(0,6),
                new KeyPos(0,7),
                new KeyPos(1,3),
                new KeyPos(1,4),
                new KeyPos(1,5),
                new KeyPos(1,6),
                new KeyPos(2,3),
                new KeyPos(2,4),
                new KeyPos(2,5),
                new KeyPos(2,6),
                new KeyPos(3,3),
                new KeyPos(3,4),
                new KeyPos(3,5),
                new KeyPos(3,6),
        };

        private KeyPos[] finger3 = {
                new KeyPos(0,3),
                new KeyPos(0,8),
                new KeyPos(1,2),
                new KeyPos(1,7),
                new KeyPos(2,2),
                new KeyPos(2,7),
                new KeyPos(3,2),
                new KeyPos(3,7),
        };

        private KeyPos[] finger4 = {
                new KeyPos(0,2),
                new KeyPos(0,9),
                new KeyPos(1,1),
                new KeyPos(1,8),
                new KeyPos(2,1),
                new KeyPos(2,8),
                new KeyPos(3,1),
                new KeyPos(3,8),
        };

        private KeyPos[] finger5 = {
                new KeyPos(0,0),
                new KeyPos(0,1),
                new KeyPos(0,10),
                new KeyPos(0,11),
                new KeyPos(0,12),
                new KeyPos(1,0),
                new KeyPos(1,9),
                new KeyPos(1,10),
                new KeyPos(1,11),
                new KeyPos(1,12),
                new KeyPos(2,0),
                new KeyPos(2,9),
                new KeyPos(2,10),
                new KeyPos(3,0),
                new KeyPos(3,9),
        };

        List<KeyPos> finger1keys = null;
        List<KeyPos> finger2keys = null;
        List<KeyPos> finger3keys = null;
        List<KeyPos> finger4keys = null;
        List<KeyPos> finger5keys = null;

        public KeyPracticeMenu()
        {
            InitializeComponent();

            finger1keys = new List<KeyPos>(finger1);
            finger2keys = new List<KeyPos>(finger2);
            finger3keys = new List<KeyPos>(finger3);
            finger4keys = new List<KeyPos>(finger4);
            finger5keys = new List<KeyPos>(finger5);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            List<KeyPos> pressedKeys = KeyLayoutBox.PressedKeys();

            if (pressedKeys.Count <= 1)
            {
                MessageBox.Show(LangStr.ErrMsg1,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            var keyPracticeWindow = new KeyPracticeWindow(pressedKeys, NoShiftMode);
            keyPracticeWindow.ShowDialog();
        }

        private void ToggleShift_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    this.NoShiftMode = false;
                }
                else
                {
                    this.NoShiftMode = true;
                }
            }
        }

        private void ToggleKeys_Checked(object sender, RoutedEventArgs e)
        {
            List<KeyPos> Keys = null;

            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton != null)
            {
                switch (toggleButton.Name)
                {
                    case "Finger1":
                        Keys = finger1keys;
                        break;
                    case "Finger2":
                        Keys = finger2keys;
                        break;
                    case "Finger3":
                        Keys = finger3keys;
                        break;
                    case "Finger4":
                        Keys = finger4keys;
                        break;
                    case "Finger5":
                        Keys = finger5keys;
                        break;
                }

                if (toggleButton.IsChecked == true)
                {

                    if (KeyLayoutBox != null)
                    {
                        KeyLayoutBox.PressDefaultKeys(Keys);
                    }
                }
                else
                {

                    if (KeyLayoutBox != null)
                    {
                        KeyLayoutBox.ReleaseKeys(Keys);
                    }
                }
            }
        }
    }
}
