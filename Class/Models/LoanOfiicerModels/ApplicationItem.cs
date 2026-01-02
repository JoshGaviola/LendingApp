using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Class.Models.LoanOfiicerModels
{
    public class ApplicationItem
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string LoanType { get; set; }
        public string Amount { get; set; }
        public string AppliedDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
    }
}
