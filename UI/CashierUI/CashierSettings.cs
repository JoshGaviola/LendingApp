using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierSettings : Form
    {
        // Personal Information
        private string _name = "Maria Santos";
        private string _employeeId = "CSH-001";
        private string _email = "maria.santos@lending.com";
        private string _phone = "+63 912 345 6789";

        // Signature
        private string _currentSignature = "signature.png";

        // Receipt Preferences
        private string _defaultPrinter = "HP-LaserJet-001";
        private bool _autoPrint = true;
        private int _copies = 1;

        // Session Info
        private string _loginTime = "9:15 AM";
        private string _lastActivity = "3:30 PM";
        private int _paymentsToday = 26;
        private int _loansReleasedToday = 2;
        private int _receiptsPrintedToday = 26;

        // UI containers
        private Panel root;

        // Personal Information controls
        private TextBox txtEmail;
        private TextBox txtPhone;

        // Password controls
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowCurrentPassword;
        private CheckBox chkShowNewPassword;
        private CheckBox chkShowConfirmPassword;

        // Signature controls
        private Label lblCurrentSignature;

        // Receipt Preferences controls
        private ComboBox cmbDefaultPrinter;
        private CheckBox chkAutoPrintReceipts;
        private ComboBox cmbCopies;

        // Session Info controls
        private Label lblLoginTime;
        private Label lblLastActivity;
        private Label lblPaymentsToday;
        private Label lblLoansReleased;
        private Label lblReceiptsPrinted;

        // Toast
        private Panel _toastPanel;
        private Label _toastLabel;
        private Timer _toastTimer;

        public CashierSettings()
        {
            InitializeComponent();

            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            BuildUI();
            LoadData();
            BuildToast();
        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16),
                BackColor = Color.Transparent
            };
            Controls.Add(root);

            var content = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0)
            };
            root.Controls.Add(content);

            // Main Settings Card
            var mainCard = CreateMainSettingsCard();
            content.Controls.Add(mainCard);

            // Session Info Card
            var sessionCard = CreateSessionInfoCard();
            sessionCard.Margin = new Padding(0, 16, 0, 0);
            content.Controls.Add(sessionCard);

            // Add empty space at bottom for scroll
            var spacer = new Panel
            {
                Height = 20,
                Width = 850
            };
            content.Controls.Add(spacer);
        }

        private Panel CreateMainSettingsCard()
        {
            var card = new Panel
            {
                Width = 850,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                MinimumSize = new Size(850, 400)
            };

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#EFF6FF"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 0, 16, 0)
            };

            var headerIcon = new Label
            {
                Text = "👤",
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(0, 16)
            };
            header.Controls.Add(headerIcon);

            var headerTitle = new Label
            {
                Text = "MY SETTINGS (CASHIER)",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(40, 18)
            };
            header.Controls.Add(headerTitle);

            card.Controls.Add(header);

            // Content container
            var contentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(24, 16, 24, 16)
            };

            // Personal Information Section
            var personalSection = CreatePersonalInfoSection();
            contentPanel.Controls.Add(personalSection);

            // Password Management Section
            var passwordSection = CreatePasswordSection();
            passwordSection.Margin = new Padding(0, 16, 0, 0);
            contentPanel.Controls.Add(passwordSection);

            // Signature Setup Section
            var signatureSection = CreateSignatureSection();
            signatureSection.Margin = new Padding(0, 16, 0, 0);
            contentPanel.Controls.Add(signatureSection);

            // Receipt Preferences Section
            var receiptSection = CreateReceiptPreferencesSection();
            receiptSection.Margin = new Padding(0, 16, 0, 0);
            contentPanel.Controls.Add(receiptSection);

            // Save All Settings Button
            var saveAllPanel = new Panel
            {
                Width = 800,
                Height = 60,
                Margin = new Padding(0, 16, 0, 0)
            };

            var divider = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB"),
                Margin = new Padding(0, 0, 0, 16)
            };
            saveAllPanel.Controls.Add(divider);

            var btnSaveAllSettings = CreateButton("💾 Save All Settings", 180, ColorTranslator.FromHtml("#16A34A"), Color.White);
            btnSaveAllSettings.Location = new Point(0, 20);
            btnSaveAllSettings.Click += (s, e) => ShowToast("All settings saved successfully!");
            saveAllPanel.Controls.Add(btnSaveAllSettings);

            contentPanel.Controls.Add(saveAllPanel);

            card.Controls.Add(contentPanel);

            return card;
        }

        private Panel CreatePersonalInfoSection()
        {
            var section = new Panel
            {
                Width = 800,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 16)
            };

            // Section Header
            var header = new Label
            {
                Text = "👤 PERSONAL INFORMATION",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            section.Controls.Add(header);

            var headerLine = new Panel
            {
                Location = new Point(0, 25),
                Width = 768,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            section.Controls.Add(headerLine);

            int y = 40;

            // Name (read-only)
            var lblNameTitle = CreateLabel("Name", 0, y);
            section.Controls.Add(lblNameTitle);

            var lblName = new Label
            {
                Text = _name,
                Location = new Point(0, y + 20),
                Width = 370,
                Height = 32,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(lblName);

            // Employee ID (read-only)
            var lblEmpIdTitle = CreateLabel("Employee ID", 390, y);
            section.Controls.Add(lblEmpIdTitle);

            var lblEmpId = new Label
            {
                Text = _employeeId,
                Location = new Point(390, y + 20),
                Width = 370,
                Height = 32,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(lblEmpId);

            y += 70;

            // Email (editable)
            var lblEmailTitle = CreateLabel("Email", 0, y);
            section.Controls.Add(lblEmailTitle);

            txtEmail = new TextBox
            {
                Location = new Point(0, y + 20),
                Width = 370,
                Height = 32,
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(txtEmail);

            // Phone (editable)
            var lblPhoneTitle = CreateLabel("Phone", 390, y);
            section.Controls.Add(lblPhoneTitle);

            txtPhone = new TextBox
            {
                Location = new Point(390, y + 20),
                Width = 370,
                Height = 32,
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(txtPhone);

            y += 70;

            // Update Button
            var btnUpdateContact = CreateButton("💾 Update Contact Info", 180, ColorTranslator.FromHtml("#2563EB"), Color.White);
            btnUpdateContact.Location = new Point(0, y);
            btnUpdateContact.Click += (s, e) =>
            {
                _email = txtEmail.Text;
                _phone = txtPhone.Text;
                ShowToast("Contact information updated successfully!");
            };
            section.Controls.Add(btnUpdateContact);

            section.Height = y + 50;

            return section;
        }

        private Panel CreatePasswordSection()
        {
            var section = new Panel
            {
                Width = 800,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 16)
            };

            // Section Header
            var header = new Label
            {
                Text = "🔒 PASSWORD MANAGEMENT",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            section.Controls.Add(header);

            var headerLine = new Panel
            {
                Location = new Point(0, 25),
                Width = 768,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            section.Controls.Add(headerLine);

            int y = 40;

            // Current Password
            var lblCurrentPwd = CreateLabel("Current Password", 0, y);
            section.Controls.Add(lblCurrentPwd);

            txtCurrentPassword = new TextBox
            {
                Location = new Point(0, y + 20),
                Width = 370,
                Height = 32,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(txtCurrentPassword);

            chkShowCurrentPassword = new CheckBox
            {
                Text = "Show",
                Location = new Point(380, y + 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            chkShowCurrentPassword.CheckedChanged += (s, e) =>
            {
                txtCurrentPassword.UseSystemPasswordChar = !chkShowCurrentPassword.Checked;
            };
            section.Controls.Add(chkShowCurrentPassword);

            y += 70;

            // New Password
            var lblNewPwd = CreateLabel("New Password", 0, y);
            section.Controls.Add(lblNewPwd);

            txtNewPassword = new TextBox
            {
                Location = new Point(0, y + 20),
                Width = 370,
                Height = 32,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(txtNewPassword);

            chkShowNewPassword = new CheckBox
            {
                Text = "Show",
                Location = new Point(380, y + 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            chkShowNewPassword.CheckedChanged += (s, e) =>
            {
                txtNewPassword.UseSystemPasswordChar = !chkShowNewPassword.Checked;
            };
            section.Controls.Add(chkShowNewPassword);

            y += 70;

            // Confirm Password
            var lblConfirmPwd = CreateLabel("Confirm Password", 0, y);
            section.Controls.Add(lblConfirmPwd);

            txtConfirmPassword = new TextBox
            {
                Location = new Point(0, y + 20),
                Width = 370,
                Height = 32,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(txtConfirmPassword);

            chkShowConfirmPassword = new CheckBox
            {
                Text = "Show",
                Location = new Point(380, y + 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            chkShowConfirmPassword.CheckedChanged += (s, e) =>
            {
                txtConfirmPassword.UseSystemPasswordChar = !chkShowConfirmPassword.Checked;
            };
            section.Controls.Add(chkShowConfirmPassword);

            y += 80;

            // Change Password Button
            var btnChangePassword = CreateButton("🔒 Change Password", 180, ColorTranslator.FromHtml("#2563EB"), Color.White);
            btnChangePassword.Location = new Point(0, y);
            btnChangePassword.Click += BtnChangePassword_Click;
            section.Controls.Add(btnChangePassword);

            section.Height = y + 50;

            return section;
        }

        private Panel CreateSignatureSection()
        {
            var section = new Panel
            {
                Width = 800,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 16)
            };

            // Section Header
            var header = new Label
            {
                Text = "✍️ SIGNATURE SETUP",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            section.Controls.Add(header);

            var headerLine = new Panel
            {
                Location = new Point(0, 25),
                Width = 768,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            section.Controls.Add(headerLine);

            int y = 40;

            var lblInfo = new Label
            {
                Text = "For receipt authorization:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, y),
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(lblInfo);

            y += 30;

            // Current signature display
            var signaturePanel = new Panel
            {
                Location = new Point(0, y),
                Width = 400,
                Height = 36,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle
            };

            var signatureIcon = new Label
            {
                Text = "✍️",
                Location = new Point(12, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 12)
            };
            signaturePanel.Controls.Add(signatureIcon);

            lblCurrentSignature = new Label
            {
                Text = _currentSignature,
                Location = new Point(40, 10),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 9)
            };
            signaturePanel.Controls.Add(lblCurrentSignature);

            section.Controls.Add(signaturePanel);

            y += 50;

            // Buttons
            var btnUploadSignature = CreateButton("📤 Upload New Signature", 180, Color.White, ColorTranslator.FromHtml("#374151"));
            btnUploadSignature.Location = new Point(0, y);
            btnUploadSignature.Click += BtnUploadSignature_Click;
            section.Controls.Add(btnUploadSignature);

            var btnClearSignature = CreateButton("❌ Clear Signature", 140, Color.White, ColorTranslator.FromHtml("#374151"));
            btnClearSignature.Location = new Point(190, y);
            btnClearSignature.Click += (s, e) =>
            {
                _currentSignature = "";
                lblCurrentSignature.Text = "(No signature)";
                ShowToast("Signature cleared");
            };
            section.Controls.Add(btnClearSignature);

            y += 60;

            // Requirements panel
            var requirementsPanel = new Panel
            {
                Location = new Point(0, y),
                Width = 400,
                Height = 90,
                BackColor = ColorTranslator.FromHtml("#FEF3C7"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12)
            };

            var reqTitle = new Label
            {
                Text = "Requirements:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0),
                AutoSize = true
            };
            requirementsPanel.Controls.Add(reqTitle);

            var reqList = new Label
            {
                Text = "• PNG/JPG format\n• Max size: 500KB\n• White background",
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 8)
            };
            requirementsPanel.Controls.Add(reqList);

            section.Controls.Add(requirementsPanel);

            section.Height = y + 110;

            return section;
        }

        private Panel CreateReceiptPreferencesSection()
        {
            var section = new Panel
            {
                Width = 800,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 16)
            };

            // Section Header
            var header = new Label
            {
                Text = "🖨️ RECEIPT PREFERENCES",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };
            section.Controls.Add(header);

            var headerLine = new Panel
            {
                Location = new Point(0, 25),
                Width = 768,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#D1D5DB")
            };
            section.Controls.Add(headerLine);

            int y = 40;

            // Default Printer
            var lblPrinter = CreateLabel("Default Printer", 0, y);
            section.Controls.Add(lblPrinter);

            cmbDefaultPrinter = new ComboBox
            {
                Location = new Point(0, y + 20),
                Width = 300,
                Height = 32,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbDefaultPrinter.Items.AddRange(new object[]
            {
                "HP LaserJet Pro (001)",
                "Canon PIXMA (002)",
                "Epson L3110 (003)",
                "Brother HL-L2321D (004)"
            });
            section.Controls.Add(cmbDefaultPrinter);

            y += 70;

            // Auto-print checkbox
            chkAutoPrintReceipts = new CheckBox
            {
                Text = "Auto-print receipts after payment",
                Location = new Point(0, y),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9)
            };
            section.Controls.Add(chkAutoPrintReceipts);

            y += 40;

            // Number of Copies
            var lblCopies = CreateLabel("Number of Copies", 0, y);
            section.Controls.Add(lblCopies);

            cmbCopies = new ComboBox
            {
                Location = new Point(0, y + 20),
                Width = 100,
                Height = 32,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbCopies.Items.AddRange(new object[] { "1", "2", "3", "4" });
            section.Controls.Add(cmbCopies);

            y += 70;

            // Buttons
            var btnTestPrinter = CreateButton("🖨️ Test Printer", 120, Color.White, ColorTranslator.FromHtml("#374151"));
            btnTestPrinter.Location = new Point(0, y);
            btnTestPrinter.Click += BtnTestPrinter_Click;
            section.Controls.Add(btnTestPrinter);

            var btnSavePreferences = CreateButton("💾 Save Preferences", 150, ColorTranslator.FromHtml("#2563EB"), Color.White);
            btnSavePreferences.Location = new Point(130, y);
            btnSavePreferences.Click += (s, e) =>
            {
                _defaultPrinter = cmbDefaultPrinter.SelectedItem?.ToString() ?? _defaultPrinter;
                _autoPrint = chkAutoPrintReceipts.Checked;
                int.TryParse(cmbCopies.SelectedItem?.ToString(), out _copies);
                ShowToast("Receipt preferences saved!");
            };
            section.Controls.Add(btnSavePreferences);

            section.Height = y + 50;

            return section;
        }

        private Panel CreateSessionInfoCard()
        {
            var card = new Panel
            {
                Width = 850,
                Height = 300,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#F3E8FF"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 0, 16, 0)
            };

            var headerIcon = new Label
            {
                Text = "📊",
                AutoSize = true,
                Font = new Font("Segoe UI", 16),
                Location = new Point(0, 16)
            };
            header.Controls.Add(headerIcon);

            var headerTitle = new Label
            {
                Text = "SESSION INFO",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(40, 18)
            };
            header.Controls.Add(headerTitle);

            card.Controls.Add(header);

            // Content
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 16)
            };

            int y = 16;

            // Login time
            var lblLoginTitle = new Label
            {
                Text = "Logged in since:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 9),
                Location = new Point(16, y)
            };
            contentPanel.Controls.Add(lblLoginTitle);

            lblLoginTime = new Label
            {
                Text = _loginTime,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(16, y + 20)
            };
            contentPanel.Controls.Add(lblLoginTime);

            // Last activity
            var lblLastTitle = new Label
            {
                Text = "Last activity:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 9),
                Location = new Point(200, y)
            };
            contentPanel.Controls.Add(lblLastTitle);

            lblLastActivity = new Label
            {
                Text = _lastActivity,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(200, y + 20)
            };
            contentPanel.Controls.Add(lblLastActivity);

            y += 60;

            // Divider
            var divider = new Panel
            {
                Location = new Point(16, y),
                Width = 780,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#E5E7EB")
            };
            contentPanel.Controls.Add(divider);

            y += 20;

            // Today's Stats
            var lblStatsTitle = new Label
            {
                Text = "Today's Stats:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(16, y)
            };
            contentPanel.Controls.Add(lblStatsTitle);

            y += 30;

            // Payments
            var lblPaymentsTitle = new Label
            {
                Text = "• Payments:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(32, y),
                Font = new Font("Segoe UI", 9)
            };
            contentPanel.Controls.Add(lblPaymentsTitle);

            lblPaymentsToday = new Label
            {
                Text = _paymentsToday.ToString(),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(150, y),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            contentPanel.Controls.Add(lblPaymentsToday);

            y += 28;

            // Loans Released
            var lblLoansTitle = new Label
            {
                Text = "• Loans Released:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(32, y),
                Font = new Font("Segoe UI", 9)
            };
            contentPanel.Controls.Add(lblLoansTitle);

            lblLoansReleased = new Label
            {
                Text = _loansReleasedToday.ToString(),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(150, y),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            contentPanel.Controls.Add(lblLoansReleased);

            y += 28;

            // Receipts Printed
            var lblReceiptsTitle = new Label
            {
                Text = "• Receipts Printed:",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(32, y),
                Font = new Font("Segoe UI", 9)
            };
            contentPanel.Controls.Add(lblReceiptsTitle);

            lblReceiptsPrinted = new Label
            {
                Text = _receiptsPrintedToday.ToString(),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(150, y),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            contentPanel.Controls.Add(lblReceiptsPrinted);

            y += 40;

            // Divider
            var divider2 = new Panel
            {
                Location = new Point(16, y),
                Width = 780,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#E5E7EB")
            };
            contentPanel.Controls.Add(divider2);

            y += 20;

            // View Activity Log Button
            var btnViewActivityLog = CreateButton("📊 View My Activity Log", 200, Color.White, ColorTranslator.FromHtml("#374151"));
            btnViewActivityLog.Location = new Point(16, y);
            btnViewActivityLog.Width = 780;
            btnViewActivityLog.Height = 36;
            btnViewActivityLog.Font = new Font("Segoe UI", 10);
            btnViewActivityLog.Click += (s, e) => ShowToast("Opening activity log...");
            contentPanel.Controls.Add(btnViewActivityLog);

            card.Controls.Add(contentPanel);

            return card;
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(x, y)
            };
        }

        private Button CreateButton(string text, int width, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = 36,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }

        private void LoadData()
        {
            if (txtEmail != null) txtEmail.Text = _email;
            if (txtPhone != null) txtPhone.Text = _phone;
            if (lblCurrentSignature != null) lblCurrentSignature.Text = _currentSignature;

            // Select default printer
            if (cmbDefaultPrinter != null) cmbDefaultPrinter.SelectedIndex = 0;
            if (chkAutoPrintReceipts != null) chkAutoPrintReceipts.Checked = _autoPrint;
            if (cmbCopies != null) cmbCopies.SelectedIndex = _copies - 1;

            if (lblLoginTime != null) lblLoginTime.Text = _loginTime;
            if (lblLastActivity != null) lblLastActivity.Text = _lastActivity;
            if (lblPaymentsToday != null) lblPaymentsToday.Text = _paymentsToday.ToString();
            if (lblLoansReleased != null) lblLoansReleased.Text = _loansReleasedToday.ToString();
            if (lblReceiptsPrinted != null) lblReceiptsPrinted.Text = _receiptsPrintedToday.ToString();
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            var currentPwd = txtCurrentPassword?.Text ?? "";
            var newPwd = txtNewPassword?.Text ?? "";
            var confirmPwd = txtConfirmPassword?.Text ?? "";

            if (string.IsNullOrWhiteSpace(currentPwd) ||
                string.IsNullOrWhiteSpace(newPwd) ||
                string.IsNullOrWhiteSpace(confirmPwd))
            {
                ShowToast("Please fill in all password fields", true);
                return;
            }

            if (newPwd != confirmPwd)
            {
                ShowToast("New password and confirm password do not match", true);
                return;
            }

            if (newPwd.Length < 8)
            {
                ShowToast("Password must be at least 8 characters long", true);
                return;
            }

            ShowToast("Password changed successfully!");

            if (txtCurrentPassword != null) txtCurrentPassword.Text = "";
            if (txtNewPassword != null) txtNewPassword.Text = "";
            if (txtConfirmPassword != null) txtConfirmPassword.Text = "";
        }

        private void BtnUploadSignature_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg";
                ofd.Title = "Select Signature Image";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(ofd.FileName);

                    // Check file type
                    var extension = fileInfo.Extension.ToLowerInvariant();
                    if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                    {
                        ShowToast("Please upload PNG or JPG format only", true);
                        return;
                    }

                    // Check file size (500KB)
                    if (fileInfo.Length > 500 * 1024)
                    {
                        ShowToast("File size must be less than 500KB", true);
                        return;
                    }

                    _currentSignature = fileInfo.Name;
                    if (lblCurrentSignature != null)
                        lblCurrentSignature.Text = fileInfo.Name;

                    ShowToast("Signature uploaded successfully!");
                }
            }
        }

        private void BtnTestPrinter_Click(object sender, EventArgs e)
        {
            var selectedPrinter = cmbDefaultPrinter?.SelectedItem?.ToString() ?? "Unknown";
            ShowToast($"Testing printer: {selectedPrinter}...");

            var timer = new Timer { Interval = 1500 };
            timer.Tick += (s, ev) =>
            {
                timer.Stop();
                timer.Dispose();
                ShowToast("Printer test successful!");
            };
            timer.Start();
        }

        #region Toast

        private void BuildToast()
        {
            _toastPanel = new Panel
            {
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#111827"),
                Padding = new Padding(16, 12, 16, 12),
                Visible = false
            };

            _toastLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };
            _toastPanel.Controls.Add(_toastLabel);
            Controls.Add(_toastPanel);
            _toastPanel.BringToFront();

            _toastTimer = new Timer { Interval = 2500 };
            _toastTimer.Tick += (s, e) => { _toastTimer.Stop(); _toastPanel.Visible = false; };

            Resize += (s, e) => PositionToast();
        }

        private void PositionToast()
        {
            if (_toastPanel == null) return;
            _toastPanel.Left = ClientSize.Width - _toastPanel.Width - 24;
            _toastPanel.Top = 24;
        }

        private void ShowToast(string message, bool isError = false)
        {
            if (_toastPanel == null || _toastLabel == null) return;

            _toastPanel.BackColor = isError ?
                ColorTranslator.FromHtml("#DC2626") :
                ColorTranslator.FromHtml("#111827");

            _toastLabel.Text = message;
            _toastPanel.Visible = true;
            _toastPanel.BringToFront();
            _toastPanel.PerformLayout();
            PositionToast();

            _toastTimer.Stop();
            _toastTimer.Start();
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Name = "CashierSettings";
            this.Text = "Cashier Settings";
            this.ResumeLayout(false);
        }
    }
}