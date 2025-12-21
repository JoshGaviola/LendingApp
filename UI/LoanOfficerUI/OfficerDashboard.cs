using LendingApp.Models.LoanOfficer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerDashboard : Form
    {

        OfficerDashboardLogic dashboard = new OfficerDashboardLogic();



        private string _username = "Officer";
        private Action _onLogout;

        private string activeNav = "Dashboard";
        private readonly List<string> navItems = new List<string>
        {
            "Dashboard", "Applications", "Customers", "Collections", "Calendar", "Settings"
        };

        // Embedded views
        private OfficerApplications _applicationsForm;
        private OfficerCollections _collectionsForm;
        private OfficerCustomers _customersForm; // Added for customers view
        private OfficerCalendar _calendarForm; // Added for calendar view
        private OfficerSettings _settingsForm; // Added for settings view
        private bool _homeResizeHooked;


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
                    else if (item == "Calendar")
                    {
                        ShowCalendarView();
                    }
                    else if (item == "Settings")
                    {
                        ShowSettingsView();
                    }
                    else if (item == "Dashboard") // FIX: handle dashboard explicitly
                    {
                        ShowDashboardHome();
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
            lblPendingCount.Text = $"[{dashboard.TotalPendingApplications}]";
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
            if (_customersForm != null && !_customersForm.IsDisposed)   // <— add this
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
            if (_customersForm != null && !_customersForm.IsDisposed)   // <— add this
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

        private void ShowCalendarView()
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

            if (_calendarForm == null || _calendarForm.IsDisposed)
            {
                _calendarForm = new OfficerCalendar
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_calendarForm);
            _calendarForm.Show();
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
            if (_calendarForm != null && !_calendarForm.IsDisposed)
            {
                _calendarForm.Hide();
                contentPanel.Controls.Remove(_calendarForm);
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
            // Hide/remove embedded views
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
            if (_calendarForm != null && !_calendarForm.IsDisposed) // ensure calendar removed
            {
                _calendarForm.Hide();
                contentPanel.Controls.Remove(_calendarForm);
            }
            if (_settingsForm != null && !_settingsForm.IsDisposed) // ensure settings removed
            {
                _settingsForm.Hide();
                contentPanel.Controls.Remove(_settingsForm);
            }

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

            foreach (var app in dashboard.AllPending)
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

            foreach (var loan in dashboard.AllOverdueLoans)
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

            foreach (var task in dashboard.AllTodayTasks)
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

            foreach (var act in dashboard.AllRecentActivity)
            {
                grid.Rows.Add(act.Time, act.Activity, act.Customer, act.Amount);
            }
        }

        private void PopulateData()
        {
            SetUsername(_username);
        }
    }
}
