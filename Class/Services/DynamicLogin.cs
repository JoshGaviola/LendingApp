using LendingApp.Class.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.User;
using LendingApp.Class.Services;

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
                    string sql = @"SELECT 
                        username as UserName,
                        password_hash as PasswordHash,
                        role as Role,
                        is_active as IsActive
                    FROM users 
                    WHERE BINARY username = @username
                      AND role = @role
                      AND is_active = 1";

                    var acc = db.Database.SqlQuery<User>(
                       sql,
                       new MySql.Data.MySqlClient.MySqlParameter("@username", username),
                       new MySql.Data.MySqlClient.MySqlParameter("@role", role))
                       .FirstOrDefault();


                    bool isPasswordValid = acc != null
                      && !string.IsNullOrEmpty(password)
                      && !string.IsNullOrEmpty(acc.PasswordHash)
                      && PasswordHashing.VerifyPassword(password, acc.PasswordHash); ;

                    if (isPasswordValid)
                    {
                        return acc;
                    }
                    else
                    {
                        return null;
                    }

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
