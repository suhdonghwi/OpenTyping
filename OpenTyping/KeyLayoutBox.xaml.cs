using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenTyping
{
    /// <summary>
    /// KeyLayoutBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyLayoutBox : UserControl
    {
        private List<List<KeyBox>> keyLayout;
        private List<KeyPos> selectedKeys;

        public bool Clickable
        {
            get => (bool)GetValue(ClickableProperty);
            set => SetValue(ClickableProperty, value);
        }
        public static readonly DependencyProperty ClickableProperty =
            DependencyProperty.Register("Clickable", typeof(bool), typeof(KeyLayoutBox), new PropertyMetadata(true));

        public double WidthRatio
        {
            get => (double)GetValue(WidthRatioProperty);
            set => SetValue(WidthRatioProperty, value);
        }
        public static readonly DependencyProperty WidthRatioProperty =
            DependencyProperty.Register("WidthRatio", typeof(double), typeof(KeyLayoutBox), new PropertyMetadata(1.0));

        public KeyLayoutBox()
        {
            InitializeComponent();
            selectedKeys = MainWindow.CurrentKeyLayout.DefaultKeys;
            Loaded += KeyLayoutBox_Loaded; 
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
            ForthRow.Children.Clear();

            keyLayout = new List<List<KeyBox>>();

            foreach (IList<Key> keyRow in MainWindow.CurrentKeyLayout.KeyLayoutData)
            {
                var keyBoxes = new List<KeyBox>();

                foreach (Key key in keyRow)
                {
                    var keyBox = new KeyBox
                    {
                        Key = key,
                        Width = 50 * WidthRatio,
                        Height = 50 * WidthRatio,
                        Margin = new Thickness(0, 0, 2, 0),
                    };
                    keyBox.KeyText.FontSize = 15 * WidthRatio;
                    keyBox.ShiftKeyText.FontSize = 15 * WidthRatio;

                    if (Clickable)
                    {
                        keyBox.MouseDown += KeyBox_MouseDown;
                    }

                    keyBoxes.Add(keyBox);
                }

                keyLayout.Add(keyBoxes);
            }

            var keyRows = new List<StackPanel> { NumberRow, FirstRow, SecondRow, ThirdRow, ForthRow };
            for (int i = 0; i < keyRows.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (i == 1 && j == keyLayout[i].Count - 1)
                    {
                        keyLayout[i][j].Width = 70 * WidthRatio;
                    }
                    else if (i == keyRows.Count - 1 && j == 0) // Spacebar
                    {
                        keyLayout[i][j].Width = 400 * WidthRatio;
                    }

                    keyRows[i].Children.Add(keyLayout[i][j]);
                }
            }

            if (Clickable)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                    new Action(PressDefaultKeys));
            }

            // Mode Keys
            BackspaceKey.Width = 100 * WidthRatio;
            BackspaceKey.KeyText.FontSize = 15 * WidthRatio;

            TabKey.Width = 80 * WidthRatio;
            TabKey.KeyText.FontSize = 15 * WidthRatio;

            CapsLockKey.Width = 100 * WidthRatio;
            CapsLockKey.KeyText.FontSize = 15 * WidthRatio;

            EnterKey.Width = 103 * WidthRatio;
            EnterKey.KeyText.FontSize = 15 * WidthRatio;

            LShiftKey.Width = 130 * WidthRatio;
            LShiftKey.KeyText.FontSize = 15 * WidthRatio;

            RShiftKey.Width = 125 * WidthRatio;
            RShiftKey.KeyText.FontSize = 15 * WidthRatio;
        }

        public void PressCorrectKey(KeyPos pos, bool isHandPopup = false)
        {
            keyLayout[pos.Row][pos.Column].PressCorrect(isHandPopup);
        }

        public void PressIncorrectKey(KeyPos pos)
        {
            keyLayout[pos.Row][pos.Column].PressIncorrect();
        }

        public void ReleaseKey(KeyPos pos, bool isColored = false)
        {
            keyLayout[pos.Row][pos.Column].Release(isColored);
        }

        public void ReleaseKeys (List<KeyPos> keys)
        {
            List<KeyPos> defaultKeys = keys;

            for (int i = 0; i < keyLayout.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (defaultKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].Release();
                    }
                }
            }
            selectedKeys = PressedKeys();
        }

        public void PressDefaultKeys()
        {
            for (int i = 0; i < keyLayout.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (selectedKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].PressCorrect(false, true);
                    }
                }
            }
        }

        public void PressDefaultKeys(List<KeyPos> keys)
        {
            List<KeyPos> defaultKeys = keys;

            for (int i = 0; i < keyLayout.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (defaultKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].PressCorrect(false, true);
                    }
                }
            }
            selectedKeys = PressedKeys();
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

        public void ToggleColoredKeys(bool colored, KeyPos currentKey)
        {
            for (int i = 0; i < keyLayout.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    // Don't fill a color in case of current practicing key
                    if (!(currentKey.Row == i && currentKey.Column == j))
                    {
                        keyLayout[i][j].ToggleColor(colored);
                    }
                }
            }
        }

        public void PressShiftKey()
        {
            LShiftKey.KeyColor = RShiftKey.KeyColor = (Brush)Application.Current.FindResource("CorrectKeyColor");
            LShiftKey.ShadowColor = RShiftKey.ShadowColor = (Brush)Application.Current.FindResource("CorrectKeyColor");
        }

        public void ReleaseShiftKey()
        {
            LShiftKey.KeyColor = RShiftKey.KeyColor = Brushes.White;
            LShiftKey.ShadowColor = RShiftKey.ShadowColor = (Brush)Application.Current.FindResource("DefaultKeyShadowColor");
        }

        private void KeyBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((KeyBox)sender).PressToggle();
            selectedKeys = PressedKeys();
        }
    }
}
