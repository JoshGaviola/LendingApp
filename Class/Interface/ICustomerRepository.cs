using LendingApp.Class.Models.LoanOfiicerModels;
using System.Collections.Generic;

namespace LendingApp.Class.Interface
{
    /// <summary>
    /// Abstraction for customer data access.
    /// The UI and services depend on this interface, not on EF directly.
    /// </summary>
    public interface ICustomerRepository
    {
        void Add(CustomerRegistrationData customer);
        CustomerRegistrationData GetById(string customerId);
        IEnumerable<CustomerRegistrationData> GetAll();
        void Update(CustomerRegistrationData customer);
        void Delete(string customerId);
    }
}