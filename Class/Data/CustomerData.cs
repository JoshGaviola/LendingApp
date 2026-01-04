using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Repo;

namespace LendingApp.Class.Data
{
    public class CustomerData
    {
        private readonly ICustomerRepository _repo;

        public BindingList<CustomerItem> AllCustomers { get; }

        public CustomerData()
            : this(new CustomerRepository())
        {
        }

        public CustomerData(ICustomerRepository repo)
        {
            _repo = repo;
            AllCustomers = new BindingList<CustomerItem>();
            LoadCustomerFromDb();
        }

        public void LoadCustomerFromDb()
        {
            var customers = _repo.GetAll()
                .OrderByDescending(c => c.RegistrationDate)
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
                    RegisteredDate = c.RegistrationDate.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")),
                    LastActivity = ""
                })
                .ToList();

            AllCustomers.RaiseListChangedEvents = false;
            AllCustomers.Clear();
            foreach (var c in customers) AllCustomers.Add(c);
            AllCustomers.RaiseListChangedEvents = true;
            AllCustomers.ResetBindings();
        }

        public BindingList<CustomerItem> GetAllCustomers()
        {
            return AllCustomers;
        }
    }
}

