using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerApplications : Form
    {
        private string statusFilter = "all";
        private string typeFilter = "all";
        private string searchQuery = "";

        private class ApplicationItem
        {
            public string Id { get; set; }
            public string Customer { get; set; }
            public string LoanType { get; set; }
            public string Amount { get; set; }
            public string AppliedDate { get; set; }
            public string Status { get; set; } // Pending | Review | Approved | Rejected | Disbursed
            public string Priority { get; set; } // High | Medium | Low (optional)
        }

        private readonly List<ApplicationItem> applications = new List<ApplicationItem>
        {
            new ApplicationItem { Id="APP-001", Customer="Juan Dela Cruz", LoanType="Personal", Amount="₱50,000", AppliedDate="Dec 10", Status="Pending",  Priority="High" },
            new ApplicationItem { Id="APP-002", Customer="Maria Santos",  LoanType="Emergency", Amount="₱15,000", AppliedDate="Dec 11", Status="Review",  Priority="Medium" },
            new ApplicationItem { Id="APP-003", Customer="Pedro Reyes",   LoanType="Salary",    Amount="₱25,000", AppliedDate="Dec 12", Status="Approved" },
            new ApplicationItem { Id="APP-004", Customer="Ana Lopez",     LoanType="Personal",  Amount="₱75,000", AppliedDate="Dec 09", Status="Review",  Priority="High" },
            new ApplicationItem { Id="APP-005", Customer="Carlos Tan",    LoanType="Emergency", Amount="₱10,000", AppliedDate="Dec 13", Status="Approved" },
            new ApplicationItem { Id="APP-006", Customer="Sofia Garcia",  LoanType="Salary",    Amount="₱30,000", AppliedDate="Dec 08", Status="Rejected" },
            new ApplicationItem { Id="APP-007", Customer="Miguel Ramos",  LoanType="Personal",  Amount="₱60,000", AppliedDate="Dec 07", Status="Disbursed" },
            new ApplicationItem { Id="APP-008", Customer="Isabel Cruz",   LoanType="Emergency", Amount="₱8,000",  AppliedDate="Dec 13", Status="Pending",  Priority="Low" },
        };

        public OfficerApplications()
        {
            InitializeComponent();
            BuildUI();
            BindFilters();
            RefreshTable();
            UpdateStatsCards();
        }

        private void BuildUI()
        {
            Text = "Officer Applications";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Header
            lblHeaderTitle.Text = "Loan Applications";
            lblHeaderTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHeaderSubtitle.Text = "Manage and review customer loan applications";
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9);
            btnNewApplication.Text = "New Application";
            btnNewApplication.BackColor = ColorTranslator.FromHtml("#3498DB");
            btnNewApplication.ForeColor = Color.White;
            btnNewApplication.FlatStyle = FlatStyle.Flat;
            btnNewApplication.Click += (s, e) => MessageBox.Show("New Application clicked");

            // Filters
            cmbStatus.Items.AddRange(new object[] { "All Status", "Pending", "Review", "Approved", "Rejected", "Disbursed" });
            cmbType.Items.AddRange(new object[] { "All Types", "Personal", "Emergency", "Salary" });
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Search by customer name or application ID...";
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by customer name or application ID...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by customer name or application ID...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray) return;
                searchQuery = txtSearch.Text ?? "";
                RefreshTable();
            };
            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbStatus.SelectedItem?.ToString() ?? "All Status";
                statusFilter = val.Equals("All Status", StringComparison.OrdinalIgnoreCase) ? "all" : val;
                RefreshTable();
            };
            cmbType.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbType.SelectedItem?.ToString() ?? "All Types";
                typeFilter = val.Equals("All Types", StringComparison.OrdinalIgnoreCase) ? "all" : val;
                RefreshTable();
            };

            // Table
            gridApplications.ReadOnly = true;
            gridApplications.AllowUserToAddRows = false;
            gridApplications.AllowUserToDeleteRows = false;
            gridApplications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridApplications.RowHeadersVisible = false;
            gridApplications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridApplications.Columns.Clear();
            gridApplications.Columns.Add("AppId", "App ID");
            gridApplications.Columns.Add("Customer", "Customer");
            gridApplications.Columns.Add("LoanType", "Loan Type");
            gridApplications.Columns.Add("Amount", "Amount");
            gridApplications.Columns.Add("Applied", "Applied");
            gridApplications.Columns.Add("Status", "Status");
            var actionCol = new DataGridViewButtonColumn
            {
                HeaderText = "Action",
                Text = "View",
                UseColumnTextForButtonValue = false // we will set per-row
            };
            gridApplications.Columns.Add(actionCol);
            gridApplications.CellContentClick += GridApplications_CellContentClick;

            // Results info
            lblResults.Text = "";

            // Stats cards are simple labels; values set in UpdateStatsCards.
        }

        private void BindFilters()
        {
            cmbStatus.SelectedIndex = 0; // All Status
            cmbType.SelectedIndex = 0;   // All Types
        }

        private IEnumerable<ApplicationItem> Filtered()
        {
            return applications.Where(app =>
            {
                bool matchesStatus = statusFilter == "all" || app.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesType = typeFilter == "all" || app.LoanType.Equals(typeFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesSearch = string.IsNullOrWhiteSpace(searchQuery)
                    || (app.Customer?.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                    || (app.Id?.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;
                return matchesStatus && matchesType && matchesSearch;
            });
        }

        private void RefreshTable()
        {
            var filtered = Filtered().ToList();

            gridApplications.Rows.Clear();
            foreach (var app in filtered)
            {
                int rowIndex = gridApplications.Rows.Add(app.Id, ToIdWithPriority(app), app.LoanType, app.Amount, app.AppliedDate, app.Status, "");
                var row = gridApplications.Rows[rowIndex];

                // Style status cell similar to pill
                var statusCell = row.Cells["Status"] as DataGridViewTextBoxCell;
                if (statusCell != null)
                {
                    statusCell.Style.BackColor = GetStatusBackColor(app.Status);
                    statusCell.Style.ForeColor = GetStatusForeColor(app.Status);
                }

                // Action button text
                var btnCell = row.Cells[gridApplications.Columns.Count - 1] as DataGridViewButtonCell;
                if (btnCell != null)
                {
                    btnCell.Value = GetActionText(app.Status);
                }
            }

            lblResults.Text = $"Showing {filtered.Count} of {applications.Count} applications";
            UpdateStatsCards();
        }

        private object ToIdWithPriority(ApplicationItem app)
        {
            // Combine ID and priority tag visually in the same cell
            return app.Id + (string.IsNullOrEmpty(app.Priority) ? "" : $"  [{app.Priority}]");
        }

        private string GetActionText(string status)
        {
            switch (status)
            {
                case "Pending": return "Review";
                case "Review": return "Evaluate";
                case "Approved":
                case "Rejected":
                case "Disbursed": return "View";
                default: return "View";
            }
        }

        private Color GetStatusBackColor(string status)
        {
            switch (status)
            {
                case "Pending": return ColorTranslator.FromHtml("#FEF3C7"); // yellow-100
                case "Review": return ColorTranslator.FromHtml("#DBEAFE");   // blue-100
                case "Approved": return ColorTranslator.FromHtml("#D1FAE5"); // green-100
                case "Rejected": return ColorTranslator.FromHtml("#FECACA"); // red-200
                case "Disbursed": return ColorTranslator.FromHtml("#EDE9FE"); // purple-100
                default: return ColorTranslator.FromHtml("#F3F4F6");
            }
        }

        private Color GetStatusForeColor(string status)
        {
            switch (status)
            {
                case "Pending": return ColorTranslator.FromHtml("#92400E");
                case "Review": return ColorTranslator.FromHtml("#1D4ED8");
                case "Approved": return ColorTranslator.FromHtml("#065F46");
                case "Rejected": return ColorTranslator.FromHtml("#7F1D1D");
                case "Disbursed": return ColorTranslator.FromHtml("#5B21B6");
                default: return ColorTranslator.FromHtml("#374151");
            }
        }

        private void GridApplications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            // Action column
            if (gridApplications.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var appId = gridApplications.Rows[e.RowIndex].Cells[0].Value?.ToString();
                var action = gridApplications.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                MessageBox.Show($"{action} {appId}", "Application Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateStatsCards()
        {
            lblTotal.Text = applications.Count.ToString();
            lblPending.Text = applications.Count(a => a.Status == "Pending").ToString();
            lblReview.Text = applications.Count(a => a.Status == "Review").ToString();
            lblApproved.Text = applications.Count(a => a.Status == "Approved").ToString();
            lblDisbursed.Text = applications.Count(a => a.Status == "Disbursed").ToString();
        }
    }
}
