using LendingApp.Data;
using LendingApp.Interface;
using LendingApp.Models.CashierModels;
using LendingApp.Models.LoanOfiicerModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LendingApp.Models.LoanOfficer
{
    public class OfficerApplicationLogic : IStatusProvider
    {
        private readonly BindingList<LoanModel> allLoans;
        public OfficerApplicationLogic(DataSample data)
        {
            allLoans = data.AllLoans; 
        }
        public List<LoanModel> GetApplications(
            string Applied,
            string type,
            string search)
        {
            var query = allLoans.AsEnumerable();

            if (!string.IsNullOrEmpty(Applied) && Applied != "All Status")
                query = query.Where(l => l.Applied == Applied);

            if (!string.IsNullOrEmpty(type) && type != "All Types")
                query = query.Where(l => l.LoanRef == type);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(l =>
                    l.Borrower.Contains(search) ||
                    l.LoanNumber.Contains(search));

            return query.ToList(); // snapshot for UI
        }

        public int TotalApplications => allLoans.Count;

        public List<StatusSummary> GetStatusSummary()
        {
            return allLoans
                .GroupBy(l => l.Applied)
                .Select(g => new StatusSummary
                {
                    Applied = g.Key,
                    Count = g.Count()
                })
                .ToList();
        }

    }
}
