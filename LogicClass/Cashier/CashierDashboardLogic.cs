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

        public CashierDashboardLogic(BindingList<TransactionModels> transaction)
        {
            _transaction = transaction;
        }

        public double CalculateTotal()
        {
            return _transaction.Sum(t => t.Amount);

        }

        public int TotalTransaction => _transaction.Count;

    }
}
