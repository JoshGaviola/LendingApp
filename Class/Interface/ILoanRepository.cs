using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Interface
{
    public interface ILoanRepository
    {
        LoanEntity GetByApplicationId(int applicationId);
        void Add(LoanEntity loan);
    }
}