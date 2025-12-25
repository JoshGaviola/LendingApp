using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Reports
{
    public partial class CustomerReportsControl : UserControl
    {
        private DataGridView customerGrid;

        public CustomerReportsControl()
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
                Text = "CUSTOMER REPORTS",
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainContainer.Controls.Add(headerLabel);
            yPos += 50;

            // Orange border container
            Panel borderPanel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width - 20, 500),
                BackColor = Color.FromArgb(255, 251, 235), // Light orange background
                BorderStyle = BorderStyle.FixedSingle
            };

            // Draw orange border
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.FromArgb(249, 115, 22), 2, ButtonBorderStyle.Solid,  // Orange border
                    Color.FromArgb(249, 115, 22), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(249, 115, 22), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(249, 115, 22), 2, ButtonBorderStyle.Solid);
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

            // Draw orange border for group
            reportGroup.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, reportGroup.ClientRectangle,
                    Color.FromArgb(253, 186, 116), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(253, 186, 116), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(253, 186, 116), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(253, 186, 116), 2, ButtonBorderStyle.Solid);
            };

            int radioY = 25;

            // Radio buttons for report selection
            RadioButton topBorrowersRadio = new RadioButton
            {
                Text = "Top Borrowers (by loan amount, frequency)",
                Location = new Point(15, radioY),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            reportGroup.Controls.Add(topBorrowersRadio);
            radioY += 25;

            RadioButton blacklistedRadio = new RadioButton
            {
                Text = "Blacklisted Customers",
                Location = new Point(15, radioY),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(blacklistedRadio);
            radioY += 25;

            RadioButton paymentBehaviorRadio = new RadioButton
            {
                Text = "Customer Payment Behavior (On-time vs Late)",
                Location = new Point(15, radioY),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(paymentBehaviorRadio);
            radioY += 25;

            RadioButton creditScoreRadio = new RadioButton
            {
                Text = "Customer Credit Score Distribution",
                Location = new Point(15, radioY),
                Size = new Size(250, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(creditScoreRadio);
            radioY += 25;

            RadioButton demographicsRadio = new RadioButton
            {
                Text = "Customer Demographic Report (Age, Location, Employment)",
                Location = new Point(15, radioY),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 9)
            };
            reportGroup.Controls.Add(demographicsRadio);

            borderPanel.Controls.Add(reportGroup);
            panelY += 190;

            // Customer Analytics Table
            customerGrid = new DataGridView
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
            customerGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            customerGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            customerGrid.EnableHeadersVisualStyles = false;
            customerGrid.ColumnHeadersHeight = 40;
            customerGrid.RowTemplate.Height = 35;

            // Add columns
            customerGrid.Columns.Add("id", "ID");
            customerGrid.Columns.Add("customer", "Customer");
            customerGrid.Columns.Add("loans", "Total Loans");
            customerGrid.Columns.Add("score", "Avg Score");
            customerGrid.Columns.Add("status", "Status");
            customerGrid.Columns.Add("rank", "Rank");

            // Add sample customer data
            AddSampleCustomerData();

            borderPanel.Controls.Add(customerGrid);
            panelY += 210;

            // Action Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 40),
                BackColor = Color.Transparent
            };

            Button segmentationChartButton = new Button
            {
                Text = "Customer Segmentation Chart",
                Location = new Point(0, 0),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(249, 115, 22), // Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            segmentationChartButton.FlatAppearance.BorderSize = 0;
            segmentationChartButton.Click += (s, e) =>
                MessageBox.Show("Opening customer segmentation chart...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button exportButton = new Button
            {
                Text = "Export",
                Location = new Point(210, 0),
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

            buttonPanel.Controls.Add(segmentationChartButton);
            buttonPanel.Controls.Add(exportButton);
            borderPanel.Controls.Add(buttonPanel);

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                headerLabel.Width = mainContainer.Width;
                borderPanel.Width = mainContainer.Width - 20;

                // Update child controls inside border panel
                reportGroup.Width = borderPanel.Width - 40;
                customerGrid.Width = borderPanel.Width - 40;
                buttonPanel.Width = borderPanel.Width - 40;
            };

            mainContainer.Controls.Add(borderPanel);
            this.Controls.Add(mainContainer);

            this.ResumeLayout(false);
        }

        private void AddSampleCustomerData()
        {
            if (customerGrid == null) return;

            customerGrid.Rows.Clear();

            // Add sample customer data
            string[][] sampleData = new string[][]
            {
                new string[] { "C-101", "Juan Dela Cruz", "₱550,000.00", "85", "Good", "1" },
                new string[] { "C-102", "Maria Santos", "₱420,000.00", "92", "Excellent", "2" },
                new string[] { "C-103", "Pedro Reyes", "₱380,000.00", "78", "Fair", "3" },
                new string[] { "C-104", "Anna Lim", "₱320,000.00", "65", "Watchlist", "4" },
                new string[] { "C-105", "Robert Tan", "₱275,000.00", "88", "Good", "5" },
                new string[] { "C-106", "Susan Wong", "₱210,000.00", "95", "Excellent", "6" },
                new string[] { "C-107", "Michael Chen", "₱180,000.00", "45", "Blacklisted", "7" },
                new string[] { "C-108", "Lisa Garcia", "₱150,000.00", "82", "Good", "8" }
            };

            foreach (var row in sampleData)
            {
                customerGrid.Rows.Add(row);
            }

            // Color code status and rank
            foreach (DataGridViewRow row in customerGrid.Rows)
            {
                if (row.Cells["status"].Value != null)
                {
                    string status = row.Cells["status"].Value.ToString();
                    if (status == "Excellent" || status == "Good")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(220, 252, 231);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(21, 128, 61);
                    }
                    else if (status == "Watchlist")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(254, 249, 195);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(161, 98, 7);
                    }
                    else if (status == "Fair")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(254, 249, 195);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(161, 98, 7);
                    }
                    else if (status == "Blacklisted")
                    {
                        row.Cells["status"].Style.BackColor = Color.FromArgb(254, 226, 226);
                        row.Cells["status"].Style.ForeColor = Color.FromArgb(185, 28, 28);
                    }
                }

                // Color rank column
                if (row.Cells["rank"].Value != null)
                {
                    string rank = row.Cells["rank"].Value.ToString();
                    if (rank == "1" || rank == "2" || rank == "3")
                    {
                        row.Cells["rank"].Style.BackColor = Color.FromArgb(255, 251, 235);
                        row.Cells["rank"].Style.ForeColor = Color.FromArgb(180, 83, 9);
                        row.Cells["rank"].Style.Font = new Font(customerGrid.Font, FontStyle.Bold);
                    }
                }
            }
        }
    }
}