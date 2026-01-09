using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Windows.Forms;
using LendingApp.Class;

namespace LendingApp.Class.Services
{
    public class TodayTaskData
    {
        public DateTime ScheduledTime { get; set; }
        public string TimeDisplay { get; set; }
        public string CustomerName { get; set; }
        public string TaskType { get; set; }
        public string Status { get; set; }
        public string LoanNumber { get; set; }
    }

    public static partial class OfficerDashboardService
    {
        /// <summary>
        /// Load tasks scheduled for today (non-completed). Returns true when DB reachable even if zero rows.
        /// </summary>
        public static bool TryGetTodayTasks(List<TodayTaskData> target, int take = 100)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            try
            {
                using (var db = new AppDbContext())
                {
                    // Use an inclusive range [today, tomorrow) — reliably translatable to SQL by EF6.
                    var todayStart = DateTime.Today;
                    var tomorrowStart = todayStart.AddDays(1);

                    // Query tasks scheduled for today and not completed.
                    var q =
                        from t in db.Tasks.AsNoTracking()
                        join c in db.Customers.AsNoTracking() on t.CustomerId equals c.CustomerId into cj
                        from c in cj.DefaultIfEmpty()
                        join l in db.Loans.AsNoTracking() on t.LoanId equals l.LoanId into lj
                        from l in lj.DefaultIfEmpty()
                        where t.ScheduledTime >= todayStart
                              && t.ScheduledTime < tomorrowStart
                              && (t.Status == null || t.Status != "Completed")
                        orderby t.ScheduledTime
                        select new
                        {
                            t.ScheduledTime,
                            CustomerFirst = c != null ? c.FirstName : null,
                            CustomerLast = c != null ? c.LastName : null,
                            t.TaskType,
                            t.Status,
                            LoanNumber = l != null ? l.LoanNumber : null
                        };

                    var data = q.Take(take).ToList();

                    target.Clear();
                    foreach (var x in data)
                    {
                        // Ensure strings and build customer name safely.
                        var first = x.CustomerFirst ?? string.Empty;
                        var last = x.CustomerLast ?? string.Empty;
                        var customerName = (first + " " + last).Trim();
                        if (string.IsNullOrWhiteSpace(customerName))
                            customerName = string.Empty;

                        target.Add(new TodayTaskData
                        {
                            ScheduledTime = x.ScheduledTime,
                            TimeDisplay = x.ScheduledTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                            CustomerName = customerName,
                            TaskType = x.TaskType ?? string.Empty,
                            Status = x.Status ?? string.Empty,
                            LoanNumber = x.LoanNumber ?? string.Empty
                        });
                    }

                    // DB reachable even if zero rows returned
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Show a concise diagnostic (inner exception if available).
                var msg = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Failed to load today's tasks from DB:\n" + msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}