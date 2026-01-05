using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Customer = LendingApp.Class.Models.LoanOfiicerModels.CustomerRegistrationData;
using LoanApplicationEntity = LendingApp.Class.Models.Loans.LoanApplicationEntity;
using LoanProductEntity = LendingApp.Class.Models.Loans.LoanProductEntity;
using LoanApplicationEvaluationEntity = LendingApp.Class.Models.Loans.LoanApplicationEvaluationEntity;
using LoanEntity = LendingApp.Class.Models.Loans.LoanEntity;

namespace LendingApp.Class
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=LendingAppDb") { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<LoanApplicationEntity> LoanApplications { get; set; }
        public DbSet<LoanProductEntity> LoanProducts { get; set; }
        public DbSet<LoanApplicationEvaluationEntity> LoanApplicationEvaluations { get; set; }
        public DbSet<LoanEntity> Loans { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer<AppDbContext>(null);  // Disable migrations

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

            // ===== loan_products mapping (ADDED) =====
            modelBuilder.Entity<LoanProductEntity>()
                .ToTable("loan_products")
                .HasKey(x => x.ProductId);

            modelBuilder.Entity<LoanProductEntity>().Property(x => x.ProductId).HasColumnName("product_id");
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100).IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.Description).HasColumnName("description");

            modelBuilder.Entity<LoanProductEntity>().Property(x => x.MinAmount).HasColumnName("min_amount").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.MaxAmount).HasColumnName("max_amount").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.MinTermMonths).HasColumnName("min_term_months").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.MaxTermMonths).HasColumnName("max_term_months").IsRequired();

            modelBuilder.Entity<LoanProductEntity>().Property(x => x.InterestRate).HasColumnName("interest_rate").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.ProcessingFeePct).HasColumnName("processing_fee_pct").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.PenaltyRate).HasColumnName("penalty_rate").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.GracePeriodDays).HasColumnName("grace_period_days").IsRequired();

            modelBuilder.Entity<LoanProductEntity>().Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
            modelBuilder.Entity<LoanProductEntity>().Property(x => x.CreatedDate).HasColumnName("created_date").IsRequired();

            // NEW: loan_application_evaluations
            modelBuilder.Entity<LoanApplicationEvaluationEntity>()
                .ToTable("loan_application_evaluations")
                .HasKey(x => x.EvaluationId);

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.EvaluationId).HasColumnName("evaluation_id");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.ApplicationId).HasColumnName("application_id");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C1Input).HasColumnName("c1_input");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C2Input).HasColumnName("c2_input");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C3Input).HasColumnName("c3_input");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C4Input).HasColumnName("c4_input");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.W1Pct).HasColumnName("w1_pct");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.W2Pct).HasColumnName("w2_pct");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.W3Pct).HasColumnName("w3_pct");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.W4Pct).HasColumnName("w4_pct");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C1Weighted).HasColumnName("c1_weighted");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C2Weighted).HasColumnName("c2_weighted");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C3Weighted).HasColumnName("c3_weighted");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.C4Weighted).HasColumnName("c4_weighted");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.TotalScore).HasColumnName("total_score");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.Decision).HasColumnName("decision");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.InterestMethod).HasColumnName("interest_method");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.InterestRatePct).HasColumnName("interest_rate_pct");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.ServiceFeePct).HasColumnName("service_fee_pct");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.TermMonths).HasColumnName("term_months");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.ApprovalLevel).HasColumnName("approval_level");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.RequireComaker).HasColumnName("require_comaker");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.ReduceAmount).HasColumnName("reduce_amount");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.ShortenTerm).HasColumnName("shorten_term");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.AdditionalCollateral).HasColumnName("additional_collateral");

            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.RejectionReason).HasColumnName("rejection_reason");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.Remarks).HasColumnName("remarks");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.EvaluatedBy).HasColumnName("evaluated_by");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.StatusAfter).HasColumnName("status_after");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<LoanApplicationEvaluationEntity>().Property(x => x.UpdatedAt).HasColumnName("updated_at");

            // ===== loans mapping (NEW) =====
            modelBuilder.Entity<LoanEntity>()
                .ToTable("loans")
                .HasKey(x => x.LoanId);

            modelBuilder.Entity<LoanEntity>().Property(x => x.LoanId).HasColumnName("loan_id");
            modelBuilder.Entity<LoanEntity>().Property(x => x.LoanNumber).HasColumnName("loan_number").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.ApplicationId).HasColumnName("application_id").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.ProductId).HasColumnName("product_id").IsRequired();

            modelBuilder.Entity<LoanEntity>().Property(x => x.PrincipalAmount).HasColumnName("principal_amount").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.InterestRate).HasColumnName("interest_rate").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.TermMonths).HasColumnName("term_months").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.MonthlyPayment).HasColumnName("monthly_payment").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.ProcessingFee).HasColumnName("processing_fee").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.TotalPayable).HasColumnName("total_payable").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.OutstandingBalance).HasColumnName("outstanding_balance").IsRequired();

            modelBuilder.Entity<LoanEntity>().Property(x => x.TotalPaid).HasColumnName("total_paid").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.TotalInterestPaid).HasColumnName("total_interest_paid").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.TotalPenaltyPaid).HasColumnName("total_penalty_paid").IsRequired();

            modelBuilder.Entity<LoanEntity>().Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.DaysOverdue).HasColumnName("days_overdue").IsRequired();

            modelBuilder.Entity<LoanEntity>().Property(x => x.ReleaseDate).HasColumnName("release_date").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.FirstDueDate).HasColumnName("first_due_date").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.NextDueDate).HasColumnName("next_due_date");
            modelBuilder.Entity<LoanEntity>().Property(x => x.MaturityDate).HasColumnName("maturity_date").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.LastPaymentDate).HasColumnName("last_payment_date");

            modelBuilder.Entity<LoanEntity>().Property(x => x.ReleaseMode).HasColumnName("release_mode").HasMaxLength(50);
            modelBuilder.Entity<LoanEntity>().Property(x => x.ReleasedBy).HasColumnName("released_by");

            modelBuilder.Entity<LoanEntity>().Property(x => x.CreatedDate).HasColumnName("created_date").IsRequired();
            modelBuilder.Entity<LoanEntity>().Property(x => x.LastUpdated).HasColumnName("last_updated").IsRequired();

        }
    }
}