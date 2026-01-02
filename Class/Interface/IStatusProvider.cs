using LendingApp.Class.Models.LoanOfiicerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.Interface
{
    public  interface IStatusProvider
    {
        List<StatusSummary> GetStatusSummary();

    }
}
