using LendingApp.Data;
using LendingApp.LogicClass.Cashier;
using LendingApp.Models.CashierModels;
using System;
using System.ComponentModel;
using System.Linq;
public class DataSample
{
    public BindingList<LoanModel> AllLoans { get; set; }
    public BindingList<LoanReleaseModels> releaseLoan { get; set; }
    public BindingList<TransactionModels> recentTransactions { get; set; }
    private CashierProcessLogic cashierProcessLogic;
    private string timeNow;

    public DataSample()
    {
        AllLoans = new BindingList<LoanModel>();
        releaseLoan = new BindingList<LoanReleaseModels>();
        recentTransactions = new BindingList<TransactionModels>();
        cashierProcessLogic = new CashierProcessLogic();

        timeNow = cashierProcessLogic.GetTimeNow();
        LoadSampleData();
        LoadReleaseLoanData();
        LoadRecentTransactions();
    }

    private void LoadReleaseLoanData()
    {
        releaseLoan = new BindingList<LoanReleaseModels>(
         AllLoans
             .Where(l => l.Status == "Released" && l.Amount > 0)
             .Select(l => new LoanReleaseModels
             {
                 LoanNumber = l.LoanNumber,
                 Borrower = l.Borrower,
                 LoanType = l.LoanRef,
                 Amount = l.Amount,
                 ApprovedDate = l.ApprovedDate,
                 TermMonths = l.TermMonths,
                 InterestRate = l.InterestRate,
                 ProcessingFee = l.ProcessingFee,
                 Status = l.Status
             }).ToList()
     );
    }

    private void LoadRecentTransactions()
    {
       recentTransactions = new BindingList<TransactionModels>(
         AllLoans
             .Where(l => l.PaidAmount > 0)
             .Select(l => new TransactionModels
             {
                 Time = l.Time,
                 Borrower = l.Borrower,
                 PaidAmount = l.PaidAmount,
                 ReceiptNo = l.ReceiptNo
               
             }).ToList()
     );
    }

    private void LoadSampleData()
    {
        // Pending Loan
        AllLoans.Add(new LoanModel
        {
            LoanNumber = "LN-2001",
            Borrower = "Pedro Reyes",
            LoanRef = "Personal Loan",
            Amount = 5000m,
            ApprovedDate = DateTime.Today.AddDays(-2),
            TermMonths = 12,
            InterestRate = 12.5m,
            ProcessingFee = 250m,
            Status = "Pending"
        });

        // Docs Needed
        AllLoans.Add(new LoanModel
        {
            LoanNumber = "LN-2002",
            Borrower = "Ana Lopez",
            LoanRef = "Salary Loan",
            Amount = 3200m,
            ApprovedDate = DateTime.Today.AddDays(-1),
            TermMonths = 6,
            InterestRate = 10m,
            ProcessingFee = 150m,
            Status = "Released"
        });

        // Released / Paid Transaction
        AllLoans.Add(new LoanModel
        {
            LoanNumber = "LN-1001",
            Borrower = "Juan Dela Cruz",
            LoanRef = "Personal Loan",
            Amount = 22500m,
            Time = timeNow,
            ReceiptNo = "OR-001",
            PaymentTime = DateTime.Today,
            Status = "Released"
        });

        AllLoans.Add(new LoanModel
        {
            LoanNumber = "LN-2002",
            ReceiptNo = "NIGGA-101",
            Borrower = "Manny Ga",
            LoanRef = "Salary Loan",
            PaidAmount = 3200m,
            Time = timeNow,
            TermMonths = 6,
            InterestRate = 10m,
            ProcessingFee = 150m,
            Status = "Paid"
        });

        AllLoans.Add(new LoanModel
        {
            LoanNumber = "LN-2002",
            Borrower = "Ana Lopez",
            LoanRef = "Salary Loan",
            Amount = 3200m,
            Time = timeNow,
            TermMonths = 6,
            InterestRate = 10m,
            ProcessingFee = 150m,
            Status = "Released"
        });
    }
}
