using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoanApplicationUI
{
    public partial class OfficerEvaluateApplicationForm : Form
    {
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

        public ApplicationData CurrentApplication
        {
            get => currentApplication;
            set
            {
                currentApplication = value;
                UpdateApplicationInfo();
            }
        }

        public OfficerEvaluateApplicationForm()
        {
            InitializeComponent();
            InitializeControls();
            SetupLayout();
        }

        private void InitializeComponent()
        {
            this.Text = "Evaluate Loan Application";
            this.Size = new Size(950, 850);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new Padding(20);
            this.BackColor = Color.White;
        }

        private void InitializeControls()
        {
            // Credit Assessment Panel
            creditAssessmentPanel = CreatePanel("Credit Assessment Calculator", Color.FromArgb(240, 248, 255));

            paymentHistoryInput = CreateNumericInput(90, 0, 100);
            creditUtilizationInput = CreateNumericInput(65, 0, 100);
            creditHistoryLengthInput = CreateNumericInput(85, 0, 100);
            incomeStabilityInput = CreateNumericInput(70, 0, 100);

            totalCreditScoreLabel = new Label
            {
                Text = "80.00/100",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Green,
                AutoSize = true
            };

            decisionComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Approve", "Approve with Conditions", "Reject" },
                SelectedIndex = 0,
                Width = 200
            };

            // Loan Computation Panel
            loanComputationPanel = CreatePanel("Loan Computation & Terms", Color.FromArgb(240, 255, 240));

            interestMethodComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Diminishing Balance", "Flat Rate", "Add-on Rate" },
                SelectedIndex = 0,
                Width = 200
            };

            interestRateInput = CreateNumericInput(12.0m, 0, 50, 0.1m);
            serviceFeeInput = CreateNumericInput(2.0m, 0, 10, 0.1m);

            serviceFeeAmountLabel = new Label
            {
                Text = "(₱0.00)",
                AutoSize = true,
                ForeColor = SystemColors.ControlDarkDark
            };

            // Calculation results labels
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

            // Approval Workflow Panel
            approvalWorkflowPanel = CreatePanel("Approval Workflow & Conditions", Color.FromArgb(255, 248, 225));

            approvalLevelComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Level 1 (≤₱50,000)", "Level 2 (₱50,001-₱100,000)", "Level 3 (>₱100,000)" },
                SelectedIndex = 0,
                Width = 250
            };

            // Checkboxes
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
                Width = 400,
                Text = "Enter evaluation remarks..."
            };

            // Action buttons
            generateContractButton = CreateButton("Generate Contract Preview", Color.FromArgb(240, 240, 240));
            viewAmortizationButton = CreateButton("View Amortization Schedule", Color.FromArgb(240, 240, 240));
            saveAsDraftButton = CreateButton("Save as Draft", Color.FromArgb(240, 240, 240));
            rejectButton = CreateButton("Reject Application", Color.FromArgb(255, 240, 240));
            approveButton = CreateButton("Approve Application", Color.FromArgb(230, 255, 230));

            // Set button colors
            rejectButton.ForeColor = Color.FromArgb(200, 0, 0);
            approveButton.ForeColor = Color.FromArgb(0, 100, 0);
            approveButton.BackColor = Color.FromArgb(144, 238, 144);

            // Wire up events
            WireUpEvents();
        }

        private void SetupLayout()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(0, 0, 0, 10)
            };

            // Credit Assessment Section
            var creditLayout = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 5,
                Padding = new Padding(10),
                AutoSize = true
            };

            AddCreditRow(creditLayout, 0, "Payment History (35%):", paymentHistoryInput);
            AddCreditRow(creditLayout, 1, "Credit Utilization (30%):", creditUtilizationInput);
            AddCreditRow(creditLayout, 2, "Credit History Length (15%):", creditHistoryLengthInput);
            AddCreditRow(creditLayout, 3, "Income Stability (20%):", incomeStabilityInput);

            // Total score row
            creditLayout.Controls.Add(new Label { Text = "TOTAL CREDIT SCORE:", AutoSize = true }, 0, 4);
            creditLayout.Controls.Add(totalCreditScoreLabel, 1, 4);

            // Decision row
            creditLayout.Controls.Add(new Label { Text = "Decision:", AutoSize = true }, 0, 5);
            creditLayout.Controls.Add(decisionComboBox, 1, 5);

            creditAssessmentPanel.Controls.Add(creditLayout);

            // Loan Computation Section
            var loanLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10),
                AutoSize = true
            };

            AddLoanRow(loanLayout, 0, "Interest Method:", interestMethodComboBox);
            AddLoanRow(loanLayout, 1, "Rate:", interestRateInput, new Label { Text = "% p.a.", AutoSize = true });

            // Service fee row
            loanLayout.Controls.Add(new Label { Text = "Service Fee:", AutoSize = true }, 0, 2);
            var serviceFeePanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            serviceFeePanel.Controls.Add(serviceFeeInput);
            serviceFeePanel.Controls.Add(new Label { Text = "%", Margin = new Padding(5, 3, 0, 0) });
            serviceFeePanel.Controls.Add(serviceFeeAmountLabel);
            loanLayout.Controls.Add(serviceFeePanel, 1, 2);

            // Calculation results
            var resultsPanel = CreateResultsPanel();
            loanLayout.SetColumnSpan(resultsPanel, 2);
            loanLayout.Controls.Add(resultsPanel, 0, 3);
            loanLayout.SetRow(resultsPanel, 3);

            // Adjust terms row
            loanLayout.Controls.Add(new Label { Text = "Adjust Terms:", AutoSize = true }, 0, 4);
            var termsPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            termsPanel.Controls.Add(loanTermComboBox);
            var recalcButton = CreateButton("Recalculate", Color.FromArgb(240, 240, 240));
            recalcButton.Click += (s, e) => RecalculateLoan();
            termsPanel.Controls.Add(recalcButton);
            loanLayout.Controls.Add(termsPanel, 1, 4);

            loanComputationPanel.Controls.Add(loanLayout);

            // Approval Workflow Section
            var approvalLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10),
                AutoSize = true
            };

            AddApprovalRow(approvalLayout, 0, "Approval Level Required:", approvalLevelComboBox);

            // Conditions
            approvalLayout.Controls.Add(new Label { Text = "Conditions:", AutoSize = true }, 0, 1);
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
            approvalLayout.Controls.Add(conditionsPanel, 0, 2);

            AddApprovalRow(approvalLayout, 3, "Rejection Reason:", rejectionReasonComboBox);
            AddApprovalRow(approvalLayout, 4, "Remarks:", remarksTextBox);

            approvalWorkflowPanel.Controls.Add(approvalLayout);

            // Additional Action Buttons
            var actionButtonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 10)
            };
            actionButtonsPanel.Controls.Add(generateContractButton);
            actionButtonsPanel.Controls.Add(viewAmortizationButton);

            // Footer Buttons
            var footerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 0, 0)
            };
            footerPanel.Controls.Add(approveButton);
            footerPanel.Controls.Add(rejectButton);
            footerPanel.Controls.Add(saveAsDraftButton);

            // Add everything to main layout
            mainLayout.Controls.Add(creditAssessmentPanel, 0, 0);
            mainLayout.Controls.Add(loanComputationPanel, 0, 1);
            mainLayout.Controls.Add(approvalWorkflowPanel, 0, 2);
            mainLayout.Controls.Add(actionButtonsPanel, 0, 3);
            mainLayout.Controls.Add(new Panel(), 0, 4); // Spacer
            mainLayout.Controls.Add(footerPanel, 0, 5);

            this.Controls.Add(mainLayout);
        }

        private void AddCreditRow(TableLayoutPanel panel, int row, string labelText, Control inputControl)
        {
            panel.Controls.Add(new Label { Text = labelText, AutoSize = true }, 0, row);
            panel.Controls.Add(inputControl, 1, row);
            panel.Controls.Add(new Label { Text = "/100", AutoSize = true }, 2, row);

            // Score calculation label
            var scoreLabel = new Label
            {
                Text = "→ 0.0",
                AutoSize = true,
                ForeColor = SystemColors.ControlDarkDark
            };
            panel.Controls.Add(scoreLabel, 3, row);
        }

        private void AddLoanRow(TableLayoutPanel panel, int row, string labelText, Control inputControl, Control additionalControl = null)
        {
            panel.Controls.Add(new Label { Text = labelText, AutoSize = true }, 0, row);

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

        private void AddApprovalRow(TableLayoutPanel panel, int row, string labelText, Control inputControl)
        {
            panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Margin = new Padding(0, 5, 0, 5) }, 0, row);
            panel.Controls.Add(inputControl, 0, row + 1);
        }

        private Panel CreatePanel(string title, Color headerColor)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
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
                Width = 80
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

        private Button CreateButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                FlatStyle = FlatStyle.Standard,
                Height = 32,
                Padding = new Padding(10, 0, 10, 0),
                Margin = new Padding(0, 0, 5, 0),
                Cursor = Cursors.Hand
            };
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
                AutoSize = true
            };

            var header = new Panel
            {
                BackColor = Color.FromArgb(200, 230, 200),
                Dock = DockStyle.Top,
                Height = 25,
                Padding = new Padding(10, 0, 0, 0)
            };

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

            resultsLayout.Controls.Add(new Label { Text = "Monthly Payment:", AutoSize = true }, 0, 0);
            resultsLayout.Controls.Add(monthlyPaymentLabel, 1, 0);

            resultsLayout.Controls.Add(new Label { Text = "Total Interest:", AutoSize = true }, 0, 1);
            resultsLayout.Controls.Add(totalInterestLabel, 1, 1);

            resultsLayout.Controls.Add(new Label { Text = "Total Payable:", AutoSize = true }, 0, 2);
            resultsLayout.Controls.Add(totalPayableLabel, 1, 2);

            resultsLayout.Controls.Add(new Label { Text = "APR:", AutoSize = true }, 0, 3);
            resultsLayout.Controls.Add(aprLabel, 1, 3);

            header.Controls.Add(headerLabel);
            panel.Controls.Add(header);
            panel.Controls.Add(resultsLayout);

            return panel;
        }

        private void WireUpEvents()
        {
            // Credit score calculation
            paymentHistoryInput.ValueChanged += UpdateCreditScore;
            creditUtilizationInput.ValueChanged += UpdateCreditScore;
            creditHistoryLengthInput.ValueChanged += UpdateCreditScore;
            incomeStabilityInput.ValueChanged += UpdateCreditScore;

            // Service fee calculation
            serviceFeeInput.ValueChanged += UpdateServiceFeeAmount;

            // Button events
            generateContractButton.Click += (s, e) => MessageBox.Show("Generating contract preview...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            viewAmortizationButton.Click += (s, e) => MessageBox.Show("Viewing amortization schedule...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            saveAsDraftButton.Click += (s, e) => SaveAsDraft();
            rejectButton.Click += (s, e) => RejectApplication();
            approveButton.Click += (s, e) => ApproveApplication();
        }

        private void UpdateCreditScore(object sender, EventArgs e)
        {
            decimal ph = (paymentHistoryInput.Value / 100) * 35;
            decimal cu = (creditUtilizationInput.Value / 100) * 30;
            decimal chl = (creditHistoryLengthInput.Value / 100) * 15;
            decimal @is = (incomeStabilityInput.Value / 100) * 20;

            decimal totalScore = ph + cu + chl + @is;
            totalCreditScoreLabel.Text = $"{totalScore:F2}/100";

            // Update color based on score
            if (totalScore >= 80)
                totalCreditScoreLabel.ForeColor = Color.Green;
            else if (totalScore >= 60)
                totalCreditScoreLabel.ForeColor = Color.Blue;
            else if (totalScore >= 40)
                totalCreditScoreLabel.ForeColor = Color.Orange;
            else
                totalCreditScoreLabel.ForeColor = Color.Red;
        }

        private void UpdateServiceFeeAmount(object sender, EventArgs e)
        {
            if (currentApplication != null)
            {
                decimal principal = decimal.Parse(currentApplication.Amount.Replace("₱", "").Replace(",", ""));
                decimal serviceFeeAmount = (principal * serviceFeeInput.Value) / 100;
                serviceFeeAmountLabel.Text = $"(₱{serviceFeeAmount:F2})";
            }
        }

        private void RecalculateLoan()
        {
            // For now, just show a message
            MessageBox.Show("Loan recalculated based on new parameters", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Update calculation results (simulated)
            monthlyPaymentLabel.Text = "₱1,329.48";
            totalInterestLabel.Text = "₱953.76";
            totalPayableLabel.Text = "₱16,253.76";
            aprLabel.Text = "13.2%";
        }

        private void UpdateApplicationInfo()
        {
            if (currentApplication != null)
            {
                this.Text = $"Evaluate Loan Application - {currentApplication.Id} (Under Review)";
                UpdateServiceFeeAmount(null, EventArgs.Empty);
                RecalculateLoan();
            }
        }

        private void SaveAsDraft()
        {
            MessageBox.Show("Evaluation saved as draft", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void RejectApplication()
        {
            if (rejectionReasonComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a rejection reason", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to reject this application?", "Confirm Rejection",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Application rejected", "Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void ApproveApplication()
        {
            var result = MessageBox.Show("Are you sure you want to approve this application?", "Confirm Approval",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Application approved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

    // Simple data class to hold application info
    public class ApplicationData
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string LoanType { get; set; }
        public string Amount { get; set; }
        public string AppliedDate { get; set; }
        public string Status { get; set; }
    }

    // Usage example:
    // In your main form, when you want to show this dialog:
    /*
    private void ShowEvaluationDialog()
    {
        var dialog = new OfficerEvaluateApplicationForm
        {
            CurrentApplication = new ApplicationData
            {
                Id = "APP-2024-00123",
                Customer = "John Doe",
                LoanType = "Personal Loan",
                Amount = "₱15,000.00",
                AppliedDate = "2024-01-15",
                Status = "Under Review"
            }
        };
        
        dialog.ShowDialog();
    }
    */
}