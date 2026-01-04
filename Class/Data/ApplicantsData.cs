using LendingApp.Class.Models.CashierModels;
using LendingApp.Data;
using LendingApp.LogicClass.Cashier;
using LendingApp.Models.LoanOfficer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class ApplicantsData
{
    public BindingList<LoanModel> AllLoans { get; set; }
    public BindingList<LoanReleaseModels> releaseLoan { get; private set; }
    public BindingList<TransactionModels> recentTransactions { get; private set; }

    private CashierProcessLogic cashierProcessLogic;
    private string timeNow;

    public ApplicantsData()
    {
        AllLoans = new BindingList<LoanModel>();
        releaseLoan = new BindingList<LoanReleaseModels>();
        recentTransactions = new BindingList<TransactionModels>();

        cashierProcessLogic = new CashierProcessLogic();
        timeNow = cashierProcessLogic.GetTimeNow();

        AllLoans.ListChanged += (s, e) => RefreshDerivedLists();
    }


    public void LoadLoans(string applied, string type, string search)
    {
        var logic = new OfficerApplicationLogic(this);
        List<LoanModel> loans = logic.GetApplications(applied, type, search);
        AllLoans.Clear();

        foreach (var loan in loans)
        {
            AllLoans.Add(loan);
        }
    }

    public void RefreshDerivedLists()
    {
        releaseLoan.Clear();
        foreach (var l in AllLoans.Where(l => l.Applied == "Released" && l.Amount > 0))
        {

            releaseLoan.Add(new LoanReleaseModels
            {
                LoanNumber = l.LoanNumber,
                Borrower = l.Borrower,
                LoanType = l.LoanRef,
                Amount = l.Amount,
                ApprovedDate = l.ApprovedDate,
                TermMonths = l.TermMonths,
                InterestRate = l.InterestRate,
                ProcessingFee = l.ProcessingFee,
                Status = l.Applied
            });
        }

        recentTransactions.Clear();
        foreach (var l in AllLoans.Where(l => l.PaidAmount > 0))
        {
            recentTransactions.Add(new TransactionModels
            {
                Time = l.Time,
                Borrower = l.Borrower,
                PaidAmount = l.PaidAmount,
                ReceiptNo = l.ReceiptNo
            });
        }
    }

}
