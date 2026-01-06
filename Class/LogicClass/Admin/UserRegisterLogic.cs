using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.User;

namespace LendingApp.Class.LogicClass.Admin
{
    public class UserRegisterLogic
    {
        public bool RegisterSuccess(UserData user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName))
            {
                MessageBox.Show("Please fill in all required fields.");
                return false;
            }

            if (user.Password != user.Confirmpassword)
            {
                MessageBox.Show("Passwords do not match.");
                return false;
            }

            try
            {
                // Map UI role values to DB enum values
                var dbRole = MapRoleToDbValue(user.Role);

                using (var db = new AppDbContext())
                {
                    string sql = @"INSERT INTO users(username,password_hash,email,first_name,last_name,role,is_active,created_date) 
                    VALUES
                    (@username,@password_hash,@email,@first_name,@last_name,@role,@is_active,@created_date); ";

                    int rows = db.Database.ExecuteSqlCommand(
                        sql,
                        new MySqlParameter("@username", user.Username),
                        new MySqlParameter("@password_hash", user.Password),
                        new MySqlParameter("@email", user.Email ?? (object)DBNull.Value),
                        new MySqlParameter("@first_name", user.FirstName),
                        new MySqlParameter("@last_name", user.LastName),
                        new MySqlParameter("@role", dbRole),
                        new MySqlParameter("@is_active", user.IsActive ? 1 : 0),
                        new MySqlParameter("@created_date", DateTime.Now)
                    );

                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private string MapRoleToDbValue(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "";

            // Normalize common UI values to DB enum values: 'Admin', 'LoanOfficer', 'Cashier'
            var r = role.Trim();

            if (string.Equals(r, "Loan Officer", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "LoanOfficer", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "Loan Officer ", StringComparison.OrdinalIgnoreCase))
            {
                return "LoanOfficer";
            }

            if (string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase))
                return "Admin";

            if (string.Equals(r, "Cashier", StringComparison.OrdinalIgnoreCase))
                return "Cashier";

            // Fallback: return original trimmed value (may still fail if not allowed by enum)
            return r;
        }
    }
}
