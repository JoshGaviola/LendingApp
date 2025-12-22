using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class ReportPreviewControl : UserControl
    {
        public event EventHandler ViewFullReportClicked;

        private Label lblReportTitle;
        private Label lblPeriod;
        private Label lblGenerated;

        private Label lblTotalCollectionsValue;
        private Label lblTotalTransactionsValue;
        private Label lblAvgDailyValue;

        private Panel pnlDailyBreakdown;
        private LinkLabel lnkViewFull;

        public ReportPreviewControl()
        {
            BackColor = ColorTranslator.FromHtml("#F9FAFB");
            BorderStyle = BorderStyle.FixedSingle;
            Width = 850;
            Height = 420;

            var title = new Label
            {
                Text = "📄 REPORT PREVIEW",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            Controls.Add(title);

            // Outer white preview box
            var outer = new Panel
            {
                Location = new Point(16, 40),
                Width = 818,
                Height = 360,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(outer);

            // Inner "double border" look
            var inner = new Panel
            {
                Location = new Point(10, 10),
                Width = 796,
                Height = 338,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            outer.Controls.Add(inner);

            // Header block
            lblReportTitle = new Label
            {
                Text = "DAILY COLLECTION REPORT",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 14)
            };
            inner.Controls.Add(lblReportTitle);

            lblPeriod = new Label
            {
                Text = "Period: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 40)
            };
            inner.Controls.Add(lblPeriod);

            lblGenerated = new Label
            {
                Text = "Generated: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(16, 60)
            };
            inner.Controls.Add(lblGenerated);

            var headerSep = new Panel
            {
                Location = new Point(16, 82),
                Width = 764,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            inner.Controls.Add(headerSep);

            // SUMMARY section
            var lblSummary = new Label
            {
                Text = "SUMMARY:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 92)
            };
            inner.Controls.Add(lblSummary);

            int y = 114;
            AddSummaryRow(inner, "Total Collections:", out lblTotalCollectionsValue, ref y);
            AddSummaryRow(inner, "Total Transactions:", out lblTotalTransactionsValue, ref y);
            AddSummaryRow(inner, "Avg. Daily:", out lblAvgDailyValue, ref y);

            var summarySep = new Panel
            {
                Location = new Point(16, y + 6),
                Width = 764,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            inner.Controls.Add(summarySep);

            // DAILY BREAKDOWN section
            var lblBreakdown = new Label
            {
                Text = "DAILY BREAKDOWN:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, y + 16)
            };
            inner.Controls.Add(lblBreakdown);

            pnlDailyBreakdown = new Panel
            {
                Location = new Point(32, y + 38),
                Width = 748,
                Height = 110,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            inner.Controls.Add(pnlDailyBreakdown);

            // View full report link (bottom)
            lnkViewFull = new LinkLabel
            {
                Text = "[View Full Report >>>]",
                AutoSize = true,
                LinkColor = ColorTranslator.FromHtml("#2563EB"),
                Location = new Point(320, inner.Height - 28)
            };
            lnkViewFull.Click += (s, e) => ViewFullReportClicked?.Invoke(this, EventArgs.Empty);
            inner.Controls.Add(lnkViewFull);
        }

        private void AddSummaryRow(Control parent, string label, out Label valueLabel, ref int y)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(32, y)
            };
            parent.Controls.Add(lbl);

            valueLabel = new Label
            {
                Text = "-",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(620, y)
            };
            parent.Controls.Add(valueLabel);

            y += 22;
        }

        public void SetPreview(
            string reportTitle,
            DateTime dateFrom,
            DateTime dateTo,
            decimal totalCollections,
            int totalTransactions,
            decimal avgDaily,
            IEnumerable<(string DateLabel, decimal Amount, int Transactions)> dailyBreakdown)
        {
            lblReportTitle.Text = (reportTitle ?? "").ToUpperInvariant();
            lblPeriod.Text = $"Period: {dateFrom:yyyy-MM-dd} to {dateTo:yyyy-MM-dd}";
            lblGenerated.Text = $"Generated: {DateTime.Now:MMM d, yyyy h:mm tt}";

            lblTotalCollectionsValue.Text = $"₱{totalCollections:N2}";
            lblTotalTransactionsValue.Text = totalTransactions.ToString();
            lblAvgDailyValue.Text = $"₱{avgDaily:N2}";

            pnlDailyBreakdown.Controls.Clear();

            int y = 0;
            if (dailyBreakdown != null)
            {
                foreach (var day in dailyBreakdown)
                {
                    var row = new Label
                    {
                        AutoSize = true,
                        ForeColor = ColorTranslator.FromHtml("#374151"),
                        Location = new Point(0, y),
                        Text = $"{day.DateLabel}: ₱{day.Amount:N2} ({day.Transactions} trans)"
                    };
                    pnlDailyBreakdown.Controls.Add(row);
                    y += 18;
                }
            }
        }
    }
}