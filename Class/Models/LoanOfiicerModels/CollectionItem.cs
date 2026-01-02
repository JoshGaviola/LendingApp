using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Class.Models.LoanOfiicerModels
{
    public class CollectionItem
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string Customer { get; set; }
        public string DueDate { get; set; }
        public string Amount { get; set; }
        public int DaysOverdue { get; set; }
        public string Contact { get; set; }
        public string Priority { get; set; } // Critical | High | Medium | Low
        public string Status { get; set; }   // Overdue | Due Today | Upcoming


    }
}
        
