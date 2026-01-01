using LendingApp.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Data
{
   public class LoanModel : IHasAmount
    {
                // Identity
                public string LoanNumber { get; set; }
                public string Borrower { get; set; }
                
                public string Customer { get; set; }

                public string Time { get; set; }

                // Loan info
                public string LoanRef { get; set; }
                public decimal LoanAmount { get; set; }
                public DateTime ApprovedDate { get; set; }
                public int TermMonths { get; set; }
                public decimal InterestRate { get; set; }
                public decimal ProcessingFee { get; set; }
                public string Status { get; set; }   // Pending, Docs Needed, Released

                // Payment / transaction info (SUMMARY)
                public decimal PaidAmount { get; set; }
                public decimal Amount { get; set; }
                public string ReceiptNo { get; set; }
                public DateTime? PaymentTime { get; set; }

             
    }

   }
   

