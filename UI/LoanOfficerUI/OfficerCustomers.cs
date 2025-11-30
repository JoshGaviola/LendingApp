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

        private class CustomerItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Contact { get; set; }
            public string Email { get; set; }
            public string Type { get; set; } // New | Regular | VIP | Delinquent
            public int CreditScore { get; set; }
            public int TotalLoans { get; set; }
            public string Balance { get; set; }
            public int BalanceAmount { get; set; }
            public string RegisteredDate { get; set; }
            public string LastActivity { get; set; }
        }

        private readonly List<CustomerItem> customers = new List<CustomerItem>
        {
            new CustomerItem { Id="CUST-001", Name="Juan Cruz", Contact="+639123456789", Email="juan.cruz@email.com", Type="Regular",  CreditScore=750, TotalLoans=2, Balance="₱85,000",  BalanceAmount=85000,  RegisteredDate="Jan 15, 2024", LastActivity="2 days ago" },
            new CustomerItem { Id="CUST-002", Name="Maria Santos", Contact="+639987654321", Email="maria.santos@email.com", Type="New",     CreditScore=680, TotalLoans=1, Balance="₱35,000",  BalanceAmount=35000,  RegisteredDate="Nov 20, 2025", LastActivity="5 days ago" },
            new CustomerItem { Id="CUST-003", Name="Pedro Reyes", Contact="+639456789012", Email="pedro.reyes@email.com", Type="VIP",     CreditScore=820, TotalLoans=0, Balance="₱0",       BalanceAmount=0,      RegisteredDate="Mar 10, 2024", LastActivity="1 hour ago" },
            new CustomerItem { Id="CUST-004", Name="Ana Lopez",   Contact="+639234567890", Email="ana.lopez@email.com",   Type="Regular", CreditScore=710, TotalLoans=3, Balance="₱120,500", BalanceAmount=120500, RegisteredDate="Feb 28, 2024", LastActivity="Yesterday" },
            new CustomerItem { Id="CUST-005", Name="Carlos Tan",  Contact="+639345678901", Email="carlos.tan@email.com",  Type="VIP",     CreditScore=795, TotalLoans=5, Balance="₱250,000", BalanceAmount=250000, RegisteredDate="Jan 05, 2024", LastActivity="3 hours ago" },
            new CustomerItem { Id="CUST-006", Name="Sofia Garcia",Contact="+639567890123", Email="sofia.garcia@email.com",Type="Delinquent", CreditScore=580, TotalLoans=1, Balance="₱15,000", BalanceAmount=15000, RegisteredDate="Aug 12, 2024", LastActivity="15 days ago" },
            new CustomerItem { Id="CUST-007", Name="Miguel Ramos",Contact="+639678901234", Email="miguel.ramos@email.com",Type="Regular", CreditScore=720, TotalLoans=2, Balance="₱60,000",  BalanceAmount=60000,  RegisteredDate="Apr 18, 2024", LastActivity="1 day ago" },
            new CustomerItem { Id="CUST-008", Name="Isabel Cruz", Contact="+639789012345", Email="isabel.cruz@email.com", Type="New",     CreditScore=650, TotalLoans=0, Balance="₱0",       BalanceAmount=0,      RegisteredDate="Nov 28, 2025", LastActivity="Today" },
        };

        public OfficerCustomers()
        {
            InitializeComponent();
            BuildUI();
            BindFilters();
            RefreshTable();
            UpdateStatsCards();
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
            btnRegisterCustomer.Click += (s, e) => MessageBox.Show("Register Customer clicked");

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

        private IEnumerable<CustomerItem> Filtered()
        {
            return customers.Where(c =>
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
                int rowIndex = gridCustomers.Rows.Add(c.Id, ToNameWithEmail(c), c.Contact, c.Type, c.CreditScore.ToString(), c.TotalLoans, c.Balance, "View");
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

            lblResults.Text = $"Showing {filtered.Count} of {customers.Count} customers";
            UpdateStatsCards();
        }

        private string ToNameWithEmail(CustomerItem c)
        {
            if (string.IsNullOrWhiteSpace(c.Email)) return c.Name;
            return $"{c.Name}  ({c.Email})";
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

        private void UpdateStatsCards()
        {
            lblTotalCustomers.Text = customers.Count.ToString();
            lblNew.Text = customers.Count(c => c.Type == "New").ToString();
            lblRegular.Text = customers.Count(c => c.Type == "Regular").ToString();
            lblVIP.Text = customers.Count(c => c.Type == "VIP").ToString();
            lblDelinquent.Text = customers.Count(c => c.Type == "Delinquent").ToString();
        }
    }
}
