using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerCollections : Form
    {
        // Data model (mirrors the TypeScript interface)
        private class CollectionItem
        {
            public string Id { get; set; }
            public string LoanId { get; set; }
            public string Customer { get; set; }
            public string DueDate { get; set; }
            public string Amount { get; set; }
            public int DaysOverdue { get; set; }
            public string Contact { get; set; }
            public string Priority { get; set; } // Critical | High | Medium | Low
            public string Status { get; set; }   // Overdue | Due Today | Upcoming
        }

        // Shell (consistent with OfficerDashboard)
        private Panel headerPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Header
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private Button btnExport;
        private Button btnRecordPayment;

        // Sidebar/nav (consistent with OfficerDashboard)
        private string activeNav = "Collections";
        private readonly List<string> navItems = new List<string>
            {
                "Dashboard", "Applications", "Customers", "Collections", "Calendar", "Settings"
            };
        private OfficerApplications _applicationsForm;
        private OfficerCustomers _customersForm;

        // Collections home content
        private Panel summaryPanel;
        private Panel cardDueToday;
        private Panel cardOverdue;
        private Panel cardThisWeek;
        private Panel cardCollectedToday;

        private Panel filtersPanel;
        private ComboBox cmbStatusFilter;
        private TextBox txtStartDate;
        private TextBox txtEndDate;
        private TextBox txtSearch;

        private Panel tableContainer;
        private Panel tableHeader;
        private Label lblTableTitle;
        private Label lblItemsCount;
        private Label lblShowingFilter;
        private DataGridView gridCollections;

        private Panel tableFooter;
        private Label lblTotalItems;
        private Label lblTotalAmount;

        private Panel quickStatsPanel;
        private Panel qsRate;
        private Panel qsAverage;
        private Panel qsFollowups;

        // Data
        private string selectedFilter = "All";
        private string startDate = "Dec 14";
        private string endDate = "Dec 21";
        private string searchTerm = "";
        private readonly Dictionary<string, string> summary = new Dictionary<string, string>
            {
                { "dueToday", "₱8,292" },
                { "overdue", "₱12,442" },
                { "thisWeek", "₱25,000" },
                { "collectedToday", "₱5,250" }
            };
        private readonly List<CollectionItem> collections = new List<CollectionItem>
            {
                new CollectionItem { Id="1", LoanId="LN-001", Customer="Juan Cruz",  DueDate="Dec 15", Amount="₱4,442", DaysOverdue=0, Contact="+639123456789", Priority="High",     Status="Due Today" },
                new CollectionItem { Id="2", LoanId="LN-002", Customer="Pedro Reyes",DueDate="Dec 10", Amount="₱3,850", DaysOverdue=5, Contact="+639456789012", Priority="Critical", Status="Overdue" },
                new CollectionItem { Id="3", LoanId="LN-003", Customer="Maria Santos",DueDate="Dec 20", Amount="₱3,850", DaysOverdue=0, Contact="+639987654321", Priority="Medium",  Status="Upcoming" },
                new CollectionItem { Id="4", LoanId="LN-004", Customer="Ana Lopez",   DueDate="Dec 15", Amount="₱2,500", DaysOverdue=0, Contact="+639111222333", Priority="Medium",  Status="Due Today" },
                new CollectionItem { Id="5", LoanId="LN-005", Customer="Carlos Tan",  DueDate="Dec 12", Amount="₱4,150", DaysOverdue=3, Contact="+639444555666", Priority="High",    Status="Overdue" },
                new CollectionItem { Id="6", LoanId="LN-006", Customer="Rosa Garcia", DueDate="Dec 18", Amount="₱3,200", DaysOverdue=0, Contact="+639777888999", Priority="Low",     Status="Upcoming" },
            };

        private readonly string[] filterOptions = { "All", "Overdue", "Due Today", "Upcoming", "Collected" };

        public OfficerCollections()
        {
            InitializeComponent();
            BuildShell();
            BuildCollectionsHome();
            BindEvents();
            ApplyData();
            RefreshTable();
        }

        private void BuildShell()
        {
            Text = "Officer Collections";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Header
            headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(16), BorderStyle = BorderStyle.FixedSingle };
            lblHeaderTitle = new Label { Text = "COLLECTIONS MANAGEMENT", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#2C3E50"), AutoSize = true, Location = new Point(16, 12) };
            lblHeaderSubtitle = new Label { Text = "Track and manage all payment collections", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true, Location = new Point(16, 34) };
            btnExport = new Button { Text = "Export", Width = 90, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExport.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnRecordPayment = new Button { Text = "Record Payment", Width = 140, Height = 28, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRecordPayment.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");

            // Sidebar
            sidebarPanel = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            BuildSidebar();

            // Content host
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true };

            // Add to form in the same order as OfficerDashboard
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(headerPanel);

            headerPanel.Controls.Add(lblHeaderTitle);
            headerPanel.Controls.Add(lblHeaderSubtitle);
            headerPanel.Controls.Add(btnExport);
            headerPanel.Controls.Add(btnRecordPayment);
            headerPanel.Resize += (s, e) =>
            {
                btnRecordPayment.Location = new Point(headerPanel.Width - btnRecordPayment.Width - 16, 16);
                btnExport.Location = new Point(btnRecordPayment.Left - btnExport.Width - 8, 16);
            };
        }

        private void BuildSidebar()
        {
            sidebarPanel.Controls.Clear();
            int y = 10;
            foreach (var item in navItems)
            {
                var btn = new Button
                {
                    Text = item,
                    Location = new Point(10, y),
                    Size = new Size(sidebarPanel.Width - 20, 36),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = activeNav == item ? ColorTranslator.FromHtml("#E8F4FF") : Color.White
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) =>
                {
                    activeNav = item;
                    if (item == "Applications")
                    {
                        ShowApplicationsView();
                    }
                    else if (item == "Customers")
                    {
                        ShowCustomersView();
                    }
                    else if (item == "Collections")
                    {
                        ShowCollectionsHome();
                    }
                    else if (item == "Dashboard")
                    {
                        NavigateToDashboard();
                    }
                    else
                    {
                        // Placeholder for Calendar/Settings
                        contentPanel.Controls.Clear();
                        var placeholder = new Label { Text = $"{item} view coming soon", AutoSize = true, Location = new Point(20, 20), ForeColor = ColorTranslator.FromHtml("#6B7280") };
                        contentPanel.Controls.Add(placeholder);
                    }
                    BuildSidebar(); // refresh highlight
                };
                sidebarPanel.Controls.Add(btn);
                y += 42;
            }
        }

        private void NavigateToDashboard()
        {
            var dash = new OfficerDashboard();
            dash.Show();
            Close();
        }

        private void BuildCollectionsHome()
        {
            // Build panels (do not add to Form; add to contentPanel)
            summaryPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.Transparent, Padding = new Padding(10, 10, 10, 0) };
            cardDueToday = MakeSummaryCard("#FFEDD5", "#EA580C", "#F97316", "Due Today", summary["dueToday"], "3 payments");
            cardOverdue = MakeSummaryCard("#FEE2E2", "#DC2626", "#EF4444", "Overdue", summary["overdue"], "2 accounts");
            cardThisWeek = MakeSummaryCard("#DBEAFE", "#1D4ED8", "#2563EB", "This Week", summary["thisWeek"], "Expected");
            cardCollectedToday = MakeSummaryCard("#ECFDF5", "#065F46", "#10B981", "Collected Today", summary["collectedToday"], "2 payments");
            summaryPanel.Controls.Add(cardDueToday);
            summaryPanel.Controls.Add(cardOverdue);
            summaryPanel.Controls.Add(cardThisWeek);
            summaryPanel.Controls.Add(cardCollectedToday);
            summaryPanel.Resize += (s, e) => LayoutSummaryCards();

            filtersPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, Padding = new Padding(16), BorderStyle = BorderStyle.FixedSingle };
            var lblStatus = new Label { Text = "Status Filter", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#4B5563"), AutoSize = true, Location = new Point(10, 10) };
            cmbStatusFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(10, 30), Width = 220 };
            cmbStatusFilter.Items.AddRange(filterOptions);
            var lblDateRange = new Label { Text = "Date Range", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#4B5563"), AutoSize = true, Location = new Point(250, 10) };
            txtStartDate = new TextBox { Text = startDate, Location = new Point(250, 30), Width = 160 };
            var lblTo = new Label { Text = "to", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#6B7280"), Location = new Point(415, 34) };
            txtEndDate = new TextBox { Text = endDate, Location = new Point(440, 30), Width = 160 };
            var lblSearch = new Label { Text = "Search", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#4B5563"), AutoSize = true, Location = new Point(620, 10) };
            txtSearch = new TextBox { Location = new Point(620, 30), Width = 260 };
            filtersPanel.Controls.AddRange(new Control[] { lblStatus, cmbStatusFilter, lblDateRange, txtStartDate, lblTo, txtEndDate, lblSearch, txtSearch });

            tableContainer = new Panel { Dock = DockStyle.Top, Height = 420, BackColor = Color.Transparent, Padding = new Padding(10) };

            tableHeader = new Panel { Height = 50, Dock = DockStyle.Top, BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            lblTableTitle = new Label { Text = "COLLECTIONS LIST", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true, Location = new Point(16, 16) };
            lblItemsCount = new Label { Text = "0 items", Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml("#1D4ED8"), AutoSize = true, Location = new Point(170, 18), BackColor = ColorTranslator.FromHtml("#DBEAFE"), Padding = new Padding(6, 2, 6, 2) };
            lblShowingFilter = new Label { Text = "Showing: All", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true };
            tableHeader.Resize += (s, e) => { lblShowingFilter.Location = new Point(tableHeader.Width - lblShowingFilter.Width - 16, 16); };

            gridCollections = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
            gridCollections.Columns.Add("LoanId", "Loan ID");
            gridCollections.Columns.Add("Customer", "Customer");
            gridCollections.Columns.Add("DueDate", "Due Date");
            gridCollections.Columns.Add("Amount", "Amount");
            gridCollections.Columns.Add("Days", "Days");
            gridCollections.Columns.Add("Contact", "Contact");
            gridCollections.Columns.Add("Priority", "Priority");
            var actionsCol = new DataGridViewButtonColumn { HeaderText = "Actions", Text = "Action", UseColumnTextForButtonValue = false };
            gridCollections.Columns.Add(actionsCol);
            gridCollections.CellContentClick += GridCollections_CellContentClick;

            var gridHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            gridHost.Controls.Add(gridCollections);

            tableFooter = new Panel { Height = 40, Dock = DockStyle.Bottom, BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(12, 8, 12, 8) };
            lblTotalItems = new Label { Text = "Total: 0 collections", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#4B5563"), AutoSize = true, Location = new Point(12, 10) };
            lblTotalAmount = new Label { Text = "Total Amount: ₱0", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true };
            tableFooter.Resize += (s, e) => { lblTotalAmount.Location = new Point(tableFooter.Width - lblTotalAmount.Width - 12, 10); };

            tableContainer.Controls.Add(gridHost);
            tableContainer.Controls.Add(tableFooter);
            tableContainer.Controls.Add(tableHeader);
            tableHeader.Controls.Add(lblTableTitle);
            tableHeader.Controls.Add(lblItemsCount);
            tableHeader.Controls.Add(lblShowingFilter);

            quickStatsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(10, 0, 10, 10) };
            qsRate = MakeQuickStatCard("Collection Rate", "78.5%", "+5.2% from last week", "#EDE9FE", "#7C3AED");
            qsAverage = MakeQuickStatCard("Average Collection", "₱3,548", "Per payment", "#DBEAFE", "#2563EB");
            qsFollowups = MakeQuickStatCard("Follow-ups Today", "5 Required", "2 high priority", "#FFEDD5", "#EA580C");
            quickStatsPanel.Controls.Add(qsRate);
            quickStatsPanel.Controls.Add(qsAverage);
            quickStatsPanel.Controls.Add(qsFollowups);

            // Add to content host in docking order
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(quickStatsPanel);
            contentPanel.Controls.Add(tableContainer);
            contentPanel.Controls.Add(filtersPanel);
            contentPanel.Controls.Add(summaryPanel);

            contentPanel.Resize += (s, e) =>
            {
                LayoutSummaryCards();
                LayoutQuickStats();
            };
        }

        private void ShowCollectionsHome()
        {
            // Hide embedded forms and show collections layout
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }

            BuildCollectionsHome();
        }

        private void ShowApplicationsView()
        {
            contentPanel.Controls.Clear();
            if (_applicationsForm == null || _applicationsForm.IsDisposed)
            {
                _applicationsForm = new OfficerApplications
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }
            contentPanel.Controls.Add(_applicationsForm);
            _applicationsForm.Show();
        }

        private void ShowCustomersView()
        {
            contentPanel.Controls.Clear();
            if (_customersForm == null || _customersForm.IsDisposed)
            {
                _customersForm = new OfficerCustomers
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }
            contentPanel.Controls.Add(_customersForm);
            _customersForm.Show();
        }

        private void BindEvents()
        {
            cmbStatusFilter.SelectedIndexChanged += (s, e) =>
            {
                selectedFilter = cmbStatusFilter.SelectedItem?.ToString() ?? "All";
                lblShowingFilter.Text = $"Showing: {selectedFilter}";
                RefreshTable();
            };

            txtStartDate.TextChanged += (s, e) => { startDate = txtStartDate.Text; };
            txtEndDate.TextChanged += (s, e) => { endDate = txtEndDate.Text; };
            txtSearch.TextChanged += (s, e) =>
            {
                searchTerm = txtSearch.Text ?? "";
                RefreshTable();
            };

            btnExport.Click += (s, e) => MessageBox.Show("Export collections (CSV/Excel)", "Export");
            btnRecordPayment.Click += (s, e) => MessageBox.Show("Record Payment workflow", "Record Payment");
        }

        private void ApplyData()
        {
            cmbStatusFilter.SelectedIndex = 0; // All
            lblShowingFilter.Text = $"Showing: {selectedFilter}";
        }

        private void RefreshTable()
        {
            var filtered = collections.Where(item =>
            {
                bool matchesFilter = selectedFilter == "All" || item.Status == selectedFilter;
                bool matchesSearch = string.IsNullOrWhiteSpace(searchTerm)
                    || (item.Customer?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                    || (item.LoanId?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;
                return matchesFilter && matchesSearch;
            }).ToList();

            gridCollections.Rows.Clear();

            foreach (var item in filtered)
            {
                int rowIndex = gridCollections.Rows.Add(item.LoanId, item.Customer, item.DueDate, item.Amount, DaysCellText(item.DaysOverdue), item.Contact, item.Priority, "");
                var row = gridCollections.Rows[rowIndex];

                var daysCell = row.Cells["Days"] as DataGridViewTextBoxCell;
                if (daysCell != null)
                {
                    daysCell.Style.BackColor = item.DaysOverdue > 0 ? ColorTranslator.FromHtml("#FEE2E2") : Color.White;
                    daysCell.Style.ForeColor = item.DaysOverdue > 0 ? ColorTranslator.FromHtml("#B91C1C") : ColorTranslator.FromHtml("#6B7280");
                }

                var prCell = row.Cells["Priority"] as DataGridViewTextBoxCell;
                if (prCell != null)
                {
                    var colors = GetPriorityColors(item.Priority);
                    prCell.Style.BackColor = colors.back;
                    prCell.Style.ForeColor = colors.fore;
                }

                var actionCell = row.Cells[gridCollections.Columns.Count - 1] as DataGridViewButtonCell;
                if (actionCell != null)
                {
                    actionCell.FlatStyle = FlatStyle.Standard;
                    actionCell.Value = GetActionText(item.Status);
                }

                var amtCell = row.Cells["Amount"] as DataGridViewTextBoxCell;
                if (amtCell != null)
                {
                    amtCell.Style.ForeColor = ColorTranslator.FromHtml("#111827");
                }

                var loanCell = row.Cells["LoanId"] as DataGridViewTextBoxCell;
                if (loanCell != null)
                {
                    loanCell.Style.ForeColor = ColorTranslator.FromHtml("#2563EB");
                }
            }

            lblItemsCount.Text = $"{filtered.Count} items";
            lblTotalItems.Text = $"Total: {filtered.Count} collection{(filtered.Count != 1 ? "s" : "")}";
            lblTotalAmount.Text = $"Total Amount: ₱{ComputeTotalAmount(filtered).ToString("N0", CultureInfo.InvariantCulture)}";
        }

        private void GridCollections_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridCollections.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var statusAction = gridCollections.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                var loanId = gridCollections.Rows[e.RowIndex].Cells["LoanId"].Value?.ToString();
                MessageBox.Show($"{statusAction} {loanId}", "Collections Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetActionText(string status)
        {
            if (status == "Overdue" || status == "Due Today") return "Collect";
            return "View";
        }

        private string DaysCellText(int days)
        {
            return days > 0 ? $"+{days}" : "0";
        }

        private decimal ComputeTotalAmount(List<CollectionItem> items)
        {
            decimal sum = 0;
            foreach (var i in items)
            {
                var cleaned = i.Amount.Replace("₱", "").Replace(",", "");
                if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    sum += val;
            }
            return sum;
        }

        private (Color back, Color fore) GetPriorityColors(string priority)
        {
            switch (priority)
            {
                case "Critical": return (ColorTranslator.FromHtml("#FEE2E2"), ColorTranslator.FromHtml("#B91C1C"));
                case "High": return (ColorTranslator.FromHtml("#FFEDD5"), ColorTranslator.FromHtml("#9A3412"));
                case "Medium": return (ColorTranslator.FromHtml("#FEF3C7"), ColorTranslator.FromHtml("#92400E"));
                case "Low": return (ColorTranslator.FromHtml("#D1FAE5"), ColorTranslator.FromHtml("#065F46"));
                default: return (ColorTranslator.FromHtml("#F3F4F6"), ColorTranslator.FromHtml("#374151"));
            }
        }

        private Panel MakeSummaryCard(string backHex, string titleHex, string accentHex, string title, string value, string sub)
        {
            var card = new Panel { Width = 250, Height = 80, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var inner = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            card.Controls.Add(inner);

            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true, Location = new Point(10, 10) };
            var lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml(titleHex), AutoSize = true, Location = new Point(10, 30) };
            var lblSub = new Label { Text = sub, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml(accentHex), AutoSize = true, Location = new Point(10, 52) };

            var iconBox = new Panel { Width = 40, Height = 40, BackColor = ColorTranslator.FromHtml(backHex), Location = new Point(card.Width - 60, 20) };
            iconBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            inner.Controls.Add(lblTitle);
            inner.Controls.Add(lblValue);
            inner.Controls.Add(lblSub);
            inner.Controls.Add(iconBox);

            return card;
        }

        private Panel MakeQuickStatCard(string title, string value, string sub, string backHex, string accentHex)
        {
            var card = new Panel { Width = 300, Height = 90, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(10) };
            var iconBox = new Panel { Width = 40, Height = 40, BackColor = ColorTranslator.FromHtml(backHex), Location = new Point(10, 25) };

            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true, Location = new Point(60, 16) };
            var lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true, Location = new Point(60, 38) };
            var lblSub = new Label { Text = sub, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml(accentHex), AutoSize = true, Location = new Point(60, 58) };

            card.Controls.Add(iconBox);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblSub);
            return card;
        }

        private void LayoutSummaryCards()
        {
            int gap = 10;
            int startX = 10;
            int y = 10;
            // Distribute 4 cards horizontally
            cardDueToday.Location = new Point(startX, y);
            cardOverdue.Location = new Point(cardDueToday.Right + gap, y);
            cardThisWeek.Location = new Point(cardOverdue.Right + gap, y);
            cardCollectedToday.Location = new Point(cardThisWeek.Right + gap, y);
        }

        private void LayoutQuickStats()
        {
            int gap = 10;
            int startX = 10;
            int y = 10;

            // 3 cards grid responsive: 1/2/3 columns
            int w = quickStatsPanel.Width - quickStatsPanel.Padding.Left - quickStatsPanel.Padding.Right;
            int columns = w >= 1280 ? 3 : (w >= 900 ? 2 : 1);
            int cardWidth = (w - (gap * (columns - 1))) / columns;

            qsRate.Width = cardWidth;
            qsAverage.Width = cardWidth;
            qsFollowups.Width = cardWidth;

            qsRate.Location = new Point(startX, y);
            qsAverage.Location = columns >= 2 ? new Point(qsRate.Right + gap, y) : new Point(startX, qsRate.Bottom + gap);
            qsFollowups.Location = columns >= 3 ? new Point(qsAverage.Right + gap, y)
                                                : (columns == 2 ? new Point(startX, qsAverage.Bottom + gap)
                                                                : new Point(startX, qsAverage.Bottom + gap));
        }
    }
}
