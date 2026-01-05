using System;

namespace LendingApp.Class.Models.Loans
{
    public class CollectionEntity
    {
        public int CollectionId { get; set; }
        public int LoanId { get; set; }
        public string CustomerId { get; set; }

        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; }

        public int DaysOverdue { get; set; }
        public string Priority { get; set; } // Low|Medium|High|Critical
        public string Status { get; set; }   // Pending|Contacted|PromiseToPay|Paid|Escalated

        public DateTime? LastContactDate { get; set; }
        public string LastContactMethod { get; set; }
        public string Notes { get; set; }

        public DateTime? PromiseDate { get; set; }
        public int? AssignedOfficerId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}