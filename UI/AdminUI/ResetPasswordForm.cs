using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI
{
    public partial class ResetPasswordForm : Form
    {
        // UI Controls
        private Label lblUserInfo;
        private TextBox txtAdminPassword;
        private RadioButton rdoRandom;
        private RadioButton rdoCustom;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkRequireChange;
        private CheckBox chkNotifyEmail;
        private CheckBox chkLogAudit;
        private TextBox txtGeneratedPassword;
        private Button btnCopy;
        private Button btnCancel;
        private Button btnSendEmail;
        private Button btnReset;

        // Container for dynamic content
        private Panel containerPanel;
        private Panel settingsPanel;
        private Panel generatedPanel;
        private Panel buttonPanel;
        private Panel customPanel;
        private Panel optionsPanel;

        // User data - will be set by constructor
        private string _username;
        private string _fullName;
        private string _role;
        private string _email;

        public ResetPasswordForm(string username, string fullName, string role, string email)
        {
            _username = username;
            _fullName = fullName;
            _role = role;
            _email = email;
            BuildForm();
        }

        private void BuildForm()
        {
            // Form settings
            this.Text = "Reset Password";
            this.Size = new Size(570, 500); 
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Padding = new Padding(10);

            // Main container with scrolling
            containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSize = false
            };

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10),
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // ===== HEADER =====
            var lblTitle = new Label
            {
                Text = "RESET PASSWORD",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainLayout.Controls.Add(lblTitle, 0, 0);

            // ===== USER INFO =====
            var userPanel = new Panel
            {
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 50,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 15)
            };

            lblUserInfo = new Label
            {
                Text = $"User: {_fullName} ({_username})\nRole: {_role}",
                Font = new Font("Segoe UI", 9),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            userPanel.Controls.Add(lblUserInfo);
            mainLayout.Controls.Add(userPanel, 0, 1);

            // ===== SECURITY VERIFICATION =====
            var securityPanel = new Panel
            {
                Height = 120,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 15)
            };

            var lblSecurity = new Label
            {
                Text = "SECURITY VERIFICATION:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            securityPanel.Controls.Add(lblSecurity);

            var lblAdmin = new Label
            {
                Text = "Admin Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 30),
                AutoSize = true
            };
            securityPanel.Controls.Add(lblAdmin);

            txtAdminPassword = new TextBox
            {
                Location = new Point(20, 55),
                Size = new Size(250, 25),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 9)
            };
            securityPanel.Controls.Add(txtAdminPassword);

            var lblHint = new Label
            {
                Text = "(Required for password reset)",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(20, 85),
                AutoSize = true
            };
            securityPanel.Controls.Add(lblHint);

            mainLayout.Controls.Add(securityPanel, 0, 2);

            // ===== NEW PASSWORD OPTIONS =====
            optionsPanel = new Panel
            {
                Height = 90, // Base height without custom panel
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 15)
            };

            var lblOptions = new Label
            {
                Text = "NEW PASSWORD OPTIONS:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            optionsPanel.Controls.Add(lblOptions);

            rdoRandom = new RadioButton
            {
                Text = "Generate Random Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 30),
                AutoSize = true,
                Checked = true
            };
            rdoRandom.CheckedChanged += RdoOption_CheckedChanged;
            optionsPanel.Controls.Add(rdoRandom);

            rdoCustom = new RadioButton
            {
                Text = "Set Custom Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 60),
                AutoSize = true
            };
            rdoCustom.CheckedChanged += RdoOption_CheckedChanged;
            optionsPanel.Controls.Add(rdoCustom);

            // ===== CUSTOM PASSWORD PANEL =====
            customPanel = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(400, 100),
                Visible = false
            };

            var lblCustomTitle = new Label
            {
                Text = "If Custom:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 0),
                AutoSize = true
            };
            customPanel.Controls.Add(lblCustomTitle);

            var lblNewPass = new Label
            {
                Text = "New Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 25),
                AutoSize = true
            };
            customPanel.Controls.Add(lblNewPass);

            txtNewPassword = new TextBox
            {
                Location = new Point(0, 50),
                Size = new Size(180, 25),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 9)
            };
            customPanel.Controls.Add(txtNewPassword);

            var lblConfirm = new Label
            {
                Text = "Confirm Password",
                Font = new Font("Segoe UI", 9),
                Location = new Point(200, 25),
                AutoSize = true
            };
            customPanel.Controls.Add(lblConfirm);

            txtConfirmPassword = new TextBox
            {
                Location = new Point(200, 50),
                Size = new Size(180, 25),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 9)
            };
            customPanel.Controls.Add(txtConfirmPassword);

            optionsPanel.Controls.Add(customPanel);
            mainLayout.Controls.Add(optionsPanel, 0, 3);

            // ===== PASSWORD SETTINGS =====
            settingsPanel = new Panel
            {
                Height = 120,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 15)
            };

            var lblSettings = new Label
            {
                Text = "PASSWORD SETTINGS:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            settingsPanel.Controls.Add(lblSettings);

            chkRequireChange = new CheckBox
            {
                Text = "Require change on next login",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 30),
                AutoSize = true,
                Checked = true
            };
            settingsPanel.Controls.Add(chkRequireChange);

            chkNotifyEmail = new CheckBox
            {
                Text = "Notify user via email",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 60),
                AutoSize = true,
                Checked = true
            };
            settingsPanel.Controls.Add(chkNotifyEmail);

            chkLogAudit = new CheckBox
            {
                Text = "Log this action in audit trail",
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 90),
                AutoSize = true
            };
            settingsPanel.Controls.Add(chkLogAudit);

            mainLayout.Controls.Add(settingsPanel, 0, 4);

            // ===== GENERATED PASSWORD PANEL =====
            generatedPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 15)
            };

            var lblGenerated = new Label
            {
                Text = "Generated Password (if random):",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            generatedPanel.Controls.Add(lblGenerated);

            txtGeneratedPassword = new TextBox
            {
                Location = new Point(0, 30),
                Size = new Size(250, 25),
                Text = "X7g#9pL2$qR",
                Font = new Font("Courier New", 10, FontStyle.Bold),
                BackColor = Color.LightYellow,
                ReadOnly = true
            };
            generatedPanel.Controls.Add(txtGeneratedPassword);

            btnCopy = new Button
            {
                Location = new Point(260, 30),
                Size = new Size(100, 25),
                Text = "Copy",
                Font = new Font("Segoe UI", 9)
            };
            btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(txtGeneratedPassword.Text);
                MessageBox.Show("Password copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            generatedPanel.Controls.Add(btnCopy);

            mainLayout.Controls.Add(generatedPanel, 0, 5);

            // ===== BUTTONS =====
            buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Fill
            };

            btnCancel = new Button
            {
                Location = new Point(200, 10),
                Size = new Size(80, 30),
                Text = "Cancel",
                Font = new Font("Segoe UI", 9)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            buttonPanel.Controls.Add(btnCancel);

            btnSendEmail = new Button
            {
                Location = new Point(290, 10),
                Size = new Size(100, 30),
                Text = "Send Email",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Blue
            };
            btnSendEmail.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtAdminPassword.Text))
                {
                    MessageBox.Show("Please enter your admin password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show($"Password reset email sent to {_email}", "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            buttonPanel.Controls.Add(btnSendEmail);

            btnReset = new Button
            {
                Location = new Point(400, 10),
                Size = new Size(80, 30),
                Text = "Reset",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnReset.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtAdminPassword.Text))
                {
                    MessageBox.Show("Please enter your admin password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (rdoCustom.Checked)
                {
                    if (string.IsNullOrEmpty(txtNewPassword.Text))
                    {
                        MessageBox.Show("Please enter a new password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (txtNewPassword.Text != txtConfirmPassword.Text)
                    {
                        MessageBox.Show("Passwords do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (txtNewPassword.Text.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                MessageBox.Show($"Password for {_username} has been reset successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            };
            buttonPanel.Controls.Add(btnReset);

            mainLayout.Controls.Add(buttonPanel, 0, 6);

            // Add main layout to container and container to form
            containerPanel.Controls.Add(mainLayout);
            this.Controls.Add(containerPanel);
        }

        private void RdoOption_CheckedChanged(object sender, EventArgs e)
        {
            if (customPanel != null)
            {
                customPanel.Visible = rdoCustom.Checked;
                generatedPanel.Visible = rdoRandom.Checked;

                // Adjust the options panel height
                if (rdoCustom.Checked)
                {
                    optionsPanel.Height = 90 + customPanel.Height + 10;
                    // Enlarge the form to show everything without scrolling
                    this.Size = new Size(this.Width, 600);
                }
                else
                {
                    optionsPanel.Height = 90;
                    // Reset form size
                    this.Size = new Size(this.Width, 500);
                }

                // Force layout recalculation
                containerPanel.PerformLayout();
            }
        }
    }
}