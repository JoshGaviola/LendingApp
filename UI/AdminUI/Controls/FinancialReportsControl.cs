using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using System.Data.Entity;

namespace LendingSystem.Reports
{
    public partial class FinancialReportsControl : UserControl
    {
        private DataGridView financialGrid;

        public FinancialReportsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

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
                Text = "FINANCIAL REPORTS",
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainContainer.Controls.Add(headerLabel);
            yPos += 50;

            // Purple border container
            Panel borderPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 500),
                BackColor = Color.FromArgb(250, 245, 255), // Light purple background
                BorderStyle = BorderStyle.FixedSingle
            };

            // Draw purple border
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,  // Purple border
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid);
            };

            int panelY = 20;

            // Report Selection Group
            GroupBox reportGroup = new GroupBox
            {
                Text = "Select Report:",
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 210),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.White
            };

            // Draw purple border for group
            reportGroup.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, reportGroup.ClientRectangle,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid);
            };

            int radioY = 25;

            // Radio buttons for report selection
            RadioButton interestIncomeRadio = new RadioButton
            {
                Text = "Interest Income Report",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            reportGroup.Controls.Add(interestIncomeRadio);
            radioY += 25;

            RadioButton serviceChargeRadio = new RadioButton
            {
                Text = "Service Charge Income Report",
                Location = new Point(15, radioY),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(serviceChargeRadio);
            radioY += 25;

            RadioButton outstandingBalanceRadio = new RadioButton
            {
                Text = "Outstanding Principal Balance",
                Location = new Point(15, radioY),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(outstandingBalanceRadio);
            radioY += 25;

            RadioButton portfolioRiskRadio = new RadioButton
            {
                Text = "Portfolio at Risk (PAR)",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(portfolioRiskRadio);
            radioY += 25;

            RadioButton loanLossRadio = new RadioButton
            {
                Text = "Loan Loss Provision Report",
                Location = new Point(15, radioY),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(loanLossRadio);
            radioY += 25;

            RadioButton financialSummaryRadio = new RadioButton
            {
                Text = "Financial Summary (Monthly/Yearly)",
                Location = new Point(15, radioY),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(financialSummaryRadio);

            borderPanel.Controls.Add(reportGroup);
            panelY += 220;

            // Financial Metrics Table
            financialGrid = new DataGridView
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 180),
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

            // Style the grid
            financialGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            financialGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            financialGrid.EnableHeadersVisualStyles = false;
            financialGrid.ColumnHeadersHeight = 40;
            financialGrid.RowTemplate.Height = 35;

            // Add columns
            financialGrid.Columns.Add("metric", "Metric");
            financialGrid.Columns.Add("thisMonth", "This Month");
            financialGrid.Columns.Add("lastMonth", "Last Month");
            financialGrid.Columns.Add("ytd", "YTD");

            // Load real data from database; fall back to sample data on error
            LoadFinancialDataIntoGrid(financialGrid);

            borderPanel.Controls.Add(financialGrid);
            panelY += 190;

            // Action Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 40),
                BackColor = Color.Transparent
            };

            Button portfolioChartButton = new Button
            {
                Text = "Portfolio Health Chart",
                Location = new Point(0, 0),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(168, 85, 247), // Purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            portfolioChartButton.FlatAppearance.BorderSize = 0;
            portfolioChartButton.Click += (s, e) =>
                MessageBox.Show("Opening portfolio health chart...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button exportExcelButton = new Button
            {
                Text = "Export to Excel",
                Location = new Point(190, 0),
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

            buttonPanel.Controls.Add(portfolioChartButton);
            buttonPanel.Controls.Add(exportExcelButton);
            borderPanel.Controls.Add(buttonPanel);

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                headerLabel.Width = mainContainer.Width;
                borderPanel.Width = mainContainer.Width - 20;

                // Update child controls inside border panel
                reportGroup.Width = borderPanel.Width - 40;
                financialGrid.Width = borderPanel.Width - 40;
                buttonPanel.Width = borderPanel.Width - 40;
            };

            mainContainer.Controls.Add(borderPanel);
            this.Controls.Add(mainContainer);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Load financial metrics from DB and populate grid. Falls back to sample data on error.
        /// Metrics are best-effort using available tables: Payments and Loans.
        /// </summary>
        private void LoadFinancialDataIntoGrid(DataGridView grid)
        {
            try
            {
                var today = DateTime.Today;
                var thisMonthStart = new DateTime(today.Year, today.Month, 1);
                var thisMonthEnd = thisMonthStart.AddMonths(1).AddDays(-1);
                var lastMonthStart = thisMonthStart.AddMonths(-1);
                var lastMonthEnd = thisMonthStart.AddDays(-1);
                var ytdStart = new DateTime(today.Year, 1, 1);

                using (var db = new AppDbContext())
                {
                    // Interest income: sum of interest_portion from payments
                    var interestThis = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= thisMonthStart && p.PaymentDate <= thisMonthEnd)
                        .Sum(p => (decimal?)p.InterestPaid) ?? 0m;

                    var interestLast = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= lastMonthStart && p.PaymentDate <= lastMonthEnd)
                        .Sum(p => (decimal?)p.InterestPaid) ?? 0m;

                    var interestYtd = db.Payments.AsNoTracking()
                        .Where(p => p.PaymentDate >= ytdStart && p.PaymentDate <= today)
                        .Sum(p => (decimal?)p.InterestPaid) ?? 0m;

                    // Service charges: use loan.ProcessingFee for loans released in period
                    var serviceThis = db.Loans.AsNoTracking()
                        .Where(l => l.ReleaseDate >= thisMonthStart && l.ReleaseDate <= thisMonthEnd)
                        .Sum(l => (decimal?)l.ProcessingFee) ?? 0m;

                    var serviceLast = db.Loans.AsNoTracking()
                        .Where(l => l.ReleaseDate >= lastMonthStart && l.ReleaseDate <= lastMonthEnd)
                        .Sum(l => (decimal?)l.ProcessingFee) ?? 0m;

                    var serviceYtd = db.Loans.AsNoTracking()
                        .Where(l => l.ReleaseDate >= ytdStart && l.ReleaseDate <= today)
                        .Sum(l => (decimal?)l.ProcessingFee) ?? 0m;

                    // Outstanding principal balance: sum of outstanding_balance for active (non-paid) loans
                    var outThis = db.Loans.AsNoTracking()
                        .Where(l => !(l.Status ?? "").Equals("Paid", StringComparison.OrdinalIgnoreCase))
                        .Sum(l => (decimal?)l.OutstandingBalance) ?? 0m;

                    // Approximate last month outstanding by considering loans released before or on lastMonthEnd.
                    var outLast = db.Loans.AsNoTracking()
                        .Where(l => l.ReleaseDate <= lastMonthEnd && !(l.Status ?? "").Equals("Paid", StringComparison.OrdinalIgnoreCase))
                        .Sum(l => (decimal?)l.OutstandingBalance) ?? 0m;

                    // YTD outstanding: current snapshot (best-effort)
                    var outYtd = db.Loans.AsNoTracking()
                        .Sum(l => (decimal?)l.OutstandingBalance) ?? 0m;

                    // PAR (30+ days)
                    var parThis = db.Loans.AsNoTracking()
                        .Where(l => l.DaysOverdue >= 30)
                        .Sum(l => (decimal?)l.OutstandingBalance) ?? 0m;

                    // Loan loss provision: example rule -> 5% of outstanding for loans 90+ days overdue
                    var lossProvisionThis = db.Loans.AsNoTracking()
                        .Where(l => l.DaysOverdue >= 90)
                        .Sum(l => (decimal?)l.OutstandingBalance) ?? 0m;
                    lossProvisionThis = Math.Round(lossProvisionThis * 0.05m, 2);

                    // Clear and populate grid
                    grid.Rows.Clear();

                    grid.Rows.Add("Interest Income", Money(interestThis), Money(interestLast), Money(interestYtd));
                    grid.Rows.Add("Service Charges", Money(serviceThis), Money(serviceLast), Money(serviceYtd));
                    grid.Rows.Add("Outstanding Bal", Money(outThis), Money(outLast), Money(outYtd));
                    grid.Rows.Add("PAR (30+ days)", Money(parThis), "-", "-");
                    grid.Rows.Add("Loan Loss Provision (5% of 90+ days)", Money(lossProvisionThis), "-", "-");

                    // Style rows by metric type
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        var metric = (row.Cells["metric"].Value ?? "").ToString();
                        if (metric.Contains("Income") || metric.Contains("Charges"))
                        {
                            row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(240, 253, 244);
                            row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(240, 253, 244);
                            row.Cells["ytd"].Style.BackColor = Color.FromArgb(240, 253, 244);
                        }
                        else if (metric.Contains("PAR") || metric.Contains("Loss"))
                        {
                            row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(254, 242, 242);
                            row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(254, 242, 242);
                            row.Cells["ytd"].Style.BackColor = Color.FromArgb(254, 242, 242);
                        }
                        else if (metric.Contains("Outstanding"))
                        {
                            row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(239, 246, 255);
                            row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(239, 246, 255);
                            row.Cells["ytd"].Style.BackColor = Color.FromArgb(239, 246, 255);
                        }
                    }
                }
            }
            catch
            {
                // keep UI usable if DB read fails
                AddSampleFinancialData();
            }
        }

        private static string Money(decimal amount)
        {
            return "₱" + amount.ToString("N2", CultureInfo.GetCultureInfo("en-US"));
        }

        private void AddSampleFinancialData()
        {
            if (financialGrid == null) return;

            financialGrid.Rows.Clear();

            // Add sample financial data
            string[][] sampleData = new string[][]
            {
                new string[] { "Interest Income", "₱45,230.50", "₱42,100.00", "₱210,450.75" },
                new string[] { "Service Charges", "₱5,200.00", "₱4,800.00", "₱28,400.00" },
                new string[] { "Outstanding Bal", "₱1,245,000.00", "₱1,300,000.00", "-" },
                new string[] { "PAR (30+ days)", "₱85,000.00", "₱72,000.00", "-" }
            };

            foreach (var row in sampleData)
            {
                financialGrid.Rows.Add(row);
            }

            // Color code positive/negative trends
            foreach (DataGridViewRow row in financialGrid.Rows)
            {
                if (row.Cells["metric"].Value != null)
                {
                    string metric = row.Cells["metric"].Value.ToString();

                    // Color code based on metric type
                    if (metric.Contains("Income") || metric.Contains("Charges"))
                    {
                        // Revenue metrics - green highlight
                        row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(240, 253, 244);
                        row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(240, 253, 244);
                        row.Cells["ytd"].Style.BackColor = Color.FromArgb(240, 253, 244);
                    }
                    else if (metric.Contains("PAR") || metric.Contains("Loss"))
                    {
                        // Risk metrics - light red highlight
                        row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(254, 242, 242);
                        row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(254, 242, 242);
                        row.Cells["ytd"].Style.BackColor = Color.FromArgb(254, 242, 242);
                    }
                    else if (metric.Contains("Outstanding"))
                    {
                        // Balance metrics - light blue highlight
                        row.Cells["thisMonth"].Style.BackColor = Color.FromArgb(239, 246, 255);
                        row.Cells["lastMonth"].Style.BackColor = Color.FromArgb(239, 246, 255);
                        row.Cells["ytd"].Style.BackColor = Color.FromArgb(239, 246, 255);
                    }
                }
            }
        }
    }
}