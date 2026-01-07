using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace LendingApp.Class.Services
{
    public class PasswordHashing
    {
        public static string GeneratePassowordHash(string password)
        {
          return BCrypt.Net.BCrypt.HashPassword(password);
        }

          public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}
