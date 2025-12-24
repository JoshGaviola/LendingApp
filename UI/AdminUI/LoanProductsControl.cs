using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI
{
    public partial class LoanProductsControl : UserControl
    {
        private DataGridView dgvLoanProducts;
        private TextBox txtSearch;
        private Button btnAddNew;
        private Button btnConfigureRules;

        public LoanProductsControl()
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            // Control settings
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Segoe UI", 9);

            // Main container
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            int yPos = 10;

            // ===== HEADER =====
            var lblTitle = new Label
            {
                Text = "LOAN PRODUCTS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 70, 120)
            };
            mainPanel.Controls.Add(lblTitle);

            yPos += 40;

            // ===== LOAN TYPES LIST HEADER =====
            var lblSubtitle = new Label
            {
                Text = "Loan Types List",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            mainPanel.Controls.Add(lblSubtitle);

            yPos += 35;

            // ===== ACTION BUTTONS =====
            btnAddNew = new Button
            {
                Text = "＋ Add New Product",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, yPos),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += (s, e) =>
            {
                MessageBox.Show("Add New Product clicked", "New Product",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            mainPanel.Controls.Add(btnAddNew);

            btnConfigureRules = new Button
            {
                Text = "⚙ Configure Rules",
                Font = new Font("Segoe UI", 9),
                Location = new Point(170, yPos),
                Size = new Size(150, 35),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnConfigureRules.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnConfigureRules.FlatAppearance.BorderSize = 1;
            btnConfigureRules.Click += (s, e) =>
            {
                MessageBox.Show("Configure Rules clicked", "Rules Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            mainPanel.Controls.Add(btnConfigureRules);

            yPos += 50;

            // ===== SEPARATOR LINE =====
            var separator = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(this.Width - 40, 1),
                BackColor = Color.LightGray,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            mainPanel.Controls.Add(separator);

            yPos += 20;

            // ===== LOAN TYPES LIST HEADER =====
            var lblListHeader = new Label
            {
                Text = "LOAN TYPES LIST",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            mainPanel.Controls.Add(lblListHeader);

            yPos += 35;

            // ===== SEARCH BAR =====
            var searchPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(300, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            var searchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 12),
                Location = new Point(5, 5),
                Size = new Size(30, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            searchPanel.Controls.Add(searchIcon);

            txtSearch = new TextBox
            {
                Location = new Point(40, 5),
                Size = new Size(250, 25),
                Text = "Search loan products...",
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None,
                ForeColor = Color.Gray
            };
            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Search loan products...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search loan products...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && txtSearch.Text != "Search loan products...")
                {
                    SearchLoanProducts(txtSearch.Text);
                }
            };
            searchPanel.Controls.Add(txtSearch);
            mainPanel.Controls.Add(searchPanel);

            yPos += 50;

            // ===== DATA GRID VIEW =====
            dgvLoanProducts = new DataGridView
            {
                Location = new Point(10, yPos),
                Size = new Size(this.Width - 40, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            // Configure columns
            dgvLoanProducts.Columns.Add("ID", "ID");
            dgvLoanProducts.Columns.Add("LoanTypeName", "Loan Type Name");
            dgvLoanProducts.Columns.Add("Interest", "Interest");
            dgvLoanProducts.Columns.Add("MaxAmount", "Max Amount");
            dgvLoanProducts.Columns.Add("Status", "Status");
            dgvLoanProducts.Columns.Add("Actions", "Actions");

            // Style columns
            dgvLoanProducts.Columns["ID"].Width = 50;
            dgvLoanProducts.Columns["LoanTypeName"].Width = 150;
            dgvLoanProducts.Columns["Interest"].Width = 80;
            dgvLoanProducts.Columns["MaxAmount"].Width = 120;
            dgvLoanProducts.Columns["Status"].Width = 100;
            dgvLoanProducts.Columns["Actions"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Center align some columns
            dgvLoanProducts.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.Columns["Interest"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.Columns["MaxAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLoanProducts.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Add sample data
            AddSampleData();

            // Style the grid
            dgvLoanProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.ColumnHeadersHeight = 40;
            dgvLoanProducts.RowTemplate.Height = 35;

            mainPanel.Controls.Add(dgvLoanProducts);

            // Handle resize
            this.Resize += (s, e) =>
            {
                separator.Width = mainPanel.Width - 40;
                dgvLoanProducts.Width = mainPanel.Width - 40;
                dgvLoanProducts.Height = mainPanel.Height - yPos - 20;
            };

            this.Controls.Add(mainPanel);
        }

        private void AddSampleData()
        {
            // Add sample rows
            dgvLoanProducts.Rows.Add("1", "Personal Loan", "12%", "¥100,000",
                CreateStatusLabel("Active"), CreateActionButtons("1", true));

            dgvLoanProducts.Rows.Add("2", "Emergency Loan", "10%", "¥50,000",
                CreateStatusLabel("Active"), CreateActionButtons("2", true));

            dgvLoanProducts.Rows.Add("3", "Salary Loan", "8%", "¥200,000",
                CreateStatusLabel("Active"), CreateActionButtons("3", true));

            dgvLoanProducts.Rows.Add("4", "Business Loan", "15%", "¥500,000",
                CreateStatusLabel("Active"), CreateActionButtons("4", true));

            dgvLoanProducts.Rows.Add("5", "Educational Loan", "9%", "¥150,000",
                CreateStatusLabel("Active"), CreateActionButtons("5", true));

            dgvLoanProducts.Rows.Add("6", "Home Improvement", "11%", "¥300,000",
                CreateStatusLabel("Inactive"), CreateActionButtons("6", false));
        }

        private Panel CreateStatusLabel(string status)
        {
            var panel = new Panel
            {
                Size = new Size(80, 25),
                Margin = new Padding(10, 5, 10, 5)
            };

            var label = new Label
            {
                Text = status,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = status == "Active" ? Color.Green : Color.Red,
                BackColor = status == "Active" ? Color.FromArgb(230, 255, 230) : Color.FromArgb(255, 230, 230),
                BorderStyle = BorderStyle.FixedSingle
            };

            panel.Controls.Add(label);
            return panel;
        }

        private Panel CreateActionButtons(string id, bool isActive)
        {
            var panel = new Panel
            {
                Size = new Size(200, 35),
                Margin = new Padding(5)
            };

            // Edit button
            var btnEdit = new Button
            {
                Text = "✏ Edit",
                Tag = id,
                Size = new Size(60, 25),
                Location = new Point(0, 5),
                Font = new Font("Segoe UI", 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.Blue
            };
            btnEdit.FlatAppearance.BorderColor = Color.Blue;
            btnEdit.FlatAppearance.BorderSize = 1;
            btnEdit.Click += (s, e) =>
            {
                MessageBox.Show($"Edit loan product ID: {id}", "Edit Product",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            panel.Controls.Add(btnEdit);

            // View button
            var btnView = new Button
            {
                Text = "👁 View",
                Tag = id,
                Size = new Size(60, 25),
                Location = new Point(70, 5),
                Font = new Font("Segoe UI", 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.Green
            };
            btnView.FlatAppearance.BorderColor = Color.Green;
            btnView.FlatAppearance.BorderSize = 1;
            btnView.Click += (s, e) =>
            {
                MessageBox.Show($"View loan product ID: {id}", "View Product",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            panel.Controls.Add(btnView);

            // Deactivate/Activate button
            var btnStatus = new Button
            {
                Text = isActive ? "✗ Deactivate" : "✓ Activate",
                Tag = id,
                Size = new Size(80, 25),
                Location = new Point(140, 5),
                Font = new Font("Segoe UI", 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = isActive ? Color.White : Color.FromArgb(230, 255, 230),
                ForeColor = isActive ? Color.Red : Color.Green
            };
            btnStatus.FlatAppearance.BorderColor = isActive ? Color.Red : Color.Green;
            btnStatus.FlatAppearance.BorderSize = 1;
            btnStatus.Click += (s, e) =>
            {
                string action = isActive ? "deactivate" : "activate";
                var result = MessageBox.Show($"Are you sure you want to {action} loan product ID: {id}?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show($"Loan product {id} has been {action}d.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            panel.Controls.Add(btnStatus);

            return panel;
        }

        private void SearchLoanProducts(string searchTerm)
        {
            foreach (DataGridViewRow row in dgvLoanProducts.Rows)
            {
                bool found = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null &&
                        cell.Value.ToString().IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        found = true;
                        break;
                    }
                }
                row.Visible = found;
            }
        }
    }
}