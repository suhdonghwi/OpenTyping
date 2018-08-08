using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private static readonly Brush DefaultKeyColor = Brushes.White;
        private static readonly Brush DefaultKeyShadowColor = Brushes.LightGray;

        private static readonly Brush PressedKeyColor = Brushes.LightGreen;
        private static readonly Brush PressedKeyShadowColor = new SolidColorBrush(Color.FromRgb(100, 198, 100));

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

            if (Clickable) PressDefaultKeys(PressedKeyColor, PressedKeyShadowColor);
        }

        public void PressKey(KeyPos pos, Brush keyColor, Brush shadowColor)
        {
            keyLayout[pos.Row][pos.Column].Press(keyColor, shadowColor);
        }

        public void ReleaseKey(KeyPos pos)
        {
            keyLayout[pos.Row][pos.Column].Release();
        }

        public void PressDefaultKeys(Brush keyColor, Brush shadowColor)
        {
            List<KeyPos> defaultKeys = MainWindow.CurrentKeyLayout.DefaultKeys;

            for (int i = 0; i < keyLayout.Count(); i++)
            {
                for (int j = 0; j < keyLayout[i].Count(); j++)
                {
                    if (defaultKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].Press(keyColor, shadowColor);
                    }
                }
            }
        }

        public void PressShift(Brush keyColor, Brush shadowColor)
        {
            LShiftKey.Press(keyColor, shadowColor);
            RShiftKey.Press(keyColor, shadowColor);
        }

        public void ReleaseShift()
        {
            LShiftKey.Release();
            RShiftKey.Release();
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
            ((KeyBox)sender).PressToggle(DefaultKeyColor, DefaultKeyShadowColor, PressedKeyColor, PressedKeyShadowColor);
            MainWindow.CurrentKeyLayout.DefaultKeys = PressedKeys();
        }
    }
}
