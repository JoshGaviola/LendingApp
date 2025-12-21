using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.LoanOfficer
{
    public class OfficerDashboardLogic : LoanOfficerLogic
    {
        //SummaryStats
        public int pendingApplicationsCount { get; set; }
        public string activePortfolio { get; set; }
        public int overdueLoansCount { get; set; }
        public string todayCollection { get; set; }

        public class PendingApplication
        {
            public string Id { get; set; }
            public string Customer { get; set; }
            public string LoanType { get; set; }
            public string Amount { get; set; }
            public int DaysWaiting { get; set; }
            public string Priority { get; set; } // High | Medium | Low
        }

        private readonly List<PendingApplication> pendingApplications = new List<PendingApplication>
        {
            new PendingApplication { Id="APP-001", Customer="Juan Cruz", LoanType="Personal", Amount="₱50,000", DaysWaiting=2, Priority="High" },
            new PendingApplication { Id="APP-002", Customer="Maria Santos", LoanType="Emergency", Amount="₱15,000", DaysWaiting=1, Priority="Medium" },
            new PendingApplication { Id="APP-003", Customer="Pedro Reyes", LoanType="Salary", Amount="₱25,000", DaysWaiting=3, Priority="High" },
            new PendingApplication { Id="APP-001", Customer="Juan Cruz", LoanType="Personal", Amount="₱50,000", DaysWaiting=2, Priority="High" },
            new PendingApplication { Id="APP-002", Customer="Maria Santos", LoanType="Emergency", Amount="₱15,000", DaysWaiting=1, Priority="Medium" },
            new PendingApplication { Id="APP-003", Customer="Pedro Reyes", LoanType="Salary", Amount="₱25,000", DaysWaiting=3, Priority="High" },
        };
        public IReadOnlyList<PendingApplication> AllPending => pendingApplications;
        public int TotalPendingApplications => pendingApplications.Count;


    }
}
