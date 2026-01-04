using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Repo
{
    public sealed class LoanRepository : ILoanRepository
    {
        public LoanEntity GetByApplicationId(int applicationId)
        {
            using (var db = new AppDbContext())
            {
                return db.Loans.AsNoTracking().FirstOrDefault(x => x.ApplicationId == applicationId);
            }
        }

        public void Add(LoanEntity loan)
        {
            using (var db = new AppDbContext())
            {
                db.Loans.Add(loan);
                db.SaveChanges();
            }
        }

        public IEnumerable<LoanEntity> GetLoansForRelease()
        {
            using (var db = new AppDbContext())
            {
                // For now: show loans that exist (created on approval) and are Active
                return db.Loans
                    .AsNoTracking()
                    .Where(l => l.Status == "Active")
                    .OrderByDescending(l => l.CreatedDate)
                    .ToList();
            }
        }
    }
}