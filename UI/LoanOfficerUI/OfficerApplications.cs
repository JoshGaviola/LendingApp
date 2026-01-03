using System;
using System.Drawing;
using System.Windows.Forms;
using LendingApp.Models.LoanOfficer;
using LendingApp.Class.Services;
using LendingApp.UI.LoanOfficerUI.Dialog;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerApplications : Form
    {
        private OfficerApplicationLogic logic;

        public OfficerApplications()
        {
            InitializeComponent();
            logic = new OfficerApplicationLogic(null);

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
            lblDisbursed.Text = logic.GetStatusSummary().Find(s => s.Applied == "Released")?.Count.ToString() ?? "0";
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

            // Filters
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[]
            {
                "All Status",
                "Pending",
                "Review",
                "Approved",
                "Rejected",
                "Released",
                "Cancelled"
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
                Text = "Review",
                UseColumnTextForButtonValue = true   // IMPORTANT
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

                // Set the button caption per row
                var btnCell = (DataGridViewButtonCell)gridApplications.Rows[rowIndex].Cells[5];
                btnCell.Value = GetActionText(app.Applied);
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

        private void GridApplications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(gridApplications.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            string appId = gridApplications.Rows[e.RowIndex].Cells["AppId"].Value?.ToString();
            string customer = gridApplications.Rows[e.RowIndex].Cells["Customer"].Value?.ToString();
            string type = gridApplications.Rows[e.RowIndex].Cells["Type"].Value?.ToString();
            string amount = gridApplications.Rows[e.RowIndex].Cells["Amount"].Value?.ToString();
            string applied = gridApplications.Rows[e.RowIndex].Cells["Applied"].Value?.ToString();

            var action = gridApplications.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

            // Open Review dialog when Review/Evaluate is clicked
            if (string.Equals(action, "Review", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(action, "Evaluate", StringComparison.OrdinalIgnoreCase))
            {
                using (var dlg = new ReviewApplicationDialog(appId, customer, type, amount, applied))
                {
                    dlg.ShowDialog(this);
                }

                return;
            }

            // default behavior
            MessageBox.Show($"{action} {appId}", "Application Action",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            using (var dialog = new NewLoanApplicationDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    updateStatusSummary();
                    LoadApplications();
                }
            }
        }
    }
}
