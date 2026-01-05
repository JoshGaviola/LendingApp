using LendingApp.UI.AdminUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.User;

namespace LendingApp.Class.LogicClass
{
   public class AdminLoginLogic
   {


        public bool LoginSuccessfully(string username, string password)
        {
           
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Enter username and password!");
                return false;
            }
            try
            {
                using (var db = new AppDbContext())
                {
                    string sql = @"SELECT * FROM users 
                           WHERE username = @username
                             AND password_hash = @password_hash 
                             AND role = 'Admin' 
                             AND is_active = 1";


                    var admin = db.Database.SqlQuery<User>(
                        sql,
                        new MySql.Data.MySqlClient.MySqlParameter("@username", username),
                        new MySql.Data.MySqlClient.MySqlParameter("@password_hash", password))
                        .FirstOrDefault();

                        return admin != null;

                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return false;
                }

        }

    }
}
