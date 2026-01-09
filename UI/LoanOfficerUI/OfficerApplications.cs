using System;
using System.Drawing;
using System.Windows.Forms;
using LendingApp.Models.LoanOfficer;
using LendingApp.Class.Services;
using LendingApp.UI.LoanOfficerUI.Dialog;
using LendingApp.Class.Repo;
using LendingApp.Class.Models.Loans;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerApplications : Form
    {
        private OfficerApplicationLogic logic;
        private ApplicantsData Loans;

        public OfficerApplications()
        {
            InitializeComponent();
            logic = new OfficerApplicationLogic(null);
            Loans = new ApplicantsData();

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

            // New: Reject column to allow quick reject from list when applicable
            var rejectCol = new DataGridViewButtonColumn
            {
                Name = "RejectAction",
                HeaderText = "Reject",
                Text = "Reject",
                UseColumnTextForButtonValue = true
            };
            gridApplications.Columns.Add(rejectCol);

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

        private string GetActionText(string status)
        {
            var s = (status ?? string.Empty).Trim();

            // normalize common DB variants
            if (s.Equals("Disbursed", StringComparison.OrdinalIgnoreCase))
                s = "Released";

            if (s.Equals("Pending", StringComparison.OrdinalIgnoreCase)) return "Review";
            if (s.Equals("Review", StringComparison.OrdinalIgnoreCase)) return "Evaluate";

            // Approved/Rejected/Released/Cancelled/etc => View
            if (s.Equals("Approved", StringComparison.OrdinalIgnoreCase)) return "View";
            if (s.Equals("Rejected", StringComparison.OrdinalIgnoreCase)) return "View";
            if (s.Equals("Released", StringComparison.OrdinalIgnoreCase)) return "View";
            if (s.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)) return "View";

            return "View";
        }

        private void LoadApplications()
        {
            gridApplications.Rows.Clear();

            string searchText =
                txtSearch.Text == "Search by customer name or application ID..."
                ? ""
                : txtSearch.Text;

            Loans.LoadLoans(
                cmbStatus.SelectedItem?.ToString(),
                cmbType.SelectedItem?.ToString(),
                searchText
              );

            foreach (var app in Loans.AllLoans)
            {
                if (string.Equals(app.Applied, "Paid", StringComparison.OrdinalIgnoreCase)) continue;

                int rowIndex = gridApplications.Rows.Add(
                    app.LoanNumber,
                    app.Borrower,
                    app.Type,
                    app.Amount,
                    app.Applied
                );

                var btnCell = (DataGridViewButtonCell)gridApplications.Rows[rowIndex].Cells[5];
                btnCell.Value = GetActionText(app.Applied);

                // Set reject button availability: only for Pending or Review
                var rejectCell = gridApplications.Rows[rowIndex].Cells["RejectAction"] as DataGridViewButtonCell;
                if (rejectCell != null)
                {
                    if (string.Equals(app.Applied, "Pending", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(app.Applied, "Review", StringComparison.OrdinalIgnoreCase))
                    {
                        rejectCell.Value = "Reject";
                        // style to indicate destructive action
                        rejectCell.Style.ForeColor = Color.White;
                        rejectCell.Style.BackColor = Color.FromArgb(200, 0, 0);
                    }
                    else
                    {
                        rejectCell.Value = "";
                        rejectCell.Style.ForeColor = Color.Black;
                        rejectCell.Style.BackColor = Color.White;
                    }
                }
            }

            lblResults.Text = $"{Loans.AllLoans.Count} of {logic.TotalApplications} applications";
        }

        private void GridApplications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(gridApplications.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            string appId = gridApplications.Rows[e.RowIndex].Cells["AppId"].Value?.ToString();
            var action = gridApplications.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

            if (string.IsNullOrWhiteSpace(appId)) return;

            // If Reject button clicked (column named "RejectAction")
            if (gridApplications.Columns[e.ColumnIndex].Name == "RejectAction")
            {
                // if no visible value, ignore
                if (string.IsNullOrWhiteSpace(action)) return;

                HandleRejectFromList(appId);
                return;
            }

            // REVIEW -> show review dialog (send for evaluation happens there)
            if (string.Equals(action, "Review", StringComparison.OrdinalIgnoreCase))
            {
                using (var dlg = new ReviewApplicationDialog(appId))
                {
                    dlg.ShowDialog(this);
                }

                updateStatusSummary();
                LoadApplications();
                return;
            }

            // EVALUATE -> open evaluation form directly
            if (string.Equals(action, "Evaluate", StringComparison.OrdinalIgnoreCase))
            {
                // Use the same flow ReviewApplicationDialog uses: load from DB, determine customer type,
                // then apply defaults and show the evaluation form.
                var loanRepo = new LendingApp.Class.Repo.LoanApplicationRepository();
                var customerRepo = new LendingApp.Class.Repo.CustomerRepository();

                var app = loanRepo.GetByApplicationNumber(appId);
                if (app == null)
                {
                    MessageBox.Show("Application not found in database.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var customer = customerRepo.GetById(app.CustomerId);

                string customerName = customer != null
                    ? ((customer.FirstName ?? "") + " " + (customer.LastName ?? "")).Trim()
                    : (app.CustomerId ?? "N/A");

                string loanTypeText;
                switch (app.ProductId)
                {
                    case 1: loanTypeText = "Personal Loan"; break;
                    case 2: loanTypeText = "Emergency Loan"; break;
                    case 3: loanTypeText = "Salary Loan"; break;
                    default: loanTypeText = "Product " + app.ProductId; break;
                }

                string amountText = "₱" + app.RequestedAmount.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                using (var eval = new LoanApplicationUI.OfficerEvaluateApplicationForm())
                {
                    eval.CurrentApplication = new LoanApplicationUI.ApplicationData
                    {
                        Id = app.ApplicationNumber,
                        Customer = customerName,
                        LoanType = loanTypeText,
                        Amount = amountText,
                        AppliedDate = app.ApplicationDate.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US")),
                        Status = app.Status
                    };

                    eval.ApplyDefaultsForCustomerType(customer?.CustomerType);
                    eval.ShowDialog(this);
                }

                // optional refresh (if evaluation later changes status)
                updateStatusSummary();
                LoadApplications();
                return;
            }

            // VIEW -> show approved/released application details
            if (string.Equals(action, "View", StringComparison.OrdinalIgnoreCase))
            {
                using (var dlg = new ApprovedLoanApplicationDialog(appId))
                {
                    dlg.ShowDialog(this);
                }

                updateStatusSummary();
                LoadApplications();
                return;
            }

            MessageBox.Show($"{action} {appId}", "Application Action",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HandleRejectFromList(string appId)
        {
            // Prompt for rejection reason (required)
            var reason = PromptRejectionReason();
            if (reason == null) return; // user cancelled

            if (MessageBox.Show($"Are you sure you want to reject application {appId}?", "Confirm Rejection",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                var loanRepo = new LendingApp.Class.Repo.LoanApplicationRepository();
                var evalRepo = new LendingApp.Class.Repo.LoanApplicationEvaluationRepository();

                var entity = loanRepo.GetByApplicationNumber(appId);
                if (entity == null)
                {
                    MessageBox.Show("Application not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create a minimal evaluation record documenting the rejection
                var eval = new LoanApplicationEvaluationEntity
                {
                    ApplicationId = entity.ApplicationId,
                    Decision = "Reject",
                    RejectionReason = reason,
                    Remarks = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    StatusAfter = "Rejected"
                };
                evalRepo.Add(eval);

                // Update application entity
                entity.PreferredTerm = entity.PreferredTerm; // keep existing
                entity.Status = "Rejected";
                entity.RejectionReason = reason;
                entity.StatusDate = DateTime.Now;

                loanRepo.Update(entity);

                MessageBox.Show("Application rejected.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                updateStatusSummary();
                LoadApplications();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to reject application.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string PromptRejectionReason()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "Rejection Reason";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.Size = new Size(420, 220);
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

                var lbl = new Label { Text = "Select reason for rejection (required):", Location = new Point(10, 10), AutoSize = true };
                panel.Controls.Add(lbl);

                var cbo = new ComboBox
                {
                    Location = new Point(10, 36),
                    Width = 380,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cbo.Items.AddRange(new object[]
                {
                    "Low Credit Score",
                    "Insufficient Income",
                    "High Existing Debt",
                    "Incomplete Documents",
                    "Failed Verification",
                    "Other"
                });
                cbo.SelectedIndex = 0;
                panel.Controls.Add(cbo);

                var notesLbl = new Label { Text = "Optional note:", Location = new Point(10, 72), AutoSize = true };
                panel.Controls.Add(notesLbl);

                var txt = new TextBox { Location = new Point(10, 92), Width = 380, Height = 40, Multiline = true, ScrollBars = ScrollBars.Vertical };
                panel.Controls.Add(txt);

                var btnOk = new Button { Text = "OK", Location = new Point(220, 140), Size = new Size(80, 30), BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White };
                var btnCancel = new Button { Text = "Cancel", Location = new Point(310, 140), Size = new Size(80, 30) };

                btnOk.Click += (s, e) =>
                {
                    if (cbo.SelectedIndex < 0)
                    {
                        MessageBox.Show("Please select a rejection reason.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    dlg.DialogResult = DialogResult.OK;
                    dlg.Close();
                };
                btnCancel.Click += (s, e) =>
                {
                    dlg.DialogResult = DialogResult.Cancel;
                    dlg.Close();
                };

                panel.Controls.Add(btnOk);
                panel.Controls.Add(btnCancel);

                dlg.Controls.Add(panel);

                if (dlg.ShowDialog(this) != DialogResult.OK) return null;

                var reason = cbo.SelectedItem?.ToString() ?? "Other";
                var note = txt.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(note))
                    return $"{reason}: {note}";
                return reason;
            }
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
