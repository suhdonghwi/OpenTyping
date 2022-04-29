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
        private List<StackPanel> keyRows;
        bool handled;

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

            keyRows = new List<StackPanel> { NumberRow, FirstRow, SecondRow, ThirdRow, ForthRow };
            for (int i = 0; i < keyRows.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {

                    if (i == 1 && j == keyLayout[i].Count - 1)
                    {
                        keyLayout[i][j].Width = 70;
                    }
                    else if (i == keyRows.Count - 1 && j == 0) // Spacebar
                    {
                        keyLayout[i][j].Width = 400;
                        keyLayout[i][j].KeyDown += Spacebar_KeyDown;
                       
                    }

                    keyRows[i].Children.Add(keyLayout[i][j]);
                }
            }

            if (Clickable)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                    new Action(PressDefaultKeys));
            }


        }

        private void Spacebar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                handled = true;
            }
        }

        //The function that allows keyboard be colorfull
        public void ChangeColorOn()
        {
            try
            {
                for (int i = 0; i < keyRows.Count; i++)
                {
                    for (int j = 0; j < keyLayout[i].Count; j++)
                    {
                        if (i == 0 && j == 0 || i == 0 && j ==1 || i == 0 && j ==10 || i == 0 && j == 11 || i == 0 && j == 12 ||
                            i == 1 && j==0 || i==1 && j==9 || i == 1 && j == 10 || i == 1 && j == 11 || i == 1 && j == 12 ||
                            i == 2 && j == 0 || i == 2 && j == 9 || i==2 && j==10 || 
                            i == 3 && j == 0 || i == 3 && j == 9)
                        {
                            keyLayout[i][j].KeyColor = (Brush)(new BrushConverter().ConvertFrom("#ad7ea8"));
                            keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#ad7ea8"));

                        }

                        else if (i == 0 && j == 2 || i ==0 && j == 9 ||
                            i == 1 && j == 1 || i == 1 && j == 8 ||
                            i == 2 && j == 1 || i == 2 && j == 8 ||
                            i == 3 && j == 1 || i == 3 && j == 8) 
                        {
                            keyLayout[i][j].KeyColor = (Brush)(new BrushConverter().ConvertFrom("#729fce"));
                            keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#729fce"));

                        }

                        else if ( i == 0 && j == 3 || i == 0 && j == 8 ||
                            i == 1 && j == 2 || i == 1 && j == 7 ||
                            i == 2 && j == 2 || i == 2 && j == 7 ||
                            i == 3 && j ==2 || i == 3 && j == 7)
                        {
                            keyLayout[i][j].KeyColor = (Brush)(new BrushConverter().ConvertFrom("#72d314"));
                            keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#72d314"));

                        }

                        else if ( i == 0 && j == 4 || i == 0 && j == 5 ||
                            i == 1 && j == 3 || i == 1 && j == 4 ||
                            i == 2 && j == 3 || i == 2 && j == 4 || 
                            i == 3 && j == 3 || i == 3 && j == 4)
                        {
                            keyLayout[i][j].KeyColor = (Brush)(new BrushConverter().ConvertFrom("#fcaf3d"));
                            keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#fcaf3d"));

                        }

                        else if(i == keyRows.Count - 1 && j == 0)
                        {
                            keyLayout[i][j].KeyColor = Brushes.White;  //Spacebar
                            keyLayout[i][j].ShadowColor = new SolidColorBrush(Color.FromRgb(206, 212, 218));  //Spacebar
                        }

                        else
                        {
                            keyLayout[i][j].KeyColor = (Brush)(new BrushConverter().ConvertFrom("#fce94f"));
                            keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#fce94f"));
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }  
        }

        //The function that disables color functionality of keyboard
        public void ChangeColorOff()
        {
            try
            {
                for (int i = 0; i < keyRows.Count; i++)
                {
                    for (int j = 0; j < keyLayout[i].Count; j++)
                    {
                        //default key color
                        keyLayout[i][j].KeyColor = Brushes.White;
                        keyLayout[i][j].ShadowColor = (Brush)(new BrushConverter().ConvertFrom("#ced4da"));
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public void PressCorrectKey(KeyPos pos, bool isHandPopup = false)
        {
            keyLayout[pos.Row][pos.Column].PressCorrect(isHandPopup);
        }
         
        public void PressCorrectKey(KeyPos pos, bool isPressed, bool isHandPopup = false)
        {
            keyLayout[pos.Row][pos.Column].PressCorrect(isPressed, isHandPopup);
        }

        public void PressIncorrectKey(KeyPos pos)
        {
            keyLayout[pos.Row][pos.Column].PressIncorrect();
        }

        public void ReleaseKey(KeyPos pos)
        {
            keyLayout[pos.Row][pos.Column].Release();
        }

        public void ReleaseKey(KeyPos pos, bool press)
        {
            keyLayout[pos.Row][pos.Column].Release(press);
            if(press == true)
            ChangeColorOn();
        }


        public void PressDefaultKeys()
        {
            List<KeyPos> defaultKeys = MainWindow.CurrentKeyLayout.DefaultKeys;

            for (int i = 0; i < keyLayout.Count; i++)
            {
                for (int j = 0; j < keyLayout[i].Count; j++)
                {
                    if (defaultKeys.Contains(new KeyPos(i, j)))
                    {
                        keyLayout[i][j].PressCorrect();
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
            MainWindow.CurrentKeyLayout.DefaultKeys = PressedKeys();
        }
    }
}
