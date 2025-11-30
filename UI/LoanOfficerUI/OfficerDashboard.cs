using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerDashboard : Form
    {
        private string _username = "Officer";
        private Action _onLogout;

        private string activeNav = "Dashboard";
        private readonly List<string> navItems = new List<string>
        {
            "Dashboard", "Applications", "Customers", "Collections", "Calendar", "Settings"
        };

        // Summary stats
        private int pendingApplicationsCount = 12;
        private string activePortfolio = "₱85,000";
        private int overdueLoansCount = 3;
        private string todayCollections = "₱15,700";

        // Keep a single instance of the Applications view to reuse
        private OfficerApplications _applicationsForm;
        private bool _homeResizeHooked;

        // Add a field to reuse the customers form instance
        private OfficerCustomers _customersForm;

        // Data models
        private class PendingApplication
        {
            public string Id { get; set; }
            public string Customer { get; set; }
            public string LoanType { get; set; }
            public string Amount { get; set; }
            public int DaysWaiting { get; set; }
            public string Priority { get; set; } // High | Medium | Low
        }

        private class OverdueLoan
        {
            public string Id { get; set; }
            public string Customer { get; set; }
            public string AmountDue { get; set; }
            public int DaysOverdue { get; set; }
            public string Contact { get; set; }
            public string Priority { get; set; } // Critical | High | Medium
        }

        private class TaskItem
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string Customer { get; set; }
            public string TaskType { get; set; }
            public string LoanId { get; set; }
            public string Status { get; set; } // Due | Pending | Completed
        }

        private class ActivityItem
        {
            public string Id { get; set; }
            public string Time { get; set; }
            public string Activity { get; set; }
            public string Customer { get; set; }
            public string Amount { get; set; }
        }

        private readonly List<PendingApplication> pendingApplications = new List<PendingApplication>
        {
            new PendingApplication { Id="APP-001", Customer="Juan Cruz", LoanType="Personal", Amount="₱50,000", DaysWaiting=2, Priority="High" },
            new PendingApplication { Id="APP-002", Customer="Maria Santos", LoanType="Emergency", Amount="₱15,000", DaysWaiting=1, Priority="Medium" },
            new PendingApplication { Id="APP-003", Customer="Pedro Reyes", LoanType="Salary", Amount="₱25,000", DaysWaiting=3, Priority="High" },
        };

        private readonly List<OverdueLoan> overdueLoans = new List<OverdueLoan>
        {
            new OverdueLoan { Id="LN-001", Customer="Pedro Reyes", AmountDue="₱4,442", DaysOverdue=5, Contact="+639123456789", Priority="Critical" },
            new OverdueLoan { Id="LN-002", Customer="Ana Lopez", AmountDue="₱3,250", DaysOverdue=3, Contact="+639987654321", Priority="High" },
            new OverdueLoan { Id="LN-003", Customer="Carlos Tan", AmountDue="₱2,100", DaysOverdue=2, Contact="+639456789012", Priority="Medium" },
        };

        private readonly List<TaskItem> todayTasks = new List<TaskItem>
        {
            new TaskItem { Id="T-001", Time="9:00 AM", Customer="Juan Cruz", TaskType="Payment Follow-up", LoanId="PLN-001", Status="Due" },
            new TaskItem { Id="T-002", Time="2:00 PM", Customer="Maria Santos", TaskType="Doc Review", LoanId="ELN-002", Status="Pending" },
            new TaskItem { Id="T-003", Time="4:00 PM", Customer="Pedro Reyes", TaskType="Credit Assessment", LoanId="SLN-003", Status="Pending" },
        };

        private readonly List<ActivityItem> recentActivity = new List<ActivityItem>
        {
            new ActivityItem { Id="A-001", Time="08:30 AM", Activity="Payment Received", Customer="Ana Lopez", Amount="₱5,250" },
            new ActivityItem { Id="A-002", Time="Yesterday", Activity="App Approved", Customer="Juan Cruz", Amount="₱50,000" },
            new ActivityItem { Id="A-003", Time="Yesterday", Activity="Payment Received", Customer="Carlos Tan", Amount="₱3,100" },
            new ActivityItem { Id="A-004", Time="2 days ago", Activity="Loan Disbursed", Customer="Maria Santos", Amount="₱15,000" },
        };

        public OfficerDashboard()
        {
            InitializeComponent();
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
                    // Navigation handling
                    if (item == "Applications")
                    {
                        ShowApplicationsView();
                    }
                    else if (item == "Customers")
                    {
                        ShowCustomersView();
                    }
                    else if (item == "Dashboard")
                    {
                        ShowDashboardHome();
                    }
                    else
                    {
                        // keep sidebar highlight only
                        contentPanel.Focus();
                    }
                    // Refresh highlight
                    BuildSidebar();
                };
                sidebarPanel.Controls.Add(btn);
                y += 42;
            }
        }

        private void LayoutHeader()
        {
            // Simple horizontal layout
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

            // Card 1
            cardPending.BorderStyle = BorderStyle.FixedSingle;
            cardPending.BackColor = ColorTranslator.FromHtml("#EFF6FF"); // blue-50
            cardPending.Location = new Point(startX, startY);
            cardPending.Size = new Size(cardWidth, 60);

            lblPendingTitle.Text = "Pending";
            lblPendingTitle.ForeColor = ColorTranslator.FromHtml("#1D4ED8");
            lblPendingTitle.Location = new Point(10, 8);
            lblPendingCount.Text = $"[{pendingApplicationsCount}]";
            lblPendingCount.ForeColor = ColorTranslator.FromHtml("#1E3A8A");
            lblPendingCount.Location = new Point(10, 28);
            lblPendingSub.Text = "Applications";
            lblPendingSub.ForeColor = ColorTranslator.FromHtml("#2563EB");
            lblPendingSub.Location = new Point(90, 28);

            cardPending.Controls.Add(lblPendingTitle);
            cardPending.Controls.Add(lblPendingCount);
            cardPending.Controls.Add(lblPendingSub);

            // Card 2
            cardActive.BorderStyle = BorderStyle.FixedSingle;
            cardActive.BackColor = ColorTranslator.FromHtml("#ECFDF5"); // green-50
            cardActive.Location = new Point(startX + (cardWidth + gap), startY);
            cardActive.Size = new Size(cardWidth, 60);
            lblActiveTitle.Text = "Active";
            lblActiveTitle.ForeColor = ColorTranslator.FromHtml("#047857");
            lblActiveTitle.Location = new Point(10, 8);
            lblActiveValue.Text = activePortfolio;
            lblActiveValue.ForeColor = ColorTranslator.FromHtml("#065F46");
            lblActiveValue.Location = new Point(10, 28);
            lblActiveSub.Text = "Portfolio";
            lblActiveSub.ForeColor = ColorTranslator.FromHtml("#10B981");
            lblActiveSub.Location = new Point(120, 28);
            cardActive.Controls.Add(lblActiveTitle);
            cardActive.Controls.Add(lblActiveValue);
            cardActive.Controls.Add(lblActiveSub);

            // Card 3
            cardOverdue.BorderStyle = BorderStyle.FixedSingle;
            cardOverdue.BackColor = ColorTranslator.FromHtml("#FEE2E2"); // red-100
            cardOverdue.Location = new Point(startX + 2 * (cardWidth + gap), startY);
            cardOverdue.Size = new Size(cardWidth, 60);
            lblOverdueTitle.Text = "Overdue";
            lblOverdueTitle.ForeColor = ColorTranslator.FromHtml("#DC2626");
            lblOverdueTitle.Location = new Point(10, 8);
            lblOverdueCount.Text = $"[{overdueLoansCount}]";
            lblOverdueCount.ForeColor = ColorTranslator.FromHtml("#991B1B");
            lblOverdueCount.Location = new Point(10, 28);
            lblOverdueSub.Text = "Loans";
            lblOverdueSub.ForeColor = ColorTranslator.FromHtml("#EF4444");
            lblOverdueSub.Location = new Point(90, 28);
            cardOverdue.Controls.Add(lblOverdueTitle);
            cardOverdue.Controls.Add(lblOverdueCount);
            cardOverdue.Controls.Add(lblOverdueSub);

            // Card 4
            cardCollections.BorderStyle = BorderStyle.FixedSingle;
            cardCollections.BackColor = ColorTranslator.FromHtml("#FFEDD5"); // orange-100
            cardCollections.Location = new Point(startX + 3 * (cardWidth + gap), startY);
            cardCollections.Size = new Size(cardWidth, 60);
            lblCollectionsTitle.Text = "Today";
            lblCollectionsTitle.ForeColor = ColorTranslator.FromHtml("#EA580C");
            lblCollectionsTitle.Location = new Point(10, 8);
            lblCollectionsValue.Text = todayCollections;
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
            // Dock panels
            headerPanel.Dock = DockStyle.Top;
            summaryPanel.Dock = DockStyle.Top;
            sidebarPanel.Dock = DockStyle.Left;
            contentPanel.Dock = DockStyle.Fill;

            // Add to form in order
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(summaryPanel);
            Controls.Add(headerPanel);

            // Place initial home sections
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
            // Hide summary for a cleaner Applications view (optional)
            summaryPanel.Visible = false;

            // Clear current content and host OfficerApplications inside contentPanel
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            if (_applicationsForm == null || _applicationsForm.IsDisposed)
            {
                _applicationsForm = new OfficerApplications();
                _applicationsForm.TopLevel = false;
                _applicationsForm.FormBorderStyle = FormBorderStyle.None;
                _applicationsForm.Dock = DockStyle.Fill;
            }

            contentPanel.Controls.Add(_applicationsForm);
            _applicationsForm.Show();
            contentPanel.ResumeLayout();
        }

        // Add this method to show the customers view
        private void ShowCustomersView()
        {
            // Optional: hide summary for full-page customers view
            summaryPanel.Visible = false;

            // Remove any embedded applications form
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }

            // Clear current content and host OfficerCustomers inside contentPanel
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            if (_customersForm == null || _customersForm.IsDisposed)
            {
                _customersForm = new OfficerCustomers();
                _customersForm.TopLevel = false;
                _customersForm.FormBorderStyle = FormBorderStyle.None;
                _customersForm.Dock = DockStyle.Fill;
            }

            contentPanel.Controls.Add(_customersForm);
            _customersForm.Show();

            contentPanel.ResumeLayout();
        }

        private void ShowDashboardHome()
        {
            // Hide applications view if present
            if (_applicationsForm != null && !_applicationsForm.IsDisposed)
            {
                _applicationsForm.Hide();
                contentPanel.Controls.Remove(_applicationsForm);
            }
            // Remove customers form if present
            if (_customersForm != null && !_customersForm.IsDisposed)
            {
                _customersForm.Hide();
                contentPanel.Controls.Remove(_customersForm);
            }

            // Rebuild and show home sections
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
                RowHeadersVisible = false
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
            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == actionsCol.Index && e.RowIndex >= 0)
                {
                    MessageBox.Show("Open review dialog...", "Review");
                }
            };

            sectionPending.Controls.Add(grid);
            sectionPending.Controls.Add(header);

            foreach (var app in pendingApplications)
            {
                grid.Rows.Add(app.Customer, app.LoanType, app.Amount, app.DaysWaiting, app.Priority);
            }
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
            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == actionsCol.Index && e.RowIndex >= 0)
                {
                    MessageBox.Show("Trigger follow-up workflow...", "Follow Up");
                }
            };

            sectionOverdue.Controls.Add(grid);
            sectionOverdue.Controls.Add(header);

            foreach (var loan in overdueLoans)
            {
                grid.Rows.Add(loan.Customer, loan.AmountDue, loan.DaysOverdue, loan.Contact, loan.Priority);
            }
        }

        private void BuildTasksSection()
        {
            sectionTasks.Controls.Clear();
            var header = new Label
            {
                Text = "TODAY'S TASKS & CALENDAR",
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

            foreach (var task in todayTasks)
            {
                grid.Rows.Add(task.Time, task.Customer, task.TaskType, task.Status);
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

            foreach (var act in recentActivity)
            {
                grid.Rows.Add(act.Time, act.Activity, act.Customer, act.Amount);
            }
        }

        private void PopulateData()
        {
            // Data already loaded in field initializers; SetUsername updates header.
            SetUsername(_username);
        }
    }
}
