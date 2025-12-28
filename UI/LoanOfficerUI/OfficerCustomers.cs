using LendingApp.Models.LoanOfficer;
using LendingApp.Models.LoanOfiicerModels;
using LendingApp.UI.CustomerUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerCustomers : Form
    {
        private string customerFilter = "all";
        private string searchQuery = "";

        private OfficerCustomersLogic CustomerLogic;
        private CustomerRegistration _openRegistrationForm;

        public OfficerCustomers()
        {
            InitializeComponent();
            CustomerLogic = new OfficerCustomersLogic();
            StatusUpdate();

            BuildUI();
            BindFilters();
            RefreshTable();
        }

        private void BuildUI()
        {
            Text = "Customer Management";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Header
            lblHeaderTitle.Text = "Customer Management";
            lblHeaderTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHeaderSubtitle.Text = "View and manage customer profiles and accounts";
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9);
            btnRegisterCustomer.Text = "Register Customer";
            btnRegisterCustomer.BackColor = ColorTranslator.FromHtml("#3498DB");
            btnRegisterCustomer.ForeColor = Color.White;
            btnRegisterCustomer.FlatStyle = FlatStyle.Flat;

            // Filters
            cmbCustomerType.Items.AddRange(new object[] { "All Customers", "New", "Regular", "VIP", "Delinquent" });
            cmbCustomerType.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbCustomerType.SelectedItem?.ToString() ?? "All Customers";
                customerFilter = val.Equals("All Customers", StringComparison.OrdinalIgnoreCase) ? "all" : val;
                RefreshTable();
            };
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Search by name, customer ID, or contact number...";
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by name, customer ID, or contact number...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray) return;
                searchQuery = txtSearch.Text ?? "";
                RefreshTable();
            };

            // Table
            gridCustomers.ReadOnly = true;
            gridCustomers.AllowUserToAddRows = false;
            gridCustomers.AllowUserToDeleteRows = false;
            gridCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridCustomers.RowHeadersVisible = false;
            gridCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCustomers.Columns.Clear();
            gridCustomers.Columns.Add("CustId", "Cust ID");
            gridCustomers.Columns.Add("Name", "Name");
            gridCustomers.Columns.Add("Contact", "Contact");
            gridCustomers.Columns.Add("Type", "Type");
            gridCustomers.Columns.Add("Score", "Score");
            gridCustomers.Columns.Add("Loans", "Loans");
            gridCustomers.Columns.Add("Balance", "Balance");
            var actionCol = new DataGridViewButtonColumn
            {
                HeaderText = "Action",
                Text = "View",
                UseColumnTextForButtonValue = true
            };
            gridCustomers.Columns.Add(actionCol);
            gridCustomers.CellContentClick += GridCustomers_CellContentClick;

            // Results info
            lblResults.Text = "";

            // Info section
            lblInfoTitle.Text = "Customer Classification";
            lblInfoText.Text = "New: First-time borrowers • Regular: 1-4 successful loans • VIP: 5+ loans with excellent history • Delinquent: Late payments or defaults";
        }

        private void BindFilters()
        {
            cmbCustomerType.SelectedIndex = 0; // All Customers
        }

        private void StatusUpdate()
        {
            lblTotalCustomers.Text = CustomerLogic.TotalCustomers.ToString();
            lblNew.Text = CustomerLogic.GetStatusSummary().Find(s => s.Type == "New")?.Count.ToString() ?? "0";
            lblRegular.Text = CustomerLogic.GetStatusSummary().Find(s => s.Type == "Regular")?.Count.ToString() ?? "0";
            lblVIP.Text = CustomerLogic.GetStatusSummary().Find(s => s.Type == "VIP")?.Count.ToString() ?? "0";
            lblDelinquent.Text = CustomerLogic.GetStatusSummary().Find(s => s.Type == "Delinquent")?.Count.ToString() ?? "0";
        }

        private IEnumerable<CustomerItem> Filtered()
        {
            return CustomerLogic.AllCustomers.Where(c =>
            {
                bool matchesType = customerFilter == "all" || c.Type.Equals(customerFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesSearch = string.IsNullOrWhiteSpace(searchQuery)
                    || (c.Name?.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                    || (c.Id?.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                    || (c.Contact?.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0;
                return matchesType && matchesSearch;
            });
        }

        private void RefreshTable()
        {
            var filtered = Filtered().ToList();

            gridCustomers.Rows.Clear();
            foreach (var c in filtered)
            {
                int rowIndex = gridCustomers.Rows.Add(
                    c.Id,                   // Cust ID
                    $"{c.Name} ({c.Email})",// Name with email
                    c.Contact,              // Contact
                    c.Type,                 // Type
                    c.CreditScore,          // Score
                    c.TotalLoans,           // Loans
                    c.Balance,              // Balance
                    "View"                  // Action button
                );

                var row = gridCustomers.Rows[rowIndex];

                // Style type pill-like
                var typeCell = row.Cells["Type"] as DataGridViewTextBoxCell;
                if (typeCell != null)
                {
                    typeCell.Style.BackColor = GetTypeBackColor(c.Type);
                    typeCell.Style.ForeColor = GetTypeForeColor(c.Type);
                }

                // Style credit score color and arrow indicator text prefix (↑/↓)
                var scoreCell = row.Cells["Score"] as DataGridViewTextBoxCell;
                if (scoreCell != null)
                {
                    scoreCell.Value = $"{(c.CreditScore >= 700 ? "↑" : "↓")} {c.CreditScore}";
                    scoreCell.Style.ForeColor = GetScoreForeColor(c.CreditScore);
                }
            }

            lblResults.Text = $"Showing {filtered.Count} of {CustomerLogic.TotalCustomers} customers";
        }

        private Color GetTypeBackColor(string type)
        {
            switch (type)
            {
                case "VIP": return ColorTranslator.FromHtml("#EDE9FE");
                case "Regular": return ColorTranslator.FromHtml("#DBEAFE");
                case "New": return ColorTranslator.FromHtml("#D1FAE5");
                case "Delinquent": return ColorTranslator.FromHtml("#FECACA");
                default: return ColorTranslator.FromHtml("#F3F4F6");
            }
        }

        private Color GetTypeForeColor(string type)
        {
            switch (type)
            {
                case "VIP": return ColorTranslator.FromHtml("#5B21B6");
                case "Regular": return ColorTranslator.FromHtml("#1D4ED8");
                case "New": return ColorTranslator.FromHtml("#065F46");
                case "Delinquent": return ColorTranslator.FromHtml("#7F1D1D");
                default: return ColorTranslator.FromHtml("#374151");
            }
        }

        private Color GetScoreForeColor(int score)
        {
            if (score >= 800) return ColorTranslator.FromHtml("#16A34A"); // green-600
            if (score >= 700) return ColorTranslator.FromHtml("#2563EB"); // blue-600
            if (score >= 600) return ColorTranslator.FromHtml("#CA8A04"); // yellow-600
            return ColorTranslator.FromHtml("#DC2626");                    // red-600
        }

        private void GridCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridCustomers.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var custId = gridCustomers.Rows[e.RowIndex].Cells[0].Value?.ToString();
                MessageBox.Show($"View {custId}", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRegisterCustomer_Click(object sender, EventArgs e)
        {
            if (_openRegistrationForm == null || _openRegistrationForm.IsDisposed)
            {
                _openRegistrationForm = new CustomerRegistration();
                _openRegistrationForm.FormClosed += (s, args) => _openRegistrationForm = null;
                _openRegistrationForm.Show(this);
            }
            else
            {
                _openRegistrationForm.Focus();
            }
        }
    }
}
