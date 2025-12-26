using System;
using System.Drawing;
using System.Windows.Forms;

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

            // Add sample financial data
            AddSampleFinancialData();

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