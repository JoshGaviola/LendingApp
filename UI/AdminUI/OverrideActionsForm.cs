using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class OverrideActionsControl : UserControl
    {
        private TabControl tabControl;
        private TabPage loanOverridesTab;

        // Controls for Loan Overrides tab
        private DataGridView loansDataGrid;
        private TextBox searchTextBox;
        private Button searchButton;
        private Panel selectedLoanPanel;
        private Panel noLoanPanel;
        private GroupBox loanDetailsGroup;
        private GroupBox overrideActionsGroup;
        private GroupBox overrideReasonGroup;
        private GroupBox approvalGroup;

        // Form controls that need to be accessed in event handlers
        private TextBox reasonTextBox;
        private Label charCountLabel;
        private TextBox passwordTextBox;
        private TextBox interestTextBox;
        private RadioButton approveRadio;
        private RadioButton modifyRadio;
        private RadioButton interestRadio;
        private RadioButton waiveRadio;

        private Button executeButton;
        private Button cancelButton;

        // Main container reference
        private Panel mainContainer;

        public OverrideActionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Main container
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Main tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height),
                Padding = new Point(12, 8)
            };

            // Create all 4 tabs
            loanOverridesTab = new TabPage("Loan Overrides");
            TabPage userActionsTab = new TabPage("User Actions");
            TabPage systemOverridesTab = new TabPage("System Overrides");
            TabPage auditLogTab = new TabPage("Audit Log");

            // Initialize tabs with actual controls (not placeholders)
            InitializeLoanOverridesTab();
            InitializeUserActionsTab(userActionsTab);  // This will now add UserActionsControl
            InitializeSystemOverridesTab(systemOverridesTab);
            InitializeAuditLogTab(auditLogTab);

            tabControl.TabPages.Add(loanOverridesTab);
            tabControl.TabPages.Add(userActionsTab);
            tabControl.TabPages.Add(systemOverridesTab);
            tabControl.TabPages.Add(auditLogTab);

            this.Controls.Add(tabControl);

            this.ResumeLayout(false);
        }

        private void InitializeLoanOverridesTab()
        {
            loanOverridesTab.SuspendLayout();
            loanOverridesTab.BackColor = Color.White;
            loanOverridesTab.Padding = new Padding(10);

            // Main container for this tab
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            int yPos = 0;

            // Header
            Label headerLabel = new Label
            {
                Text = "LOAN OVERRIDES",
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            yPos += 50;

            // Search panel
            Panel searchPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 50),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(10, 15),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70)
            };

            searchTextBox = new TextBox
            {
                Location = new Point(80, 12),
                Size = new Size(300, 26),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            AddPlaceholderText(searchTextBox, "Loan ID or Customer Name");

            searchButton = new Button
            {
                Text = "🔍 Search",
                Location = new Point(390, 12),
                Size = new Size(100, 26),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += SearchButton_Click;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(searchButton);
            yPos += 60;

            // Loans Data Grid
            loansDataGrid = new DataGridView
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 200),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(229, 231, 235)
            };

            // Add columns with better styling
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                Width = 80
            };

            DataGridViewTextBoxColumn customerColumn = new DataGridViewTextBoxColumn
            {
                Name = "Customer",
                HeaderText = "Customer",
                Width = 200
            };

            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 150
            };

            DataGridViewTextBoxColumn statusColumn = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 150
            };

            DataGridViewButtonColumn actionColumn = new DataGridViewButtonColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                Text = "Select",
                UseColumnTextForButtonValue = true,
                Width = 100
            };

            loansDataGrid.Columns.Add(idColumn);
            loansDataGrid.Columns.Add(customerColumn);
            loansDataGrid.Columns.Add(amountColumn);
            loansDataGrid.Columns.Add(statusColumn);
            loansDataGrid.Columns.Add(actionColumn);

            // Style the grid
            loansDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 244, 246);
            loansDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            loansDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            loansDataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            loansDataGrid.EnableHeadersVisualStyles = false;

            // Sample data
            AddSampleLoanData();

            loansDataGrid.CellClick += LoansDataGrid_CellClick;
            yPos += 210;

            // "No loan selected" message (initially visible)
            noLoanPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 120),
                BackColor = Color.FromArgb(254, 252, 232),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = true
            };

            Label noLoanLabel = new Label
            {
                Text = "⚠ Select a loan from the table above to view details and perform override actions",
                Location = new Point(20, 40),
                Size = new Size(mainContainer.Width - 60, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(146, 64, 14)
            };

            noLoanPanel.Controls.Add(noLoanLabel);

            // Selected Loan Panel (initially hidden) - INCREASED HEIGHT HERE
            selectedLoanPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 550), // Increased from 450 to 550
                BackColor = Color.FromArgb(255, 250, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                AutoScroll = true // Enable scrolling
            };

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                headerLabel.Width = mainContainer.Width;
                searchPanel.Width = mainContainer.Width - 20;
                loansDataGrid.Width = mainContainer.Width - 20;
                noLoanPanel.Width = mainContainer.Width - 20;
                selectedLoanPanel.Width = mainContainer.Width - 20;

                // Reinitialize selected loan panel with new width
                InitializeSelectedLoanPanel();
            };

            // Add controls to main container
            mainContainer.Controls.Add(headerLabel);
            mainContainer.Controls.Add(searchPanel);
            mainContainer.Controls.Add(loansDataGrid);
            mainContainer.Controls.Add(noLoanPanel);
            mainContainer.Controls.Add(selectedLoanPanel);

            // Add main container to tab
            loanOverridesTab.Controls.Add(mainContainer);

            loanOverridesTab.ResumeLayout(true);
        }
        private void InitializeSelectedLoanPanel()
        {
            if (selectedLoanPanel == null) return;

            selectedLoanPanel.SuspendLayout();

            // Clear existing controls
            selectedLoanPanel.Controls.Clear();

            int padding = 20;
            int groupWidth = Math.Max(300, selectedLoanPanel.Width - (padding * 2)); // Ensure minimum width
            int yPos = padding;

            // Header
            Label detailsHeader = new Label
            {
                Text = "SELECTED LOAN DETAILS",
                Location = new Point(padding, yPos),
                Size = new Size(groupWidth, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            selectedLoanPanel.Controls.Add(detailsHeader);
            yPos += 40;

            // Loan Details Group
            loanDetailsGroup = new GroupBox
            {
                Text = "Loan Information",
                Location = new Point(padding, yPos),
                Size = new Size(groupWidth, 80), // Reduced height from 90 to 80
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            // REMOVED "Reason: \"Insufficient income\"" from the text
            Label loanInfoLabel = new Label
            {
                Text = "Loan ID: 101 | Juan Dela Cruz\nType: Personal Loan | ₱50,000 | 12 months\nStatus: Rejected (by Officer: John Smith)",
                Location = new Point(10, 20),
                Size = new Size(groupWidth - 20, 50), // Reduced height from 60 to 50
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(55, 65, 81)
            };

            loanDetailsGroup.Controls.Add(loanInfoLabel);
            selectedLoanPanel.Controls.Add(loanDetailsGroup);
            yPos += 90; // Reduced from 100 to 90

            // ... rest of the code remains the same ...
            // Override Actions Group
            overrideActionsGroup = new GroupBox
            {
                Text = "OVERRIDE ACTION",
                Location = new Point(padding, yPos),
                Size = new Size(groupWidth, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            approveRadio = new RadioButton
            {
                Text = "Approve Loan (Override Rejection)",
                Location = new Point(10, 25),
                Size = new Size(groupWidth - 20, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };

            modifyRadio = new RadioButton
            {
                Text = "Modify Terms",
                Location = new Point(10, 50),
                Size = new Size(groupWidth - 20, 20),
                Font = new Font("Segoe UI", 9)
            };

            // Container for interest rate option
            Panel interestContainer = new Panel
            {
                Location = new Point(10, 75),
                Size = new Size(groupWidth - 20, 25)
            };

            interestRadio = new RadioButton
            {
                Text = "Change Interest Rate:",
                Location = new Point(0, 0),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9)
            };

            interestTextBox = new TextBox
            {
                Location = new Point(155, 1),
                Size = new Size(60, 22),
                Text = "0.00",
                Enabled = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label percentLabel = new Label
            {
                Text = "%",
                Location = new Point(220, 3),
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 9)
            };

            interestContainer.Controls.Add(interestRadio);
            interestContainer.Controls.Add(interestTextBox);
            interestContainer.Controls.Add(percentLabel);

            waiveRadio = new RadioButton
            {
                Text = "Waive Penalties/Fees",
                Location = new Point(10, 100),
                Size = new Size(groupWidth - 20, 20),
                Font = new Font("Segoe UI", 9)
            };

            // Enable/disable interest textbox based on radio selection
            interestRadio.CheckedChanged += (s, e) =>
            {
                interestTextBox.Enabled = interestRadio.Checked;
                if (interestRadio.Checked)
                {
                    interestTextBox.BackColor = Color.White;
                    interestTextBox.Focus();
                }
            };

            overrideActionsGroup.Controls.Add(approveRadio);
            overrideActionsGroup.Controls.Add(modifyRadio);
            overrideActionsGroup.Controls.Add(interestContainer);
            overrideActionsGroup.Controls.Add(waiveRadio);
            selectedLoanPanel.Controls.Add(overrideActionsGroup);
            yPos += 130;

            // Override Reason Group
            overrideReasonGroup = new GroupBox
            {
                Text = "OVERRIDE REASON (Required)",
                Location = new Point(padding, yPos),
                Size = new Size(groupWidth, 110),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            reasonTextBox = new TextBox
            {
                Location = new Point(10, 25),
                Size = new Size(groupWidth - 20, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            AddPlaceholderText(reasonTextBox, "Enter detailed reason for override...");

            // Character count label
            charCountLabel = new Label
            {
                Text = "0/500 characters",
                Location = new Point(groupWidth - 120, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 8),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Gray
            };

            // Update character count as user types
            reasonTextBox.TextChanged += (s, e) =>
            {
                if (reasonTextBox.Text == "Enter detailed reason for override...")
                {
                    charCountLabel.Text = "0/500 characters";
                }
                else
                {
                    charCountLabel.Text = $"{reasonTextBox.Text.Length}/500 characters";
                }
            };

            overrideReasonGroup.Controls.Add(reasonTextBox);
            overrideReasonGroup.Controls.Add(charCountLabel);
            selectedLoanPanel.Controls.Add(overrideReasonGroup);
            yPos += 120;

            // Approval Group
            approvalGroup = new GroupBox
            {
                Text = "REQUIRED APPROVAL",
                Location = new Point(padding, yPos),
                Size = new Size(groupWidth, 70),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                BackColor = Color.White
            };

            Label passwordLabel = new Label
            {
                Text = "Admin Password:",
                Location = new Point(10, 30),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(120, 28),
                Size = new Size(groupWidth - 140, 22),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            AddPlaceholderText(passwordTextBox, "Enter admin password");

            approvalGroup.Controls.Add(passwordLabel);
            approvalGroup.Controls.Add(passwordTextBox);
            selectedLoanPanel.Controls.Add(approvalGroup);
            yPos += 80;

            // Action Buttons - Position them after all content
            executeButton = new Button
            {
                Text = "Execute Override",
                Location = new Point(padding, yPos + 20), // Add extra space before buttons
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(220, 38, 38), // Red color for important action
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            executeButton.FlatAppearance.BorderSize = 0;
            executeButton.Click += ExecuteButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(padding + 140, yPos + 20), // Add extra space before buttons
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            selectedLoanPanel.Controls.Add(executeButton);
            selectedLoanPanel.Controls.Add(cancelButton);

            // Calculate total height needed and adjust panel if needed
            int totalHeightNeeded = yPos + 100; // Add space for buttons and padding
            if (totalHeightNeeded > selectedLoanPanel.Height)
            {
                // Enable vertical scrollbar since content is taller than panel
                selectedLoanPanel.AutoScroll = true;
            }

            selectedLoanPanel.ResumeLayout(true);
        }

        private void AddPlaceholderText(TextBox textBox, string placeholder)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = SystemColors.WindowText;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void InitializeUserActionsTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Create and add the UserActionsControl
            var userActionsControl = new UserActionsControl();
            userActionsControl.Dock = DockStyle.Fill;
            tab.Controls.Add(userActionsControl);
        }

        private void InitializeSystemOverridesTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            Label placeholder = new Label
            {
                Text = "System Overrides\n\nFeature coming soon...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175)
            };

            tab.Controls.Add(placeholder);
        }

        private void InitializeAuditLogTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            Label placeholder = new Label
            {
                Text = "Audit Log\n\nFeature coming soon...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 163, 175)
            };

            tab.Controls.Add(placeholder);
        }

        private void AddSampleLoanData()
        {
            // Clear existing rows
            loansDataGrid.Rows.Clear();

            // Add sample rows
            loansDataGrid.Rows.Add("101", "Juan Dela Cruz", "₱50,000", "❌ Rejected");
            loansDataGrid.Rows.Add("205", "Maria Santos", "₱120,000", "⏳ Pending");
            loansDataGrid.Rows.Add("312", "Pedro Reyes", "₱80,000", "✅ Approved");
            loansDataGrid.Rows.Add("418", "Anna Lim", "₱200,000", "✅ Approved");
            loansDataGrid.Rows.Add("529", "Robert Tan", "₱75,000", "❌ Rejected");

            // Style rows
            foreach (DataGridViewRow row in loansDataGrid.Rows)
            {
                if (row.Cells["Status"].Value.ToString().Contains("❌"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242); // Light red
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                }
                else if (row.Cells["Status"].Value.ToString().Contains("⏳"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 252, 232); // Light yellow
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(161, 98, 7);
                }
                else if (row.Cells["Status"].Value.ToString().Contains("✅"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244); // Light green
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(21, 128, 61);
                }
            }
        }

        private void LoansDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on the button column and not header
            if (e.RowIndex >= 0 && e.ColumnIndex == loansDataGrid.Columns["Actions"].Index)
            {
                selectedLoanPanel.Visible = true;
                noLoanPanel.Visible = false;

                // Update loan details based on selected row
                DataGridViewRow row = loansDataGrid.Rows[e.RowIndex];
                UpdateLoanDetails(row);
            }
        }

        private void UpdateLoanDetails(DataGridViewRow row)
        {
            // Update the loan details group with actual data from the row
            string loanId = row.Cells["ID"].Value.ToString();
            string customer = row.Cells["Customer"].Value.ToString();
            string amount = row.Cells["Amount"].Value.ToString();
            string status = row.Cells["Status"].Value.ToString();

            // In a real app, you'd fetch more details from database
            string loanType = "Personal Loan";
            string term = "12 months";
            string officer = "John Smith";

            // Find and update the label in loanDetailsGroup
            foreach (Control control in loanDetailsGroup.Controls)
            {
                if (control is Label label)
                {
                    // REMOVED the Reason line
                    label.Text = $"Loan ID: {loanId} | {customer}\n" +
                                 $"Type: {loanType} | {amount} | {term}\n" +
                                 $"Status: {status} (by Officer: {officer})";
                    break;
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.ToLower();

            // Skip if it's placeholder text
            if (searchTerm == "loan id or customer name")
            {
                // Show all rows
                foreach (DataGridViewRow row in loansDataGrid.Rows)
                {
                    row.Visible = true;
                }
                return;
            }

            foreach (DataGridViewRow row in loansDataGrid.Rows)
            {
                bool visible = string.IsNullOrEmpty(searchTerm) ||
                              row.Cells["ID"].Value.ToString().ToLower().Contains(searchTerm) ||
                              row.Cells["Customer"].Value.ToString().ToLower().Contains(searchTerm);

                row.Visible = visible;
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            // Validate inputs - check if it's placeholder text
            string reason = reasonTextBox.Text;
            if (string.IsNullOrWhiteSpace(reason) || reason == "Enter detailed reason for override...")
            {
                MessageBox.Show("Please enter an override reason.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string password = passwordTextBox.Text;
            if (string.IsNullOrWhiteSpace(password) || password == "Enter admin password")
            {
                MessageBox.Show("Please enter admin password.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate interest rate if that option is selected
            if (interestRadio.Checked)
            {
                if (!decimal.TryParse(interestTextBox.Text, out decimal interestRate) || interestRate < 0)
                {
                    MessageBox.Show("Please enter a valid interest rate.", "Validation Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to execute this override? This action will be logged in the audit trail.",
                "Confirm Override",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Determine which action was selected
                string selectedAction = "Approve Loan";
                if (modifyRadio.Checked) selectedAction = "Modify Terms";
                else if (interestRadio.Checked) selectedAction = $"Change Interest Rate to {interestTextBox.Text}%";
                else if (waiveRadio.Checked) selectedAction = "Waive Penalties/Fees";

                // Log the override (in real implementation, save to database)
                LogOverrideAction(selectedAction);

                MessageBox.Show("Override executed successfully!", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                ResetForm();
            }
        }

        private void LogOverrideAction(string action)
        {
            // In a real implementation, this would save to a database
            Console.WriteLine($"Override logged: {action}");
            Console.WriteLine($"Reason: {reasonTextBox.Text}");
            Console.WriteLine($"Performed by Admin at: {DateTime.Now}");

            // You could also show a notification in the UI
            MessageBox.Show("Override has been logged in the audit trail.", "Audit Logged",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetForm()
        {
            // Reset textboxes to placeholder state
            reasonTextBox.ForeColor = Color.Gray;
            reasonTextBox.Text = "Enter detailed reason for override...";

            passwordTextBox.ForeColor = Color.Gray;
            passwordTextBox.Text = "Enter admin password";

            interestTextBox.Text = "0.00";
            interestTextBox.Enabled = false;
            approveRadio.Checked = true;

            // Hide selected loan panel
            selectedLoanPanel.Visible = false;
            noLoanPanel.Visible = true;

            // Clear selection in grid
            loansDataGrid.ClearSelection();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
    }
}