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
            using (var db = new AppDbContext())
            {
                string sql = @"INSERT INTO users(username,password_hash,email,first_name,last_name,role,is_active) 
                VALUES
                (@username,@password_hash,@email,@first_name,@last_name,@role,@is_active); ";

                int rows = db.Database.ExecuteSqlCommand(
                    sql,
                    new MySqlParameter("@username", user.Username),
                    new MySqlParameter("@password_hash", user.Password),
                    new MySqlParameter("@email", user.Email),
                    new MySqlParameter("@first_name", user.FirstName),
                    new MySqlParameter("@last_name", user.LastName),
                    new MySqlParameter("@role", user.Role),
                    new MySqlParameter("@is_active", user.IsActive)
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



}
}
