using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using System;

namespace LendingApp.Class.Service
{
    /// <summary>
    /// Handles validation and orchestrates customer registration.
    /// Depends on ICustomerRepository, not AppDbContext.
    /// </summary>
    public class CustomerRegistrationService : ICustomerRegistrationService
    {
        private readonly ICustomerRepository _repository;

        public CustomerRegistrationService(ICustomerRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public RegistrationResult Register(CustomerRegistrationData customer)
        {
            // Validation (SRP: validation logic is here, not in UI)
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                return RegistrationResult.Fail("First Name is required.");

            if (string.IsNullOrWhiteSpace(customer.LastName))
                return RegistrationResult.Fail("Last Name is required.");

            // Set audit fields
            customer.RegistrationDate = DateTime.Now;
            customer.LastModifiedDate = DateTime.Now;

            // Ensure defaults
            if (string.IsNullOrWhiteSpace(customer.CustomerType))
                customer.CustomerType = "New";

            if (string.IsNullOrWhiteSpace(customer.Status))
                customer.Status = "Active";

            // Persist via repository
            _repository.Add(customer);

            return RegistrationResult.Ok();
        }
    }
}