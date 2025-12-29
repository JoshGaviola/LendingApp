using LendingApp.Models.CashierModels;
using System.ComponentModel;

namespace LendingApp.LogicClass.Cashier
{
    public static class DataSample
    {
        public static void SeedInitialTransactions(BindingList<TransactionModels> transactions)
        {
            if (transactions.Count != 0) return;

            transactions.Add(new TransactionModels { Time = "9:30 AM", Customer = "Maria Santos", Amount = 2150m, ReceiptNo = "OR-001", LoanRef = "LN-2024-001" });
            transactions.Add(new TransactionModels { Time = "9:30 AM", Customer = "Maria Santos", Amount = 2150m, ReceiptNo = "OR-001", LoanRef = "LN-2024-001" });
            transactions.Add(new TransactionModels { Time = "10:15 AM", Customer = "Juan Dela Cruz", Amount = 4442m, ReceiptNo = "OR-002", LoanRef = "LN-2024-002" });
        }

    }
}
