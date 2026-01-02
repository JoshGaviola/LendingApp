using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingApp.Interface;

namespace LendingApp.Class.Models.CashierModels
{
    public class LoanReleaseModels : IHasAmount
    {
        public string LoanNumber { get; set; }
        public string Borrower { get; set; }
        public string LoanType { get; set; }
        public decimal Amount { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int TermMonths { get; set; }
        public decimal InterestRate { get; set; } // annual percent
        public decimal ProcessingFee { get; set; }
        public string Status { get; set; } // Ready | Pending | Docs Needed
    }

}
