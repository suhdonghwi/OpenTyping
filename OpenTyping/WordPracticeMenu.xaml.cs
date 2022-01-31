using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OpenTyping.Properties;
using OpenTyping.Resources.Lang;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenTyping
{
    /// <summary>
    /// WordPracticeMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WordPracticeMenu : UserControl
    {
        private readonly Rank Rank;
        private User newUser;

        public WordPracticeMenu()
        {
            InitializeComponent();
            SetTextBylanguage();

            Rank = new Rank();
            Rank.users.Sort();
            LVusers.ItemsSource = Rank.users;
        }

        private void SetTextBylanguage()
        {
            TBname.Text = (string)Settings.Default["Name"];
            TBorg.Text = (string)Settings.Default["Org"];
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default["Name"] = TBname.Text;
            Settings.Default["Org"] = TBorg.Text;

            WordPracticeWindow wordPracticeWindow = new WordPracticeWindow();
            wordPracticeWindow.Closed += new EventHandler(WordPracticeWindow_Closed);
            wordPracticeWindow.RtnNewUser += value => this.newUser = value;
            wordPracticeWindow.ShowDialog();
        }

        private void WordPracticeWindow_Closed(object sender, EventArgs e)
        {
            this.FinishPracticeAsync();
        }

        private async void FinishPracticeAsync()
        {
            string congMsg = "";

            int curPos = await Rank.Add(newUser);
            LVusers.Items.Refresh();

            if (curPos >= 0 && curPos <= 9)
            {
                congMsg = LangStr.CongratMsg;
                LVusers.SelectedIndex = curPos;
            }

            await this.TryFindParent<MetroWindow>().ShowMessageAsync(LangStr.FinishedPrac + " " + congMsg,
                                        LangStr.LastSpeed + ": " + newUser.Speed + ", " 
                                        + LangStr.Accuracy + ": " + newUser.Accuracy + "%" + ", "
                                        + LangStr.WordCount + ": " + newUser.Count + ", " 
                                        + LangStr.ElapsedTime + ": " 
                                        + newUser.Time.ToString(CultureInfo.GetCultureInfo("en-US")),
                                         MessageDialogStyle.Affirmative,
                                         new MetroDialogSettings { AnimateHide = false });
        }
    }

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return (index + 1).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
