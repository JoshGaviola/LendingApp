using LendingApp.Models.LoanOfficer;
using LendingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class.Services;
using static LendingApp.Class.Services.DataGetter;
using LendingApp.Class;                 // <-- needed for AppDbContext
using System.Data.Entity;               // <-- AsNoTracking

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerDashboard : Form
    {
        OfficerDashboardLogic dashboard = new OfficerDashboardLogic();
        OfficerApplicationLogic officerAppLogic;

        private string _username = "Officer";
        private Action _onLogout;

        private string activeNav = "Dashboard";
        private readonly List<string> navItems = new List<string>
        {
            "Dashboard", "Applications", "Customers", "Collections", "Settings", "Loan Calculator"
        };

        // Embedded views
        private OfficerApplications _applicationsForm;
        private OfficerCollections _collectionsForm;
        private OfficerCustomers _customersForm; // Added for customers view
        private OfficerSettings _settingsForm; // Added for settings view
        private bool _homeResizeHooked;

        private OfficerApplicationReviewControl _reviewControl;
        private OfficerCollectionFollowUpControl _collectionFollowUpControl;
        private OfficerApplicationLogic applicationLogic;

        // Keep latest DB-backed overdue rows so the Follow-Up dialog can use real data.
        private readonly List<OverdueLoanData> _dbOverdueLoans = new List<OverdueLoanData>();

        private class ActivityItem
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string Activity { get; set; }
            public string Customer { get; set; }
            public string Amount { get; set; }
        }

        public OfficerDashboard()
        {
            InitializeComponent();
            applicationLogic = new OfficerApplicationLogic(DataGetter.Data);

            BuildUI();
            PopulateData();
        }

        public void SetUsername(string username)
        {
            _username = string.IsNullOrWhiteSpace(username) ? "Officer" : username;
            lblWelcome.Text = $"Welcome, {_username}";
        }

        public void OnLogout(Action logout)
        {
            _onLogout = logout;
        }

        private void BuildUI()
        {
            // Form settings
            Text = "Officer Dashboard";
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            // Header bar
            headerPanel.BackColor = Color.White;
            headerPanel.Height = 60;
            headerPanel.BorderStyle = BorderStyle.FixedSingle;

            lblTitle.Text = "OFFICER DASHBOARD";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = ColorTranslator.FromHtml("#2C3E50");

            lblWelcome.Text = $"Welcome, {_username}";
            lblWelcome.Font = new Font("Segoe UI", 9);
            lblWelcome.ForeColor = ColorTranslator.FromHtml("#6B7280");

            btnNotifications.Text = "🔔 5";
            btnNotifications.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btnNotifications.BackColor = Color.White;
            btnNotifications.FlatStyle = FlatStyle.Flat;
            btnNotifications.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");

            btnLogout.Text = "Logout";
            btnLogout.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btnLogout.BackColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnLogout.Click += (s, e) =>
            {
                _onLogout?.Invoke();
                Close();
            };

            // Summary bar
            summaryPanel.BackColor = Color.White;
            summaryPanel.Height = 80;
            summaryPanel.BorderStyle = BorderStyle.FixedSingle;

            // Sidebar
            sidebarPanel.BackColor = Color.White;
            sidebarPanel.Width = 220;
            sidebarPanel.BorderStyle = BorderStyle.FixedSingle;
            BuildSidebar();

            // Content panel
            contentPanel.BackColor = Color.White;
            contentPanel.BorderStyle = BorderStyle.None;
            contentPanel.AutoScroll = true;

            // Sections
            BuildPendingApplicationsSection();
            BuildOverdueLoansSection();
            BuildTasksSection();
            BuildActivitySection();

            LayoutHeader();
            LayoutSummary();
            LayoutMain();
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
                        ShowCollectionsView();
                    }
                    else if (item == "Settings")
                    {
                        ShowSettingsView();
                    }
                    else if (item == "Dashboard") // FIX: handle dashboard explicitly
                    {
                        ShowDashboardHome();
                    }
                    else if (item == "Loan Calculator")
                    {
                        using (var f = new LoanCalculatorForm())
                            f.ShowDialog(this);
                    }
                    else
                    {
                        summaryPanel.Visible = false;
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

        private void LayoutHeader()
        {
            lblTitle.Location = new Point(16, 18);
            lblWelcome.Location = new Point(200, 20);
            btnNotifications.Size = new Size(70, 28);
            btnLogout.Size = new Size(80, 28);
            btnNotifications.Location = new Point(headerPanel.Width - 170, 16);
            btnLogout.Location = new Point(headerPanel.Width - 90, 16);

            headerPanel.Resize += (s, e) =>
            {
                btnNotifications.Left = headerPanel.Width - 170;
                btnLogout.Left = headerPanel.Width - 90;
            };
        }

        private void LayoutSummary()
        {
            int cardWidth = 220;
            int gap = 10;
            int startX = 10;
            int startY = 10;

            cardPending.BorderStyle = BorderStyle.FixedSingle;
            cardPending.BackColor = ColorTranslator.FromHtml("#EFF6FF");
            cardPending.Location = new Point(startX, startY);
            cardPending.Size = new Size(cardWidth, 60);

            lblPendingTitle.Text = "Pending";
            lblPendingTitle.ForeColor = ColorTranslator.FromHtml("#1D4ED8");
            lblPendingTitle.Location = new Point(10, 8);
            lblPendingCount.Text = $"[{applicationLogic.TotalApplications}]";
            lblPendingCount.ForeColor = ColorTranslator.FromHtml("#1E3A8A");
            lblPendingCount.Location = new Point(10, 28);
            lblPendingSub.Text = "Applications";
            lblPendingSub.ForeColor = ColorTranslator.FromHtml("#2563EB");
            lblPendingSub.Location = new Point(90, 28);
            cardPending.Controls.Add(lblPendingTitle);
            cardPending.Controls.Add(lblPendingCount);
            cardPending.Controls.Add(lblPendingSub);

            cardActive.BorderStyle = BorderStyle.FixedSingle;
            cardActive.BackColor = ColorTranslator.FromHtml("#ECFDF5");
            cardActive.Location = new Point(startX + (cardWidth + gap), startY);
            cardActive.Size = new Size(cardWidth, 60);
            lblActiveTitle.Text = "Active";
            lblActiveTitle.ForeColor = ColorTranslator.FromHtml("#047857");
            lblActiveTitle.Location = new Point(10, 8);
            lblActiveValue.Text = dashboard.activePortfolio = "6000";
            lblActiveValue.ForeColor = ColorTranslator.FromHtml("#065F46");
            lblActiveValue.Location = new Point(10, 28);
            lblActiveSub.Text = "Portfolio";
            lblActiveSub.ForeColor = ColorTranslator.FromHtml("#10B981");
            lblActiveSub.Location = new Point(120, 28);
            cardActive.Controls.Add(lblActiveTitle);
            cardActive.Controls.Add(lblActiveValue);
            cardActive.Controls.Add(lblActiveSub);

            cardOverdue.BorderStyle = BorderStyle.FixedSingle;
            cardOverdue.BackColor = ColorTranslator.FromHtml("#FEE2E2");
            cardOverdue.Location = new Point(startX + 2 * (cardWidth + gap), startY);
            cardOverdue.Size = new Size(cardWidth, 60);
            lblOverdueTitle.Text = "Overdue";
            lblOverdueTitle.ForeColor = ColorTranslator.FromHtml("#DC2626");
            lblOverdueTitle.Location = new Point(10, 8);

            // This will be updated in BuildOverdueLoansSection after DB load.
            lblOverdueCount.Text = $"[{dashboard.TotalOverdueLoans}]";

            lblOverdueCount.ForeColor = ColorTranslator.FromHtml("#991B1B");
            lblOverdueCount.Location = new Point(10, 28);
            lblOverdueSub.Text = "Loans";
            lblOverdueSub.ForeColor = ColorTranslator.FromHtml("#EF4444");
            lblOverdueSub.Location = new Point(90, 28);
            cardOverdue.Controls.Add(lblOverdueTitle);
            cardOverdue.Controls.Add(lblOverdueCount);
            cardOverdue.Controls.Add(lblOverdueSub);

            cardCollections.BorderStyle = BorderStyle.FixedSingle;
            cardCollections.BackColor = ColorTranslator.FromHtml("#FFEDD5");
            cardCollections.Location = new Point(startX + 3 * (cardWidth + gap), startY);
            cardCollections.Size = new Size(cardWidth, 60);
            lblCollectionsTitle.Text = "Today";
            lblCollectionsTitle.ForeColor = ColorTranslator.FromHtml("#EA580C");
            lblCollectionsTitle.Location = new Point(10, 8);
            lblCollectionsValue.Text = $"[{dashboard.todayCollection = "5000"}]";
            lblCollectionsValue.ForeColor = ColorTranslator.FromHtml("#9A3412");
            lblCollectionsValue.Location = new Point(10, 28);
            lblCollectionsSub.Text = "Collections";
            lblCollectionsSub.ForeColor = ColorTranslator.FromHtml("#F97316");
            lblCollectionsSub.Location = new Point(120, 28);
            cardCollections.Controls.Add(lblCollectionsTitle);
            cardCollections.Controls.Add(lblCollectionsValue);
            cardCollections.Controls.Add(lblCollectionsSub);
        }

        private void LayoutMain()
        {
            headerPanel.Dock = DockStyle.Top;
            summaryPanel.Dock = DockStyle.Top;
            sidebarPanel.Dock = DockStyle.Left;
            contentPanel.Dock = DockStyle.Fill;

            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(summaryPanel);
            Controls.Add(headerPanel);

            PlaceHomeSections();
            _homeResizeHooked = true;
        }

        private void PlaceHomeSections()
        {
            summaryPanel.Visible = true;
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            int y = 10;

            sectionPending.Location = new Point(10, y);
            sectionPending.Size = new Size(contentPanel.Width - 40, 240);
            sectionPending.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Controls.Add(sectionPending);
            y += sectionPending.Height + 10;

            sectionOverdue.Location = new Point(10, y);
            sectionOverdue.Size = new Size(contentPanel.Width - 40, 240);
            sectionOverdue.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Controls.Add(sectionOverdue);
            y += sectionOverdue.Height + 10;

            sectionTasks.Location = new Point(10, y);
            sectionTasks.Size = new Size((contentPanel.Width - 50) / 2, 260);
            sectionTasks.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Controls.Add(sectionTasks);

            sectionActivity.Location = new Point(sectionTasks.Right + 10, y);
            sectionActivity.Size = new Size((contentPanel.Width - 50) / 2, 260);
            sectionActivity.BorderStyle = BorderStyle.FixedSingle;
            contentPanel.Controls.Add(sectionActivity);

            if (!_homeResizeHooked)
            {
                contentPanel.Resize += (s, e) =>
                {
                    sectionPending.Width = contentPanel.Width - 40;
                    sectionOverdue.Width = contentPanel.Width - 40;
                    sectionTasks.Width = (contentPanel.Width - 50) / 2;
                    sectionActivity.Width = (contentPanel.Width - 50) / 2;
                    sectionActivity.Left = sectionTasks.Right + 10;
                };
                _homeResizeHooked = true;
            }

            contentPanel.ResumeLayout();
        }

        private void ShowApplicationsView()
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }

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
            contentPanel.ResumeLayout();
        }

        private void ShowCollectionFollowUp(LendingSystem.OverdueLoanData loanData)
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            // Hide/remove other embedded views
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }
            if (_settingsForm != null && !_settingsForm.IsDisposed)
            {
                _settingsForm.Hide();
                contentPanel.Controls.Remove(_settingsForm);
            }
            if (_reviewControl != null && !_reviewControl.IsDisposed)
            {
                _reviewControl.Hide();
                contentPanel.Controls.Remove(_reviewControl);
            }

            // Create NEW follow-up control every time (don't reuse)
            _collectionFollowUpControl = new OfficerCollectionFollowUpControl(loanData)
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Wire the back button
            _collectionFollowUpControl.OnBack += () =>
            {
                // First remove the control
                if (_collectionFollowUpControl != null && !_collectionFollowUpControl.IsDisposed)
                {
                    _collectionFollowUpControl.Hide();
                    contentPanel.Controls.Remove(_collectionFollowUpControl);
                    _collectionFollowUpControl.Dispose();
                    _collectionFollowUpControl = null;
                }

                // Then show dashboard
                ShowDashboardHome();
            };

            contentPanel.Controls.Add(_collectionFollowUpControl);
            contentPanel.ResumeLayout();
        }

        private void ShowCollectionsView()
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

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

            if (_collectionsForm == null || _collectionsForm.IsDisposed)
            {
                _collectionsForm = new OfficerCollections(true)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_collectionsForm);
            _collectionsForm.Show();
            contentPanel.ResumeLayout();
        }

        private void ShowCustomersView()
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            // Hide/remove other embedded views
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }

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
            contentPanel.ResumeLayout();
        }

        private void ShowSettingsView()
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            // Hide/remove other embedded views
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }

            if (_settingsForm == null || _settingsForm.IsDisposed)
            {
                _settingsForm = new OfficerSettings
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_settingsForm);
            _settingsForm.Show();
            contentPanel.ResumeLayout();
        }

        private void ShowDashboardHome()
        {
            // Hide/remove all embedded views
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }
            if (_settingsForm != null && !_settingsForm.IsDisposed)
            {
                _settingsForm.Hide();
                contentPanel.Controls.Remove(_settingsForm);
            }

            // Properly clean up review control
            if (_reviewControl != null && !_reviewControl.IsDisposed)
            {
                _reviewControl.Hide();
                contentPanel.Controls.Remove(_reviewControl);
                _reviewControl.Dispose();
                _reviewControl = null;
            }

            // Properly clean up follow-up control
            if (_collectionFollowUpControl != null && !_collectionFollowUpControl.IsDisposed)
            {
                _collectionFollowUpControl.Hide();
                contentPanel.Controls.Remove(_collectionFollowUpControl);
                _collectionFollowUpControl.Dispose();
                _collectionFollowUpControl = null;
            }

            // Clear the content panel
            contentPanel.Controls.Clear();

            // Rebuild home sections
            BuildPendingApplicationsSection();
            BuildOverdueLoansSection();
            BuildTasksSection();
            BuildActivitySection();
            PlaceHomeSections();
        }

        private void BuildPendingApplicationsSection()
        {
            sectionPending.Controls.Clear();
            var header = new Label
            {
                Text = "PENDING APPLICATIONS (URGENT)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#FFF7ED"),
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grid.Columns.Add("Customer", "Customer");
            grid.Columns.Add("LoanType", "Loan Type");
            grid.Columns.Add("Amount", "Amount");
            grid.Columns.Add("Days", "Days");
            grid.Columns.Add("Priority", "Priority");
            var actionsCol = new DataGridViewButtonColumn
            {
                HeaderText = "Actions",
                Text = "Review",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(actionsCol);

            // Wire the Review button click
            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == actionsCol.Index && e.RowIndex >= 0)
                {
                    var row = grid.Rows[e.RowIndex];
                    string customer = row.Cells["Customer"].Value?.ToString() ?? "Unknown";
                    string loanType = row.Cells["LoanType"].Value?.ToString() ?? "Loan";
                    string amount = row.Cells["Amount"].Value?.ToString() ?? "₱0";
                    ShowApplicationReview(customer, loanType, amount);
                }
            };

            sectionPending.Controls.Add(grid);
            sectionPending.Controls.Add(header);

            // Load real loan applications from DB (populate ApplicantsData.AllLoans)
            try
            {
                // Request all loans; OfficerApplicationLogic/GetApplications treats "All Status" as no status filter
                DataGetter.Data.LoadLoans("All Status", "All Types", "");

                // If no data, show friendly placeholder
                if (DataGetter.Data.AllLoans == null || DataGetter.Data.AllLoans.Count == 0)
                {
                    var placeholder = new Label
                    {
                        Text = "No pending applications.",
                        AutoSize = true,
                        Location = new Point(20, 40),
                        ForeColor = ColorTranslator.FromHtml("#6B7280")
                    };
                    sectionPending.Controls.Add(placeholder);
                    return;
                }

                // Populate grid using real data and compute priority using ApplicationPriority
                foreach (var loan in DataGetter.Data.AllLoans)
                {
                    // Determine days waiting from the Time string if possible
                    string daysStr = "-";
                    int days = 0;
                    if (!string.IsNullOrWhiteSpace(loan.Time))
                    {
                        if (DateTime.TryParse(loan.Time, out var dt))
                        {
                            days = (DateTime.Now.Date - dt.Date).Days;
                            daysStr = days <= 0 ? "0 days" : $"{days} days";
                        }
                    }

                    // Amount formatting (fall back to LoanAmount if Amount is zero)
                    decimal amt = loan.Amount;
                    if (amt == 0 && loan.LoanAmount != 0) amt = loan.LoanAmount;
                    string amountStr = amt > 0 ? $"₱{amt:N0}" : "₱0";

                    // Compute priority using the shared calculator (tune inputs later)
                    double score = LendingApp.Class.LogicClass.LoanOfficer.ApplicationPriority
                        .ComputePriorityScore(days, amt, loan.Type ?? loan.LoanRef ?? "");
                    string priority = LendingApp.Class.LogicClass.LoanOfficer.ApplicationPriority.GetPriorityLabel(score);

                    grid.Rows.Add(loan.Borrower, loan.Type ?? loan.LoanRef, amountStr, daysStr, priority, "Review");
                }
            }
            catch
            {
                // Avoid breaking UI on DB errors; show a placeholder and allow retry via Applications view.
                var placeholder = new Label
                {
                    Text = "Unable to load pending applications. Open Applications view to retry.",
                    AutoSize = true,
                    Location = new Point(20, 40),
                    ForeColor = ColorTranslator.FromHtml("#6B7280")
                };
                sectionPending.Controls.Add(placeholder);
            }
        }

        private void ShowApplicationReview(string customer, string loanType, string amount)
        {
            summaryPanel.Visible = false;

            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            // Hide/remove other embedded views
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            if (_collectionsForm != null && !_collectionsForm.IsDisposed)
            {
                _collectionsForm.Hide();
                contentPanel.Controls.Remove(_collectionsForm);
            }
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }
            if (_settingsForm != null && !_settingsForm.IsDisposed)
            {
                _settingsForm.Hide();
                contentPanel.Controls.Remove(_settingsForm);
            }

            // Create NEW review control every time
            _reviewControl = new OfficerApplicationReviewControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            contentPanel.Controls.Add(_reviewControl);

            // Wire the back button immediately after creating the control
            WireReviewBackButton();

            contentPanel.ResumeLayout();
        }

        // Handle back button from review
        private void ReviewBackButton_Click(object sender, EventArgs e)
        {
            // First remove the control from content panel
            if (_reviewControl != null && !_reviewControl.IsDisposed)
            {
                _reviewControl.Hide();
                contentPanel.Controls.Remove(_reviewControl);
                _reviewControl.Dispose();
                _reviewControl = null;
            }

            // Then show applications view
            ShowApplicationsView();
        }

        private void BuildOverdueLoansSection()
        {
            sectionOverdue.Controls.Clear();

            var header = new Label
            {
                Text = "OVERDUE LOANS (NEEDS FOLLOW-UP)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#FEE2E2"),
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            grid.Columns.Add("Customer", "Customer");
            grid.Columns.Add("AmountDue", "Amount Due");
            grid.Columns.Add("DaysOverdue", "Days Over");
            grid.Columns.Add("Contact", "Contact");
            grid.Columns.Add("Priority", "Priority");

            var actionsCol = new DataGridViewButtonColumn
            {
                HeaderText = "Actions",
                Text = "Follow Up",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(actionsCol);

            sectionOverdue.Controls.Add(grid);
            sectionOverdue.Controls.Add(header);

            // Always load from DB first; if it fails, fall back to sample dashboard.AllOverdueLoans.
            _dbOverdueLoans.Clear();
            var hasDb = TryLoadOverdueLoansFromDb(_dbOverdueLoans);

            var rows = hasDb
                ? _dbOverdueLoans
                : dashboard.AllOverdueLoans.Select(x => new OverdueLoanData
                {
                    Id = x.Id,
                    Customer = x.Customer,
                    CustomerId = "",
                    LoanType = "",
                    OriginalAmount = "",
                    Term = "",
                    DueDate = "",
                    DaysOverdue = x.DaysOverdue,
                    AmountDue = x.AmountDue,
                    Penalty = "",
                    TotalDue = "",
                    OutstandingBalance = "",
                    Contact = x.Contact
                }).ToList();

            // Update the Overdue card count to reflect DB rows when possible.
            try
            {
                lblOverdueCount.Text = $"[{rows.Count}]";
            }
            catch { /* keep UI resilient */ }

            foreach (var r in rows)
            {
                grid.Rows.Add(
                    r.Customer ?? "",
                    r.AmountDue ?? "",
                    $"{r.DaysOverdue}",
                    r.Contact ?? "",
                    GetPriorityLabelFromDays(r.DaysOverdue));
            }

            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == actionsCol.Index && e.RowIndex >= 0)
                {
                    // Prefer DB-backed object if available, else fallback from grid.
                    OverdueLoanData selected = null;

                    if (hasDb && e.RowIndex < _dbOverdueLoans.Count)
                    {
                        selected = _dbOverdueLoans[e.RowIndex];
                    }
                    else
                    {
                        var row = grid.Rows[e.RowIndex];
                        string customer = row.Cells["Customer"].Value?.ToString() ?? "";
                        string amountDue = row.Cells["AmountDue"].Value?.ToString() ?? "";
                        int daysOverdue = 0;
                        int.TryParse(row.Cells["DaysOverdue"].Value?.ToString() ?? "0", out daysOverdue);

                        selected = new OverdueLoanData
                        {
                            Id = "",
                            Customer = customer,
                            CustomerId = "",
                            LoanType = "",
                            OriginalAmount = "",
                            Term = "",
                            DueDate = "",
                            DaysOverdue = daysOverdue,
                            AmountDue = amountDue,
                            Penalty = "",
                            TotalDue = "",
                            OutstandingBalance = "",
                            Contact = row.Cells["Contact"].Value?.ToString() ?? ""
                        };
                    }

                    ShowCollectionFollowUp(selected);
                }
            };
        }

        /// <summary>
        /// Loads overdue loans from DB using the `loans` table (and joins customers + loan_products).
        /// Overdue condition: Status == 'Active' AND (DaysOverdue > 0 OR NextDueDate < Today).
        /// Uses fields that exist in lendingapp.sql: loans.days_overdue, loans.next_due_date, customers.mobile_number, etc.
        /// </summary>
        private bool TryLoadOverdueLoansFromDb(List<OverdueLoanData> target)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var today = DateTime.Today;

                    // Query minimal columns; avoid DbFunctions/TruncateTime calls to keep translation simple.
                    var query =
                        from l in db.Loans.AsNoTracking()
                        join c in db.Customers.AsNoTracking() on l.CustomerId equals c.CustomerId into cj
                        from c in cj.DefaultIfEmpty()
                        join p in db.LoanProducts.AsNoTracking() on l.ProductId equals p.ProductId into pj
                        from p in pj.DefaultIfEmpty()
                        where (l.Status ?? "") == "Active"
                              && (
                                  l.DaysOverdue > 0
                                  // next_due_date on or before today
                                  || (l.NextDueDate != null && l.NextDueDate <= today)
                                  // or outstanding balance exists and next due is null or due already
                                  || (l.OutstandingBalance > 0 && (l.NextDueDate == null || l.NextDueDate <= today))
                                 )
                        orderby l.DaysOverdue descending, l.NextDueDate
                        select new
                        {
                            l.LoanNumber,
                            l.CustomerId,
                            CustomerFirst = c != null ? c.FirstName : null,
                            CustomerLast = c != null ? c.LastName : null,
                            Contact = c != null ? c.MobileNumber : null,
                            ProductName = p != null ? p.ProductName : null,
                            l.PrincipalAmount,
                            l.TermMonths,
                            l.NextDueDate,
                            l.DaysOverdue,
                            l.MonthlyPayment,
                            l.OutstandingBalance
                        };

                    var data = query.Take(100).ToList();

                    // DB reachable — clear and populate target (may be zero rows)
                    target.Clear();

                    foreach (var x in data)
                    {
                        int days = x.DaysOverdue;
                        if (days <= 0 && x.NextDueDate.HasValue && x.NextDueDate.Value.Date < today)
                            days = (today - x.NextDueDate.Value.Date).Days;

                        var dueDateText = x.NextDueDate.HasValue
                            ? x.NextDueDate.Value.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)
                            : string.Empty;

                        var amountDue = x.MonthlyPayment > 0 ? $"₱{x.MonthlyPayment:N2}" : $"₱{x.OutstandingBalance:N2}";

                        var customerName = (x.CustomerFirst ?? "").Trim();
                        if (!string.IsNullOrWhiteSpace(x.CustomerLast))
                        {
                            customerName = (customerName + " " + x.CustomerLast).Trim();
                        }
                        if (string.IsNullOrWhiteSpace(customerName))
                            customerName = x.CustomerId ?? "";

                        target.Add(new OverdueLoanData
                        {
                            Id = x.LoanNumber ?? "",
                            Customer = customerName,
                            CustomerId = x.CustomerId ?? "",
                            LoanType = x.ProductName ?? "",
                            OriginalAmount = x.PrincipalAmount > 0 ? $"₱{x.PrincipalAmount:N0}" : "",
                            Term = x.TermMonths > 0 ? $"{x.TermMonths} months" : "",
                            DueDate = dueDateText,
                            DaysOverdue = days,
                            AmountDue = amountDue,
                            Penalty = "",
                            TotalDue = "",
                            OutstandingBalance = $"₱{x.OutstandingBalance:N2}",
                            Contact = x.Contact ?? ""
                        });
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // show inner exception where available for precise diagnostics
                var msg = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Failed to load overdue loans from DB:\n" + msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private static string GetPriorityLabelFromDays(int daysOverdue)
        {
            if (daysOverdue >= 7) return "Critical";
            if (daysOverdue >= 4) return "High";
            if (daysOverdue >= 1) return "Medium";
            return "Low";
        }

        private void BuildTasksSection()
        {
            sectionTasks.Controls.Clear();
            var header = new Label
            {
                Text = "TODAY'S TASKS",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            grid.Columns.Add("Time", "Time");
            grid.Columns.Add("Customer", "Customer");
            grid.Columns.Add("TaskType", "Task Type");
            grid.Columns.Add("Status", "Status");

            sectionTasks.Controls.Add(grid);
            sectionTasks.Controls.Add(header);

            // Try load today's tasks from DB via service
            var todayTasks = new List<LendingApp.Class.Services.TodayTaskData>();
            var hasDb = LendingApp.Class.Services.OfficerDashboardService.TryGetTodayTasks(todayTasks, 200);

            if (hasDb)
            {
                if (todayTasks.Count == 0)
                {
                    var placeholder = new Label
                    {
                        Text = "No tasks scheduled for today.",
                        AutoSize = true,
                        Location = new Point(20, 40),
                        ForeColor = ColorTranslator.FromHtml("#6B7280")
                    };
                    sectionTasks.Controls.Add(placeholder);
                    return;
                }

                foreach (var t in todayTasks)
                {
                    grid.Rows.Add(t.TimeDisplay, t.CustomerName, t.TaskType, t.Status);
                }
            }
            else
            {
                // fallback to in-memory sample data (existing behavior)
                foreach (var task in dashboard.AllTodayTasks)
                {
                    grid.Rows.Add(task.Time, task.Customer, task.TaskType, task.Status);
                }
            }
        }

        private void BuildActivitySection()
        {
            sectionActivity.Controls.Clear();
            var header = new Label
            {
                Text = "RECENT ACTIVITY",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                BackColor = ColorTranslator.FromHtml("#ECFDF5"),
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 8, 0, 0)
            };
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            grid.Columns.Add("Time", "Time");
            grid.Columns.Add("Activity", "Activity");
            grid.Columns.Add("Customer", "Customer");
            grid.Columns.Add("Amount", "Amount");

            sectionActivity.Controls.Add(grid);
            sectionActivity.Controls.Add(header);

            foreach (var act in dashboard.AllRecentActivity)
            {
                grid.Rows.Add(act.Time, act.Activity, act.Customer, act.Amount);
            }
        }

        private void PopulateData()
        {
            SetUsername(_username);
        }

        private void WireReviewBackButton()
        {
            if (_reviewControl != null)
            {
                // Remove any existing handlers
                if (_reviewControl.BackButton != null)
                {
                    _reviewControl.BackButton.Click -= ReviewBackButton_Click;
                    _reviewControl.BackButton.Click += ReviewBackButton_Click;
                }
                else
                {
                    // If BackButton is null, wait for it to be initialized
                    // We'll check again after a short delay
                    var timer = new Timer { Interval = 100 };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        timer.Dispose();

                        if (_reviewControl.BackButton != null)
                        {
                            _reviewControl.BackButton.Click -= ReviewBackButton_Click;
                            _reviewControl.BackButton.Click += ReviewBackButton_Click;
                        }
                    };
                    timer.Start();
                }
            }
        }
    }
}