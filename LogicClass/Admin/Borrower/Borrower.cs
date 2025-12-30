using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models
{
    public class Borrower : User
    {
        private int CustomerId { get; set; }

        public void SubmitApplication()
        {
            // Implementation for submitting a loan application
        }
        public string ViewLoanStatus()
        {
           return "Loan Status";
        }
        public void ViewPaymentHistory()
        {
            // Implementation for viewing payment history
        }
    }
}
