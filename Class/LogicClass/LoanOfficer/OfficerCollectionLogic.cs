using LendingApp.Class.Models.LoanOfiicerModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingApp.LogicClass.LoanOfficer
{
    public class OfficerCollectionLogic

    {

    public string CollectionRate { get; set; }
    public string AverageCollection { get; set; }
    public string FollowUp { get; set; }


    private readonly List<CollectionItem> collections = new List<CollectionItem>
        {
            new CollectionItem { Id="1", LoanId="LN-001", Customer="Juan Cruz",  DueDate="Dec 15", Amount="₱4,442", DaysOverdue=0, Contact="+639123456789", Priority="High",     Status="Due Today" },
            new CollectionItem { Id="2", LoanId="LN-002", Customer="Pedro Reyes",DueDate="Dec 10", Amount="₱3,850", DaysOverdue=5, Contact="+639456789012", Priority="Critical", Status="Overdue" },
            new CollectionItem { Id="3", LoanId="LN-003", Customer="Maria Santos",DueDate="Dec 20", Amount="₱3,850", DaysOverdue=0, Contact="+639987654321", Priority="Medium",  Status="Upcoming" },
            new CollectionItem { Id="4", LoanId="LN-004", Customer="Ana Lopez",   DueDate="Dec 15", Amount="₱2,500", DaysOverdue=0, Contact="+639111222333", Priority="Medium",  Status="Due Today" },
            new CollectionItem { Id="5", LoanId="LN-005", Customer="Carlos Tan",  DueDate="Dec 12", Amount="₱4,150", DaysOverdue=3, Contact="+639444555666", Priority="High",    Status="Overdue" },
            new CollectionItem { Id="6", LoanId="LN-006", Customer="Rosa Garcia", DueDate="Dec 18", Amount="₱3,200", DaysOverdue=0, Contact="+639777888999", Priority="Low",     Status="Upcoming" },
        };

       public IReadOnlyList<CollectionItem> Allcollections => collections;

       public readonly Dictionary<string, string> summary = new Dictionary<string, string>

        {
            { "dueToday", "₱8,292" },
            { "overdue", "₱12,442" },
            { "thisWeek", "₱25,000" },
            { "collectedToday", "₱5,250" }
       };


    }
}
