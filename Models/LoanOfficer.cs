using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models
{
    public class LoanOfficer : User
    {
        private decimal approvalLimit {  get; set; }


        public bool ProcessApplication()
        {
            // Implementation for processing a loan application
            return true;
        }

        public void EvaluateCredit()
        {
            // Implementation for evaluating creditworthiness
        }

        public bool ProcessLaonApproval()
        {
            // Implementation for approving or rejecting a loan based on approval limit
            return true;
        }

        public void MonitorPortfolio()
        {
            // Implementation for monitoring loan portfolio
        }
    }
}
