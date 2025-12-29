using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.LogicClass.Cashier;
using LendingApp.Models.CashierModels;


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

        // Layout constants
        private const int PagePadding = 16;
        private const int Gap = 12;

        // Root layout
        private Panel root;
        private TableLayoutPanel layout;

        // Header
        private Panel header;
        private Label lblTitle;

        // Search + customer
        private TextBox txtLoanNumber;
        private Button btnSearch;
        private Panel customerCard;
        private Label lblCustomerName;
        private Label lblCustomerBalance;

        // Payment details
        private TextBox txtPaymentAmount;
        private Label lblMonthlyDue;
        private RadioButton rbCash;
        private RadioButton rbGCash;
        private RadioButton rbBank;

        // Allocation + actions
        private Panel allocationCard;
        private Label lblAllocInterest;
        private Label lblAllocPrincipal;
        private Label lblAllocPenalty;
        private Label lblAllocNewBalance;

        private Button btnCalc;
        private Button btnProcess;
        private Button btnPrint;

        // Transactions
        private DataGridView gridTransactions;

        // Toast
        private Panel _toastPanel;
        private Label _toastLabel;
        private Timer _toastTimer;
        private BindingList<TransactionModels> _transactions;
        public CashierProcessPayment(BindingList<TransactionModels> transactions)
        {
            InitializeComponent();
            _transactions = transactions;

            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            //TopLevel = false;
            BuildUI();
            BindTransactions();
            RefreshState();

        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(PagePadding),
                BackColor = Color.Transparent
            };
            Controls.Add(root);

            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.Controls.Add(layout);

            header = MakeCard();
            header.Padding = new Padding(16, 14, 16, 14);
            header.BackColor = ColorTranslator.FromHtml("#ECFDF5");

            lblTitle = new Label
            {
                Text = "PAYMENT PROCESSING",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            header.Controls.Add(lblTitle);

            var searchCard = MakeCard();
            searchCard.Padding = new Padding(16);

            var searchGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2
            };
            searchGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            searchGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            searchCard.Controls.Add(searchGrid);

            // Left: loan input
            var left = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3
            };
            left.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));

            var lblLoan = MakeFieldLabel("Loan Number");
            txtLoanNumber = new TextBox { Dock = DockStyle.Fill };
            txtLoanNumber.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SearchLoan();
                }
            };

            btnSearch = MakePrimaryButton("Search", 90);
            btnSearch.Click += (s, e) => SearchLoan();

            left.Controls.Add(lblLoan, 0, 0);
            left.SetColumnSpan(lblLoan, 3);

            left.Controls.Add(txtLoanNumber, 1, 1);
            left.Controls.Add(btnSearch, 2, 1);

            var hint = new Label
            {
                Text = "Try: LN-2024-001, LN-2024-002, or LN-2024-003",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 8)
            };
            left.Controls.Add(hint, 1, 2);
            left.SetColumnSpan(hint, 2);

            // Right: customer card
            customerCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = false
            };
            lblCustomerName = new Label
            {
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            lblCustomerBalance = new Label
            {
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 8)
            };
            customerCard.Controls.Add(lblCustomerName);
            customerCard.Controls.Add(lblCustomerBalance);
            customerCard.Layout += (s, e) =>
            {
                lblCustomerName.Location = new Point(10, 10);
                lblCustomerBalance.Location = new Point(10, 32);
            };

            searchGrid.Controls.Add(left, 0, 0);
            searchGrid.Controls.Add(customerCard, 1, 0);

            var paymentCard = MakeCard();
            paymentCard.Padding = new Padding(16);

            var paymentGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2
            };
            paymentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            paymentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            paymentCard.Controls.Add(paymentGrid);

            // Amount section
            var amountPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            amountPanel.Controls.Add(MakeFieldLabel("Payment Amount"));
            txtPaymentAmount = new TextBox { Width = 240 };
            txtPaymentAmount.TextChanged += (s, e) =>
            {
                _allocation = null;
                UpdateAllocationPanel();
                UpdateButtons();
            };
            amountPanel.Controls.Add(txtPaymentAmount);

            lblMonthlyDue = new Label
            {
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 8)
            };
            amountPanel.Controls.Add(lblMonthlyDue);

            // Method section
            var methodPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            methodPanel.Controls.Add(MakeFieldLabel("Payment Method"));
            var methodRow = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            rbCash = MakeMethodRadio("Cash", true);
            rbGCash = MakeMethodRadio("GCash", false);
            rbBank = MakeMethodRadio("Bank", false);
            methodRow.Controls.Add(rbCash);
            methodRow.Controls.Add(rbGCash);
            methodRow.Controls.Add(rbBank);
            methodPanel.Controls.Add(methodRow);

            paymentGrid.Controls.Add(amountPanel, 0, 0);
            paymentGrid.Controls.Add(methodPanel, 1, 0);

            // Actions
            var actionsCard = MakeCard();
            actionsCard.Padding = new Padding(16);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnCalc = MakeOutlineButton("Calculate Allocation", 160);
            btnCalc.Click += (s, e) => CalculateAllocation();

            btnProcess = MakeSuccessButton("Process Payment", 140);
            btnProcess.Click += (s, e) => ProcessPayment();

            btnPrint = MakeOutlineButton("Print Receipt", 120);
            btnPrint.Click += (s, e) => PrintReceipt();

            actions.Controls.Add(btnCalc);
            actions.Controls.Add(btnProcess);
            actions.Controls.Add(btnPrint);
            actionsCard.Controls.Add(actions);

            // Allocation card (starts hidden)
            allocationCard = MakeCard();
            allocationCard.Padding = new Padding(16);
            allocationCard.Visible = false;

            var allocTitle = new Label
            {
                Text = "Payment Allocation",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };

            var allocGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(0, 10, 0, 0)
            };
            allocGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            allocGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            lblAllocInterest = MakeValueLabel();
            lblAllocPrincipal = MakeValueLabel();
            lblAllocPenalty = MakeValueLabel();
            lblAllocNewBalance = new Label
            {
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#16A34A"),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            AddAllocRow(allocGrid, "Interest:", lblAllocInterest, 0);
            AddAllocRow(allocGrid, "Principal:", lblAllocPrincipal, 1);
            AddAllocRow(allocGrid, "Penalty:", lblAllocPenalty, 2);

            var divider = new Panel
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#D1D5DB"),
                Margin = new Padding(0, 10, 0, 10)
            };

            var nbLeft = new Label
            {
                Text = "New Balance:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 4, 0, 4)
            };
            allocGrid.RowCount += 1;
            allocGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            allocGrid.Controls.Add(nbLeft, 0, 3);
            allocGrid.Controls.Add(lblAllocNewBalance, 1, 3);

            allocationCard.Controls.Add(allocGrid);
            allocationCard.Controls.Add(divider);
            allocationCard.Controls.Add(allocTitle);

            // Transactions card
            var txCard = MakeCard();
            txCard.Padding = new Padding(0);

            var txHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                Padding = new Padding(16, 12, 16, 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            var txTitle = new Label
            {
                Text = "RECENT TRANSACTIONS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            txHeader.Controls.Add(txTitle);

            // NEW: grid host with fixed height so the grid is visible inside AutoSize card
            var txBody = new Panel
            {
                Dock = DockStyle.Top,
                Height = 260,
                Padding = new Padding(0),
                BackColor = Color.White
            };

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
                BorderStyle = BorderStyle.None,

                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 28 }
            };

            gridTransactions.Columns.Clear();
            gridTransactions.Columns.Add("Time", "Time");
            gridTransactions.Columns.Add("Customer", "Customer");
            gridTransactions.Columns.Add("Amount", "Amount");
            gridTransactions.Columns.Add("Receipt", "Receipt #");

            var reprintCol = new DataGridViewButtonColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Text = "Reprint",
                UseColumnTextForButtonValue = true
            };
            gridTransactions.Columns.Add(reprintCol);

            gridTransactions.CellContentClick += GridTransactions_CellContentClick;

            txBody.Controls.Add(gridTransactions);
            txCard.Controls.Add(txBody);
            txCard.Controls.Add(txHeader);

            // Add to main layout (top to bottom)
            AddRow(header);
            AddRow(searchCard);
            AddRow(paymentCard);
            AddRow(actionsCard);
            AddRow(allocationCard);
            AddRow(txCard);

            BuildToast();

            allocationCard.Visible = false;
        }

        private void AddRow(Control c)
        {
            c.Margin = new Padding(0, 0, 0, Gap);
            layout.RowCount += 1;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(c, 0, layout.RowCount - 1);
        }

        private Panel MakeCard()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private static Label MakeFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Margin = new Padding(0, 0, 0, 6)
            };
        }

        private static RadioButton MakeMethodRadio(string text, bool isChecked)
        {
            return new RadioButton
            {
                Text = text,
                AutoSize = true,
                Checked = isChecked,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Margin = new Padding(0, 0, 12, 0)
            };
        }

        private static Button MakePrimaryButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 28,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            return b;
        }

        private static Button MakeOutlineButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 30,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            return b;
        }

        private static Button MakeSuccessButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 30,
                BackColor = ColorTranslator.FromHtml("#16A34A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#15803D");
            return b;
        }

        private static Label MakeValueLabel()
        {
            return new Label
            {
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
        }

        private static void AddAllocRow(TableLayoutPanel grid, string label, Label valueLabel, int row)
        {
            if (grid.RowCount <= row) grid.RowCount = row + 1;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var left = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Margin = new Padding(0, 4, 0, 4)
            };

            valueLabel.Margin = new Padding(0, 4, 0, 4);

            grid.Controls.Add(left, 0, row);
            grid.Controls.Add(valueLabel, 1, row);
        }

        private void BindTransactions()
        {   
            if (gridTransactions == null) return;

            gridTransactions.Rows.Clear();
            foreach (var t in _transactions)
            {
                int idx = gridTransactions.Rows.Add(t.Time, t.Customer, t.Amount, t.ReceiptNo, "Reprint");
                var row = gridTransactions.Rows[idx];
                row.Tag = t;

                var amtCell = row.Cells["Amount"] as DataGridViewTextBoxCell;
                if (amtCell != null) amtCell.Style.ForeColor = ColorTranslator.FromHtml("#16A34A");
            }
        }

        private void GridTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // NEW: only handle clicks on the Actions button column
            if (gridTransactions.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var tx = gridTransactions.Rows[e.RowIndex].Tag as TransactionModels;
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

            allocationCard.Visible = true;
            UpdateAllocationPanel();
            UpdateButtons();
        }

        private void ProcessPayment()
        {
            if (_loanDetails == null || _allocation == null)
            {
                ShowToast("Please complete all fields and calculate allocation", isError: true);
                return;
            }

            

            string receiptNo = "OR-" + (_transactions.Count + 1).ToString("000", CultureInfo.InvariantCulture);
            string time = DateTime.Now.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-US"));

            var tx = new TransactionModels
            {
                Time = time,
                Customer = _loanDetails.Customer,
                Amount = 2.222,
                ReceiptNo = receiptNo,
                LoanRef = (txtLoanNumber.Text ?? "").Trim()
            };

            _transactions.Insert(0, tx);
            BindTransactions();

            ShowToast("Payment processed! Receipt: " + receiptNo);

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
                customerCard.Visible = true;
                lblCustomerName.Text = _loanDetails.Customer;
                lblCustomerBalance.Text = "Balance: ₱" + _loanDetails.Balance.ToString("N0", CultureInfo.InvariantCulture);
                lblMonthlyDue.Text = "Monthly Due: ₱" + _loanDetails.MonthlyPayment.ToString("N2", CultureInfo.InvariantCulture);
            }
            else
            {
                customerCard.Visible = false;
                lblMonthlyDue.Text = "";
            }

            allocationCard.Visible = (_allocation != null);
            UpdateAllocationPanel();
            UpdateButtons();
        }

        private void UpdateAllocationPanel()
        {
            if (_allocation == null) return;

            lblAllocInterest.Text = "₱" + _allocation.Interest.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocPrincipal.Text = "₱" + _allocation.Principal.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocPenalty.Text = "₱" + _allocation.Penalty.ToString("N2", CultureInfo.InvariantCulture);
            lblAllocNewBalance.Text = "₱" + _allocation.NewBalance.ToString("N2", CultureInfo.InvariantCulture);
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

        private void BuildToast()
        {
            _toastPanel = new Panel
            {
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#111827"),
                Padding = new Padding(12, 8, 12, 8),
                Visible = false
            };
            _toastLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            _toastPanel.Controls.Add(_toastLabel);
            Controls.Add(_toastPanel);
            _toastPanel.BringToFront();

            _toastTimer = new Timer { Interval = 2200 };
            _toastTimer.Tick += (s, e) =>
            {
                _toastTimer.Stop();
                _toastPanel.Visible = false;
            };

            Resize += (s, e) => PositionToast();
            PositionToast();
        }

        private void PositionToast()
        {
            if (_toastPanel == null) return;
            _toastPanel.Left = ClientSize.Width - _toastPanel.Width - 12;
            _toastPanel.Top = 12;
        }

        private void ShowToast(string message, bool isError = false)
        {
            if (_toastPanel == null || _toastLabel == null) return;

            _toastPanel.BackColor = isError
                ? ColorTranslator.FromHtml("#991B1B")
                : ColorTranslator.FromHtml("#111827");

            _toastLabel.Text = message;
            _toastPanel.Visible = true;
            _toastPanel.BringToFront();
            _toastPanel.PerformLayout();
            PositionToast();

            _toastTimer.Stop();
            _toastTimer.Start();
        }
    }
}