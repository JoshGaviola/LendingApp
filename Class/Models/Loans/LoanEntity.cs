using System;

namespace LendingApp.Class.Models.Loans
{
    public class LoanEntity
    {
        public int LoanId { get; set; }
        public string LoanNumber { get; set; }
        public int ApplicationId { get; set; }
        public string CustomerId { get; set; }
        public int ProductId { get; set; }

        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; } // annual percent (e.g. 12.00)
        public int TermMonths { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal OutstandingBalance { get; set; }

        public decimal TotalPaid { get; set; }
        public decimal TotalInterestPaid { get; set; }
        public decimal TotalPenaltyPaid { get; set; }

        public string Status { get; set; } // Active|Paid|Defaulted|Restructured|WrittenOff
        public int DaysOverdue { get; set; }

        public DateTime ReleaseDate { get; set; }
        public DateTime FirstDueDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }

        public string ReleaseMode { get; set; }
        public int? ReleasedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}