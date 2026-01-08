using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.Loans;
using LendingApp.Class.Repo;
using LendingApp.Class.Services.Loans;

namespace LoanApplicationUI
{
    public partial class OfficerEvaluateApplicationForm : Form
    {
        // Repos
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly ILoanApplicationEvaluationRepository _evalRepo;
        private readonly LendingApp.Class.Interface.ILoanRepository _loanReleaseRepo;
        private readonly int? _currentUserId;

        // Business constants (align with UI text)
        private const decimal ReducedAmountValue = 12000m;

        // Application data
        private ApplicationData currentApplication;

        // Form controls
        private Panel creditAssessmentPanel;
        private Panel loanComputationPanel;
        private Panel approvalWorkflowPanel;

        // Credit assessment controls
        private NumericUpDown paymentHistoryInput;
        private NumericUpDown creditUtilizationInput;
        private NumericUpDown creditHistoryLengthInput;
        private NumericUpDown incomeStabilityInput;
        private Label totalCreditScoreLabel;
        private ComboBox decisionComboBox;

        // labels so we can update the displayed weights per customer type
        private Label lblC1;
        private Label lblC2;
        private Label lblC3;
        private Label lblC4;

        // runtime weights (percent)
        private decimal _w1 = 35m;
        private decimal _w2 = 30m;
        private decimal _w3 = 15m;
        private decimal _w4 = 20m;

        // Loan computation controls
        private ComboBox interestMethodComboBox;
        private NumericUpDown interestRateInput;
        private NumericUpDown serviceFeeInput;
        private Label serviceFeeAmountLabel;
        private Label monthlyPaymentLabel;
        private Label totalInterestLabel;
        private Label totalPayableLabel;
        private Label aprLabel;
        private ComboBox loanTermComboBox;

        // Approval workflow controls
        private ComboBox approvalLevelComboBox;
        private CheckBox requireCoMakerCheckBox;
        private CheckBox reduceAmountCheckBox;
        private CheckBox shortenTermCheckBox;
        private CheckBox additionalCollateralCheckBox;
        private ComboBox rejectionReasonComboBox;
        private TextBox remarksTextBox;

        // Buttons
        private Button generateContractButton;
        private Button viewAmortizationButton;
        private Button saveAsDraftButton;
        private Button rejectButton;
        private Button approveButton;

        // Main container for scrolling
        private Panel mainContainer;

        // score labels for each input row
        private readonly Dictionary<NumericUpDown, Label> _scoreLabels = new Dictionary<NumericUpDown, Label>();

        public ApplicationData CurrentApplication
        {
            get => currentApplication;
            set
            {
                currentApplication = value;
                UpdateApplicationInfo();
            }
        }

        // Default ctor
        public OfficerEvaluateApplicationForm()
    : this(new LoanApplicationRepository(), new LoanApplicationEvaluationRepository(), new LendingApp.Class.Repo.LoanRepository(), null)
        {
        }

        public OfficerEvaluateApplicationForm(
            ILoanApplicationRepository loanRepo,
            ILoanApplicationEvaluationRepository evalRepo,
            LendingApp.Class.Interface.ILoanRepository loanReleaseRepo,
            int? currentUserId)
        {
            _loanRepo = loanRepo ?? new LoanApplicationRepository();
            _evalRepo = evalRepo ?? new LoanApplicationEvaluationRepository();
            _loanReleaseRepo = loanReleaseRepo ?? new LendingApp.Class.Repo.LoanRepository();
            _currentUserId = currentUserId;

            InitializeComponent();
            InitializeControls();
            SetupLayout();
        }

