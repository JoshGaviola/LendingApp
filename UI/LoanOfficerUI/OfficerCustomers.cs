using LendingApp.Class.Data;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Repo;
using LendingApp.UI.CustomerUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerCustomers : Form
    {
        private string customerFilter = "all";
        private string searchQuery = "";

        private CustomerRegistration _openRegistrationForm;
        private CustomerData customerData = new CustomerData();

        private BindingList<CustomerItem> customers;
        private readonly ICustomerRepository _customerRepo = new CustomerRepository();

        public OfficerCustomers()
        {
            InitializeComponent();
            customers = customerData.GetAllCustomers();

            BuildUI();
            BindFilters();

            RefreshTable();
            StatusUpdate();
        }

        private void ReloadCustomers()
        {
            customerData.LoadCustomerFromDb();
            customers = customerData.GetAllCustomers();

            RefreshTable();
            StatusUpdate();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.ForeColor == Color.Gray) return;

            searchQuery = txtSearch.Text ?? "";
            RefreshTable();
            StatusUpdate();
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

            // Ensure the search handler is wired exactly once
            txtSearch.TextChanged -= txtSearch_TextChanged;
            txtSearch.TextChanged += txtSearch_TextChanged;

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

        private IEnumerable<CustomerItem> Filtered()
        {
            var list = customers ?? new BindingList<CustomerItem>();

            return list.Where(c =>
            {
                bool matchesType = customerFilter == "all" || (c.Type ?? "").Equals(customerFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesSearch = string.IsNullOrWhiteSpace(searchQuery)
                    || ((c.Name ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((c.Id ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((c.Contact ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);

                return matchesType && matchesSearch;
            });
        }

        // KEEP ONLY THIS StatusUpdate() (filtered-aware)
        private void StatusUpdate()
        {
            var filtered = Filtered().ToList();

            lblTotalCustomers.Text = filtered.Count.ToString();
            lblNew.Text = filtered.Count(c => string.Equals(c.Type, "New", StringComparison.OrdinalIgnoreCase)).ToString();
            lblRegular.Text = filtered.Count(c => string.Equals(c.Type, "Regular", StringComparison.OrdinalIgnoreCase)).ToString();
            lblVIP.Text = filtered.Count(c => string.Equals(c.Type, "VIP", StringComparison.OrdinalIgnoreCase)).ToString();
            lblDelinquent.Text = filtered.Count(c => string.Equals(c.Type, "Delinquent", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void RefreshTable()
        {
            var filtered = Filtered().ToList();

            gridCustomers.Rows.Clear();
            foreach (var c in filtered)
            {
                int rowIndex = gridCustomers.Rows.Add(
                    c.Id,
                    c.Name,
                    c.Contact,
                    c.Type,
                    c.CreditScore,
                    c.TotalLoans,
                    c.Balance,
                    "View"
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

            lblResults.Text = $"Showing {filtered.Count} of {(customers?.Count ?? 0)} customers";
        }

        private void GridCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(gridCustomers.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var custId = gridCustomers.Rows[e.RowIndex].Cells["CustId"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(custId)) return;

            var c = _customerRepo.GetById(custId);
            if (c == null)
            {
                MessageBox.Show("Customer not found in database.", "Customer",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialogData = new CustomerProfileDialog.CustomerData
            {
                Id = c.CustomerId,
                FullName = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                DOB = c.DateOfBirth.HasValue ? c.DateOfBirth.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")) : "",
                Age = c.DateOfBirth.HasValue ? (int)((DateTime.Today - c.DateOfBirth.Value.Date).TotalDays / 365.2425) : 0,
                Gender = c.Gender,
                CivilStatus = c.CivilStatus,
                Nationality = c.Nationality,
                Email = c.EmailAddress,
                Mobile = c.MobileNumber,
                Telephone = c.TelephoneNumber,
                PresentAddress = c.PresentAddress,
                PermanentAddress = c.PermanentAddress,
                RegistrationDate = c.RegistrationDate.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")),
                CustomerType = c.CustomerType,
                CreditScore = c.InitialCreditScore,
                CreditLimit = "₱" + c.CreditLimit.ToString("N2", CultureInfo.GetCultureInfo("en-US")),
                Status = c.Status,

                ActiveLoans = 0,
                TotalBalance = "₱0.00",
                PaymentHistory = ""
            };

            using (var dlg = new CustomerProfileDialog(dialogData))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadCustomers();
                }
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
                        ReloadCustomers();
                    }
                };
                _openRegistrationForm.Show(this);
            }
            else
            {
                _openRegistrationForm.Focus();
            }
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
    }
}