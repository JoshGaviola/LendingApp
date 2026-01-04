using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Services;
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

            // ===== CREDIT SCORING (NEW) =====
            // For brand-new customers, compute an initial score from available data.
            // Only compute if caller didn't explicitly set a score.
            if (customer.InitialCreditScore <= 0)
            {
                var breakdown = CreditScoringService.CalculateInitial(customer);
                customer.InitialCreditScore = breakdown.TotalScore1000;
            }

            // Optional: if credit limit isn't set, derive a conservative default from score.
            if (customer.CreditLimit <= 0)
            {
                customer.CreditLimit = CreditScoringService.SuggestCreditLimit(customer.InitialCreditScore);
            }

            // Persist via repository
            _repository.Add(customer);

            return RegistrationResult.Ok();
        }
    }
}