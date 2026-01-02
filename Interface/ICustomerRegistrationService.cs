using LendingApp.Models.LoanOfiicerModels;

namespace LendingApp.Class.Interface
{
    /// <summary>
    /// Abstraction for customer registration business logic.
    /// </summary>
    public interface ICustomerRegistrationService
    {
        /// <summary>
        /// Validates and registers a new customer.
        /// </summary>
        /// <param name="customer">Customer data to register</param>
        /// <returns>True if successful, false if validation failed</returns>
        RegistrationResult Register(CustomerRegistrationData customer);
    }

    /// <summary>
    /// Result of a registration attempt.
    /// </summary>
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static RegistrationResult Ok() => new RegistrationResult { Success = true };
        public static RegistrationResult Fail(string message) => new RegistrationResult { Success = false, ErrorMessage = message };
    }
}