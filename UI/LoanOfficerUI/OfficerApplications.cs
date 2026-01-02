using System;
using System.Drawing;
using System.Windows.Forms;
using LendingApp.Models.LoanOfficer;
using LendingApp.Services;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerApplications : Form
    {
        private OfficerApplicationLogic logic;

        public OfficerApplications()
        {
            InitializeComponent();
            logic = new OfficerApplicationLogic(DataGetter.Data);

            updateStatusSummary();
            BuildUI();
            BindFilters();
            LoadApplications();
        }

        private void updateStatusSummary()
        {
            lblTotal.Text = logic.TotalApplications.ToString();
            lblPending.Text = logic.GetStatusSummary().Find(s => s.Applied == "Pending")?.Count.ToString() ?? "0";
            lblReview.Text = logic.GetStatusSummary().Find(s => s.Applied == "Review")?.Count.ToString() ?? "0";
            lblApproved.Text = logic.GetStatusSummary().Find(s => s.Applied == "Approved")?.Count.ToString() ?? "0";
            lblDisbursed.Text = logic.GetStatusSummary().Find(s => s.Applied == "Disbursed")?.Count.ToString() ?? "0";
        }

        private void BuildUI()
        {
            Text = "Officer Applications";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Headers
            lblHeaderTitle.Text = "Loan Applications";  
            lblHeaderTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            lblHeaderSubtitle.Text = "Manage and review customer loan applications";
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9);

            btnNewApplication.Text = "New Application";
            btnNewApplication.BackColor = ColorTranslator.FromHtml("#3498DB");
            btnNewApplication.ForeColor = Color.White;
            btnNewApplication.FlatStyle = FlatStyle.Flat;
            btnNewApplication.Click += (s, e) =>
                MessageBox.Show("New Application clicked");

            // Filters
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[]
            {
                "All Status",
                "Pending",
                "Review",
                "Released",
                "Rejected",
                "Disbursed"
            });

            cmbType.Items.Clear();
            cmbType.Items.AddRange(new object[]
            {
                "All Types",
                "Personal",
                "Emergency",
                "Salary"
            });

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
            gridApplications.Columns.Add("Type", "Type");
            gridApplications.Columns.Add("Amount", "Amount");
            gridApplications.Columns.Add("Applied", "Applied");

            var actionCol = new DataGridViewButtonColumn
            {
                HeaderText = "Action",
                Text = "View",
                UseColumnTextForButtonValue = false
            };
            gridApplications.Columns.Add(actionCol);

            gridApplications.CellContentClick += GridApplications_CellContentClick;

            lblResults.Text = "";
        }

        private void BindFilters()
        {
            cmbStatus.SelectedIndex = 0;
            cmbType.SelectedIndex = 0;

            cmbStatus.SelectedIndexChanged += (s, e) => LoadApplications();
            cmbType.SelectedIndexChanged += (s, e) => LoadApplications();
            txtSearch.TextChanged += (s, e) => LoadApplications();
        }

        private void LoadApplications()
        {
            gridApplications.Rows.Clear();

            string searchText =
                txtSearch.Text == "Search by customer name or application ID..."
                ? ""
                : txtSearch.Text;

            var data = logic.GetApplications(
                cmbStatus.SelectedItem?.ToString(),
                cmbType.SelectedItem?.ToString(),
                searchText
            );

            foreach (var app in data)
            {

                if (app.Applied == "Paid") continue;

                int rowIndex = gridApplications.Rows.Add(
                    app.LoanNumber,
                    app.Borrower,
                    app.Type,
                    app.Amount,
                    app.Applied
                );

                var btnCell =
                    (DataGridViewButtonCell)gridApplications.Rows[rowIndex].Cells[5];
                btnCell.Value = GetActionText(app.Applied);

                var statusCell = gridApplications.Rows[rowIndex].Cells[5];
                statusCell.Style.BackColor = GetStatusBackColor(app.Applied);
                statusCell.Style.ForeColor = GetStatusForeColor(app.Applied);
            }

            lblResults.Text = $"{data.Count} of {logic.TotalApplications} applications";
        }

        private string GetActionText(string status)
        {
            switch (status)
            {
                case "Pending": return "Review";
                case "Review": return "Evaluate";
                case "Approved":
                case "Rejected":
                case "Disbursed":
                default:
                    return "View";
            }
        }

        private Color GetStatusBackColor(string status)
        {
            switch (status)
            {
                case "Pending": return ColorTranslator.FromHtml("#FEF3C7");
                case "Review": return ColorTranslator.FromHtml("#DBEAFE");
                case "Approved": return ColorTranslator.FromHtml("#D1FAE5");
                case "Rejected": return ColorTranslator.FromHtml("#FECACA");
                case "Disbursed": return ColorTranslator.FromHtml("#EDE9FE");
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

            if (gridApplications.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string appId = gridApplications.Rows[e.RowIndex].Cells[0].Value?.ToString();
                string action = gridApplications.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                MessageBox.Show(
                    $"{action} {appId}",
                    "Application Action",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void lblTotalTitle_Click(object sender, EventArgs e)
        {

        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }
    }
}
