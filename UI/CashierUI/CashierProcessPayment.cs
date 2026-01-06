using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.LogicClass.Cashier;
using LendingApp.Class.Models.CashierModels;
using LendingApp.Class.Service;
using LendingApp.Class;
using System.Data.Entity;
using LendingApp.Class.Models.Loans;
using LendingApp.Class.Repo;
using LendingApp.Class.Interface;
using LoanApplicationUI;

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

        private sealed class LoanRow
        {
            public string LoanNumber { get; set; }
            public string Customer { get; set; }
            public decimal Balance { get; set; }
            public decimal MonthlyDue { get; set; }
            public decimal InterestRate { get; set; }
            public int Term { get; set; }
            public int PaymentsDue { get; set; }
        }

        private readonly ILoanRepository _loanRepo;
        private readonly ICustomerRepository _customerRepo;

        private List<LoanRow> _loanRows = new List<LoanRow>();

        private LoanInfo _loanDetails;
        private AllocationInfo _allocation;

        // NEW: keep selected loan entity id for DB update
        private int? _selectedLoanId;

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

        // Loans list table
        private DataGridView gridLoans;

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
        private Button btnSettleLoan; // NEW: settle loan button

        // Transactions
        private DataGridView gridTransactions;

        // Toast
        private Panel _toastPanel;
        private Label _toastLabel;
        private Timer _toastTimer;
        private BindingList<TransactionModels> _transactions;
        private CashierProcessLogic cashierProcessLogic;

        public CashierProcessPayment(BindingList<TransactionModels> transactions)
            : this(transactions, new LoanRepository(), new CustomerRepository())
        {
        }

        public CashierProcessPayment(
            BindingList<TransactionModels> transactions,
            ILoanRepository loanRepo,
            ICustomerRepository customerRepo)
        {
            InitializeComponent();
            _transactions = transactions ?? new BindingList<TransactionModels>();
            cashierProcessLogic = new CashierProcessLogic();

            _loanRepo = loanRepo ?? new LoanRepository();
            _customerRepo = customerRepo ?? new CustomerRepository();

            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;

            BuildUI();
            LoadLoansFromDb();
            BindLoans();
            BindTransactions();
            RefreshState();
        }

        private void LoadLoansFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Show active loans for payment processing
                    var loans = db.Loans.AsNoTracking()
                        .Where(l => l.Status == "Active")
                        .OrderByDescending(l => l.CreatedDate)
                        .ToList();

                    var customerIds = loans.Select(l => l.CustomerId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

                    var customerNames = db.Customers.AsNoTracking()
                        .Where(c => customerIds.Contains(c.CustomerId))
                        .Select(c => new
                        {
                            c.CustomerId,
                            Name = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim()
                        })
                        .ToList()
                        .ToDictionary(x => x.CustomerId, x => x.Name, StringComparer.OrdinalIgnoreCase);

                    _loanRows = loans.Select(l =>
                    {
                        string name;
                        if (!customerNames.TryGetValue(l.CustomerId ?? "", out name))
                            name = l.CustomerId ?? "";

                        // PaymentsDue isn't in `loans`; keep simple / placeholder for now
                        int paymentsDue = l.DaysOverdue > 0 ? 1 : 0;

                        return new LoanRow
                        {
                            LoanNumber = l.LoanNumber,
                            Customer = name,
                            Balance = l.OutstandingBalance,
                            MonthlyDue = l.MonthlyPayment,
                            InterestRate = l.InterestRate,
                            Term = l.TermMonths,
                            PaymentsDue = paymentsDue
                        };
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _loanRows = new List<LoanRow>();
                ShowToast("Failed to load loans from database: " + ex.Message, isError: true);
            }
        }

        // ==========================
        // UI BUILD (unchanged)
        // ==========================
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

            // Left: loan input (optional)
            var left = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3
            };
            left.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));

            var lblLoan = MakeFieldLabel("Loan Number (optional — select from table below)");
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
                Text = "Tip: double-click a row in the Loans table to load the loan automatically.",
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

            // Loans table card
            var loansCard = MakeCard();
            loansCard.Padding = new Padding(0);

            var loansHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = ColorTranslator.FromHtml("#F3E8FF"),
                Padding = new Padding(16, 12, 16, 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            var loansTitle = new Label
            {
                Text = "ALL LOANS (DOUBLE-CLICK TO SELECT)",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            loansHeader.Controls.Add(loansTitle);

            var loansBody = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = Color.White
            };

            gridLoans = new DataGridView
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

            gridLoans.Columns.Clear();
            gridLoans.Columns.Add("LoanNumber", "Loan #");
            gridLoans.Columns.Add("Customer", "Customer");
            gridLoans.Columns.Add("Balance", "Balance");
            gridLoans.Columns.Add("MonthlyDue", "Monthly Due");
            gridLoans.Columns.Add("PaymentsDue", "Payments Due");
            gridLoans.Columns.Add("Rate", "Rate (%)");
            gridLoans.Columns.Add("Term", "Term (mo)");

            gridLoans.CellDoubleClick += GridLoans_CellDoubleClick;
            gridLoans.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SelectLoanFromGrid();
                }
            };

            loansBody.Controls.Add(gridLoans);
            loansCard.Controls.Add(loansBody);
            loansCard.Controls.Add(loansHeader);

            // Payment card
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

            btnSettleLoan = MakeOutlineButton("Settle Loan", 120);
            btnSettleLoan.Click += (s, e) => OpenSettleLoanDialog();

            actions.Controls.Add(btnCalc);
            actions.Controls.Add(btnProcess);
            actions.Controls.Add(btnPrint);
            actions.Controls.Add(btnSettleLoan);
            actionsCard.Controls.Add(actions);

            // Allocation card
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

            var txBody = new Panel
            {
                Dock = DockStyle.Top,
                Height = 260,
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
            gridTransactions.Columns.Add("Borrower", "Borrower");
            gridTransactions.Columns.Add("PaidAmount", "PaidAmount");
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
            AddRow(loansCard);
            AddRow(paymentCard);
            AddRow(actionsCard);
            AddRow(allocationCard);
            AddRow(txCard);

            BuildToast();
            allocationCard.Visible = false;
        }

        // ==========================
        // MISSING HELPERS (FIXES YOUR CS0103 ERRORS)
        // ==========================
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

        private void GridTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (gridTransactions.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var tx = gridTransactions.Rows[e.RowIndex].Tag as TransactionModels;
                if (tx == null) return;

                ShowToast("Receipt " + tx.ReceiptNo + " sent to printer");
            }
        }

        private void BindTransactions()
        {
            if (gridTransactions == null) return;

            gridTransactions.Rows.Clear();
            foreach (var t in _transactions)
            {
                int idx = gridTransactions.Rows.Add(t.Time, t.Borrower, t.PaidAmount, t.ReceiptNo, "Reprint");
                var row = gridTransactions.Rows[idx];
                row.Tag = t;

                var amtCell = row.Cells["PaidAmount"] as DataGridViewTextBoxCell;
                if (amtCell != null) amtCell.Style.ForeColor = ColorTranslator.FromHtml("#16A34A");
            }
        }

        // ==========================
        // LOANS GRID + SEARCH
        // ==========================
        private void BindLoans()
        {
            if (gridLoans == null) return;

            gridLoans.Rows.Clear();

            foreach (var r in _loanRows)
            {
                int idx = gridLoans.Rows.Add(
                    r.LoanNumber,
                    r.Customer,
                    "₱" + r.Balance.ToString("N2", CultureInfo.GetCultureInfo("en-US")),
                    "₱" + r.MonthlyDue.ToString("N2", CultureInfo.GetCultureInfo("en-US")),
                    r.PaymentsDue.ToString(CultureInfo.InvariantCulture),
                    r.InterestRate.ToString("N2", CultureInfo.GetCultureInfo("en-US")),
                    r.Term.ToString(CultureInfo.InvariantCulture));

                // keep easy selection by loan number
                gridLoans.Rows[idx].Tag = r.LoanNumber;
            }
        }

        private void GridLoans_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SelectLoanFromGrid();
        }

        private void SelectLoanFromGrid()
        {
            if (gridLoans == null || gridLoans.CurrentRow == null) return;

            var loanNo = gridLoans.CurrentRow.Tag as string;
            if (string.IsNullOrWhiteSpace(loanNo)) return;

            txtLoanNumber.Text = loanNo;
            SearchLoan();
        }

        private void SearchLoan()
        {
            var loanNumber = (txtLoanNumber.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(loanNumber))
            {
                ShowToast("Please enter a loan number", isError: true);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var loan = db.Loans.AsNoTracking().FirstOrDefault(l => l.LoanNumber == loanNumber);
                    if (loan == null)
                    {
                        _selectedLoanId = null;
                        _loanDetails = null;
                        _allocation = null;
                        ShowToast("Loan not found", isError: true);
                        RefreshState();
                        return;
                    }

                    _selectedLoanId = loan.LoanId;

                    var cust = db.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == loan.CustomerId);
                    var custName = cust != null
                        ? ((cust.FirstName ?? "") + " " + (cust.LastName ?? "")).Trim()
                        : (loan.CustomerId ?? "");

                    int paymentsDue = loan.DaysOverdue > 0 ? 1 : 0;

                    _loanDetails = new LoanInfo
                    {
                        Customer = custName,
                        Balance = loan.OutstandingBalance,
                        MonthlyPayment = loan.MonthlyPayment,
                        Principal = loan.PrincipalAmount,
                        InterestRate = loan.InterestRate,
                        Term = loan.TermMonths,
                        PaymentsDue = paymentsDue
                    };

                    txtPaymentAmount.Text = loan.MonthlyPayment.ToString("0.00", CultureInfo.InvariantCulture);
                    _allocation = null;

                    ShowToast("Loan found!");
                    RefreshState();
                }
            }
            catch (Exception ex)
            {
                _selectedLoanId = null;
                _loanDetails = null;
                _allocation = null;
                ShowToast("Failed to search loan: " + ex.Message, isError: true);
                RefreshState();
            }
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

        // ==========================
        // UPDATED: ProcessPayment now updates the loan record in DB
        // ==========================
        private void ProcessPayment()
        {
            if (_loanDetails == null || _allocation == null || !_selectedLoanId.HasValue)
            {
                ShowToast("Please select a loan and calculate allocation", isError: true);
                return;
            }

            decimal paid;
            if (!TryParseAmount(txtPaymentAmount.Text, out paid) || paid <= 0)
            {
                ShowToast("Enter a valid payment amount", isError: true);
                return;
            }

            try
            {
                string receiptNo = "OR-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
                string timeNow = cashierProcessLogic.GetTimeNow();

                using (var db = new AppDbContext())
                {
                    // Load loan (tracked)
                    var loan = db.Loans.FirstOrDefault(l => l.LoanId == _selectedLoanId.Value);
                    if (loan == null)
                    {
                        ShowToast("Loan record not found (refresh and try again).", isError: true);
                        return;
                    }

                    // ===== 1) UPDATE LOAN TOTALS =====
                    loan.TotalPaid = RoundMoney(loan.TotalPaid + _allocation.Principal);
                    loan.TotalInterestPaid = RoundMoney(loan.TotalInterestPaid + _allocation.Interest);
                    loan.TotalPenaltyPaid = RoundMoney(loan.TotalPenaltyPaid + _allocation.Penalty);

                    loan.OutstandingBalance = RoundMoney(Math.Max(0m, loan.OutstandingBalance - _allocation.Principal));
                    loan.LastPaymentDate = DateTime.Now;
                    loan.LastUpdated = DateTime.Now;

                    if (loan.OutstandingBalance <= 0m)
                    {
                        loan.Status = "Paid";
                        loan.NextDueDate = null;
                    }

                    // ===== 2) INSERT COLLECTION RECORD =====
                    // We log one row per payment (simple + safe).
                    // If you want "update existing due row instead", tell me your desired rule.
                    var dueDate = (loan.NextDueDate ?? loan.FirstDueDate).Date;

                    var collection = new CollectionEntity
                    {
                        LoanId = loan.LoanId,
                        CustomerId = loan.CustomerId,

                        DueDate = dueDate,
                        AmountDue = RoundMoney(paid),

                        DaysOverdue = Math.Max(0, loan.DaysOverdue),
                        Priority = "Medium",
                        Status = "Paid",

                        LastContactDate = DateTime.Now.Date,
                        LastContactMethod = GetSelectedPaymentMethodText(),
                        Notes = "Payment received. Receipt: " + receiptNo,

                        PromiseDate = null,
                        AssignedOfficerId = null,

                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    db.Collections.Add(collection);

                    // NEW: INSERT PAYMENT RECORD
                    var payment = new PaymentEntity
                    {
                        LoanId = loan.LoanId,
                        CustomerId = loan.CustomerId,

                        PaymentDate = DateTime.Now,

                        AmountPaid = RoundMoney(paid),
                        PrincipalPaid = RoundMoney(_allocation.Principal),
                        InterestPaid = RoundMoney(_allocation.Interest),
                        PenaltyPaid = RoundMoney(_allocation.Penalty),

                        PaymentMethod = GetSelectedPaymentMethodText(),
                        ReceiptNo = receiptNo,

                        // safe; mapped/ignored depending on your context config
                        CreatedDate = DateTime.Now
                    };

                    db.Payments.Add(payment);

                    // Commit both loan update + collection insert together
                    db.SaveChanges();

                    // Sync UI in-memory snapshot
                    _loanDetails.Balance = loan.OutstandingBalance;
                }

                // ===== 3) UI transactions list (unchanged) =====
                var tx = new TransactionModels
                {
                    Time = timeNow,
                    Borrower = _loanDetails.Customer,
                    PaidAmount = paid,
                    ReceiptNo = receiptNo,
                    LoanRef = (txtLoanNumber.Text ?? "").Trim()
                };

                _transactions.Insert(0, tx);
                BindTransactions();

                LoadLoansFromDb();
                BindLoans();

                ShowToast("Payment processed! Receipt: " + receiptNo);

                txtLoanNumber.Text = "";
                txtPaymentAmount.Text = "";
                _selectedLoanId = null;
                _loanDetails = null;
                _allocation = null;

                RefreshState();
            }
            catch (Exception ex)
            {
                ShowToast("Failed to process payment: " + ex.ToString(), isError: true);
            }
        }

        // NEW: helper to store method in `collections.last_contact_method`
        private string GetSelectedPaymentMethodText()
        {
            if (rbGCash != null && rbGCash.Checked) return "GCash";
            if (rbBank != null && rbBank.Checked) return "Bank";
            return "Cash";
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
            btnSettleLoan.Enabled = (_loanDetails != null);
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

        // ==========================
        // TOAST (missing in your current file)
        // ==========================
        private void BuildToast()
        {
            if (_toastPanel != null) return;

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
            // Safety: if some path calls ShowToast before BuildToast()
            if (_toastPanel == null || _toastLabel == null) BuildToast();

            _toastPanel.BackColor = isError
                ? ColorTranslator.FromHtml("#991B1B")
                : ColorTranslator.FromHtml("#111827");

            _toastLabel.Text = message ?? "";
            _toastPanel.Visible = true;
            _toastPanel.BringToFront();
            _toastPanel.PerformLayout();
            PositionToast();

            _toastTimer.Stop();
            _toastTimer.Start();
        }

        private void OpenSettleLoanDialog()
        {
            if (_loanDetails == null)
            {
                ShowToast("Please select a loan first", isError: true);
                return;
            }

            // Use the currently selected loan details to prefill the settlement form.
            // (Values like OriginalBalance/PaymentsRemaining are not available in this form,
            // so we provide safe defaults for now.)
            using (var frm = new CashierLoanSettlementForm())
            {
                frm.SettlementData = new SettlementData
                {
                    LoanNumber = (txtLoanNumber.Text ?? "").Trim(),
                    Customer = _loanDetails.Customer,
                    OriginalBalance = _loanDetails.Principal,     // best available proxy here
                    CurrentBalance = _loanDetails.Balance,
                    PaymentsRemaining = Math.Max(0, _loanDetails.PaymentsDue),
                    IsFinalPayment = (_loanDetails.Balance <= 0m)
                };

                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
        }
    }
}