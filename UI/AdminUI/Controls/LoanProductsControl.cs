using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using LendingApp.Class;

namespace LendingApp.UI.AdminUI
{
    public partial class LoanProductsControl : UserControl
    {
        private DataGridView dgvLoanProducts;
        private TextBox txtSearch;
        private Button btnAddNew;
        private Button btnLoanTypes;
        private Button btnLoanProducts;
        private Button btnEditSelected;
        private Button btnViewSelected;
        private Button btnDeactivateSelected;

        // Track selected row ID and status
        private string selectedProductId = null;
        private bool isSelectedProductActive = false;

        // Track current view
        private enum ViewMode { LoanTypesList, AddNewProduct }
        private ViewMode currentView = ViewMode.LoanTypesList;

        // Panels for different views
        private Panel mainPanel;
        private Panel loanTypesListPanel;
        private AddNewLoanProductControl addNewProductControl;

        // Track if tab buttons have been initialized
        private bool tabButtonsInitialized = false;

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
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Initialize all views
            InitializeLoanTypesListView();
            InitializeAddNewProductView();

            // Show default view
            ShowView(currentView);

            this.Controls.Add(mainPanel);
        }

        private void InitializeLoanTypesListView()
        {
            loanTypesListPanel = new Panel
            {
                Dock = DockStyle.Fill
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
            loanTypesListPanel.Controls.Add(lblTitle);

            yPos += 40;

            // ===== SUB-NAVIGATION TABS =====
            var subNavPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(400, 40)
            };

            // Loan Products button (inactive/outline style)
            btnLoanProducts = new Button
            {
                Text = "Loan Products",
                Size = new Size(120, 35),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            btnLoanProducts.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnLoanProducts.FlatAppearance.BorderSize = 1;
            btnLoanProducts.Click += (s, e) =>
            {
                MessageBox.Show("Showing Loan Products dashboard", "Loan Products",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            subNavPanel.Controls.Add(btnLoanProducts);

            // Loan Types List button (active/filled style)
            btnLoanTypes = new Button
            {
                Text = "Loan Types List",
                Size = new Size(120, 35),
                Location = new Point(130, 0),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnLoanTypes.FlatAppearance.BorderSize = 0;
            btnLoanTypes.Click += (s, e) =>
            {
                ShowView(ViewMode.LoanTypesList);
            };
            subNavPanel.Controls.Add(btnLoanTypes);

            loanTypesListPanel.Controls.Add(subNavPanel);
            yPos += 50;

            // ===== ACTION BUTTONS (Top Right) =====
            // Container for right-aligned buttons
            var topActionPanel = new Panel
            {
                Location = new Point(loanTypesListPanel.Width - 170, yPos - 50), // Align with sub-nav (adjusted width)
                Size = new Size(160, 35)
            };

            // Add New Product button
            btnAddNew = new Button
            {
                Text = "＋ Add New Product",
                Size = new Size(150, 35),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddNew.FlatAppearance.BorderSize = 0;
            btnAddNew.Click += (s, e) =>
            {
                ShowView(ViewMode.AddNewProduct);
            };
            topActionPanel.Controls.Add(btnAddNew);

            loanTypesListPanel.Controls.Add(topActionPanel);

            // Mark tab buttons as initialized
            tabButtonsInitialized = true;

            // ===== SEPARATOR LINE =====
            var separator = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(loanTypesListPanel.Width - 40, 1),
                BackColor = Color.LightGray,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            loanTypesListPanel.Controls.Add(separator);

            yPos += 20;

            // ===== LOAN TYPES LIST HEADER =====
            var lblListHeader = new Label
            {
                Text = "LOAN TYPES LIST",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            loanTypesListPanel.Controls.Add(lblListHeader);

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
                if (string.IsNullOrWhiteSpace(selectedProductId))
                    return;

                int pid;
                if (!int.TryParse(selectedProductId, NumberStyles.Integer, CultureInfo.InvariantCulture, out pid))
                {
                    MessageBox.Show("Invalid product id.", "Edit Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ensure control exists
                if (addNewProductControl == null)
                    InitializeAddNewProductView();

                try
                {
                    // Switch to AddNewProduct view and load selected product into edit mode
                    ShowView(ViewMode.AddNewProduct);

                    addNewProductControl.LoadProductForEdit(pid);

                    // Refresh list when save completes, then go back to list
                    addNewProductControl.ProductSaved -= OnProductSavedFromEditor;
                    addNewProductControl.ProductSaved += OnProductSavedFromEditor;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to load product for edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            loanTypesListPanel.Controls.Add(actionButtonsPanel);
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
            loanTypesListPanel.Controls.Add(searchPanel);

            yPos += 50;

            // ===== DATA GRID VIEW =====
            dgvLoanProducts = new DataGridView
            {
                Location = new Point(10, yPos),
                Size = new Size(loanTypesListPanel.Width - 40, 300),
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

            loanTypesListPanel.Controls.Add(dgvLoanProducts);

            // Handle resize to keep buttons aligned
            loanTypesListPanel.Resize += (s, e) =>
            {
                separator.Width = loanTypesListPanel.Width - 40;
                dgvLoanProducts.Width = loanTypesListPanel.Width - 40;
                dgvLoanProducts.Height = loanTypesListPanel.Height - yPos - 20;

                // Keep top action buttons aligned to the right
                topActionPanel.Left = loanTypesListPanel.Width - topActionPanel.Width - 20;
            };

            // Initial positioning of top action buttons
            topActionPanel.Left = loanTypesListPanel.Width - topActionPanel.Width - 20;
        }

        private void InitializeAddNewProductView()
        {
            addNewProductControl = new AddNewLoanProductControl();
            addNewProductControl.Dock = DockStyle.Fill;
            addNewProductControl.Visible = false;
        }

        private void ShowView(ViewMode viewMode)
        {
            currentView = viewMode;

            // Clear current view
            mainPanel.Controls.Clear();

            // Update tab buttons only if they've been initialized
            if (tabButtonsInitialized)
            {
                UpdateTabButtons(viewMode);
            }

            // Show the selected view
            if (viewMode == ViewMode.LoanTypesList)
            {
                mainPanel.Controls.Add(loanTypesListPanel);
                if (addNewProductControl != null)
                    addNewProductControl.Visible = false;
            }
            else if (viewMode == ViewMode.AddNewProduct)
            {
                if (addNewProductControl == null)
                    InitializeAddNewProductView();

                mainPanel.Controls.Add(addNewProductControl);
                addNewProductControl.Visible = true;
            }
        }

        private void UpdateTabButtons(ViewMode currentView)
        {
            // Only update if buttons exist
            if (btnLoanProducts == null || btnLoanTypes == null)
                return;

            // Reset all tab buttons to inactive style
            btnLoanProducts.BackColor = Color.White;
            btnLoanProducts.ForeColor = Color.FromArgb(0, 120, 215);
            btnLoanProducts.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnLoanProducts.FlatAppearance.BorderSize = 1;
            btnLoanProducts.Font = new Font("Segoe UI", 9, FontStyle.Regular);

            btnLoanTypes.BackColor = Color.White;
            btnLoanTypes.ForeColor = Color.FromArgb(0, 120, 215);
            btnLoanTypes.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnLoanTypes.FlatAppearance.BorderSize = 1;
            btnLoanTypes.Font = new Font("Segoe UI", 9, FontStyle.Regular);

            // Set active style for current view tab
            if (currentView == ViewMode.LoanTypesList)
            {
                btnLoanTypes.BackColor = Color.FromArgb(0, 120, 215);
                btnLoanTypes.ForeColor = Color.White;
                btnLoanTypes.FlatAppearance.BorderSize = 0;
                btnLoanTypes.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            // Note: AddNewProduct does not have tab button
        }

        private void AddSampleData()
        {
            LoadLoanProductsFromDb();
        }

        private void LoadLoanProductsFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var products = db.LoanProducts.AsNoTracking()
                        .OrderBy(p => p.ProductId)
                        .ToList();

                    dgvLoanProducts.Rows.Clear();

                    foreach (var p in products)
                    {
                        dgvLoanProducts.Rows.Add(
                            p.ProductId.ToString(CultureInfo.InvariantCulture),
                            p.ProductName ?? "",
                            p.InterestRate.ToString("0.##", CultureInfo.InvariantCulture) + "%",
                            "₱" + p.MaxAmount.ToString("N0", CultureInfo.InvariantCulture),
                            p.IsActive ? "Active" : "Inactive"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to load loan products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvLoanProducts.Rows.Clear();
            }
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
            try
            {
                var s = (searchTerm ?? "").Trim();
                if (string.IsNullOrWhiteSpace(s) || s == "Search loan products...")
                {
                    LoadLoanProductsFromDb();
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var products = db.LoanProducts.AsNoTracking()
                        .Where(p =>
                            (p.ProductName ?? "").Contains(s) ||
                            (p.Description ?? "").Contains(s))
                        .OrderBy(p => p.ProductId)
                        .ToList();

                    dgvLoanProducts.Rows.Clear();

                    foreach (var p in products)
                    {
                        dgvLoanProducts.Rows.Add(
                            p.ProductId.ToString(CultureInfo.InvariantCulture),
                            p.ProductName ?? "",
                            p.InterestRate.ToString("0.##", CultureInfo.InvariantCulture) + "%",
                            "₱" + p.MaxAmount.ToString("N0", CultureInfo.InvariantCulture),
                            p.IsActive ? "Active" : "Inactive"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Search failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnProductSavedFromEditor(int productId)
        {
            // Go back to list and refresh grid
            ShowView(ViewMode.LoanTypesList);
            LoadLoanProductsFromDb();

            // Optionally reselect edited product
            SelectProductRow(productId);
        }

        private void SelectProductRow(int productId)
        {
            if (dgvLoanProducts == null) return;

            var idText = productId.ToString(CultureInfo.InvariantCulture);

            foreach (DataGridViewRow row in dgvLoanProducts.Rows)
            {
                if (row.Cells["ID"].Value != null && string.Equals(row.Cells["ID"].Value.ToString(), idText, StringComparison.OrdinalIgnoreCase))
                {
                    dgvLoanProducts.ClearSelection();
                    row.Selected = true;
                    dgvLoanProducts.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }
    }
}