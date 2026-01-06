using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Data;
using LendingApp.Class.Models.User;

namespace LendingApp.Class.Services
{
    public static class DataGetter
    {
        public static ApplicantsData Data { get; } = new ApplicantsData();

        // Allow reading current user; set via SetCurrentUser to keep control of assignment.
        public static User CurrentUser { get; private set; }

        public static void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }
}
