using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LendingApp.Class.Models.LoanOfiicerModels;

namespace LendingApp.Class.Data
{
    public class CustomerData
    {
        public  BindingList<CustomerItem> AllCustomers { get; private set; }

        public CustomerData()
        {
            AllCustomers = new BindingList<CustomerItem>();
            LoadCustomerFromDb();
        }
        public void LoadCustomerFromDb()
        {
            using (var db = new AppDbContext())
            {
                var customers = db.Customers
                    .AsNoTracking()
                    .OrderByDescending(c => c.RegistrationDate)
                    .ToList()
                    .Select(c => new CustomerItem
                    {
                        Id = c.CustomerId,
                        Name = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                        Contact = c.MobileNumber,
                        Email = c.EmailAddress,
                        Type = c.CustomerType,
                        CreditScore = c.InitialCreditScore,
                        TotalLoans = 0,
                        BalanceAmount = 0,
                        Balance = "₱0.00",
                        RegisteredDate = c.RegistrationDate.ToString("MMM dd, yyyy"),
                        LastActivity = ""
                    })
                    .ToList();

                AllCustomers.Clear();        
                foreach (var c in customers)
                {
                    AllCustomers.Add(c);      
                }

            }
        }
        public BindingList<CustomerItem> GetAllCustomers()
        {
            return AllCustomers;
        }

    }
}

