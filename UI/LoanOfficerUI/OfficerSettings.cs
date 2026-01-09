using System;
using System.Drawing;
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

            // Tabs - only Profile tab kept
            tabs = new TabControl { Dock = DockStyle.Fill };
            var tabProfile = new TabPage("Profile") { BackColor = Color.White };
            tabs.TabPages.Add(tabProfile);

            BuildProfileTab(tabProfile);

            Controls.Add(tabs);
            Controls.Add(headerPanel);
        }

        private void BuildProfileTab(TabPage tab)
        {
            tab.Controls.Clear();

            var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White };
            tab.Controls.Add(host);

            // Personal information panel (full width now that security section is removed)
            var personalPanel = new Panel { Location = new Point(16, 16), Size = new Size(host.Width - 48, 360), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
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
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            btnSaveProfile.Click += (s, e) => ShowToast("Profile updated successfully!");
            personalPanel.Controls.Add(btnSaveProfile);

            host.Controls.Add(personalPanel);

            // Ensure controls resize with host
            host.Resize += (s, e) =>
            {
                personalPanel.Width = host.Width - 48;
                btnSaveProfile.Width = personalPanel.Width - 24;
            };
        }

        private TextBox AddLabeledTextBox(Control parent, string label, string defaultValue, int x, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#2C3E50"), Font = new Font("Segoe UI", 9) };
            var txt = new TextBox { Text = defaultValue, Location = new Point(x, y + 18), Width = parent.Width - (x * 2), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            y += 56;
            return txt;
        }

        private void ShowToast(string message)
        {
            // Simple toast-like feedback using MessageBox for WinForms
            MessageBox.Show(message, "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
