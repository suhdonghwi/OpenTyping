using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace OpenTyping
{
    class PracticeData
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
            PracticeData practiceData = null;

            try
            {
                practiceData = Parse(dataLines);
            }
            catch (InvalidPracticeDataException ex)
            {
                MessageBox.Show(dataFileLocation + " : " + ex.Message, 
                                "열린타자",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            practiceData.Location = dataFileLocation;

            return practiceData;
        }
    }
}
