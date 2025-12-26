using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class AdminAuditLogControl : UserControl
    {
        private DataGridView auditTable;
        private TextBox dateFromBox;
        private TextBox dateToBox;
        private ComboBox userCombo;
        private ComboBox actionCombo;
        private Panel detailsPanel;
        private Button searchBtn;
        private Button exportBtn;
        private Button viewBtn;
        private Button printBtn;
        private Panel mainPanel; // Add this line

        public AdminAuditLogControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            // Main container with AutoScroll
            mainPanel = new Panel // Initialize here
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            int currentY = 20;

            // Header
            Panel header = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 60), // Fixed width for now
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label title = new Label
            {
                Text = "AUDIT LOG",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            header.Controls.Add(title);
            mainPanel.Controls.Add(header);
            currentY += 70;

            // Filter Panel
            Panel filterPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 120), // Fixed width
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Date filters
            int y = 15;
            Label dateLabel = new Label
            {
                Text = "Date Range:",
                Location = new Point(15, y),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9)
            };

            dateFromBox = new TextBox
            {
                Location = new Point(100, y),
                Size = new Size(120, 25),
                Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                Font = new Font("Segoe UI", 9)
            };

            Label toLabel = new Label
            {
                Text = "to",
                Location = new Point(225, y),
                Size = new Size(20, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };

            dateToBox = new TextBox
            {
                Location = new Point(250, y),
                Size = new Size(120, 25),
                Text = DateTime.Now.ToString("yyyy-MM-dd"),
                Font = new Font("Segoe UI", 9)
            };

            searchBtn = new Button
            {
                Text = "Search",
                Location = new Point(380, y - 2),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            searchBtn.FlatAppearance.BorderSize = 0;
            searchBtn.Click += (s, e) => MessageBox.Show("Searching audit log...");

            // User filter
            y += 40;
            Label userLabel = new Label
            {
                Text = "User:",
                Location = new Point(15, y),
                Size = new Size(40, 25),
                Font = new Font("Segoe UI", 9)
            };

            userCombo = new ComboBox
            {
                Location = new Point(60, y),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            userCombo.Items.AddRange(new object[] { "All Users", "admin_user", "loan_officer1", "loan_officer2", "cashier1", "system" });
            userCombo.SelectedIndex = 0;

            // Action filter
            Label actionLabel = new Label
            {
                Text = "Action Type:",
                Location = new Point(220, y),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 9)
            };

            actionCombo = new ComboBox
            {
                Location = new Point(295, y),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            actionCombo.Items.AddRange(new object[] { "All Actions", "User Created", "Loan Approved", "Payment Processed",
                "System Config Changed", "Loan Rejected", "Override Action" });
            actionCombo.SelectedIndex = 0;

            filterPanel.Controls.AddRange(new Control[] { dateLabel, dateFromBox, toLabel, dateToBox, searchBtn,
                userLabel, userCombo, actionLabel, actionCombo });

            mainPanel.Controls.Add(filterPanel);
            currentY += 130;

            // Table with fixed large height
            auditTable = new DataGridView
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 400),  // Fixed 600x400 size
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            };

            // Style the table
            auditTable.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            auditTable.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            auditTable.RowTemplate.Height = 35;  // Taller rows

            // Add columns
            auditTable.Columns.Add("ID", "ID");
            auditTable.Columns.Add("Timestamp", "Timestamp");
            auditTable.Columns.Add("User", "User");
            auditTable.Columns.Add("Action", "Action");

            // Style columns
            auditTable.Columns["ID"].Width = 60;
            auditTable.Columns["Timestamp"].Width = 200;
            auditTable.Columns["User"].Width = 150;
            auditTable.Columns["Action"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Add sample data
            AddSampleData();

            auditTable.CellClick += (s, e) => ShowEntryDetails(e.RowIndex);

            mainPanel.Controls.Add(auditTable);
            currentY += 410;

            // Details Panel
            detailsPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 180), // Fixed width
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label detailsTitle = new Label
            {
                Text = "SELECTED AUDIT ENTRY DETAILS:",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Height = 25
            };
            detailsPanel.Controls.Add(detailsTitle);
            detailsPanel.Visible = false;

            mainPanel.Controls.Add(detailsPanel);
            currentY += 190;

            // Action Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(600, 50)
            };

            exportBtn = new Button
            {
                Text = "Export to File",
                Location = new Point(0, 10),
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            exportBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            exportBtn.FlatAppearance.BorderSize = 1;
            exportBtn.Click += (s, e) => MessageBox.Show("Exporting audit log to file...");

            viewBtn = new Button
            {
                Text = "View Full Details",
                Location = new Point(130, 10),
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            viewBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            viewBtn.FlatAppearance.BorderSize = 1;
            viewBtn.Click += (s, e) => MessageBox.Show("Viewing full details...");

            printBtn = new Button
            {
                Text = "Print Log",
                Location = new Point(260, 10),
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            printBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            printBtn.FlatAppearance.BorderSize = 1;
            printBtn.Click += (s, e) => MessageBox.Show("Printing audit log...");

            buttonPanel.Controls.AddRange(new Control[] { exportBtn, viewBtn, printBtn });
            mainPanel.Controls.Add(buttonPanel);

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private void AddSampleData()
        {
            // Clear existing rows
            auditTable.Rows.Clear();

            // Add mock data - more rows to fill the larger table
            auditTable.Rows.Add(1, "2024-03-15 14:30:25", "admin_user", "User Created");
            auditTable.Rows.Add(2, "2024-03-15 14:25:18", "loan_officer1", "Loan Approved");
            auditTable.Rows.Add(3, "2024-03-15 14:20:42", "cashier1", "Payment Processed");
            auditTable.Rows.Add(4, "2024-03-15 14:15:33", "admin_user", "System Config Changed");
            auditTable.Rows.Add(5, "2024-03-15 14:10:27", "loan_officer1", "Loan Rejected");
            auditTable.Rows.Add(6, "2024-03-15 14:05:19", "admin_user", "Override Action");
            auditTable.Rows.Add(7, "2024-03-15 14:00:58", "cashier1", "Loan Released");
            auditTable.Rows.Add(8, "2024-03-15 13:55:44", "loan_officer2", "Credit Score Updated");
            auditTable.Rows.Add(9, "2024-03-15 13:50:32", "admin_user", "Financial Transaction");
            auditTable.Rows.Add(10, "2024-03-15 13:45:21", "system", "Penalty Calculated");
            auditTable.Rows.Add(11, "2024-03-15 13:40:15", "admin_user", "User Role Changed");
            auditTable.Rows.Add(12, "2024-03-15 13:35:09", "loan_officer1", "Loan Document Uploaded");
            auditTable.Rows.Add(13, "2024-03-15 13:30:05", "cashier1", "Partial Payment Processed");
            auditTable.Rows.Add(14, "2024-03-15 13:25:01", "admin_user", "Interest Rate Updated");
            auditTable.Rows.Add(15, "2024-03-15 13:20:58", "loan_officer2", "Customer Profile Updated");
            auditTable.Rows.Add(16, "2024-03-15 13:15:47", "cashier1", "Check Issued");
            auditTable.Rows.Add(17, "2024-03-15 13:10:39", "admin_user", "Report Generated");
            auditTable.Rows.Add(18, "2024-03-15 13:05:28", "loan_officer1", "Customer Interview");
            auditTable.Rows.Add(19, "2024-03-15 13:00:22", "system", "Auto Backup");
            auditTable.Rows.Add(20, "2024-03-15 12:55:17", "admin_user", "Database Cleanup");

            // Style alternating rows
            foreach (DataGridViewRow row in auditTable.Rows)
            {
                if (row.Index % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                }
            }
        }

        private void ShowEntryDetails(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < auditTable.Rows.Count)
            {
                detailsPanel.Visible = true;
                detailsPanel.Controls.Clear();

                // Title
                Label title = new Label
                {
                    Text = "SELECTED AUDIT ENTRY DETAILS:",
                    Dock = DockStyle.Top,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Height = 25
                };
                detailsPanel.Controls.Add(title);

                // Details content
                var row = auditTable.Rows[rowIndex];
                string[] details = {
                    $"Entry ID: {row.Cells["ID"].Value}",
                    $"User: {row.Cells["User"].Value}",
                    $"Action: {row.Cells["Action"].Value}",
                    $"Timestamp: {row.Cells["Timestamp"].Value}",
                    "Details: Full details of the selected audit entry",
                    "IP Address: 192.168.1.100",
                    "Location: Main Server",
                    "Session ID: SESS-789456123"
                };

                int y = 30;
                foreach (string detail in details)
                {
                    Label lbl = new Label
                    {
                        Text = detail,
                        Location = new Point(15, y),
                        Size = new Size(detailsPanel.Width - 30, 20),
                        Font = new Font("Segoe UI", 9)
                    };
                    detailsPanel.Controls.Add(lbl);
                    y += 22;
                }

                // Move buttons down
                var buttonPanel = mainPanel.Controls[mainPanel.Controls.Count - 1];
                buttonPanel.Top = detailsPanel.Bottom + 10;
            }
        }

        // Handle resizing
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Adjust widths when control is resized
            if (mainPanel != null)
            {
                int newWidth = this.ClientSize.Width - 40;
                if (newWidth > 0)
                {
                    // Update widths of all panels
                    foreach (Control control in mainPanel.Controls)
                    {
                        control.Width = newWidth;
                    }

                    // Update table columns
                    if (auditTable != null)
                    {
                        auditTable.Columns["Action"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }
            }
        }
    }
}