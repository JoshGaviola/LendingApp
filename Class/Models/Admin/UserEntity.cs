using System;

namespace LendingApp.Class.Models.Admin
{
    public class UserEntity
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Admin | LoanOfficer | Cashier (stored as enum in MySQL)
        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}