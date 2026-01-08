using System;

namespace LendingApp.Class.LogicClass.LoanOfficer
{
    public static class ApplicationPriority
    {
        // Compute a normalized priority score 0..1
        public static double ComputePriorityScore(
            int daysWaiting,
            decimal amount,
            string loanType,
            int? creditScore = null,            // 300..850 typical; null if unknown
            bool docsComplete = true,
            int relationshipMonths = 0)
        {
            // normalize inputs (0..1)
            double daysN = Math.Min(daysWaiting / 30.0, 1.0);                       // 0..30+ days
            double amountN = Math.Min((double)amount / 200000.0, 1.0);              // scale by expected max loan
            double relationshipN = Math.Min(relationshipMonths / 60.0, 1.0);        // 0..60 months

            // loan type urgency mapping (C#7-compatible)
            string loanTypeLower = (loanType ?? string.Empty).ToLowerInvariant();
            double loanTypeUrgency;
            if (loanTypeLower.Contains("emergency"))
                loanTypeUrgency = 1.0;
            else if (loanTypeLower.Contains("salary"))
                loanTypeUrgency = 0.8;
            else if (loanTypeLower.Contains("personal"))
                loanTypeUrgency = 0.6;
            else if (loanTypeLower.Contains("business"))
                loanTypeUrgency = 0.5;
            else
                loanTypeUrgency = 0.5;

            // credit risk: higher risk => higher priority; if unknown assume medium
            double creditRiskN;
            if (creditScore.HasValue)
            {
                double v = 1.0 - (creditScore.Value - 300) / 550.0; // 300->1, 850->0
                creditRiskN = Clamp(v, 0.0, 1.0);
            }
            else
            {
                creditRiskN = 0.5;
            }

            // Weights (tune for your business)
            const double wDays = 0.35;
            const double wLoanType = 0.25;
            const double wAmount = 0.20;
            const double wCredit = 0.15;
            const double wRelationship = 0.05;

            // penalty for incomplete docs
            double docsPenalty = docsComplete ? 0.0 : -0.10;

            double raw =
                daysN * wDays +
                loanTypeUrgency * wLoanType +
                amountN * wAmount +
                creditRiskN * wCredit +
                (1.0 - relationshipN) * wRelationship + // shorter relationship => slightly higher priority
                docsPenalty;

            // clamp to 0..1
            return Clamp(raw, 0.0, 1.0);
        }

        public static string GetPriorityLabel(double score)
        {
            if (score >= 0.85) return "Critical";
            if (score >= 0.65) return "High";
            if (score >= 0.40) return "Medium";
            return "Low";
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}