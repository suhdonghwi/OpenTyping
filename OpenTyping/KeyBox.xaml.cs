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
            DependencyProperty.Register("ShadowColor", typeof(Brush), typeof(KeyBox), 
                new PropertyMetadata((Brush)Application.Current.FindResource("DefaultKeyShadowColor")));

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyBox));

        public bool Pressed { get; private set; } = false;

        private const double PressDiff = 1.7;
        private Brush defaultKeyColor;
        private Brush defaultShadowColor;

        private static readonly Brush IncorrectKeyColor = (Brush)Application.Current.FindResource("IncorrectKeyColor");
        private static readonly Brush IncorrectKeyShadowColor = (Brush)Application.Current.FindResource("IncorrectKeyShadowColor");

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

        private Brush FindKeyColor(string FingerPos)
        {
            return (Brush)Application.Current.FindResource(FingerPos);
        }

        public void PressCorrect(bool isHandPopup = false, bool isColored = false)
        {
            Brush CorrectKeyColor = (Brush)Application.Current.FindResource("CorrectKeyColor");
            Brush CorrectKeyShadowColor = (Brush)Application.Current.FindResource("CorrectKeyShadowColor");

            if (isColored && Key.KeyData != " ") // If not, Spacebar
            {
                CorrectKeyColor = FindKeyColor(Key.FingerPosition);
                CorrectKeyShadowColor = FindKeyColor(Key.FingerPosition);
            }

            Press(CorrectKeyColor, CorrectKeyShadowColor, isHandPopup);
        }

        public void PressIncorrect()
        {
            Press(IncorrectKeyColor, IncorrectKeyShadowColor);
        }

        public void Release(bool isColored = false)
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
            if (isColored && (Key.KeyData != " ")) // If not, Spacebar
            {
                KeyColor = FindKeyColor(Key.FingerPosition);
                ShadowColor = FindKeyColor(Key.FingerPosition);
            }
           
            handPopup.IsOpen = false;

            Pressed = false;
        }

        public void PressToggle()
        {
            if (Pressed) Release();
            else PressCorrect(false, true);
        }

        public void ToggleColor(bool colored)
        {
            if (colored && (Key.KeyData != " ")) // If not, Spacebar
            {
                KeyColor = FindKeyColor(Key.FingerPosition);
                ShadowColor = FindKeyColor(Key.FingerPosition);
            }
            else
            {
                KeyColor = Brushes.White;
                ShadowColor = (Brush)Application.Current.FindResource("DefaultKeyShadowColor");
            }
        }
    }
}
