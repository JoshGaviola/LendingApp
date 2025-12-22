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

        public class OverdueLoan
        {
            public string Id { get; set; }
            public string Customer { get; set; }
            public string AmountDue { get; set; }
            public int DaysOverdue { get; set; }
            public string Contact { get; set; }
            public string Priority { get; set; } // Critical | High | Medium
        }
        public class TaskItem
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string Customer { get; set; }
            public string TaskType { get; set; }
            public string LoanId { get; set; }
            public string Status { get; set; } // Due | Pending | Completed
        }

       public class ActivityItem
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string Activity { get; set; }
            public string Customer { get; set; }
            public string Amount { get; set; }
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

        private readonly List<OverdueLoan> overdueLoans = new List<OverdueLoan>
        {
            new OverdueLoan { Id="LN-001", Customer="Pedro Reyes", AmountDue="₱4,442", DaysOverdue=5, Contact="+639123456789", Priority="Critical" },
            new OverdueLoan { Id="LN-002", Customer="Ana Lopez", AmountDue="₱3,250", DaysOverdue=3, Contact="+639987654321", Priority="High" },
            new OverdueLoan { Id="LN-003", Customer="Carlos Tan", AmountDue="₱2,100", DaysOverdue=2, Contact="+639456789012", Priority="Medium" },
            new OverdueLoan { Id="LN-001", Customer="Pedro Reyes", AmountDue="₱4,442", DaysOverdue=5, Contact="+639123456789", Priority="Critical" },
            new OverdueLoan { Id="LN-002", Customer="Ana Lopez", AmountDue="₱3,250", DaysOverdue=3, Contact="+639987654321", Priority="High" },
            new OverdueLoan { Id="LN-003", Customer="Carlos Tan", AmountDue="₱2,100", DaysOverdue=2, Contact="+639456789012", Priority="Medium" },
        };

        private readonly List<TaskItem> todayTasks = new List<TaskItem>
        {
            new TaskItem { Id="T-001", Time="9:00 AM", Customer="Juan Cruz", TaskType="Payment Follow-up", LoanId="PLN-001", Status="Due" },
            new TaskItem { Id="T-002", Time="2:00 PM", Customer="Maria Santos", TaskType="Doc Review", LoanId="ELN-002", Status="Pending" },
            new TaskItem { Id="T-003", Time="4:00 PM", Customer="Pedro Reyes", TaskType="Credit Assessment", LoanId="SLN-003", Status="Pending" },
            new TaskItem { Id="T-001", Time="9:00 AM", Customer="Juan Cruz", TaskType="Payment Follow-up", LoanId="PLN-001", Status="Due" },
            new TaskItem { Id="T-002", Time="2:00 PM", Customer="Maria Santos", TaskType="Doc Review", LoanId="ELN-002", Status="Pending" },
            new TaskItem { Id="T-003", Time="4:00 PM", Customer="Pedro Reyes", TaskType="Credit Assessment", LoanId="SLN-003", Status="Pending" },
        };

        private readonly List<ActivityItem> recentActivity = new List<ActivityItem>
        {
            new ActivityItem { Id="A-001", Time="08:30 AM", Activity="Payment Received", Customer="Ana Lopez", Amount="₱5,250" },
            new ActivityItem { Id="A-002", Time="Yesterday", Activity="App Approved", Customer="Juan Cruz", Amount="₱50,000" },
            new ActivityItem { Id="A-003", Time="Yesterday", Activity="Payment Received", Customer="Carlos Tan", Amount="₱3,100" },
            new ActivityItem { Id="A-004", Time="2 days ago", Activity="Loan Disbursed", Customer="Maria Santos", Amount="₱15,000" },
            new ActivityItem { Id="A-001", Time="08:30 AM", Activity="Payment Received", Customer="Ana Lopez", Amount="₱5,250" },
            new ActivityItem { Id="A-002", Time="Yesterday", Activity="App Approved", Customer="Juan Cruz", Amount="₱50,000" },
            new ActivityItem { Id="A-003", Time="Yesterday", Activity="Payment Received", Customer="Carlos Tan", Amount="₱3,100" },
            new ActivityItem { Id="A-004", Time="2 days ago", Activity="Loan Disbursed", Customer="Maria Santos", Amount="₱15,000" },
        };

        public IReadOnlyList<OverdueLoan> AllOverdueLoans => overdueLoans;      
        public IReadOnlyList<TaskItem> AllTodayTasks => todayTasks;
        public IReadOnlyList<ActivityItem> AllRecentActivity => recentActivity;
        public int TotalTodayTasks => todayTasks.Count;
        public int TotalOverdueLoans => overdueLoans.Count;


    }
}
