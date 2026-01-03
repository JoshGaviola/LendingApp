using System;

namespace LendingApp.Class.Models.Loans
{
    public class LoanProductEntity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }

        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

        public int MinTermMonths { get; set; }
        public int MaxTermMonths { get; set; }

        public decimal InterestRate { get; set; }
        public decimal ProcessingFeePct { get; set; }
        public decimal PenaltyRate { get; set; }

        public int GracePeriodDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}