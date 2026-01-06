using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.User;
namespace LendingApp.Class.LogicClass.Cashier
{
    public class CashierLoginLogic
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
                    WHERE BINARY username = @username
                      AND BINARY password_hash = @password_hash
                      AND role = 'Cashier' 
                      AND is_active = 1";

                    var cashier = db.Database.SqlQuery<User>(
                       sql,
                       new MySql.Data.MySqlClient.MySqlParameter("@username", username),
                       new MySql.Data.MySqlClient.MySqlParameter("@password_hash", password))
                       .FirstOrDefault();

                    return cashier != null;

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


