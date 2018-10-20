using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTyping.Properties;

namespace OpenTyping
{
    public class PracticeData
    {
        public PracticeData() {}

        public string Name { get; set; }
        public string Author { get; set; }
        public IList<string> TextData { get; set; }
        public string Character { get; set; }

        [JsonIgnore]
        public string Location { get; set; }

        public void RemoveDuplicates()
        {
            TextData = TextData.Distinct().ToList();
        }

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

        public static IList<PracticeData> LoadFromDirectory(string dataDirectory, string character = null)
        {
            var practiceDataList = new List<PracticeData>();

            Directory.CreateDirectory(dataDirectory);
            string[] practiceDataFiles = Directory.GetFiles(dataDirectory, "*.json");

            if (!practiceDataFiles.Any())
            {
                string message = "경로 " + (string) Settings.Default[MainWindow.PracticeDataDirStr] +
                                 "에서 연습 데이터 파일을 찾을 수 없습니다. 해당 경로에 연습 데이터를 생성하고 다시 시도하세요.";
                throw new PracticeDataLoadFail(message);
            }

            foreach (string practiceDataFile in practiceDataFiles)
            {
                PracticeData practiceData = Load(practiceDataFile);
                if (character != null && practiceData.Character != character)
                {
                    continue; 
                }

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

        public static PracticeData FitPracticeData(PracticeData oldData, TextBlock textBlock) // 넘치지 않게 연습 데이터의 문장들을 적절히 자른다.
        {
            PracticeData newData = new PracticeData()
            {
                Name = oldData.Name,
                Author = oldData.Author,
                Character = oldData.Character,
            };

            var newTextData = new List<string>();

            IList<string> FitLine(string line)
            {
                IList<string> splited = line.Split(' ').ToList();
                if (splited.Count == 1) return splited;

                for (int i = 1; i <= splited.Count; i++)
                {
                    var formattedText = new FormattedText(
                        string.Join(" ", splited.Take(i)),
                        CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface(textBlock.FontFamily,
                                     textBlock.FontStyle,
                                     textBlock.FontWeight,
                                     textBlock.FontStretch),
                        textBlock.FontSize,
                        Brushes.Black,
                        new NumberSubstitution(),
                        TextFormattingMode.Display);

                    if (formattedText.Width > textBlock.ActualWidth - 10)
                    {
                        var result = new List<string>();

                        result.Add(string.Join(" ", splited.Take(i - 1)));
                        result.AddRange(FitLine(string.Join(" ", splited.Skip(i - 1))));

                        return result;
                    }
                }

                return new List<string> { line };
            }

            foreach (string line in oldData.TextData)
            {
                newTextData.AddRange(FitLine(line));
            }

            newData.TextData = newTextData;
            return newData;
        }

    }
}
