using LendingApp.Models.CashierModels;
using System;
using System.ComponentModel;

namespace LendingApp.LogicClass.Cashier
{
    public class DataSample
    {
        public BindingList<TransactionModels> Transactions { get; set; }
        public BindingList<LoanReleaseModels> PendingLoans { get; set; }

        public DataSample()
        {
            Transactions = new BindingList<TransactionModels>();
            PendingLoans = new BindingList<LoanReleaseModels>();

            LoadTransactions();
            LoadPendingLoans();
        }

        private void LoadTransactions()
        {   
            Transactions.Add(new TransactionModels
            {
                Customer = "Juan Dela Cruz",
                Amount = 2500m,
                ReceiptNo = "OR-001",
                LoanRef = "LN-1001",
                Time = "9:30 AM"
            });

            Transactions.Add(new TransactionModels
            {
                Customer = "Maria Santos",
                Amount = 1800m,
                ReceiptNo = "OR-002",
                LoanRef = "LN-1002",
                Time = "10:15 AM"
            });
            Transactions.Add(new TransactionModels
            {
                Customer = "Juan Dela Cruz",
                Amount = 2500m,
                ReceiptNo = "OR-001",
                LoanRef = "LN-1001",
                Time = "9:30 AM"
            });

        }
        private void LoadPendingLoans()
        {
            PendingLoans.Add(new LoanReleaseModels
            {
                LoanNumber = "LN-2001",
                Borrower = "Pedro Reyes",
                LoanType = "Personal Loan",
                Amount = 5000m,
                ApprovedDate = DateTime.Today.AddDays(-2),
                TermMonths = 12,
                InterestRate = 12.5m,
                ProcessingFee = 250m,
                Status = "Pending"
            });

            PendingLoans.Add(new LoanReleaseModels
            {
                LoanNumber = "LN-2002",
                Borrower = "Ana Lopez",
                LoanType = "Salary Loan",
                Amount = 3200m,
                ApprovedDate = DateTime.Today.AddDays(-1),
                TermMonths = 6,
                InterestRate = 10m,
                ProcessingFee = 150m,
                Status = "Docs Needed"
            });
            PendingLoans.Add(new LoanReleaseModels
            {
                LoanNumber = "LN-2001",
                Borrower = "Pedro Reyes",
                LoanType = "Personal Loan",
                Amount = 5000m,
                ApprovedDate = DateTime.Today.AddDays(-2),
                TermMonths = 12,
                InterestRate = 12.5m,
                ProcessingFee = 250m,
                Status = "Pending"
            });

            PendingLoans.Add(new LoanReleaseModels
            {
                LoanNumber = "LN-2002",
                Borrower = "Ana Lopez",
                LoanType = "Salary Loan",
                Amount = 3200m,
                ApprovedDate = DateTime.Today.AddDays(-1),
                TermMonths = 6,
                InterestRate = 10m,
                ProcessingFee = 150m,
                Status = "Docs Needed"
            });
        }


    }
}
