using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using System.Data.Entity;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierReciept : Form
    {
        private class ReceiptData
        {
            public string Id { get; set; }
            public string ReceiptNo { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; }
            public string Customer { get; set; }
            public string LoanAccount { get; set; }
            public decimal Amount { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal Penalty { get; set; }
            public decimal Charges { get; set; }
            public string PaymentMode { get; set; } // Cash | GCash | Bank
            public string Cashier { get; set; }
            public string Type { get; set; } // Payment | Loan
            public string Status { get; set; } // Printed | Emailed | Voided
        }

        // Data
        private List<ReceiptData> receipts;
        private ReceiptData selectedReceipt;
        private string searchQuery = "";
        private DateTime dateFrom;
        private DateTime dateTo;
        private string typeFilter = "All";
        private string statusFilter = "All";

        // UI Components
        private Panel root;
        private Panel mainCard;

        // Search & Filter
        private TextBox txtSearch;
        private DateTimePicker dtpDateFrom;
        private DateTimePicker dtpDateTo;
        private ComboBox cmbType;
        private ComboBox cmbStatus;

        // Receipts List
        private DataGridView dgvReceipts;
        private Label lblReceiptCount;

        // Receipt Details
        private Panel pnlReceiptDetails;
        private Label lblSelectedReceipt;

        // Receipt Preview Labels
        private Label lblPreviewReceiptNo;
        private Label lblPreviewDate;
        private Label lblPreviewCustomer;
        private Label lblPreviewLoanAccount;
        private Label lblPreviewPrincipal;
        private Label lblPreviewInterest;
        private Label lblPreviewPenalty;
        private Label lblPreviewTotal;
        private Label lblPreviewPaymentMode;
        private Label lblPreviewCashier;

        // Action Buttons
        private Button btnReprint;
        private Button btnEmail;
        private Button btnVoid;
        private Button btnExportBatch;
        private Button btnPrintAllToday;
        private Button btnSearchCustomer;

        // Summary Labels
        private Label lblTotalReceipts;
        private Label lblTotalAmount;
        private Label lblPrintedCount;
        private Label lblEmailedCount;
        private Label lblVoidedCount;

        public CashierReciept()
        {
            InitializeComponent();
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            // Default to recent week
            dateFrom = DateTime.Today.AddDays(-7);
            dateTo = DateTime.Today;

            // IMPORTANT: keep your original UI builder
            BuildUI();

            // Refresh when a new receipt is created by payment processing
            ReceiptEvents.ReceiptCreated += OnReceiptCreated;

            ApplyFilters();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReceiptEvents.ReceiptCreated -= OnReceiptCreated;
            }
            base.Dispose(disposing);
        }

        private void OnReceiptCreated(ReceiptDto dto)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnReceiptCreated(dto)));
                return;
            }
            // Re-query DB to stay consistent
            ApplyFilters();
        }

        // Load receipts from DB (payments table)
        private void LoadReceiptsFromDb()
        {
            receipts = new List<ReceiptData>();

            var start = dateFrom.Date;
            var endExclusive = dateTo.Date.AddDays(1);

            using (var db = new AppDbContext())
            {
                var rows =
                    (from p in db.Payments.AsNoTracking()
                     join l in db.Loans.AsNoTracking() on p.LoanId equals l.LoanId
                     join c in db.Customers.AsNoTracking() on p.CustomerId equals c.CustomerId
                     where p.PaymentDate >= start && p.PaymentDate < endExclusive
                     orderby p.PaymentDate descending
                     select new
                     {
                         p.PaymentId,
                         p.ReceiptNo,
                         p.PaymentDate,
                         Customer = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                         LoanNumber = l.LoanNumber,
                         Amount = p.AmountPaid,
                         Principal = p.PrincipalPaid,
                         Interest = p.InterestPaid,
                         Penalty = p.PenaltyPaid,
                         Method = p.PaymentMethod
                     })
                    .ToList();

                receipts = rows.Select(x => new ReceiptData
                {
                    Id = x.PaymentId.ToString(CultureInfo.InvariantCulture),
                    ReceiptNo = x.ReceiptNo,
                    Date = x.PaymentDate.Date,
                    Time = x.PaymentDate.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-US")),
                    Customer = x.Customer,
                    LoanAccount = x.LoanNumber,
                    Amount = x.Amount,
                    Principal = x.Principal,
                    Interest = x.Interest,
                    Penalty = x.Penalty,
                    Charges = 0m,
                    PaymentMode = x.Method,
                    Cashier = "",      // Option A: not joining users yet
                    Type = "Payment",
                    Status = "Printed" // Option A: no status columns in DB
                }).ToList();
            }
        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(16)
            };
            Controls.Add(root);

            // Main Card
            mainCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#F3E8FF"),
                BorderStyle = BorderStyle.FixedSingle
            };

            var headerIcon = new Label
            {
                Text = "🧾",
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(16, 16)
            };

            var headerTitle = new Label
            {
                Text = "RECEIPT MANAGEMENT",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(50, 20)
            };

            header.Controls.Add(headerIcon);
            header.Controls.Add(headerTitle);

            // Content
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                Padding = new Padding(24)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // Row 0: Search & Filter
            var filterPanel = CreateFilterPanel();
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.Controls.Add(filterPanel, 0, 0);

            // Row 1: Receipts List
            var listPanel = CreateReceiptsListPanel();
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.Controls.Add(listPanel, 0, 1);

            // Row 2: Receipt Details
            pnlReceiptDetails = CreateReceiptDetailsPanel();
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.Controls.Add(pnlReceiptDetails, 0, 2);

            // Row 3: Action Buttons
            var actionsPanel = CreateActionsPanel();
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.Controls.Add(actionsPanel, 0, 3);

            mainCard.Controls.Add(content);
            mainCard.Controls.Add(header);

            // Today's Summary Card
            var summaryCard = CreateSummaryCard();
            summaryCard.Dock = DockStyle.Bottom;

            root.Controls.Add(summaryCard);
            root.Controls.Add(mainCard);
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 140,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16)
            };

            var title = new Label
            {
                Text = "🔍 SEARCH & FILTER",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            panel.Controls.Add(title);

            // Search
            var lblSearch = new Label
            {
                Text = "Search",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 40)
            };
            panel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Width = 280,
                Location = new Point(16, 60)
            };
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Receipt #, Customer, or Loan #";
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.ForeColor = Color.Gray;
                    txtSearch.Text = "Receipt #, Customer, or Loan #";
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray) return;
                searchQuery = txtSearch.Text;
                ApplyFilters();
            };
            panel.Controls.Add(txtSearch);

            var btnSearch = CreateButton("🔍", 36, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnSearch.Location = new Point(300, 58);
            panel.Controls.Add(btnSearch);

            // Date From
            var lblDateFrom = new Label
            {
                Text = "Date From",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(360, 40)
            };
            panel.Controls.Add(lblDateFrom);

            dtpDateFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Value = dateFrom,
                Location = new Point(360, 60)
            };
            dtpDateFrom.ValueChanged += (s, e) =>
            {
                dateFrom = dtpDateFrom.Value;
                ApplyFilters();
            };
            panel.Controls.Add(dtpDateFrom);

            // Date To
            var lblDateTo = new Label
            {
                Text = "Date To",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(500, 40)
            };
            panel.Controls.Add(lblDateTo);

            dtpDateTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 120,
                Value = dateTo,
                Location = new Point(500, 60)
            };
            dtpDateTo.ValueChanged += (s, e) =>
            {
                dateTo = dtpDateTo.Value;
                ApplyFilters();
            };
            panel.Controls.Add(dtpDateTo);

            // Type Filter
            var lblType = new Label
            {
                Text = "Type",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(640, 40)
            };
            panel.Controls.Add(lblType);

            cmbType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100,
                Location = new Point(640, 60)
            };
            cmbType.Items.AddRange(new object[] { "All", "Payment", "Loan" });
            cmbType.SelectedIndex = 0;
            cmbType.SelectedIndexChanged += (s, e) =>
            {
                typeFilter = cmbType.SelectedItem?.ToString() ?? "All";
                ApplyFilters();
            };
            panel.Controls.Add(cmbType);

            // Status Filter
            var lblStatus = new Label
            {
                Text = "Status",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(760, 40)
            };
            panel.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100,
                Location = new Point(760, 60)
            };
            cmbStatus.Items.AddRange(new object[] { "All", "Printed", "Emailed", "Voided" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                statusFilter = cmbStatus.SelectedItem?.ToString() ?? "All";
                ApplyFilters();
            };
            panel.Controls.Add(cmbStatus);

            return panel;
        }

        private Panel CreateReceiptsListPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 280,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16),
                Margin = new Padding(0, 12, 0, 0)
            };

            lblReceiptCount = new Label
            {
                Text = "RECEIPTS LIST (0)",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            panel.Controls.Add(lblReceiptCount);

            dgvReceipts = new DataGridView
            {
                Location = new Point(16, 40),
                Width = 850,
                Height = 220,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 32 }
            };

            dgvReceipts.Columns.Add("ReceiptNo", "Receipt #");
            dgvReceipts.Columns.Add("Date", "Date");
            dgvReceipts.Columns.Add("Customer", "Customer");
            dgvReceipts.Columns.Add("Amount", "Amount");
            dgvReceipts.Columns.Add("Status", "Status");

            dgvReceipts.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvReceipts.SelectionChanged += DgvReceipts_SelectionChanged;

            panel.Controls.Add(dgvReceipts);

            return panel;
        }

        private Panel CreateReceiptDetailsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 560,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16),
                Margin = new Padding(0, 12, 0, 0)
            };

            lblSelectedReceipt = new Label
            {
                Text = "Selected Receipt: -",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            panel.Controls.Add(lblSelectedReceipt);

            var detailsTitle = new Label
            {
                Text = "RECEIPT DETAILS",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 40)
            };
            panel.Controls.Add(detailsTitle);

            var previewCard = new Panel
            {
                Location = new Point(150, 70),
                Width = 500,
                Height = 430,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var innerBorder = new Panel
            {
                Location = new Point(16, 16),
                Width = 466,
                Height = 596,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16)
            };

            var receiptHeader = new Label
            {
                Text = "OFFICIAL RECEIPT",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(150, 16)
            };
            innerBorder.Controls.Add(receiptHeader);

            var companyName = new Label
            {
                Text = "Lending Company Name",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(160, 42)
            };
            innerBorder.Controls.Add(companyName);

            int y = 75;

            AddPreviewRow(innerBorder, "Receipt No:", ref lblPreviewReceiptNo, ref y);
            AddPreviewRow(innerBorder, "Date:", ref lblPreviewDate, ref y);

            y += 10;
            AddPreviewRow(innerBorder, "Received from:", ref lblPreviewCustomer, ref y);
            AddPreviewRow(innerBorder, "Loan Account:", ref lblPreviewLoanAccount, ref y);

            y += 10;
            var paymentDetails = new Label
            {
                Text = "Payment Details:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, y)
            };
            innerBorder.Controls.Add(paymentDetails);
            y += 22;

            AddPreviewRow(innerBorder, "  • Principal:", ref lblPreviewPrincipal, ref y, 30);
            AddPreviewRow(innerBorder, "  • Interest:", ref lblPreviewInterest, ref y, 30);
            AddPreviewRow(innerBorder, "  • Penalty:", ref lblPreviewPenalty, ref y, 30);

            y += 10;
            AddPreviewRow(innerBorder, "Total Amount:", ref lblPreviewTotal, ref y, 0, true);
            AddPreviewRow(innerBorder, "Payment Mode:", ref lblPreviewPaymentMode, ref y);

            y += 10;
            AddPreviewRow(innerBorder, "Cashier:", ref lblPreviewCashier, ref y);

            previewCard.Controls.Add(innerBorder);
            panel.Controls.Add(previewCard);

            return panel;
        }

        private void AddPreviewRow(Panel parent, string label, ref Label valueLabel, ref int y, int indent = 0, bool bold = false)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16 + indent, y)
            };
            parent.Controls.Add(lbl);

            valueLabel = new Label
            {
                Text = "-",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(350, y)
            };
            parent.Controls.Add(valueLabel);

            y += 22;
        }

        private Panel CreateActionsPanel()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 50,
                AutoSize = false,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = new Padding(0, 12, 0, 0)
            };

            btnReprint = CreateButton("🖨️ Reprint Receipt", 140, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnReprint.Click += (s, e) =>
            {
                if (selectedReceipt == null)
                {
                    ShowToast("No receipt selected");
                    return;
                }

                try
                {
                    var path = ReceiptPdfGenerator.GeneratePdf(
                        selectedReceipt.ReceiptNo,
                        selectedReceipt.Date,
                        selectedReceipt.Time,
                        selectedReceipt.Customer,
                        selectedReceipt.LoanAccount,
                        selectedReceipt.Principal,
                        selectedReceipt.Interest,
                        selectedReceipt.Penalty,
                        selectedReceipt.Amount,
                        selectedReceipt.PaymentMode,
                        selectedReceipt.Cashier
                    );

                    ShowToast("PDF opened: " + path);
                }
                catch (Exception ex)
                {
                    ShowToast("Failed to generate PDF: " + ex.Message);
                }
            };

            btnEmail = CreateButton("📧 Email Receipt", 130, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnEmail.Click += (s, e) => ShowToast($"Receipt {selectedReceipt?.ReceiptNo} emailed to customer");

            btnVoid = CreateButton("❌ Void Receipt", 120, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#B91C1C"));
            btnVoid.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#FCA5A5");
            btnVoid.Click += (s, e) => ShowToast($"Receipt {selectedReceipt?.ReceiptNo} marked as void");

            btnExportBatch = CreateButton("📥 Export Batch", 120, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnExportBatch.Click += (s, e) => ShowToast("Exporting batch receipts...");

            btnPrintAllToday = CreateButton("🖨️ Print All Today", 140, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnPrintAllToday.Click += (s, e) =>
            {
                var todayReceipts = receipts.Where(r => r.Date.Date == DateTime.Today).ToList();
                ShowToast($"Printing all {todayReceipts.Count} receipts from today");
            };

            btnSearchCustomer = CreateButton("👤 Search by Customer", 160, ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#374151"));
            btnSearchCustomer.Click += (s, e) => ShowToast("Opening customer search dialog...");

            panel.Controls.Add(btnReprint);
            panel.Controls.Add(btnEmail);
            panel.Controls.Add(btnVoid);
            panel.Controls.Add(btnExportBatch);
            panel.Controls.Add(btnPrintAllToday);
            panel.Controls.Add(btnSearchCustomer);

            return panel;
        }

        private Panel CreateSummaryCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 12, 0, 0)
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#ECFDF5"),
                BorderStyle = BorderStyle.FixedSingle
            };

            var headerTitle = new Label
            {
                Text = "📅 TODAY'S RECEIPT SUMMARY",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 12),
            };
            header.Controls.Add(headerTitle);
            card.Controls.Add(header);

            var body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24)
            };

            // Total Receipts
            lblTotalReceipts = new Label
            {
                Text = "0",
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(60, 20)
            };
            body.Controls.Add(lblTotalReceipts);

            var lblTotalReceiptsTitle = new Label
            {
                Text = "Total Receipts",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(40, 50)
            };
            body.Controls.Add(lblTotalReceiptsTitle);

            // Total Amount
            lblTotalAmount = new Label
            {
                Text = "₱0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#16A34A"),
                Location = new Point(200, 20)
            };
            body.Controls.Add(lblTotalAmount);

            var lblTotalAmountTitle = new Label
            {
                Text = "Total Amount",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(200, 50)
            };
            body.Controls.Add(lblTotalAmountTitle);

            // Printed/Emailed
            lblPrintedCount = new Label
            {
                Text = "Printed: 0",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(400, 25)
            };
            body.Controls.Add(lblPrintedCount);

            lblEmailedCount = new Label
            {
                Text = "Emailed: 0",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(400, 50)
            };
            body.Controls.Add(lblEmailedCount);

            // Voided
            lblVoidedCount = new Label
            {
                Text = "Voided: 0",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#DC2626"),
                Location = new Point(550, 35)
            };
            body.Controls.Add(lblVoidedCount);

            card.Controls.Add(body);

            return card;
        }

        private Button CreateButton(string text, int width, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
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

        private void ApplyFilters()
        {
            try
            {
                // Load fresh data for the selected date range
                LoadReceiptsFromDb();
            }
            catch (Exception ex)
            {
                receipts = new List<ReceiptData>();
                ShowToast("Failed to load receipts: " + ex.Message);
            }

            var filtered = receipts.Where(r =>
            {
                bool matchesSearch = string.IsNullOrWhiteSpace(searchQuery) ||
                    (r.ReceiptNo ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.Customer ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.LoanAccount ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                bool matchesType = typeFilter == "All" || r.Type == typeFilter;
                bool matchesStatus = statusFilter == "All" || r.Status == statusFilter;
                bool matchesDate = r.Date >= dateFrom.Date && r.Date <= dateTo.Date;

                return matchesSearch && matchesType && matchesStatus && matchesDate;
            }).ToList();

            BindReceipts(filtered);
            UpdateSummary();
        }

        private void BindReceipts(List<ReceiptData> filtered)
        {
            dgvReceipts.Rows.Clear();
            lblReceiptCount.Text = $"RECEIPTS LIST ({filtered.Count})";

            foreach (var r in filtered)
            {
                int idx = dgvReceipts.Rows.Add(
                    r.ReceiptNo,
                    r.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    r.Customer,
                    $"₱{r.Amount:N2}",
                    r.Status
                );

                dgvReceipts.Rows[idx].Tag = r;

                // Style status cell
                var statusCell = dgvReceipts.Rows[idx].Cells["Status"];
                switch (r.Status)
                {
                    case "Printed":
                        statusCell.Style.BackColor = ColorTranslator.FromHtml("#DCFCE7");
                        statusCell.Style.ForeColor = ColorTranslator.FromHtml("#166534");
                        break;
                    case "Emailed":
                        statusCell.Style.BackColor = ColorTranslator.FromHtml("#DBEAFE");
                        statusCell.Style.ForeColor = ColorTranslator.FromHtml("#1E40AF");
                        break;
                    case "Voided":
                        statusCell.Style.BackColor = ColorTranslator.FromHtml("#FEE2E2");
                        statusCell.Style.ForeColor = ColorTranslator.FromHtml("#991B1B");
                        break;
                }
            }

            if (filtered.Count > 0 && dgvReceipts.Rows.Count > 0)
            {
                dgvReceipts.Rows[0].Selected = true;
            }
        }

        private void DgvReceipts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceipts.SelectedRows.Count > 0)
            {
                selectedReceipt = dgvReceipts.SelectedRows[0].Tag as ReceiptData;
                UpdateReceiptPreview();
            }
        }

        private void UpdateReceiptPreview()
        {
            if (selectedReceipt == null)
            {
                lblSelectedReceipt.Text = "Selected Receipt: -";
                return;
            }

            lblSelectedReceipt.Text = $"Selected Receipt: {selectedReceipt.ReceiptNo}";

            lblPreviewReceiptNo.Text = selectedReceipt.ReceiptNo;
            lblPreviewDate.Text = $"{selectedReceipt.Date:MMMM d, yyyy} {selectedReceipt.Time}";
            lblPreviewCustomer.Text = selectedReceipt.Customer;
            lblPreviewLoanAccount.Text = selectedReceipt.LoanAccount;
            lblPreviewPrincipal.Text = $"₱{selectedReceipt.Principal:N2}";
            lblPreviewInterest.Text = $"₱{selectedReceipt.Interest:N2}";
            lblPreviewPenalty.Text = $"₱{selectedReceipt.Penalty:N2}";
            lblPreviewTotal.Text = $"₱{selectedReceipt.Amount:N2}";
            lblPreviewPaymentMode.Text = selectedReceipt.PaymentMode;
            lblPreviewCashier.Text = selectedReceipt.Cashier;

            // Update button states
            bool hasSelection = selectedReceipt != null;
            btnReprint.Enabled = hasSelection;
            btnEmail.Enabled = hasSelection;
            btnVoid.Enabled = hasSelection;
        }

        private void UpdateSummary()
        {
            var todayReceipts = receipts.Where(r => r.Date.Date == DateTime.Today).ToList();
            decimal totalAmount = todayReceipts.Sum(r => r.Amount);
            int printedCount = todayReceipts.Count(r => r.Status == "Printed");
            int emailedCount = todayReceipts.Count(r => r.Status == "Emailed");
            int voidedCount = todayReceipts.Count(r => r.Status == "Voided");

            lblTotalReceipts.Text = todayReceipts.Count.ToString(CultureInfo.InvariantCulture);
            lblTotalAmount.Text = $"₱{totalAmount:N2}";
            lblPrintedCount.Text = $"Printed: {printedCount}";
            lblEmailedCount.Text = $"Emailed: {emailedCount}";
            lblVoidedCount.Text = $"Voided: {voidedCount}";
        }

        private void ShowToast(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Add this public helper so other forms can load and select a specific receipt by receipt number.
        public void LoadAndSelectReceipt(string receiptNo)
        {
            if (string.IsNullOrWhiteSpace(receiptNo)) return;

            // Use a wide but safe date range so ApplyFilters() / DateTimePicker won't overflow.
            // DateTimePicker and AddDays on DateTime.MaxValue can overflow; avoid MinValue/MaxValue.
            dateFrom = DateTime.Today.AddYears(-50);
            dateTo = DateTime.Today.AddYears(50);
            searchQuery = receiptNo;

            // Rebuild list and bind
            ApplyFilters();

            // Find and select the matching row
            for (int i = 0; i < dgvReceipts.Rows.Count; i++)
            {
                var cellVal = dgvReceipts.Rows[i].Cells["ReceiptNo"].Value?.ToString();
                if (string.Equals(cellVal, receiptNo, StringComparison.OrdinalIgnoreCase))
                {
                    dgvReceipts.ClearSelection();
                    dgvReceipts.Rows[i].Selected = true;
                    dgvReceipts.FirstDisplayedScrollingRowIndex = Math.Max(0, i - 2);
                    // Ensure preview updates
                    if (dgvReceipts.Rows[i].Tag is ReceiptData rd)
                    {
                        selectedReceipt = rd;
                        UpdateReceiptPreview();
                    }
                    return;
                }
            }

            // If not found, notify user
            MessageBox.Show($"Receipt {receiptNo} not found.", "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}