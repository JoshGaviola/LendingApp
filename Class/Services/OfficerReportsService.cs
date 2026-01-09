using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Globalization;
using LendingApp.Class;

namespace LendingApp.Class.Services
{
    public class ReportsSummary
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public decimal TotalOutstanding { get; set; }
    }

    public class RecentLoanData
    {
        public string LoanNumber { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int DaysOverdue { get; set; }
        public DateTime? NextDueDate { get; set; }
    }

    public static class OfficerReportsService
    {
        /// <summary>
        /// Returns a high-level summary for reports. Returns true when DB reachable (even if zeros).
        /// </summary>
        public static bool TryGetReportsSummary(out ReportsSummary summary)
        {
            summary = null;
            try
            {
                using (var db = new AppDbContext())
                {
                    var today = DateTime.Today;

                    var total = db.Loans.AsNoTracking().Count();
                    var active = db.Loans.AsNoTracking().Count(l => (l.Status ?? "") == "Active");

                    var overdue = db.Loans.AsNoTracking().Count(l =>
                        (l.Status ?? "") == "Active" &&
                        (
                            l.DaysOverdue > 0
                            || (l.NextDueDate != null && l.NextDueDate <= today)
                            || (l.OutstandingBalance > 0 && (l.NextDueDate == null || l.NextDueDate <= today))
                        )
                    );

                    var totalOutstanding = db.Loans.AsNoTracking().Select(l => (decimal?)l.OutstandingBalance).Sum() ?? 0m;

                    summary = new ReportsSummary
                    {
                        TotalLoans = total,
                        ActiveLoans = active,
                        OverdueLoans = overdue,
                        TotalOutstanding = totalOutstanding
                    };

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load recent loans (most recently created) for display.
        /// </summary>
        public static bool TryGetRecentLoans(List<RecentLoanData> target, int take = 50)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            try
            {
                using (var db = new AppDbContext())
                {
                    var q =
                        from l in db.Loans.AsNoTracking()
                        join c in db.Customers.AsNoTracking() on l.CustomerId equals c.CustomerId into cj
                        from c in cj.DefaultIfEmpty()
                        orderby l.CreatedDate descending
                        select new
                        {
                            l.LoanNumber,
                            CustomerFirst = c != null ? c.FirstName : null,
                            CustomerLast = c != null ? c.LastName : null,
                            l.Status,
                            l.OutstandingBalance,
                            l.DaysOverdue,
                            l.NextDueDate
                        };

                    var data = q.Take(take).ToList();

                    target.Clear();
                    foreach (var x in data)
                    {
                        var first = x.CustomerFirst ?? string.Empty;
                        var last = x.CustomerLast ?? string.Empty;
                        var customerName = (first + " " + last).Trim();
                        if (string.IsNullOrWhiteSpace(customerName)) customerName = string.Empty;

                        target.Add(new RecentLoanData
                        {
                            LoanNumber = x.LoanNumber ?? string.Empty,
                            Customer = customerName,
                            Status = x.Status ?? string.Empty,
                            OutstandingBalance = x.OutstandingBalance,
                            DaysOverdue = x.DaysOverdue,
                            NextDueDate = x.NextDueDate
                        });
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
