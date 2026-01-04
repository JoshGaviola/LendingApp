using System;
using LendingApp.Class.Models.LoanOfiicerModels;

namespace LendingApp.Class.Services
{
    public sealed class CreditScoreBreakdown
    {
        public int PaymentHistory { get; set; }       // 0..100 (proxy for new customer)
        public int CreditUtilization { get; set; }    // 0..100 (proxy/default)
        public int CreditHistoryLength { get; set; }  // 0..100 (proxy/default)
        public int IncomeStability { get; set; }      // 0..100 (derived)

        public int TotalScore1000 { get; set; }       // 0..1000
    }

    public static class CreditScoringService
    {
        // weights aligned with OfficerEvaluateApplicationForm
        private const decimal W_PH = 35m;
        private const decimal W_CU = 30m;
        private const decimal W_CHL = 15m;
        private const decimal W_IS = 20m;

        public static CreditScoreBreakdown CalculateInitial(CustomerRegistrationData c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));

            var score = new CreditScoreBreakdown
            {
                // New customers: proxies
                PaymentHistory = ScoreIdentityAndDocs(c),
                CreditUtilization = ScoreCreditUtilization(c),
                CreditHistoryLength = ScoreHistoryLength(c),
                IncomeStability = ScoreIncomeStability(c)
            };

            decimal total100 =
                (score.PaymentHistory / 100m) * W_PH +
                (score.CreditUtilization / 100m) * W_CU +
                (score.CreditHistoryLength / 100m) * W_CHL +
                (score.IncomeStability / 100m) * W_IS;

            score.TotalScore1000 = ClampInt((int)Math.Round(total100 * 10m), 0, 1000);
            return score;
        }

        /// <summary>
        /// Recompute the score based on current customer details.
        /// This is intended for EDIT scenarios (add/remove details).
        /// </summary>
        public static int RecalculateScore1000(CustomerRegistrationData c)
        {
            return CalculateInitial(c).TotalScore1000;
        }

        /// <summary>
        /// Conservative initial credit limit suggestion based on score only.
        /// </summary>
        public static decimal SuggestCreditLimit(int score1000)
        {
            if (score1000 >= 850) return 100_000m;
            if (score1000 >= 750) return 50_000m;
            if (score1000 >= 650) return 25_000m;
            if (score1000 >= 550) return 10_000m;
            return 5_000m;
        }

        /// <summary>
        /// If you want credit limit to auto-adjust upward when profile improves.
        /// Never lowers the limit to avoid surprising the user.
        /// </summary>
        public static decimal SuggestCreditLimitNoDecrease(int score1000, decimal currentLimit)
        {
            var suggested = SuggestCreditLimit(score1000);
            return currentLimit > suggested ? currentLimit : suggested;
        }

        /// <summary>
        /// Minimum score requirement per loan type.
        /// Score uses the 0..1000 scale used throughout the app.
        /// </summary>
        public static int GetMinimumApprovalScore(string loanType)
        {
            var t = (loanType ?? "").Trim();

            if (t.Equals("Personal Loan", StringComparison.OrdinalIgnoreCase)) return 650;
            if (t.Equals("Emergency Loan", StringComparison.OrdinalIgnoreCase)) return 600;
            if (t.Equals("Salary Loan", StringComparison.OrdinalIgnoreCase)) return 620;

            // Unknown => no strict rule (0 means "not enforced")
            return 0;
        }

        public static bool MeetsMinimumScore(string loanType, int creditScore1000, out int minimumScore)
        {
            minimumScore = GetMinimumApprovalScore(loanType);
            if (minimumScore <= 0) return true;
            return creditScore1000 >= minimumScore;
        }

        private static int ScoreIdentityAndDocs(CustomerRegistrationData c)
        {
            int points = 0;
            int max = 0;

            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.FirstName));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.LastName));
            Add(ref points, ref max, c.DateOfBirth.HasValue);
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.MobileNumber));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.PresentAddress));

            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.ValidId1Path));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.ProofOfIncomePath));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.ProofOfAddressPath));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.SignatureImagePath));

            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.ValidId2Path));

            return Percent(points, max);
        }

        private static int ScoreIncomeStability(CustomerRegistrationData c)
        {
            int points = 0;
            int max = 0;

            var employed = string.Equals(c.EmploymentStatus, "Employed", StringComparison.OrdinalIgnoreCase);
            var selfEmployed = string.Equals(c.EmploymentStatus, "Self-Employed", StringComparison.OrdinalIgnoreCase);

            max += 2;
            if (employed) points += 2;
            else if (selfEmployed) points += 1;

            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.CompanyName));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.Position));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.CompanyAddress));
            Add(ref points, ref max, !string.IsNullOrWhiteSpace(c.CompanyPhone));

            max += 2;
            if (!string.IsNullOrWhiteSpace(c.ProofOfIncomePath)) points += 2;

            return Percent(points, max);
        }

        private static int ScoreHistoryLength(CustomerRegistrationData c)
        {
            if (!c.DateOfBirth.HasValue) return 50;

            var age = (int)((DateTime.Today - c.DateOfBirth.Value.Date).TotalDays / 365.2425);
            if (age < 18) return 0;
            if (age < 21) return 40;
            if (age < 30) return 55;
            if (age < 45) return 65;
            if (age < 60) return 60;
            return 50;
        }

        private static int ScoreCreditUtilization(CustomerRegistrationData c)
        {
            var baseScore = 50;
            if (!string.IsNullOrWhiteSpace(c.BankName) && !string.IsNullOrWhiteSpace(c.BankAccountNumber))
                baseScore = 60;

            return baseScore;
        }

        private static void Add(ref int points, ref int max, bool ok)
        {
            max += 1;
            if (ok) points += 1;
        }

        private static int Percent(int points, int max)
        {
            if (max <= 0) return 0;
            return ClampInt((int)Math.Round((points * 100m) / max), 0, 100);
        }

        private static int ClampInt(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}