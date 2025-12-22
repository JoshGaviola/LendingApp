using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.Loan_Product
{
    public class LoanProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string LoanType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal InterestRate { get; set; }
        public string InterestType { get; set; }
        public decimal ServiceCharge { get; set; }
        public int AvailableTerms { get; set; }
        public int GracePeriodDays { get; set; }
        public decimal PenaltyRate { get; set; }
        public bool RequiresCollateral { get; set; }
        public string RequiredDocuments { get; set; }
        public bool isActive { get; set; }

        public void ValidateEligibility()
        {
            //Implement eligibility
        }
        public void CalculateInterest()
        {
            //Code to calculate interest
        }
    }
}