        /// <summary>
        /// Sets the credit assessment calculator defaults based on customer type.
        /// </summary>
        public void ApplyDefaultsForCustomerType(string customerType)
        {
            var type = (customerType ?? "").Trim();

            if (type.Equals("New", StringComparison.OrdinalIgnoreCase))
            {
                _w1 = 40m;
                _w2 = 30m;
                _w3 = 20m;
                _w4 = 10m;

                if (lblC1 != null) lblC1.Text = "Document Verification & Identity (40%):";
                if (lblC2 != null) lblC2.Text = "Employment & Income Stability (30%):";
                if (lblC3 != null) lblC3.Text = "Personal Profile & Stability (20%):";
                if (lblC4 != null) lblC4.Text = "Initial Relationship & Deposit (10%):";

                paymentHistoryInput.Value = 80;
                creditUtilizationInput.Value = 50;
                creditHistoryLengthInput.Value = 60;
                incomeStabilityInput.Value = 70;

                UpdateCreditScore(this, EventArgs.Empty);
                return;
            }

            _w1 = 35m;
            _w2 = 30m;
            _w3 = 15m;
            _w4 = 20m;

            if (lblC1 != null) lblC1.Text = "Payment History (35%):";
            if (lblC2 != null) lblC2.Text = "Credit Utilization (30%):";
            if (lblC3 != null) lblC3.Text = "Credit History Length (15%):";
            if (lblC4 != null) lblC4.Text = "Income Stability (20%):";

            paymentHistoryInput.Value = 90;
            creditUtilizationInput.Value = 65;
            creditHistoryLengthInput.Value = 85;
            incomeStabilityInput.Value = 70;

            UpdateCreditScore(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            Text = "Evaluate Loan Application";
            Size = new Size(900, 700);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(10);
            BackColor = Color.White;
        }

        private void InitializeControls()
        {
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            creditAssessmentPanel = CreatePanel("Credit Assessment Calculator", Color.FromArgb(240, 248, 255), 850);

            paymentHistoryInput = CreateNumericInput(90, 0, 100);
            creditUtilizationInput = CreateNumericInput(65, 0, 100);
            creditHistoryLengthInput = CreateNumericInput(85, 0, 100);
            incomeStabilityInput = CreateNumericInput(70, 0, 100);

            totalCreditScoreLabel = new Label
            {
                Text = "0.00/100",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Green,
                AutoSize = true
            };

            decisionComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Approve", "Approve with Conditions", "Reject" },
                SelectedIndex = 0,
                Width = 180
            };

            lblC1 = new Label { Text = "Payment History (35%):", AutoSize = true, Width = 250 };
            lblC2 = new Label { Text = "Credit Utilization (30%):", AutoSize = true, Width = 250 };
            lblC3 = new Label { Text = "Credit History Length (15%):", AutoSize = true, Width = 250 };
            lblC4 = new Label { Text = "Income Stability (20%):", AutoSize = true, Width = 250 };

            loanComputationPanel = CreatePanel("Loan Computation & Terms", Color.FromArgb(240, 255, 240), 850);

            interestMethodComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Diminishing Balance", "Flat Rate", "Add-on Rate" },
                SelectedIndex = 0,
                Width = 180
            };

            interestRateInput = CreateNumericInput(12.0m, 0, 50, 0.1m);
            serviceFeeInput = CreateNumericInput(2.0m, 0, 10, 0.1m);

            serviceFeeAmountLabel = new Label
            {
                Text = "(₱0.00)",
                AutoSize = true,
                ForeColor = SystemColors.ControlDarkDark
            };

