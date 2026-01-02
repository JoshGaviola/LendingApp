using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Class.Models.LoanOfiicerModels
{
    public class CustomerItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Type { get; set; } // New | Regular | VIP | Delinquent
        public int CreditScore { get; set; }
        public int TotalLoans { get; set; }
        public string Balance { get; set; }
        public int BalanceAmount { get; set; }
        public string RegisteredDate { get; set; }
        public string LastActivity { get; set; }
    }

}
