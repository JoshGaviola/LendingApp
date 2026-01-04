using System.Collections.Generic;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Interface
{
    public interface ILoanApplicationRepository
    {
        void Add(LoanApplicationEntity application);
        LoanApplicationEntity GetByApplicationNumber(string applicationNumber);
        IEnumerable<LoanApplicationEntity> GetAll();
        void Update(LoanApplicationEntity application);
    }
}