using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Models.LoanOfiicerModels
{
    public class TaskItem
    {
        public string Id { get; set; }
        public string Time { get; set; }
        public string Customer { get; set; }
        public string TaskType { get; set; }
        public string LoanId { get; set; }
        public string Status { get; set; } // Due | Pending | Completed
    }
}
