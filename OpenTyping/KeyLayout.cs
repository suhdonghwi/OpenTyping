using Newtonsoft.Json;
using OpenTyping.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private const string ChoseongTable = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        private const string JungseongTable = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        private const string JongseongTable = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

        private static bool IsHangulCompatibilityJamo(char ch)
            => ch >= (char)0x3131 || ch <= (char)0x3163;

        private static bool IsHangulSyllable(char ch)
            => ch >= (char)0xAC00 || ch <= (char)0xD79F;

        private static IEnumerable<char> DecomposeHangul(char ch)
        {
            if (IsHangulCompatibilityJamo(ch)) // ch is in Hangul Compatibility Jamo unicode block
            {
                return new List<char> { ch };
            }
            if (!IsHangulSyllable(ch)) // ch is not in Hangul Syllables unicode block
            {
                return new List<char>();
            }

            int code = ch - (char)0xAC00;
            var result = new List<char>();

            int choseongIndex = code / (21 * 28);
            result.Add(ChoseongTable[choseongIndex]);
            code %= 21 * 28;

            int jungseongIndex = code / 28;
            result.Add(JungseongTable[jungseongIndex]);
            code %= 28;

            int jongseongIndex = code;
            if (jongseongIndex != 0) result.Add(JongseongTable[jongseongIndex]);

            return result;
        }

        //public int CountLetter(string text)
        //{
        //    int count = 0;
        //    foreach (char ch in text)
        //    {
        //        IList<char> decomposed = DecomposeHangul(ch).ToList();

        //        if (!decomposed.Any()) // ch is not a hangul character
        //        {
        //            count++;
        //            continue;
        //        }

        //        foreach (char jamo in decomposed)
        //        {
        //        }
        //    }
        //}

        public static KeyLayout Parse(string data)
        {
            KeyLayout keyLayout = JsonConvert.DeserializeObject<KeyLayout>(data);

            if (string.IsNullOrEmpty(keyLayout.Name))
            {
                const string message = "자판 데이터의 이름(Name 필드)이 주어지지 않았습니다.";
                throw new InvalidKeyLayoutDataException(message);
            }

            if (keyLayout.KeyLayoutData is null)
            {
                const string message = "자판 데이터(KeyLayoutData 필드)가 주어지지 않았습니다.";
                throw new InvalidKeyLayoutDataException(message);
            }

            if (string.IsNullOrEmpty(keyLayout.Character))
            {
                const string message = "자판 데이터의 문자 종류(Character 필드)가 주어지지 않았습니다.";
                throw new InvalidKeyLayoutDataException(message);
            }

            if (keyLayout.DefaultKeys is null)
            {
                keyLayout.DefaultKeys = new List<KeyPos>();
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
                string message = "경로 " + (string) Settings.Default[MainWindow.KeyLayoutDataDir] +
                                 "에서 자판 데이터 파일을 찾을 수 없습니다. 해당 경로에 자판 데이터를 생성하고 다시 시도하세요.";
                throw new KeyLayoutLoadFail(message);
            }

            foreach (string keyLayoutFile in keyLayoutFiles)
            {
                KeyLayout keyLayout = Load(keyLayoutFile);
                KeyLayout duplicate = keyLayouts.Find(kl => kl.Name == keyLayout.Name);

                if (duplicate != null)
                {
                    string message = "자판 이름 \"" + keyLayout.Name + "\" 이 중복되게 존재합니다.\n" +
                                     keyLayout.Location + "\n" + duplicate.Location;
                    throw new KeyLayoutLoadFail(message);
                }
                keyLayouts.Add(keyLayout);
            }

            return keyLayouts;
        }
    }
}