using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenTyping
{
    public class Rank
    {
        public List<User> users;
        private SqliteProvider sqlite = new SqliteProvider();

        public Rank() { }

        public async Task GetUsers()
        {
            if (await sqlite.OpenDatabase())
            {
                users = await sqlite.GetUsersAsync();
                if (users.Count == 0) // If first insertion
                {
                    users = new List<User>();
                }
                users.Sort();
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

            sqlite.ReWriteAllAsync(users);
            return curPos;
        }
    }
}