            monthlyPaymentLabel = CreateResultLabel("₱0.00");
            totalInterestLabel = CreateResultLabel("₱0.00");
            totalPayableLabel = CreateResultLabel("₱0.00");
            aprLabel = CreateResultLabel("0%");
            loanTermComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "6 months", "12 months", "18 months", "24 months", "36 months" },
                SelectedIndex = 1,
                Width = 120
            };

            approvalWorkflowPanel = CreatePanel("Approval Workflow & Conditions", Color.FromArgb(255, 248, 225), 850);

            approvalLevelComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Level 1 (≤₱50,000)", "Level 2 (₱50,001-₱100,000)", "Level 3 (>₱100,000)" },
                SelectedIndex = 0,
                Width = 250
            };

            requireCoMakerCheckBox = CreateCheckBox("Require co-maker/guarantor");
            reduceAmountCheckBox = CreateCheckBox("Reduce loan amount to ₱12,000");
            shortenTermCheckBox = CreateCheckBox("Shorten term to 6 months");
            additionalCollateralCheckBox = CreateCheckBox("Additional collateral required");

            rejectionReasonComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items =
                {
                    "Select reason",
                    "Low Credit Score",
                    "Insufficient Income",
                    "High Existing Debt",
                    "Incomplete Documents",
                    "Failed Verification",
                    "Other"
                },
                SelectedIndex = 0,
                Width = 250
            };

            remarksTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Height = 60,
                Width = 400
            };

            remarksTextBox.Enter += (s, e) =>
            {
                if (remarksTextBox.Text == "Enter evaluation remarks...")
                {
                    remarksTextBox.Text = "";
                    remarksTextBox.ForeColor = SystemColors.WindowText;
                }
            };
            remarksTextBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(remarksTextBox.Text))
                {
                    remarksTextBox.Text = "Enter evaluation remarks...";
                    remarksTextBox.ForeColor = SystemColors.GrayText;
                }
            };
            remarksTextBox.Text = "Enter evaluation remarks...";
            remarksTextBox.ForeColor = SystemColors.GrayText;

            generateContractButton = CreateButton("Generate Contract Preview", Color.FromArgb(240, 240, 240), 180);
            viewAmortizationButton = CreateButton("View Amortization Schedule", Color.FromArgb(240, 240, 240), 180);
            saveAsDraftButton = CreateButton("Save as Draft", Color.FromArgb(240, 240, 240), 120);
            rejectButton = CreateButton("Reject Application", Color.FromArgb(255, 240, 240), 140);
            approveButton = CreateButton("Approve Application", Color.FromArgb(230, 255, 230), 140);

            rejectButton.ForeColor = Color.FromArgb(200, 0, 0);
            approveButton.ForeColor = Color.FromArgb(0, 100, 0);
            approveButton.BackColor = Color.FromArgb(144, 238, 144);

            WireUpEvents();

            // Trigger initial calculations once UI exists
            UpdateCreditScore(this, EventArgs.Empty);
            UpdateServiceFeeAmount(this, EventArgs.Empty);
        }

        private void SetupLayout()
        {
            Controls.Add(mainContainer);

            var mainLayout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Padding = new Padding(5)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // CREDIT PANEL
            var creditLayout = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 6,
                Padding = new Padding(10),
                AutoSize = true,
                Width = 800
            };

            creditLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            creditLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            creditLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            creditLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            AddCreditRow(creditLayout, 0, lblC1, paymentHistoryInput);
            AddCreditRow(creditLayout, 1, lblC2, creditUtilizationInput);
            AddCreditRow(creditLayout, 2, lblC3, creditHistoryLengthInput);
            AddCreditRow(creditLayout, 3, lblC4, incomeStabilityInput);

            creditLayout.Controls.Add(new Label
            {
                Text = "TOTAL CREDIT SCORE:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            }, 0, 4);
            creditLayout.Controls.Add(totalCreditScoreLabel, 1, 4);
            creditLayout.SetColumnSpan(totalCreditScoreLabel, 3);

            creditLayout.Controls.Add(new Label
            {
                Text = "Decision:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            }, 0, 5);

            var decisionPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };
            decisionPanel.Controls.Add(decisionComboBox);
            decisionPanel.Controls.Add(new Label
            {
                Text = "(Auto-suggested based on score)",
                AutoSize = true,
                ForeColor = SystemColors.GrayText,
                Font = new Font("Segoe UI", 8),
                Margin = new Padding(10, 5, 0, 0)
            });

            creditLayout.Controls.Add(decisionPanel, 1, 5);
            creditLayout.SetColumnSpan(decisionPanel, 3);

            creditAssessmentPanel.Controls.Add(creditLayout);

            // LOAN PANEL
            var loanLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10),
                AutoSize = true,
                Width = 800
            };

            loanLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            loanLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Interest method row (was missing from UI)
            AddLoanRow(loanLayout, 0, "Method:", interestMethodComboBox);

            AddLoanRow(loanLayout, 1, "Rate:", interestRateInput, new Label { Text = "% p.a.", AutoSize = true });

            loanLayout.Controls.Add(new Label { Text = "Service Fee:", AutoSize = true }, 0, 2);
            var serviceFeePanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            serviceFeePanel.Controls.Add(serviceFeeInput);
            serviceFeePanel.Controls.Add(new Label { Text = "%", Margin = new Padding(5, 3, 0, 0), AutoSize = true });
            serviceFeePanel.Controls.Add(serviceFeeAmountLabel);
            loanLayout.Controls.Add(serviceFeePanel, 1, 2);

            var resultsPanel = CreateResultsPanel();
            loanLayout.SetColumnSpan(resultsPanel, 2);
            loanLayout.Controls.Add(resultsPanel, 0, 3);

            loanLayout.Controls.Add(new Label
            {
                Text = "Adjust Terms:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            }, 0, 4);

            var termsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };
            termsPanel.Controls.Add(loanTermComboBox);
            var recalcButton = CreateButton("Recalculate", Color.FromArgb(240, 240, 240), 100);
            recalcButton.Click += (s, e) => RecalculateLoan();
            termsPanel.Controls.Add(recalcButton);
            loanLayout.Controls.Add(termsPanel, 1, 4);

            loanComputationPanel.Controls.Clear();
            loanComputationPanel.Controls.Add(loanLayout);

            // APPROVAL PANEL
            var approvalLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 8,
                Padding = new Padding(10),
                AutoSize = true,
                Width = 800
            };

            approvalLayout.Controls.Add(new Label { Text = "Approval Level Required:", AutoSize = true, Margin = new Padding(0, 0, 0, 5) }, 0, 0);
            approvalLayout.Controls.Add(approvalLevelComboBox, 0, 1);

            approvalLayout.Controls.Add(new Label { Text = "Conditions:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) }, 0, 2);

            var conditionsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Margin = new Padding(20, 5, 0, 10)
            };
            conditionsPanel.Controls.Add(requireCoMakerCheckBox);
            conditionsPanel.Controls.Add(reduceAmountCheckBox);
            conditionsPanel.Controls.Add(shortenTermCheckBox);
            conditionsPanel.Controls.Add(additionalCollateralCheckBox);

            approvalLayout.Controls.Add(conditionsPanel, 0, 3);

            approvalLayout.Controls.Add(new Label { Text = "Rejection Reason:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) }, 0, 4);
            approvalLayout.Controls.Add(rejectionReasonComboBox, 0, 5);

            approvalLayout.Controls.Add(new Label { Text = "Remarks:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) }, 0, 6);
            approvalLayout.Controls.Add(remarksTextBox, 0, 7);

            approvalWorkflowPanel.Controls.Add(approvalLayout);

            var actionButtonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(10, 10, 10, 10),
                Margin = new Padding(0, 10, 0, 0)
            };
            actionButtonsPanel.Controls.Add(generateContractButton);
            actionButtonsPanel.Controls.Add(viewAmortizationButton);

            var footerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(10, 20, 10, 10),
                Margin = new Padding(0, 20, 0, 0)
            };
            footerPanel.Controls.Add(approveButton);
            footerPanel.Controls.Add(rejectButton);
            footerPanel.Controls.Add(saveAsDraftButton);

            mainLayout.Controls.Add(creditAssessmentPanel, 0, 0);
            mainLayout.Controls.Add(loanComputationPanel, 0, 1);
            mainLayout.Controls.Add(approvalWorkflowPanel, 0, 2);
            mainLayout.Controls.Add(actionButtonsPanel, 0, 3);
            mainLayout.Controls.Add(footerPanel, 0, 4);

            mainContainer.Controls.Add(mainLayout);
        }

        private void WireUpEvents()
        {
            paymentHistoryInput.ValueChanged += UpdateCreditScore;
            creditUtilizationInput.ValueChanged += UpdateCreditScore;
            creditHistoryLengthInput.ValueChanged += UpdateCreditScore;
            incomeStabilityInput.ValueChanged += UpdateCreditScore;

            // Loan recalc triggers
            interestMethodComboBox.SelectedIndexChanged += (s, e) => RecalculateLoan();
            interestRateInput.ValueChanged += (s, e) => RecalculateLoan();
            serviceFeeInput.ValueChanged += UpdateServiceFeeAmount;
            serviceFeeInput.ValueChanged += (s, e) => RecalculateLoan();
            loanTermComboBox.SelectedIndexChanged += (s, e) => RecalculateLoan();
            reduceAmountCheckBox.CheckedChanged += (s, e) =>
            {
                UpdateServiceFeeAmount(s, e);
                RecalculateLoan();
            };

            generateContractButton.Click += (s, e) =>
            {
                try
                {
                    var appEntity = _loanRepo.GetByApplicationNumber(currentApplication?.Id);
                    if (appEntity == null)
                    {
                        MessageBox.Show("Application not loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // optional: persist current evaluation as draft first, then use its id.
                    // For now we just generate using applicationId and latest evaluation if any.
                    var latestEval = _evalRepo.GetLatestByApplicationId(appEntity.ApplicationId);
                    long? evalId = latestEval?.EvaluationId;

                    var tempPath = Path.Combine(Path.GetTempPath(), $"contract_{appEntity.ApplicationNumber}_{Guid.NewGuid():N}.pdf");
                    var svc = new LendingApp.Class.Services.Contracts.ContractService();
                    svc.GenerateContractPdf(appEntity.ApplicationId, evalId, tempPath);

                    using (var preview = new LendingApp.UI.LoanOfficerUI.Dialog.ContractPreviewForm(tempPath))
                    {
                        preview.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to generate contract preview:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            viewAmortizationButton.Click += ViewAmortizationButton_Click;

            saveAsDraftButton.Click += (s, e) => SaveAsDraft();
            rejectButton.Click += (s, e) => RejectApplication();
            approveButton.Click += (s, e) => ApproveApplication();
        }

        private void ViewAmortizationButton_Click(object sender, EventArgs e)
        {
            try
            {
                var principal = GetEffectivePrincipal();
                if (principal <= 0m)
                {
                    MessageBox.Show("Invalid principal amount for amortization.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var term = ParseSelectedTermMonths(loanTermComboBox);
                if (term <= 0)
                {
                    MessageBox.Show("Select a valid term.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rate = (decimal)interestRateInput.Value;
                var fee = (decimal)serviceFeeInput.Value;
                var method = MapInterestMethod(interestMethodComboBox.SelectedItem?.ToString());

                var schedule = LoanComputationService.GetAmortizationSchedule(
                    principal: principal,
                    annualRatePct: rate,
                    termMonths: term,
                    serviceFeePct: fee,
                    method: method);

                using (var f = new AmortizationScheduleForm(schedule, $"Amortization — ₱{principal:N2} · {term} months"))
                {
                    f.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to generate amortization schedule.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------
        // Credit Assessment Logic
        // ---------------------------
        private void UpdateCreditScore(object sender, EventArgs e)
        {
            decimal s1 = (paymentHistoryInput.Value / 100m) * _w1;
            decimal s2 = (creditUtilizationInput.Value / 100m) * _w2;
            decimal s3 = (creditHistoryLengthInput.Value / 100m) * _w3;
            decimal s4 = (incomeStabilityInput.Value / 100m) * _w4;

            if (_scoreLabels.TryGetValue(paymentHistoryInput, out var l1)) l1.Text = $"→ {s1:F1}";
            if (_scoreLabels.TryGetValue(creditUtilizationInput, out var l2)) l2.Text = $"→ {s2:F1}";
            if (_scoreLabels.TryGetValue(creditHistoryLengthInput, out var l3)) l3.Text = $"→ {s3:F1}";
            if (_scoreLabels.TryGetValue(incomeStabilityInput, out var l4)) l4.Text = $"→ {s4:F1}";

            decimal totalScore = s1 + s2 + s3 + s4;
            totalCreditScoreLabel.Text = $"{totalScore:F2}/100";

            if (totalScore >= 80) totalCreditScoreLabel.ForeColor = Color.Green;
            else if (totalScore >= 60) totalCreditScoreLabel.ForeColor = Color.Blue;
            else if (totalScore >= 40) totalCreditScoreLabel.ForeColor = Color.Orange;
            else totalCreditScoreLabel.ForeColor = Color.Red;

            ApplyAutoDecision(totalScore);

            // keep loan numbers in sync (not required, but good UX)
            RecalculateLoan();
        }

        private void ApplyAutoDecision(decimal score0to100)
        {
            // Simple policy:
            // >= 80 approve, >= 60 approve with conditions, else reject
            var desired =
                (score0to100 >= 80m) ? "Approve" :
                (score0to100 >= 60m) ? "Approve with Conditions" :
                "Reject";

            if (!string.Equals(decisionComboBox.SelectedItem?.ToString(), desired, StringComparison.OrdinalIgnoreCase))
            {
                // Prevent annoying override if officer manually changes it:
                // Only auto-change if previously auto-suggested or still default.
                decisionComboBox.SelectedItem = desired;
            }

            // Enable/disable rejection reason based on decision
            var isReject = string.Equals(desired, "Reject", StringComparison.OrdinalIgnoreCase);
            rejectionReasonComboBox.Enabled = isReject;
        }

        // ---------------------------
        // Loan Computation Logic (UI adapter only)
        // ---------------------------

        private static LoanInterestMethod MapInterestMethod(string uiText)
        {
            var m = (uiText ?? "").Trim();

            if (m.Equals("Diminishing Balance", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.DiminishingBalance;

            if (m.Equals("Flat Rate", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.FlatRate;

            if (m.Equals("Add-on Rate", StringComparison.OrdinalIgnoreCase))
                return LoanInterestMethod.AddOnRate;

            // default: preserves old behavior (anything else treated like flat/add-on)
            return LoanInterestMethod.FlatRate;
        }

        private void UpdateServiceFeeAmount(object sender, EventArgs e)
        {
            try
            {
                var principal = GetEffectivePrincipal();
                var r = LoanComputationService.Calculate(
                    principal,
                    (decimal)interestRateInput.Value,
                    ParseSelectedTermMonths(loanTermComboBox),
                    (decimal)serviceFeeInput.Value,
                    MapInterestMethod(interestMethodComboBox.SelectedItem?.ToString()));

                serviceFeeAmountLabel.Text = "(₱" + r.ServiceFeeAmount.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + ")";
            }
            catch
            {
                serviceFeeAmountLabel.Text = "(₱0.00)";
            }
        }

        private void RecalculateLoan()
        {
            try
            {
                var principal = GetEffectivePrincipal();
                var term = ParseSelectedTermMonths(loanTermComboBox);
                var rate = (decimal)interestRateInput.Value;
                var fee = (decimal)serviceFeeInput.Value;
                var method = MapInterestMethod(interestMethodComboBox.SelectedItem?.ToString());

                var r = LoanComputationService.Calculate(principal, rate, term, fee, method);

                monthlyPaymentLabel.Text = "₱" + r.MonthlyPayment.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
                totalInterestLabel.Text = "₱" + r.TotalInterest.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
                totalPayableLabel.Text = "₱" + r.TotalPayable.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
                aprLabel.Text = r.AprPct.ToString("N2", CultureInfo.GetCultureInfo("en-US")) + "%";
            }
            catch
            {
                monthlyPaymentLabel.Text = "₱0.00";
                totalInterestLabel.Text = "₱0.00";
                totalPayableLabel.Text = "₱0.00";
                aprLabel.Text = "0%";
            }
        }

        private void CreateLoanRecordIfMissing(LoanApplicationEntity app)
        {
            var existing = _loanReleaseRepo.GetByApplicationId(app.ApplicationId);
            if (existing != null) return;

            var principal = app.RequestedAmount;
            var termMonths = app.PreferredTerm > 0 ? app.PreferredTerm : ParseSelectedTermMonths(loanTermComboBox);

            var interestRatePct = (decimal)interestRateInput.Value;
            var serviceFeePct = (decimal)serviceFeeInput.Value;
            var method = MapInterestMethod(interestMethodComboBox.SelectedItem?.ToString());

            var calc = LoanComputationService.Calculate(principal, interestRatePct, termMonths, serviceFeePct, method);

            var releaseDate = DateTime.Today;
            var firstDue = GetNext15th(releaseDate);
            var maturity = firstDue.AddMonths(termMonths);

            var loan = new LoanEntity
            {
                LoanNumber = GenerateLoanNumber(app.ApplicationNumber),

                ApplicationId = app.ApplicationId,
                CustomerId = app.CustomerId,
                ProductId = app.ProductId,

                PrincipalAmount = Math.Round(principal, 2, MidpointRounding.AwayFromZero),
                InterestRate = Math.Round(interestRatePct, 2, MidpointRounding.AwayFromZero),
                TermMonths = termMonths,
                MonthlyPayment = calc.MonthlyPayment,

                ProcessingFee = calc.ServiceFeeAmount,
                TotalPayable = calc.TotalPayable,
                OutstandingBalance = calc.TotalPayable,

                TotalPaid = 0m,
                TotalInterestPaid = 0m,
                TotalPenaltyPaid = 0m,

                Status = "Active",
                DaysOverdue = 0,

                ReleaseDate = releaseDate,
                FirstDueDate = firstDue,
                NextDueDate = firstDue,
                MaturityDate = maturity,
                LastPaymentDate = null,

                ReleaseMode = null,
                ReleasedBy = null,

                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            _loanReleaseRepo.Add(loan);
        }

        // ---------------------------
        // DB Persistence Logic
        // ---------------------------
        private LoanApplicationEntity LoadEntityOrThrow()
        {
            if (currentApplication == null || string.IsNullOrWhiteSpace(currentApplication.Id))
                throw new InvalidOperationException("CurrentApplication.Id (ApplicationNumber) is not set.");

            var entity = _loanRepo.GetByApplicationNumber(currentApplication.Id);
            if (entity == null)
                throw new InvalidOperationException("Application not found in DB: " + currentApplication.Id);

            return entity;
        }

        private static string CleanRemarks(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "Enter evaluation remarks...")
                return null;

            return text.Trim();
        }

        private static string MapApprovalLevelText(string uiText)
        {
            if (string.IsNullOrWhiteSpace(uiText)) return null;
            var idx = uiText.IndexOf('(');
            return idx > 0 ? uiText.Substring(0, idx).Trim() : uiText.Trim();
        }

        private (decimal c1w, decimal c2w, decimal c3w, decimal c4w, decimal total) ComputeWeightedScores()
        {
            decimal c1w = (paymentHistoryInput.Value / 100m) * _w1;
            decimal c2w = (creditUtilizationInput.Value / 100m) * _w2;
            decimal c3w = (creditHistoryLengthInput.Value / 100m) * _w3;
            decimal c4w = (incomeStabilityInput.Value / 100m) * _w4;
            decimal total = c1w + c2w + c3w + c4w;

            return (Math.Round(c1w, 2), Math.Round(c2w, 2), Math.Round(c3w, 2), Math.Round(c4w, 2), Math.Round(total, 2));
        }

        private LoanApplicationEvaluationEntity BuildEvaluationEntity(LoanApplicationEntity app, string decision, string statusAfter)
        {
            var scores = ComputeWeightedScores();

            return new LoanApplicationEvaluationEntity
            {
                ApplicationId = app.ApplicationId,

                C1Input = paymentHistoryInput.Value,
                C2Input = creditUtilizationInput.Value,
                C3Input = creditHistoryLengthInput.Value,
                C4Input = incomeStabilityInput.Value,

                W1Pct = _w1,
                W2Pct = _w2,
                W3Pct = _w3,
                W4Pct = _w4,

                C1Weighted = scores.c1w,
                C2Weighted = scores.c2w,
                C3Weighted = scores.c3w,
                C4Weighted = scores.c4w,
                TotalScore = scores.total,

                Decision = decision,

                InterestMethod = interestMethodComboBox.SelectedItem != null ? interestMethodComboBox.SelectedItem.ToString() : "Diminishing Balance",
                InterestRatePct = interestRateInput.Value,
                ServiceFeePct = serviceFeeInput.Value,
                TermMonths = ParseSelectedTermMonths(loanTermComboBox),

                ApprovalLevel = MapApprovalLevelText(approvalLevelComboBox.SelectedItem != null ? approvalLevelComboBox.SelectedItem.ToString() : null),

                RequireComaker = requireCoMakerCheckBox.Checked,
                ReduceAmount = reduceAmountCheckBox.Checked,
                ShortenTerm = shortenTermCheckBox.Checked,
                AdditionalCollateral = additionalCollateralCheckBox.Checked,

                RejectionReason = string.Equals(decision, "Reject", StringComparison.OrdinalIgnoreCase)
                    ? (rejectionReasonComboBox.SelectedIndex > 0 ? rejectionReasonComboBox.SelectedItem.ToString() : null)
                    : null,

                Remarks = CleanRemarks(remarksTextBox.Text),

                EvaluatedBy = _currentUserId,
                StatusAfter = statusAfter,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private void SaveAsDraft()
        {
            try
            {
                var app = LoadEntityOrThrow();

                var decision = decisionComboBox.SelectedItem != null ? decisionComboBox.SelectedItem.ToString() : "Approve";

                _evalRepo.Add(BuildEvaluationEntity(app, decision, "Review"));

                app.PreferredTerm = ParseSelectedTermMonths(loanTermComboBox);
                app.Status = "Review";
                app.StatusDate = DateTime.Now;

                if (reduceAmountCheckBox.Checked)
                    app.RequestedAmount = ReducedAmountValue;

                _loanRepo.Update(app);

                MessageBox.Show("Evaluation saved as draft.", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save draft.\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RejectApplication()
        {
            if (rejectionReasonComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a rejection reason.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to reject this application?",
                    "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                var app = LoadEntityOrThrow();

                _evalRepo.Add(BuildEvaluationEntity(app, "Reject", "Rejected"));

                app.PreferredTerm = ParseSelectedTermMonths(loanTermComboBox);
                app.Status = "Rejected";
                app.RejectionReason = rejectionReasonComboBox.SelectedItem.ToString();
                app.StatusDate = DateTime.Now;

                if (reduceAmountCheckBox.Checked)
                    app.RequestedAmount = ReducedAmountValue;

                _loanRepo.Update(app);

                MessageBox.Show("Application rejected.", "Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.Abort;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to reject application.\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApproveApplication()
        {
            var decision = decisionComboBox.SelectedItem != null ? decisionComboBox.SelectedItem.ToString() : "Approve";

            // user chose approve-with-conditions => still Approved (per your instruction)
            if (string.Equals(decision, "Reject", StringComparison.OrdinalIgnoreCase))
                decision = "Approve";

            if (MessageBox.Show("Are you sure you want to approve this application?",
                    "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var app = LoadEntityOrThrow();

                _evalRepo.Add(BuildEvaluationEntity(app, decision, "Approved"));

                app.PreferredTerm = ParseSelectedTermMonths(loanTermComboBox);
                app.Status = "Approved";
                app.ApprovedDate = DateTime.Now;
                app.StatusDate = DateTime.Now;

                if (reduceAmountCheckBox.Checked)
                    app.RequestedAmount = ReducedAmountValue;

                _loanRepo.Update(app);
                CreateLoanRecordIfMissing(app);

                MessageBox.Show("Application approved successfully.", "Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to approve application.\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateApplicationInfo()
        {
            if (currentApplication == null) return;

            Text = $"Evaluate Loan Application - {currentApplication.Id} (Review)";
            UpdateServiceFeeAmount(null, EventArgs.Empty);
            RecalculateLoan();
        }

        // ---------------------------
        // UI Helper Methods (unchanged structure)
        // ---------------------------
        private void AddCreditRow(TableLayoutPanel panel, int row, Label label, NumericUpDown inputControl)
        {
            panel.Controls.Add(label, 0, row);
            panel.Controls.Add(inputControl, 1, row);

            panel.Controls.Add(new Label
            {
                Text = "/100",
                AutoSize = true,
                Margin = new Padding(10, 0, 0, 0)
            }, 2, row);

            var scoreLabel = new Label
            {
                Text = "→ 0.0",
                AutoSize = true,
                ForeColor = SystemColors.ControlDarkDark,
                Margin = new Padding(10, 0, 0, 0)
            };
            panel.Controls.Add(scoreLabel, 3, row);

            _scoreLabels[inputControl] = scoreLabel;
        }

        private void AddLoanRow(TableLayoutPanel panel, int row, string labelText, Control inputControl, Control additionalControl = null)
        {
            panel.Controls.Add(new Label
            {
                Text = labelText,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5)
            }, 0, row);

            if (additionalControl != null)
            {
                var flowPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
                flowPanel.Controls.Add(inputControl);
                flowPanel.Controls.Add(additionalControl);
                panel.Controls.Add(flowPanel, 1, row);
            }
            else
            {
                panel.Controls.Add(inputControl, 1, row);
            }
        }

        private Panel CreatePanel(string title, Color headerColor, int width)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(0, 0, 0, 5),
                Width = width
            };

            var header = new Panel
            {
                BackColor = headerColor,
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(10, 0, 0, 0)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            header.Controls.Add(titleLabel);
            panel.Controls.Add(header);

            return panel;
        }

        private NumericUpDown CreateNumericInput(decimal defaultValue, decimal min, decimal max, decimal increment = 1)
        {
            return new NumericUpDown
            {
                Value = defaultValue,
                Minimum = min,
                Maximum = max,
                Increment = increment,
                DecimalPlaces = increment < 1 ? 1 : 0,
                Width = 80,
                Margin = new Padding(0, 0, 5, 0)
            };
        }

        private CheckBox CreateCheckBox(string text)
        {
            return new CheckBox
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 2, 0, 2)
            };
        }

        private Button CreateButton(string text, Color backColor, int width = 0)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                FlatStyle = FlatStyle.Standard,
                Height = 32,
                Padding = new Padding(10, 0, 10, 0),
                Margin = new Padding(0, 0, 5, 0),
                Cursor = Cursors.Hand
            };

            if (width > 0) button.Width = width;
            return button;
        }

        private Label CreateResultLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(0, 2, 0, 2)
            };
        }

        private Panel CreateResultsPanel()
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 255, 245),
                Margin = new Padding(0, 10, 0, 10),
                Padding = new Padding(10),
                AutoSize = true,
                Width = 780
            };

            var header = new Panel { Dock = DockStyle.Top, Height = 15, Padding = new Padding(10, 0, 0, 0) };

            var headerLabel = new Label
            {
                Text = "CALCULATION RESULTS",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            var resultsLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true,
                Padding = new Padding(0, 30, 0, 0)
            };

            resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            resultsLayout.Controls.Add(new Label { Text = "Monthly Payment:", AutoSize = true, Margin = new Padding(0, 2, 10, 2) }, 0, 0);
            resultsLayout.Controls.Add(monthlyPaymentLabel, 1, 0);

            resultsLayout.Controls.Add(new Label { Text = "Total Interest:", AutoSize = true, Margin = new Padding(0, 2, 10, 2) }, 0, 1);
            resultsLayout.Controls.Add(totalInterestLabel, 1, 1);

            resultsLayout.Controls.Add(new Label { Text = "Total Payable:", AutoSize = true, Margin = new Padding(0, 2, 10, 2) }, 0, 2);
            resultsLayout.Controls.Add(totalPayableLabel, 1, 2);

            resultsLayout.Controls.Add(new Label { Text = "APR:", AutoSize = true, Margin = new Padding(0, 2, 10, 2) }, 0, 3);
            resultsLayout.Controls.Add(aprLabel, 1, 3);

            header.Controls.Add(headerLabel);
            panel.Controls.Add(header);
            panel.Controls.Add(resultsLayout);

            return panel;
        }

        private static string GenerateLoanNumber(string applicationNumber)
        {
            // Fits VARCHAR(20). Example: LN-APP-20240104-083000 might be too long.
            // Keep it short & deterministic.
            // Example: APP-20240104-083000 => LN-240104083000 (14 chars + "LN-" = 17)
            var digits = new string((applicationNumber ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length >= 12) digits = digits.Substring(digits.Length - 12);
            if (string.IsNullOrWhiteSpace(digits)) digits = DateTime.Now.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
            return "LN-" + digits;
        }

        private static DateTime GetNext15th(DateTime from)
        {
            // if today is <= 15, choose this month 15th; else next month 15th
            var baseDate = new DateTime(from.Year, from.Month, 1);
            var fifteenth = baseDate.AddDays(14);
            return (from.Date <= fifteenth) ? fifteenth : baseDate.AddMonths(1).AddDays(14);
        }

        private decimal GetEffectivePrincipal()
        {
            // If checkbox says reduce, we compute using reduced amount immediately (best UX)
            if (reduceAmountCheckBox != null && reduceAmountCheckBox.Checked) return ReducedAmountValue;

            decimal principal;
            if (TryParseMoney(currentApplication != null ? currentApplication.Amount : null, out principal))
                return principal;

            return 0m;
        }

        private static bool TryParseMoney(string text, out decimal amount)
        {
            amount = 0m;
            if (string.IsNullOrWhiteSpace(text)) return false;

            var cleaned = text.Replace("₱", "").Replace(",", "").Trim();
            return decimal.TryParse(
                cleaned,
                NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                CultureInfo.GetCultureInfo("en-US"),
                out amount);
        }

        private static int ParseSelectedTermMonths(ComboBox cbo)
        {
            var t = (cbo != null ? (cbo.SelectedItem ?? "") : "").ToString();
            if (string.IsNullOrWhiteSpace(t)) return 12;

            var cleaned = t.ToLowerInvariant()
                .Replace("months", "")
                .Replace("month", "")
                .Trim();

            int months;
            return int.TryParse(cleaned, out months) ? months : 12;
        }
    }
}