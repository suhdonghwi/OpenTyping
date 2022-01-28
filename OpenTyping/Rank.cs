using Newtonsoft.Json;
using OpenTyping.Properties;
using System;
using System.Collections.Generic;

namespace OpenTyping
{
    public class Rank
    {
        public List<User> users;
        private readonly string json;

        public Rank()
        {
            json = (string)Settings.Default["RankLocal"];
            users = JsonConvert.DeserializeObject<List<User>>(json);
            if (users == null) // If first insertion
            {
                users = new List<User>();
            }
        }

        public int Add(User user)
        {
            users.Add(user);
            users.Sort();
            int curPos = users.FindIndex(user.Equals);
            if (curPos == 10) // If not new record
            {
                users.RemoveAt(users.Count - 1);
                return -1;
            }

            if (users.Count > 10) // If records are already full
            {
                users.RemoveAt(users.Count - 1); // Remove always 11th last record
            }
            string json = JsonConvert.SerializeObject(users);
            Settings.Default["RankLocal"] = json;
            Settings.Default.Save();
            return curPos;
        }
    }
}