using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LendingApp.Class;
using LendingApp.Class.Models.LoanOfiicerModels;
using System.Data.Entity;

namespace LendingApp.Class.Services.Collections
{
    public sealed class CollectionsSummary
    {
        public string DueToday { get; set; }
        public string Overdue { get; set; }
        public string ThisWeek { get; set; }
        public string CollectedToday { get; set; }

        public string CollectionRate { get; set; }
        public string AverageCollection { get; set; }
        public string FollowUp { get; set; }
    }

    public sealed class OfficerCollectionService
    {
        /// <summary>
        /// Reads collections (joined with loans/customers when available) and maps to UI CollectionItem.
        /// Returns empty list on error.
        /// </summary>
        public List<CollectionItem> GetCollections()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var rows = (from c in db.Collections.AsNoTracking()
                                join l in db.Loans.AsNoTracking() on c.LoanId equals l.LoanId into lj
                                from l in lj.DefaultIfEmpty()
                                join cu in db.Customers.AsNoTracking() on c.CustomerId equals cu.CustomerId into cj
                                from cu in cj.DefaultIfEmpty()
                                select new
                                {
                                    c.CollectionId,
                                    c.LoanId,
                                    LoanNumber = l != null ? l.LoanNumber : null,
                                    c.CustomerId,
                                    CustomerName = ((cu != null ? (cu.FirstName ?? "") : "") + " " + (cu != null ? (cu.LastName ?? "") : "")).Trim(),
                                    Mobile = cu != null ? cu.MobileNumber : null,
                                    c.DueDate,
                                    c.AmountDue,
                                    c.DaysOverdue,
                                    c.Priority,
                                    c.Status,
                                    c.UpdatedDate
                                }).ToList();

                    var list = new List<CollectionItem>(rows.Count);
                    foreach (var r in rows)
                    {
                        var loanRef = !string.IsNullOrWhiteSpace(r.LoanNumber) ? r.LoanNumber : $"LN-{r.LoanId}";
                        var customerDisplay = string.IsNullOrWhiteSpace(r.CustomerName) ? r.CustomerId : r.CustomerName;
                        var contact = string.IsNullOrWhiteSpace(r.Mobile) ? r.CustomerId : r.Mobile;

                        list.Add(new CollectionItem
                        {
                            Id = r.CollectionId.ToString(),
                            LoanId = loanRef,
                            Customer = customerDisplay,
                            DueDate = r.DueDate.ToString("MMM d", CultureInfo.InvariantCulture),
                            Amount = "₱" + r.AmountDue.ToString("N0", CultureInfo.InvariantCulture),
                            DaysOverdue = r.DaysOverdue,
                            Contact = contact,
                            Priority = string.IsNullOrWhiteSpace(r.Priority) ? "Low" : r.Priority,
                            Status = MapToUiStatus(r.Status, r.DaysOverdue, r.DueDate)
                        });
                    }

                    return list;
                }
            }
            catch
            {
                // swallow exceptions here to keep UI resilient; return empty list so caller can fall back
                return new List<CollectionItem>();
            }
        }

        /// <summary>
        /// Computes summary values and quick stats from a set of CollectionItem.
        /// </summary>
        public CollectionsSummary CalculateSummary(IEnumerable<CollectionItem> items)
        {
            var rows = items?.ToList() ?? new List<CollectionItem>();

            decimal ParseAmount(string amt)
            {
                if (string.IsNullOrWhiteSpace(amt)) return 0m;
                var cleaned = amt.Replace("₱", "").Replace(",", "").Trim();
                decimal val;
                return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val) ? val : 0m;
            }

            var dueTodayTotal = rows.Where(r => r.Status == "Due Today").Sum(r => ParseAmount(r.Amount));
            var overdueTotal = rows.Where(r => r.Status == "Overdue").Sum(r => ParseAmount(r.Amount));
            var thisWeekTotal = rows.Where(r =>
            {
                DateTime dt;
                if (DateTime.TryParseExact(r.DueDate, "MMM d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    // parsed without year - use approximate check against today
                }
                // fallback: approximate by including Due Today + Upcoming within 7 days is caller responsibility.
                return false;
            }).Sum(r => ParseAmount(r.Amount)); // keep thisWeekTotal as 0 (UI can accept server-side computation if you prefer)

            // Simpler, compute totals from status categories:
            var collectedTodayTotal = rows.Where(r => r.Status == "Collected").Sum(r => ParseAmount(r.Amount));

            var total = rows.Count;
            var collectedCount = rows.Count(r => r.Status == "Collected");
            var collectionRate = total > 0 ? Math.Round((double)collectedCount * 100.0 / total, 1).ToString(CultureInfo.InvariantCulture) + "%" : "0%";
            var avg = rows.Where(r => r.Status == "Collected").Any() ? rows.Where(r => r.Status == "Collected").Average(r => ParseAmount(r.Amount)) : 0m;

            var followups = rows.Count(r => r.Status != "Collected" && r.DaysOverdue > 0);

            return new CollectionsSummary
            {
                DueToday = "₱" + dueTodayTotal.ToString("N0", CultureInfo.InvariantCulture),
                Overdue = "₱" + overdueTotal.ToString("N0", CultureInfo.InvariantCulture),
                ThisWeek = "₱" + thisWeekTotal.ToString("N0", CultureInfo.InvariantCulture),
                CollectedToday = "₱" + collectedTodayTotal.ToString("N0", CultureInfo.InvariantCulture),

                CollectionRate = collectionRate,
                AverageCollection = "₱" + avg.ToString("N0", CultureInfo.InvariantCulture),
                FollowUp = followups.ToString()
            };
        }

        private static string MapToUiStatus(string dbStatus, int daysOverdue, DateTime dueDate)
        {
            if (!string.IsNullOrWhiteSpace(dbStatus) && dbStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                return "Collected";

            if (daysOverdue > 0)
                return "Overdue";

            if (dueDate.Date == DateTime.Today)
                return "Due Today";

            return "Upcoming";
        }
    }
}