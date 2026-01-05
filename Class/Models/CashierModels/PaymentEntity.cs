using System;

namespace LendingApp.Class.Models.CashierModels
{
    /// <summary>
    /// Persists cashier payment transactions so "Recent Transactions" survives app restart.
    /// MySQL table expected: `payments`
    /// </summary>
    public class PaymentEntity
    {
        public int PaymentId { get; set; }

        public int LoanId { get; set; }
        public string CustomerId { get; set; }

        public DateTime PaymentDate { get; set; }   // date+time
        public decimal AmountPaid { get; set; }

        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal PenaltyPaid { get; set; }

        public string PaymentMethod { get; set; }   // Cash|GCash|Bank
        public string ReceiptNo { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}