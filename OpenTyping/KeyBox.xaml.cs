using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        private const double PressDiff = 1.7;
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
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            defaultKeyColor = KeyColor;
            defaultShadowColor = ShadowColor;
        }

        private void Press(Brush keyColor, Brush shadowColor)
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

            Pressed = true;
        }

        public void PressCorrect()
        {
            Press(CorrectKeyColor, CorrectKeyShadowColor);
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

            Pressed = false;
        }

        public void PressToggle()
        {
            if (Pressed) Release();
            else PressCorrect();
        }
    }
}
