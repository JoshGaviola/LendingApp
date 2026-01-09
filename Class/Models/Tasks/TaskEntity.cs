using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Class.Models.Tasks
{
    public class TaskEntity
    {
        public int TaskId { get; set; }
        public int AssignedTo { get; set; }
        public int? LoanId { get; set; }
        public string CustomerId { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
