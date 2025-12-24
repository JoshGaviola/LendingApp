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
        private Button btnEditSelected;
        private Button btnViewSelected;
        private Button btnDeactivateSelected;

        // Track selected row ID and status
        private string selectedProductId = null;
        private bool isSelectedProductActive = false;

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

            // ===== ACTION BUTTONS (Top Row) =====
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

            // ===== ROW ACTION BUTTONS (Edit, View, Deactivate) =====
            var actionButtonsPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(400, 35),
                Visible = false // Initially hidden until a row is selected
            };

            btnEditSelected = new Button
            {
                Text = "✏ Edit Selected",
                Size = new Size(100, 30),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.Blue,
                Enabled = false
            };
            btnEditSelected.FlatAppearance.BorderColor = Color.Blue;
            btnEditSelected.FlatAppearance.BorderSize = 1;
            btnEditSelected.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(selectedProductId))
                {
                    MessageBox.Show($"Edit loan product ID: {selectedProductId}", "Edit Product",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            actionButtonsPanel.Controls.Add(btnEditSelected);

            btnViewSelected = new Button
            {
                Text = "👁 View Selected",
                Size = new Size(100, 30),
                Location = new Point(110, 0),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.Green,
                Enabled = false
            };
            btnViewSelected.FlatAppearance.BorderColor = Color.Green;
            btnViewSelected.FlatAppearance.BorderSize = 1;
            btnViewSelected.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(selectedProductId))
                {
                    MessageBox.Show($"View loan product ID: {selectedProductId}", "View Product",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            actionButtonsPanel.Controls.Add(btnViewSelected);

            btnDeactivateSelected = new Button
            {
                Text = "✗ Deactivate Selected",
                Size = new Size(120, 30),
                Location = new Point(220, 0),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.Red,
                Enabled = false
            };
            btnDeactivateSelected.FlatAppearance.BorderColor = Color.Red;
            btnDeactivateSelected.FlatAppearance.BorderSize = 1;
            btnDeactivateSelected.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(selectedProductId))
                {
                    string action = isSelectedProductActive ? "deactivate" : "activate";
                    var result = MessageBox.Show($"Are you sure you want to {action} loan product ID: {selectedProductId}?",
                        $"Confirm {action}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show($"Loan product {selectedProductId} has been {action}d.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Update the status in the grid
                        UpdateProductStatus(selectedProductId, !isSelectedProductActive);
                    }
                }
            };
            actionButtonsPanel.Controls.Add(btnDeactivateSelected);

            mainPanel.Controls.Add(actionButtonsPanel);
            yPos += 45;

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

            // Style columns
            dgvLoanProducts.Columns["ID"].Width = 50;
            dgvLoanProducts.Columns["LoanTypeName"].Width = 200;
            dgvLoanProducts.Columns["Interest"].Width = 100;
            dgvLoanProducts.Columns["MaxAmount"].Width = 150;
            dgvLoanProducts.Columns["Status"].Width = 120;

            // Center align some columns
            dgvLoanProducts.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.Columns["Interest"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.Columns["MaxAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLoanProducts.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style Status column cells
            dgvLoanProducts.Columns["Status"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Add sample data
            AddSampleData();

            // Style the grid
            dgvLoanProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvLoanProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLoanProducts.ColumnHeadersHeight = 40;
            dgvLoanProducts.RowTemplate.Height = 35;

            // Handle row selection
            dgvLoanProducts.SelectionChanged += (s, e) =>
            {
                if (dgvLoanProducts.SelectedRows.Count > 0)
                {
                    var selectedRow = dgvLoanProducts.SelectedRows[0];
                    selectedProductId = selectedRow.Cells["ID"].Value?.ToString();

                    // Get status from the selected row
                    var statusValue = selectedRow.Cells["Status"].Value?.ToString();
                    isSelectedProductActive = statusValue == "Active";

                    // Update action buttons
                    actionButtonsPanel.Visible = true;
                    btnEditSelected.Enabled = true;
                    btnViewSelected.Enabled = true;
                    btnDeactivateSelected.Enabled = true;

                    // Update deactivate button text based on status
                    if (isSelectedProductActive)
                    {
                        btnDeactivateSelected.Text = "✗ Deactivate Selected";
                        btnDeactivateSelected.ForeColor = Color.Red;
                        btnDeactivateSelected.FlatAppearance.BorderColor = Color.Red;
                    }
                    else
                    {
                        btnDeactivateSelected.Text = "✓ Activate Selected";
                        btnDeactivateSelected.ForeColor = Color.Green;
                        btnDeactivateSelected.FlatAppearance.BorderColor = Color.Green;
                    }
                }
                else
                {
                    // No row selected, hide and disable action buttons
                    actionButtonsPanel.Visible = false;
                    btnEditSelected.Enabled = false;
                    btnViewSelected.Enabled = false;
                    btnDeactivateSelected.Enabled = false;
                    selectedProductId = null;
                    isSelectedProductActive = false;
                }
            };

            // Handle cell formatting for Status column
            dgvLoanProducts.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgvLoanProducts.Columns["Status"].Index && e.Value != null)
                {
                    var status = e.Value.ToString();
                    if (status == "Active")
                    {
                        e.CellStyle.ForeColor = Color.Green;
                        e.CellStyle.BackColor = Color.FromArgb(230, 255, 230);
                    }
                    else if (status == "Inactive")
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.BackColor = Color.FromArgb(255, 230, 230);
                    }
                }
            };

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
            // Add sample rows with simple status text
            dgvLoanProducts.Rows.Add("1", "Personal Loan", "12%", "¥100,000", "Active");
            dgvLoanProducts.Rows.Add("2", "Emergency Loan", "10%", "¥50,000", "Active");
            dgvLoanProducts.Rows.Add("3", "Salary Loan", "8%", "¥200,000", "Active");
            dgvLoanProducts.Rows.Add("4", "Business Loan", "15%", "¥500,000", "Active");
            dgvLoanProducts.Rows.Add("5", "Educational Loan", "9%", "¥150,000", "Active");
            dgvLoanProducts.Rows.Add("6", "Home Improvement", "11%", "¥300,000", "Inactive");
        }

        private void UpdateProductStatus(string productId, bool newActiveStatus)
        {
            foreach (DataGridViewRow row in dgvLoanProducts.Rows)
            {
                if (row.Cells["ID"].Value?.ToString() == productId)
                {
                    string newStatus = newActiveStatus ? "Active" : "Inactive";
                    row.Cells["Status"].Value = newStatus;

                    // If this is the currently selected row, update the button
                    if (row.Selected)
                    {
                        isSelectedProductActive = newActiveStatus;
                        if (newActiveStatus)
                        {
                            btnDeactivateSelected.Text = "✗ Deactivate Selected";
                            btnDeactivateSelected.ForeColor = Color.Red;
                            btnDeactivateSelected.FlatAppearance.BorderColor = Color.Red;
                        }
                        else
                        {
                            btnDeactivateSelected.Text = "✓ Activate Selected";
                            btnDeactivateSelected.ForeColor = Color.Green;
                            btnDeactivateSelected.FlatAppearance.BorderColor = Color.Green;
                        }
                    }
                    break;
                }
            }
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