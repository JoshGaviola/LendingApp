using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerSettings : Form
    {
        // Profile
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtMobile;
        private TextBox txtPosition;
        private Button btnSaveProfile;

        // Notifications
        private CheckBox chkPaymentDueReminders;
        private CheckBox chkNewLoanApplications;
        private CheckBox chkOverdueLoanAlerts;
        private CheckBox chkCustomerMessages;
        private CheckBox chkWeeklyReports;
        private CheckBox chkMonthlyReports;
        private Button btnSaveNotifications;

        // System preferences
        private CheckBox chkAutoLogout;
        private CheckBox chkSoundNotifications;
        private CheckBox chkEmailNotifications;
        private CheckBox chkDarkMode;
        private Button btnSaveSystem;

        // Password change dialog controls
        private Form passwordDialog;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowCurrent;
        private CheckBox chkShowNew;
        private CheckBox chkShowConfirm;
        private Button btnChangePassword;
        private Button btnCancelPassword;

        // UI containers
        private Panel headerPanel;
        private Label lblHeaderTitle;
        private TabControl tabs;

        public OfficerSettings()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Officer Settings";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Header
            headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            lblHeaderTitle = new Label
            {
                Text = "OFFICER SETTINGS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                AutoSize = true,
                Location = new Point(16, 18)
            };
            headerPanel.Controls.Add(lblHeaderTitle);

            // Tabs
            tabs = new TabControl { Dock = DockStyle.Fill };
            var tabProfile = new TabPage("Profile") { BackColor = Color.White };
            var tabNotifications = new TabPage("Notifications") { BackColor = Color.White };
            var tabSystem = new TabPage("System") { BackColor = Color.White };
            tabs.TabPages.Add(tabProfile);
            tabs.TabPages.Add(tabNotifications);
            tabs.TabPages.Add(tabSystem);

            BuildProfileTab(tabProfile);
            BuildNotificationsTab(tabNotifications);
            BuildSystemTab(tabSystem);

            Controls.Add(tabs);
            Controls.Add(headerPanel);
        }

        private void BuildProfileTab(TabPage tab)
        {
            tab.Controls.Clear();

            var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White };
            tab.Controls.Add(host);

            // Left: personal information
            var personalPanel = new Panel { Location = new Point(16, 16), Size = new Size(500, 320), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblPI = new Label { Text = "Personal Information", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            personalPanel.Controls.Add(lblPI);

            int y = 44;
            txtFullName = AddLabeledTextBox(personalPanel, "Full Name", "John Doe", 12, ref y);
            txtEmail = AddLabeledTextBox(personalPanel, "Email Address", "john@lending.com", 12, ref y);
            txtMobile = AddLabeledTextBox(personalPanel, "Mobile Number", "+639123456789", 12, ref y);
            txtPosition = AddLabeledTextBox(personalPanel, "Position", "Loan Officer", 12, ref y);
            txtPosition.Enabled = false;
            personalPanel.Height = y + 60;

            btnSaveProfile = new Button
            {
                Text = "Save Profile",
                Location = new Point(12, y + 12),
                Width = personalPanel.Width - 24,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveProfile.Click += (s, e) => ShowToast("Profile updated successfully!");
            personalPanel.Controls.Add(btnSaveProfile);

            // Right: security
            var securityPanel = new Panel { Location = new Point(540, 16), Size = new Size(500, 320), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblSec = new Label { Text = "Security", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            securityPanel.Controls.Add(lblSec);

            var grpPwd = new Panel { Location = new Point(12, 44), Size = new Size(securityPanel.Width - 24, 140), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var lblPwdTitle = new Label { Text = "Password", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            var lblPwdInfo = new Label { Text = "Last changed: 30 days ago", Location = new Point(10, 34), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            var btnChange = new Button { Text = "Change Password", Location = new Point(10, 64), Width = grpPwd.Width - 20, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnChange.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#93C5FD"); // blue-300
            btnChange.Click += (s, e) => OpenPasswordDialog();
            grpPwd.Controls.Add(lblPwdTitle);
            grpPwd.Controls.Add(lblPwdInfo);
            grpPwd.Controls.Add(btnChange);

            var grpSession = new Panel { Location = new Point(12, 194), Size = new Size(securityPanel.Width - 24, 96), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var lblSessTitle = new Label { Text = "Session Info", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            var lblSess1 = new Label { Text = "Last login: Today, 8:30 AM", Location = new Point(10, 34), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            var lblSess2 = new Label { Text = "Device: Desktop - Chrome", Location = new Point(10, 52), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            var lblSess3 = new Label { Text = "IP Address: 192.168.1.100", Location = new Point(10, 70), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            grpSession.Controls.Add(lblSessTitle);
            grpSession.Controls.Add(lblSess1);
            grpSession.Controls.Add(lblSess2);
            grpSession.Controls.Add(lblSess3);

            securityPanel.Controls.Add(grpPwd);
            securityPanel.Controls.Add(grpSession);

            host.Controls.Add(personalPanel);
            host.Controls.Add(securityPanel);
        }

        private void BuildNotificationsTab(TabPage tab)
        {
            tab.Controls.Clear();
            var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White };
            tab.Controls.Add(host);

            // Left: Notification Preferences
            var notifPanel = new Panel { Location = new Point(16, 16), Size = new Size(500, 340), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblNotif = new Label { Text = "Notification Preferences", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            notifPanel.Controls.Add(lblNotif);
            int y = 44;
            chkPaymentDueReminders = AddCheckbox(notifPanel, "Payment due reminders", "Get notified before payment due dates", true, 12, ref y);
            chkNewLoanApplications = AddCheckbox(notifPanel, "New loan applications", "Alert when new applications are submitted", true, 12, ref y);
            chkOverdueLoanAlerts = AddCheckbox(notifPanel, "Overdue loan alerts", "Critical alerts for overdue payments", true, 12, ref y);
            chkCustomerMessages = AddCheckbox(notifPanel, "Customer messages", "Notifications for customer inquiries", false, 12, ref y);

            btnSaveNotifications = new Button
            {
                Text = "Save Preferences",
                Location = new Point(12, y + 12),
                Width = notifPanel.Width - 24,
                BackColor = ColorTranslator.FromHtml("#EA580C"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveNotifications.Click += (s, e) => ShowToast("Notification preferences saved!");
            notifPanel.Controls.Add(btnSaveNotifications);

            // Right: Report Notifications
            var reportsPanel = new Panel { Location = new Point(540, 16), Size = new Size(500, 340), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblReports = new Label { Text = "Report Notifications", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            reportsPanel.Controls.Add(lblReports);
            int ry = 44;
            chkWeeklyReports = AddCheckbox(reportsPanel, "Weekly performance reports", "Sent every Monday morning", true, 12, ref ry);
            chkMonthlyReports = AddCheckbox(reportsPanel, "Monthly summary reports", "Comprehensive monthly analytics", false, 12, ref ry);

            var summary = new Panel { Location = new Point(12, ry + 12), Size = new Size(reportsPanel.Width - 24, 90), BackColor = ColorTranslator.FromHtml("#EFF6FF"), BorderStyle = BorderStyle.FixedSingle };
            var lblSumTitle = new Label { Text = "Notification Summary", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#1D4ED8") };
            var lblActive = new Label { Text = "Active notifications: 0", Location = new Point(10, 34), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#2563EB"), Font = new Font("Segoe UI", 8) };
            summary.Controls.Add(lblSumTitle);
            summary.Controls.Add(lblActive);
            reportsPanel.Controls.Add(summary);

            // Update summary on any change
            EventHandler recalc = (s, e) =>
            {
                int count = new[] { chkPaymentDueReminders, chkNewLoanApplications, chkOverdueLoanAlerts, chkCustomerMessages, chkWeeklyReports, chkMonthlyReports }
                    .Count(c => c.Checked);
                lblActive.Text = $"Active notifications: {count}";
            };
            chkPaymentDueReminders.CheckedChanged += recalc;
            chkNewLoanApplications.CheckedChanged += recalc;
            chkOverdueLoanAlerts.CheckedChanged += recalc;
            chkCustomerMessages.CheckedChanged += recalc;
            chkWeeklyReports.CheckedChanged += recalc;
            chkMonthlyReports.CheckedChanged += recalc;
            recalc(null, EventArgs.Empty);

            host.Controls.Add(notifPanel);
            host.Controls.Add(reportsPanel);
        }

        private void BuildSystemTab(TabPage tab)
        {
            tab.Controls.Clear();
            var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White };
            tab.Controls.Add(host);

            // Left: System Preferences
            var sysPanel = new Panel { Location = new Point(16, 16), Size = new Size(500, 340), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblSys = new Label { Text = "System Preferences", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            sysPanel.Controls.Add(lblSys);
            int y = 44;
            chkAutoLogout = AddCheckbox(sysPanel, "Auto-logout after 15 minutes", "Automatically log out when inactive", false, 12, ref y);
            chkSoundNotifications = AddCheckbox(sysPanel, "Sound notifications", "Play sound for important alerts", false, 12, ref y);
            chkEmailNotifications = AddCheckbox(sysPanel, "Email notifications", "Receive notifications via email", true, 12, ref y);
            chkDarkMode = AddCheckbox(sysPanel, "Dark mode", "Use dark theme (Coming soon)", false, 12, ref y);

            btnSaveSystem = new Button
            {
                Text = "Save Preferences",
                Location = new Point(12, y + 12),
                Width = sysPanel.Width - 24,
                BackColor = ColorTranslator.FromHtml("#16A34A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveSystem.Click += (s, e) => ShowToast("System preferences saved!");
            sysPanel.Controls.Add(btnSaveSystem);

            // Right: Application Information
            var infoPanel = new Panel { Location = new Point(540, 16), Size = new Size(500, 340), BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblAppInfo = new Label { Text = "Application Information", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 12), AutoSize = true };
            infoPanel.Controls.Add(lblAppInfo);

            var sysDetails = new Panel { Location = new Point(12, 44), Size = new Size(infoPanel.Width - 24, 120), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            AddInfoRow(sysDetails, "Version:", "1.0.0", 10, 10);
            AddInfoRow(sysDetails, "Build:", "2024.12.11", 10, 34);
            AddInfoRow(sysDetails, "Environment:", "Production", 10, 58);
            AddInfoRow(sysDetails, "Server:", "Online", 10, 82);

            var statsPanel = new Panel { Location = new Point(12, 174), Size = new Size(infoPanel.Width - 24, 120), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            AddInfoRow(statsPanel, "Total Applications:", "156", 10, 10);
            AddInfoRow(statsPanel, "Approved This Month:", "23", 10, 34);
            AddInfoRow(statsPanel, "Active Loans:", "45", 10, 58);
            AddInfoRow(statsPanel, "Portfolio Value:", "₱850,000", 10, 82);

            var notePanel = new Panel { Location = new Point(12, 304), Size = new Size(infoPanel.Width - 24, 24), BackColor = ColorTranslator.FromHtml("#FEF3C7"), BorderStyle = BorderStyle.FixedSingle };
            var lblNote = new Label { Text = "Note: Some settings may require admin approval. Changes will be saved to your profile.", AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 8, FontStyle.Regular), ForeColor = ColorTranslator.FromHtml("#92400E") };
            notePanel.Controls.Add(lblNote);

            infoPanel.Controls.Add(sysDetails);
            infoPanel.Controls.Add(statsPanel);
            infoPanel.Controls.Add(notePanel);

            host.Controls.Add(sysPanel);
            host.Controls.Add(infoPanel);
        }

        private TextBox AddLabeledTextBox(Control parent, string label, string defaultValue, int x, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#2C3E50"), Font = new Font("Segoe UI", 9) };
            var txt = new TextBox { Text = defaultValue, Location = new Point(x, y + 18), Width = parent.Width - (x * 2) };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            y += 56;
            return txt;
        }

        private CheckBox AddCheckbox(Control parent, string caption, string sub, bool @checked, int x, ref int y)
        {
            var panel = new Panel { Location = new Point(x, y), Size = new Size(parent.Width - (x * 2), 60), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var chk = new CheckBox { Text = "", Location = new Point(10, 18), Checked = @checked };
            var lblCaption = new Label { Text = caption, Location = new Point(36, 12), AutoSize = true, Font = new Font("Segoe UI", 9) };
            var lblSub = new Label { Text = sub, Location = new Point(36, 32), AutoSize = true, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml("#6B7280") };
            panel.Controls.Add(chk);
            panel.Controls.Add(lblCaption);
            panel.Controls.Add(lblSub);
            parent.Controls.Add(panel);
            y += 68;
            return chk;
        }

        private void AddInfoRow(Control parent, string left, string right, int x, int y)
        {
            var lblL = new Label { Text = left, Location = new Point(x, y), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Font = new Font("Segoe UI", 8) };
            var lblR = new Label { Text = right, Location = new Point(parent.Width - 140, y), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827"), Font = new Font("Segoe UI", 8) };
            parent.Controls.Add(lblL);
            parent.Controls.Add(lblR);
            parent.Resize += (s, e) => { lblR.Left = parent.Width - 140; };
        }

        private void OpenPasswordDialog()
        {
            if (passwordDialog != null && !passwordDialog.IsDisposed)
            {
                passwordDialog.Focus();
                return;
            }

            passwordDialog = new Form
            {
                Text = "Change Password",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(420, 360),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            int y = 10;

            // Current
            var lblCurrent = new Label { Text = "Current Password", Location = new Point(10, y), AutoSize = true };
            txtCurrentPassword = new TextBox { Location = new Point(10, y + 18), Width = 360, UseSystemPasswordChar = true };
            chkShowCurrent = new CheckBox { Text = "Show", Location = new Point(10 + 360 - 60, y + 18 + 2), AutoSize = true };
            chkShowCurrent.CheckedChanged += (s, e) => txtCurrentPassword.UseSystemPasswordChar = !chkShowCurrent.Checked;
            y += 54;

            // New
            var lblNew = new Label { Text = "New Password", Location = new Point(10, y), AutoSize = true };
            txtNewPassword = new TextBox { Location = new Point(10, y + 18), Width = 360, UseSystemPasswordChar = true };
            chkShowNew = new CheckBox { Text = "Show", Location = new Point(10 + 360 - 60, y + 18 + 2), AutoSize = true };
            chkShowNew.CheckedChanged += (s, e) => txtNewPassword.UseSystemPasswordChar = !chkShowNew.Checked;
            var hint = new Label { Text = "Must be at least 8 characters", Location = new Point(10, y + 42), AutoSize = true, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml("#6B7280") };
            y += 66;

            // Confirm
            var lblConfirm = new Label { Text = "Confirm New Password", Location = new Point(10, y), AutoSize = true };
            txtConfirmPassword = new TextBox { Location = new Point(10, y + 18), Width = 360, UseSystemPasswordChar = true };
            chkShowConfirm = new CheckBox { Text = "Show", Location = new Point(10 + 360 - 60, y + 18 + 2), AutoSize = true };
            chkShowConfirm.CheckedChanged += (s, e) => txtConfirmPassword.UseSystemPasswordChar = !chkShowConfirm.Checked;
            y += 54;

            btnCancelPassword = new Button { Text = "Cancel", Location = new Point(10, y), Width = 80 };
            btnChangePassword = new Button { Text = "Change Password", Location = new Point(100, y), Width = 140, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelPassword.Click += (s, e) => passwordDialog.Close();
            btnChangePassword.Click += (s, e) =>
            {
                var current = txtCurrentPassword.Text ?? "";
                var newP = txtNewPassword.Text ?? "";
                var confirm = txtConfirmPassword.Text ?? "";

                if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(newP) || string.IsNullOrWhiteSpace(confirm))
                {
                    MessageBox.Show("Please fill all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newP != confirm)
                {
                    MessageBox.Show("New passwords do not match!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newP.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // TODO: integrate with real password change logic
                ShowToast("Password changed successfully!");
                passwordDialog.Close();
            };

            panel.Controls.AddRange(new Control[]
            {
                lblCurrent, txtCurrentPassword, chkShowCurrent,
                lblNew, txtNewPassword, chkShowNew, hint,
                lblConfirm, txtConfirmPassword, chkShowConfirm,
                btnCancelPassword, btnChangePassword
            });
            passwordDialog.Controls.Add(panel);
            passwordDialog.ShowDialog(this);
        }

        private void ShowToast(string message)
        {
            // Simple toast-like feedback using MessageBox for WinForms
            MessageBox.Show(message, "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
