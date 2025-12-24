using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class UserActionsControl : UserControl
    {
        // Controls
        private ComboBox userComboBox;
        private CheckBox unlockAccountCheckBox;
        private CheckBox resetPasswordCheckBox;
        private CheckBox extendSessionCheckBox;
        private CheckBox increaseLimitCheckBox;
        private TextBox newLimitTextBox;
        private CheckBox grantAdminCheckBox;
        private CheckBox bypassTwoFactorCheckBox;
        private TextBox reasonTextBox;
        private Label charCountLabel;
        private TextBox passwordTextBox;
        private CheckBox secondApprovalCheckBox;
        private Button executeButton;
        private Button viewLogsButton;

        // GroupBox references for resize handling
        private GroupBox overridesGroup;
        private GroupBox reasonGroup;
        private GroupBox verificationGroup;

        public UserActionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            // Main container panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 249, 255), // Light blue background
                BorderStyle = BorderStyle.FixedSingle
            };

            int yPos = 20;

            // Header
            Label headerLabel = new Label
            {
                Text = "USER ACTIONS OVERRIDE",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(headerLabel);
            yPos += 40;

            // Select User
            Label selectUserLabel = new Label
            {
                Text = "Select User:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70)
            };
            mainPanel.Controls.Add(selectUserLabel);

            userComboBox = new ComboBox
            {
                Location = new Point(130, yPos - 3),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            // Add sample users
            userComboBox.Items.Add("John Doe - Loan Officer");
            userComboBox.Items.Add("Sarah Lee - Supervisor");
            userComboBox.Items.Add("Mike Johnson - Cashier");
            userComboBox.Items.Add("Anna Garcia - Loan Officer");
            userComboBox.Items.Add("Carlos Rodriguez - Manager");

            if (userComboBox.Items.Count > 0)
                userComboBox.SelectedIndex = 0;

            mainPanel.Controls.Add(userComboBox);
            yPos += 40;

            // Available Overrides Group - WILL STRETCH TO FILL WIDTH
            overridesGroup = new GroupBox
            {
                Text = "AVAILABLE OVERRIDES:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 250),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            int checkBoxY = 25;
            int checkBoxWidth = overridesGroup.Width - 30; // Dynamic width

            // Unlock Account
            unlockAccountCheckBox = new CheckBox
            {
                Text = "Unlock User Account (if locked)",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(unlockAccountCheckBox);
            checkBoxY += 30;

            // Reset Password
            resetPasswordCheckBox = new CheckBox
            {
                Text = "Reset Password",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(resetPasswordCheckBox);
            checkBoxY += 30;

            // Extend Session
            extendSessionCheckBox = new CheckBox
            {
                Text = "Extend Session Timeout",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(extendSessionCheckBox);
            checkBoxY += 30;

            // Increase Approval Limit
            increaseLimitCheckBox = new CheckBox
            {
                Text = "Increase Approval Limit (current: ₱50,000)",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(increaseLimitCheckBox);
            checkBoxY += 30;

            // CONTAINER FOR NEW LIMIT - Initially invisible
            Panel limitContainer = new Panel
            {
                Location = new Point(30, checkBoxY),
                Size = new Size(250, 30),
                Visible = false
            };

            Label pesoLabel = new Label
            {
                Text = "₱",
                Location = new Point(0, 5),
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 9)
            };

            newLimitTextBox = new TextBox
            {
                Location = new Point(20, 3),
                Size = new Size(150, 22),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Text = "" // Start with empty text
            };
            AddPlaceholderText(newLimitTextBox, "Enter new limit");

            limitContainer.Controls.Add(pesoLabel);
            limitContainer.Controls.Add(newLimitTextBox);
            overridesGroup.Controls.Add(limitContainer);
            checkBoxY += 35;

            // Grant Admin Rights
            grantAdminCheckBox = new CheckBox
            {
                Text = "Grant Temporary Admin Rights (24 hours)",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(grantAdminCheckBox);
            checkBoxY += 30;

            // Bypass Two-Factor
            bypassTwoFactorCheckBox = new CheckBox
            {
                Text = "Bypass Two-Factor Authentication",
                Location = new Point(15, checkBoxY),
                Size = new Size(checkBoxWidth, 20),
                Font = new Font("Segoe UI", 9)
            };
            overridesGroup.Controls.Add(bypassTwoFactorCheckBox);

            // Show/hide new limit container when checkbox is checked
            increaseLimitCheckBox.CheckedChanged += (s, e) =>
            {
                limitContainer.Visible = increaseLimitCheckBox.Checked;
                if (increaseLimitCheckBox.Checked)
                {
                    newLimitTextBox.BackColor = Color.White;
                    newLimitTextBox.Focus();
                }
            };

            mainPanel.Controls.Add(overridesGroup);
            yPos += 270;

            // Override Reason Group - WILL STRETCH TO FILL WIDTH
            reasonGroup = new GroupBox
            {
                Text = "OVERRIDE REASON:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            reasonTextBox = new TextBox
            {
                Location = new Point(15, 25),
                Size = new Size(reasonGroup.Width - 30, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            AddPlaceholderText(reasonTextBox, "Enter detailed reason for user action override...");

            // Character count label
            charCountLabel = new Label
            {
                Text = "0/500 characters",
                Location = new Point(reasonGroup.Width - 120, 85),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 8),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Gray
            };

            // Update character count as user types
            reasonTextBox.TextChanged += (s, e) =>
            {
                if (reasonTextBox.Text == "Enter detailed reason for user action override...")
                {
                    charCountLabel.Text = "0/500 characters";
                }
                else
                {
                    charCountLabel.Text = $"{reasonTextBox.Text.Length}/500 characters";
                }
            };

            reasonGroup.Controls.Add(reasonTextBox);
            reasonGroup.Controls.Add(charCountLabel);
            mainPanel.Controls.Add(reasonGroup);
            yPos += 140;

            // Admin Verification Group - WILL STRETCH TO FILL WIDTH
            verificationGroup = new GroupBox
            {
                Text = "ADMIN VERIFICATION:",
                Location = new Point(20, yPos),
                Size = new Size(mainPanel.Width - 40, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            // Password
            Label passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(15, 30),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 9)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(90, 28),
                Size = new Size(verificationGroup.Width - 110, 22), // Dynamic width
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            AddPlaceholderText(passwordTextBox, "Enter admin password");

            // Second approval checkbox
            secondApprovalCheckBox = new CheckBox
            {
                Text = "Require Second Admin Approval",
                Location = new Point(15, 60),
                Size = new Size(verificationGroup.Width - 30, 20), // Dynamic width
                Font = new Font("Segoe UI", 9)
            };

            verificationGroup.Controls.Add(passwordLabel);
            verificationGroup.Controls.Add(passwordTextBox);
            verificationGroup.Controls.Add(secondApprovalCheckBox);
            mainPanel.Controls.Add(verificationGroup);
            yPos += 120;

            // Action Buttons
            executeButton = new Button
            {
                Text = "Execute",
                Location = new Point(20, yPos),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(37, 99, 235), // Blue color
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            executeButton.FlatAppearance.BorderSize = 0;
            executeButton.Click += ExecuteButton_Click;

            viewLogsButton = new Button
            {
                Text = "View User Logs",
                Location = new Point(150, yPos),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            viewLogsButton.FlatAppearance.BorderSize = 0;
            viewLogsButton.Click += ViewLogsButton_Click;

            mainPanel.Controls.Add(executeButton);
            mainPanel.Controls.Add(viewLogsButton);

            // Handle resize - GROUPS WILL STRETCH WITH PANEL
            mainPanel.Resize += (s, e) =>
            {
                UpdateLayout(mainPanel.Width);
            };

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void UpdateLayout(int containerWidth)
        {
            if (overridesGroup == null || reasonGroup == null || verificationGroup == null) return;

            int groupWidth = containerWidth - 40; // 20px padding on each side

            // Update group widths
            overridesGroup.Width = groupWidth;
            reasonGroup.Width = groupWidth;
            verificationGroup.Width = groupWidth;

            // Update controls inside overrides group
            int checkBoxWidth = groupWidth - 30;
            foreach (Control control in overridesGroup.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Width = checkBoxWidth;
                }
                else if (control is Panel panel && panel.Controls.Count > 0)
                {
                    // This is the limit container
                    foreach (Control subControl in panel.Controls)
                    {
                        if (subControl is TextBox textBox)
                        {
                            // Keep textbox at fixed width
                            break;
                        }
                    }
                }
            }

            // Update reason textbox width
            reasonTextBox.Width = groupWidth - 30;
            charCountLabel.Left = groupWidth - 120;

            // Update verification group controls
            passwordTextBox.Width = groupWidth - 110;
            secondApprovalCheckBox.Width = groupWidth - 30;

            // Position buttons
            int buttonLeft = 20; // Same left margin as GroupBoxes
            executeButton.Left = buttonLeft;
            viewLogsButton.Left = buttonLeft + 130;

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

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            // Validate at least one action is selected
            bool hasSelectedAction = unlockAccountCheckBox.Checked ||
                                    resetPasswordCheckBox.Checked ||
                                    extendSessionCheckBox.Checked ||
                                    increaseLimitCheckBox.Checked ||
                                    grantAdminCheckBox.Checked ||
                                    bypassTwoFactorCheckBox.Checked;

            if (!hasSelectedAction)
            {
                MessageBox.Show("Please select at least one override action.",
                              "Validation Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Validate reason
            string reason = reasonTextBox.Text;
            if (string.IsNullOrWhiteSpace(reason) || reason == "Enter detailed reason for user action override...")
            {
                MessageBox.Show("Override reason is required.",
                              "Validation Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Validate password
            string password = passwordTextBox.Text;
            if (string.IsNullOrWhiteSpace(password) || password == "Enter admin password")
            {
                MessageBox.Show("Admin password is required.",
                              "Validation Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Validate new limit if increase limit is checked
            if (increaseLimitCheckBox.Checked)
            {
                if (string.IsNullOrWhiteSpace(newLimitTextBox.Text) || newLimitTextBox.Text == "Enter new limit")
                {
                    MessageBox.Show("Please enter a new approval limit.",
                                  "Validation Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(newLimitTextBox.Text, out decimal newLimit) || newLimit <= 0)
                {
                    MessageBox.Show("Please enter a valid approval limit amount.",
                                  "Validation Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to execute user override for {userComboBox.SelectedItem}?",
                "Confirm User Override",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Execute the override (in real app, save to database)
                MessageBox.Show($"User override executed successfully for {userComboBox.SelectedItem}!",
                              "Success",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);

                // Reset form
                ResetForm();
            }
        }

        private void ViewLogsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("View user logs functionality coming soon...",
                          "Info",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
        }

        private void ResetForm()
        {
            // Reset all checkboxes
            unlockAccountCheckBox.Checked = false;
            resetPasswordCheckBox.Checked = false;
            extendSessionCheckBox.Checked = false;
            increaseLimitCheckBox.Checked = false;
            grantAdminCheckBox.Checked = false;
            bypassTwoFactorCheckBox.Checked = false;

            // Reset textboxes
            newLimitTextBox.ForeColor = Color.Gray;
            newLimitTextBox.Text = "Enter new limit";

            reasonTextBox.ForeColor = Color.Gray;
            reasonTextBox.Text = "Enter detailed reason for user action override...";

            passwordTextBox.ForeColor = Color.Gray;
            passwordTextBox.Text = "Enter admin password";

            secondApprovalCheckBox.Checked = false;

            // Reset character count
            charCountLabel.Text = "0/500 characters";
        }
    }
}