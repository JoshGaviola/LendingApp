using System;

namespace LendingApp.UI.CashierUI
{
    // Lightweight DTO used to share receipt info between forms
    public class ReceiptDto
    {
        public string Id { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Customer { get; set; }
        public string LoanAccount { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public decimal Charges { get; set; }
        public string PaymentMode { get; set; } // Cash | GCash | Bank
        public string Cashier { get; set; }
        public string Type { get; set; } // Payment | Loan
        public string Status { get; set; } // Printed | Emailed | Voided
    }
}