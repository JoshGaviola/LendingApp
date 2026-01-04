using System;
using System.Collections.Generic;
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

        // NEW: labels so we can update the displayed weights per customer type
        private Label lblC1;
        private Label lblC2;
        private Label lblC3;
        private Label lblC4;

        // NEW: runtime weights (percent)
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

        /// <summary>
        /// Sets the credit assessment calculator defaults based on customer type.
        /// </summary>
        public void ApplyDefaultsForCustomerType(string customerType)
        {
            var type = (customerType ?? "").Trim();

            if (type.Equals("New", StringComparison.OrdinalIgnoreCase))
            {
                // NEW CUSTOMER SCORING (0-100)
                // 1. DOCUMENT VERIFICATION & IDENTITY (40%)
                // 2. EMPLOYMENT & INCOME STABILITY (30%)
                // 3. PERSONAL PROFILE & STABILITY (20%)
                // 4. INITIAL RELATIONSHIP & DEPOSIT (10%)

                // Update runtime weights
                _w1 = 40m;
                _w2 = 30m;
                _w3 = 20m;
                _w4 = 10m;

                // Update labels (meaning + weights)
                if (lblC1 != null) lblC1.Text = "Document Verification & Identity (40%):";
                if (lblC2 != null) lblC2.Text = "Employment & Income Stability (30%):";
                if (lblC3 != null) lblC3.Text = "Personal Profile & Stability (20%):";
                if (lblC4 != null) lblC4.Text = "Initial Relationship & Deposit (10%):";

                // Defaults for a "New" customer (you can tune these later)
                paymentHistoryInput.Value = 80;
                creditUtilizationInput.Value = 50;
                creditHistoryLengthInput.Value = 60;
                incomeStabilityInput.Value = 70;

                UpdateCreditScore(this, EventArgs.Empty);
                return;
            }

            // Default scoring (existing calculator meaning)
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
            this.Text = "Evaluate Loan Application";
            this.Size = new Size(900, 700); // Fixed size, not too large
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new Padding(10);
            this.BackColor = Color.White;
        }

        private void InitializeControls()
        {
            // Initialize main container for scrolling
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
                Width = 180
            };

            // Initialize label references
            lblC1 = new Label { Text = "Payment History (35%):", AutoSize = true, Width = 250 };
            lblC2 = new Label { Text = "Credit Utilization (30%):", AutoSize = true, Width = 250 };
            lblC3 = new Label { Text = "Credit History Length (15%):", AutoSize = true, Width = 250 };
            lblC4 = new Label { Text = "Income Stability (20%):", AutoSize = true, Width = 250 };

            // Loan computation controls
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

            // Approval workflow controls
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
            remarksTextBox.Enter += (s, e) => {
                if (remarksTextBox.Text == "Enter evaluation remarks...")
                {
                    remarksTextBox.Text = "";
                    remarksTextBox.ForeColor = SystemColors.WindowText;
                }
            };
            remarksTextBox.Leave += (s, e) => {
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
        }

        private void SetupLayout()
        {
            // Add main container to form
            this.Controls.Add(mainContainer);

            var mainLayout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Padding = new Padding(5)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Credit Assessment Section
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

            // Add rows using the label references
            AddCreditRow(creditLayout, 0, lblC1, paymentHistoryInput);
            AddCreditRow(creditLayout, 1, lblC2, creditUtilizationInput);
            AddCreditRow(creditLayout, 2, lblC3, creditHistoryLengthInput);
            AddCreditRow(creditLayout, 3, lblC4, incomeStabilityInput);

            // Total score row
            creditLayout.Controls.Add(new Label
            {
                Text = "TOTAL CREDIT SCORE:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            }, 0, 4);
            creditLayout.Controls.Add(totalCreditScoreLabel, 1, 4);
            creditLayout.SetColumnSpan(totalCreditScoreLabel, 3);

            // Decision row
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

            // Loan Computation Section (REPLACE the whole loanLayout building block)
            var loanLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(10),
                AutoSize = true,
                Width = 800
            };

            loanLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            loanLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Rate row
            AddLoanRow(loanLayout, 0, "Rate:", interestRateInput, new Label { Text = "% p.a.", AutoSize = true });

            // Service fee row (with peso amount)
            loanLayout.Controls.Add(new Label { Text = "Service Fee:", AutoSize = true }, 0, 1);
            var serviceFeePanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            serviceFeePanel.Controls.Add(serviceFeeInput);
            serviceFeePanel.Controls.Add(new Label { Text = "%", Margin = new Padding(5, 3, 0, 0), AutoSize = true });
            serviceFeePanel.Controls.Add(serviceFeeAmountLabel);
            loanLayout.Controls.Add(serviceFeePanel, 1, 1);

            // Results panel
            var resultsPanel = CreateResultsPanel();
            loanLayout.SetColumnSpan(resultsPanel, 2);
            loanLayout.Controls.Add(resultsPanel, 0, 2);

            // Adjust terms row (term + recalc)
            loanLayout.Controls.Add(new Label
            {
                Text = "Adjust Terms:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            }, 0, 3);

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
            loanLayout.Controls.Add(termsPanel, 1, 3);

            loanComputationPanel.Controls.Clear();
            loanComputationPanel.Controls.Add(loanLayout);

            // Approval Workflow Section
            var approvalLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 8,
                Padding = new Padding(10),
                AutoSize = true,
                Width = 800
            };

            // Approval Level
            approvalLayout.Controls.Add(new Label
            {
                Text = "Approval Level Required:",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            }, 0, 0);
            approvalLayout.Controls.Add(approvalLevelComboBox, 0, 1);

            // Conditions
            approvalLayout.Controls.Add(new Label
            {
                Text = "Conditions:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            }, 0, 2);

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

            // Rejection Reason
            approvalLayout.Controls.Add(new Label
            {
                Text = "Rejection Reason:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            }, 0, 4);
            approvalLayout.Controls.Add(rejectionReasonComboBox, 0, 5);

            // Remarks
            approvalLayout.Controls.Add(new Label
            {
                Text = "Remarks:",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            }, 0, 6);
            approvalLayout.Controls.Add(remarksTextBox, 0, 7);

            approvalWorkflowPanel.Controls.Add(approvalLayout);

            // Additional Action Buttons
            var actionButtonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(10, 10, 10, 10),
                Margin = new Padding(0, 10, 0, 0)
            };
            actionButtonsPanel.Controls.Add(generateContractButton);
            actionButtonsPanel.Controls.Add(viewAmortizationButton);

            // Footer Buttons
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

            // Add everything to main layout
            mainLayout.Controls.Add(creditAssessmentPanel, 0, 0);
            mainLayout.Controls.Add(loanComputationPanel, 0, 1);
            mainLayout.Controls.Add(approvalWorkflowPanel, 0, 2);
            mainLayout.Controls.Add(actionButtonsPanel, 0, 3);
            mainLayout.Controls.Add(footerPanel, 0, 4);

            // Add main layout to container
            mainContainer.Controls.Add(mainLayout);
        }

        // add field near other fields
        private readonly Dictionary<NumericUpDown, Label> _scoreLabels = new Dictionary<NumericUpDown, Label>();

        // REPLACE AddCreditRow with this version
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
                var flowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true
                };
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

            if (width > 0)
                button.Width = width;

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

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 15,
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

            resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            resultsLayout.Controls.Add(new Label
            {
                Text = "Monthly Payment:",
                AutoSize = true,
                Margin = new Padding(0, 2, 10, 2)
            }, 0, 0);
            resultsLayout.Controls.Add(monthlyPaymentLabel, 1, 0);

            resultsLayout.Controls.Add(new Label
            {
                Text = "Total Interest:",
                AutoSize = true,
                Margin = new Padding(0, 2, 10, 2)
            }, 0, 1);
            resultsLayout.Controls.Add(totalInterestLabel, 1, 1);

            resultsLayout.Controls.Add(new Label
            {
                Text = "Total Payable:",
                AutoSize = true,
                Margin = new Padding(0, 2, 10, 2)
            }, 0, 2);
            resultsLayout.Controls.Add(totalPayableLabel, 1, 2);

            resultsLayout.Controls.Add(new Label
            {
                Text = "APR:",
                AutoSize = true,
                Margin = new Padding(0, 2, 10, 2)
            }, 0, 3);
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
        }

        private void UpdateServiceFeeAmount(object sender, EventArgs e)
        {
            if (currentApplication != null)
            {
                try
                {
                    decimal principal = decimal.Parse(currentApplication.Amount.Replace("₱", "").Replace(",", ""));
                    decimal serviceFeeAmount = (principal * serviceFeeInput.Value) / 100;
                    serviceFeeAmountLabel.Text = $"(₱{serviceFeeAmount:F2})";
                }
                catch
                {
                    serviceFeeAmountLabel.Text = "(₱0.00)";
                }
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
}