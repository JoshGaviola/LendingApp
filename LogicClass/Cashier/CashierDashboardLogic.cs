using LendingApp.Interface;
using LendingApp.Models.CashierModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.LogicClass.Cashier
{
   public class CashierDashboardLogic : ITotalCalc
    {
        private BindingList<TransactionModels> _transaction;
        private BindingList<LoanReleaseModels> _pendingLoan;

        public CashierDashboardLogic(DataSample data)
        {
            _transaction = data.Transactions;
            _pendingLoan = data.PendingLoans;
        }

        public decimal CalculateTotal<T>(BindingList<T> list) where T : IHasAmount
        {
            return list.Sum(t => t.Amount);

        }

        public int TotalTransaction => _transaction.Count;
        public int TotalLoanRelease => _pendingLoan.Count;


    }
}
