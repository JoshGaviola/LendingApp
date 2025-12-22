using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LendingApp.UI.CashierUI.Controls;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierReport : Form
    {
        private string selectedReport = "Daily Collection Report";
        private DateTime dateFrom = new DateTime(2024, 6, 1);
        private DateTime dateTo = new DateTime(2024, 6, 10);

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
            // Mock values (same as your React example)
            decimal totalCollections = 128592.44m;
            int totalTransactions = 156;
            decimal avgDaily = 12859.24m;

            var breakdown = new (string DateLabel, decimal Amount, int Transactions)[]
            {
                ("Jun 01", 11292.44m, 15),
                ("Jun 02", 14650.00m, 18),
                ("Jun 03", 13300.00m, 16),
                ("Jun 04", 11700.00m, 14),
                ("Jun 05", 12450.00m, 15),
                ("Jun 06", 11000.00m, 13),
                ("Jun 07", 15500.00m, 19),
                ("Jun 08", 13300.00m, 17),
                ("Jun 09", 10900.00m, 12),
                ("Jun 10", 14400.00m, 17)
            };

            previewControl.SetPreview(
                selectedReport,
                dateFrom,
                dateTo,
                totalCollections,
                totalTransactions,
                avgDaily,
                breakdown);

            // Update Quick Stats
            decimal thisMonth = 128592.44m;
            decimal lastMonth = 115420.00m;

            var chartData = breakdown.Select(b => (b.DateLabel, b.Amount)).ToArray();

            quickStatsControl.SetStats(
                thisMonth: thisMonth,
                lastMonth: lastMonth,
                topLoanType: "Personal Loan",
                topLoanTypePercent: "65%",
                topPaymentMode: "Cash",
                topPaymentModePercent: "58%",
                collectionRate: "94.2%",
                dailyBreakdown: chartData);
        }

        private void ShowFullReport()
        {
            // Build mock transaction data (in real app, fetch from database)
            var transactions = new List<Controls.FullReportDialog.TransactionItem>
            {
                new Controls.FullReportDialog.TransactionItem { Id="1", Date="Jun 01", ReceiptNo="OR-001", Customer="Juan Dela Cruz", LoanNo="LN-001", Amount=4442.44m, PaymentMode="Cash", Principal=4000m, Interest=442.44m, Penalty=0m },
                new Controls.FullReportDialog.TransactionItem { Id="2", Date="Jun 01", ReceiptNo="OR-002", Customer="Maria Santos", LoanNo="LN-002", Amount=2150m, PaymentMode="GCash", Principal=2000m, Interest=150m, Penalty=0m },
                new Controls.FullReportDialog.TransactionItem { Id="3", Date="Jun 02", ReceiptNo="OR-003", Customer="Pedro Reyes", LoanNo="LN-003", Amount=3500m, PaymentMode="Cash", Principal=3200m, Interest=250m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="4", Date="Jun 02", ReceiptNo="OR-004", Customer="Ana Garcia", LoanNo="LN-004", Amount=5000m, PaymentMode="Bank", Principal=4500m, Interest=400m, Penalty=100m },
                new Controls.FullReportDialog.TransactionItem { Id="5", Date="Jun 03", ReceiptNo="OR-005", Customer="Carlos Mendoza", LoanNo="LN-005", Amount=2800m, PaymentMode="Cash", Principal=2500m, Interest=250m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="6", Date="Jun 03", ReceiptNo="OR-006", Customer="Rosa Cruz", LoanNo="LN-006", Amount=1800m, PaymentMode="GCash", Principal=1600m, Interest=150m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="7", Date="Jun 04", ReceiptNo="OR-007", Customer="David Santos", LoanNo="LN-007", Amount=3200m, PaymentMode="Cash", Principal=2900m, Interest=250m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="8", Date="Jun 04", ReceiptNo="OR-008", Customer="Elena Ramos", LoanNo="LN-008", Amount=1500m, PaymentMode="GCash", Principal=1350m, Interest=150m, Penalty=0m },
                new Controls.FullReportDialog.TransactionItem { Id="9", Date="Jun 05", ReceiptNo="OR-009", Customer="Fernando Lopez", LoanNo="LN-009", Amount=2000m, PaymentMode="Bank", Principal=1800m, Interest=150m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="10", Date="Jun 05", ReceiptNo="OR-010", Customer="Gloria Tan", LoanNo="LN-010", Amount=4200m, PaymentMode="Cash", Principal=3800m, Interest=350m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="11", Date="Jun 06", ReceiptNo="OR-011", Customer="Henry Bautista", LoanNo="LN-011", Amount=5500m, PaymentMode="Cash", Principal=5000m, Interest=400m, Penalty=100m },
                new Controls.FullReportDialog.TransactionItem { Id="12", Date="Jun 06", ReceiptNo="OR-012", Customer="Isabel Cruz", LoanNo="LN-012", Amount=2600m, PaymentMode="GCash", Principal=2350m, Interest=200m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="13", Date="Jun 07", ReceiptNo="OR-013", Customer="Jose Reyes", LoanNo="LN-013", Amount=3100m, PaymentMode="Cash", Principal=2800m, Interest=250m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="14", Date="Jun 07", ReceiptNo="OR-014", Customer="Karen Lopez", LoanNo="LN-014", Amount=1900m, PaymentMode="Bank", Principal=1700m, Interest=150m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="15", Date="Jun 08", ReceiptNo="OR-015", Customer="Luis Garcia", LoanNo="LN-015", Amount=4800m, PaymentMode="Cash", Principal=4400m, Interest=350m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="16", Date="Jun 08", ReceiptNo="OR-016", Customer="Maria Ramos", LoanNo="LN-016", Amount=2200m, PaymentMode="GCash", Principal=2000m, Interest=150m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="17", Date="Jun 09", ReceiptNo="OR-017", Customer="Noel Santos", LoanNo="LN-017", Amount=3600m, PaymentMode="Cash", Principal=3300m, Interest=250m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="18", Date="Jun 09", ReceiptNo="OR-018", Customer="Oscar Cruz", LoanNo="LN-018", Amount=2700m, PaymentMode="Bank", Principal=2450m, Interest=200m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="19", Date="Jun 10", ReceiptNo="OR-019", Customer="Patricia Tan", LoanNo="LN-019", Amount=4100m, PaymentMode="Cash", Principal=3750m, Interest=300m, Penalty=50m },
                new Controls.FullReportDialog.TransactionItem { Id="20", Date="Jun 10", ReceiptNo="OR-020", Customer="Quincy Reyes", LoanNo="LN-020", Amount=3300m, PaymentMode="GCash", Principal=3000m, Interest=250m, Penalty=50m }
            };

            using (var dialog = new Controls.FullReportDialog())
            {
                dialog.SetReportData(selectedReport, dateFrom, dateTo, transactions);
                dialog.ShowDialog(this);
            }
        }

        private void ShowToast(string msg)
        {
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}