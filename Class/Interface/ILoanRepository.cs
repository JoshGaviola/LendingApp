using System.Collections.Generic;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Interface
{
    public interface ILoanRepository
    {
        LoanEntity GetByApplicationId(int applicationId);
        void Add(LoanEntity loan);

        // NEW: for cashier release table
        IEnumerable<LoanEntity> GetLoansForRelease();
    }
}