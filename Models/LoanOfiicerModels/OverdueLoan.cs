using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.LoanOfiicerModels
{
    public class OverdueLoan
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string AmountDue { get; set; }
        public int DaysOverdue { get; set; }
        public string Contact { get; set; }
        public string Priority { get; set; } // Critical | High | Medium
    }
}
