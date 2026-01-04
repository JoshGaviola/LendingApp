using System;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Models.Loans;
using LendingApp.Class.Repo;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public sealed class ApprovedLoanApplicationDialog : Form
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoanApplicationEvaluationRepository _evalRepo;

        private readonly string _appId;

        private LoanApplicationEntity _application;
        private CustomerRegistrationData _customer;
        private LoanProductEntity _product;
        private LoanApplicationEvaluationEntity _latestEval;

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
            LoadFromDb();
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

        private void LoadFromDb()
        {
            _application = _loanRepo.GetByApplicationNumber(_appId);
            if (_application == null) return;

            _customer = _customerRepo.GetById(_application.CustomerId);
            _latestEval = _evalRepo.GetLatestByApplicationId(_application.ApplicationId);

            using (var db = new AppDbContext())
            {
                _product = db.LoanProducts.AsNoTracking().FirstOrDefault(p => p.ProductId == _application.ProductId);
            }
        }

        private void BuildUi()
        {
            Controls.Clear();

            if (_application == null)
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

            var status = _application.Status ?? "N/A";
            Text = $"Loan Application - {_application.ApplicationNumber} ({status})";

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
                Text = $"Loan Application - {_application.ApplicationNumber} ({status})",
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
                ("App ID", _application.ApplicationNumber),
                ("Status", status),
                ("Customer", GetCustomerDisplay()),
                ("Loan Type", GetLoanTypeText()),
                ("Applied", FormatDate(_application.ApplicationDate)),
                ("Amount", Money(_application.RequestedAmount)),
                ("Approved", _application.ApprovedDate.HasValue ? FormatDate(_application.ApprovedDate.Value) : ""),
                ("Term", (_application.PreferredTerm > 0 ? _application.PreferredTerm.ToString(CultureInfo.InvariantCulture) : "") + " months"),
                ("Approved By", GetApproverDisplayFromEvaluation()),
                ("Purpose", _application.Purpose ?? "")
            });

            // Loan Computation
            AddSection(root, "Loan Computation", ref y);

            var comp = ComputeLoanNumbers();
            AddKeyValueBox(root, ref y, 680, new (string, string)[]
            {
                ("Principal", Money(comp.Principal)),
                ("Interest Rate", comp.InterestRateText),
                ("Service Fee", comp.ServiceFeeText),
                ("Total Interest", Money(comp.TotalInterest)),
                ("Total Payable", Money(comp.TotalPayable)),
                ("Monthly Amortization", Money(comp.MonthlyPayment)),
                ("Payment Start", comp.PaymentStartText),
                ("Due Date", "15th of each month")
            });

            // Approval Details
            AddSection(root, "Approval Details", ref y);
            AddKeyValueBox(root, ref y, 680, new (string, string)[]
            {
                ("Approver", GetApproverDisplayFromEvaluation()),
                ("Approval Level", _latestEval?.ApprovalLevel ?? ""),
                ("Approval Date", _latestEval != null ? _latestEval.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.GetCultureInfo("en-US")) : ""),
                ("Remarks", _latestEval?.Remarks ?? ""),
                ("Credit Score", (_customer != null ? _customer.InitialCreditScore.ToString(CultureInfo.InvariantCulture) : "0") + "/1000"),
                ("Existing Loans", "None")
            });

            y += 8;

            // Buttons row
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

            var btnCancelLoan = MakeDangerOutlineButton("Cancel Loan", 120);
            btnCancelLoan.Click += (s, e) => CancelLoan();

            var btnClose = MakePrimaryButton("Close", 90);
            btnClose.Click += (s, e) => Close();

            buttons.Controls.Add(btnContract);
            buttons.Controls.Add(btnAmort);
            buttons.Controls.Add(btnDocs);
            buttons.Controls.Add(btnCancelLoan);
            buttons.Controls.Add(btnClose);

            root.Controls.Add(buttons);
        }

        private void CancelLoan()
        {
            var status = (_application.Status ?? "").Trim();
            if (!string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cancel Loan is only allowed for Approved applications.", "Not Allowed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Cancel this approved loan application?", "Confirm Cancel",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _application.Status = "Cancelled";
                _application.StatusDate = DateTime.Now;
                _loanRepo.Update(_application);

                MessageBox.Show("Loan application cancelled.", "Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                BuildUi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to cancel loan.\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetCustomerDisplay()
        {
            if (_customer == null) return _application.CustomerId ?? "N/A";
            var name = ((_customer.FirstName ?? "") + " " + (_customer.LastName ?? "")).Trim();
            return name + " (" + (_customer.CustomerId ?? "") + ")";
        }

        private string GetLoanTypeText()
        {
            // Prefer product name if available
            if (_product != null && !string.IsNullOrWhiteSpace(_product.ProductName))
                return _product.ProductName;

            switch (_application.ProductId)
            {
                case 1: return "Personal Loan";
                case 2: return "Emergency Loan";
                case 3: return "Salary Loan";
                default: return "Product " + _application.ProductId.ToString(CultureInfo.InvariantCulture);
            }
        }

        private string GetApproverDisplayFromEvaluation()
        {
            // No users table wired here; use EvaluatedBy id + level, or fallback to ApprovedBy id.
            if (_latestEval != null && _latestEval.EvaluatedBy.HasValue)
                return "User #" + _latestEval.EvaluatedBy.Value.ToString(CultureInfo.InvariantCulture);

            if (_application.ApprovedBy.HasValue)
                return "User #" + _application.ApprovedBy.Value.ToString(CultureInfo.InvariantCulture);

            return "";
        }

        private static string Money(decimal amount)
        {
            return "₱" + amount.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
        }

        private static string FormatDate(DateTime dt)
        {
            return dt.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
        }

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

        private sealed class LoanComputation
        {
            public decimal Principal { get; set; }
            public decimal MonthlyPayment { get; set; }
            public decimal TotalInterest { get; set; }
            public decimal TotalPayable { get; set; }
            public string InterestRateText { get; set; }
            public string ServiceFeeText { get; set; }
            public string PaymentStartText { get; set; }
        }

        private LoanComputation ComputeLoanNumbers()
        {
            var principal = _application.RequestedAmount;

            // Use overrides from latest evaluation if present, otherwise product defaults
            var interestRatePct = _latestEval?.InterestRatePct ?? (_product != null ? _product.InterestRate : 0m);
            var serviceFeePct = _latestEval?.ServiceFeePct ?? (_product != null ? _product.ProcessingFeePct : 0m);
            var termMonths = _latestEval?.TermMonths ?? _application.PreferredTerm;

            if (termMonths <= 0) termMonths = _application.PreferredTerm;
            if (termMonths <= 0) termMonths = 12;

            var method = _latestEval?.InterestMethod ?? "Diminishing Balance";

            decimal monthlyPayment;
            decimal totalInterest;
            decimal totalPayable;

            var annualRate = interestRatePct / 100m;
            var monthlyRate = annualRate / 12m;

            if (string.Equals(method, "Diminishing Balance", StringComparison.OrdinalIgnoreCase))
            {
                monthlyPayment = Pmt(monthlyRate, termMonths, principal);
                totalInterest = (monthlyPayment * termMonths) - principal;
                totalPayable = principal + totalInterest;
            }
            else
            {
                totalInterest = principal * annualRate * (termMonths / 12m);
                totalPayable = principal + totalInterest;
                monthlyPayment = totalPayable / termMonths;
            }

            var serviceFeeAmount = principal * (serviceFeePct / 100m);
            var totalPayableWithFee = totalPayable + serviceFeeAmount;

            // Payment start: 15th of next month (simple rule for UI)
            var approvedDate = _application.ApprovedDate ?? DateTime.Today;
            var nextMonth = new DateTime(approvedDate.Year, approvedDate.Month, 1).AddMonths(1);
            var paymentStart = new DateTime(nextMonth.Year, nextMonth.Month, 15);

            return new LoanComputation
            {
                Principal = RoundMoney(principal),
                MonthlyPayment = RoundMoney(monthlyPayment),
                TotalInterest = RoundMoney(totalInterest),
                TotalPayable = RoundMoney(totalPayableWithFee),

                InterestRateText = interestRatePct.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "% p.a. (" +
                                  (interestRatePct / 12m).ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "% monthly)",

                ServiceFeeText = Money(serviceFeeAmount) + " (" + serviceFeePct.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "%)",

                PaymentStartText = paymentStart.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"))
            };
        }

        private static decimal Pmt(decimal ratePerPeriod, int numberOfPeriods, decimal presentValue)
        {
            if (numberOfPeriods <= 0) return 0m;
            if (ratePerPeriod == 0m) return presentValue / numberOfPeriods;

            var r = (double)ratePerPeriod;
            var pv = (double)presentValue;
            var n = numberOfPeriods;

            var denom = 1.0 - Math.Pow(1.0 + r, -n);
            if (denom == 0.0) return 0m;

            return (decimal)((r * pv) / denom);
        }

        private static decimal RoundMoney(decimal x) => Math.Round(x, 2, MidpointRounding.AwayFromZero);
    }
}