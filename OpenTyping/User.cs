using Newtonsoft.Json;
using System;

namespace OpenTyping
{
    public class User : IComparable<User>
    {
        public User(string name, string org, int accuracy, int speed, int count, double time)
        {
            this.Name = name;
            this.Org = org;
            this.Accuracy = accuracy;
            this.Speed = speed;
            this.Count = count;
            this.Time = time;
        }

        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        public string Name { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        public string Org { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        public int Accuracy { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        public int Speed { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        public int Count { get; set; }
        [JsonConverter(typeof(EncryptingJsonConverter), "UseYourEncKey")]
        private double time;
        public double Time { 
            get => (double)time;
            set => time = value;
        }

        public int CompareTo(User other)
        {
            if (this.Accuracy < other.Accuracy) return 1; // Descending
            else if (this.Accuracy > other.Accuracy) return -1;
            else
            {
                if (this.Speed < other.Speed) return 1; // Descending
                else if (this.Speed > other.Speed) return -1;
                else
                {
                    if (this.Count < other.Count) return 1; // Descending
                    else if (this.Count > other.Count) return -1;
                    else
                    {
                        if (this.Time > other.Time) return 1; // Ascending
                        else if (this.Time < other.Time) return -1;
                        else return 0;
                    }
                }
            }
        }
    }
}