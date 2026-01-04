using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models
{
    public abstract class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }


        public bool Login()
        {
            return true;
        }

        public void Logout()
        {
            // Implementation for user logout
        }

        public void ChangePassword(string NewPasswordHash)
        {
            // Implementation for changing password
             PasswordHash = NewPasswordHash;
        }

    }
}
