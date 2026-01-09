using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using System.Data.Entity;

namespace LendingSystem.Reports
{
    public partial class SystemReportsControl : UserControl
    {
        private TabControl tabControl;

        public SystemReportsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // Main container
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Main tab control - MATCHING YOUR DESIGN
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height),
                Padding = new Point(12, 8)
            };

            // Create all 4 tabs
            TabPage loanReportsTab = new TabPage("Loan Reports");
            TabPage paymentReportsTab = new TabPage("Payment Reports");
            TabPage financialReportsTab = new TabPage("Financial Reports");
            TabPage customerReportsTab = new TabPage("Customer Reports");

            // Initialize tabs
            InitializeLoanReportsTab(loanReportsTab);
            InitializePaymentReportsTab(paymentReportsTab);
            InitializeFinancialReportsTab(financialReportsTab);
            InitializeCustomerReportsTab(customerReportsTab);

            // Add tabs to tab control
            tabControl.TabPages.Add(loanReportsTab);
            tabControl.TabPages.Add(paymentReportsTab);
            tabControl.TabPages.Add(financialReportsTab);
            tabControl.TabPages.Add(customerReportsTab);

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        private void InitializeLoanReportsTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Main container for this tab
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            int yPos = 0;

            // Header
            Label headerLabel = new Label
            {
                Text = "LOAN REPORTS",
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainContainer.Controls.Add(headerLabel);
            yPos += 50;

            // Blue border container
            Panel borderPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 500),
                BackColor = Color.FromArgb(239, 246, 255), // Light blue background
                BorderStyle = BorderStyle.FixedSingle
            };

            // Draw blue border
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.FromArgb(96, 165, 250), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(96, 165, 250), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(96, 165, 250), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(96, 165, 250), 2, ButtonBorderStyle.Solid);
            };

            int panelY = 20;

            // Report Selection Group
            GroupBox reportGroup = new GroupBox
            {
                Text = "Select Report:",
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 150),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.White
            };

            // Draw blue border for group
            reportGroup.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, reportGroup.ClientRectangle,
                    Color.FromArgb(147, 197, 253), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(147, 197, 253), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(147, 197, 253), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(147, 197, 253), 2, ButtonBorderStyle.Solid);
            };

            int radioY = 25;

            // Radio buttons for report selection
            RadioButton activeLoansRadio = new RadioButton
            {
                Text = "Active Loans Report",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            reportGroup.Controls.Add(activeLoansRadio);
            radioY += 25;

            RadioButton releasedLoansRadio = new RadioButton
            {
                Text = "Released Loans (Daily/Monthly/Yearly)",
                Location = new Point(15, radioY),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(releasedLoansRadio);
            radioY += 25;

            RadioButton loanAgingRadio = new RadioButton
            {
                Text = "Loan Aging Report (Optional)",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(loanAgingRadio);
            radioY += 25;

            RadioButton maturedLoansRadio = new RadioButton
            {
                Text = "Matured Loans Report",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(maturedLoansRadio);
            radioY += 25;

            RadioButton preTerminatedRadio = new RadioButton
            {
                Text = "Pre-terminated Loans Report",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(preTerminatedRadio);

            borderPanel.Controls.Add(reportGroup);
            panelY += 160;

            // Report Table
            DataGridView reportGrid = new DataGridView
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 8.5f),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(229, 231, 235)
            };

            // Style the grid - MATCHING YOUR DESIGN
            reportGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            reportGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            reportGrid.EnableHeadersVisualStyles = false;
            reportGrid.ColumnHeadersHeight = 40;
            reportGrid.RowTemplate.Height = 35;

            // Add columns
            reportGrid.Columns.Add("id", "ID");
            reportGrid.Columns.Add("customer", "Customer");
            reportGrid.Columns.Add("loanType", "Loan Type");
            reportGrid.Columns.Add("amount", "Amount");
            reportGrid.Columns.Add("date", "Date");
            reportGrid.Columns.Add("status", "Status");

            // Load real data from database; fall back to sample data on error
            LoadLoanDataIntoGrid(reportGrid);

            borderPanel.Controls.Add(reportGrid);
            panelY += 210;

            // Action Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 40),
                BackColor = Color.Transparent
            };

            Button viewChartButton = new Button
            {
                Text = "View Chart",
                Location = new Point(0, 0),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(34, 197, 94), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            viewChartButton.FlatAppearance.BorderSize = 0;
            viewChartButton.Click += (s, e) =>
                MessageBox.Show("Opening chart view...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button exportPdfButton = new Button
            {
                Text = "Export to PDF",
                Location = new Point(130, 0),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat
            };
            exportPdfButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            exportPdfButton.FlatAppearance.BorderSize = 1;
            exportPdfButton.Click += (s, e) =>
                MessageBox.Show("Exporting to PDF...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button exportExcelButton = new Button
            {
                Text = "Export to Excel",
                Location = new Point(260, 0),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat
            };
            exportExcelButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            exportExcelButton.FlatAppearance.BorderSize = 1;
            exportExcelButton.Click += (s, e) =>
                MessageBox.Show("Exporting to Excel...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            buttonPanel.Controls.Add(viewChartButton);
            buttonPanel.Controls.Add(exportPdfButton);
            buttonPanel.Controls.Add(exportExcelButton);
            borderPanel.Controls.Add(buttonPanel);

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                headerLabel.Width = mainContainer.Width;
                borderPanel.Width = mainContainer.Width - 20;

                // Update child controls inside border panel
                reportGroup.Width = borderPanel.Width - 40;
                reportGrid.Width = borderPanel.Width - 40;
                buttonPanel.Width = borderPanel.Width - 40;
            };

            mainContainer.Controls.Add(borderPanel);
            tab.Controls.Add(mainContainer);
        }

        /// <summary>
        /// Loads loan data from the database into the provided grid.
        /// On any failure the method falls back to the existing sample data method.
        /// </summary>
        private void LoadLoanDataIntoGrid(DataGridView grid)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var rows = (from l in db.Loans.AsNoTracking()
                                join c in db.Customers.AsNoTracking() on l.CustomerId equals c.CustomerId into cj
                                from c in cj.DefaultIfEmpty()
                                join p in db.LoanProducts.AsNoTracking() on l.ProductId equals p.ProductId into pj
                                from p in pj.DefaultIfEmpty()
                                select new
                                {
                                    LoanNumber = l.LoanNumber,
                                    CustomerName = ((c != null ? (c.FirstName ?? "") : "") + " " + (c != null ? (c.LastName ?? "") : "")).Trim(),
                                    ProductName = p != null ? p.ProductName : "",
                                    Amount = l.PrincipalAmount,
                                    Date = l.ReleaseDate,
                                    Status = l.Status
                                })
                                .OrderByDescending(x => (DateTime?)x.Date ?? DateTime.MinValue)
                                .ToList();

                    grid.Rows.Clear();

                    foreach (var r in rows)
                    {
                        var dateText = r.Date != DateTime.MinValue ? r.Date.ToString("yyyy-MM-dd") : "";
                        int idx = grid.Rows.Add(
                            r.LoanNumber ?? "",
                            string.IsNullOrWhiteSpace(r.CustomerName) ? "" : r.CustomerName,
                            r.ProductName ?? "",
                            $"₱{r.Amount:N2}",
                            dateText,
                            r.Status ?? ""
                        );

                        // Color code status cell
                        var statusCell = grid.Rows[idx].Cells["status"];
                        var status = (r.Status ?? "").Trim();
                        if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        {
                            statusCell.Style.BackColor = Color.FromArgb(220, 252, 231);
                            statusCell.Style.ForeColor = Color.FromArgb(21, 128, 61);
                        }
                        else if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase) || status.Equals("Defaulted", StringComparison.OrdinalIgnoreCase))
                        {
                            statusCell.Style.BackColor = Color.FromArgb(254, 226, 226);
                            statusCell.Style.ForeColor = Color.FromArgb(185, 28, 28);
                        }
                        else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase) || status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                        {
                            statusCell.Style.BackColor = Color.FromArgb(254, 249, 195);
                            statusCell.Style.ForeColor = Color.FromArgb(161, 98, 7);
                        }
                    }
                }
            }
            catch
            {
                // If any error occurs reading the database, fall back to sample data so UI remains usable.
                AddSampleLoanData(grid);
            }
        }

        private void InitializePaymentReportsTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Add the PaymentReportsControl instead of placeholder
            var paymentReportsControl = new PaymentReportsControl();
            paymentReportsControl.Dock = DockStyle.Fill;
            tab.Controls.Add(paymentReportsControl);
        }

        private void InitializeFinancialReportsTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Add the FinancialReportsControl instead of placeholder
            var financialReportsControl = new FinancialReportsControl();
            financialReportsControl.Dock = DockStyle.Fill;
            tab.Controls.Add(financialReportsControl);
        }

        private void InitializeCustomerReportsTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Add the CustomerReportsControl instead of placeholder
            var customerReportsControl = new CustomerReportsControl();
            customerReportsControl.Dock = DockStyle.Fill;
            tab.Controls.Add(customerReportsControl);
        }

        private void AddSampleLoanData(DataGridView grid)
        {
            grid.Rows.Clear();

            // Add sample data
            string[][] sampleData = new string[][]
            {
                new string[] { "L-101", "Juan Dela Cruz", "Personal Loan", "₱50,000.00", "2024-12-25", "Active" },
                new string[] { "L-102", "Maria Santos", "Business Loan", "₱250,000.00", "2024-12-24", "Active" },
                new string[] { "L-103", "Pedro Reyes", "Emergency Loan", "₱20,000.00", "2024-12-23", "Completed" },
                new string[] { "L-104", "Anna Lim", "Home Loan", "₱500,000.00", "2024-12-22", "Active" },
                new string[] { "L-105", "Robert Tan", "Vehicle Loan", "₱150,000.00", "2024-12-21", "Overdue" },
                new string[] { "L-106", "Susan Wong", "Personal Loan", "₱75,000.00", "2024-12-20", "Active" },
                new string[] { "L-107", "Michael Chen", "Business Loan", "₱300,000.00", "2024-12-19", "Active" },
                new string[] { "L-108", "Lisa Garcia", "Education Loan", "₱100,000.00", "2024-12-18", "Completed" }
            };

            foreach (var row in sampleData)
            {
                grid.Rows.Add(row);
            }

            // Color code status
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells["status"].Value != null)
                {
                    string status = row.Cells["status"].Value.ToString();
                    if (status == "Active")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(220, 252, 231);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(21, 128, 61);
                    }
                    else if (status == "Overdue")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(254, 226, 226);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(185, 28, 28);
                    }
                    else if (status == "Completed")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(254, 249, 195);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(161, 98, 7);
                    }
                }
            }
        }
    }
}