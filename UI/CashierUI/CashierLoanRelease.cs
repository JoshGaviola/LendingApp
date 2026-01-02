using LendingApp.Models.CashierModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierLoanRelease : Form
    {

        private LoanReleaseModels _selected;

        // layout
        private Panel root;
        private TextBox txtLoanNumber;
        private Button btnSearch;

        private Panel pnlDetails;
        private Label lblBorrower;
        private Label lblLoanType;
        private Label lblAmount;
        private Label lblApprovedDate;

        private DateTimePicker dtpReleaseDate;
        private ComboBox cmbReleaseMode;

        private Label lblNet;
        private Label lblMonthly;
        private Label lblFirstPayment;
        private Label lblTotal;

        private CheckBox chkIds;
        private CheckBox chkIncome;
        private CheckBox chkContract;
        private CheckBox chkCoMaker;

        private Button btnConfirm;
        private Button btnReleasePrint;
        private Button btnCancel;
        private Button btnPrintVoucher;
        private Button btnViewContract;

        private DataGridView gridToday;

        // toast
        private Panel _toastPanel;
        private Label _toastLabel;
        private Timer _toastTimer;

        private BindingList<LoanReleaseModels> _pendingLoans;

        public CashierLoanRelease(BindingList<LoanReleaseModels> pending)
        {
            InitializeComponent();
            _pendingLoans = pending;

            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            BuildUI();
            BindToday();
            RefreshUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(16), BackColor = Color.Transparent };
            Controls.Add(root);

            // ===== Search card =====
            var searchCard = Card("LOAN RELEASE - CASHIER", "#EFF6FF");
            var searchBody = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(16), BackColor = Color.White };
            searchCard.Controls.Add(searchBody);

            var lbl = new Label { Text = "Pending Releases", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151"), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            searchBody.Controls.Add(lbl);

            var row = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 10, 0, 0) };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110f));

            txtLoanNumber = new TextBox { Dock = DockStyle.Fill };
            txtLoanNumber.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    Search();
                }
            };

            btnSearch = new Button
            {
                Text = "Search",
                Dock = DockStyle.Fill,
                Height = 28,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            btnSearch.Click += (s, e) => Search();

            row.Controls.Add(txtLoanNumber, 0, 0);
            row.Controls.Add(btnSearch, 1, 0);
            searchBody.Controls.Add(row);

            var hint = new Label
            {
                Text = "Try: LN-APP-2024-014, LN-APP-2024-015, LN-APP-2024-016",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 8),
                Margin = new Padding(0, 8, 0, 0)
            };
            searchBody.Controls.Add(hint);

            // ===== Details panel (hidden until selected) =====
            pnlDetails = new Panel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 12, 0, 0), Visible = false };
            searchBody.Controls.Add(pnlDetails);

            // info
            var info = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            lblBorrower = InfoValue();
            lblLoanType = InfoValue();
            lblAmount = InfoValue();
            lblApprovedDate = InfoValue();

            info.Controls.Add(InfoBlock("Borrower", lblBorrower), 0, 0);
            info.Controls.Add(InfoBlock("Loan Type", lblLoanType), 1, 0);
            info.Controls.Add(InfoBlock("Amount", lblAmount), 0, 1);
            info.Controls.Add(InfoBlock("Approved Date", lblApprovedDate), 1, 1);

            // config
            var cfg = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 10, 0, 0) };
            cfg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            cfg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            dtpReleaseDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Width = 180 };
            dtpReleaseDate.ValueChanged += (s, e) => RefreshUI();

            cmbReleaseMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
            cmbReleaseMode.Items.AddRange(new object[] { "Cash", "Bank Transfer", "Check", "GCash" });
            cmbReleaseMode.SelectedIndex = 0;

            cfg.Controls.Add(Field("Release Date", dtpReleaseDate), 0, 0);
            cfg.Controls.Add(Field("Release Mode", cmbReleaseMode), 1, 0);

            // release details
            var details = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#ECFDF5"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12),
                Margin = new Padding(0, 10, 0, 0)
            };

            var detailsTitle = new Label { Text = "RELEASE DETAILS", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827") };
            details.Controls.Add(detailsTitle);

            var d = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 10, 0, 0) };
            d.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            d.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            lblNet = ValueRight("#16A34A", false);
            lblMonthly = ValueRight("#111827", false);
            lblFirstPayment = ValueRight("#111827", false);
            lblTotal = ValueRight("#16A34A", true);

            AddPair(d, "Net Amount:", lblNet, 0);
            AddPair(d, "First Payment:", lblFirstPayment, 1);
            AddPair(d, "Monthly Due:", lblMonthly, 2);
            AddPair(d, "Total Payable:", lblTotal, 3);

            details.Controls.Add(d);

            // documents
            var docsTitle = new Label { Text = "Documents Verified:", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Margin = new Padding(0, 12, 0, 6) };

            var docs = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            docs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            docs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            chkIds = Doc("IDs Scanned");
            chkIncome = Doc("Proof of Income");
            chkContract = Doc("Contract Signed");
            chkCoMaker = Doc("Co-maker Verified");

            chkIds.CheckedChanged += (s, e) => RefreshUI();
            chkIncome.CheckedChanged += (s, e) => RefreshUI();
            chkContract.CheckedChanged += (s, e) => RefreshUI();
            chkCoMaker.CheckedChanged += (s, e) => RefreshUI();

            docs.Controls.Add(WrapDoc(chkIds), 0, 0);
            docs.Controls.Add(WrapDoc(chkIncome), 1, 0);
            docs.Controls.Add(WrapDoc(chkContract), 0, 1);
            docs.Controls.Add(WrapDoc(chkCoMaker), 1, 1);

            // actions
            var row1 = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 12, 0, 0) };
            btnViewContract = Outline("View Contract", 130);
            btnViewContract.Click += (s, e) => Toast("Viewing contract for " + _selected.LoanNumber);

            btnPrintVoucher = Outline("Print Voucher", 130);
            btnPrintVoucher.Click += (s, e) => Toast("Disbursement voucher sent to printer");

            row1.Controls.Add(btnViewContract);
            row1.Controls.Add(btnPrintVoucher);

            var row2 = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 8, 0, 0) };
            btnConfirm = Success("Confirm Release", 140);
            btnConfirm.Click += (s, e) => Confirm(false);

            btnReleasePrint = Primary("Release & Print", 140);
            btnReleasePrint.Click += (s, e) => Confirm(true);

            btnCancel = DangerOutline("Cancel Release", 140);
            btnCancel.Click += (s, e) => Cancel();

            row2.Controls.Add(btnConfirm);
            row2.Controls.Add(btnReleasePrint);
            row2.Controls.Add(btnCancel);

            pnlDetails.Controls.Add(row2);
            pnlDetails.Controls.Add(row1);
            pnlDetails.Controls.Add(docs);
            pnlDetails.Controls.Add(docsTitle);
            pnlDetails.Controls.Add(details);
            pnlDetails.Controls.Add(cfg);
            pnlDetails.Controls.Add(info);

            root.Controls.Add(searchCard);

            // ===== Today table card =====
            var todayCard = Card("TODAY'S LOANS TO RELEASE", "#F3E8FF");
            var todayBody = new Panel { Dock = DockStyle.Top, Height = 260, BackColor = Color.White };
            todayCard.Controls.Add(todayBody);

            gridToday = new DataGridView
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
            gridToday.Columns.Add("Loan", "Loan #");
            gridToday.Columns.Add("Customer", "Customer");
            gridToday.Columns.Add("Amount", "Amount");
            gridToday.Columns.Add("Applied", "Applied");
            gridToday.Columns.Add(new DataGridViewButtonColumn { HeaderText = "Actions", Text = "Process", UseColumnTextForButtonValue = true });
            gridToday.CellContentClick += GridToday_CellContentClick;

            todayBody.Controls.Add(gridToday);
            root.Controls.Add(todayCard);

            BuildToast();
        }

        private void Search()
        {
            var key = (txtLoanNumber.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                Toast("Please enter a loan number", true);
                return;
            }

            var found = _pendingLoans.FirstOrDefault(x => x.LoanNumber.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (found == null)
            {
                _selected = null;
                Toast("Loan application not found or not approved for release", true);
                RefreshUI();
                return;
            }

            _selected = found;

            // mimic React: auto-check docs if Ready
            bool ready = string.Equals(found.Status, "Ready", StringComparison.OrdinalIgnoreCase);
            chkIds.Checked = ready;
            chkIncome.Checked = ready;
            chkContract.Checked = ready;
            chkCoMaker.Checked = ready;

            Toast("Loan application found!");
            RefreshUI();
        }

        private void Confirm(bool printAfter)
        {
            if (_selected == null) return;

            if (!AllDocs())
            {
                Toast("Please verify all required documents before releasing", true);
                return;
            }

            Toast("Loan " + _selected.LoanNumber + " released to " + _selected.Borrower + "!");

            // keep logic simple: mark Ready like React example
            var item = _pendingLoans.FirstOrDefault(x => x.LoanNumber == _selected.LoanNumber);
            if (item != null) item.Status = "Ready";
            BindToday();

            if (printAfter) Toast("Disbursement voucher sent to printer");

            Cancel(silent: true);
        }

        private void Cancel(bool silent = false)
        {
            txtLoanNumber.Text = "";
            _selected = null;
            chkIds.Checked = false;
            chkIncome.Checked = false;
            chkContract.Checked = false;
            chkCoMaker.Checked = false;

            if (!silent) Toast("Release cancelled");
            RefreshUI();
        }

        private void BindToday()
        {
            gridToday.Rows.Clear();
            foreach (var loan in _pendingLoans)
            {
                int idx = gridToday.Rows.Add(
                    loan.LoanNumber,
                    loan.Borrower,
                    "₱" + (loan.Amount / 1000m).ToString("0", CultureInfo.InvariantCulture) + "K",
                    loan.Status,
                    "Process");
                gridToday.Rows[idx].Tag = loan;
            }
        }

        private void GridToday_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(gridToday.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var loan = gridToday.Rows[e.RowIndex].Tag as LoanReleaseModels;
            if (loan == null) return;

            txtLoanNumber.Text = loan.LoanNumber;
            Search();
        }

        private void RefreshUI()
        {
            pnlDetails.Visible = (_selected != null);

            btnViewContract.Enabled = (_selected != null);
            btnPrintVoucher.Enabled = (_selected != null);
            btnCancel.Enabled = (_selected != null);

            btnConfirm.Enabled = (_selected != null) && AllDocs();
            btnReleasePrint.Enabled = btnConfirm.Enabled;

            if (_selected == null) return;

            lblBorrower.Text = _selected.Borrower;
            lblLoanType.Text = _selected.LoanType + " (" + _selected.TermMonths.ToString(CultureInfo.InvariantCulture) + " months)";
            lblAmount.Text = Money(_selected.Amount);
            lblApprovedDate.Text = _selected.ApprovedDate.ToString("d", CultureInfo.GetCultureInfo("en-US"));

            var details = Calc();
            lblNet.Text = Money(details.net);
            lblFirstPayment.Text = details.firstPayment.ToString("d", CultureInfo.GetCultureInfo("en-US"));
            lblMonthly.Text = Money(details.monthly);
            lblTotal.Text = Money(details.total);
        }

        private bool AllDocs()
        {
            return chkIds.Checked && chkIncome.Checked && chkContract.Checked && chkCoMaker.Checked;
        }

        private (decimal net, decimal monthly, decimal total, DateTime firstPayment) Calc()
        {
            var releaseDate = dtpReleaseDate.Value.Date;
            var net = _selected.Amount - _selected.ProcessingFee;

            var totalInterest = (_selected.Amount * _selected.InterestRate * _selected.TermMonths) / 100m / 12m;
            var total = _selected.Amount + totalInterest;
            var monthly = total / _selected.TermMonths;

            return (Round2(net), Round2(monthly), Round2(total), releaseDate.AddMonths(1));
        }

        private static decimal Round2(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);
        private static string Money(decimal v) => "₱" + v.ToString("N2", CultureInfo.GetCultureInfo("en-US"));

        // ===== UI helpers =====
        private static Panel Card(string title, string headerHex)
        {
            var card = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 0, 0, 12) };
            var header = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = ColorTranslator.FromHtml(headerHex), BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(16, 12, 16, 12) };
            var lbl = new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827") };
            header.Controls.Add(lbl);
            card.Controls.Add(header);
            return card;
        }

        private static Panel Field(string label, Control input)
        {
            var p = new Panel { Dock = DockStyle.Top, AutoSize = true };
            var lbl = new Label { Text = label, AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            p.Controls.Add(lbl);
            p.Controls.Add(input);
            p.Layout += (s, e) =>
            {
                lbl.Location = new Point(0, 0);
                input.Location = new Point(0, lbl.Bottom + 4);
            };
            return p;
        }

        private static Panel InfoBlock(string label, Label value)
        {
            var p = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };
            var lbl = new Label { Text = label, AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            p.Controls.Add(lbl);
            p.Controls.Add(value);
            p.Layout += (s, e) =>
            {
                lbl.Location = new Point(0, 0);
                value.Location = new Point(0, lbl.Bottom + 2);
            };
            return p;
        }

        private static Label InfoValue()
        {
            return new Label { AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827"), Font = new Font("Segoe UI", 9) };
        }

        private static Label ValueRight(string hex, bool bold)
        {
            return new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = ColorTranslator.FromHtml(hex),
                Font = new Font("Segoe UI", 9, bold ? FontStyle.Bold : FontStyle.Regular)
            };
        }

        private static void AddPair(TableLayoutPanel t, string left, Control right, int row)
        {
            if (t.RowCount <= row) t.RowCount = row + 1;
            t.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var l = new Label { Text = left, AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151"), Margin = new Padding(0, 4, 0, 4) };
            right.Margin = new Padding(0, 4, 0, 4);

            t.Controls.Add(l, 0, row);
            t.Controls.Add(right, 1, row);
        }

        private static CheckBox Doc(string text) => new CheckBox { Text = text, AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827") };

        private static Panel WrapDoc(CheckBox c)
        {
            var p = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10), Margin = new Padding(0, 0, 10, 10) };
            p.Controls.Add(c);
            return p;
        }

        private static Button Outline(string text, int width)
        {
            var b = new Button { Text = text, Width = width, Height = 30, BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#111827"), FlatStyle = FlatStyle.Flat };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            return b;
        }

        private static Button Primary(string text, int width)
        {
            var b = new Button { Text = text, Width = width, Height = 30, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            return b;
        }

        private static Button Success(string text, int width)
        {
            var b = new Button { Text = text, Width = width, Height = 30, BackColor = ColorTranslator.FromHtml("#16A34A"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#15803D");
            return b;
        }

        private static Button DangerOutline(string text, int width)
        {
            var b = new Button { Text = text, Width = width, Height = 30, BackColor = Color.White, ForeColor = ColorTranslator.FromHtml("#B91C1C"), FlatStyle = FlatStyle.Flat };
            b.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#FCA5A5");
            return b;
        }

        // ===== Toast =====
        private void BuildToast()
        {
            _toastPanel = new Panel { AutoSize = true, BackColor = ColorTranslator.FromHtml("#111827"), Padding = new Padding(12, 8, 12, 8), Visible = false };
            _toastLabel = new Label { AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 9) };
            _toastPanel.Controls.Add(_toastLabel);
            Controls.Add(_toastPanel);
            _toastPanel.BringToFront();

            _toastTimer = new Timer { Interval = 2200 };
            _toastTimer.Tick += (s, e) => { _toastTimer.Stop(); _toastPanel.Visible = false; };

            Resize += (s, e) => PositionToast();
            PositionToast();
        }

        private void PositionToast()
        {
            if (_toastPanel == null) return;
            _toastPanel.Left = ClientSize.Width - _toastPanel.Width - 12;
            _toastPanel.Top = 12;
        }

        private void Toast(string msg, bool isError = false)
        {
            if (_toastPanel == null || _toastLabel == null) return;

            _toastPanel.BackColor = isError ? ColorTranslator.FromHtml("#991B1B") : ColorTranslator.FromHtml("#111827");
            _toastLabel.Text = msg;

            _toastPanel.Visible = true;
            _toastPanel.BringToFront();
            _toastPanel.PerformLayout();
            PositionToast();

            _toastTimer.Stop();
            _toastTimer.Start();
        }

        private void InitializeComponent()
        {
        }
    }
}