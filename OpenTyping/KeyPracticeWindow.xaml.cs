using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace OpenTyping
{
    /// <summary>
    /// KeyPracticeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPracticeWindow : MetroWindow
    {
        private List<KeyPos> keyList;

        public Tuple<string, KeyPos> PreviousKey { get; }
        public Tuple<string, KeyPos> CurrentKey { get; }
        public Tuple<string, KeyPos> NextKey { get; }

        private static readonly Random randomizer = new Random();

        public KeyPracticeWindow(List<KeyPos> keyList)
        {
            InitializeComponent();

            this.DataContext = this;

            this.keyList = keyList;

            CurrentKey = RandomKey();
            NextKey = RandomKey();
        }

        private Tuple<string, KeyPos> RandomKey()
        {
            KeyPos keyPos = keyList[randomizer.Next(0, keyList.Count)];
            Key key = MainWindow.CurrentKeyLayout[keyPos];

            return Tuple.Create(randomizer.Next(0, 1) == 0 ? key.KeyData : key.ShiftKeyData, keyPos); 
        }

        private void MoveKey()
        {

        }
    }
}
