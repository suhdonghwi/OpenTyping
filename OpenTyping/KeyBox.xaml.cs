using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenTyping
{
    /// <summary>
    /// KeyBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyBox : UserControl
    {
        public Brush KeyColor
        {
            get => (Brush)GetValue(KeyColorProperty);
            set => SetValue(KeyColorProperty, value);
        }
        public static readonly DependencyProperty KeyColorProperty =
            DependencyProperty.Register("KeyColor", typeof(Brush), typeof(KeyBox), new PropertyMetadata(Brushes.White));

        public Brush ShadowColor
        {
            get => (Brush)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }
        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Brush), typeof(KeyBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(206, 212, 218))));

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyBox));

        public bool Pressed { get; private set; } = false;

        private const double PressDiff = 0.0000001;
        private Brush defaultKeyColor;
        private Brush defaultShadowColor;

        private static readonly Brush CorrectKeyColor = new SolidColorBrush(Color.FromRgb(140, 233, 154));
        private static readonly Brush CorrectKeyShadowColor = new SolidColorBrush(Color.FromRgb(105, 219, 124));

        private static readonly Brush IncorrectKeyColor = new SolidColorBrush(Color.FromRgb(255, 168, 168));
        private static readonly Brush IncorrectKeyShadowColor = new SolidColorBrush(Color.FromRgb(255, 135, 135));

        public KeyBox()
        {
            InitializeComponent();

            defaultKeyColor = KeyColor;
            defaultShadowColor = ShadowColor;
            handPopup.IsOpen = false;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            defaultKeyColor = KeyColor;
            defaultShadowColor = ShadowColor;

            Uri resourceUri = new Uri("Resources/imgs/" + Key.FingerPosition + ".png", UriKind.Relative);
            handImg.Source = new BitmapImage(resourceUri);
        }

        private void Press(Brush keyColor, Brush shadowColor, bool isHandPopup = false)
        {
            if (!Pressed)
            {
                KeyTop.Height += PressDiff;
                Canvas.SetTop(KeyTop, PressDiff);
                KeyBack.Height -= PressDiff;
                Canvas.SetTop(KeyBack, PressDiff);
            }

            KeyColor = keyColor;
            ShadowColor = shadowColor;
            handPopup.IsOpen = isHandPopup;
            if (handPopup.IsOpen)
            {
                // Re-positioning on Spacebar
                if (Key.KeyData == " ")
                {
                    handPopup.VerticalOffset = 0;
                }

                // Re-positioning when KeyPracticeWindow LocChanged
                handPopup.HorizontalOffset += 1;
                handPopup.HorizontalOffset -= 1;
            }

            Pressed = true;
        }

        private void Press(bool Press, bool isHandPopup = false)
        {
            if (!Pressed && Press)
            {
                KeyTop.Height += PressDiff;
                Canvas.SetTop(KeyTop, PressDiff);
                KeyBack.Height -= PressDiff;
                Canvas.SetTop(KeyBack, PressDiff);
            }
            handPopup.IsOpen = isHandPopup;
            if (handPopup.IsOpen)
            {
                // Re-positioning on Spacebar
                if (Key.KeyData == " ")
                {
                    handPopup.VerticalOffset = 0;
                }

                // Re-positioning when KeyPracticeWindow LocChanged
                handPopup.HorizontalOffset += 1;
                handPopup.HorizontalOffset -= 1;
            }

            Pressed = true;
        }
        public void PressCorrect( bool isHandPopup = false)
        {
            Press(CorrectKeyColor, CorrectKeyShadowColor, isHandPopup); 
        }

        public void PressCorrect(bool isButtonClicked, bool isHandPopup = false)
        {
            Press(isButtonClicked, isHandPopup);
        }
        public void PressIncorrect()
        {
            Press(IncorrectKeyColor, IncorrectKeyShadowColor);
        }

        public void Release()
        {
            if (Pressed)
            {
                KeyTop.Height -= PressDiff;
                Canvas.SetTop(KeyTop, 0);
                KeyBack.Height += PressDiff;
                Canvas.SetTop(KeyBack, 0);
            }
            
            KeyColor = defaultKeyColor;
            ShadowColor = defaultShadowColor;
            handPopup.IsOpen = false;

            Pressed = false;
        }

        public void Release(bool pressed)
        {
            if (Pressed && pressed)
            {
                KeyTop.Height -= PressDiff;
                Canvas.SetTop(KeyTop, 0);
                KeyBack.Height += PressDiff;
                Canvas.SetTop(KeyBack, 0);
                handPopup.IsOpen = false;
                Pressed = false;
            }
            else
            {
                KeyTop.Height -= PressDiff;
                Canvas.SetTop(KeyTop, 0);
                KeyBack.Height += PressDiff;
                Canvas.SetTop(KeyBack, 0);
                KeyColor = defaultKeyColor;
                ShadowColor = defaultShadowColor;
                handPopup.IsOpen = false;
                Pressed = false;
            }
        }

        public void PressToggle()
        {
            if (Pressed) Release();
            else PressCorrect();
        }
    }
}
