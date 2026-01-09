using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using System.Data.Entity;

namespace LendingSystem.Reports
{
    public partial class PaymentReportsControl : UserControl
    {
        private DataGridView paymentGrid;

        public PaymentReportsControl()
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
                Text = "PAYMENT REPORTS",
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainContainer.Controls.Add(headerLabel);
            yPos += 50;

            // Green border container
            Panel borderPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 500),
                BackColor = Color.FromArgb(240, 253, 244), // Light green background
                BorderStyle = BorderStyle.FixedSingle
            };

            // Draw green border
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.FromArgb(34, 197, 94), 2, ButtonBorderStyle.Solid,  // Green border
                    Color.FromArgb(34, 197, 94), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(34, 197, 94), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(34, 197, 94), 2, ButtonBorderStyle.Solid);
            };

            int panelY = 20;

            // Report Selection Group
            GroupBox reportGroup = new GroupBox
            {
                Text = "Select Report:",
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.White
            };

            // Draw green border for group
            reportGroup.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, reportGroup.ClientRectangle,
                    Color.FromArgb(134, 239, 172), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(134, 239, 172), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(134, 239, 172), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(134, 239, 172), 2, ButtonBorderStyle.Solid);
            };

            int radioY = 25;

            // Radio buttons for report selection
            RadioButton dailyCollectionRadio = new RadioButton
            {
                Text = "Daily Collection Report",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            reportGroup.Controls.Add(dailyCollectionRadio);
            radioY += 25;

            RadioButton paymentHistoryRadio = new RadioButton
            {
                Text = "Payment History per Customer",
                Location = new Point(15, radioY),
                Size = new Size(220, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(paymentHistoryRadio);
            radioY += 25;

            RadioButton expectedActualRadio = new RadioButton
            {
                Text = "Expected vs Actual Collections (Optional)",
                Location = new Point(15, radioY),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(expectedActualRadio);
            radioY += 25;

            RadioButton overdueAccountsRadio = new RadioButton
            {
                Text = "Overdue Accounts Report (Optional)",
                Location = new Point(15, radioY),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(overdueAccountsRadio);
            radioY += 25;

            RadioButton paymentAllocationRadio = new RadioButton
            {
                Text = "Payment Allocation Report (Interest/Principal/Penalty)",
                Location = new Point(15, radioY),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(paymentAllocationRadio);

            borderPanel.Controls.Add(reportGroup);
            panelY += 190;

            // Report Table
            paymentGrid = new DataGridView
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

            // Style the grid
            paymentGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            paymentGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            paymentGrid.EnableHeadersVisualStyles = false;
            paymentGrid.ColumnHeadersHeight = 40;
            paymentGrid.RowTemplate.Height = 35;

            // Add columns
            paymentGrid.Columns.Add("id", "ID");
            paymentGrid.Columns.Add("customer", "Customer");
            paymentGrid.Columns.Add("date", "Date Paid");
            paymentGrid.Columns.Add("amount", "Amount");
            paymentGrid.Columns.Add("type", "Type");
            paymentGrid.Columns.Add("status", "Status");

            // Load real data from database; fall back to sample data on error
            LoadPaymentDataIntoGrid(paymentGrid);

            borderPanel.Controls.Add(paymentGrid);
            panelY += 210;

            // Action Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 40),
                BackColor = Color.Transparent
            };

            Button dailyChartButton = new Button
            {
                Text = "Daily Collection Chart",
                Location = new Point(0, 0),
                Size = new Size(160, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(34, 197, 94), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            dailyChartButton.FlatAppearance.BorderSize = 0;
            dailyChartButton.Click += (s, e) =>
                MessageBox.Show("Opening daily collection chart...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button exportButton = new Button
            {
                Text = "Export",
                Location = new Point(170, 0),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat
            };
            exportButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            exportButton.FlatAppearance.BorderSize = 1;
            exportButton.Click += (s, e) =>
                MessageBox.Show("Exporting report...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            buttonPanel.Controls.Add(dailyChartButton);
            buttonPanel.Controls.Add(exportButton);
            borderPanel.Controls.Add(buttonPanel);

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                headerLabel.Width = mainContainer.Width;
                borderPanel.Width = mainContainer.Width - 20;

                // Update child controls inside border panel
                reportGroup.Width = borderPanel.Width - 40;
                paymentGrid.Width = borderPanel.Width - 40;
                buttonPanel.Width = borderPanel.Width - 40;
            };

            mainContainer.Controls.Add(borderPanel);
            this.Controls.Add(mainContainer);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Load payments from the database and populate the provided grid.
        /// Falls back to sample data on failure.
        /// </summary>
        private void LoadPaymentDataIntoGrid(DataGridView grid)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var rows = (from p in db.Payments.AsNoTracking()
                                join l in db.Loans.AsNoTracking() on p.LoanId equals l.LoanId into lj
                                from l in lj.DefaultIfEmpty()
                                join c in db.Customers.AsNoTracking() on p.CustomerId equals c.CustomerId into cj
                                from c in cj.DefaultIfEmpty()
                                select new
                                {
                                    p.PaymentId,
                                    Receipt = p.ReceiptNo,
                                    Date = p.PaymentDate,
                                    Customer = ((c != null ? (c.FirstName ?? "") : "") + " " + (c != null ? (c.LastName ?? "") : "")).Trim(),
                                    LoanNumber = l != null ? l.LoanNumber : "",
                                    Amount = p.AmountPaid,
                                    Method = p.PaymentMethod,
                                    Principal = p.PrincipalPaid,
                                    Interest = p.InterestPaid,
                                    Penalty = p.PenaltyPaid
                                })
                                .OrderByDescending(x => x.Date)
                                .ToList();

                    grid.Rows.Clear();

                    foreach (var r in rows)
                    {
                        var customerDisplay = string.IsNullOrWhiteSpace(r.Customer) ? r.LoanNumber : r.Customer;
                        var dateText = r.Date != DateTime.MinValue ? r.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : "";

                        int idx = grid.Rows.Add(
                            r.PaymentId.ToString(),
                            customerDisplay,
                            dateText,
                            $"₱{r.Amount:N2}",
                            r.Method ?? "",
                            "Recorded"
                        );

                        // Color code status as recorded (green) - you can refine logic later
                        var statusCell = grid.Rows[idx].Cells["status"];
                        statusCell.Style.BackColor = Color.FromArgb(220, 252, 231);
                        statusCell.Style.ForeColor = Color.FromArgb(21, 128, 61);
                    }

                    // Select first row if any
                    if (grid.Rows.Count > 0)
                        grid.Rows[0].Selected = true;
                }
            }
            catch
            {
                // On error, keep UI usable by falling back to sample data.
                AddSamplePaymentData();
            }
        }

        private void AddSamplePaymentData()
        {
            if (paymentGrid == null) return;

            paymentGrid.Rows.Clear();

            // Add sample payment data
            string[][] sampleData = new string[][]
            {
                new string[] { "P-101", "Juan Dela Cruz", "2024-12-25", "₱5,000.00", "Monthly", "On Time" },
                new string[] { "P-102", "Maria Santos", "2024-12-25", "₱12,500.00", "Monthly", "On Time" },
                new string[] { "P-103", "Pedro Reyes", "2024-12-24", "₱2,000.00", "Final", "Completed" },
                new string[] { "P-104", "Anna Lim", "2024-12-23", "₱15,000.00", "Monthly", "Late" },
                new string[] { "P-105", "Robert Tan", "2024-12-22", "₱7,500.00", "Partial", "On Time" },
                new string[] { "P-106", "Susan Wong", "2024-12-22", "₱3,750.00", "Monthly", "On Time" },
                new string[] { "P-107", "Michael Chen", "2024-12-21", "₱25,000.00", "Advance", "Early" },
                new string[] { "P-108", "Lisa Garcia", "2024-12-21", "₱5,000.00", "Monthly", "On Time" }
            };

            foreach (var row in sampleData)
            {
                paymentGrid.Rows.Add(row);
            }

            // Color code status
            foreach (DataGridViewRow row in paymentGrid.Rows)
            {
                if (row.Cells["status"].Value != null)
                {
                    string status = row.Cells["status"].Value.ToString();
                    if (status == "On Time" || status == "Early")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(220, 252, 231);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(21, 128, 61);
                    }
                    else if (status == "Late")
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