using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierDailyReport : Form
    {
        private class Transaction
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string ReceiptNo { get; set; }
            public string Customer { get; set; }
            public decimal Amount { get; set; }
            public string Mode { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal Penalty { get; set; }
            public decimal Charges { get; set; }
        }

        // UI Components
        private Panel root;
        private Panel mainCard;
        private Panel mainHeader;
        private Label lblMainTitle;

        private DateTimePicker dtpReportDate;
        private TextBox txtCashierName;

        // Summary Cards
        private Panel cardOpeningBalance;
        private Label lblOpeningBalanceValue;
        private Panel cardTotalCollections;
        private Label lblTotalCollectionsValue;
        private Panel cardTotalReleased;
        private Label lblTotalReleasedValue;
        private Panel cardClosingBalance;
        private Label lblClosingBalanceValue;
        private Panel cardVariance;
        private Label lblVarianceValue;

        // Tables
        private DataGridView dgvCollectionBreakdown;
        private DataGridView dgvPaymentAllocation;
        private DataGridView dgvTransactionDetails;

        // Buttons
        private Button btnRefresh;
        private Button btnPrintReport;
        private Button btnExportExcel;
        private Button btnEmailSupervisor;
        private Button btnEndOfDay;

        // Data
        private List<Transaction> transactions;
        private decimal openingBalance = 15000m;
        private decimal totalReleased = 50000m;
        private decimal variance = 0m;
        private string cashierName = "Maria Santos";

        public CashierDailyReport()
        {
            InitializeComponent();
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            Text = "Cashier - Daily Report";
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(16)
            };
            Controls.Add(root);

            // ===== Main Card =====
            mainCard = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Top,
                AutoSize = true
            };

            // Header
            mainHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                BorderStyle = BorderStyle.FixedSingle
            };

            var headerIcon = new Label
            {
                Text = "📄",
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(16, 16)
            };

            lblMainTitle = new Label
            {
                Text = "DAILY COLLECTION REPORT",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(50, 20)
            };

            mainHeader.Controls.Add(headerIcon);
            mainHeader.Controls.Add(lblMainTitle);

            // Content Panel - use TableLayoutPanel for proper ordering
            var contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                Padding = new Padding(24)
            };
            contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // Row 0: Report Header Info
            var headerInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 80,
                AutoSize = false
            };

            var lblReportDate = new Label
            {
                Text = "Report Date",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 10)
            };

            dtpReportDate = new DateTimePicker
            {
                Width = 200,
                Location = new Point(0, 32),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            var lblCashier = new Label
            {
                Text = "Cashier",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(240, 10)
            };

            txtCashierName = new TextBox
            {
                Text = cashierName,
                ReadOnly = true,
                Width = 200,
                Location = new Point(240, 32),
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle
            };

            headerInfoPanel.Controls.AddRange(new Control[] { lblReportDate, dtpReportDate, lblCashier, txtCashierName });

            // Row 1: Summary Totals
            var summaryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 130,
                AutoSize = false,
                BackColor = ColorTranslator.FromHtml("#ECFDF5"),
                BorderStyle = BorderStyle.FixedSingle
            };

            var summaryTitle = new Label
            {
                Text = "📈 SUMMARY TOTALS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 16)
            };
            summaryPanel.Controls.Add(summaryTitle);
            CreateSummaryCards(summaryPanel);

            // Row 2: Collection Breakdown
            var breakdownPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 200,
                AutoSize = false
            };

            var breakdownTitle = new Label
            {
                Text = "COLLECTION BREAKDOWN",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            breakdownPanel.Controls.Add(breakdownTitle);

            dgvCollectionBreakdown = CreateDataGridView();
            dgvCollectionBreakdown.Location = new Point(0, 30);
            dgvCollectionBreakdown.Height = 160;
            breakdownPanel.Controls.Add(dgvCollectionBreakdown);

            // Row 3: Payment Allocation
            var allocationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 200,
                AutoSize = false
            };

            var allocationTitle = new Label
            {
                Text = "PAYMENT ALLOCATION",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            allocationPanel.Controls.Add(allocationTitle);

            dgvPaymentAllocation = CreateDataGridView();
            dgvPaymentAllocation.Location = new Point(0, 30);
            dgvPaymentAllocation.Height = 160;
            allocationPanel.Controls.Add(dgvPaymentAllocation);

            // Row 4: Transaction Details
            var transactionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 320,
                AutoSize = false
            };

            var transactionTitle = new Label
            {
                Text = "TRANSACTION DETAILS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            transactionPanel.Controls.Add(transactionTitle);

            dgvTransactionDetails = CreateDataGridView();
            dgvTransactionDetails.Location = new Point(0, 30);
            dgvTransactionDetails.Height = 280;
            transactionPanel.Controls.Add(dgvTransactionDetails);

            // Row 5: Action Buttons
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 50,
                AutoSize = false,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            btnRefresh = CreateButton("⟳ Refresh", ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnRefresh.Click += (s, e) => ShowToast("Report refreshed");

            btnPrintReport = CreateButton("🖨️ Print Report", ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnPrintReport.Click += (s, e) => ShowToast("Sending daily report to printer...");

            btnExportExcel = CreateButton("📥 Export Excel", ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnExportExcel.Click += (s, e) => ShowToast("Exporting report to Excel...");

            btnEmailSupervisor = CreateButton("📧 Email to Supervisor", ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnEmailSupervisor.Width = 160;
            btnEmailSupervisor.Click += (s, e) => ShowToast("Daily report emailed to supervisor");

            btnEndOfDay = CreateButton("✅ End of Day", ColorTranslator.FromHtml("#16A34A"), Color.White);
            btnEndOfDay.Click += (s, e) => ShowToast("End of day process initiated. Finalizing transactions...");

            buttonsPanel.Controls.AddRange(new Control[] { btnRefresh, btnPrintReport, btnExportExcel, btnEmailSupervisor, btnEndOfDay });

            // Add rows to TableLayoutPanel in correct order (top to bottom)
            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(headerInfoPanel, 0, 0);

            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(summaryPanel, 0, 1);

            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(breakdownPanel, 0, 2);

            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(allocationPanel, 0, 3);

            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(transactionPanel, 0, 4);

            contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contentPanel.Controls.Add(buttonsPanel, 0, 5);

            // Add to mainCard (reverse order for Dock.Top)
            mainCard.Controls.Add(contentPanel);
            mainCard.Controls.Add(mainHeader);

            root.Controls.Add(mainCard);
        }

        private void CreateSummaryCards(Panel parent)
        {
            int cardWidth = 150;
            int cardHeight = 70;
            int spacing = 16;
            int startX = 16;
            int startY = 45;

            // Opening Balance Card
            cardOpeningBalance = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                Location = new Point(startX, startY),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };

            var lblOpeningBalanceTitle = new Label
            {
                Text = "Opening Balance",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(8, 8)
            };

            lblOpeningBalanceValue = new Label
            {
                Text = "₱15,000.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(8, 28)
            };

            cardOpeningBalance.Controls.Add(lblOpeningBalanceTitle);
            cardOpeningBalance.Controls.Add(lblOpeningBalanceValue);

            // Total Collections Card
            cardTotalCollections = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                Location = new Point(startX + cardWidth + spacing, startY),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            cardTotalCollections.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#86EFAC"), 2), 0, 0, cardWidth - 1, cardHeight - 1);

            var lblTotalCollectionsTitle = new Label
            {
                Text = "Total Collections",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(8, 8)
            };

            lblTotalCollectionsValue = new Label
            {
                Text = "₱0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#16A34A"),
                Location = new Point(8, 28)
            };

            cardTotalCollections.Controls.Add(lblTotalCollectionsTitle);
            cardTotalCollections.Controls.Add(lblTotalCollectionsValue);

            // Total Released Card
            cardTotalReleased = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                Location = new Point(startX + (cardWidth + spacing) * 2, startY),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            cardTotalReleased.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#FECACA"), 2), 0, 0, cardWidth - 1, cardHeight - 1);

            var lblTotalReleasedTitle = new Label
            {
                Text = "Total Released",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(8, 8)
            };

            lblTotalReleasedValue = new Label
            {
                Text = "₱50,000.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#DC2626"),
                Location = new Point(8, 28)
            };

            cardTotalReleased.Controls.Add(lblTotalReleasedTitle);
            cardTotalReleased.Controls.Add(lblTotalReleasedValue);

            // Closing Balance Card
            cardClosingBalance = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                Location = new Point(startX + (cardWidth + spacing) * 3, startY),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            cardClosingBalance.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#D8B4FE"), 2), 0, 0, cardWidth - 1, cardHeight - 1);

            var lblClosingBalanceTitle = new Label
            {
                Text = "Closing Balance",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(8, 8)
            };

            lblClosingBalanceValue = new Label
            {
                Text = "₱0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#7C3AED"),
                Location = new Point(8, 28)
            };

            cardClosingBalance.Controls.Add(lblClosingBalanceTitle);
            cardClosingBalance.Controls.Add(lblClosingBalanceValue);

            // Variance Card
            cardVariance = new Panel
            {
                Width = cardWidth,
                Height = cardHeight,
                Location = new Point(startX + (cardWidth + spacing) * 4, startY),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            cardVariance.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(ColorTranslator.FromHtml("#93C5FD"), 2), 0, 0, cardWidth - 1, cardHeight - 1);

            var lblVarianceTitle = new Label
            {
                Text = "Variance",
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(8, 8)
            };

            lblVarianceValue = new Label
            {
                Text = "₱0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1D4ED8"),
                Location = new Point(8, 28)
            };

            cardVariance.Controls.Add(lblVarianceTitle);
            cardVariance.Controls.Add(lblVarianceValue);

            parent.Controls.AddRange(new Control[] {
                cardOpeningBalance,
                cardTotalCollections,
                cardTotalReleased,
                cardClosingBalance,
                cardVariance
            });
        }

        private DataGridView CreateDataGridView()
        {
            return new DataGridView
            {
                Width = 850,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 32 },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(4)
                }
            };
        }

        private Button CreateButton(string text, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = 130,
                Height = 36,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            return btn;
        }

        private void LoadData()
        {
            transactions = new List<Transaction>
            {
                new Transaction { Id = "1", Time = "9:30 AM", ReceiptNo = "OR-001", Customer = "Juan Dela Cruz", Amount = 4442.44m, Mode = "Cash", Principal = 4000m, Interest = 442.44m, Penalty = 0m, Charges = 0m },
                new Transaction { Id = "2", Time = "10:15 AM", ReceiptNo = "OR-002", Customer = "Maria Santos", Amount = 2150m, Mode = "GCash", Principal = 2000m, Interest = 150m, Penalty = 0m, Charges = 0m },
                new Transaction { Id = "3", Time = "11:00 AM", ReceiptNo = "OR-003", Customer = "Pedro Reyes", Amount = 1500m, Mode = "Cash", Principal = 1350m, Interest = 150m, Penalty = 0m, Charges = 0m },
                new Transaction { Id = "4", Time = "1:30 PM", ReceiptNo = "OR-004", Customer = "Ana Garcia", Amount = 3000m, Mode = "Bank", Principal = 2700m, Interest = 250m, Penalty = 50m, Charges = 0m },
                new Transaction { Id = "5", Time = "2:15 PM", ReceiptNo = "OR-005", Customer = "Carlos Mendoza", Amount = 2500m, Mode = "Cash", Principal = 2200m, Interest = 250m, Penalty = 50m, Charges = 0m },
                new Transaction { Id = "6", Time = "2:45 PM", ReceiptNo = "OR-006", Customer = "Rosa Cruz", Amount = 1800m, Mode = "GCash", Principal = 1600m, Interest = 150m, Penalty = 50m, Charges = 0m },
                new Transaction { Id = "7", Time = "3:00 PM", ReceiptNo = "OR-007", Customer = "David Santos", Amount = 3200m, Mode = "Cash", Principal = 2900m, Interest = 250m, Penalty = 50m, Charges = 0m },
                new Transaction { Id = "8", Time = "3:15 PM", ReceiptNo = "OR-008", Customer = "Elena Ramos", Amount = 1500m, Mode = "GCash", Principal = 1350m, Interest = 150m, Penalty = 0m, Charges = 0m },
                new Transaction { Id = "9", Time = "3:30 PM", ReceiptNo = "OR-009", Customer = "Fernando Lopez", Amount = 2000m, Mode = "Bank", Principal = 1800m, Interest = 150m, Penalty = 50m, Charges = 0m },
                new Transaction { Id = "10", Time = "3:45 PM", ReceiptNo = "OR-010", Customer = "Gloria Tan", Amount = 1500m, Mode = "Cash", Principal = 1350m, Interest = 150m, Penalty = 0m, Charges = 0m },
                new Transaction { Id = "11", Time = "4:00 PM", ReceiptNo = "OR-011", Customer = "Henry Bautista", Amount = 5000m, Mode = "Cash", Principal = 4500m, Interest = 400m, Penalty = 100m, Charges = 0m }
            };

            CalculateAndDisplayData();
        }

        private void CalculateAndDisplayData()
        {
            decimal totalCollections = transactions.Sum(t => t.Amount);
            decimal closingBalance = openingBalance + totalCollections - totalReleased;

            lblOpeningBalanceValue.Text = $"₱{openingBalance:N2}";
            lblTotalCollectionsValue.Text = $"₱{totalCollections:N2}";
            lblTotalReleasedValue.Text = $"₱{totalReleased:N2}";
            lblClosingBalanceValue.Text = $"₱{closingBalance:N2}";
            lblVarianceValue.Text = $"₱{variance:N2}";

            lblClosingBalanceValue.ForeColor = closingBalance >= 0 ?
                ColorTranslator.FromHtml("#7C3AED") :
                ColorTranslator.FromHtml("#DC2626");

            // Collection Breakdown Table
            dgvCollectionBreakdown.Columns.Clear();
            dgvCollectionBreakdown.Columns.Add("Mode", "Mode");
            dgvCollectionBreakdown.Columns.Add("Count", "Count");
            dgvCollectionBreakdown.Columns.Add("Amount", "Amount");

            var cashTransactions = transactions.Where(t => t.Mode == "Cash").ToList();
            var gcashTransactions = transactions.Where(t => t.Mode == "GCash").ToList();
            var bankTransactions = transactions.Where(t => t.Mode == "Bank").ToList();

            dgvCollectionBreakdown.Rows.Add("Cash", cashTransactions.Count, $"₱{cashTransactions.Sum(t => t.Amount):N2}");
            dgvCollectionBreakdown.Rows.Add("GCash", gcashTransactions.Count, $"₱{gcashTransactions.Sum(t => t.Amount):N2}");
            dgvCollectionBreakdown.Rows.Add("Bank", bankTransactions.Count, $"₱{bankTransactions.Sum(t => t.Amount):N2}");
            dgvCollectionBreakdown.Rows.Add("Total", transactions.Count, $"₱{totalCollections:N2}");

            // Style total row
            if (dgvCollectionBreakdown.Rows.Count > 0)
            {
                var totalRow = dgvCollectionBreakdown.Rows[dgvCollectionBreakdown.Rows.Count - 1];
                totalRow.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                totalRow.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F3F4F6");
            }

            // Payment Allocation Table
            dgvPaymentAllocation.Columns.Clear();
            dgvPaymentAllocation.Columns.Add("Type", "Type");
            dgvPaymentAllocation.Columns.Add("Amount", "Amount");
            dgvPaymentAllocation.Columns.Add("Percentage", "%");

            decimal totalPrincipal = transactions.Sum(t => t.Principal);
            decimal totalInterest = transactions.Sum(t => t.Interest);
            decimal totalPenalty = transactions.Sum(t => t.Penalty);
            decimal totalCharges = transactions.Sum(t => t.Charges);

            string CalculatePercentage(decimal amount) =>
                totalCollections > 0 ? ((amount / totalCollections) * 100).ToString("0.0", CultureInfo.InvariantCulture) : "0.0";

            dgvPaymentAllocation.Rows.Add("Principal", $"₱{totalPrincipal:N2}", CalculatePercentage(totalPrincipal) + "%");
            dgvPaymentAllocation.Rows.Add("Interest", $"₱{totalInterest:N2}", CalculatePercentage(totalInterest) + "%");
            dgvPaymentAllocation.Rows.Add("Penalty", $"₱{totalPenalty:N2}", CalculatePercentage(totalPenalty) + "%");
            dgvPaymentAllocation.Rows.Add("Charges", $"₱{totalCharges:N2}", CalculatePercentage(totalCharges) + "%");
            dgvPaymentAllocation.Rows.Add("Total", $"₱{totalCollections:N2}", "100%");

            // Style total row
            if (dgvPaymentAllocation.Rows.Count > 0)
            {
                var totalRow = dgvPaymentAllocation.Rows[dgvPaymentAllocation.Rows.Count - 1];
                totalRow.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                totalRow.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F3F4F6");
            }

            // Transaction Details Table
            dgvTransactionDetails.Columns.Clear();
            dgvTransactionDetails.Columns.Add("Time", "Time");
            dgvTransactionDetails.Columns.Add("Receipt", "Receipt #");
            dgvTransactionDetails.Columns.Add("Customer", "Customer");
            dgvTransactionDetails.Columns.Add("Amount", "Amount");
            dgvTransactionDetails.Columns.Add("Mode", "Mode");

            foreach (var transaction in transactions)
            {
                int rowIndex = dgvTransactionDetails.Rows.Add(
                    transaction.Time,
                    transaction.ReceiptNo,
                    transaction.Customer,
                    $"₱{transaction.Amount:N2}",
                    transaction.Mode
                );

                // Style mode column with colors
                var modeCell = dgvTransactionDetails.Rows[rowIndex].Cells["Mode"];
                switch (transaction.Mode)
                {
                    case "Cash":
                        modeCell.Style.BackColor = ColorTranslator.FromHtml("#DCFCE7");
                        modeCell.Style.ForeColor = ColorTranslator.FromHtml("#166534");
                        break;
                    case "GCash":
                        modeCell.Style.BackColor = ColorTranslator.FromHtml("#DBEAFE");
                        modeCell.Style.ForeColor = ColorTranslator.FromHtml("#1E40AF");
                        break;
                    case "Bank":
                        modeCell.Style.BackColor = ColorTranslator.FromHtml("#F3E8FF");
                        modeCell.Style.ForeColor = ColorTranslator.FromHtml("#5B21B6");
                        break;
                }

                // Make amount column right-aligned
                dgvTransactionDetails.Rows[rowIndex].Cells["Amount"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void ShowToast(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}