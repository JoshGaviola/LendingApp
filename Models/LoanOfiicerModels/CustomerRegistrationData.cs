using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.LoanOfiicerModels
{
    public class CustomerRegistrationData
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CivilStatus { get; set; }
        public string Nationality { get; set; } = "Filipino";
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string SSSNumber { get; set; }
        public string TINNumber { get; set; }
        public string PassportNumber { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string UMIDNumber { get; set; }
        public string PhilhealthNumber { get; set; }
        public string PagibigNumber { get; set; }
        public string EmploymentStatus { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string EmergencyContactAddress { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now; public string CustomerType { get; set; } = "New"; public string Status { get; set; } = "Active";
        public int InitialCreditScore { get; set; }
        public decimal CreditLimit { get; set; }
        public string CreatedBy { get; set; } = "Admin User"; public DateTime LastModifiedDate { get; set; } = DateTime.Now; public string LastModifiedBy { get; set; } = "Admin User"; public string Remarks { get; set; }
        public string ValidId1Path { get; set; }
        public string ValidId2Path { get; set; }
        public string ProofOfIncomePath { get; set; }
        public string ProofOfAddressPath { get; set; }
        public string SignatureImagePath { get; set; }
    }
}
