using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using LendingApp.Class;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Models.Loans;

namespace LendingApp.Class.Services.Loans
{
    public sealed class ApprovedLoanApplicationSummaryVm
    {
        // Header
        public string WindowTitle { get; set; }
        public string HeaderText { get; set; }

        // Summary rows
        public string ApplicationNumber { get; set; }
        public string Status { get; set; }
        public string CustomerDisplay { get; set; }
        public string LoanTypeText { get; set; }
        public string AppliedDateText { get; set; }
        public string AmountText { get; set; }
        public string ApprovedDateText { get; set; }
        public string TermText { get; set; }
        public string ApprovedByText { get; set; }
        public string Purpose { get; set; }

        // Computation rows
        public string PrincipalText { get; set; }
        public string InterestRateText { get; set; }
        public string ServiceFeeText { get; set; }
        public string TotalInterestText { get; set; }
        public string TotalPayableText { get; set; }
        public string MonthlyAmortizationText { get; set; }
        public string PaymentStartText { get; set; }

        // Approval details rows
        public string ApproverText { get; set; }
        public string ApprovalLevel { get; set; }
        public string ApprovalDateText { get; set; }
        public string Remarks { get; set; }
        public string CreditScoreText { get; set; }
    }

    public sealed class ApprovedLoanApplicationSummaryService
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoanApplicationEvaluationRepository _evalRepo;

        public ApprovedLoanApplicationSummaryService(
            ILoanApplicationRepository loanRepo,
            ICustomerRepository customerRepo,
            ILoanApplicationEvaluationRepository evalRepo)
        {
            _loanRepo = loanRepo;
            _customerRepo = customerRepo;
            _evalRepo = evalRepo;
        }

        public ApprovedLoanApplicationSummaryVm Build(string applicationNumber)
        {
            var app = _loanRepo.GetByApplicationNumber(applicationNumber);
            if (app == null) return null;

            var customer = _customerRepo.GetById(app.CustomerId);
            var eval = _evalRepo.GetLatestByApplicationId(app.ApplicationId);

            LoanProductEntity product;
            using (var db = new AppDbContext())
            {
                product = db.LoanProducts.AsNoTracking().FirstOrDefault(p => p.ProductId == app.ProductId);
            }

            var status = (app.Status ?? "N/A").Trim();

            // ---- Compute numbers (reuse the shared service) ----
            var principal = app.RequestedAmount;

            var interestRatePct = eval != null && eval.InterestRatePct.HasValue
                ? eval.InterestRatePct.Value
                : (product != null ? product.InterestRate : 0m);

            // NOTE: LoanApplicationEvaluationEntity signature shown in your context is truncated;
            // in your actual code file it has ServiceFeePct and TermMonths (used in your dialog).
            // We assume those properties exist since ApprovedLoanApplicationDialog currently compiles.
            var serviceFeePct = GetEvalServiceFeePct(eval, product);
            var termMonths = GetEvalTermMonths(eval, app);

            var methodText = (eval != null ? eval.InterestMethod : null) ?? "Diminishing Balance";
            var method = MapInterestMethod(methodText);

            var calc = LoanComputationService.Calculate(principal, interestRatePct, termMonths, serviceFeePct, method);

            // Payment start: 15th of next month (same behavior as the dialog had)
            var approvedDate = app.ApprovedDate ?? DateTime.Today;
            var nextMonth = new DateTime(approvedDate.Year, approvedDate.Month, 1).AddMonths(1);
            var paymentStart = new DateTime(nextMonth.Year, nextMonth.Month, 15);

            // ---- Build view model ----
            var vm = new ApprovedLoanApplicationSummaryVm
            {
                Status = status,
                WindowTitle = $"Loan Application - {app.ApplicationNumber} ({status})",
                HeaderText = $"Loan Application - {app.ApplicationNumber} ({status})",

                ApplicationNumber = app.ApplicationNumber,
                CustomerDisplay = BuildCustomerDisplay(app, customer),
                LoanTypeText = GetLoanTypeText(app, product),
                AppliedDateText = FormatDate(app.ApplicationDate),
                AmountText = Money(app.RequestedAmount),
                ApprovedDateText = app.ApprovedDate.HasValue ? FormatDate(app.ApprovedDate.Value) : "",
                TermText = (termMonths > 0 ? termMonths.ToString(CultureInfo.InvariantCulture) : "") + " months",
                ApprovedByText = GetApproverDisplay(app, eval),
                Purpose = app.Purpose ?? "",

                PrincipalText = Money(principal),
                InterestRateText = interestRatePct.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "% p.a. (" +
                                   (interestRatePct / 12m).ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "% monthly)",
                ServiceFeeText = Money(calc.ServiceFeeAmount) + " (" + serviceFeePct.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "%)",
                TotalInterestText = Money(calc.TotalInterest),
                TotalPayableText = Money(calc.TotalPayable),
                MonthlyAmortizationText = Money(calc.MonthlyPayment),
                PaymentStartText = paymentStart.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")),

                ApproverText = GetApproverDisplay(app, eval),
                ApprovalLevel = eval != null ? (eval.ApprovalLevel ?? "") : "",
                ApprovalDateText = eval != null ? eval.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.GetCultureInfo("en-US")) : "",
                Remarks = eval != null ? (eval.Remarks ?? "") : "",
                CreditScoreText = (customer != null ? customer.InitialCreditScore.ToString(CultureInfo.InvariantCulture) : "0") + "/1000"
            };

