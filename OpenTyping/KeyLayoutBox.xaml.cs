using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// KeyLayoutBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyLayoutBox : UserControl
    {
        private List<List<KeyBox>> keyLayout;

        public bool ClickToggle { get; set; } = false;

        public KeyLayoutBox()
        {
            InitializeComponent();
            LoadKeyLayout();
        }

        public void LoadKeyLayout()
        {
            NumberRow.Children.Clear();
            FirstRow.Children.Clear();
            SecondRow.Children.Clear();
            ThirdRow.Children.Clear();

            keyLayout = new List<List<KeyBox>>();

            for (int i = 0; i < MainWindow.CurrentKeyLayout.KeyLayoutData.Count(); i++)
            {
                var keyBoxes = new List<KeyBox>();

                for (int j = 0; j < MainWindow.CurrentKeyLayout.KeyLayoutData[i].Count(); j++)
                {
                    Key key = MainWindow.CurrentKeyLayout.KeyLayoutData[i][j];

                    KeyBox keyBox = new KeyBox
                    {
                        Key = key,
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 2, 0)
                    };

                    keyBox.MouseDown += KeyBox_MouseDown;
                    keyBoxes.Add(keyBox);
                }

                keyLayout.Add(keyBoxes);
            }

            var keyRows = new List<StackPanel> { NumberRow, FirstRow, SecondRow, ThirdRow };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (i == 1 && j == keyLayout[i].Count - 1)
                    {
                        keyLayout[i][j].Width = 70;
                    }
                    keyRows[i].Children.Add(keyLayout[i][j]);
                }
            }

            PressKeys();
        }

        public void PressKeys()
        {
            var unpressKeys = new List<KeyPos>
            {
                new KeyPos(0, 0), new KeyPos(0, 11), new KeyPos(0, 12),
                new KeyPos(1, 10), new KeyPos(1, 11), new KeyPos(1, 12),
                new KeyPos(2, 9), new KeyPos(2, 10),
                new KeyPos(3, 7), new KeyPos(3, 8), new KeyPos(3, 9)
            };

            for (int i = 0; i < keyLayout.Count(); i++)
            {
                for (int j = 0; j < keyLayout[i].Count(); j++)
                {
                    if (!unpressKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].PressToggle();
                    }
                }
            }
        }

        public List<KeyPos> PressedKeys()
        {
            var result = new List<KeyPos>();

            for (int i=0; i<keyLayout.Count; i++)
            {
                for (int j=0; j<keyLayout[i].Count; j++)
                {
                    if (keyLayout[i][j].Pressed)
                    {
                        result.Add(new KeyPos(i, j));
                    }
                }
            }

            return result;
        }

        private void KeyBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClickToggle)
            {
                ((KeyBox)sender).PressToggle();
            }
        }
    }
}
