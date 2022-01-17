using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTyping.Properties;
using OpenTyping.Resources.Lang;

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
                string message = LangStr.ErrMsg11;
                throw new InvalidPracticeDataException(message);
            }

            if (practiceData.TextData is null)
            {
                string message = LangStr.ErrMsg15;
                throw new InvalidPracticeDataException(message);
            }

            if (practiceData.TextData.Count == 0)
            {
                string message = LangStr.ErrMsg14;
                throw new InvalidPracticeDataException(message);
            }

            if (string.IsNullOrEmpty(practiceData.Character))
            {
                string message = LangStr.ErrMsg13;
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
                string message = (string) Settings.Default[MainWindow.PracticeDataDirStr]
                                + LangStr.ErrMsg9;
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
                    string message = "\"" + practiceData.Name + "\" " + LangStr.ErrMsg10 + "\n" +
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
