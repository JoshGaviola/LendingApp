using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.Loans
{
    public class LoanApplication
    {
     public int ApplicationId { get; set; }
     public DateTime ApplicationDate { get; set; }
     public decimal RequestedAmount { get; set; }
     public int PreferredTerm { get; set; } 
     public string Purpose { get; set; }
     public DateTime DesireReleaseDate { get; set; }
     public string ApplicationStatus { get; set; }
     public string RejectionReason { get; set; }
     public DateTime StatusDate { get; set; }


     public void SubmitApplication()
     {
      // Implementation for submitting a loan application
     }

     public void UpdateStatus()
     {

     }

     public void ValidateDocuments()
     {
       // Implementation for validating documents
     }


    }
}
