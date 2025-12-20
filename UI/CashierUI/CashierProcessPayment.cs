using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierProcessPayment : Form
    {
        private sealed class LoanInfo
        {
            public string Customer { get; set; }
            public decimal Balance { get; set; }
            public decimal MonthlyPayment { get; set; }
            public decimal Principal { get; set; }
            public decimal InterestRate { get; set; } // annual percent
            public int Term { get; set; }
            public int PaymentsDue { get; set; }
        }

        private sealed class AllocationInfo
        {
            public decimal Interest { get; set; }
            public decimal Principal { get; set; }
            public decimal Penalty { get; set; }
            public decimal NewBalance { get; set; }
        }

        private sealed class TransactionRow
        {
            public string Time { get; set; }
            public string Customer { get; set; }
            public string Amount { get; set; }
            public string ReceiptNo { get; set; }
            public string LoanRef { get; set; }
        }

        private readonly Dictionary<string, LoanInfo> _mockLoans = new Dictionary<string, LoanInfo>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "LN-2024-001",
                new LoanInfo
                {
                    Customer = "Juan Dela Cruz",
                    Balance = 35657m,
                    MonthlyPayment = 4442.44m,
                    Principal = 50000m,
                    InterestRate = 12m,
                    Term = 12,
                    PaymentsDue = 8
                }
            },
            {
                "LN-2024-002",
                new LoanInfo
                {
                    Customer = "Maria Santos",
                    Balance = 12500m,
                    MonthlyPayment = 2150m,
                    Principal = 15000m,
                    InterestRate = 10m,
                    Term = 6,
                    PaymentsDue = 6
                }
            },
            {
                "LN-2024-003",
                new LoanInfo
                {
                    Customer = "Pedro Reyes",
                    Balance = 8200m,
                    MonthlyPayment = 1500m,
                    Principal = 10000m,
                    InterestRate = 8m,
                    Term = 12,
                    PaymentsDue = 6
                }
            }
        };

        private LoanInfo _loanDetails;
        private AllocationInfo _allocation;

        // UI
        private Panel root;
        private Panel mainCard;
        private Panel mainHeader;
        private Label lblMainTitle;

        private TextBox txtLoanNumber;
        private Button btnSearch;

        private Panel pnlCustomerInfo;
        private Label lblCustomerName;
        private Label lblCustomerBalance;

        private TextBox txtPaymentAmount;
        private Label lblMonthlyDue;

        private RadioButton rbCash;
        private RadioButton rbGCash;
        private RadioButton rbBank;

        private Button btnCalc;
        private Panel pnlAllocation;
        private Label lblAllocInterest;
        private Label lblAllocPrincipal;
        private Label lblAllocPenalty;
        private Label lblAllocNewBalance;

        private Button btnProcess;
        private Button btnPrint;

        private Panel transactionsCard;
        private Panel transactionsHeader;
        private Label lblTransactionsTitle;
        private DataGridView gridTransactions;

        private readonly List<TransactionRow> _transactions = new List<TransactionRow>();

        public CashierProcessPayment()
        {
            InitializeComponent();

            // IMPORTANT when hosted inside `CashierDashboard`:
            // - Do not maximize (it would try to maximize inside the MDI/parent)
            // - Instead, let the parent dock+size us.
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            BuildUI();
            SeedTransactions();
            BindTransactions();
            RefreshState();

            // Ensure initial layout header positions run once
            // (some child panels are using Resize to position inner labels)
            PerformLayout();
        }

        private void BuildUI()
        {
            Text = "Cashier - Payment Processing";

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(16)
            };

            Controls.Clear();
            Controls.Add(root);

            // ===== Main card =====
            mainCard = new Panel { BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            mainHeader = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = ColorTranslator.FromHtml("#ECFDF5"), BorderStyle = BorderStyle.FixedSingle };
            lblMainTitle = new Label
            {
                Text = "PAYMENT PROCESSING",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 16)
            };
            mainHeader.Controls.Add(lblMainTitle);
            mainCard.Controls.Add(mainHeader);

            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(90) };
            mainCard.Controls.Add(body);

            // Loan search
            var lblLoan = new Label { Text = "Loan Number", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            txtLoanNumber = new TextBox { Width = 240 };
            txtLoanNumber.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SearchLoan();
                }
            };

            btnSearch = new Button
            {
                Text = "Search",
                Width = 90,
                Height = 26,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            btnSearch.Click += (s, e) => SearchLoan();

            var hint = new Label
            {
                Text = "Try: LN-2024-001, LN-2024-002, or LN-2024-003",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 8, FontStyle.Regular)
            };

            // Customer info panel
            pnlCustomerInfo = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Padding = new Padding(10),
                Height = 70
            };
            lblCustomerName = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827"), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            lblCustomerBalance = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151"), Font = new Font("Segoe UI", 8, FontStyle.Regular) };
            pnlCustomerInfo.Controls.Add(lblCustomerName);
            pnlCustomerInfo.Controls.Add(lblCustomerBalance);
            pnlCustomerInfo.Resize += (s, e) =>
            {
                lblCustomerName.Location = new Point(10, 10);
                lblCustomerBalance.Location = new Point(10, 32);
            };

            // Payment details
            var lblAmount = new Label { Text = "Payment Amount", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            txtPaymentAmount = new TextBox { Width = 240 };
            txtPaymentAmount.TextChanged += (s, e) =>
            {
                _allocation = null;
                UpdateAllocationPanel();
                UpdateButtons();
            };

            lblMonthlyDue = new Label
            {
                Text = "",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 8, FontStyle.Regular)
            };

            var lblMethod = new Label { Text = "Payment Method", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            rbCash = MakeMethodRadio("Cash", true);
            rbGCash = MakeMethodRadio("GCash", false);
            rbBank = MakeMethodRadio("Bank", false);

            btnCalc = new Button
            {
                Text = "Calculate Allocation",
                Width = 160,
                Height = 28,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#1D4ED8"),
                FlatStyle = FlatStyle.Flat
            };
            btnCalc.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#93C5FD");
            btnCalc.Click += (s, e) => CalculateAllocation();

            // Allocation panel
            pnlAllocation = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Padding = new Padding(16),
                Height = 170
            };

            var lblAllocTitle = new Label
            {
                Text = "Payment Allocation",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 14)
            };
            lblAllocInterest = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            lblAllocPrincipal = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            lblAllocPenalty = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            lblAllocNewBalance = new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#16A34A"), Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            pnlAllocation.Controls.Add(lblAllocTitle);
            pnlAllocation.Controls.Add(lblAllocInterest);
            pnlAllocation.Controls.Add(lblAllocPrincipal);
            pnlAllocation.Controls.Add(lblAllocPenalty);
            pnlAllocation.Controls.Add(lblAllocNewBalance);

            pnlAllocation.Resize += (s, e) =>
            {
                int y = 44;
                lblAllocInterest.Location = new Point(16, y); y += 24;
                lblAllocPrincipal.Location = new Point(16, y); y += 24;
                lblAllocPenalty.Location = new Point(16, y); y += 30;
                lblAllocNewBalance.Location = new Point(16, y);
            };

            // Action buttons
            btnProcess = new Button
            {
                Text = "Process Payment",
                Width = 140,
                Height = 30,
                BackColor = ColorTranslator.FromHtml("#16A34A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnProcess.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#15803D");
            btnProcess.Click += (s, e) => ProcessPayment();

            btnPrint = new Button
            {
                Text = "Print Receipt",
                Width = 120,
                Height = 30,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnPrint.Click += (s, e) => PrintReceipt();

            // Empty state
            var emptyState = new Label
            {
                Text = "Enter a loan number to begin processing payment",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            var emptyPanel = new Panel { Dock = DockStyle.Top, Height = 90, Margin = new Padding(0, 12, 0, 0) };
            emptyPanel.Controls.Add(emptyState);
            emptyPanel.Resize += (s, e) =>
            {
                emptyState.Left = (emptyPanel.Width - emptyState.Width) / 2;
                emptyState.Top = (emptyPanel.Height - emptyState.Height) / 2;
            };

            // Layout with TableLayoutPanel
            var tlp = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

            var left = new Panel { Dock = DockStyle.Fill, AutoSize = true };
            var right = new Panel { Dock = DockStyle.Fill, AutoSize = true };

            left.Controls.Add(lblLoan);
            lblLoan.Location = new Point(0, 0);

            left.Controls.Add(txtLoanNumber);
            left.Controls.Add(btnSearch);
            txtLoanNumber.Location = new Point(0, 22);
            btnSearch.Location = new Point(txtLoanNumber.Right + 8, 21);

            left.Controls.Add(hint);
            hint.Location = new Point(0, 52);

            right.Controls.Add(pnlCustomerInfo);
            pnlCustomerInfo.Dock = DockStyle.Top;

            tlp.Controls.Add(left, 0, 0);
            tlp.Controls.Add(right, 1, 0);

            // Payment details row
            var tlp2 = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 14, 0, 0) };
            tlp2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            tlp2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

            var pAmount = new Panel { Dock = DockStyle.Fill, AutoSize = true };
            pAmount.Controls.Add(lblAmount);
            lblAmount.Location = new Point(0, 0);
            pAmount.Controls.Add(txtPaymentAmount);
            txtPaymentAmount.Location = new Point(0, 22);
            pAmount.Controls.Add(lblMonthlyDue);
            lblMonthlyDue.Location = new Point(0, 52);

            var pMethod = new Panel { Dock = DockStyle.Fill, AutoSize = true };
            pMethod.Controls.Add(lblMethod);
            lblMethod.Location = new Point(0, 0);
            pMethod.Controls.Add(rbCash);
            pMethod.Controls.Add(rbGCash);
            pMethod.Controls.Add(rbBank);
            rbCash.Location = new Point(0, 22);
            rbGCash.Location = new Point(80, 22);
            rbBank.Location = new Point(160, 22);

            tlp2.Controls.Add(pAmount, 0, 0);
            tlp2.Controls.Add(pMethod, 1, 0);

            // Buttons row
            var actionsRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 12, 0, 0)
            };
            actionsRow.Controls.Add(btnCalc);
            actionsRow.Controls.Add(btnProcess);
            actionsRow.Controls.Add(btnPrint);
            btnCalc.Margin = new Padding(0, 0, 10, 0);
            btnProcess.Margin = new Padding(10, 0, 10, 0);

            body.Controls.Add(emptyPanel);
            body.Controls.Add(actionsRow);
            body.Controls.Add(pnlAllocation);
            body.Controls.Add(tlp2);
            body.Controls.Add(tlp);

            // Initial visibility state (FIX: ensure correct state without needing a resize/visible change)
            emptyPanel.Visible = (_loanDetails == null);
            tlp2.Visible = (_loanDetails != null);
            actionsRow.Visible = (_loanDetails != null);
            pnlAllocation.Visible = (_allocation != null);

            // Visibility controller
            EventHandler refreshVisibility = (s, e) =>
            {
                emptyPanel.Visible = (_loanDetails == null);
                tlp2.Visible = (_loanDetails != null);
                actionsRow.Visible = (_loanDetails != null);
                pnlAllocation.Visible = (_allocation != null);
            };
            body.VisibleChanged += refreshVisibility;
            body.Resize += refreshVisibility;

            // ===== Transactions card =====
            transactionsCard = new Panel { BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            transactionsHeader = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = ColorTranslator.FromHtml("#DBEAFE"), BorderStyle = BorderStyle.FixedSingle };
            lblTransactionsTitle = new Label
            {
                Text = "RECENT TRANSACTIONS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 14)
            };
            transactionsHeader.Controls.Add(lblTransactionsTitle);
            transactionsCard.Controls.Add(transactionsHeader);

            gridTransactions = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,

                // FIX: prevents the first visible row from being hidden under the header area
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                ColumnHeadersHeight = 36,

                // FIX: adds a bit more breathing room so the first row is fully visible
                RowTemplate = { Height = 28 }
            };

            gridTransactions.Columns.Clear();
            gridTransactions.Columns.Add("Time", "Time");
            gridTransactions.Columns.Add("Customer", "Customer");
            gridTransactions.Columns.Add("Amount", "Amount");
            gridTransactions.Columns.Add("Receipt", "Receipt #");

            var reprintCol = new DataGridViewButtonColumn
            {
                HeaderText = "Actions",
                Text = "Reprint",
                UseColumnTextForButtonValue = true
            };
            gridTransactions.Columns.Add(reprintCol);
            gridTransactions.CellContentClick += GridTransactions_CellContentClick;

            // FIX: ensure the DataGridView is not flush against the header border
            var gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 2, 0, 0) // tiny top padding so first row isn't visually covered
            };
            gridHost.Controls.Add(gridTransactions);

            transactionsCard.Controls.Add(gridHost);

            // Place on root (order matters: last added appears on top visually)
            root.Controls.Add(transactionsCard);
            root.Controls.Add(mainCard);

            root.Resize += (s, e) => LayoutCards();
            LayoutCards();
        }

        private RadioButton MakeMethodRadio(string text, bool isChecked)
        {
            return new RadioButton
            {
                Text = text,
                AutoSize = true,
                Checked = isChecked,
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
        }

        private void LayoutCards()
        {
            int pad = root.Padding.Left;
            int gap = 16;

            int availableW = root.ClientSize.Width - (pad * 2);
            if (availableW < 400) availableW = 400;

            int x = pad;
            int y = pad;

            // Let the cards size naturally based on host; avoid forcing giant heights.
            mainCard.SetBounds(x, y, availableW, 420);
            y += mainCard.Height + gap;

            int remainingH = Math.Max(260, root.ClientSize.Height - y - pad);
            transactionsCard.SetBounds(x, y, availableW, remainingH);
        }

        private void SeedTransactions()
        {
            _transactions.Clear();
            _transactions.Add(new TransactionRow { Time = "9:30 AM", Customer = "Maria Santos", Amount = "₱2,150", ReceiptNo = "OR-001", LoanRef = "LN-2024-001" });
            _transactions.Add(new TransactionRow { Time = "10:15 AM", Customer = "Juan Dela Cruz", Amount = "₱4,442", ReceiptNo = "OR-002", LoanRef = "LN-2024-002" });
            _transactions.Add(new TransactionRow { Time = "11:00 AM", Customer = "Pedro Reyes", Amount = "₱1,500", ReceiptNo = "OR-003", LoanRef = "LN-2024-003" });
        }

        private void BindTransactions()
        {
            gridTransactions.Rows.Clear();

            foreach (var t in _transactions)
            {
                int idx = gridTransactions.Rows.Add(t.Time, t.Customer, t.Amount, t.ReceiptNo, "Reprint");
                var row = gridTransactions.Rows[idx];
                row.Tag = t;

                var amtCell = row.Cells["Amount"] as DataGridViewTextBoxCell;
                if (amtCell != null) amtCell.Style.ForeColor = ColorTranslator.FromHtml("#16A34A");

                var receiptCell = row.Cells["Receipt"] as DataGridViewTextBoxCell;
                if (receiptCell != null) receiptCell.Style.ForeColor = ColorTranslator.FromHtml("#374151");

                var btnCell = row.Cells[gridTransactions.Columns.Count - 1] as DataGridViewButtonCell;
                if (btnCell != null) btnCell.Value = "Reprint";
            }
        }

        private void GridTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridTransactions.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var tx = gridTransactions.Rows[e.RowIndex].Tag as TransactionRow;
                if (tx == null) return;
                ShowToast("Receipt " + tx.ReceiptNo + " sent to printer");
            }
        }

        private void SearchLoan()
        {
            var loanNumber = (txtLoanNumber.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(loanNumber))
            {
                ShowToast("Please enter a loan number", isError: true);
                return;
            }

            LoanInfo loan;
            if (_mockLoans.TryGetValue(loanNumber, out loan))
            {
                _loanDetails = loan;
                txtPaymentAmount.Text = loan.MonthlyPayment.ToString("0.00", CultureInfo.InvariantCulture);
                _allocation = null;
                ShowToast("Loan found!");
            }
            else
            {
                _loanDetails = null;
                _allocation = null;
                ShowToast("Loan not found", isError: true);
            }

            RefreshState();
        }

        private void CalculateAllocation()
        {
            if (_loanDetails == null) return;

            decimal amount;
            if (!TryParseAmount(txtPaymentAmount.Text, out amount) || amount <= 0)
            {
                ShowToast("Enter a valid payment amount", isError: true);
                return;
            }

            var balance = _loanDetails.Balance;
            var monthlyInterest = (balance * _loanDetails.InterestRate) / 100m / 12m;

            var interest = Math.Min(monthlyInterest, amount);
            var principal = Math.Max(0m, amount - interest);
            var penalty = 0m;
            var newBalance = balance - principal;

            _allocation = new AllocationInfo
            {
                Interest = RoundMoney(interest),
                Principal = RoundMoney(principal),
                Penalty = RoundMoney(penalty),
                NewBalance = RoundMoney(newBalance)
            };

            RefreshState();
        }

        private void ProcessPayment()
        {
            if (_loanDetails == null || _allocation == null)
            {
                ShowToast("Please complete all fields and calculate allocation", isError: true);
                return;
            }

            decimal amount;
            if (!TryParseAmount(txtPaymentAmount.Text, out amount) || amount <= 0)
            {
                ShowToast("Enter a valid payment amount", isError: true);
                return;
            }

            string receiptNo = "OR-" + (_transactions.Count + 1).ToString("000", CultureInfo.InvariantCulture);
            string time = DateTime.Now.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-US"));

            var tx = new TransactionRow
            {
                Time = time,
                Customer = _loanDetails.Customer,
                Amount = "₱" + amount.ToString("N0", CultureInfo.InvariantCulture),
                ReceiptNo = receiptNo,
                LoanRef = (txtLoanNumber.Text ?? "").Trim()
            };

            _transactions.Insert(0, tx);
            BindTransactions();

            ShowToast("Payment processed! Receipt: " + receiptNo);

            // Reset
            txtLoanNumber.Text = "";
            txtPaymentAmount.Text = "";
            _loanDetails = null;
            _allocation = null;

            RefreshState();
        }

        private void PrintReceipt()
        {
            if (_loanDetails == null || _allocation == null)
            {
                ShowToast("No payment to print", isError: true);
                return;
            }

            ShowToast("Receipt sent to printer");
        }

        private void RefreshState()
        {
            if (_loanDetails != null)
            {
                pnlCustomerInfo.Visible = true;
                lblCustomerName.Text = _loanDetails.Customer;
                lblCustomerBalance.Text = "Balance: ₱" + _loanDetails.Balance.ToString("N0", CultureInfo.InvariantCulture);
                lblMonthlyDue.Text = "Monthly Due: ₱" + _loanDetails.MonthlyPayment.ToString("N2", CultureInfo.InvariantCulture);
            }
            else
            {
                pnlCustomerInfo.Visible = false;
                lblMonthlyDue.Text = "";
            }

            UpdateAllocationPanel();
            UpdateButtons();
        }

        private void UpdateAllocationPanel()
        {
            pnlAllocation.Visible = (_allocation != null);

            if (_allocation == null)
                return;

            lblAllocInterest.Text = "Interest: ₱" + _allocation.Interest.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocPrincipal.Text = "Principal: ₱" + _allocation.Principal.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocPenalty.Text = "Penalty: ₱" + _allocation.Penalty.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocNewBalance.Text = "New Balance: ₱" + _allocation.NewBalance.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void UpdateButtons()
        {
            btnCalc.Enabled = (_loanDetails != null) && !string.IsNullOrWhiteSpace(txtPaymentAmount.Text);

            bool canAct = (_allocation != null);
            btnProcess.Enabled = canAct;
            btnPrint.Enabled = canAct;
        }

        private static bool TryParseAmount(string text, out decimal amount)
        {
            var cleaned = (text ?? "").Trim().Replace("₱", "").Replace(",", "");
            return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount);
        }

        private static decimal RoundMoney(decimal v)
        {
            return Math.Round(v, 2, MidpointRounding.AwayFromZero);
        }

        private void ShowToast(string message, bool isError = false)
        {
            MessageBox.Show(
                message,
                isError ? "Error" : "Info",
                MessageBoxButtons.OK,
                isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information
            );
        }
    }
}