using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.UI
{
    public partial class EditUserForm : Form
    {
        private User _user;

        // Declare txtCurrentRole at class level so we can access it
        private TextBox txtCurrentRole;

        // Form controls (rest remain the same)
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbRole;
        private RadioButton rbActive;
        private RadioButton rbInactive;
        private RadioButton rbLocked;
        private CheckBox chkForcePasswordReset;
        private CheckBox chkUnlockAccount;

        private Label lblUserId;
        private Label lblCreatedDate;
        private Label lblLastLogin;
        private Label lblFailedLogins;
        private Label lblCreatedBy;

        private Button btnSave;
        private Button btnReset;
        private Button btnCancel;

        // Keep reference to scrollPanel for resizing
        private Panel scrollPanel;

        // Event for when user is updated
        public event Action<User> OnUserUpdated;

        public EditUserForm(User user)
        {
            _user = user;
            InitializeForm();
            LoadUserData();
        }

        private void InitializeForm()
        {
            // Form settings
            Text = $"Edit User - {_user.Username}";
            Size = new Size(600, 700);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Main panel with padding
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Create scrollable panel
            scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            int yPos = 10;

            // Header section
            var lblHeader = new Label
            {
                Text = "EDIT USER",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 30;

            lblUserId = new Label
            {
                Text = $"User ID: {_user.EmployeeId}",
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            lblCreatedDate = new Label
            {
                Text = $"Created: {_user.CreatedDate}",
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 30;

            // Basic Information section
            var lblBasicInfo = new Label
            {
                Text = "BASIC INFORMATION:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            // Username (read-only)
            var lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(250, 23),
                ReadOnly = true,
                BackColor = Color.LightGray,
                Name = "txtUsername" // Add a name
            };
            yPos += 30;

            // Full Name
            var lblFullName = new Label
            {
                Text = "Full Name:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            txtFullName = new TextBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(250, 23),
                Name = "txtFullName"
            };
            yPos += 30;

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(250, 23),
                Name = "txtEmail"
            };
            yPos += 30;

            // Phone
            var lblPhone = new Label
            {
                Text = "Phone:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            txtPhone = new TextBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(250, 23),
                Name = "txtPhone"
            };
            yPos += 40;

            // Role Management section
            var lblRoleManagement = new Label
            {
                Text = "ROLE MANAGEMENT:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            var lblCurrentRole = new Label
            {
                Text = "Current Role:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            // FIXED: Store reference to txtCurrentRole
            txtCurrentRole = new TextBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(120, 23),
                ReadOnly = true,
                BackColor = Color.LightGray,
                Name = "txtCurrentRole"
            };
            yPos += 30;

            var lblNewRole = new Label
            {
                Text = "Change to:",
                Location = new Point(20, yPos),
                AutoSize = true
            };

            cmbRole = new ComboBox
            {
                Location = new Point(120, yPos - 3),
                Size = new Size(150, 23),
                Name = "cmbRole"
            };
            cmbRole.Items.AddRange(new string[] { "Admin", "Loan Officer", "Cashier" });
            yPos += 40;

            // Status Control section
            var lblStatusControl = new Label
            {
                Text = "STATUS CONTROL:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            var lblCurrentStatus = new Label
            {
                Text = $"Current Status: {_user.Status}",
                Location = new Point(20, yPos),
                AutoSize = true
            };
            yPos += 25;

            var lblChangeStatus = new Label
            {
                Text = "Change to:",
                Location = new Point(20, yPos),
                AutoSize = true
            };
            yPos += 25;

            rbActive = new RadioButton
            {
                Text = "Active",
                Location = new Point(40, yPos),
                AutoSize = true,
                Name = "rbActive"
            };
            yPos += 25;

            rbInactive = new RadioButton
            {
                Text = "Inactive",
                Location = new Point(40, yPos),
                AutoSize = true,
                Name = "rbInactive"
            };
            yPos += 25;

            rbLocked = new RadioButton
            {
                Text = "Locked",
                Location = new Point(40, yPos),
                AutoSize = true,
                Name = "rbLocked"
            };
            yPos += 40;

            // Security Settings section
            var lblSecuritySettings = new Label
            {
                Text = "SECURITY SETTINGS:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            var lblLastPasswordChange = new Label
            {
                Text = "Last Password Change: 2024-05-15",
                Location = new Point(20, yPos),
                AutoSize = true
            };
            yPos += 25;

            chkForcePasswordReset = new CheckBox
            {
                Text = "Force Password Reset on Next Login",
                Location = new Point(20, yPos),
                AutoSize = true,
                Name = "chkForcePasswordReset"
            };
            yPos += 25;

            chkUnlockAccount = new CheckBox
            {
                Text = "Unlock Account (if locked)",
                Location = new Point(20, yPos),
                AutoSize = true,
                Name = "chkUnlockAccount"
            };
            yPos += 40;

            // Audit Information section
            var lblAuditInfo = new Label
            {
                Text = "AUDIT INFORMATION:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            yPos += 25;

            lblLastLogin = new Label
            {
                Text = $"Last Login: {_user.LastLogin}",
                Location = new Point(20, yPos),
                AutoSize = true,
                Name = "lblLastLogin"
            };
            yPos += 20;

            lblFailedLogins = new Label
            {
                Text = "Failed Login Attempts: 0",
                Location = new Point(20, yPos),
                AutoSize = true,
                Name = "lblFailedLogins"
            };
            yPos += 20;

            lblCreatedBy = new Label
            {
                Text = "Created By: admin",
                Location = new Point(20, yPos),
                AutoSize = true,
                Name = "lblCreatedBy"
            };
            yPos += 40;

            // Adjust scroll panel height based on content
            scrollPanel.Height = yPos + 10;

            // Buttons panel at bottom
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20)
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(80, 30),
                Location = new Point(300, 15),
                Name = "btnCancel"
            };
            btnCancel.Click += (s, e) => Close();

            btnReset = new Button
            {
                Text = "Reset",
                Size = new Size(80, 30),
                Location = new Point(390, 15),
                Name = "btnReset"
            };
            btnReset.Click += BtnReset_Click;

            btnSave = new Button
            {
                Text = "Save",
                Size = new Size(80, 30),
                Location = new Point(480, 15),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Name = "btnSave"
            };
            btnSave.Click += BtnSave_Click;

            // Add all controls to scroll panel
            scrollPanel.Controls.AddRange(new Control[]
            {
                lblHeader, lblUserId, lblCreatedDate,
                lblBasicInfo, lblUsername, txtUsername,
                lblFullName, txtFullName, lblEmail, txtEmail,
                lblPhone, txtPhone, lblRoleManagement,
                lblCurrentRole, txtCurrentRole, lblNewRole, cmbRole,
                lblStatusControl, lblCurrentStatus, lblChangeStatus,
                rbActive, rbInactive, rbLocked, lblSecuritySettings,
                lblLastPasswordChange, chkForcePasswordReset, chkUnlockAccount,
                lblAuditInfo, lblLastLogin, lblFailedLogins, lblCreatedBy
            });

            buttonPanel.Controls.AddRange(new Control[] { btnCancel, btnReset, btnSave });

            mainPanel.Controls.Add(scrollPanel);
            Controls.Add(mainPanel);
            Controls.Add(buttonPanel);
        }

        private void LoadUserData()
        {
            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            txtPhone.Text = _user.Phone;

            // FIXED: Use the class-level reference instead of finding by name
            txtCurrentRole.Text = _user.Role;

            cmbRole.SelectedItem = _user.Role;

            // Set status radio button
            switch (_user.Status)
            {
                case "Active":
                    rbActive.Checked = true;
                    break;
                case "Inactive":
                    rbInactive.Checked = true;
                    break;
                case "Locked":
                    rbLocked.Checked = true;
                    break;
            }

            // Load other audit info
            lblLastLogin.Text = $"Last Login: {_user.LastLogin}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Please enter full name", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please enter email address", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update user object
                _user.FullName = txtFullName.Text.Trim();
                _user.Email = txtEmail.Text.Trim();
                _user.Phone = txtPhone.Text.Trim();
                _user.Role = cmbRole.SelectedItem?.ToString() ?? _user.Role;

                // Set status
                if (rbActive.Checked) _user.Status = "Active";
                else if (rbInactive.Checked) _user.Status = "Inactive";
                else if (rbLocked.Checked) _user.Status = "Locked";

                // Security settings would be saved separately

                // Fire event
                OnUserUpdated?.Invoke(_user);

                MessageBox.Show($"User {_user.Username} updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            try
            {
                // Reset form to original values
                LoadUserData();
                chkForcePasswordReset.Checked = false;
                chkUnlockAccount.Checked = false;

                MessageBox.Show("Form reset to original values", "Reset",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Simple User class (should match your actual User class)
    public class User
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string EmployeeId { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string LastLogin { get; set; }
    }
}