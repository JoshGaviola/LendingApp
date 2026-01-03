using System;

namespace LendingApp.Class.Models.Loans
{
    public class LoanApplicationEntity
    {
        public int ApplicationId { get; set; }
        public string ApplicationNumber { get; set; }
        public string CustomerId { get; set; }
        public int ProductId { get; set; }
        public decimal RequestedAmount { get; set; }
        public int PreferredTerm { get; set; }
        public string Purpose { get; set; }
        public DateTime? DesiredReleaseDate { get; set; }
        public string Status { get; set; }           // enum in DB, keep string in EF6
        public string Priority { get; set; }         // enum in DB, keep string in EF6
        public string RejectionReason { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? AssignedOfficerId { get; set; }
        public int? ApprovedBy { get; set; }
    }
}