using LendingApp.Interface;
using LendingApp.Models.CashierModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.LogicClass.Cashier
{
   public class CashierDashboardLogic : ITotalCalc
    {
        public List<TransactionModels> _transaction;

        public CashierDashboardLogic(List<TransactionModels> transaction)
        {
            _transaction = transaction;
        }

        public double CalculateTotal()
        {
            return _transaction.Sum(t => t.Amount);
        }
    }
}
