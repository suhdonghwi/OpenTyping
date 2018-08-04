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

        public bool Clickable
        {
            get => (bool)GetValue(ClickableProperty);
            set => SetValue(ClickableProperty, value);
        }
        public static readonly DependencyProperty ClickableProperty =
            DependencyProperty.Register("Clickable", typeof(bool), typeof(KeyLayoutBox), new PropertyMetadata(true));

        public KeyLayoutBox()
        {
            InitializeComponent();
            this.Loaded += KeyLayoutBox_Loaded; 
        }

        private void KeyLayoutBox_Loaded(object sender, RoutedEventArgs e)
        {
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

                    var keyBox = new KeyBox
                    {
                        Key = key,
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 2, 0)
                    };

                    if (Clickable)
                    {
                        keyBox.MouseDown += KeyBox_MouseDown;
                    }

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

            if (Clickable) PressKeys();
        }

        public void PressKeys()
        {
            List<KeyPos> pressingKeys = MainWindow.CurrentKeyLayout.Pressing;

            for (int i = 0; i < keyLayout.Count(); i++)
            {
                for (int j = 0; j < keyLayout[i].Count(); j++)
                {
                    if (pressingKeys.Contains(new KeyPos(i, j)))
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
            ((KeyBox)sender).PressToggle();
            MainWindow.CurrentKeyLayout.Pressing = PressedKeys();
        }
    }
}
