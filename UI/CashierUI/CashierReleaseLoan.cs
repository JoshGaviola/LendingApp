using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierReleaseLoan : Form
    {
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblCashierName;

        private Panel sidebarPanel;
        private Panel contentPanel;

        // Search
        private TextBox txtSearch;

        // Loans list
        private DataGridView gridLoans;
        private Button btnSelectAll;
        private Button btnDeselectAll;

        // Release configuration
        private DateTimePicker dtReleaseDate;
        private RadioButton rbCash;
        private RadioButton rbBankTransfer;
        private RadioButton rbCheck;
        private RadioButton rbGCash;
        private Button btnPrintContract;
        private Button btnPreviewContract;
        private Button btnReleaseSelected;

        // Summary
        private Label lblSummaryReleased;
        private Label lblSummaryTotalAmount;
        private Label lblSummaryLastRelease;

        // In-memory sample data
        private string _cashierName = "Cashier";
        private DataTable loansTable;
        private int releasesTodayCount = 2;
        private decimal releasesTodayTotal = 80000m;
        private DateTime? lastReleaseTime = DateTime.Today.AddHours(10).AddMinutes(30);

        public CashierReleaseLoan()
        {
            InitializeComponent();
            BuildUI();
            SeedLoans();
            ApplyFilter();
            UpdateSummary();
        }

        public void SetCashierName(string name)
        {
            _cashierName = string.IsNullOrWhiteSpace(name) ? "Cashier" : name;
            lblCashierName.Text = $"[{_cashierName}]";
            headerPanel.PerformLayout();
        }

        private void BuildUI()
        {
            // Form shell
            Text = "Cashier Dashboard - Release Loan";
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
                Text = "💰 CASHIER DASHBOARD - RELEASE LOAN",
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

            // Sidebar (visual parity with dashboard)
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
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(headerPanel);

            BuildReleaseContent();
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
                    BackColor = it.Key == "release" ? ColorTranslator.FromHtml("#E8F4FF") : Color.White
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
                case "daily":
                case "receipts":
                case "reports":
                case "settings":
                    MessageBox.Show($"{btnThis.Text} view coming soon", "Info");
                    break;
                case "release":
                    // already here
                    break;
                case "logout":
                    Close();
                    break;
            }
        }

        private void BuildReleaseContent()
        {
            contentPanel.Controls.Clear();

            var container = new Panel
            {
                Left = 10,
                Top = 10,
                Width = 1100,
                Height = 700,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(container);

            var header = new Label
            {
                Text = "LOAN RELEASE",
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#DBEAFE")
            };
            container.Controls.Add(header);

            var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
            container.Controls.Add(body);

            // Search box area
            var searchBox = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(520, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblSearchTitle = new Label
            {
                Text = "🔍 Search:",
                Location = new Point(10, 14),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            txtSearch = new TextBox { Location = new Point(90, 10), Width = 410 };
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            searchBox.Controls.Add(lblSearchTitle);
            searchBox.Controls.Add(txtSearch);
            body.Controls.Add(searchBox);

            // Loans list area
            var loansHeader = new Label
            {
                Text = "LOANS TO RELEASE TODAY (" + DateTime.Today.ToString("MMM dd, yyyy", CultureInfo.CurrentCulture) + ")",
                Location = new Point(12, searchBox.Bottom + 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50")
            };
            body.Controls.Add(loansHeader);

            gridLoans = new DataGridView
            {
                Location = new Point(12, loansHeader.Bottom + 6),
                Size = new Size(520, 240),
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            var colSelect = new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "Select", Width = 60, FillWeight = 20 };
            var colCustomer = new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "Customer" };
            var colAmount = new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount" };
            var colStatus = new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status" };
            gridLoans.Columns.AddRange(colSelect, colCustomer, colAmount, colStatus);
            body.Controls.Add(gridLoans);

            btnSelectAll = new Button
            {
                Text = "Select All",
                Location = new Point(12, gridLoans.Bottom + 8),
                Width = 100,
                Height = 30,
                BackColor = ColorTranslator.FromHtml("#E5E7EB"),
                FlatStyle = FlatStyle.Flat
            };
            btnSelectAll.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnSelectAll.Click += (s, e) => SetAllSelection(true);
            body.Controls.Add(btnSelectAll);

            btnDeselectAll = new Button
            {
                Text = "Deselect All",
                Location = new Point(btnSelectAll.Right + 8, gridLoans.Bottom + 8),
                Width = 120,
                Height = 30,
                BackColor = ColorTranslator.FromHtml("#E5E7EB"),
                FlatStyle = FlatStyle.Flat
            };
            btnDeselectAll.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnDeselectAll.Click += (s, e) => SetAllSelection(false);
            body.Controls.Add(btnDeselectAll);

            // Release configuration box
            var cfgBox = new Panel
            {
                Location = new Point(searchBox.Right + 20, 12),
                Size = new Size(520, 260),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var cfgHeader = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = ColorTranslator.FromHtml("#F9FAFB") };
            var cfgTitle = new Label
            {
                Text = "RELEASE CONFIGURATION",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            cfgHeader.Controls.Add(cfgTitle);
            cfgBox.Controls.Add(cfgHeader);

            var lblReleaseDate = new Label { Text = "Release Date:", Location = new Point(12, 52), AutoSize = true };
            dtReleaseDate = new DateTimePicker
            {
                Location = new Point(110, 48),
                Width = 180,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MMM dd, yyyy",
                Value = DateTime.Today
            };

            var lblMethod = new Label { Text = "Disbursement Method:", Location = new Point(12, 88), AutoSize = true };
            rbCash = new RadioButton { Text = "Cash", Location = new Point(30, 112), AutoSize = true, Checked = true };
            rbBankTransfer = new RadioButton { Text = "Bank Transfer", Location = new Point(120, 112), AutoSize = true };
            rbCheck = new RadioButton { Text = "Check", Location = new Point(250, 112), AutoSize = true };
            rbGCash = new RadioButton { Text = "GCash", Location = new Point(330, 112), AutoSize = true };

            btnPrintContract = new Button
            {
                Text = "Print Contract",
                Location = new Point(12, 160),
                Width = 120,
                Height = 30,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrintContract.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnPrintContract.Click += (s, e) => MessageBox.Show("Printing contract preview...", "Contract");

            btnPreviewContract = new Button
            {
                Text = "Preview Contract",
                Location = new Point(btnPrintContract.Right + 8, 160),
                Width = 140,
                Height = 30,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPreviewContract.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnPreviewContract.Click += (s, e) => MessageBox.Show("Opening contract preview...", "Contract");

            btnReleaseSelected = new Button
            {
                Text = "RELEASE SELECTED LOANS",
                Location = new Point(12, 204),
                Width = 240,
                Height = 34,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnReleaseSelected.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            btnReleaseSelected.Click += ReleaseSelected_Click;

            cfgBox.Controls.Add(lblReleaseDate);
            cfgBox.Controls.Add(dtReleaseDate);
            cfgBox.Controls.Add(lblMethod);
            cfgBox.Controls.Add(rbCash);
            cfgBox.Controls.Add(rbBankTransfer);
            cfgBox.Controls.Add(rbCheck);
            cfgBox.Controls.Add(rbGCash);
            cfgBox.Controls.Add(btnPrintContract);
            cfgBox.Controls.Add(btnPreviewContract);
            cfgBox.Controls.Add(btnReleaseSelected);
            body.Controls.Add(cfgBox);

            // Summary box
            var summaryBox = new Panel
            {
                Location = new Point(searchBox.Right + 20, cfgBox.Bottom + 16),
                Size = new Size(520, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var summaryHeader = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = ColorTranslator.FromHtml("#F9FAFB") };
            var summaryTitle = new Label
            {
                Text = "TODAY'S RELEASES SUMMARY",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            summaryHeader.Controls.Add(summaryTitle);
            summaryBox.Controls.Add(summaryHeader);

            lblSummaryReleased = new Label { Text = "• Loans Released: 0", Location = new Point(12, 56), AutoSize = true };
            lblSummaryTotalAmount = new Label { Text = "• Total Amount: ₱0.00", Location = new Point(12, 80), AutoSize = true };
            lblSummaryLastRelease = new Label { Text = "• Last Release: —", Location = new Point(12, 104), AutoSize = true };

            summaryBox.Controls.Add(lblSummaryReleased);
            summaryBox.Controls.Add(lblSummaryTotalAmount);
            summaryBox.Controls.Add(lblSummaryLastRelease);
            body.Controls.Add(summaryBox);

            // Resize behavior
            contentPanel.Resize += (s, e) =>
            {
                container.Width = contentPanel.ClientSize.Width - 20;
                container.Height = contentPanel.ClientSize.Height - 20;

                // Keep left column widths
                gridLoans.Width = Math.Min(600, container.Width / 2 - 30);
                searchBox.Width = gridLoans.Width;
                loansHeader.MaximumSize = new Size(gridLoans.Width, 0);

                // Right column align
                cfgBox.Left = searchBox.Right + 20;
                summaryBox.Left = searchBox.Right + 20;
                cfgBox.Width = container.Width - (cfgBox.Left + 12);
                summaryBox.Width = cfgBox.Width;
            };
        }

        private void SeedLoans()
        {
            loansTable = new DataTable();
            loansTable.Columns.Add("Customer", typeof(string));
            loansTable.Columns.Add("Amount", typeof(decimal));
            loansTable.Columns.Add("Status", typeof(string));
            loansTable.Rows.Add("Juan DC", 50000m, "Approved");
            loansTable.Rows.Add("Maria S.", 30000m, "Approved");
            loansTable.Rows.Add("Pedro R.", 20000m, "Approved");
            loansTable.Rows.Add("Liza K.", 15000m, "Approved");
        }

        private void ApplyFilter()
        {
            var q = (txtSearch.Text ?? "").Trim();
            gridLoans.Rows.Clear();
            foreach (DataRow row in loansTable.Rows)
            {
                var name = Convert.ToString(row["Customer"] ?? "");
                var amt = Convert.ToDecimal(row["Amount"]);
                var status = Convert.ToString(row["Status"] ?? "");
                if (string.IsNullOrEmpty(q) || name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    gridLoans.Rows.Add(true, name, FormatPeso(amt), status);
                }
            }
        }

        private void SetAllSelection(bool check)
        {
            foreach (DataGridViewRow r in gridLoans.Rows)
            {
                var cell = r.Cells["Select"] as DataGridViewCheckBoxCell;
                if (cell != null) cell.Value = check;
            }
        }

        private void ReleaseSelected_Click(object sender, EventArgs e)
        {
            int releasedCount = 0;
            decimal releasedTotal = 0m;

            foreach (DataGridViewRow r in gridLoans.Rows)
            {
                bool selected = false;
                var selCell = r.Cells["Select"] as DataGridViewCheckBoxCell;
                if (selCell != null && selCell.Value != null)
                {
                    selected = Convert.ToBoolean(selCell.Value);
                }
                if (!selected) continue;

                var amountText = Convert.ToString(r.Cells["Amount"].Value ?? "₱0");
                decimal amount;
                if (!TryParseAmount(amountText, out amount)) amount = 0m;

                r.Cells["Status"].Value = "Released";
                releasedCount++;
                releasedTotal += amount;
            }

            if (releasedCount == 0)
            {
                MessageBox.Show("No loans selected.", "Release Loans", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            releasesTodayCount += releasedCount;
            releasesTodayTotal += releasedTotal;
            lastReleaseTime = DateTime.Now;

            UpdateSummary();

            MessageBox.Show($"Released {releasedCount} loan(s) via {GetMethod()} on {dtReleaseDate.Value.ToString("MMM dd, yyyy")}.\nTotal: {FormatPeso(releasedTotal)}",
                "Release Loans", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateSummary()
        {
            lblSummaryReleased.Text = "• Loans Released: " + releasesTodayCount;
            lblSummaryTotalAmount.Text = "• Total Amount: " + FormatPeso(releasesTodayTotal);
            lblSummaryLastRelease.Text = "• Last Release: " + (lastReleaseTime.HasValue ? lastReleaseTime.Value.ToString("hh:mm tt", CultureInfo.CurrentCulture) : "—");
        }

        private string GetMethod()
        {
            if (rbBankTransfer.Checked) return "Bank Transfer";
            if (rbCheck.Checked) return "Check";
            if (rbGCash.Checked) return "GCash";
            return "Cash";
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
            return "₱" + amount.ToString("N0", CultureInfo.CurrentCulture);
        }
    }
}
