using System;
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
            MessageBox.Show("Full report dialog would open here.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowToast(string msg)
        {
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}