using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public partial class CustomerLoansDialog : Form
    {
        private readonly string _customerId;
        private readonly string _customerName;
        private DataGridView _grid;
        private Button _btnClose;
        private Button _btnRefresh;

        public CustomerLoansDialog(string customerId, string customerName = null)
        {
            _customerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
            _customerName = customerName ?? customerId;

            InitializeComponent();
            InitializeControls();
            LoadData();
        }

        // Renamed from InitializeComponent to avoid duplicate with designer
        private void InitializeControls()
        {
            Text = $"Loans — {_customerName}";
            Size = new Size(800, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);

            _grid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 420,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };

            _grid.Columns.Clear();

            // Hidden source column used for wiring the View action
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Source", HeaderText = "Source", Visible = false });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date" });

            var actions = new DataGridViewButtonColumn
            {
                Name = "Action",
                HeaderText = "Action",
                Text = "View",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            _grid.Columns.Add(actions);

            _grid.CellContentClick -= Grid_CellContentClick;
            _grid.CellContentClick += Grid_CellContentClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Width = 100,
                Height = 30,
                Location = new Point(12, 432),
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnRefresh.Click += (s, e) => LoadData();

            _btnClose = new Button
            {
                Text = "Close",
                Width = 100,
                Height = 30,
                Location = new Point(124, 432)
            };
            _btnClose.Click += (s, e) => Close();

            // Add controls to the form's Controls collection (designer may have added others)
            // Ensure we don't duplicate if designer already placed some controls.
            if (!Controls.Contains(_grid)) Controls.Add(_grid);
            if (!Controls.Contains(_btnRefresh)) Controls.Add(_btnRefresh);
            if (!Controls.Contains(_btnClose)) Controls.Add(_btnClose);
        }

        private void LoadData()
        {
            _grid.Rows.Clear();

            try
            {
                using (var db = new AppDbContext())
                {
                    // Load loan applications for this customer (if any)
                    var apps = db.LoanApplications
                                 .AsNoTracking()
                                 .Where(a => a.CustomerId == _customerId)
                                 .OrderByDescending(a => a.ApplicationDate)
                                 .ToList();

                    foreach (var a in apps)
                    {
                        var id = !string.IsNullOrWhiteSpace(a.ApplicationNumber)
                            ? a.ApplicationNumber
                            : a.ApplicationId.ToString();

                        var amount = a.RequestedAmount.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                        var date = a.ApplicationDate.ToString("yyyy-MM-dd");

                        // ProductId -> friendly text (keeps existing mapping approach)
                        string type;
                        switch (a.ProductId)
                        {
                            case 1: type = "Personal"; break;
                            case 2: type = "Emergency"; break;
                            case 3: type = "Salary"; break;
                            default: type = "Product " + a.ProductId; break;
                        }

                        int row = _grid.Rows.Add("Application", id, type, amount, a.Status ?? "", date);
                        _grid.Rows[row].Cells["Source"].Value = "Application";
                    }

                    // Loans (this DbSet exists in your AppDbContext; no reflection/dynamic needed)
                    var loans = db.Loans
                        .AsNoTracking()
                        .Where(l => l.CustomerId == _customerId)
                        .OrderByDescending(l => l.ReleaseDate)
                        .ToList();

                    foreach (var l in loans)
                    {
                        var id = l.LoanNumber ?? l.LoanId.ToString();
                        var amount = l.PrincipalAmount.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                        var date = l.ReleaseDate.ToString("yyyy-MM-dd");

                        string type;
                        switch (l.ProductId)
                        {
                            case 1: type = "Personal"; break;
                            case 2: type = "Emergency"; break;
                            case 3: type = "Salary"; break;
                            default: type = "Product " + l.ProductId; break;
                        }

                        int row = _grid.Rows.Add("Loan", id, type, amount, l.Status ?? "", date);
                        _grid.Rows[row].Cells["Source"].Value = "Loan";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load loans: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(_grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var row = _grid.Rows[e.RowIndex];
            var source = row.Cells["Source"].Value?.ToString();
            var id = row.Cells["Id"].Value?.ToString() ?? "";

            if (string.Equals(source, "Application", StringComparison.OrdinalIgnoreCase))
            {
                // Open existing Review dialog if appropriate, else show Approved dialog if approved.
                try
                {
                    var status = row.Cells["Status"].Value?.ToString() ?? "";
                    if (status.IndexOf("Approved", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        status.IndexOf("Released", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (var dlg = new LendingApp.UI.LoanOfficerUI.Dialog.ApprovedLoanApplicationDialog(id))
                        {
                            dlg.ShowDialog(this);
                        }
                    }
                    else
                    {
                        using (var dlg = new LendingApp.UI.LoanOfficerUI.Dialog.ReviewApplicationDialog(id))
                        {
                            dlg.ShowDialog(this);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show($"Open application: {id}", "View", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            if (string.Equals(source, "Loan", StringComparison.OrdinalIgnoreCase))
            {
                // No loan details dialog exists yet in provided context.
                MessageBox.Show($"Open loan details for {id} (not implemented).", "Loan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show($"View {id}", "View", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
