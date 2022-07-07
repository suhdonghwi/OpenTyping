using SQLite;
using System;

namespace OpenTyping
{
    [Table("Users")]
    public class User : IComparable<User>
    {
        public User() { }

        public User(string name, string org, int accuracy, int speed, int count, double time)
        {
            this.Name = name;
            this.Org = org;
            this.Accuracy = accuracy;
            this.Speed = speed;
            this.Count = count;
            this.Time = time;
        }

        public string Name { get; set; }
        public string Org { get; set; }
        public int Accuracy { get; set; }
        public int Speed { get; set; }
        public int Count { get; set; }
        private double time;
        public double Time
        {
            // Add this to fix sqlite datatype-conversion
            get => (double)time;
            set => time = (double)value;
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