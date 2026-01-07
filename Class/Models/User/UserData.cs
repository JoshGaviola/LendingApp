using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Class.Models.User
{
    public class UserData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string hashedPassoword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime HireDate { get; set; }

        public string Confirmpassword { get; set; }
    }

}
