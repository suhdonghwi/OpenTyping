using Newtonsoft.Json;
using OpenTyping.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTyping.Resources.Lang;

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
        public List<KeyPos> DefaultKeys { get; }

        [JsonProperty]
        public KeyLayoutStats Stats { get; set; } = new KeyLayoutStats();

        [JsonIgnore]
        public string Location { get; set; } = "";

        public Key this[KeyPos pos] => KeyLayoutData[pos.Row][pos.Column];

        public static void SaveKeyLayout(KeyLayout keyLayout)
        {
            File.WriteAllText(keyLayout.Location, JsonConvert.SerializeObject(keyLayout, Formatting.Indented));
        }

        public static KeyLayout Parse(string data)
        {
            KeyLayout keyLayout = JsonConvert.DeserializeObject<KeyLayout>(data);

            if (string.IsNullOrEmpty(keyLayout.Name))
            {
                string message = LangStr.ErrMsg11;
                throw new InvalidKeyLayoutDataException(message);
            }

            if (keyLayout.KeyLayoutData is null)
            {
                string message = LangStr.ErrMsg12;
                throw new InvalidKeyLayoutDataException(message);
            }

            if (string.IsNullOrEmpty(keyLayout.Character))
            {
                string message = LangStr.ErrMsg13;
                throw new InvalidKeyLayoutDataException(message);
            }

            if (keyLayout.DefaultKeys is null)
            {
                string message = LangStr.ErrMsg25;
                throw new InvalidKeyLayoutDataException(message);
            }

            var rowNumberData = new List<Tuple<string, int>>
            {
                Tuple.Create(LangStr.ErrMsg16, 13), // Key count for 1st row on Keyboard
                Tuple.Create(LangStr.ErrMsg17, 13), // 2nd row
                Tuple.Create(LangStr.ErrMsg18, 11), // 3rd row
                Tuple.Create(LangStr.ErrMsg19, 10), // 4th row
                Tuple.Create(LangStr.ErrMsg23, 1)   // 5th row
            };

            for (int i = 0; i < keyLayout.KeyLayoutData.Count; i++)
            {
                if (keyLayout.KeyLayoutData[i].Count != rowNumberData[i].Item2)
                {
                    string message = rowNumberData[i].Item1 + LangStr.ErrMsg20 + " " + rowNumberData[i].Item2 + LangStr.ErrMsg21
                                   + keyLayout.KeyLayoutData[i].Count + LangStr.ErrMsg22;
                    throw new InvalidKeyLayoutDataException(message);
                }
            }

            return keyLayout;
        }

        public static KeyLayout Load(string dataFileLocation)
        {
            string keyLayoutLines = File.ReadAllText(dataFileLocation, Encoding.UTF8);

            try
            {
                KeyLayout keyLayout = Parse(keyLayoutLines);
                keyLayout.Location = dataFileLocation;

                return keyLayout;
            }
            catch (InvalidKeyLayoutDataException ex)
            {
                throw new InvalidKeyLayoutDataException(dataFileLocation + " : " + ex.Message, ex);
            }
        }

        public static IList<KeyLayout> LoadFromDirectory(string layoutsDirectory)
        {
            var keyLayouts = new List<KeyLayout>();

            Directory.CreateDirectory(layoutsDirectory);
            string[] keyLayoutFiles = Directory.GetFiles(layoutsDirectory, "*.json");

            if (!keyLayoutFiles.Any())
            {
                string message = (string) Settings.Default[MainWindow.KeyLayoutDataDirStr]
                                 + LangStr.ErrMsg8;
                throw new KeyLayoutLoadFail(message);
            }

            foreach (string keyLayoutFile in keyLayoutFiles)
            {
                KeyLayout keyLayout = Load(keyLayoutFile);
                KeyLayout duplicate = keyLayouts.Find(kl => kl.Name == keyLayout.Name);

                if (duplicate != null)
                {
                    string message = "\"" + keyLayout.Name + "\" " + LangStr.ErrMsg10 + "\n" +
                                     keyLayout.Location + "\n" + duplicate.Location;
                    throw new KeyLayoutLoadFail(message);
                }
                keyLayouts.Add(keyLayout);
            }

            return keyLayouts;
        }
    }
}