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
            DependencyProperty.Register("ShadowColor", typeof(Brush), typeof(KeyBox), new PropertyMetadata(Brushes.LightGray));

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

        public void Press(Brush keyColor, Brush shadowColor)
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

        public void PressToggle(Brush releaseKeyColor, Brush releaseShadowColor, Brush pressKeyColor, Brush pressShadowColor)
        {
            if (Pressed) Release();
            else Press(pressKeyColor, pressShadowColor);
        }
    }
}
