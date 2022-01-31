using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using OpenTyping.Resources.Lang;
using SQLite;

namespace OpenTyping
{
    public class SqliteProvider
    {
        private SQLiteAsyncConnection _db;

        public SqliteProvider() { }

        public async Task<bool> OpenDatabase()
        {
            string filename = Directory.GetCurrentDirectory() + @"\Resources\db.db";
            string password = GetMachineID() + "AddYourSalt";
            Debug.WriteLine(password);

            var options = new SQLiteConnectionString(filename, true, key: password);
            _db = new SQLiteAsyncConnection(options);

            if (!File.Exists(filename))
            {
                await _db.CreateTableAsync<User>().ContinueWith((results) =>
                {
                    Debug.WriteLine("New database! Table created!");
                });
            }
            else // Check it's broken DB
            {
                try
                {
                    await _db.GetTableInfoAsync("Users");
                } 
                catch (Exception ex)
                {
                    if (ex.Message.Contains("file is not a database"))
                    {

                        MessageBox.Show(LangStr.ErrMsg24,
                                    LangStr.AppName,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                        Environment.Exit(-1);
                        return false;
                    }
                }
            }

            await _db.CloseAsync();

            return true;
        } 

        private string GetMachineID()
        {
            string id = "";
            ManagementObjectCollection mbsList;
            mbsList= new ManagementObjectSearcher("Select * From Win32_processor").Get();
            
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorID"].ToString();
            }

            return id;
        }

        public void AddUserAsync(User user)
        {
            _db.InsertAsync(user).ContinueWith((t) =>
            {
                Debug.WriteLine("New user Name: {0}", user.Name);
            });
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _db.QueryAsync<User>("Select * From Users");
        }

        public async void ReWriteAllAsync(List<User> users)
        {
            await _db.DeleteAllAsync<User>();
            Debug.WriteLine("All records are deleted");

            await _db.InsertAllAsync(users);
            Debug.WriteLine("Rewrited all users");
        }
    }
}
