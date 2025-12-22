using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class ReportPreviewControl : UserControl
    {
        public event EventHandler ViewFullReportClicked;

        public event EventHandler GenerateFullReportClicked;
        public event EventHandler ExportPdfClicked;
        public event EventHandler ExportExcelClicked;
        public event EventHandler ExportCsvClicked;
        public event EventHandler PrintClicked;

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
            Height = 520;

            var title = new Label
            {
                Text = "📄 REPORT PREVIEW",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            Controls.Add(title);

            // Outer preview container
            var outer = new Panel
            {
                Location = new Point(16, 40),
                Width = 818,
                Height = 460,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            Controls.Add(outer);

            // Inner double-border effect
            var inner = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12)
            };
            outer.Controls.Add(inner);

            // Main layout inside inner
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // header block
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // summary block
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // breakdown header
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));     // breakdown list (fills)
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // view full report
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));          // buttons row (bottom)
            inner.Controls.Add(layout);

            // ===== Header block =====
            var headerBlock = new Panel { Dock = DockStyle.Top, AutoSize = true };

            lblReportTitle = new Label
            {
                Text = "DAILY COLLECTION REPORT",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            headerBlock.Controls.Add(lblReportTitle);

            lblPeriod = new Label
            {
                Text = "Period: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 28)
            };
            headerBlock.Controls.Add(lblPeriod);

            lblGenerated = new Label
            {
                Text = "Generated: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(0, 48)
            };
            headerBlock.Controls.Add(lblGenerated);

            var sep1 = new Panel
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#D1D5DB"),
                Margin = new Padding(0, 8, 0, 8)
            };
            headerBlock.Controls.Add(sep1);

            headerBlock.Height = 70;
            layout.Controls.Add(headerBlock, 0, 0);

            // ===== Summary block =====
            var summaryBlock = new Panel { Dock = DockStyle.Top, AutoSize = true };

            var lblSummary = new Label
            {
                Text = "SUMMARY:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            summaryBlock.Controls.Add(lblSummary);

            var summaryGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Location = new Point(0, 20)
            };
            summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            AddSummaryRow(summaryGrid, 0, "Total Collections:", out lblTotalCollectionsValue);
            AddSummaryRow(summaryGrid, 1, "Total Transactions:", out lblTotalTransactionsValue);
            AddSummaryRow(summaryGrid, 2, "Avg. Daily:", out lblAvgDailyValue);

            summaryBlock.Controls.Add(summaryGrid);

            var sep2 = new Panel
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#D1D5DB"),
                Margin = new Padding(0, 8, 0, 8)
            };
            summaryBlock.Controls.Add(sep2);

            layout.Controls.Add(summaryBlock, 0, 1);

            // ===== Breakdown header =====
            var lblBreakdown = new Label
            {
                Text = "DAILY BREAKDOWN:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Margin = new Padding(0, 8, 0, 6)
            };
            layout.Controls.Add(lblBreakdown, 0, 2);

            // ===== Breakdown list (fills) =====
            pnlDailyBreakdown = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(12, 0, 0, 0)
            };
            layout.Controls.Add(pnlDailyBreakdown, 0, 3);

            // ===== View full report =====
            var viewPanel = new Panel { Dock = DockStyle.Top, Height = 26 };
            lnkViewFull = new LinkLabel
            {
                Text = "[View Full Report >>>]",
                AutoSize = true,
                LinkColor = ColorTranslator.FromHtml("#2563EB")
            };
            lnkViewFull.Click += (s, e) => ViewFullReportClicked?.Invoke(this, EventArgs.Empty);
            viewPanel.Controls.Add(lnkViewFull);
            viewPanel.Resize += (s, e) =>
            {
                lnkViewFull.Left = (viewPanel.Width - lnkViewFull.Width) / 2;
                lnkViewFull.Top = 4;
            };
            layout.Controls.Add(viewPanel, 0, 4);

            // ===== Buttons row (bottom) =====
            var actionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Margin = new Padding(0, 8, 0, 0)
            };

            var btnGenerate = CreateActionButton("📄 Generate Full Report", 170, ColorTranslator.FromHtml("#2563EB"), Color.White);
            btnGenerate.Click += (s, e) => GenerateFullReportClicked?.Invoke(this, EventArgs.Empty);

            var btnPdf = CreateActionButton("📥 Export PDF", 110, Color.White, ColorTranslator.FromHtml("#374151"));
            btnPdf.Click += (s, e) => ExportPdfClicked?.Invoke(this, EventArgs.Empty);

            var btnExcel = CreateActionButton("📥 Export Excel", 120, Color.White, ColorTranslator.FromHtml("#374151"));
            btnExcel.Click += (s, e) => ExportExcelClicked?.Invoke(this, EventArgs.Empty);

            var btnCsv = CreateActionButton("📥 Export CSV", 110, Color.White, ColorTranslator.FromHtml("#374151"));
            btnCsv.Click += (s, e) => ExportCsvClicked?.Invoke(this, EventArgs.Empty);

            var btnPrint = CreateActionButton("🖨️ Print", 90, Color.White, ColorTranslator.FromHtml("#374151"));
            btnPrint.Click += (s, e) => PrintClicked?.Invoke(this, EventArgs.Empty);

            actionsPanel.Controls.Add(btnGenerate);
            actionsPanel.Controls.Add(btnPdf);
            actionsPanel.Controls.Add(btnExcel);
            actionsPanel.Controls.Add(btnCsv);
            actionsPanel.Controls.Add(btnPrint);

            layout.Controls.Add(actionsPanel, 0, 5);
        }

        private Button CreateActionButton(string text, int width, Color back, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 0, 8, 0)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            return btn;
        }

        private void AddSummaryRow(TableLayoutPanel grid, int row, string label, out Label valueLabel)
        {
            if (grid.RowCount <= row) grid.RowCount = row + 1;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Margin = new Padding(0, 4, 0, 4)
            };

            valueLabel = new Label
            {
                Text = "-",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4)
            };

            grid.Controls.Add(lbl, 0, row);
            grid.Controls.Add(valueLabel, 1, row);
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