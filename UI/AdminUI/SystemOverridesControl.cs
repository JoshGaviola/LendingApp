using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class SystemOverridesControl : UserControl
    {
        // Radio buttons
        private RadioButton bypassCreditRadio;
        private RadioButton disablePenaltyRadio;
        private RadioButton extendMaintenanceRadio;
        private RadioButton maxLoanAmountRadio;
        private RadioButton forceReportRadio;
        private RadioButton adjustInterestRadio;
        private RadioButton freezeCollectionsRadio;
        private RadioButton overrideClosingRadio;

        // Conditional controls - REMOVED Panel references since we're not using them
        private TextBox maintenanceFromTextBox;
        private TextBox maintenanceToTextBox;
        private TextBox maxLoanTextBox;
        private TextBox interestAdjustmentTextBox;

        // Duration controls - REMOVED since not initialized
        private RadioButton permanentRadio;
        private RadioButton temporaryRadio;
        private DateTimePicker temporaryDatePicker;

        // Reason and approval
        private TextBox reasonTextBox;
        private Label charCountLabel;
        private TextBox passwordTextBox;

        // Buttons
        private Button applyButton;
        private Button scheduleButton;
        private Button cancelButton;

        // Track Y positions for dynamic layout
        private int systemGroupY;
        private int financialGroupY;
        private GroupBox systemGroup;
        private GroupBox financialGroup;
        private GroupBox durationGroup;
        private GroupBox reasonGroup;

        // Labels for maintenance time
        private Label fromLabel;
        private Label toLabel;
        private Label newMaxLabel;
        private Label adjustmentLabel;
        private Label percentLabel;

        public SystemOverridesControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            // Main container
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 253, 244), // Light green background
                BorderStyle = BorderStyle.FixedSingle
            };

            int yPos = 20;

            // Header
            Label headerLabel = new Label
            {
                Text = "SYSTEM OVERRIDES",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(headerLabel);
            yPos += 40;

            // System Settings Group - FIXED VERSION
            systemGroup = new GroupBox
            {
                Text = "SYSTEM SETTINGS OVERRIDE:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 250), // Increased height to fit everything
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            int systemY = 25;
            int radioWidth = systemGroup.Width - 30;

            // Bypass Credit Score Check
            bypassCreditRadio = new RadioButton
            {
                Text = "Bypass Credit Score Check",
                Location = new Point(15, systemY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            systemGroup.Controls.Add(bypassCreditRadio);
            systemY += 30;

            // Disable Auto-Penalty
            disablePenaltyRadio = new RadioButton
            {
                Text = "Disable Auto-Penalty Calculation",
                Location = new Point(15, systemY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            systemGroup.Controls.Add(disablePenaltyRadio);
            systemY += 30;

            // Extend Maintenance Window
            extendMaintenanceRadio = new RadioButton
            {
                Text = "Extend System Maintenance Window",
                Location = new Point(15, systemY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            systemGroup.Controls.Add(extendMaintenanceRadio);
            systemY += 30;

            // Maintenance time controls - ALWAYS VISIBLE, ALIGNED PROPERLY
            fromLabel = new Label
            {
                Text = "From:",
                Location = new Point(30, systemY),
                Size = new Size(40, 20),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            maintenanceFromTextBox = new TextBox
            {
                Location = new Point(75, systemY - 2),
                Size = new Size(80, 22),
                Text = "00:00",
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false,
                BackColor = SystemColors.Control
            };

            toLabel = new Label
            {
                Text = "To:",
                Location = new Point(165, systemY),
                Size = new Size(25, 20),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            maintenanceToTextBox = new TextBox
            {
                Location = new Point(195, systemY - 2),
                Size = new Size(80, 22),
                Text = "23:59",
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false,
                BackColor = SystemColors.Control
            };

            systemGroup.Controls.Add(fromLabel);
            systemGroup.Controls.Add(maintenanceFromTextBox);
            systemGroup.Controls.Add(toLabel);
            systemGroup.Controls.Add(maintenanceToTextBox);
            systemY += 35;

            // Enable/disable maintenance controls based on radio selection
            extendMaintenanceRadio.CheckedChanged += (s, e) =>
            {
                bool enabled = extendMaintenanceRadio.Checked;
                maintenanceFromTextBox.Enabled = enabled;
                maintenanceToTextBox.Enabled = enabled;
                fromLabel.Enabled = enabled;
                toLabel.Enabled = enabled;

                if (enabled)
                {
                    maintenanceFromTextBox.BackColor = Color.White;
                    maintenanceToTextBox.BackColor = Color.White;
                    maintenanceFromTextBox.Focus();
                }
                else
                {
                    maintenanceFromTextBox.BackColor = SystemColors.Control;
                    maintenanceToTextBox.BackColor = SystemColors.Control;
                }
            };

            // Override Maximum Loan Amount
            maxLoanAmountRadio = new RadioButton
            {
                Text = "Override Maximum Loan Amount Rule",
                Location = new Point(15, systemY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            systemGroup.Controls.Add(maxLoanAmountRadio);
            systemY += 30;

            // Max loan controls - ALWAYS VISIBLE, ALIGNED PROPERLY
            newMaxLabel = new Label
            {
                Text = "New Max: ₱",
                Location = new Point(30, systemY),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            maxLoanTextBox = new TextBox
            {
                Location = new Point(105, systemY - 2),
                Size = new Size(150, 22),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false,
                BackColor = SystemColors.Control
            };
            AddPlaceholderText(maxLoanTextBox, "Enter new maximum");

            systemGroup.Controls.Add(newMaxLabel);
            systemGroup.Controls.Add(maxLoanTextBox);
            systemY += 35;

            // Enable/disable max loan controls based on radio selection
            maxLoanAmountRadio.CheckedChanged += (s, e) =>
            {
                bool enabled = maxLoanAmountRadio.Checked;
                maxLoanTextBox.Enabled = enabled;
                newMaxLabel.Enabled = enabled;

                if (enabled)
                {
                    maxLoanTextBox.BackColor = Color.White;
                    maxLoanTextBox.Focus();
                }
                else
                {
                    maxLoanTextBox.BackColor = SystemColors.Control;
                }
            };

            // Force Daily Report
            forceReportRadio = new RadioButton
            {
                Text = "Force Daily Report Generation",
                Location = new Point(15, systemY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            systemGroup.Controls.Add(forceReportRadio);

            mainPanel.Controls.Add(systemGroup);
            systemGroupY = yPos;
            yPos += 260; // Increased from 190 to 260 for system group

            // Financial Overrides Group - FIXED VERSION
            financialGroup = new GroupBox
            {
                Text = "FINANCIAL OVERRIDES:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 180), // Increased height
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            int financialY = 25;
            radioWidth = financialGroup.Width - 30;

            // Adjust Interest Rate
            adjustInterestRadio = new RadioButton
            {
                Text = "Adjust Interest Rate for All Active Loans",
                Location = new Point(15, financialY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            financialGroup.Controls.Add(adjustInterestRadio);
            financialY += 30;

            // Interest adjustment controls - ALWAYS VISIBLE
            adjustmentLabel = new Label
            {
                Text = "Adjustment:",
                Location = new Point(30, financialY),
                Size = new Size(65, 20),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            interestAdjustmentTextBox = new TextBox
            {
                Location = new Point(100, financialY - 2),
                Size = new Size(80, 22),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false,
                BackColor = SystemColors.Control
            };
            AddPlaceholderText(interestAdjustmentTextBox, "+/- 0.00");

            percentLabel = new Label
            {
                Text = "%",
                Location = new Point(185, financialY),
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 9),
                Enabled = false
            };

            financialGroup.Controls.Add(adjustmentLabel);
            financialGroup.Controls.Add(interestAdjustmentTextBox);
            financialGroup.Controls.Add(percentLabel);
            financialY += 35;

            // Enable/disable interest controls based on radio selection
            adjustInterestRadio.CheckedChanged += (s, e) =>
            {
                bool enabled = adjustInterestRadio.Checked;
                interestAdjustmentTextBox.Enabled = enabled;
                adjustmentLabel.Enabled = enabled;
                percentLabel.Enabled = enabled;

                if (enabled)
                {
                    interestAdjustmentTextBox.BackColor = Color.White;
                    interestAdjustmentTextBox.Focus();
                }
                else
                {
                    interestAdjustmentTextBox.BackColor = SystemColors.Control;
                }
            };

            // Freeze Collections
            freezeCollectionsRadio = new RadioButton
            {
                Text = "Freeze All Collections",
                Location = new Point(15, financialY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            financialGroup.Controls.Add(freezeCollectionsRadio);
            financialY += 30;

            // Override Closing Time
            overrideClosingRadio = new RadioButton
            {
                Text = "Override Closing Time",
                Location = new Point(15, financialY),
                Size = new Size(radioWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            financialGroup.Controls.Add(overrideClosingRadio);

            mainPanel.Controls.Add(financialGroup);
            financialGroupY = yPos;
            yPos += 190; // Increased from 130 to 190 for financial group

            // Duration Group (Initialize it here)
            durationGroup = new GroupBox
            {
                Text = "DURATION:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 80),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            permanentRadio = new RadioButton
            {
                Text = "Permanent",
                Location = new Point(15, 25),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };

            temporaryRadio = new RadioButton
            {
                Text = "Temporary",
                Location = new Point(100, 25),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9)
            };

            temporaryDatePicker = new DateTimePicker
            {
                Location = new Point(185, 23),
                Size = new Size(120, 22),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            temporaryRadio.CheckedChanged += (s, e) =>
            {
                temporaryDatePicker.Enabled = temporaryRadio.Checked;
                temporaryDatePicker.BackColor = temporaryRadio.Checked ? Color.White : SystemColors.Control;
            };

            durationGroup.Controls.Add(permanentRadio);
            durationGroup.Controls.Add(temporaryRadio);
            durationGroup.Controls.Add(temporaryDatePicker);
            mainPanel.Controls.Add(durationGroup);
            yPos += 90;

            // Reason & Approval Group
            reasonGroup = new GroupBox
            {
                Text = "REASON & APPROVAL:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 160),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            reasonTextBox = new TextBox
            {
                Location = new Point(15, 25),
                Size = new Size(reasonGroup.Width - 30, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            AddPlaceholderText(reasonTextBox, "Enter detailed reason for system override...");

            // Character count label
            charCountLabel = new Label
            {
                Text = "0/500 characters",
                Location = new Point(reasonGroup.Width - 120, 105),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 8),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Gray
            };

            // Update character count
            reasonTextBox.TextChanged += (s, e) =>
            {
                if (reasonTextBox.Text == "Enter detailed reason for system override...")
                {
                    charCountLabel.Text = "0/500 characters";
                }
                else
                {
                    charCountLabel.Text = $"{reasonTextBox.Text.Length}/500 characters";
                }
            };

            Label passwordLabel = new Label
            {
                Text = "Admin Password:",
                Location = new Point(15, 115),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(120, 113),
                Size = new Size(200, 22),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            AddPlaceholderText(passwordTextBox, "Enter admin password");

            reasonGroup.Controls.Add(reasonTextBox);
            reasonGroup.Controls.Add(charCountLabel);
            reasonGroup.Controls.Add(passwordLabel);
            reasonGroup.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(reasonGroup);
            yPos += 170;

            // Action Buttons - LEFT ALIGNED
            applyButton = new Button
            {
                Text = "Apply Override",
                Location = new Point(20, yPos),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(34, 197, 94), // Green color
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            applyButton.FlatAppearance.BorderSize = 0;
            applyButton.Click += ApplyButton_Click;

            scheduleButton = new Button
            {
                Text = "Schedule",
                Location = new Point(150, yPos),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            scheduleButton.FlatAppearance.BorderSize = 0;
            scheduleButton.Click += ScheduleButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, yPos),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            mainPanel.Controls.Add(applyButton);
            mainPanel.Controls.Add(scheduleButton);
            mainPanel.Controls.Add(cancelButton);

            // REMOVED: Wire up radio button events for conditional panels
            // We're not using UpdateConditionalPanels or UpdateGroupLayouts anymore
            // because they reference null panels

            mainPanel.Resize += (s, e) =>
            {
                UpdateLayout(mainPanel.Width);
            };

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void UpdateLayout(int containerWidth)
        {
            int groupWidth = containerWidth - 40;

            // Update all group widths
            foreach (Control control in this.Controls[0].Controls)
            {
                if (control is GroupBox group)
                {
                    group.Width = groupWidth;

                    // Update controls inside groups
                    foreach (Control innerControl in group.Controls)
                    {
                        if (innerControl is RadioButton radio)
                        {
                            radio.Width = groupWidth - 30;
                        }
                        else if (innerControl is TextBox textBox && textBox.Multiline)
                        {
                            textBox.Width = groupWidth - 30;
                        }
                        else if (innerControl is Label label && label.Text.Contains("characters"))
                        {
                            label.Left = groupWidth - 120;
                        }
                    }
                }
                else if (control is Button)
                {
                    // KEEP BUTTONS LEFT ALIGNED
                    applyButton.Left = 20;
                    scheduleButton.Left = 150;
                    cancelButton.Left = 260;
                }
            }
        }

        private void AddPlaceholderText(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Apply override functionality", "Info",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ScheduleButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Schedule override functionality coming soon", "Info",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Reset form
            bypassCreditRadio.Checked = true;
            
            // Reset maintenance fields
            extendMaintenanceRadio.Checked = false;
            maintenanceFromTextBox.Text = "00:00";
            maintenanceToTextBox.Text = "23:59";
            maintenanceFromTextBox.BackColor = SystemColors.Control;
            maintenanceToTextBox.BackColor = SystemColors.Control;
            fromLabel.Enabled = false;
            toLabel.Enabled = false;
            
            // Reset max loan fields
            maxLoanAmountRadio.Checked = false;
            maxLoanTextBox.Text = "Enter new maximum";
            maxLoanTextBox.ForeColor = Color.Gray;
            maxLoanTextBox.BackColor = SystemColors.Control;
            newMaxLabel.Enabled = false;
            
            // Reset interest fields (but keep checked since it's the default in financial group)
            adjustInterestRadio.Checked = true;
            interestAdjustmentTextBox.Text = "+/- 0.00";
            interestAdjustmentTextBox.ForeColor = Color.Gray;
            interestAdjustmentTextBox.BackColor = Color.White; // It's enabled because radio is checked
            adjustmentLabel.Enabled = true;
            percentLabel.Enabled = true;
            
            // Reset duration
            permanentRadio.Checked = true;
            temporaryDatePicker.Enabled = false;
            temporaryDatePicker.BackColor = SystemColors.Control;
            
            // Reset reason and password
            reasonTextBox.Text = "Enter detailed reason for system override...";
            reasonTextBox.ForeColor = Color.Gray;
            passwordTextBox.Text = "Enter admin password";
            passwordTextBox.ForeColor = Color.Gray;
            charCountLabel.Text = "0/500 characters";

            MessageBox.Show("System override cancelled", "Info",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}