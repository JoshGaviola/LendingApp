using LendingApp.Class.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.User;

namespace LendingApp.Class.Services
{
    public class DynamicLogin : ILogin
    {
        public User Authenticate(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Enter username and password!");
                return null;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    string sql = @"SELECT * FROM users 
                    WHERE BINARY username = @username
                      AND BINARY password_hash = @password_hash
                      AND role = @role
                      AND is_active = 1";

                    var acc = db.Database.SqlQuery<User>(
                       sql,
                       new MySql.Data.MySqlClient.MySqlParameter("@username", username),
                       new MySql.Data.MySqlClient.MySqlParameter("@password_hash", password),
                       new MySql.Data.MySqlClient.MySqlParameter("@role", role))
                       .FirstOrDefault();

                    return acc;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
