// Remove this duplicate class definition if another definition of LoanApplicationEvaluationEntity exists in the same namespace.
// Ensure that only one definition of LoanApplicationEvaluationEntity exists in the 'LendingApp.Class.Models.Loans' namespace.
// If this is the intended definition, delete or rename the other duplicate(s).
// If you want to keep both, consider renaming one of the classes.

using System;

namespace LendingApp.Class.Models.Loans
{
    public class LoanApplicationEvaluationEntity
    {
        public long EvaluationId { get; set; }
        public int ApplicationId { get; set; }

        public decimal C1Input { get; set; }
        public decimal C2Input { get; set; }
        public decimal C3Input { get; set; }
        public decimal C4Input { get; set; }

        public decimal W1Pct { get; set; }
        public decimal W2Pct { get; set; }
        public decimal W3Pct { get; set; }
        public decimal W4Pct { get; set; }

        public decimal C1Weighted { get; set; }
        public decimal C2Weighted { get; set; }
        public decimal C3Weighted { get; set; }
        public decimal C4Weighted { get; set; }
        public decimal TotalScore { get; set; }

        public string Decision { get; set; }
        public string InterestMethod { get; set; }
        public decimal? InterestRatePct { get; set; }
        public decimal? ServiceFeePct { get; set; }
        public int? TermMonths { get; set; }

        public string ApprovalLevel { get; set; }

        public bool RequireComaker { get; set; }
        public bool ReduceAmount { get; set; }
        public bool ShortenTerm { get; set; }
        public bool AdditionalCollateral { get; set; }

        public string RejectionReason { get; set; }
        public string Remarks { get; set; }

        public int? EvaluatedBy { get; set; }
        public string StatusAfter { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}