            return vm;
        }

        private static string BuildCustomerDisplay(LoanApplicationEntity app, CustomerRegistrationData customer)
        {
            if (customer == null) return app.CustomerId ?? "N/A";
            var name = ((customer.FirstName ?? "") + " " + (customer.LastName ?? "")).Trim();
            return name + " (" + (customer.CustomerId ?? "") + ")";
        }

        private static string GetLoanTypeText(LoanApplicationEntity app, LoanProductEntity product)
        {
            if (product != null && !string.IsNullOrWhiteSpace(product.ProductName))
                return product.ProductName;

            switch (app.ProductId)
            {
                case 1: return "Personal Loan";
                case 2: return "Emergency Loan";
                case 3: return "Salary Loan";
                default: return "Product " + app.ProductId.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static string GetApproverDisplay(LoanApplicationEntity app, LoanApplicationEvaluationEntity eval)
        {
            if (eval != null && eval.EvaluatedBy.HasValue)
                return "User #" + eval.EvaluatedBy.Value.ToString(CultureInfo.InvariantCulture);

            if (app.ApprovedBy.HasValue)
                return "User #" + app.ApprovedBy.Value.ToString(CultureInfo.InvariantCulture);

            return "";
        }

        private static string Money(decimal amount)
        {
            return "₱" + amount.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
        }

        private static string FormatDate(DateTime dt)
        {
            return dt.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
        }

        private static LoanInterestMethod MapInterestMethod(string uiText)
        {
            var m = (uiText ?? "").Trim();

            if (m.Equals("Diminishing Balance", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.DiminishingBalance;

            if (m.Equals("Flat Rate", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.FlatRate;

            if (m.Equals("Add-on Rate", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.AddOnRate;

            return LoanInterestMethod.FlatRate;
        }

        // These helpers keep this service compile-safe even if the evaluation fields vary.
        private static decimal GetEvalServiceFeePct(LoanApplicationEvaluationEntity eval, LoanProductEntity product)
        {
            // Your existing dialog uses _latestEval?.ServiceFeePct, so this should exist in your real model.
            // If not, this method safely falls back to product default.
            if (eval == null) return product != null ? product.ProcessingFeePct : 0m;

            var prop = eval.GetType().GetProperty("ServiceFeePct");
            if (prop == null) return product != null ? product.ProcessingFeePct : 0m;

            var v = prop.GetValue(eval, null);
            if (v == null) return product != null ? product.ProcessingFeePct : 0m;

            // explicit checks and casts (avoid pattern matching issues across compiler targets)
            if (v is decimal) return (decimal)v;
            if (v is decimal?) { var dn = (decimal?)v; if (dn.HasValue) return dn.Value; }

            // numeric could be boxed as double/float/int depending on source — try safe conversion
            try
            {
                return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
            }
            catch
            {
                return product != null ? product.ProcessingFeePct : 0m;
            }
        }

        private static int GetEvalTermMonths(LoanApplicationEvaluationEntity eval, LoanApplicationEntity app)
        {
            if (eval == null) return app != null && app.PreferredTerm > 0 ? app.PreferredTerm : 12;

            var prop = eval.GetType().GetProperty("TermMonths");
            if (prop == null) return app != null && app.PreferredTerm > 0 ? app.PreferredTerm : 12;

            var v = prop.GetValue(eval, null);
            if (v == null) return app != null && app.PreferredTerm > 0 ? app.PreferredTerm : 12;

            if (v is int) return (int)v;
            if (v is int?) { var inull = (int?)v; if (inull.HasValue) return inull.Value; }

            try
            {
                return Convert.ToInt32(v, CultureInfo.InvariantCulture);
            }
            catch
            {
                return app != null && app.PreferredTerm > 0 ? app.PreferredTerm : 12;
            }
        }
    }
}