using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Repo
{
    public class LoanApplicationEvaluationRepository : ILoanApplicationEvaluationRepository
    {
        public void Add(LoanApplicationEvaluationEntity evaluation)
        {
            using (var db = new AppDbContext())
            {
                db.LoanApplicationEvaluations.Add(evaluation);
                db.SaveChanges();
            }
        }

        public IEnumerable<LoanApplicationEvaluationEntity> GetByApplicationId(int applicationId)
        {
            using (var db = new AppDbContext())
            {
                return db.LoanApplicationEvaluations
                    .AsNoTracking()
                    .Where(x => x.ApplicationId == applicationId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();
            }
        }

        public LoanApplicationEvaluationEntity GetLatestByApplicationId(int applicationId)
        {
            using (var db = new AppDbContext())
            {
                return db.LoanApplicationEvaluations
                    .AsNoTracking()
                    .Where(x => x.ApplicationId == applicationId)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();
            }
        }
    }
}