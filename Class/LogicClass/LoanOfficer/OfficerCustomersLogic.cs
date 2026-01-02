using LendingApp.Data;
using LendingApp.Interface;
using LendingApp.Class.Models.CashierModels;
using LendingApp.Class.Models.LoanOfiicerModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.LoanOfficer
{
   public class OfficerCustomersLogic : IStatusProvider
    {

        private readonly BindingList<LoanModel> allLoans;

        public OfficerCustomersLogic(DataSample data)
        {
            allLoans = data.AllLoans;
        }
      
        
        public IReadOnlyList<LoanModel> AllCustomers => allLoans;
        public int TotalCustomers => allLoans.Count;

        public List<StatusSummary> GetStatusSummary()
        {
            return allLoans
                .GroupBy(a => a.Type)
                .Select(g => new StatusSummary { Type = g.Key, Count = g.Count() })
                .ToList();
        }

    }
}
