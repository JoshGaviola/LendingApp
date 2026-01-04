using LendingApp.Class;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Data;
using LendingApp.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LendingApp.Models.LoanOfficer
{
    public class OfficerApplicationLogic : IStatusProvider
    {
        // Keep constructor for compatibility with older calls
        public OfficerApplicationLogic(ApplicantsData data)
        {
        }

        public List<LoanModel> GetApplications(string applied, string type, string search)
        {
            using (var db = new AppDbContext())
            {
                var query =
                    from a in db.LoanApplications.AsNoTracking()
                    join c in db.Customers.AsNoTracking() on a.CustomerId equals c.CustomerId
                    join p in db.LoanProducts.AsNoTracking() on a.ProductId equals p.ProductId
                    select new
                    {
                        a.ApplicationNumber,
                        CustomerName = (c.FirstName ?? "") + " " + (c.LastName ?? ""),
                        ProductName = p.ProductName,
                        a.RequestedAmount,
                        a.Status,
                        a.ApplicationDate,
                        a.ApprovedDate,        // ← Added
                        p.InterestRate,        // ← Added
                        p.ProcessingFeePct,    // ← Added
                        a.PreferredTerm        // ← Added
                    };

                if (!string.IsNullOrEmpty(applied) && applied != "All Status")
                    query = query.Where(x => x.Status == applied);

                if (!string.IsNullOrEmpty(type) && type != "All Types")
                    query = query.Where(x => x.ProductName.Contains(type));

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim();
                    query = query.Where(x =>
                        x.CustomerName.Contains(s) ||
                        x.ApplicationNumber.Contains(s));
                }

                var rows = query
                    .OrderByDescending(x => x.ApplicationDate)
                    .ToList();

                return rows.Select(x => new LoanModel
                {
                    LoanNumber = x.ApplicationNumber,
                    Borrower = x.CustomerName.Trim(),
                    Type = x.ProductName,
                    Amount = x.RequestedAmount,
                    Applied = x.Status,
                    LoanRef = x.ProductName,                    // ← Added
                    ApprovedDate = x.ApprovedDate ?? DateTime.MinValue,  // ← Added
                    TermMonths = x.PreferredTerm,              // ← Added
                    InterestRate = x.InterestRate,             // ← Added
                    ProcessingFee = x.ProcessingFeePct,        // ← Added
                    PaidAmount = 0,                            // ← Added (default)
                    ReceiptNo = "",                            // ← Added (default)
                    Time = x.ApplicationDate.ToString("MMM dd, yyyy") // ← Added
                }).ToList();
            }
        }

        public int TotalApplications
        {
            get
            {
                using (var db = new AppDbContext())
                    return db.LoanApplications.AsNoTracking().Count();
            }
        }

        public List<StatusSummary> GetStatusSummary()
        {
            using (var db = new AppDbContext())
            {
                return db.LoanApplications.AsNoTracking()
                    .GroupBy(a => a.Status)
                    .Select(g => new StatusSummary
                    {
                        Applied = g.Key,
                        Count = g.Count()
                    })
                    .ToList();
            }
        }
    }
}
