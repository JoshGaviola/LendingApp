using System.Collections.Generic;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Interface
{
    public interface ILoanApplicationEvaluationRepository
    {
        void Add(LoanApplicationEvaluationEntity evaluation);
        IEnumerable<LoanApplicationEvaluationEntity> GetByApplicationId(int applicationId);
        LoanApplicationEvaluationEntity GetLatestByApplicationId(int applicationId);
    }
}