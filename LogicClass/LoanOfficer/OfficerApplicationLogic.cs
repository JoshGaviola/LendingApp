using System.Collections.Generic;
using System.Linq;
using LendingApp.Models.LoanOfiicerModels;
using LendingApp.Interface;

namespace LendingApp.Models.LoanOfficer
{
    public class OfficerApplicationLogic : IStatusProvider
    {

        private readonly List<ApplicationItem> applications = new List<ApplicationItem>
         {
            new ApplicationItem { Id="APP-001", Customer="Juan Dela Cruz", LoanType="Personal", Amount="₱50,000", AppliedDate="Dec 10", Status="Pending", Priority="High" },
            new ApplicationItem { Id="APP-002", Customer="Maria Santos", LoanType="Emergency", Amount="₱15,000", AppliedDate="Dec 11", Status="Review", Priority="Medium" },
            new ApplicationItem { Id="APP-003", Customer="Pedro Reyes", LoanType="Salary", Amount="₱25,000", AppliedDate="Dec 12", Status="Approved" },
            new ApplicationItem { Id="APP-004", Customer="Ana Lopez", LoanType="Personal", Amount="₱75,000", AppliedDate="Dec 09", Status="Review", Priority="High" },
            new ApplicationItem { Id="APP-005", Customer="Carlos Tan", LoanType="Emergency", Amount="₱10,000", AppliedDate="Dec 13", Status="Approved" },
         };

        public IReadOnlyList<ApplicationItem> Allapplications => applications;

        public List<ApplicationItem> GetApplications(
            string status,
            string type,
            string search)
        {
            var query = applications.AsEnumerable();

            if (!string.IsNullOrEmpty(status) && status != "All Status")
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(type) && type != "All Types")
                query = query.Where(a => a.LoanType == type);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a =>
                    a.Customer.ToLower().Contains(search.ToLower()) ||
                    a.Id.ToLower().Contains(search.ToLower()));

            return query.ToList();
        }

        public List<StatusSummary> GetStatusSummary()
        {
            return applications
                .GroupBy(a => a.Status)
                .Select(g => new  StatusSummary{ Status = g.Key, Count = g.Count() })
                .ToList();
        }



        public int TotalApplications => applications.Count;

        
    }
}
