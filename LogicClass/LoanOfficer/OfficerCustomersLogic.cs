using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Models.LoanOfiicerModels;

namespace LendingApp.Models.LoanOfficer
{
   public class OfficerCustomersLogic
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

      
        private readonly List<CustomerItem> customers = new List<CustomerItem>
        {
            new CustomerItem { Id="CUST-001", Name="Juan Cruz", Contact="+639123456789", Email="juan.cruz@email.com", Type="Regular",  CreditScore=750, TotalLoans=2, Balance="₱85,000",  BalanceAmount=85000,  RegisteredDate="Jan 15, 2024", LastActivity="2 days ago" },
            new CustomerItem { Id="CUST-002", Name="Maria Santos", Contact="+639987654321", Email="maria.santos@email.com", Type="New",     CreditScore=680, TotalLoans=1, Balance="₱35,000",  BalanceAmount=35000,  RegisteredDate="Nov 20, 2025", LastActivity="5 days ago" },
        };

        public IReadOnlyList<CustomerItem> AllCustomers => customers;
        public int TotalCustomers => customers.Count;

        public List<StatusSummary> GetStatusSummary()
        {
            return customers
                .GroupBy(a => a.Type)
                .Select(g => new StatusSummary { Type = g.Key, Count = g.Count() })
                .ToList();
        }

    }
}
