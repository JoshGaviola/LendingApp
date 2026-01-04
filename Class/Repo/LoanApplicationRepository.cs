using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Repo
{
    /// <summary>
    /// Concrete implementation that uses EF + AppDbContext.
    /// Only this class knows about EF.
    /// </summary>
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        public void Add(LoanApplicationEntity application)
        {
            using (var db = new AppDbContext())
            {
                db.LoanApplications.Add(application);
                db.SaveChanges();
            }
        }

        public LoanApplicationEntity GetByApplicationNumber(string applicationNumber)
        {
            using (var db = new AppDbContext())
            {
                return db.LoanApplications
                    .AsNoTracking()
                    .FirstOrDefault(a => a.ApplicationNumber == applicationNumber);
            }
        }

        public IEnumerable<LoanApplicationEntity> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.LoanApplications.AsNoTracking().ToList();
            }
        }

        public void Update(LoanApplicationEntity application)
        {
            using (var db = new AppDbContext())
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}