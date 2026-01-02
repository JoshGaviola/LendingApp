using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Customer = LendingApp.Models.LoanOfiicerModels.CustomerRegistrationData;

namespace LendingApp.Class
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=LendingAppDb")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: prevent pluralization issues
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Customer>()
                .ToTable("customers")
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>().Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.MiddleName).HasColumnName("middle_name").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
            modelBuilder.Entity<Customer>().Property(x => x.Gender).HasColumnName("gender").HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(x => x.CivilStatus).HasColumnName("civil_status").HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(x => x.Nationality).HasColumnName("nationality").HasMaxLength(50);

            modelBuilder.Entity<Customer>().Property(x => x.EmailAddress).HasColumnName("email_address").HasMaxLength(200);
            modelBuilder.Entity<Customer>().Property(x => x.MobileNumber).HasColumnName("mobile_number").HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(x => x.TelephoneNumber).HasColumnName("telephone_number").HasMaxLength(30);

            modelBuilder.Entity<Customer>().Property(x => x.PresentAddress).HasColumnName("present_address").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.PermanentAddress).HasColumnName("permanent_address").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.City).HasColumnName("city").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.Province).HasColumnName("province").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.ZipCode).HasColumnName("zip_code").HasMaxLength(20);

            modelBuilder.Entity<Customer>().Property(x => x.SSSNumber).HasColumnName("sss_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.TINNumber).HasColumnName("tin_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.PassportNumber).HasColumnName("passport_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.DriversLicenseNumber).HasColumnName("drivers_license_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.UMIDNumber).HasColumnName("umid_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.PhilhealthNumber).HasColumnName("philhealth_number").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.PagibigNumber).HasColumnName("pagibig_number").HasMaxLength(50);

            modelBuilder.Entity<Customer>().Property(x => x.EmploymentStatus).HasColumnName("employment_status").HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.CompanyName).HasColumnName("company_name").HasMaxLength(200);
            modelBuilder.Entity<Customer>().Property(x => x.Position).HasColumnName("position").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.Department).HasColumnName("department").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.CompanyAddress).HasColumnName("company_address").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.CompanyPhone).HasColumnName("company_phone").HasMaxLength(30);

            modelBuilder.Entity<Customer>().Property(x => x.BankName).HasColumnName("bank_name").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.BankAccountNumber).HasColumnName("bank_account_number").HasMaxLength(100);

            modelBuilder.Entity<Customer>().Property(x => x.InitialCreditScore).HasColumnName("initial_credit_score").IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.CreditLimit).HasColumnName("credit_limit").IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(200);
            modelBuilder.Entity<Customer>().Property(x => x.EmergencyContactRelationship).HasColumnName("emergency_contact_relationship").HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.EmergencyContactNumber).HasColumnName("emergency_contact_number").HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(x => x.EmergencyContactAddress).HasColumnName("emergency_contact_address").HasMaxLength(500);

            modelBuilder.Entity<Customer>().Property(x => x.CustomerType).HasColumnName("customer_type").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.RegistrationDate).HasColumnName("registration_date").IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.CreatedBy).HasColumnName("created_by");
            modelBuilder.Entity<Customer>().Property(x => x.LastModifiedDate).HasColumnName("last_modified_date").IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.LastModifiedBy).HasColumnName("last_modified_by");
            modelBuilder.Entity<Customer>().Property(x => x.Remarks).HasColumnName("remarks");

            modelBuilder.Entity<Customer>().Property(x => x.ValidId1Path).HasColumnName("valid_id1_path").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.ValidId2Path).HasColumnName("valid_id2_path").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.ProofOfIncomePath).HasColumnName("proof_of_income_path").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.ProofOfAddressPath).HasColumnName("proof_of_address_path").HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(x => x.SignatureImagePath).HasColumnName("signature_image_path").HasMaxLength(500);
        }
    }
}