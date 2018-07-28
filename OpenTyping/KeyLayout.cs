using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using OpenTyping.Properties;

namespace OpenTyping
{
    public class KeyLayout
    {
        public KeyLayout(IList<IList<Key>> keyLayoutData, string name)
        {
            KeyLayoutData = keyLayoutData;
            Name = name;
        }

        public IList<IList<Key>> KeyLayoutData { get; }
        public string Name { get; }

        public string Location { get; set; } = "";

        public static KeyLayout ParseKeyLayoutData(string[] dataLines)
        {
            string name = dataLines[0];
            dataLines = dataLines.Skip(1).ToArray();

            List<string[]> splitedData
                = dataLines.Select(line => line.Split(new string[] { " | " }, StringSplitOptions.None)).ToList();

            if (splitedData.Count != 4)
            {
                string exMessage = "자판 데이터의 행 수는 4 여야 하는데 " + splitedData.Count + "개가 주어졌습니다.";
                throw new InvalidKeyLayoutDataException(exMessage);
            }

            var rowNumberData = new List<Tuple<string, int>>
            {
                Tuple.Create("숫자열", 13),
                Tuple.Create("첫째 열", 13),
                Tuple.Create("둘째 열", 11),
                Tuple.Create("셋째 열", 10)
            };

            List<IList<Key>> keyLayoutData = new List<IList<Key>>();

            for (int i = 0; i < splitedData.Count; i++)
            {
                string[] row = splitedData[i];

                if (row.Length != rowNumberData[i].Item2)
                {
                    string exMessage
                        = rowNumberData[i].Item1 + "의 키 개수는 " + rowNumberData[i].Item2 + " 이어야 하는데 " + row.Length +
                          "개가 주어졌습니다.";
                    throw new InvalidKeyLayoutDataException(exMessage);
                }

                var newRow = new List<Key>();

                foreach (string keyString in row)
                {
                    string[] splitedKeyString = Regex.Split(keyString, @"(?<!\\)(?:\\\\)*,");
                    splitedKeyString = splitedKeyString.Select(str => str.Replace(@"\,", ",")).ToArray();

                    if (splitedKeyString.Length != 2)
                    {
                        string exMessage
                            = "글쇠, 윗글쇠 2개가 주어져야 하는데 " + splitedKeyString.Length + "개가 주어졌습니다.";
                        throw new InvalidKeyLayoutDataException(exMessage);
                    }

                    newRow.Add(new Key(splitedKeyString[0], splitedKeyString[1]));
                }

                keyLayoutData.Add(newRow);
            }

            return new KeyLayout(keyLayoutData, name);
        }

        public static KeyLayout LoadKeyLayout(string dataFileLocation)
        {
            string[] keyLayoutLines = File.ReadAllLines(dataFileLocation, Encoding.UTF8);
            KeyLayout keyLayout = null;

            try
            {
                keyLayout = ParseKeyLayoutData(keyLayoutLines);         
            }
            catch (InvalidKeyLayoutDataException ex)
            {
                MessageBox.Show(dataFileLocation + " : " + ex.Message, 
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            return keyLayout;
        }

        public static IList<KeyLayout> LoadKeyLayouts(string layoutsDirectory)
        {
            var keyLayouts = new List<KeyLayout>();

            Directory.CreateDirectory(layoutsDirectory);
            string[] keyLayoutFiles = Directory.GetFiles(layoutsDirectory, "*.kl");

            foreach (string keyLayoutFile in keyLayoutFiles)
            {
                KeyLayout keyLayout = LoadKeyLayout(keyLayoutFile);

                if (keyLayouts.Any(kl => kl.Name == keyLayout.Name))
                {
                    MessageBox.Show("자판 이름 \"" + keyLayout.Name + "\" 이 중복되게 존재합니다.",
                                    "열린타자",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Environment.Exit(-1);
                }

                keyLayout.Location = keyLayoutFile;
                keyLayouts.Add(keyLayout);
            }

            return keyLayouts;
        }
    }
}