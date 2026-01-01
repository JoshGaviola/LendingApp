using LendingApp.Data;
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
   public class CashierDashboardLogic
    {
        private BindingList<TransactionModels> transaction;
        private BindingList<LoanReleaseModels> releaseLoan;
        public CashierDashboardLogic(DataSample data)
        {
            transaction = data.recentTransactions;
            releaseLoan = data.releaseLoan;
        }
        public decimal CalculateTotalRecentTransaction()
        {
            return transaction.Sum(t => t.PaidAmount);

        }
        public decimal CalculateTotalLoansPending()
        {
            return releaseLoan.Sum(l => l.Amount);
        }

        public int TotalTransaction => transaction.Count;
        public int TotalLoansPending => releaseLoan.Count;


    }
}