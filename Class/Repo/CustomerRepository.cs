using LendingApp.Class.Interface;
using LendingApp.Models.LoanOfiicerModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LendingApp.Class.Repo
{
    /// <summary>
    /// Concrete implementation that uses EF + AppDbContext.
    /// Only this class knows about EF.
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        public void Add(CustomerRegistrationData customer)
        {
            using (var db = new AppDbContext())
            {
                db.Customers.Add(customer);
                db.SaveChanges();
            }
        }

        public CustomerRegistrationData GetById(string customerId)
        {
            using (var db = new AppDbContext())
            {
                return db.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == customerId);
            }
        }

        public IEnumerable<CustomerRegistrationData> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Customers.AsNoTracking().ToList();
            }
        }

        public void Update(CustomerRegistrationData customer)
        {
            using (var db = new AppDbContext())
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(string customerId)
        {
            using (var db = new AppDbContext())
            {
                var entity = db.Customers.Find(customerId);
                if (entity != null)
                {
                    db.Customers.Remove(entity);
                    db.SaveChanges();
                }
            }
        }
    }
}