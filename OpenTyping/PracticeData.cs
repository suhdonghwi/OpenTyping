using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using OpenTyping.Properties;

namespace OpenTyping
{
    public class PracticeData
    {
        public PracticeData(string name, IList<string> textData, string character)
        {
            Name = name;
            TextData = textData;
            Character = character;
        }

        public string Name { get; }
        public IList<string> TextData { get; }
        public string Character { get; }

        [JsonIgnore]
        public string Location { get; set; }

        public static PracticeData Parse(string data)
        {
            PracticeData practiceData = JsonConvert.DeserializeObject<PracticeData>(data);

            if (string.IsNullOrEmpty(practiceData.Name))
            {
                const string message = "연습 데이터의 이름(Name 필드)이 주어지지 않았습니다.";
                throw new InvalidPracticeDataException(message);
            }

            if (practiceData.TextData is null)
            {
                const string message = "연습 데이터의 글자 데이터(TextData 필드)가 주어지지 않았습니다.";
                throw new InvalidPracticeDataException(message);
            }

            if (practiceData.TextData.Count == 0)
            {
                const string message = "연습 데이터의 글자 데이터(TextData 필드) 크기가 0 입니다.";
                throw new InvalidPracticeDataException(message);
            }

            if (string.IsNullOrEmpty(practiceData.Character))
            {
                const string message = "연습 데이터의 문자 종류(Character 필드)가 주어지지 않았습니다.";
                throw new InvalidPracticeDataException(message);
            }

            return practiceData;
        }

        public static PracticeData Load(string dataFileLocation)
        {
            string dataLines = File.ReadAllText(dataFileLocation, Encoding.UTF8);

            try
            {
                PracticeData practiceData = Parse(dataLines);
                practiceData.Location = dataFileLocation;

                return practiceData;
            }
            catch (InvalidPracticeDataException ex)
            {
                throw new InvalidPracticeDataException(dataFileLocation + " : " + ex.Message, ex);
            }
        }

        public static IList<PracticeData> LoadFromDirectory(string dataDirectory)
        {
            var practiceDataList = new List<PracticeData>();

            Directory.CreateDirectory(dataDirectory);
            string[] practiceDataFiles = Directory.GetFiles(dataDirectory, "*.json");

            if (!practiceDataFiles.Any())
            {
                string message = "경로 " + (string) Settings.Default[MainWindow.PracticeDataDir] +
                                 "에서 연습 데이터 파일을 찾을 수 없습니다. 해당 경로에 연습 데이터를 생성하고 다시 시도하세요.";
                throw new PracticeDataLoadFail(message);
            }

            foreach (string practiceDataFile in practiceDataFiles)
            {
                PracticeData practiceData = Load(practiceDataFile);
                PracticeData duplicate = practiceDataList.Find(data => data.Name == practiceData.Name);

                if (duplicate != null)
                {
                    string message = "연습 데이터 이름 \"" + practiceData.Name + "\" 이 중복되게 존재합니다.\n" +
                                     practiceData.Location + "\n" + duplicate.Location;
                    throw new PracticeDataLoadFail(message);
                }
                practiceDataList.Add(practiceData);
            }

            return practiceDataList;
        }
    }
}
