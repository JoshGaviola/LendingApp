using System;

namespace LendingApp.Class.Models.CashierModels
{
    public class PaymentEntity
    {
        public int PaymentId { get; set; }

        public int LoanId { get; set; }
        public string CustomerId { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }

        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal PenaltyPaid { get; set; }

        // NEW: required by DB schema
        public decimal BalanceAfter { get; set; }
        public int ProcessedBy { get; set; }

        // Optional columns in DB
        public string ReferenceNumber { get; set; }
        public string Remarks { get; set; }

        public string PaymentMethod { get; set; }
        public string ReceiptNo { get; set; }

        // Not in DB (ignored in AppDbContext)
        public DateTime? CreatedDate { get; set; }
    }
}