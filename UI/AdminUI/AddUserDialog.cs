using System;
using System.Drawing;
using System.Windows.Forms;
using LendingApp.Class.LogicClass.Admin;
using LendingApp.Class.Models.User;

namespace LendingApp.UI.AdminUI.Views
{
    public partial class AddUserDialog : Form
    {
        public event Action<UserData> UserCreated;

        private TextBox txtUsername, txtPassword, txtConfirmPassword;
        private TextBox txtFirstName, txtLastName, txtEmail, txtPhone;
        private ComboBox cmbRole;
        private DateTimePicker dtpHireDate;
        private RadioButton rbActive, rbInactive;
        private Button btnCancel, btnCreate;

        public AddUserDialog()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            Text = "Add New User";
            Size = new Size(550, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(248, 250, 252);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void InitializeUI()
        {
            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            var lblHeader = new Label
            {
                Text = "Add New User",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(25, 20)
            };
            headerPanel.Controls.Add(lblHeader);

            var lblSubHeader = new Label
            {
                Text = "Create a new user account for the system",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(203, 213, 225),
                AutoSize = true,
                Location = new Point(25, 50)
            };
            headerPanel.Controls.Add(lblSubHeader);

            Controls.Add(headerPanel);

            // Content Panel with ScrollBar
            var contentPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(550, 510),
                BackColor = Color.FromArgb(248, 250, 252),
                AutoScroll = true
            };

            var innerPanel = new Panel
            {
                Location = new Point(25, 20),
                Size = new Size(480, 750),
                BackColor = Color.Transparent
            };

            int y = 0;

            // Account Credentials Section
            y = AddSectionHeader(innerPanel, "Account Credentials", y);
            txtUsername = AddStyledTextBox(innerPanel, "Username", y, true); y += 65;
            txtPassword = AddStyledTextBox(innerPanel, "Password", y, true, true); y += 65;
            txtConfirmPassword = AddStyledTextBox(innerPanel, "Confirm Password", y, true, true); y += 80;

            // Personal Information Section
            y = AddSectionHeader(innerPanel, "Personal Information", y);
            txtFirstName = AddStyledTextBox(innerPanel, "First Name", y, true); y += 65;
            txtLastName = AddStyledTextBox(innerPanel, "Last Name", y, true); y += 65;
            txtEmail = AddStyledTextBox(innerPanel, "Email Address", y, false); y += 65;
            txtPhone = AddStyledTextBox(innerPanel, "Phone Number", y, false); y += 80;

            // Role & Status Section
            y = AddSectionHeader(innerPanel, "Role & Status", y);
            cmbRole = AddStyledComboBox(innerPanel, "User Role", new[] { "Admin", "Loan Officer", "Cashier" }, y);
            cmbRole.SelectedIndex = 1;
            y += 65;

            AddStyledStatusRadioButtons(innerPanel, y); y += 65;
            dtpHireDate = AddStyledDatePicker(innerPanel, "Hire Date", y); y += 65;

            contentPanel.Controls.Add(innerPanel);
            Controls.Add(contentPanel);

            // Footer Button Panel
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White
            };

            // Add top border to footer
            footerPanel.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, footerPanel.Width, 0);
                }
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 38),
                Location = new Point(320, 16),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            btnCreate = new Button
            {
                Text = "Create User",
                Size = new Size(120, 38),
                Location = new Point(430, 16),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnCreate.Click += BtnCreate_Click;

            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnCreate);
            Controls.Add(footerPanel);
        }

        private int AddSectionHeader(Panel parent, string text, int y)
        {
            var panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(480, 40),
                BackColor = Color.Transparent
            };

            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 8),
                AutoSize = true
            };
            panel.Controls.Add(lbl);

            var line = new Panel
            {
                BackColor = Color.FromArgb(226, 232, 240),
                Location = new Point(0, 35),
                Size = new Size(480, 1)
            };
            panel.Controls.Add(line);

            parent.Controls.Add(panel);
            return y + 50;
        }

        private TextBox AddStyledTextBox(Panel parent, string labelText, int y, bool required, bool isPassword = false)
        {
            var lbl = new Label
            {
                Text = labelText + (required ? " *" : ""),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);

            var tb = new TextBox
            {
                Width = 480,
                Height = 35,
                Location = new Point(0, y + 22),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = isPassword
            };

            tb.GotFocus += (s, e) =>
            {
                tb.BackColor = Color.FromArgb(249, 250, 251);
            };

            tb.LostFocus += (s, e) =>
            {
                tb.BackColor = Color.White;
            };

            parent.Controls.Add(tb);
            return tb;
        }

        private ComboBox AddStyledComboBox(Panel parent, string labelText, string[] items, int y)
        {
            var lbl = new Label
            {
                Text = labelText + " *",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);

            var cb = new ComboBox
            {
                Width = 480,
                Height = 35,
                Location = new Point(0, y + 22),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cb.Items.AddRange(items);

            parent.Controls.Add(cb);
            return cb;
        }

        private void AddStyledStatusRadioButtons(Panel parent, int y)
        {
            var lbl = new Label
            {
                Text = "Account Status *",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);

            var statusPanel = new Panel
            {
                Location = new Point(0, y + 22),
                Size = new Size(480, 40),
                BackColor = Color.Transparent
            };

            rbActive = new RadioButton
            {
                Text = "Active",
                Checked = true,
                Location = new Point(0, 8),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true
            };
            statusPanel.Controls.Add(rbActive);

            rbInactive = new RadioButton
            {
                Text = "Inactive",
                Location = new Point(100, 8),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true
            };
            statusPanel.Controls.Add(rbInactive);

            parent.Controls.Add(statusPanel);
        }

        private DateTimePicker AddStyledDatePicker(Panel parent, string labelText, int y)
        {
            var lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);

            var dp = new DateTimePicker
            {
                Width = 480,
                Height = 35,
                Location = new Point(0, y + 22),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Long,
                CalendarForeColor = Color.FromArgb(30, 41, 59)
            };

            parent.Controls.Add(dp);
            return dp;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields marked with *",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match. Please verify and try again.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserData newUser = new UserData
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                Confirmpassword = txtConfirmPassword.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Role = cmbRole.SelectedItem?.ToString().Trim(),
                IsActive = rbActive.Checked
            };

            UserRegisterLogic logic = new UserRegisterLogic();
            if (logic.RegisterSuccess(newUser))
            {
                MessageBox.Show("User account has been created successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Failed to create user account. The username may already exist.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}