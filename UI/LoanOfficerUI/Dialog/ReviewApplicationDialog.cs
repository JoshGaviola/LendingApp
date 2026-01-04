using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Models.Loans;
using LendingApp.Class.Repo;
using LendingApp.Class.Services;
using LoanApplicationUI;
using LendingApp.Class.Service;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public partial class ReviewApplicationDialog : Form
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly ICustomerRepository _customerRepo;

        private readonly string _appId;

        // Loaded from DB
        private LoanApplicationEntity _application;
        private CustomerRegistrationData _customer;

        public ReviewApplicationDialog(string appId)
            : this(appId, new LoanApplicationRepository(), new CustomerRepository())
        {
        }

        public ReviewApplicationDialog(string appId, ILoanApplicationRepository loanRepo, ICustomerRepository customerRepo)
        {
            _appId = appId;
            _loanRepo = loanRepo;
            _customerRepo = customerRepo;

            InitializeComponent();
            LoadFromDb();
            SetupUI();
        }

        // Backwards compatible constructor (existing caller passes more fields)
        public ReviewApplicationDialog(string appId, string customer, string type, string amount, string status)
            : this(appId)
        {
        }

        private void LoadFromDb()
        {
            _application = _loanRepo.GetByApplicationNumber(_appId);
            if (_application == null) return;

            _customer = _customerRepo.GetById(_application.CustomerId);
        }

        private void ReloadFromDbAndRefreshUi()
        {
            LoadFromDb();
            SetupUI();
        }

        private void SetupUI()
        {
            Text = $"Review Application - {_appId ?? "N/A"}";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(700, 550);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Clear();

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            int currentY = 20;

            string statusText = _application?.Status ?? "N/A";

            var title = new Label
            {
                Text = $"REVIEW APPLICATION - {_appId ?? "N/A"} ({statusText})",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, currentY),
                Size = new Size(650, 30)
            };
            mainPanel.Controls.Add(title);
            currentY += 40;

            int leftY = currentY;
            AddSectionLeft(mainPanel, "Application Details", ref leftY);
            AddApplicationDetails(mainPanel, ref leftY);

            int rightY = currentY;
            AddSectionRight(mainPanel, "Customer Quick Info", ref rightY);
            AddCustomerInfo(mainPanel, ref rightY);

            currentY = Math.Max(leftY, rightY) + 20;

            AddSection(mainPanel, "Required Documents Checklist", ref currentY);
            AddDocumentsChecklist(mainPanel, ref currentY);
            currentY += 20;

            AddSection(mainPanel, "Preliminary Assessment", ref currentY);
            currentY += 20;

            currentY += 50;

            // Decision panel
            var decisionPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 40)
            };

            // Determine workflow action based on status
            var appStatus = (_application?.Status ?? "").Trim();
            bool canEvaluate = string.Equals(appStatus, "Review", StringComparison.OrdinalIgnoreCase);
            bool canSendForEvaluation = string.Equals(appStatus, "Pending", StringComparison.OrdinalIgnoreCase);

            var btnPrimary = new Button
            {
                Text = canEvaluate ? "Evaluate" : "Send for Evaluation",
                Location = new Point(0, 5),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = canEvaluate || canSendForEvaluation
            };
            btnPrimary.FlatAppearance.BorderSize = 0;

            btnPrimary.Click += (s, e) =>
            {
                if (_application == null)
                {
                    MessageBox.Show("Application not found in database.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var currentStatus = (_application.Status ?? "").Trim();

                // 1) Send for Evaluation: Pending -> Review (persist)
                if (string.Equals(currentStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    var creditScore = GetCreditScore();
                    var loanTypeText = GetLoanTypeText();

                    if (!CreditScoringService.MeetsMinimumScore(loanTypeText, creditScore, out int min))
                    {
                        MessageBox.Show(
                            $"This application does not meet the minimum credit score requirement.\n\n" +
                            $"Loan Type: {loanTypeText}\n" +
                            $"Customer Score: {creditScore}/1000\n" +
                            $"Minimum Required: {min}/1000",
                            "Minimum Score Not Met",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    // Update status in DB
                    _application.Status = "Review";
                    _application.StatusDate = DateTime.Now;

                    try
                    {
                        _loanRepo.Update(_application);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update application status.\n\n" + ex.Message, "Database Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    MessageBox.Show("Application sent for evaluation. Status is now REVIEW.", "Updated",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh screen so the button changes into Evaluate immediately
                    ReloadFromDbAndRefreshUi();
                    return;
                }

                // 2) Evaluate: open evaluation form
                if (string.Equals(currentStatus, "Review", StringComparison.OrdinalIgnoreCase))
                {
                    OpenEvaluationForm();
                    return;
                }

                MessageBox.Show("Action not allowed for the current status: " + currentStatus, "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var btnReject = new Button
            {
                Text = "Reject",
                Location = new Point(150, 5),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat
            };
            btnReject.FlatAppearance.BorderColor = Color.Red;
            btnReject.FlatAppearance.BorderSize = 1;
            btnReject.Click += (s, e) =>
            {
                MessageBox.Show("Reject clicked. Feature not implemented.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            decisionPanel.Controls.Add(btnPrimary);
            decisionPanel.Controls.Add(btnReject);
            mainPanel.Controls.Add(decisionPanel);
            currentY += 50;

            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(520, currentY),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9)
            };
            btnClose.Click += (s, e) => Close();
            mainPanel.Controls.Add(btnClose);

            if (_application == null)
            {
                var warn = new Label
                {
                    Text = "Application not found in database.",
                    ForeColor = Color.DarkRed,
                    AutoSize = true,
                    Location = new Point(0, currentY + 40)
                };
                mainPanel.Controls.Add(warn);
            }

            Controls.Add(mainPanel);
        }

        private void OpenEvaluationForm()
        {
            if (_application == null)
            {
                MessageBox.Show("Application not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var customerName = _customer != null
                ? ((_customer.FirstName ?? "") + " " + (_customer.LastName ?? "")).Trim()
                : (_application.CustomerId ?? "N/A");

            var loanTypeText = GetLoanTypeText();
            var amountText = Money(_application.RequestedAmount);

            var eval = new LoanApplicationUI.OfficerEvaluateApplicationForm
            {
                CurrentApplication = new LoanApplicationUI.ApplicationData
                {
                    Id = _application.ApplicationNumber,
                    Customer = customerName,
                    LoanType = loanTypeText,
                    Amount = amountText,
                    AppliedDate = _application.ApplicationDate.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")),
                    Status = _application.Status
                }
            };

            eval.ApplyDefaultsForCustomerType(_customer?.CustomerType);
            eval.ShowDialog(this);
        }

        private string GetLoanTypeText()
        {
            if (_application == null) return "Unknown";

            switch (_application.ProductId)
            {
                case 1: return "Personal Loan";
                case 2: return "Emergency Loan";
                case 3: return "Salary Loan";
                default: return "Product " + _application.ProductId.ToString(CultureInfo.InvariantCulture);
            }
        }

        private int GetCreditScore()
        {
            if (_customer == null) return 0;
            return _customer.InitialCreditScore;
        }

        private static string Money(decimal? amount)
        {
            if (!amount.HasValue) return "";
            return "₱" + amount.Value.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
        }

        // ===== existing layout helpers unchanged =====

        private void AddApplicationDetails(Panel parent, ref int y)
        {
            var panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(290, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            string customerName = _customer != null
                ? ((_customer.FirstName ?? "") + " " + (_customer.LastName ?? "")).Trim()
                : (_application?.CustomerId ?? "N/A");

            string productText = _application != null ? _application.ProductId.ToString(CultureInfo.InvariantCulture) : "N/A";

            string[] details =
            {
                $"Customer: {customerName}",
                $"Customer ID: {_application?.CustomerId ?? "N/A"}",
                $"Product ID: {productText}",
                $"Amount: {Money(_application?.RequestedAmount)}",
                $"Term: {_application?.PreferredTerm.ToString(CultureInfo.InvariantCulture) ?? "N/A"} months",
                $"Purpose: {(_application?.Purpose ?? "")}",
                $"Desired Release: {(_application?.DesiredReleaseDate.HasValue == true ? _application.DesiredReleaseDate.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")) : "")}"
            };

            for (int i = 0; i < details.Length; i++)
            {
                var lbl = new Label
                {
                    Text = details[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(270, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            parent.Controls.Add(panel);
            y += 160;
        }

        private void AddCustomerInfo(Panel parent, ref int y)
        {
            var panel = new Panel
            {
                Location = new Point(310, y),
                Size = new Size(290, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            int creditScore = GetCreditScore();
            string loanTypeText = GetLoanTypeText();
            int minScore = CreditScoringService.GetMinimumApprovalScore(loanTypeText);

            string[] info =
            {
                $"Credit Score: {creditScore}/1000",
                minScore > 0 ? $"Min Required ({loanTypeText}): {minScore}/1000" : $"Min Required ({loanTypeText}): N/A",
                $"Customer Type: {_customer?.CustomerType ?? "N/A"}",
                $"Status: {_customer?.Status ?? "N/A"}",
                $"Mobile: {_customer?.MobileNumber ?? ""}",
                $"Email: {_customer?.EmailAddress ?? ""}"
            };

            for (int i = 0; i < info.Length; i++)
            {
                var lbl = new Label
                {
                    Text = info[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(270, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            parent.Controls.Add(panel);
            y += 160;
        }

        private void AddDocumentsChecklist(Panel parent, ref int y)
        {
            var panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 160),
                BorderStyle = BorderStyle.FixedSingle
            };

            int rowY = 10;

            AddDocumentRow(panel, "Valid ID 1", _customer?.ValidId1Path, ref rowY);
            AddDocumentRow(panel, "Valid ID 2", _customer?.ValidId2Path, ref rowY);
            AddDocumentRow(panel, "Proof of Income", _customer?.ProofOfIncomePath, ref rowY);
            AddDocumentRow(panel, "Proof of Address", _customer?.ProofOfAddressPath, ref rowY);
            AddDocumentRow(panel, "Signature", _customer?.SignatureImagePath, ref rowY);

            parent.Controls.Add(panel);
            y += 170;
        }

        private void AddDocumentRow(Panel panel, string docName, string filePath, ref int y)
        {
            bool exists = !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);

            var checkBox = new CheckBox
            {
                Text = docName,
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Checked = exists,
                Enabled = false
            };

            var btnAction = new Button
            {
                Text = exists ? "View" : "Missing",
                Location = new Point(450, y),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Enabled = exists
            };
            btnAction.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnAction.Click += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open file.\n\n" + ex.Message, "Document",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            panel.Controls.Add(checkBox);
            panel.Controls.Add(btnAction);
            y += 30;
        }

        private void AddSection(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionLeft(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionRight(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(310, y),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(220, 252, 231),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }
    }
}