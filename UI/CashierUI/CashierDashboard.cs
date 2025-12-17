using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierDashboard : Form
    {
        private string _cashierName = "Cashier";

        // Shell
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblCashierName;

        private Panel sidebarPanel;
        private Panel contentPanel;

        // Payment processing
        private Panel paymentPanel;
        private TextBox txtLoanNo;
        private Label lblCustomer;
        private Label lblBalance;
        private TextBox txtPayment;
        private ComboBox cmbMethod;

        private Label lblAllocInterest;
        private Label lblAllocPrincipal;
        private Label lblAllocPenalty;
        private Label lblNewBalance;

        private Button btnProcessPayment;
        private Button btnPrintReceipt;

        // Recent transactions
        private Panel recentPanel;
        private DataGridView gridRecent;

        // In-memory state for sample
        private string currentCustomer = "Juan Dela Cruz";
        private decimal currentBalance = 35657m;
        private int receiptCounter = 2;

        public CashierDashboard()
        {
            InitializeComponent();
            BuildUI();
        }

        public void SetCashierName(string name)
        {
            _cashierName = string.IsNullOrWhiteSpace(name) ? "Cashier" : name;
            lblCashierName.Text = $"[{_cashierName}]";
            headerPanel.PerformLayout();
        }

        private void BuildUI()
        {
            // Form
            Text = "Cashier Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");

            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 12, 16, 12)
            };
            lblTitle = new Label
            {
                Text = "💰 CASHIER DASHBOARD",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Location = new Point(16, 16)
            };
            lblCashierName = new Label
            {
                Text = $"[{_cashierName}]",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#6B7280")
            };
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblCashierName);
            headerPanel.Resize += (s, e) =>
            {
                lblCashierName.Location = new Point(headerPanel.Width - lblCashierName.Width - 16, 16);
            };

            // Sidebar
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            BuildSidebar(sidebarPanel);

            // Content
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true
            };

            // Sections
            BuildPaymentPanel();
            BuildRecentPanel();

            // Layout
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(headerPanel);

            // Default content
            ShowPaymentProcessing();
        }

        private void BuildSidebar(Panel host)
        {
            host.Controls.Clear();
            var items = new (string Key, string Text)[]
            {
                ("process", "🔄 Process Payment"),
                ("release", "💰 Release Loan"),
                ("daily", "📄 Daily Report"),
                ("receipts", "🧾 Receipts"),
                ("reports", "📊 Reports"),
                ("settings", "⚙️ Settings"),
                ("logout", "🚪 Logout"),
            };

            int y = 10;
            foreach (var it in items)
            {
                var btn = new Button
                {
                    Tag = it.Key,
                    Text = it.Text,
                    Location = new Point(10, y),
                    Size = new Size(host.Width - 20, 38),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = it.Key == "process" ? ColorTranslator.FromHtml("#E8F4FF") : Color.White
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += Sidebar_Click;
                host.Controls.Add(btn);
                y += 44;
            }
        }

        private void Sidebar_Click(object sender, EventArgs e)
        {
            foreach (Control c in sidebarPanel.Controls)
            {
                if (c is Button b) b.BackColor = Color.White;
            }
            var btnThis = sender as Button;
            if (btnThis != null) btnThis.BackColor = ColorTranslator.FromHtml("#E8F4FF");

            var key = btnThis?.Tag as string;
            switch (key)
            {
                case "process":
                    ShowPaymentProcessing();
                    break;
                case "release":
                case "daily":
                case "receipts":
                case "reports":
                case "settings":
                    ShowPlaceholder($"{btnThis.Text} view coming soon");
                    break;
                case "logout":
                    Close();
                    break;
            }
        }

        private void ShowPaymentProcessing()
        {
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            // Payment panel first
            contentPanel.Controls.Add(paymentPanel);

            // Then recent transactions
            recentPanel.Top = paymentPanel.Bottom + 10;
            contentPanel.Controls.Add(recentPanel);

            contentPanel.Resize += (s, e) =>
            {
                paymentPanel.Width = contentPanel.ClientSize.Width - 20;
                recentPanel.Width = contentPanel.ClientSize.Width - 20;
                recentPanel.Top = paymentPanel.Bottom + 10;
            };
            contentPanel.ResumeLayout();
        }

        private void ShowPlaceholder(string text)
        {
            contentPanel.Controls.Clear();
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(20, 20),
                ForeColor = ColorTranslator.FromHtml("#6B7280")
            };
            contentPanel.Controls.Add(lbl);
        }

        private void BuildPaymentPanel()
        {
            paymentPanel = new Panel
            {
                Left = 10,
                Top = 10,
                Width = 1000,
                Height = 320,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var header = new Label
            {
                Text = "PAYMENT PROCESSING",
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#DBEAFE")
            };
            paymentPanel.Controls.Add(header);

            // Body host
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
            paymentPanel.Controls.Add(body);

            int xCol1 = 14;
            int xCol2 = 360;
            int y = 12;
            int labelH = 22;

            // Loan #
            var lblLoan = new Label { Text = "Loan #:", Location = new Point(xCol1, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            txtLoanNo = new TextBox { Location = new Point(xCol1 + 70, y), Width = 220, Text = "LN-2024-001" };
            body.Controls.Add(lblLoan);
            body.Controls.Add(txtLoanNo);

            // Customer
            y += 38;
            var lblCust = new Label { Text = "Customer:", Location = new Point(xCol1, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            lblCustomer = new Label { Text = currentCustomer, Location = new Point(xCol1 + 70, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827") };
            body.Controls.Add(lblCust);
            body.Controls.Add(lblCustomer);

            // Balance
            y += 28;
            var lblBal = new Label { Text = "Balance:", Location = new Point(xCol1, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            lblBalance = new Label { Text = FormatPeso(currentBalance), Location = new Point(xCol1 + 70, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#111827") };
            body.Controls.Add(lblBal);
            body.Controls.Add(lblBalance);

            // Payment amount
            y += 38;
            var lblPay = new Label { Text = "Payment:", Location = new Point(xCol1, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            txtPayment = new TextBox { Location = new Point(xCol1 + 70, y), Width = 220, TextAlign = HorizontalAlignment.Right };
            txtPayment.KeyPress += Amount_KeyPress;
            txtPayment.TextChanged += (s, e) => RecomputeAllocation();
            body.Controls.Add(lblPay);
            body.Controls.Add(txtPayment);

            // Method
            y += 38;
            var lblMethod = new Label { Text = "Method:", Location = new Point(xCol1, y + 4), AutoSize = true, ForeColor = ColorTranslator.FromHtml("#374151") };
            cmbMethod = new ComboBox { Location = new Point(xCol1 + 70, y), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMethod.Items.AddRange(new object[] { "Cash", "GCash", "Bank" });
            cmbMethod.SelectedIndex = 0;
            body.Controls.Add(lblMethod);
            body.Controls.Add(cmbMethod);

            // Allocation "box"
            var allocBox = new Panel
            {
                Location = new Point(xCol2, 12),
                Size = new Size(320, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var allocHeader = new Label
            {
                Text = "Allocation",
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 4, 0, 0),
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            allocBox.Controls.Add(allocHeader);

            lblAllocInterest = new Label { Text = "• Interest:    ₱0.00", Location = new Point(10, 40), AutoSize = true };
            lblAllocPrincipal = new Label { Text = "• Principal:   ₱0.00", Location = new Point(10, 64), AutoSize = true };
            lblAllocPenalty = new Label { Text = "• Penalty:     ₱0.00", Location = new Point(10, 88), AutoSize = true };
            lblNewBalance = new Label { Text = "• New Balance: " + FormatPeso(currentBalance), Location = new Point(10, 112), AutoSize = true };

            allocBox.Controls.Add(lblAllocInterest);
            allocBox.Controls.Add(lblAllocPrincipal);
            allocBox.Controls.Add(lblAllocPenalty);
            allocBox.Controls.Add(lblNewBalance);
            body.Controls.Add(allocBox);

            // Action buttons
            btnProcessPayment = new Button
            {
                Text = "Process Payment",
                Location = new Point(xCol2, allocBox.Bottom + 20),
                Width = 150,
                Height = 32,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnProcessPayment.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            btnProcessPayment.Click += ProcessPayment_Click;

            btnPrintReceipt = new Button
            {
                Text = "Print Receipt",
                Location = new Point(btnProcessPayment.Right + 8, allocBox.Bottom + 20),
                Width = 140,
                Height = 32,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrintReceipt.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnPrintReceipt.Click += (s, e) => MessageBox.Show("Printing receipt...", "Receipt");

            body.Controls.Add(btnProcessPayment);
            body.Controls.Add(btnPrintReceipt);
        }

        private void BuildRecentPanel()
        {
            recentPanel = new Panel
            {
                Left = 10,
                Top = 10,
                Width = 1000,
                Height = 260,
                BackColor = Color.Transparent
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle
            };
            var lbl = new Label
            {
                Text = "RECENT TRANSACTIONS",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Location = new Point(12, 10)
            };
            header.Controls.Add(lbl);

            gridRecent = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            gridRecent.Columns.Add("Time", "Time");
            gridRecent.Columns.Add("Customer", "Customer");
            gridRecent.Columns.Add("Amount", "Amount");
            gridRecent.Columns.Add("Receipt", "Receipt #");

            var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            host.Controls.Add(gridRecent);

            // Seed sample rows from the layout
            gridRecent.Rows.Add("9:30", "Maria S.", "₱2,150", "OR-001");
            gridRecent.Rows.Add("10:15", "Juan D.", "₱4,442", "OR-002");
            gridRecent.Rows.Add("11:00", "Pedro R.", "₱1,500", "OR-003");

            recentPanel.Controls.Add(host);
            recentPanel.Controls.Add(header);
        }

        private void ProcessPayment_Click(object sender, EventArgs e)
        {
            if (!TryParseAmount(txtPayment.Text, out var amount) || amount <= 0m)
            {
                MessageBox.Show("Enter a valid payment amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simple allocation example: 10% interest, 0 penalty, rest principal
            var interest = Math.Round(amount * 0.10m, 2);
            var penalty = 0.00m;
            var principal = Math.Max(0m, amount - interest - penalty);

            // Do not allow principal to exceed current balance
            if (principal > currentBalance) principal = currentBalance;
            var newBalance = Math.Max(0m, currentBalance - principal);

            // Update UI
            lblAllocInterest.Text = $"• Interest:    {FormatPeso(interest)}";
            lblAllocPrincipal.Text = $"• Principal:   {FormatPeso(principal)}";
            lblAllocPenalty.Text = $"• Penalty:     {FormatPeso(penalty)}";
            lblNewBalance.Text = $"• New Balance: {FormatPeso(newBalance)}";

            // Update state and header balance
            currentBalance = newBalance;
            lblBalance.Text = FormatPeso(currentBalance);

            // Append recent transaction
            var timeText = DateTime.Now.ToString("HH:mm");
            var shortName = ToShortName(currentCustomer);
            var receipt = NextReceiptNo();
            gridRecent.Rows.Insert(0, timeText, shortName, FormatPeso(amount), receipt);

            MessageBox.Show("Payment processed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Amount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, control, one decimal separator
            char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != dec)
            {
                e.Handled = true;
            }
            if (e.KeyChar == dec && (sender as TextBox)?.Text.IndexOf(dec) >= 0)
            {
                e.Handled = true;
            }
        }

        private void RecomputeAllocation()
        {
            if (!TryParseAmount(txtPayment.Text, out var amount) || amount <= 0m)
            {
                lblAllocInterest.Text = "• Interest:    ₱0.00";
                lblAllocPrincipal.Text = "• Principal:   ₱0.00";
                lblAllocPenalty.Text = "• Penalty:     ₱0.00";
                lblNewBalance.Text = "• New Balance: " + FormatPeso(currentBalance);
                return;
            }
            var interest = Math.Round(amount * 0.10m, 2);
            var penalty = 0.00m;
            var principal = Math.Max(0m, amount - interest - penalty);
            if (principal > currentBalance) principal = currentBalance;
            var newBalance = Math.Max(0m, currentBalance - principal);

            lblAllocInterest.Text = $"• Interest:    {FormatPeso(interest)}";
            lblAllocPrincipal.Text = $"• Principal:   {FormatPeso(principal)}";
            lblAllocPenalty.Text = $"• Penalty:     {FormatPeso(penalty)}";
            lblNewBalance.Text = $"• New Balance: {FormatPeso(newBalance)}";
        }

        private bool TryParseAmount(string s, out decimal value)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                value = 0m;
                return false;
            }
            var cleaned = s.Replace("₱", "").Replace(",", "").Trim();
            return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        private string FormatPeso(decimal amount)
        {
            return "₱" + amount.ToString("N2", CultureInfo.CurrentCulture);
        }

        private string ToShortName(string fullName)
        {
            // Simple "Juan D." style
            var parts = (fullName ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1) return fullName ?? "";
            return parts[0] + " " + parts[1][0] + ".";
        }

        private string NextReceiptNo()
        {
            receiptCounter++;
            return "OR-" + receiptCounter.ToString("D3");
        }
    }
}
