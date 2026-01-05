using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;

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

        // TODO: move these to a proper table (e.g., cashier_sessions / cash_drawer)
        private decimal openingBalance = 15000m;
        private decimal variance = 0m;

        // TODO: connect to authenticated user
        private string cashierName = "Maria Santos";

        public CashierDailyReport()
        {
            InitializeComponent();
            BuildUI();

            // Load from DB for today's date
            LoadDataFromDb(dtpReportDate.Value.Date);
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
            dtpReportDate.ValueChanged += (s, e) => LoadDataFromDb(dtpReportDate.Value.Date);

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
            btnRefresh.Click += (s, e) => LoadDataFromDb(dtpReportDate.Value.Date);

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

        private void LoadDataFromDb(DateTime reportDate)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var dayStart = reportDate.Date;
                    var dayEnd = reportDate.Date.AddDays(1);

                    // ===== payments for the day =====
                    var payments = (from p in db.Payments.AsNoTracking()
                                    where p.PaymentDate >= dayStart && p.PaymentDate < dayEnd
                                    select p).ToList();

                    // ===== map to "Transaction" rows =====
                    // join customers for display names
                    var customerIds = payments.Select(p => p.CustomerId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

                    var customerNames = db.Customers.AsNoTracking()
                        .Where(c => customerIds.Contains(c.CustomerId))
                        .Select(c => new
                        {
                            c.CustomerId,
                            Name = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim()
                        })
                        .ToList()
                        .ToDictionary(x => x.CustomerId, x => x.Name, StringComparer.OrdinalIgnoreCase);

                    transactions = payments
                        .OrderBy(p => p.PaymentDate)
                        .Select(p =>
                        {
                            string name;
                            if (!customerNames.TryGetValue(p.CustomerId ?? "", out name))
                                name = p.CustomerId ?? "";

                            return new Transaction
                            {
                                Id = p.PaymentId.ToString(CultureInfo.InvariantCulture),
                                Time = p.PaymentDate.ToString("h:mm tt", CultureInfo.InvariantCulture),
                                ReceiptNo = p.ReceiptNo ?? "",
                                Customer = name,
                                Amount = p.AmountPaid,
                                Mode = p.PaymentMethod ?? "Cash",
                                Principal = p.PrincipalPaid,
                                Interest = p.InterestPaid,
                                Penalty = p.PenaltyPaid,
                                Charges = 0m
                            };
                        })
                        .ToList();

                    // ===== totalReleased for the day =====
                    var releasedTotal = db.Loans.AsNoTracking()
                        .Where(l => l.ReleaseDate >= dayStart && l.ReleaseDate < dayEnd)
                        .Select(l => (decimal?)l.PrincipalAmount)
                        .DefaultIfEmpty(0m)
                        .Sum() ?? 0m;

                    // apply and render
                    CalculateAndDisplayData(releasedTotal);
                }
            }
            catch (Exception ex)
            {
                transactions = new List<Transaction>();
                CalculateAndDisplayData(0m);

                // shows the REAL database error (inner exception, SQL, etc.)
                MessageBox.Show(ex.ToString(), "Failed to load daily report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UPDATED: accept totalReleased from DB
        private void CalculateAndDisplayData(decimal totalReleasedFromDb)
        {
            var totalReleased = totalReleasedFromDb;

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

                dgvTransactionDetails.Rows[rowIndex].Cells["Amount"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void ShowToast(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}