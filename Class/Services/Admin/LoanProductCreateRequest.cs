using System;
using System.Collections.Generic;

namespace LendingApp.Class.Services.Admin
{
    public sealed class LoanProductCreateRequest
    {
        public string ProductName { get; set; }
        public string Description { get; set; }

        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

        public List<int> SelectedTerms { get; set; } = new List<int>();

        public decimal InterestRate { get; set; }
        public string InterestType { get; set; }      // "Fixed" | "Variable"
        public string InterestPeriod { get; set; }    // "Month" | "Year"

        public decimal ServiceFeePct { get; set; }    // mapped to processing_fee_pct
        public decimal? ServiceFeeFixedAmount { get; set; } // loan_products.service_fee_fixed_amount

        public int GracePeriodDays { get; set; }

        public decimal PenaltyRatePct { get; set; }   // loan_products.penalty_rate
        public string PenaltyPeriod { get; set; }     // "Day" | "Week" | "Month"

        public bool RequiresCollateral { get; set; }
        public bool IsActive { get; set; }

        // requirement_key -> requirement_text (null except "Others")
        public List<LoanProductRequirement> Requirements { get; set; } = new List<LoanProductRequirement>();
    }

    public sealed class LoanProductRequirement
    {
        public string Key { get; set; }   // e.g. "ValidId"
        public string Text { get; set; }  // only for "Others"
    }

    public sealed class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}