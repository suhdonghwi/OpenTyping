using Newtonsoft.Json;
using OpenTyping.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace OpenTyping
{
    public class KeyLayout
    {
        public KeyLayout(string name, string character, IList<IList<Key>> keyLayoutData, List<KeyPos> defaultKeys)
        {  
            Name = name;
            Character = character;
            KeyLayoutData = keyLayoutData;
            DefaultKeys = defaultKeys;
        }

        public string Name { get; }
        public string Character { get; }
        public IList<IList<Key>> KeyLayoutData { get; }
        public List<KeyPos> DefaultKeys { get; set; }

        [JsonProperty]
        public KeyLayoutStats Stats { get; private set; } = new KeyLayoutStats();

        [JsonIgnore]
        public string Location { get; set; } = "";

        public Key this[KeyPos pos] => KeyLayoutData[pos.Row][pos.Column];

        public static KeyLayout Parse(string data)
        {
            KeyLayout keyLayout = JsonConvert.DeserializeObject<KeyLayout>(data);

            if (string.IsNullOrEmpty(keyLayout.Name))
            {
                const string message = "자판 데이터의 이름(Name 필드)이 주어지지 않았습니다.";
                throw new InvalidKeyLayoutDataException(message);
            }

            var rowNumberData = new List<Tuple<string, int>>
            {
                Tuple.Create("숫자열", 13),
                Tuple.Create("첫째 열", 13),
                Tuple.Create("둘째 열", 11),
                Tuple.Create("셋째 열", 10)
            };

            for (int i = 0; i < keyLayout.KeyLayoutData.Count; i++)
            {
                if (keyLayout.KeyLayoutData[i].Count != rowNumberData[i].Item2)
                {
                    string message = rowNumberData[i].Item1 + "의 키 개수는 " + rowNumberData[i].Item2 + " 이어야 하는데 "
                                   + keyLayout.KeyLayoutData[i].Count + "개가 주어졌습니다.";
                    throw new InvalidKeyLayoutDataException(message);
                }
            }

            return keyLayout;
        }

        public static KeyLayout Load(string dataFileLocation)
        {
            string keyLayoutLines = File.ReadAllText(dataFileLocation, Encoding.UTF8);
            KeyLayout keyLayout = null;

            try
            {
                keyLayout = Parse(keyLayoutLines);         
            }
            catch (InvalidKeyLayoutDataException ex)
            {
                MessageBox.Show(dataFileLocation + " : " + ex.Message, 
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            keyLayout.Location = dataFileLocation;

            return keyLayout;
        }

        public static IList<KeyLayout> LoadFromDirectory(string layoutsDirectory)
        {
            var keyLayouts = new List<KeyLayout>();

            Directory.CreateDirectory(layoutsDirectory);
            string[] keyLayoutFiles = Directory.GetFiles(layoutsDirectory, "*.json");

            if (!keyLayoutFiles.Any())
            {
                MessageBox.Show("경로 " + (string)Settings.Default[MainWindow.KeyLayoutDataDir] +
                                "에서 자판 데이터 파일을 찾을 수 없습니다. 해당 경로에 자판 데이터를 생성하고 다시 시도하세요.",
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            foreach (string keyLayoutFile in keyLayoutFiles)
            {
                KeyLayout keyLayout = Load(keyLayoutFile);
                KeyLayout duplicate = keyLayouts.Find(kl => kl.Name == keyLayout.Name);

                if (duplicate != null)
                {
                    MessageBox.Show("자판 이름 \"" + keyLayout.Name + "\" 이 중복되게 존재합니다.\n" +
                                    keyLayout.Location + "\n" + duplicate.Location + ")",
                                    "열린타자",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
                keyLayouts.Add(keyLayout);
            }

            return keyLayouts;
        }
    }
}