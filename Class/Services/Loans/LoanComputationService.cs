using System;
using System.Collections.Generic;

namespace LendingApp.Class.Services.Loans
{
    public enum LoanInterestMethod
    {
        DiminishingBalance,
        FlatRate,
        AddOnRate
    }

    public sealed class LoanComputationResult
    {
        public decimal MonthlyPayment { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayable { get; set; } // includes service fee
        public decimal AprPct { get; set; }
        public decimal ServiceFeeAmount { get; set; }
    }

    // New: amortization row model
    public sealed class AmortizationRow
    {
        public int Month { get; set; }
        public decimal Payment { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Balance { get; set; }
    }

    public static class LoanComputationService
    {
        public static LoanComputationResult Calculate(
            decimal principal,
            decimal annualRatePct,
            int termMonths,
            decimal serviceFeePct,
            LoanInterestMethod method)
        {
            if (principal <= 0m || termMonths <= 0)
                return new LoanComputationResult();

            var annualRate = annualRatePct / 100m;
            var monthlyRate = annualRate / 12m;

            decimal monthlyPayment;
            decimal totalPayableNoFee;
            decimal totalInterest;

            if (method == LoanInterestMethod.DiminishingBalance)
            {
                monthlyPayment = Pmt(monthlyRate, termMonths, principal);
                totalPayableNoFee = monthlyPayment * termMonths;
                totalInterest = totalPayableNoFee - principal;
            }
            else
            {
                // Flat/Add-on: interest computed on principal across term
                totalInterest = principal * annualRate * (termMonths / 12m);
                totalPayableNoFee = principal + totalInterest;
                monthlyPayment = termMonths > 0 ? (totalPayableNoFee / termMonths) : 0m;
            }

            var serviceFeeAmount = principal * (serviceFeePct / 100m);

            // Same APR approximation as before (needs changing?)
            var apr = annualRatePct + ((serviceFeeAmount / principal) * (12m / termMonths) * 100m);

            return new LoanComputationResult
            {
                MonthlyPayment = RoundMoney(monthlyPayment),
                TotalInterest = RoundMoney(totalInterest),
                TotalPayable = RoundMoney(totalPayableNoFee + serviceFeeAmount),
                AprPct = Math.Round(apr, 2),
                ServiceFeeAmount = RoundMoney(serviceFeeAmount)
            };
        }

        // New: produce an amortization schedule (full schedule)
        public static List<AmortizationRow> GetAmortizationSchedule(
            decimal principal,
            decimal annualRatePct,
            int termMonths,
            decimal serviceFeePct,
            LoanInterestMethod method)
        {
            var rows = new List<AmortizationRow>();

            if (principal <= 0m || termMonths <= 0)
                return rows;

            var annualRate = annualRatePct / 100m;
            var monthlyRate = annualRate / 12m;
            int n = termMonths;

            if (method == LoanInterestMethod.DiminishingBalance)
            {
                var monthlyPayment = Pmt(monthlyRate, n, principal);
                decimal balance = principal;

                for (int month = 1; month <= n; month++)
                {
                    decimal interest = RoundMoney(balance * monthlyRate);
                    decimal principalPortion = RoundMoney(monthlyPayment - interest);

                    // guard rounding: last payment may need adjustment
                    if (month == n)
                    {
                        principalPortion = balance;
                        monthlyPayment = principalPortion + interest;
                    }

                    balance = RoundMoney(balance - principalPortion);
                    if (balance < 0m) balance = 0m;

                    rows.Add(new AmortizationRow
                    {
                        Month = month,
                        Payment = RoundMoney(monthlyPayment),
                        Principal = principalPortion,
                        Interest = interest,
                        Balance = balance
                    });
                }
            }
            else // FlatRate / AddOnRate - equal interest slice each month
            {
                decimal totalInterest = principal * annualRate * (n / 12m);
                decimal totalPayableNoFee = principal + totalInterest;
                decimal monthlyPayment = n > 0 ? (totalPayableNoFee / n) : 0m;
                decimal interestPortion = RoundMoney(totalInterest / n);
                decimal principalPortion = RoundMoney(monthlyPayment - interestPortion);
                decimal balance = principal;

                for (int month = 1; month <= n; month++)
                {
                    // last month adjust principal portion to clear rounding drift
                    if (month == n)
                    {
                        principalPortion = balance;
                        monthlyPayment = principalPortion + interestPortion;
                    }

                    balance = RoundMoney(balance - principalPortion);
                    if (balance < 0m) balance = 0m;

                    rows.Add(new AmortizationRow
                    {
                        Month = month,
                        Payment = RoundMoney(monthlyPayment),
                        Principal = principalPortion,
                        Interest = interestPortion,
                        Balance = balance
                    });
                }
            }

            return rows;
        }

        private static decimal Pmt(decimal ratePerPeriod, int numberOfPeriods, decimal presentValue)
        {
            if (numberOfPeriods <= 0) return 0m;
            if (ratePerPeriod == 0m) return presentValue / numberOfPeriods;

            // PMT = r*PV / (1 - (1+r)^-n)
            var r = (double)ratePerPeriod;
            var pv = (double)presentValue;
            var n = numberOfPeriods;

            var denom = 1.0 - Math.Pow(1.0 + r, -n);
            if (denom == 0.0) return 0m;

            return (decimal)((r * pv) / denom);
        }

        private static decimal RoundMoney(decimal x) => Math.Round(x, 2, MidpointRounding.AwayFromZero);
    }
}