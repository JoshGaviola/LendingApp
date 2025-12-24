using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI
{
    public partial class ConfigureLoanRulesControl : UserControl
    {
        // Form fields
        private ComboBox cmbLoanType;
        private RadioButton rdoDiminishingBalance;
        private RadioButton rdoFlatRate;
        private RadioButton rdoAddOnRate;
        private RadioButton rdoCompoundInterest;
        private TextBox txtPaymentHistWeight;
        private TextBox txtPaymentHistMinScore;
        private TextBox txtCreditUtilWeight;
        private TextBox txtCreditUtilMinScore;
        private TextBox txtLengthHistWeight;
        private TextBox txtLengthHistMinScore;
        private TextBox txtIncomeStabWeight;
        private TextBox txtIncomeStabMinScore;
        private TextBox txtMinApprovalScore;
        private RadioButton rdoApprovalOfficer;
        private RadioButton rdoApprovalSupervisor;
        private RadioButton rdoApprovalManager;
        private TextBox txtPenaltyRate;
        private RadioButton rdoApplyAfterGraceYes;
        private RadioButton rdoApplyAfterGraceNo;
        private TextBox txtPenaltyCap;
        private TextBox txtMaxRestructures;
        private TextBox txtMinDaysDelayed;
        private Button btnSaveRules;
        private Button btnResetToDefault;

        // For total weight calculation
        private Label lblTotalWeight;

        // Reference to panels for resize handling
        private Panel mainPanel;
        private Panel contentPanel;
        private Panel panel1, panel2, panel3, panel4, panel5;
        private Panel headerPanel;
        private Panel buttonsPanel;
        private Label lblSettingsIcon;
        private Panel tablePanel;
        private Panel workflowTable;

        public ConfigureLoanRulesControl()
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            // Control settings
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Segoe UI", 9);
            this.AutoScroll = true;

            // Main container
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Create content panel
            contentPanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Width = mainPanel.Width - 40,
                MinimumSize = new Size(800, 0) // Set minimum width
            };

            int yPos = 10;

            // ===== HEADER =====
            headerPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 20, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 245, 255) // Light purple background
            };

            // Add purple border effect
            var borderPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(headerPanel.Width, 3),
                BackColor = Color.Purple
            };
            headerPanel.Controls.Add(borderPanel);

            var lblHeader = new Label
            {
                Text = "CONFIGURE LOAN RULES",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 20),
                AutoSize = true,
                ForeColor = Color.DarkSlateBlue
            };

            // Settings icon (using text symbol)
            lblSettingsIcon = new Label
            {
                Text = "⚙",
                Font = new Font("Segoe UI", 14),
                Location = new Point(headerPanel.Width - 40, 20),
                Size = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Purple
            };

            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(lblSettingsIcon);
            contentPanel.Controls.Add(headerPanel);

            yPos += 70;

            // ===== SELECT LOAN TYPE =====
            var lblLoanType = new Label
            {
                Text = "Select Loan Type:",
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblLoanType);

            cmbLoanType = new ComboBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLoanType.Items.AddRange(new object[] {
                "Personal Loan",
                "Emergency Loan",
                "Salary Loan",
                "Business Loan",
                "Educational Loan"
            });
            cmbLoanType.SelectedIndex = 0;
            contentPanel.Controls.Add(cmbLoanType);

            yPos += 40;

            // ===== 1. INTEREST CALCULATION METHOD =====
            panel1 = CreateSectionPanel("1. INTEREST CALCULATION METHOD", 10, yPos, contentPanel.Width - 40, 100);
            contentPanel.Controls.Add(panel1);
            yPos += 110;

            rdoDiminishingBalance = new RadioButton
            {
                Text = "Diminishing Balance",
                Location = new Point(20, 40),
                AutoSize = true,
                Checked = true
            };
            panel1.Controls.Add(rdoDiminishingBalance);

            rdoFlatRate = new RadioButton
            {
                Text = "Flat Rate",
                Location = new Point(200, 40),
                AutoSize = true
            };
            panel1.Controls.Add(rdoFlatRate);

            rdoAddOnRate = new RadioButton
            {
                Text = "Add-On Rate",
                Location = new Point(20, 70),
                AutoSize = true
            };
            panel1.Controls.Add(rdoAddOnRate);

            rdoCompoundInterest = new RadioButton
            {
                Text = "Compound Interest",
                Location = new Point(200, 70),
                AutoSize = true
            };
            panel1.Controls.Add(rdoCompoundInterest);

            // ===== 2. CREDIT SCORING RULES =====
            panel2 = CreateSectionPanel("2. CREDIT SCORING RULES", 10, yPos, contentPanel.Width - 40, 300);
            contentPanel.Controls.Add(panel2);
            yPos += 310;

            // Credit Scoring Table
            tablePanel = new Panel
            {
                Location = new Point(10, 40),
                Size = new Size(panel2.Width - 20, 160),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Table headers
            var lblFactorHeader = new Label
            {
                Text = "Factor",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblFactorHeader);

            var lblWeightHeader = new Label
            {
                Text = "Weight (%)",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(150, 10),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblWeightHeader);

            var lblMinScoreHeader = new Label
            {
                Text = "Min Score",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(250, 10),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblMinScoreHeader);

            // Payment History row
            var lblPaymentHist = new Label
            {
                Text = "Payment History",
                Location = new Point(10, 40),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblPaymentHist);

            txtPaymentHistWeight = new TextBox
            {
                Location = new Point(150, 37),
                Size = new Size(80, 25),
                Text = "35"
            };
            tablePanel.Controls.Add(txtPaymentHistWeight);

            txtPaymentHistMinScore = new TextBox
            {
                Location = new Point(250, 37),
                Size = new Size(80, 25),
                Text = "60"
            };
            tablePanel.Controls.Add(txtPaymentHistMinScore);

            // Credit Utilization row
            var lblCreditUtil = new Label
            {
                Text = "Credit Utilization",
                Location = new Point(10, 70),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblCreditUtil);

            txtCreditUtilWeight = new TextBox
            {
                Location = new Point(150, 67),
                Size = new Size(80, 25),
                Text = "30"
            };
            tablePanel.Controls.Add(txtCreditUtilWeight);

            txtCreditUtilMinScore = new TextBox
            {
                Location = new Point(250, 67),
                Size = new Size(80, 25),
                Text = "50"
            };
            tablePanel.Controls.Add(txtCreditUtilMinScore);

            // Length History row
            var lblLengthHist = new Label
            {
                Text = "Length History",
                Location = new Point(10, 100),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblLengthHist);

            txtLengthHistWeight = new TextBox
            {
                Location = new Point(150, 97),
                Size = new Size(80, 25),
                Text = "15"
            };
            tablePanel.Controls.Add(txtLengthHistWeight);

            txtLengthHistMinScore = new TextBox
            {
                Location = new Point(250, 97),
                Size = new Size(80, 25),
                Text = "40"
            };
            tablePanel.Controls.Add(txtLengthHistMinScore);

            // Income Stability row
            var lblIncomeStab = new Label
            {
                Text = "Income Stability",
                Location = new Point(10, 130),
                AutoSize = true
            };
            tablePanel.Controls.Add(lblIncomeStab);

            txtIncomeStabWeight = new TextBox
            {
                Location = new Point(150, 127),
                Size = new Size(80, 25),
                Text = "20"
            };
            tablePanel.Controls.Add(txtIncomeStabWeight);

            txtIncomeStabMinScore = new TextBox
            {
                Location = new Point(250, 127),
                Size = new Size(80, 25),
                Text = "55"
            };
            tablePanel.Controls.Add(txtIncomeStabMinScore);

            panel2.Controls.Add(tablePanel);

            // Total Weight
            lblTotalWeight = new Label
            {
                Text = "Total Weight Must Equal 100%: 100%",
                Location = new Point(10, 220),
                AutoSize = true,
                ForeColor = Color.Green
            };
            panel2.Controls.Add(lblTotalWeight);

            // Minimum Approval Score
            var lblMinApprovalScore = new Label
            {
                Text = "Minimum Approval Score: (points)",
                Location = new Point(10, 240),
                AutoSize = true
            };
            panel2.Controls.Add(lblMinApprovalScore);
            txtMinApprovalScore = new TextBox
            {
                Location = new Point(15, 260),
                Size = new Size(100, 25),
                Text = "70"
            };
            panel2.Controls.Add(txtMinApprovalScore);

            // Wire up weight calculation
            txtPaymentHistWeight.TextChanged += CalculateTotalWeight;
            txtCreditUtilWeight.TextChanged += CalculateTotalWeight;
            txtLengthHistWeight.TextChanged += CalculateTotalWeight;
            txtIncomeStabWeight.TextChanged += CalculateTotalWeight;

            // ===== 3. APPROVAL WORKFLOW =====
            panel3 = CreateSectionPanel("3. APPROVAL WORKFLOW (by loan amount)", 10, yPos, contentPanel.Width - 40, 150);
            contentPanel.Controls.Add(panel3);
            yPos += 160;

            // Approval workflow table
            workflowTable = new Panel
            {
                Location = new Point(10, 40),
                Size = new Size(panel3.Width - 20, 90),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Row 1: ≤ ₱50,000
            var lblAmount1 = new Label
            {
                Text = "≤ ₱50,000",
                Location = new Point(10, 10),
                AutoSize = true
            };
            workflowTable.Controls.Add(lblAmount1);

            rdoApprovalOfficer = new RadioButton
            {
                Text = "Loan Officer",
                Location = new Point(150, 10),
                AutoSize = true,
                Checked = true
            };
            workflowTable.Controls.Add(rdoApprovalOfficer);

            // Row 2: ₱50k-200k
            var lblAmount2 = new Label
            {
                Text = "₱50k-200k",
                Location = new Point(10, 40),
                AutoSize = true
            };
            workflowTable.Controls.Add(lblAmount2);

            rdoApprovalSupervisor = new RadioButton
            {
                Text = "Officer + Supervisor",
                Location = new Point(150, 40),
                AutoSize = true
            };
            workflowTable.Controls.Add(rdoApprovalSupervisor);

            // Row 3: ≥ ₱200,000
            var lblAmount3 = new Label
            {
                Text = "≥ ₱200,000",
                Location = new Point(10, 70),
                AutoSize = true
            };
            workflowTable.Controls.Add(lblAmount3);

            rdoApprovalManager = new RadioButton
            {
                Text = "Officer + Supervisor + Manager",
                Location = new Point(150, 70),
                AutoSize = true
            };
            workflowTable.Controls.Add(rdoApprovalManager);

            panel3.Controls.Add(workflowTable);

            // ===== 4. PENALTY CALCULATION RULES =====
            panel4 = CreateSectionPanel("4. PENALTY CALCULATION RULES", 10, yPos, contentPanel.Width - 40, 150);
            contentPanel.Controls.Add(panel4);
            yPos += 160;

            // Formula
            var lblFormula = new Label
            {
                Text = "Formula: [Overdue Amount] ×",
                Location = new Point(10, 40),
                AutoSize = true
            };
            panel4.Controls.Add(lblFormula);

            txtPenaltyRate = new TextBox
            {
                Location = new Point(180, 37),
                Size = new Size(60, 25),
                Text = "0.5"
            };
            panel4.Controls.Add(txtPenaltyRate);

            var lblFormula2 = new Label
            {
                Text = "% × [Days Late]",
                Location = new Point(250, 40),
                AutoSize = true
            };
            panel4.Controls.Add(lblFormula2);

            // Apply After Grace Period?
            var lblApplyGrace = new Label
            {
                Text = "Apply After Grace Period?",
                Location = new Point(10, 75),
                AutoSize = true
            };
            panel4.Controls.Add(lblApplyGrace);

            rdoApplyAfterGraceYes = new RadioButton
            {
                Text = "Yes",
                Location = new Point(180, 75),
                AutoSize = true,
                Checked = true
            };
            panel4.Controls.Add(rdoApplyAfterGraceYes);

            rdoApplyAfterGraceNo = new RadioButton
            {
                Text = "No",
                Location = new Point(240, 75),
                AutoSize = true
            };
            panel4.Controls.Add(rdoApplyAfterGraceNo);

            // Penalty Cap
            var lblPenaltyCap = new Label
            {
                Text = "Cap Penalty at: (% of Principal)",
                Location = new Point(10, 105),
                AutoSize = true
            };
            panel4.Controls.Add(lblPenaltyCap);

            txtPenaltyCap = new TextBox
            {
                Location = new Point(180, 102),
                Size = new Size(80, 25),
                Text = "10"
            };
            panel4.Controls.Add(txtPenaltyCap);

            // ===== 5. LOAN RESTRUCTURING RULES =====
            panel5 = CreateSectionPanel("5. LOAN RESTRUCTURING RULES", 10, yPos, contentPanel.Width - 40, 100);
            contentPanel.Controls.Add(panel5);
            yPos += 110;

            // Max Restructures
            var lblMaxRestructures = new Label
            {
                Text = "Max Restructures Allowed: (times)",
                Location = new Point(10, 40),
                AutoSize = true
            };
            panel5.Controls.Add(lblMaxRestructures);

            txtMaxRestructures = new TextBox
            {
                Location = new Point(200, 37),
                Size = new Size(80, 25),
                Text = "3"
            };
            panel5.Controls.Add(txtMaxRestructures);

            // Min Days Delayed
            var lblMinDaysDelayed = new Label
            {
                Text = "Minimum Days Delayed for Restructure:",
                Location = new Point(10, 75),
                AutoSize = true
            };
            panel5.Controls.Add(lblMinDaysDelayed);

            txtMinDaysDelayed = new TextBox
            {
                Location = new Point(230, 72),
                Size = new Size(80, 25),
                Text = "30"
            };
            panel5.Controls.Add(txtMinDaysDelayed);

            // ===== ACTION BUTTONS =====
            buttonsPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSaveRules = new Button
            {
                Text = "Save Rules",
                Location = new Point(20, 15),
                Size = new Size(120, 35),
                BackColor = Color.Purple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveRules.FlatAppearance.BorderSize = 0;
            btnSaveRules.Click += (s, e) => SaveRules();
            buttonsPanel.Controls.Add(btnSaveRules);

            btnResetToDefault = new Button
            {
                Text = "Reset to Default",
                Location = new Point(160, 15),
                Size = new Size(120, 35),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnResetToDefault.FlatAppearance.BorderColor = Color.Gray;
            btnResetToDefault.FlatAppearance.BorderSize = 1;
            btnResetToDefault.Click += (s, e) => ResetToDefault();
            buttonsPanel.Controls.Add(btnResetToDefault);

            contentPanel.Controls.Add(buttonsPanel);
            yPos += 70;

            // Update content panel height
            contentPanel.Height = yPos + 20;

            // Handle resize
            this.Resize += (s, e) => HandleResize();

            // Initial resize to set proper widths
            HandleResize();

            // Add content panel to main panel
            mainPanel.Controls.Add(contentPanel);

            // Add main panel to user control
            this.Controls.Add(mainPanel);
        }

        private Panel CreateSectionPanel(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BorderStyle = BorderStyle.FixedSingle,
                MinimumSize = new Size(400, height) // Set minimum width
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            panel.Controls.Add(lblTitle);

            return panel;
        }

        private void HandleResize()
        {
            if (contentPanel == null) return;

            int newWidth = Math.Max(820, mainPanel.Width - 40);
            contentPanel.Width = newWidth;

            // Update panel widths
            UpdatePanelWidth(headerPanel, newWidth - 20);
            UpdatePanelWidth(panel1, newWidth - 40);
            UpdatePanelWidth(panel2, newWidth - 40);
            UpdatePanelWidth(panel3, newWidth - 40);
            UpdatePanelWidth(panel4, newWidth - 40);
            UpdatePanelWidth(panel5, newWidth - 40);
            UpdatePanelWidth(buttonsPanel, newWidth - 40);

            // Update inner table panel widths
            if (tablePanel != null)
            {
                tablePanel.Width = panel2.Width - 20;

                // Adjust column positions for wider table
                if (tablePanel.Width > 350)
                {
                    // Move weight column further right
                    var weightHeader = tablePanel.Controls[1] as Label;
                    if (weightHeader != null) weightHeader.Left = 180;

                    // Move min score column further right  
                    var minScoreHeader = tablePanel.Controls[2] as Label;
                    if (minScoreHeader != null) minScoreHeader.Left = 280;

                    // Adjust textbox positions
                    txtPaymentHistWeight.Left = 180;
                    txtPaymentHistMinScore.Left = 280;
                    txtCreditUtilWeight.Left = 180;
                    txtCreditUtilMinScore.Left = 280;
                    txtLengthHistWeight.Left = 180;
                    txtLengthHistMinScore.Left = 280;
                    txtIncomeStabWeight.Left = 180;
                    txtIncomeStabMinScore.Left = 280;
                }
            }

            // Update workflow table width
            if (workflowTable != null)
            {
                workflowTable.Width = panel3.Width - 20;

                // Adjust radio button positions for wider table
                if (workflowTable.Width > 350)
                {
                    rdoApprovalOfficer.Left = 180;
                    rdoApprovalSupervisor.Left = 180;
                    rdoApprovalManager.Left = 180;
                }
            }

            // Update settings icon position
            if (lblSettingsIcon != null)
            {
                lblSettingsIcon.Left = (newWidth - 20) - 40;
            }
        }

        private void UpdatePanelWidth(Panel panel, int width)
        {
            if (panel != null)
            {
                panel.Width = width;
            }
        }

        private void CalculateTotalWeight(object sender, EventArgs e)
        {
            try
            {
                double total = 0;

                if (double.TryParse(txtPaymentHistWeight.Text, out double weight1))
                    total += weight1;
                if (double.TryParse(txtCreditUtilWeight.Text, out double weight2))
                    total += weight2;
                if (double.TryParse(txtLengthHistWeight.Text, out double weight3))
                    total += weight3;
                if (double.TryParse(txtIncomeStabWeight.Text, out double weight4))
                    total += weight4;

                lblTotalWeight.Text = $"Total Weight Must Equal 100%: {total}%";
                lblTotalWeight.ForeColor = Math.Abs(total - 100) < 0.01 ? Color.Green : Color.Red;
            }
            catch
            {
                lblTotalWeight.Text = "Total Weight Must Equal 100%: Error";
                lblTotalWeight.ForeColor = Color.Red;
            }
        }

        private void SaveRules()
        {
            MessageBox.Show("Rules saved successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetToDefault()
        {
            // Reset all fields to default values
            cmbLoanType.SelectedIndex = 0;
            rdoDiminishingBalance.Checked = true;

            txtPaymentHistWeight.Text = "35";
            txtPaymentHistMinScore.Text = "60";
            txtCreditUtilWeight.Text = "30";
            txtCreditUtilMinScore.Text = "50";
            txtLengthHistWeight.Text = "15";
            txtLengthHistMinScore.Text = "40";
            txtIncomeStabWeight.Text = "20";
            txtIncomeStabMinScore.Text = "55";
            txtMinApprovalScore.Text = "70";

            rdoApprovalOfficer.Checked = true;

            txtPenaltyRate.Text = "0.5";
            rdoApplyAfterGraceYes.Checked = true;
            txtPenaltyCap.Text = "10";

            txtMaxRestructures.Text = "3";
            txtMinDaysDelayed.Text = "30";

            CalculateTotalWeight(null, EventArgs.Empty);
        }
    }
}