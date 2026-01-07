using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LendingApp.Class.Interface;
using LendingApp.Class.Repo;
using LendingApp.Class.Services.Loans;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public sealed class ApprovedLoanApplicationDialog : Form
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoanApplicationEvaluationRepository _evalRepo;

        private readonly string _appId;

        // NEW: view model instead of EF entities in the UI
        private ApprovedLoanApplicationSummaryVm _vm;

        public ApprovedLoanApplicationDialog(string appId)
            : this(appId, new LoanApplicationRepository(), new CustomerRepository(), new LoanApplicationEvaluationRepository())
        {
        }

        public ApprovedLoanApplicationDialog(
            string appId,
            ILoanApplicationRepository loanRepo,
            ICustomerRepository customerRepo,
            ILoanApplicationEvaluationRepository evalRepo)
        {
            _appId = appId;
            _loanRepo = loanRepo;
            _customerRepo = customerRepo;
            _evalRepo = evalRepo;

            InitializeComponent();
            LoadVm();
            BuildUi();
        }

        private void InitializeComponent()
        {
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(760, 720);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void LoadVm()
        {
            var svc = new ApprovedLoanApplicationSummaryService(_loanRepo, _customerRepo, _evalRepo);
            _vm = svc.Build(_appId);
        }

        private void BuildUi()
        {
            Controls.Clear();

            if (_vm == null)
            {
                Text = "Loan Application - N/A";
                Controls.Add(new Label
                {
                    Text = "Application not found in database.",
                    ForeColor = Color.DarkRed,
                    AutoSize = true,
                    Location = new Point(20, 20)
                });
                return;
            }

            Text = _vm.WindowTitle;

            var root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16),
                BackColor = Color.White
            };
            Controls.Add(root);

            int y = 0;

            // Header
            root.Controls.Add(new Label
            {
                Text = _vm.HeaderText,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Location = new Point(0, y)
            });
            y += 34;

            // Application Summary
            AddSection(root, "Application Summary", ref y);
            AddKeyValueBox(root, ref y, 680, new (string, string)[]
            {
                ("App ID", _vm.ApplicationNumber),
                ("Status", _vm.Status),
                ("Customer", _vm.CustomerDisplay),
                ("Loan Type", _vm.LoanTypeText),
                ("Applied", _vm.AppliedDateText),
                ("Amount", _vm.AmountText),
                ("Approved", _vm.ApprovedDateText),
                ("Term", _vm.TermText),
                ("Approved By", _vm.ApprovedByText),
                ("Purpose", _vm.Purpose)
            });

            // Loan Computation
            AddSection(root, "Loan Computation", ref y);
            AddKeyValueBox(root, ref y, 680, new (string, string)[]
            {
                ("Principal", _vm.PrincipalText),
                ("Interest Rate", _vm.InterestRateText),
                ("Service Fee", _vm.ServiceFeeText),
                ("Total Interest", _vm.TotalInterestText),
                ("Total Payable", _vm.TotalPayableText),
                ("Monthly Amortization", _vm.MonthlyAmortizationText),
                ("Payment Start", _vm.PaymentStartText),
                ("Due Date", "15th of each month")
            });

            // Approval Details
            AddSection(root, "Approval Details", ref y);
            AddKeyValueBox(root, ref y, 680, new (string, string)[]
            {
                ("Approver", _vm.ApproverText),
                ("Approval Level", _vm.ApprovalLevel),
                ("Approval Date", _vm.ApprovalDateText),
                ("Remarks", _vm.Remarks),
                ("Credit Score", _vm.CreditScoreText),
                ("Existing Loans", "None")
            });

            y += 8;

            // Buttons row (unchanged)
            var buttons = new FlowLayoutPanel
            {
                Location = new Point(0, y),
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            var btnContract = MakeOutlineButton("View Contract", 120);
            btnContract.Click += (s, e) => MessageBox.Show("Contract viewer not implemented yet.", "Contract");

            var btnAmort = MakeOutlineButton("View Amortization", 150);
            btnAmort.Click += (s, e) => MessageBox.Show("Amortization viewer not implemented yet.", "Amortization");

            var btnDocs = MakeOutlineButton("View Documents", 130);
            btnDocs.Click += (s, e) => MessageBox.Show("Documents viewer not implemented yet.", "Documents");

            // NOTE: CancelLoan in the old dialog wrote to DB.
            // If you want to fully separate logic, move that to a service too.
            var btnCancelLoan = MakeDangerOutlineButton("Cancel Loan", 120);
            btnCancelLoan.Click += (s, e) => MessageBox.Show("Cancel Loan logic should be moved to a service next.", "Info");

            var btnClose = MakePrimaryButton("Close", 90);
            btnClose.Click += (s, e) => Close();

            buttons.Controls.Add(btnContract);
            buttons.Controls.Add(btnAmort);
            buttons.Controls.Add(btnDocs);
            buttons.Controls.Add(btnCancelLoan);
            buttons.Controls.Add(btnClose);

            root.Controls.Add(buttons);
        }

        // UI helpers below remain unchanged (AddSection/AddKeyValueBox/Make*Button...)
        // Keep your existing implementations as-is.
        private void AddSection(Control parent, string title, ref int y)
        {
            var header = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(700, 34),
                BackColor = Color.FromArgb(239, 246, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 7),
                ForeColor = Color.FromArgb(17, 24, 39)
            };

            header.Controls.Add(label);
            parent.Controls.Add(header);
            y += 42;
        }

        private void AddKeyValueBox(Control parent, ref int y, int width, (string key, string value)[] rows)
        {
            var box = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(width, 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            int rowY = 10;
            foreach (var r in rows)
            {
                var k = new Label
                {
                    Text = r.key + ":",
                    AutoSize = true,
                    Location = new Point(10, rowY),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.FromArgb(55, 65, 81)
                };
                box.Controls.Add(k);

                var v = new Label
                {
                    Text = r.value ?? "",
                    AutoSize = false,
                    Location = new Point(170, rowY),
                    Size = new Size(width - 190, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(17, 24, 39)
                };
                box.Controls.Add(v);

                rowY += 22;
            }

            box.Height = rowY + 10;
            parent.Controls.Add(box);
            y += box.Height + 16;
        }

        private static Button MakeOutlineButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(17, 24, 39),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            return b;
        }

        private static Button MakeDangerOutlineButton(string text, int width)
        {
            var b = MakeOutlineButton(text, width);
            b.ForeColor = Color.FromArgb(185, 28, 28);
            b.FlatAppearance.BorderColor = Color.FromArgb(185, 28, 28);
            return b;
        }

        private static Button MakePrimaryButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }
    }
}