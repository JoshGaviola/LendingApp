using LendingApp.Class;
using LendingApp.Class.Models.CashierModels;
using LendingApp.Data;
using LendingApp.LogicClass.Cashier;
using LendingApp.Models.LoanOfficer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
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

        // NEW: persist Recent Transactions across app restarts
        LoadRecentTransactionsFromCollections();
    }

    private void LoadRecentTransactionsFromCollections(int take = 50)
    {
        try
        {
            using (var db = new AppDbContext())
            {
                // collections table does NOT store receipt_no directly;
                // we will read it back from Notes ("Receipt: OR-001") which we already write.
                var rows = db.Collections.AsNoTracking()
                    .Where(c => c.Status == "Paid")
                    .OrderByDescending(c => c.UpdatedDate)
                    .Take(take)
                    .Select(c => new
                    {
                        c.UpdatedDate,
                        c.CustomerId,
                        c.AmountDue,
                        c.Notes
                    })
                    .ToList();

                recentTransactions.Clear();

                foreach (var r in rows)
                {
                    recentTransactions.Add(new TransactionModels
                    {
                        Time = r.UpdatedDate.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture),
                        Borrower = r.CustomerId, // shows ID (safe). If you want name, we can join customers.
                        PaidAmount = r.AmountDue,
                        ReceiptNo = ExtractReceiptNoFromNotes(r.Notes),
                        LoanRef = "" // not stored in TransactionModels UI anyway
                    });
                }
            }
        }
        catch
        {
            // don't break startup if DB read fails
        }
    }

    private static string ExtractReceiptNoFromNotes(string notes)
    {
        // Notes format: "Payment received. Receipt: OR-001"
        if (string.IsNullOrWhiteSpace(notes)) return "";
        var key = "Receipt:";
        var idx = notes.IndexOf(key, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return "";
        return notes.Substring(idx + key.Length).Trim();
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

        // Keep this for compatibility (it will work if AllLoans is populated with PaidAmount),
        // but startup now uses `payments` table as the authoritative source.
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
