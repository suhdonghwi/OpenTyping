using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using OpenTyping.Properties;
using OpenTyping.Resources.Lang;

namespace OpenTyping
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Volume volume;

        public static KeyLayout CurrentKeyLayout { get; private set; }

        public const string KeyLayoutDataDirStr = "KeyLayoutDataDir";
        public const string KeyLayoutStr = "KeyLayout";
        public const string PracticeDataDirStr = "PracticeDataDir";
        public const string ProgramLang = "ProgramLang";

        public MainWindow()
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (exeDirectory is null)
            {
                MessageBox.Show(LangStr.ErrMsg7,
                                LangStr.AppName,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (string.IsNullOrEmpty((string)Settings.Default[KeyLayoutDataDirStr]))
            {
                string layoutsDirectory = Path.Combine(exeDirectory, "layouts");
                Settings.Default[KeyLayoutDataDirStr] = layoutsDirectory;
            }

            try
            {
                var keyLayouts =
                    new List<KeyLayout>(KeyLayout.LoadFromDirectory((string)Settings.Default[KeyLayoutDataDirStr]));

                var layoutName = (string)Settings.Default[KeyLayoutStr];
                KeyLayout currentKeylayout = keyLayouts.FirstOrDefault(keyLayout => keyLayout.Name == layoutName);

                if (currentKeylayout == null)
                {
                    KeyLayout qwertyLayout = keyLayouts.Find(keyLayout => keyLayout.Name == "QWERTY");

                    if (qwertyLayout != null)
                    {
                        Settings.Default[KeyLayoutStr] = qwertyLayout.Name;
                        CurrentKeyLayout = qwertyLayout;
                    }
                    else
                    {
                        Settings.Default[KeyLayoutStr] = keyLayouts[0].Name;
                        CurrentKeyLayout = keyLayouts[0];
                    }
                }
                else
                {
                    CurrentKeyLayout = currentKeylayout;
                }
            }
            catch (Exception ex)
            {
                if (ex is KeyLayoutLoadFail || ex is InvalidKeyLayoutDataException)
                {
                    MessageBox.Show(ex.Message, LangStr.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }

            if (string.IsNullOrEmpty((string)Settings.Default[PracticeDataDirStr]))
            {
                string dataDirectory = Path.Combine(exeDirectory, "data");
                Settings.Default[PracticeDataDirStr] = dataDirectory;
            }

            if (string.IsNullOrEmpty((string)Settings.Default[ProgramLang]))
            {
                Settings.Default[ProgramLang] = "uz"; // App default language
            }

            InitializeComponent();
            this.SetTextBylanguage(Settings.Default[ProgramLang].ToString());

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

            this.volume = (Volume)Settings.Default["Volume"];
            VolumeValToSetIcon(this.volume);
        }

        private void ChangeCulture(string nationCode)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(nationCode);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(nationCode);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSyllablePractice();
            CheckTutorial();

            // Decide a base font size by the screen resolution
            App.BaseFontSize = ((SystemParameters.PrimaryScreenWidth / 12) / 3 * 2) / 5 * 0.7;
            App.BaseFontSize = Math.Round(App.BaseFontSize);
            Debug.WriteLine(App.BaseFontSize, "BaseFontSize");
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            KeyLayout.SaveKeyLayout(CurrentKeyLayout);
            Settings.Default.Save();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            KeyLayout.SaveKeyLayout(CurrentKeyLayout);

            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

            if (settingsWindow.KeyLayoutUpdated || settingsWindow.KeyLayoutDataDirUpdated)
            {
                CurrentKeyLayout = settingsWindow.SelectedKeyLayout;

                KeyPracticeMenu.KeyLayoutBox.LoadKeyLayout();

                var currentKeyLayoutNameBinding = new Binding
                {
                    Path = new PropertyPath("Name"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.CurrentKeyLayoutName.SetBinding(TextBlock.TextProperty, currentKeyLayoutNameBinding);

                var currentKeyLayoutCharBinding = new Binding
                {
                    Path = new PropertyPath("Character"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.CurrentKeyLayoutChar.SetBinding(TextBlock.TextProperty, currentKeyLayoutCharBinding);

                var mostIncorrectBinding = new Binding
                {
                    Path = new PropertyPath("Stats.MostIncorrect.Key"),
                    Source = CurrentKeyLayout,
                    Converter = new KeyPosToKeyConverter()
                };
                HomeMenu.MostIncorrectKey.SetBinding(KeyBox.KeyProperty, mostIncorrectBinding);

                var averageSpeedBinding = new Binding
                {
                    Path = new PropertyPath("Stats.AverageTypingSpeed"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.AverageTypingSpeed.SetBinding(TextBlock.TextProperty, averageSpeedBinding);

                var averageAccuracyBinding = new Binding
                {
                    Path = new PropertyPath("Stats.AverageAccuracy"),
                    Source = CurrentKeyLayout,
                    StringFormat = "{0}%"
                };
                HomeMenu.AverageAccuracy.SetBinding(TextBlock.TextProperty, averageAccuracyBinding);

                var sentencePracticeCountBinding = new Binding
                {
                    Path = new PropertyPath("Stats.SentencePracticeCount"),
                    Source = CurrentKeyLayout,
                };
                HomeMenu.SentencePracticeCount.SetBinding(TextBlock.TextProperty, sentencePracticeCountBinding);

                CheckSyllablePractice();
            }

            if (settingsWindow.LangUpdated)
            {
                CheckTutorial();
            }

            SentencePracticeMenu.LoadData();
            ArticlePracticeMenu.LoadData();
        }

        private void CheckSyllablePractice()
        {
            if (CurrentKeyLayout.Character == "한글")
            {
                SyllablePracticeTabItem.Visibility = Visibility.Visible;
            }
            else
            {
                SyllablePracticeTabItem.Visibility = Visibility.Collapsed;

                if (SyllablePracticeTabItem.IsSelected)
                {
                    MenuTabControl.SelectedIndex = 0;
                }
            }
        }

        private void CheckTutorial()
        {
            if ((string)Settings.Default[ProgramLang] == "uz")
            {
                TutorialTabItem.Visibility = Visibility.Visible;
            }
            else
            {
                TutorialTabItem.Visibility = Visibility.Collapsed;

                if (TutorialTabItem.IsSelected)
                {
                    MenuTabControl.SelectedIndex = 0;
                }
            }
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            this.volume++;
            if (this.volume == Volume.Up + 1)
            {
                this.volume = Volume.Off;
            }

            VolumeValToSetIcon(this.volume);
            Settings.Default["Volume"] = (int)this.volume;
        }

        private void VolumeValToSetIcon(Volume volume)
        {
            switch (volume)
            {
                case Volume.Off:
                    VolumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.VolumeMuteSolid;
                    break;
                case Volume.Down:
                    VolumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.VolumeDownSolid;
                    break;
                case Volume.Up:
                    VolumeBtnIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.VolumeUpSolid;
                    break;
            }
        }

        public void SetTextBylanguage(string langCode)
        {
            this.ChangeCulture(langCode);

            // Title Bar
            MainTitle.Title = LangStr.AppName;
            MenuSetting.Text = LangStr.Setting;

            // Top Menu
            MenuLbl1.Content = LangStr.Home;
            MenuLbl2.Content = LangStr.KeyPrac;
            MenuLbl3.Content = LangStr.SylPrac;
            MenuLbl4.Content = LangStr.WordPrac;
            MenuLbl5.Content = LangStr.SenPrac;
            MenuLbl6.Content = LangStr.ArtPrac;
            MenuLbl7.Content = LangStr.Tutorial;

            // Home menu
            HomeMenu.MenuName.Text = LangStr.AppName;
            HomeMenu.MenuDesc.Text = LangStr.AppDesc;
            HomeMenu.Tile1.Title = LangStr.MostWrongKey;
            HomeMenu.Tile2.Title = LangStr.AvgSpeed;
            HomeMenu.Tile3.Title = LangStr.AvgAccuracy;
            HomeMenu.Tile4.Title = LangStr.NumPracSen;

            // Key menu
            KeyPracticeMenu.MenuName.Text = LangStr.KeyPrac;
            KeyPracticeMenu.MenuDesc.Text = LangStr.KeyPracDesc;
            KeyPracticeMenu.MenuHelp.Text = LangStr.KeyPracHelp;
            KeyPracticeMenu.SetToggle.Text = LangStr.ExceptShftKey;
            KeyPracticeMenu.StartBtn.Text = LangStr.StartPrac;

            // Syllable menu
            SyllablePracticeMenu.MenuName.Text = LangStr.SylPrac;
            SyllablePracticeMenu.MenuDesc.Text = LangStr.SylPracDesc;
            SyllablePracticeMenu.Start2350Tile.Title = LangStr.Char2350;
            SyllablePracticeMenu.StartModernHangulTile.Title = LangStr.Char11722;
            SyllablePracticeMenu.StartCustomTile.Title = LangStr.UserDefined;
            SyllablePracticeMenu.MenuHelp.Text = LangStr.SylPracHelp;

            // Word menu
            WordPracticeMenu.MenuName.Text = LangStr.WordPrac;
            WordPracticeMenu.MenuDesc.Text = LangStr.WordPracDesc;
            WordPracticeMenu.StartBtn.Text = LangStr.StartPrac;
            WordPracticeMenu.TabLbl1.Content = LangStr.Local;
            WordPracticeMenu.TabLbl2.Content = LangStr.Server;
            WordPracticeMenu.Header1.Header = LangStr.Rank;
            WordPracticeMenu.Header2.Header = LangStr.Name;
            WordPracticeMenu.Header3.Header = LangStr.Org;
            WordPracticeMenu.Header4.Header = LangStr.Accuracy;
            WordPracticeMenu.Header5.Header = LangStr.Speed;
            WordPracticeMenu.Header6.Header = LangStr.WordCount;
            WordPracticeMenu.Header7.Header = LangStr.ElapsedTime;
            WordPracticeMenu.NotSupport.Text = LangStr.NotSupport;
            WordPracticeMenu.TBlblName.Text = LangStr.Name;
            WordPracticeMenu.TBlblOrg.Text = LangStr.Org;

            // Sentence menu
            SentencePracticeMenu.MenuName.Text = LangStr.SenPrac;
            SentencePracticeMenu.MenuDesc.Text = LangStr.SenPracDesc;
            SentencePracticeMenu.MenuHelp.Text = LangStr.SenPracHelp;
            SentencePracticeMenu.SetToggle.Text = LangStr.RanSenLoc;
            SentencePracticeMenu.StartBtn.Text = LangStr.StartPrac;

            // Article menu
            ArticlePracticeMenu.MenuName.Text = LangStr.ArtPrac;
            ArticlePracticeMenu.MenuDesc.Text = LangStr.ArtPracDesc;
            ArticlePracticeMenu.MenuHelp.Text = LangStr.SenPracHelp;
            ArticlePracticeMenu.StartBtn.Text = LangStr.StartPrac;
        }
    }
}