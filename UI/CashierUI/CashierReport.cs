using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.UI.CashierUI.Controls;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierReport : Form
    {
        private string selectedReport = "Daily Collection Report";
        private DateTime dateFrom = DateTime.Today.AddDays(-7);
        private DateTime dateTo = DateTime.Today;

        private Panel root;
        private ReportSelectionControl selectionControl;
        private ReportParametersControl parametersControl;
        private ReportPreviewControl previewControl;
        private QuickStatsControl quickStatsControl;

        public CashierReport()
        {
            InitializeComponent();
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            BuildUI();
            RefreshPreview();
        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(16) };
            Controls.Add(root);

            var content = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            root.Controls.Add(content);

            // Header card (simple)
            var header = new Panel
            {
                Width = 850,
                Height = 50,
                BackColor = ColorTranslator.FromHtml("#EFF6FF"),
                BorderStyle = BorderStyle.FixedSingle
            };
            header.Controls.Add(new Label
            {
                Text = "📊 CASHIER REPORTS CENTER",
                Location = new Point(16, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            });
            content.Controls.Add(header);

            selectionControl = new ReportSelectionControl();
            selectionControl.SelectedReportChanged += (s, e) =>
            {
                selectedReport = selectionControl.SelectedReport;
                RefreshPreview();
            };
            content.Controls.Add(selectionControl);

            parametersControl = new ReportParametersControl();
            parametersControl.DateFrom = dateFrom;
            parametersControl.DateTo = dateTo;

            parametersControl.ApplyClicked += (s, e) =>
            {
                dateFrom = parametersControl.DateFrom;
                dateTo = parametersControl.DateTo;
                RefreshPreview();
                ShowToast("Filters applied. Report updated.");
            };

            parametersControl.ResetClicked += (s, e) =>
            {
                dateFrom = new DateTime(2024, 6, 1);
                dateTo = new DateTime(2024, 6, 10);
                parametersControl.DateFrom = dateFrom;
                parametersControl.DateTo = dateTo;
                RefreshPreview();
                ShowToast("Filters reset to default");
            };

            content.Controls.Add(parametersControl);

            previewControl = new ReportPreviewControl();
            previewControl.ViewFullReportClicked += (s, e) => ShowFullReport();

            previewControl.GenerateFullReportClicked += (s, e) => ShowToast("Generating full report...");
            previewControl.ExportPdfClicked += (s, e) => ShowToast("Exporting report to PDF...");
            previewControl.ExportExcelClicked += (s, e) => ShowToast("Exporting report to Excel...");
            previewControl.ExportCsvClicked += (s, e) => ShowToast("Exporting report to CSV...");
            previewControl.PrintClicked += (s, e) => ShowToast("Sending report to printer...");

            content.Controls.Add(previewControl);

            quickStatsControl = new QuickStatsControl();
            quickStatsControl.Margin = new Padding(0, 16, 0, 0);
            content.Controls.Add(quickStatsControl);
        }

        private void RefreshPreview()
        {
            // Guard (controls might not be created yet during constructor sequencing)
            if (previewControl == null || quickStatsControl == null || parametersControl == null)
                return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var start = dateFrom.Date;
                    var endExclusive = dateTo.Date.AddDays(1);

                    // Base query (Payments in date range)
                    var q = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= start && p.PaymentDate < endExclusive);

                    // Optional filter: Payment Mode (if your parameters control provides it)
                    var payMode = (parametersControl.SelectedPaymentMode ?? "").Trim();

                    // Treat empty / "All" as no filter
                    if (!string.IsNullOrWhiteSpace(payMode) && !string.Equals(payMode, "All", StringComparison.OrdinalIgnoreCase))
                    {
                        q = q.Where(p => (p.PaymentMethod ?? "").Trim().Equals(payMode, StringComparison.OrdinalIgnoreCase));
                    }

                    // NOTE:
                    // - SelectedCashier requires join to users table (not implemented in DB model here)
                    // - SelectedLoanType requires join to Loans -> Products (not implemented here)

                    var payments = q.ToList();

                    // Totals
                    decimal totalCollections = payments.Sum(p => (decimal?)p.AmountPaid) ?? 0m;
                    int totalTransactions = payments.Count;
                    int days = Math.Max(1, (dateTo.Date - dateFrom.Date).Days + 1);
                    decimal avgDaily = days > 0 ? Math.Round(totalCollections / days, 2, MidpointRounding.AwayFromZero) : 0m;

                    // Breakdown by day
                    var breakdownRows = payments
                        .GroupBy(p => p.PaymentDate.Date)
                        .OrderBy(g => g.Key)
                        .Select(g => (DateLabel: g.Key.ToString("MMM dd", CultureInfo.InvariantCulture),
                                      Amount: g.Sum(x => x.AmountPaid),
                                      Transactions: g.Count()))
                        .ToArray();

                    previewControl.SetPreview(
                        selectedReport,
                        dateFrom,
                        dateTo,
                        totalCollections,
                        totalTransactions,
                        avgDaily,
                        breakdownRows);

                    // Quick stats: month totals
                    var today = DateTime.Today;
                    var firstOfThisMonth = new DateTime(today.Year, today.Month, 1);
                    var firstOfNextMonth = firstOfThisMonth.AddMonths(1);
                    var firstOfLastMonth = firstOfThisMonth.AddMonths(-1);

                    var thisMonthQ = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= firstOfThisMonth && p.PaymentDate < firstOfNextMonth);

                    var lastMonthQ = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= firstOfLastMonth && p.PaymentDate < firstOfThisMonth);

                    if (!string.IsNullOrWhiteSpace(payMode) && !string.Equals(payMode, "All", StringComparison.OrdinalIgnoreCase))
                    {
                        thisMonthQ = thisMonthQ.Where(p => (p.PaymentMethod ?? "") == payMode);
                        lastMonthQ = lastMonthQ.Where(p => (p.PaymentMethod ?? "") == payMode);
                    }

                    decimal thisMonthTotal = thisMonthQ.Sum(p => (decimal?)p.AmountPaid) ?? 0m;
                    decimal lastMonthTotal = lastMonthQ.Sum(p => (decimal?)p.AmountPaid) ?? 0m;

                    var chartData = breakdownRows.Select(b => (b.DateLabel, b.Amount)).ToArray();

                    quickStatsControl.SetStats(
                        thisMonth: thisMonthTotal,
                        lastMonth: lastMonthTotal,
                        topLoanType: "N/A",
                        topLoanTypePercent: "—",
                        topPaymentMode: string.IsNullOrWhiteSpace(payMode) || payMode == "All" ? "All" : payMode,
                        topPaymentModePercent: "—",
                        collectionRate: totalCollections > 0m ? "100%" : "0%",
                        dailyBreakdown: chartData);
                }
            }
            catch (Exception ex)
            {
                // Keep UI working even if DB is temporarily unavailable
                ShowToast("Failed to load report from database: " + ex.Message);

                previewControl.SetPreview(
                    selectedReport,
                    dateFrom,
                    dateTo,
                    0m,
                    0,
                    0m,
                    new (string DateLabel, decimal Amount, int Transactions)[0]);

                quickStatsControl.SetStats(
                    thisMonth: 0m,
                    lastMonth: 0m,
                    topLoanType: "N/A",
                    topLoanTypePercent: "—",
                    topPaymentMode: "N/A",
                    topPaymentModePercent: "—",
                    collectionRate: "0%",
                    dailyBreakdown: new (string DateLabel, decimal Amount)[0]);
            }
        }

        private void ShowFullReport()
        {
            // Leave as-is for now (you can wire this to a DB-backed FullReportDialog later)
            ShowToast("Full report view not yet wired to database.");
        }

        private void ShowToast(string msg)
        {
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}