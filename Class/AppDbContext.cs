using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Customer = LendingApp.Class.Models.LoanOfiicerModels.CustomerRegistrationData;
using LoanApplicationEntity = LendingApp.Class.Models.Loans.LoanApplicationEntity;

namespace LendingApp.Class
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=LendingAppDb")
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<LoanApplicationEntity> LoanApplications { get; set; } // ADDED

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // ===== loan_applications mapping (ADDED) =====
            modelBuilder.Entity<LoanApplicationEntity>()
                .ToTable("loan_applications")
                .HasKey(x => x.ApplicationId);

            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ApplicationId).HasColumnName("application_id");
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ApplicationNumber).HasColumnName("application_number").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.RequestedAmount).HasColumnName("requested_amount").IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.PreferredTerm).HasColumnName("preferred_term").IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(500);
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.DesiredReleaseDate).HasColumnName("desired_release_date");

            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20).IsRequired();

            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.RejectionReason).HasColumnName("rejection_reason");
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ApplicationDate).HasColumnName("application_date").IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.StatusDate).HasColumnName("status_date").IsRequired();
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ApprovedDate).HasColumnName("approved_date");
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.AssignedOfficerId).HasColumnName("assigned_officer_id");
            modelBuilder.Entity<LoanApplicationEntity>().Property(x => x.ApprovedBy).HasColumnName("approved_by");
        }
    }
}