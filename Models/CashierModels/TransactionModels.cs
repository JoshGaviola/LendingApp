using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Interface;

namespace LendingApp.Models.CashierModels
{
    public class TransactionModels : IHasAmount
    {
        public string Time { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string LoanRef { get; set; }
    }
}
