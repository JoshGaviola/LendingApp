using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.CashierModels
{
    public class TransactionModels
    {
        public string Time { get; set; }
        public string Customer { get; set; }
        public double Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string LoanRef { get; set; }
    }
}
