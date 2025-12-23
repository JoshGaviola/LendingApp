using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI.Views
{
    public partial class AddUserDialog : Form
    {
        public event Action<UserData> UserCreated;

        public class UserData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Role { get; set; }
            public string EmployeeId { get; set; }
            public string Department { get; set; }
            public DateTime HireDate { get; set; }
            public bool IsActive { get; set; }
            public bool CanApproveLoan { get; set; }
            public bool CanReleaseLoan { get; set; }
            public bool CanGenerateReports { get; set; }
        }

        public AddUserDialog()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            Text = "Add New User";
            Size = new Size(500, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(20);
        }

        private void InitializeUI()
        {
            // Main container
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Content
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Content panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var fieldsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Title
            var titleLabel = new Label
            {
                Text = "ADD NEW USER",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Margin = new Padding(0, 0, 0, 20),
                AutoSize = true
            };
            fieldsPanel.Controls.Add(titleLabel);

            // Basic Information Section
            AddSectionLabel("BASIC INFORMATION:", fieldsPanel);
            var txtUsername = AddTextField("Username:", fieldsPanel);
            var txtPassword = AddPasswordField("Password:", fieldsPanel);
            var txtConfirmPassword = AddPasswordField("Confirm Password:", fieldsPanel);
            var txtFullName = AddTextField("Full Name:", fieldsPanel);
            var txtEmail = AddTextField("Email:", fieldsPanel);
            var txtPhone = AddTextField("Phone:", fieldsPanel);

            // Role Assignment Section
            AddSectionLabel("ROLE ASSIGNMENT:", fieldsPanel);
            var cmbRole = AddComboBox("Role:", new[] { "Admin", "Loan Officer", "Cashier" }, fieldsPanel);

            // User-Specific Settings Section
            AddSectionLabel("USER-SPECIFIC SETTINGS:", fieldsPanel);
            var txtEmployeeId = AddTextField("Employee ID:", fieldsPanel);
            var txtDepartment = AddTextField("Department:", fieldsPanel);
            var dtpHireDate = AddDatePicker("Hire Date:", fieldsPanel);

            // Status Section
            AddSectionLabel("INITIAL STATUS:", fieldsPanel);
            var rbActive = AddRadioButton("Active - User can login immediately", true, fieldsPanel);
            var rbInactive = AddRadioButton("Inactive - User requires activation", false, fieldsPanel);

            // Permissions Section
            AddSectionLabel("PERMISSIONS (if applicable):", fieldsPanel);
            var chkCanApproveLoan = AddCheckBox("Can approve loans", true, fieldsPanel);
            var chkCanReleaseLoan = AddCheckBox("Can release loans", false, fieldsPanel);
            var chkCanGenerateReports = AddCheckBox("Can generate reports", true, fieldsPanel);

            // Set defaults based on role
            cmbRole.SelectedIndex = 1; // Loan Officer
            UpdateEmployeeIdPrefix(txtEmployeeId, cmbRole.Text);
            UpdatePermissions(chkCanApproveLoan, chkCanReleaseLoan, chkCanGenerateReports, cmbRole.Text);

            cmbRole.SelectedIndexChanged += (s, e) =>
            {
                UpdateEmployeeIdPrefix(txtEmployeeId, cmbRole.Text);
                UpdatePermissions(chkCanApproveLoan, chkCanReleaseLoan, chkCanGenerateReports, cmbRole.Text);
            };

            // Add fields to content panel
            contentPanel.Controls.Add(fieldsPanel);
            mainPanel.Controls.Add(contentPanel, 0, 0);

            // Button panel at bottom
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 20, 0) // Add right padding
            };

            // Calculate total width needed for both buttons with spacing
            int buttonSpacing = 10;
            int cancelButtonWidth = 100;
            int createButtonWidth = 120;
            int totalButtonsWidth = cancelButtonWidth + createButtonWidth + buttonSpacing;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(cancelButtonWidth, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            btnCancel.FlatAppearance.BorderColor = Color.LightGray;

            var btnCreate = new Button
            {
                Text = "Create User",
                Size = new Size(createButtonWidth, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White
            };

            // Position buttons next to each other on the right
            btnCancel.Location = new Point(buttonPanel.Width - totalButtonsWidth, 10);
            btnCreate.Location = new Point(buttonPanel.Width - createButtonWidth, 10);

            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            btnCreate.Click += (s, e) => CreateUser(
                txtUsername, txtPassword, txtConfirmPassword, txtFullName, txtEmail, txtPhone,
                cmbRole, txtEmployeeId, txtDepartment, dtpHireDate,
                rbActive, chkCanApproveLoan, chkCanReleaseLoan, chkCanGenerateReports);

            // Handle resizing to keep buttons on the right
            buttonPanel.Resize += (s, e) =>
            {
                btnCancel.Left = buttonPanel.Width - totalButtonsWidth;
                btnCreate.Left = buttonPanel.Width - createButtonWidth;
            };

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnCreate);
            mainPanel.Controls.Add(buttonPanel, 0, 1);

            Controls.Add(mainPanel);
        }

        private void AddSectionLabel(string text, FlowLayoutPanel panel)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 20, 0, 5),
                AutoSize = true
            };
            panel.Controls.Add(label);
        }

        private TextBox AddTextField(string labelText, FlowLayoutPanel panel)
        {
            var label = new Label
            {
                Text = labelText,
                Width = 150,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
            panel.Controls.Add(label);

            var textBox = new TextBox
            {
                Width = 300,
                Height = 30,
                Margin = new Padding(0, 0, 0, 10)
            };
            panel.Controls.Add(textBox);

            return textBox;
        }

        private TextBox AddPasswordField(string labelText, FlowLayoutPanel panel)
        {
            var textBox = AddTextField(labelText, panel);
            textBox.UseSystemPasswordChar = true;
            return textBox;
        }

        private ComboBox AddComboBox(string labelText, string[] items, FlowLayoutPanel panel)
        {
            var label = new Label
            {
                Text = labelText,
                Width = 150,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
            panel.Controls.Add(label);

            var comboBox = new ComboBox
            {
                Width = 300,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 10)
            };
            comboBox.Items.AddRange(items);
            panel.Controls.Add(comboBox);

            return comboBox;
        }

        private DateTimePicker AddDatePicker(string labelText, FlowLayoutPanel panel)
        {
            var label = new Label
            {
                Text = labelText,
                Width = 150,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0)
            };
            panel.Controls.Add(label);

            var datePicker = new DateTimePicker
            {
                Width = 300,
                Height = 30,
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 0, 10)
            };
            panel.Controls.Add(datePicker);

            return datePicker;
        }

        private RadioButton AddRadioButton(string text, bool isChecked, FlowLayoutPanel panel)
        {
            var radioButton = new RadioButton
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5),
                Checked = isChecked
            };
            panel.Controls.Add(radioButton);

            return radioButton;
        }

        private CheckBox AddCheckBox(string text, bool isChecked, FlowLayoutPanel panel)
        {
            var checkBox = new CheckBox
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 5),
                Checked = isChecked
            };
            panel.Controls.Add(checkBox);

            return checkBox;
        }

        private void UpdateEmployeeIdPrefix(TextBox txtEmployeeId, string role)
        {
            string prefix;
            switch (role)
            {
                case "Admin":
                    prefix = "ADMIN-";
                    break;
                case "Loan Officer":
                    prefix = "LO-";
                    break;
                case "Cashier":
                    prefix = "CSH-";
                    break;
                default:
                    prefix = "LO-";
                    break;
            }

            if (string.IsNullOrEmpty(txtEmployeeId.Text) ||
                txtEmployeeId.Text.StartsWith("ADMIN-") ||
                txtEmployeeId.Text.StartsWith("LO-") ||
                txtEmployeeId.Text.StartsWith("CSH-"))
            {
                txtEmployeeId.Text = prefix;
            }
        }

        private void UpdatePermissions(CheckBox chkApprove, CheckBox chkRelease, CheckBox chkReports, string role)
        {
            switch (role)
            {
                case "Admin":
                    chkApprove.Visible = true;
                    chkRelease.Visible = true;
                    chkReports.Visible = true;
                    chkApprove.Checked = true;
                    chkRelease.Checked = true;
                    chkReports.Checked = true;
                    break;
                case "Loan Officer":
                    chkApprove.Visible = true;
                    chkRelease.Visible = false;
                    chkReports.Visible = true;
                    chkApprove.Checked = true;
                    chkReports.Checked = true;
                    break;
                case "Cashier":
                    chkApprove.Visible = false;
                    chkRelease.Visible = true;
                    chkReports.Visible = true;
                    chkRelease.Checked = true;
                    chkReports.Checked = true;
                    break;
            }
        }

        private void CreateUser(
            TextBox txtUsername, TextBox txtPassword, TextBox txtConfirmPassword,
            TextBox txtFullName, TextBox txtEmail, TextBox txtPhone,
            ComboBox cmbRole, TextBox txtEmployeeId, TextBox txtDepartment, DateTimePicker dtpHireDate,
            RadioButton rbActive, CheckBox chkApprove, CheckBox chkRelease, CheckBox chkReports)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter full name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter email address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            // Create user data
            var userData = new UserData
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text,
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Role = cmbRole.Text,
                EmployeeId = txtEmployeeId.Text.Trim(),
                Department = txtDepartment.Text.Trim(),
                HireDate = dtpHireDate.Value,
                IsActive = rbActive.Checked,
                CanApproveLoan = chkApprove.Checked,
                CanReleaseLoan = chkRelease.Checked,
                CanGenerateReports = chkReports.Checked
            };

            UserCreated?.Invoke(userData);

            MessageBox.Show($"User '{userData.Username}' created successfully!",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}