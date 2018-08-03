using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;
using OpenTyping.Properties;

namespace OpenTyping
{
    public class KeyLayout
    {
        public KeyLayout(IList<IList<Key>> keyLayoutData, string name, List<KeyPos> pressing)
        {
            KeyLayoutData = keyLayoutData;
            Name = name;
            Pressing = pressing;
        }

        public string Name { get; }
        public IList<IList<Key>> KeyLayoutData { get; }
        public List<KeyPos> Pressing { get; }

        [JsonIgnore]
        public string Location { get; set; } = "";

        public Key this[KeyPos pos]
        {
            get
            {
                return KeyLayoutData[pos.Row][pos.Column];
            }
        }


        public static KeyLayout ParseKeyLayoutData(string data)
        {
            return JsonConvert.DeserializeObject<KeyLayout>(data);
        }

        public static KeyLayout LoadKeyLayout(string dataFileLocation)
        {
            string keyLayoutLines = File.ReadAllText(dataFileLocation, Encoding.UTF8);
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
            string[] keyLayoutFiles = Directory.GetFiles(layoutsDirectory, "*.json");

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