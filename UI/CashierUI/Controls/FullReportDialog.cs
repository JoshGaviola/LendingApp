using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    /// <summary>
    /// Modal dialog showing full report with detailed transactions and summary statistics.
    /// </summary>
    public class FullReportDialog : Form
    {
        public class TransactionItem
        {
            public string Id { get; set; }
            public string Date { get; set; }
            public string ReceiptNo { get; set; }
            public string Customer { get; set; }
            public string LoanNo { get; set; }
            public decimal Amount { get; set; }
            public string PaymentMode { get; set; } // Cash | GCash | Bank
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal Penalty { get; set; }
        }

        // Data
        private string _reportTitle;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private List<TransactionItem> _allTransactions;

        // Pagination
        private int _currentPage = 1;
        private const int PageSize = 15;
        private int _totalPages = 1;

        // UI
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblPeriod;
        private Label lblGenerated;

        private Panel transactionsPanel;
        private DataGridView dgvTransactions;

        private Panel summaryPanel;

        private Panel paginationPanel;
        private Button btnPrevPage;
        private Button btnNextPage;
        private Label lblPageInfo;
        private Button btnClose;

        // Summary labels
        private Label lblTotalCollections;
        private Label lblTotalInterest;
        private Label lblTotalPrincipal;
        private Label lblTotalPenalties;
        private Label lblAvgDaily;
        private Label lblCashTotal;
        private Label lblGCashTotal;
        private Label lblBankTotal;

        public FullReportDialog()
        {
            InitializeDialog();
            BuildUI();
        }

        private void InitializeDialog()
        {
            Text = "Full Report";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 700);
            MinimumSize = new Size(800, 600);
            MaximizeBox = true;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = Color.White;
            ShowInTaskbar = false;
        }

        private void BuildUI()
        {
            Controls.Clear();

            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(16)
            };

            lblTitle = new Label
            {
                Text = "DAILY COLLECTION REPORT",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 12)
            };

            lblPeriod = new Label
            {
                Text = "Period: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 40)
            };

            lblGenerated = new Label
            {
                Text = "Generated: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblPeriod);
            headerPanel.Controls.Add(lblGenerated);

            headerPanel.Resize += (s, e) =>
            {
                lblGenerated.Location = new Point(headerPanel.Width - lblGenerated.Width - 16, 12);
            };

            // Separator
            var sep1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = ColorTranslator.FromHtml("#E5E7EB")
            };

            // Transactions panel
            transactionsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                BackColor = Color.White,
                Padding = new Padding(16)
            };

            var transHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = ColorTranslator.FromHtml("#F3F4F6"),
                BorderStyle = BorderStyle.FixedSingle
            };
            var transTitle = new Label
            {
                Text = "DETAILED TRANSACTION LIST",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(12, 6)
            };
            transHeader.Controls.Add(transTitle);

            dgvTransactions = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 32,
                RowTemplate = { Height = 28 }
            };

            dgvTransactions.Columns.Add("Date", "Date");
            dgvTransactions.Columns.Add("ReceiptNo", "Receipt#");
            dgvTransactions.Columns.Add("Customer", "Customer");
            dgvTransactions.Columns.Add("LoanNo", "Loan#");
            dgvTransactions.Columns.Add("Amount", "Amount");

            dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 8, 0, 0)
            };
            gridHost.Controls.Add(dgvTransactions);

            transactionsPanel.Controls.Add(gridHost);
            transactionsPanel.Controls.Add(transHeader);

            // Summary panel
            summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 260, // Increased from 240
                BackColor = Color.White,
                Padding = new Padding(16)
            };
            BuildSummaryPanel();

            // Pagination
            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 12)
            };

            btnPrevPage = CreateButton("◀ Previous", 100, Color.White, ColorTranslator.FromHtml("#374151"));
            btnPrevPage.Location = new Point(16, 14);
            btnPrevPage.Click += (s, e) => ChangePage(_currentPage - 1);

            lblPageInfo = new Label
            {
                Text = "Page 1 of 1",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };

            btnNextPage = CreateButton("Next ▶", 100, Color.White, ColorTranslator.FromHtml("#374151"));
            btnNextPage.Click += (s, e) => ChangePage(_currentPage + 1);

            btnClose = CreateButton("✕ Close", 90, Color.White, ColorTranslator.FromHtml("#374151"));
            btnClose.Click += (s, e) => Close();

            paginationPanel.Controls.Add(btnPrevPage);
            paginationPanel.Controls.Add(lblPageInfo);
            paginationPanel.Controls.Add(btnNextPage);
            paginationPanel.Controls.Add(btnClose);

            paginationPanel.Resize += (s, e) =>
            {
                int centerX = paginationPanel.Width / 2;
                lblPageInfo.Location = new Point(centerX - lblPageInfo.Width / 2, 18);
                btnNextPage.Location = new Point(centerX + 60, 14);
                btnClose.Location = new Point(paginationPanel.Width - btnClose.Width - 16, 14);
            };

            // Add controls (order matters for docking)
            Controls.Add(summaryPanel);
            Controls.Add(transactionsPanel);
            Controls.Add(sep1);
            Controls.Add(headerPanel);
            Controls.Add(paginationPanel);
        }

        private void BuildSummaryPanel()
        {
            summaryPanel.Controls.Clear();

            var summaryHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = ColorTranslator.FromHtml("#F3F4F6"),
                BorderStyle = BorderStyle.FixedSingle
            };
            var summaryTitle = new Label
            {
                Text = "SUMMARY STATISTICS",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(12, 6)
            };
            summaryHeader.Controls.Add(summaryTitle);

            var summaryBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 12, 16, 12),
                AutoScroll = true
            };

            // Main financial summary
            var summaryGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 12)
            };
            summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
            summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));

            AddSummaryRow(summaryGrid, "Total Collections:", out lblTotalCollections);
            AddSummaryRow(summaryGrid, "Total Interest Collected:", out lblTotalInterest);
            AddSummaryRow(summaryGrid, "Total Principal Collected:", out lblTotalPrincipal);
            AddSummaryRow(summaryGrid, "Total Penalties:", out lblTotalPenalties);
            AddSummaryRow(summaryGrid, "Average Daily Collection:", out lblAvgDaily);

            summaryBody.Controls.Add(summaryGrid);

            // Separator
            var sep = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#E5E7EB"),
                Margin = new Padding(0, 0, 0, 12)
            };
            summaryBody.Controls.Add(sep);

            // By Payment Mode section
            var modeTitle = new Label
            {
                Text = "By Payment Mode:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };
            summaryBody.Controls.Add(modeTitle);

            var modeGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Margin = new Padding(16, 0, 0, 0) // Indent for bullet points
            };
            modeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            modeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            AddSummaryRow(modeGrid, "• Cash:", out lblCashTotal);
            AddSummaryRow(modeGrid, "• GCash:", out lblGCashTotal);
            AddSummaryRow(modeGrid, "• Bank:", out lblBankTotal);

            summaryBody.Controls.Add(modeGrid);

            // Add spacer at bottom to prevent clipping
            var spacer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 20
            };
            summaryBody.Controls.Add(spacer);

            // Add all to summary panel
            summaryPanel.Controls.Add(summaryBody);
            summaryPanel.Controls.Add(summaryHeader);

            // Ensure proper layout
            summaryPanel.PerformLayout();
        }

        private void AddSummaryRow(TableLayoutPanel grid, string label, out Label valueLabel)
        {
            int row = grid.RowCount;
            grid.RowCount = row + 1;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Margin = new Padding(0, 4, 0, 4),
                Dock = DockStyle.Fill
            };

            valueLabel = new Label
            {
                Text = "₱0.00 (0%)",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4),
                Padding = new Padding(0, 0, 10, 0) // Right padding to prevent clipping
            };

            grid.Controls.Add(lbl, 0, row);
            grid.Controls.Add(valueLabel, 1, row);
        }

        private Button CreateButton(string text, int width, Color back, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            return btn;
        }

        /// <summary>
        /// Sets report data and refreshes the dialog.
        /// </summary>
        public void SetReportData(
            string reportTitle,
            DateTime dateFrom,
            DateTime dateTo,
            IEnumerable<TransactionItem> transactions)
        {
            _reportTitle = reportTitle ?? "Report";
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _allTransactions = transactions?.ToList() ?? new List<TransactionItem>();

            _totalPages = Math.Max(1, (int)Math.Ceiling(_allTransactions.Count / (double)PageSize));
            _currentPage = 1;

            RefreshUI();
        }

        private void RefreshUI()
        {
            lblTitle.Text = _reportTitle.ToUpperInvariant();
            lblPeriod.Text = $"{_dateFrom:yyyy-MM-dd} to {_dateTo:yyyy-MM-dd}";
            lblGenerated.Text = $"Generated: {DateTime.Now:MMM d, yyyy h:mm tt}";

            BindTransactionsPage();
            UpdateSummary();
            UpdatePagination();

            // Ensure header right aligned label recomputes after text changes
            headerPanel.PerformLayout();
            headerPanel.Invalidate();
        }

        private void BindTransactionsPage()
        {
            dgvTransactions.Rows.Clear();

            var pageItems = _allTransactions
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (var t in pageItems)
            {
                int idx = dgvTransactions.Rows.Add(
                    t.Date,
                    t.ReceiptNo,
                    t.Customer,
                    t.LoanNo,
                    $"₱{t.Amount:N2}"
                );

                dgvTransactions.Rows[idx].Cells["Amount"].Style.ForeColor = ColorTranslator.FromHtml("#111827");
            }
        }

        private void UpdateSummary()
        {
            if (_allTransactions == null || _allTransactions.Count == 0)
            {
                lblTotalCollections.Text = "₱0.00";
                lblTotalInterest.Text = "₱0.00 (0%)";
                lblTotalPrincipal.Text = "₱0.00 (0%)";
                lblTotalPenalties.Text = "₱0.00 (0%)";
                lblAvgDaily.Text = "₱0.00";
                lblCashTotal.Text = "₱0.00 (0%)";
                lblGCashTotal.Text = "₱0.00 (0%)";
                lblBankTotal.Text = "₱0.00 (0%)";
                return;
            }

            decimal totalCollections = _allTransactions.Sum(t => t.Amount);
            decimal totalInterest = _allTransactions.Sum(t => t.Interest);
            decimal totalPrincipal = _allTransactions.Sum(t => t.Principal);
            decimal totalPenalties = _allTransactions.Sum(t => t.Penalty);

            int days = Math.Max(1, (_dateTo - _dateFrom).Days + 1);
            decimal avgDaily = totalCollections / days;

            decimal cashTotal = _allTransactions.Where(t => t.PaymentMode == "Cash").Sum(t => t.Amount);
            decimal gcashTotal = _allTransactions.Where(t => t.PaymentMode == "GCash").Sum(t => t.Amount);
            decimal bankTotal = _allTransactions.Where(t => t.PaymentMode == "Bank").Sum(t => t.Amount);

            string Pct(decimal val) => totalCollections > 0
                ? ((val / totalCollections) * 100).ToString("0", CultureInfo.InvariantCulture) + "%"
                : "0%";

            lblTotalCollections.Text = $"₱{totalCollections:N2}";
            lblTotalInterest.Text = $"₱{totalInterest:N2} ({Pct(totalInterest)})";
            lblTotalPrincipal.Text = $"₱{totalPrincipal:N2} ({Pct(totalPrincipal)})";
            lblTotalPenalties.Text = $"₱{totalPenalties:N2} ({Pct(totalPenalties)})";
            lblAvgDaily.Text = $"₱{avgDaily:N2}";

            lblCashTotal.Text = $"₱{cashTotal:N2} ({Pct(cashTotal)})";
            lblGCashTotal.Text = $"₱{gcashTotal:N2} ({Pct(gcashTotal)})";
            lblBankTotal.Text = $"₱{bankTotal:N2} ({Pct(bankTotal)})";
        }

        private void UpdatePagination()
        {
            lblPageInfo.Text = $"Page {_currentPage} of {_totalPages}";
            btnPrevPage.Enabled = _currentPage > 1;
            btnNextPage.Enabled = _currentPage < _totalPages;
        }

        private void ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > _totalPages) return;
            _currentPage = newPage;
            BindTransactionsPage();
            UpdatePagination();
        }
    }
}