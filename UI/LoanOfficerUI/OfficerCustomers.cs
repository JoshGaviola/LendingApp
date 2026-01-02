using LendingApp.Class;
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

        private CustomerRegistration _openRegistrationForm;

        private List<CustomerItem> _dbCustomers = new List<CustomerItem>();

        public OfficerCustomers()
        {
            InitializeComponent();

            BuildUI();
            BindFilters();

            ReloadCustomersFromDb();
            RefreshTable();
            StatusUpdate();
        }

        // ADD THIS METHOD (fixes the designer compile error)
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Keep behavior consistent with your placeholder setup in BuildUI()
            if (txtSearch.ForeColor == Color.Gray) return;

            searchQuery = txtSearch.Text ?? "";
            RefreshTable();
            StatusUpdate();
        }

        private void ReloadCustomersFromDb()
        {
            using (var db = new AppDbContext())
            {
                // Step 1: Fetch raw data from DB (no ToString formatting)
                var rawData = db.Customers
                    .AsNoTracking()
                    .OrderByDescending(c => c.RegistrationDate)
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.FirstName,
                        c.LastName,
                        c.MobileNumber,
                        c.EmailAddress,
                        c.CustomerType,
                        c.InitialCreditScore,
                        c.RegistrationDate
                    })
                    .ToList();

                // Step 2: Project to CustomerItem in memory (formatting allowed here)
                _dbCustomers = rawData
                    .Select(c => new CustomerItem
                    {
                        Id = c.CustomerId,
                        Name = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                        Contact = c.MobileNumber,
                        Email = c.EmailAddress,
                        Type = c.CustomerType,
                        CreditScore = c.InitialCreditScore,
                        TotalLoans = 0,
                        BalanceAmount = 0,
                        Balance = "₱0.00",
                        RegisteredDate = c.RegistrationDate.ToString("MMM dd, yyyy"),
                        LastActivity = ""
                    })
                    .ToList();
            }
        }

        private void BuildUI()
        {
            Text = "Customer Management";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            lblHeaderTitle.Text = "Customer Management";
            lblHeaderTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHeaderSubtitle.Text = "View and manage customer profiles and accounts";
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9);
            btnRegisterCustomer.Text = "Register Customer";
            btnRegisterCustomer.BackColor = ColorTranslator.FromHtml("#3498DB");
            btnRegisterCustomer.ForeColor = Color.White;
            btnRegisterCustomer.FlatStyle = FlatStyle.Flat;

            cmbCustomerType.Items.AddRange(new object[] { "All Customers", "New", "Regular", "VIP", "Delinquent" });
            cmbCustomerType.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbCustomerType.SelectedItem?.ToString() ?? "All Customers";
                customerFilter = val.Equals("All Customers", StringComparison.OrdinalIgnoreCase) ? "all" : val;
                RefreshTable();
                StatusUpdate();
            };

            // Placeholder behavior
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

            // IMPORTANT: remove the inline TextChanged handler to avoid double firing.
            // The designer already wires txtSearch_TextChanged.
            // txtSearch.TextChanged += (s, e) => { ... };

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

            lblResults.Text = "";
            lblInfoTitle.Text = "Customer Classification";
            lblInfoText.Text = "New: First-time borrowers • Regular: 1-4 successful loans • VIP: 5+ loans with excellent history • Delinquent: Late payments or defaults";
        }

        private void BindFilters()
        {
            cmbCustomerType.SelectedIndex = 0; // All Customers
        }

        private void StatusUpdate()
        {
            lblTotalCustomers.Text = _dbCustomers.Count.ToString();

            lblNew.Text = _dbCustomers.Count(c => string.Equals(c.Type, "New", StringComparison.OrdinalIgnoreCase)).ToString();
            lblRegular.Text = _dbCustomers.Count(c => string.Equals(c.Type, "Regular", StringComparison.OrdinalIgnoreCase)).ToString();
            lblVIP.Text = _dbCustomers.Count(c => string.Equals(c.Type, "VIP", StringComparison.OrdinalIgnoreCase)).ToString();
            lblDelinquent.Text = _dbCustomers.Count(c => string.Equals(c.Type, "Delinquent", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private IEnumerable<CustomerItem> Filtered()
        {
            return _dbCustomers.Where(c =>
            {
                bool matchesType = customerFilter == "all" || (c.Type ?? "").Equals(customerFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesSearch = string.IsNullOrWhiteSpace(searchQuery)
                    || ((c.Name ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((c.Id ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((c.Contact ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);

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
                    c.Id,                    // Cust ID
                    $"{c.Name} ({c.Email})", // Name with email
                    c.Contact,               // Contact
                    c.Type,                  // Type
                    c.CreditScore,           // Score
                    c.TotalLoans,            // Loans
                    c.Balance,               // Balance
                    "View"                   // Action button
                );

                var row = gridCustomers.Rows[rowIndex];

                var typeCell = row.Cells["Type"] as DataGridViewTextBoxCell;
                if (typeCell != null)
                {
                    typeCell.Style.BackColor = GetTypeBackColor(c.Type);
                    typeCell.Style.ForeColor = GetTypeForeColor(c.Type);
                }

                var scoreCell = row.Cells["Score"] as DataGridViewTextBoxCell;
                if (scoreCell != null)
                {
                    scoreCell.Value = $"{(c.CreditScore >= 700 ? "↑" : "↓")} {c.CreditScore}";
                    scoreCell.Style.ForeColor = GetScoreForeColor(c.CreditScore);
                }
            }

            lblResults.Text = $"Showing {filtered.Count} of {_dbCustomers.Count} customers";
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
            if (score >= 800) return ColorTranslator.FromHtml("#16A34A");
            if (score >= 700) return ColorTranslator.FromHtml("#2563EB");
            if (score >= 600) return ColorTranslator.FromHtml("#CA8A04");
            return ColorTranslator.FromHtml("#DC2626");
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

        private void btnRegisterCustomer_Click(object sender, EventArgs e)
        {
            if (_openRegistrationForm == null || _openRegistrationForm.IsDisposed)
            {
                _openRegistrationForm = new CustomerRegistration();
                _openRegistrationForm.FormClosed += (s, args) =>
                {
                    var result = _openRegistrationForm.DialogResult;
                    _openRegistrationForm = null;

                    if (result == DialogResult.OK)
                    {
                        ReloadCustomersFromDb();
                        RefreshTable();
                        StatusUpdate();
                    }
                };
                _openRegistrationForm.Show(this);
            }
            else
            {
                _openRegistrationForm.Focus();
            }
        }
    }
}
