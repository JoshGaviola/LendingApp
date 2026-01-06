using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Class.Models.User;

namespace LendingApp.Class.Interface
{
    public interface ILogin
    {
        User Authenticate(string username, string password,string role);
    }
}
