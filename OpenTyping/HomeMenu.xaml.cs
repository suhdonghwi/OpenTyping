using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenTyping
{
    /// <summary>
    ///     HomeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomeMenu : UserControl
    {
        public HomeMenu()
        {
            InitializeComponent();
        }
    }

    public class KeyPosToKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is null ? new Key() { KeyData = "(아직 없음)" } : MainWindow.CurrentKeyLayout[(KeyPos)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}