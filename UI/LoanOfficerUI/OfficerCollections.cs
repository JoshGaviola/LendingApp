using LendingApp.LogicClass.LoanOfficer;
using LendingApp.Models.LoanOfficer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using System.Data.Entity;
using LendingApp.Class.Models.LoanOfiicerModels;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerCollections : Form
    {
        // Host mode: when true, do not render our own header/sidebar (embed inside OfficerDashboard
        private readonly bool _hosted;
        private OfficerCollectionLogic collectionLogic;

        // Data model
        private readonly List<CollectionItem> dbCollections = new List<CollectionItem>();

        // Shell (consistent with OfficerDashboard)
        private Panel headerPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Header
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private Button btnExport;
        private Button btnRecordPayment;

        // Sidebar/nav (standalone only)
        private string activeNav = "Portfolio Monitoring";
        private readonly List<string> navItems = new List<string>
        {
            "Dashboard", "Applications", "Customers", "Portfolio Monitoring", "Calendar", "Settings"
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


        private readonly string[] filterOptions = { "All", "Overdue", "Due Today", "Upcoming", "Collected" };

        // Constructors
        public OfficerCollections() : this(false) { }

        public OfficerCollections(bool hosted)
        {
            _hosted = hosted;
            InitializeComponent();
            BuildShell();

            // create logic holder and attempt to load DB-backed data
            collectionLogic = new OfficerCollectionLogic();
            LoadCollectionsFromDb();

            BuildCollectionsHome();
            BindEvents();
            ApplyData();
            RefreshTable();
        }

        /// <summary>
        /// Load collections from the database. If DB read fails we silently fall back to the
        /// built-in sample data already present in OfficerCollectionLogic.
        /// </summary>
        private void LoadCollectionsFromDb()
        {
            try
            {
                var svc = new LendingApp.Class.Services.Collections.OfficerCollectionService();
                var items = svc.GetCollections();

                if (items != null && items.Any())
                {
                    dbCollections.Clear();
                    dbCollections.AddRange(items);

                    var summary = svc.CalculateSummary(items);

                    // Update the existing collectionLogic fields so UI cards keep using collectionLogic
                    collectionLogic.summary["dueToday"] = summary.DueToday;
                    collectionLogic.summary["overdue"] = summary.Overdue;
                    collectionLogic.summary["thisWeek"] = summary.ThisWeek;
                    collectionLogic.summary["collectedToday"] = summary.CollectedToday;

                    collectionLogic.CollectionRate = summary.CollectionRate;
                    collectionLogic.AverageCollection = summary.AverageCollection;
                    collectionLogic.FollowUp = summary.FollowUp;
                }
            }
            catch
            {
                // keep UI resilient: leave in-memory sample data in place if service fails
            }
        }

        /// <summary>
        /// Map the DB collection status + metadata to the UI status categories used by filters.
        /// </summary>
        private string MapToUiStatus(string dbStatus, int daysOverdue, DateTime dueDate)
        {
            if (!string.IsNullOrWhiteSpace(dbStatus) && dbStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                return "Collected";

            if (daysOverdue > 0)
                return "Overdue";

            if (dueDate.Date == DateTime.Today)
                return "Due Today";

            // default: upcoming
            return "Upcoming";
        }

        private void BuildShell()
        {
            Text = "Portfolio Monitoring";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            if (_hosted)
            {
                // Hosted: no header/sidebar; only provide a fill content host
                contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true };
                Controls.Clear();
                Controls.Add(contentPanel);
                return;
            }

            // Standalone shell (header + sidebar) to match OfficerDashboard
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };
            lblHeaderTitle = new Label
            {
                Text = "PORTFOLIO MONITORING",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            lblHeaderSubtitle = new Label
            {
                Text = "Monitor portfolio health and collection status",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                AutoSize = true,
                Location = new Point(16, 34)
            };
            btnExport = new Button
            {
                Text = "Export",
                Width = 90,
                Height = 28,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");

            // Officers monitor — action should be "View Payments" rather than recording payments
            btnRecordPayment = new Button
            {
                Text = "View Payments",
                Width = 140,
                Height = 28,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRecordPayment.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");

            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            BuildSidebar();

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true
            };

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
            if (_hosted) return;

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
                    else if (item == "Portfolio Monitoring")
                    {
                        ShowCollectionsHome();
                    }
                    else if (item == "Dashboard")
                    {
                        NavigateToDashboard();
                    }
                    else
                    {
                        contentPanel.Controls.Clear();
                        var placeholder = new Label
                        {
                            Text = $"{item} view coming soon",
                            AutoSize = true,
                            Location = new Point(20, 20),
                            ForeColor = ColorTranslator.FromHtml("#6B7280")
                        };
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
            if (_hosted) return; // host manages dashboard UI
            var dash = new OfficerDashboard();
            dash.Show();
            Close();
        }

        private void BuildCollectionsHome()
        {
            // Build panels and add to contentPanel
            summaryPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.Transparent, Padding = new Padding(10, 10, 10, 0) };
            cardDueToday = MakeSummaryCard("#FFEDD5", "#EA580C", "#F97316", "Due Today", collectionLogic.summary["dueToday"], "3 payments");
            cardOverdue = MakeSummaryCard("#FEE2E2", "#DC2626", "#EF4444", "Overdue", collectionLogic.summary["overdue"], "2 accounts");
            cardThisWeek = MakeSummaryCard("#DBEAFE", "#1D4ED8", "#2563EB", "This Week", collectionLogic.summary["thisWeek"], "Expected");
            cardCollectedToday = MakeSummaryCard("#ECFDF5", "#065F46", "#10B981", "Collected Today", collectionLogic.summary["collectedToday"], "2 payments");
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
            lblTableTitle = new Label { Text = "PORTFOLIO ITEMS", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true, Location = new Point(16, 16) };
            lblItemsCount = new Label { Text = "0 items", Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml("#1D4ED8"), AutoSize = true, Location = new Point(170, 18), BackColor = ColorTranslator.FromHtml("#DBEAFE"), Padding = new Padding(6, 2, 6, 2) };
            lblShowingFilter = new Label { Text = "Showing: All", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true };
            tableHeader.Resize += (s, e) =>
            {
                lblShowingFilter.Location = new Point(tableHeader.Width - lblShowingFilter.Width - 16, 16);
            };

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

            tableFooter = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8)
            };
            lblTotalItems = new Label { Text = "Total: 0 items", Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#4B5563"), AutoSize = true, Location = new Point(12, 10) };
            lblTotalAmount = new Label { Text = "Total Amount: ₱0", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true };
            tableFooter.Resize += (s, e) =>
            {
                lblTotalAmount.Location = new Point(tableFooter.Width - lblTotalAmount.Width - 12, 10);
            };

            tableContainer.Controls.Add(gridHost);
            tableContainer.Controls.Add(tableFooter);
            tableContainer.Controls.Add(tableHeader);
            tableHeader.Controls.Add(lblTableTitle);
            tableHeader.Controls.Add(lblItemsCount);
            tableHeader.Controls.Add(lblShowingFilter);

            quickStatsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(10, 0, 10, 10) };
            qsRate = MakeQuickStatCard("Collection Rate", collectionLogic.CollectionRate = "75.5%", "+5.2% from last week", "#EDE9FE", "#7C3AED");
            qsAverage = MakeQuickStatCard("Average Collection", collectionLogic.AverageCollection = "3,548", "Per payment", "#DBEAFE", "#2563EB");
            qsFollowups = MakeQuickStatCard("Follow-ups Today", collectionLogic.FollowUp = "5", " 2 high priority", "#FFEDD5", "#EA580C");
            quickStatsPanel.Controls.Add(qsRate);
            quickStatsPanel.Controls.Add(qsAverage);
            quickStatsPanel.Controls.Add(qsFollowups);

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
            // Standalone only: ensure our collections layout is visible
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
            if (_hosted) return; // hosted navigation handled by dashboard

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
            if (_hosted) return; // hosted navigation handled by dashboard

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

            if (!_hosted)
            {
                btnExport.Click += (s, e) => MessageBox.Show("Export portfolio data (CSV/Excel)", "Export");
                btnRecordPayment.Click += (s, e) => MessageBox.Show("Open payment records (view-only)", "Payments");
            }
        }

        private void ApplyData()
        {
            cmbStatusFilter.SelectedIndex = 0; // All
            lblShowingFilter.Text = $"Showing: {selectedFilter}";
        }

        private void RefreshTable()
        {
            // Prefer DB-backed collections if available; otherwise fall back to in-memory sample data
            var source = dbCollections.Any() ? dbCollections : collectionLogic.Allcollections.ToList();

            var filtered = source.Where(item =>
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

            // update counts and totals
            lblItemsCount.Text = $"{filtered.Count} items";
            lblTotalItems.Text = $"Total: {filtered.Count} items";

            // compute total amount from the Amount strings (format "₱x,xxx")
            decimal totalAmount = 0m;
            foreach (var it in filtered)
            {
                if (string.IsNullOrWhiteSpace(it.Amount)) continue;
                var cleaned = it.Amount.Replace("₱", "").Replace(",", "").Trim();
                decimal val;
                if (decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
                {
                    totalAmount += val;
                }
            }
            lblTotalAmount.Text = $"Total Amount: ₱{totalAmount.ToString("N0", CultureInfo.InvariantCulture)}";
        }

        private void GridCollections_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Only handle clicks on the Actions button column
            if (!(gridCollections.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var loanId = gridCollections.Rows[e.RowIndex].Cells["LoanId"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(loanId))
            {
                MessageBox.Show("Loan identifier is missing.", "View Loan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Resolve loan -> application number when possible, then open ApprovedLoanApplicationDialog
                var appNumber = ResolveApplicationNumberFromLoanId(loanId);

                using (var dlg = new LendingApp.UI.LoanOfficerUI.Dialog.ApprovedLoanApplicationDialog(appNumber))
                {
                    dlg.StartPosition = FormStartPosition.CenterParent;
                    dlg.ShowDialog(this);

                    // Optionally refresh collections after closing if the dialog may change state
                    RefreshTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open loan view:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Try to resolve a displayed LoanId (could be loan_number or a LN-{loanId} placeholder)
        /// into an application number acceptable to ApprovedLoanApplicationDialog.
        /// Returns the original loanId when resolution fails so the dialog can attempt lookup by application number.
        /// </summary>
        private string ResolveApplicationNumberFromLoanId(string loanId)
        {
            if (string.IsNullOrWhiteSpace(loanId)) return loanId;

            try
            {
                using (var db = new AppDbContext())
                {
                    // 1) Try exact loan_number match
                    var loan = db.Loans.AsNoTracking().FirstOrDefault(l => l.LoanNumber == loanId);
                    if (loan == null)
                    {
                        // 2) Try parse LN-{id} pattern
                        if (loanId.StartsWith("LN-", StringComparison.OrdinalIgnoreCase))
                        {
                            var part = loanId.Substring(3);
                            int numericId;
                            if (int.TryParse(part, out numericId))
                            {
                                loan = db.Loans.AsNoTracking().FirstOrDefault(l => l.LoanId == numericId);
                            }
                        }
                    }

                    if (loan != null)
                    {
                        // find corresponding application entity to get the application number
                        var app = db.LoanApplications.AsNoTracking().FirstOrDefault(a => a.ApplicationId == loan.ApplicationId);
                        if (app != null && !string.IsNullOrWhiteSpace(app.ApplicationNumber))
                            return app.ApplicationNumber;
                    }
                }
            }
            catch
            {
                // fail silently — return original id below
            }

            // fallback: maybe the UI already contains an application number; return original value
            return loanId;
        }

        private string GetActionText(string status)
        {
            // Officers monitor only — always provide a view action
            return "View";
        }

        private string DaysCellText(int days)
        {
            return days > 0 ? $"+{days}" : "0";
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
