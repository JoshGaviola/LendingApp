using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class AuditLogControl : UserControl
    {
        // Date controls
        private DateTimePicker dateFromPicker;
        private DateTimePicker dateToPicker;
        private Button filterButton;

        // Table
        private DataGridView auditLogGrid;

        // Details panel
        private Panel detailsPanel;
        private Label detailsLabel;
        private Label selectedIdLabel;
        private Label selectedDateLabel;
        private Label selectedTimeLabel;
        private Label selectedAdminLabel;
        private Label selectedActionLabel;
        private Label selectedTargetLabel;
        private Label selectedReasonLabel;
        private Label selectedDetailsLabel;

        // Empty state
        private Panel emptyStatePanel;
        private Label emptyStateLabel;

        // Action buttons
        private Button exportButton;
        private Button printButton;
        private Button viewDetailsButton;

        // Main container
        private Panel mainContainer;

        public AuditLogControl()
        {
            InitializeComponent();
            InitializeSampleData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            // Main container with AutoScroll
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            int yPos = 20;

            // Header
            Label headerLabel = new Label
            {
                Text = "AUDIT LOG",
                Location = new Point(20, yPos),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 33, 168),
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainContainer.Controls.Add(headerLabel);
            yPos += 40;

            // Border panel - DYNAMIC WIDTH
            Panel borderPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(Math.Max(800, mainContainer.Width - 40), 600), // Dynamic minimum width
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Draw custom border
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(168, 85, 247), 2, ButtonBorderStyle.Solid);
            };

            int panelY = 20;

            // Date Range Section - DYNAMIC POSITIONING
            Label dateRangeLabel = new Label
            {
                Text = "Date Range:",
                Location = new Point(20, panelY),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            dateFromPicker = new DateTimePicker
            {
                Location = new Point(105, panelY),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            Label toLabel = new Label
            {
                Text = "to",
                Location = new Point(230, panelY),
                Size = new Size(20, 25),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            dateToPicker = new DateTimePicker
            {
                Location = new Point(255, panelY),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            filterButton = new Button
            {
                Text = "Filter",
                Location = new Point(380, panelY),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            filterButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            filterButton.FlatAppearance.BorderSize = 1;
            filterButton.Click += FilterButton_Click;

            borderPanel.Controls.Add(dateRangeLabel);
            borderPanel.Controls.Add(dateFromPicker);
            borderPanel.Controls.Add(toLabel);
            borderPanel.Controls.Add(dateToPicker);
            borderPanel.Controls.Add(filterButton);
            panelY += 40;

            // Audit Log Table - DYNAMIC WIDTH
            auditLogGrid = new DataGridView
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Fill available space
                Font = new Font("Segoe UI", 8.5f),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Style the grid
            auditLogGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            auditLogGrid.EnableHeadersVisualStyles = false;
            auditLogGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            auditLogGrid.ColumnHeadersHeight = 40;
            auditLogGrid.RowTemplate.Height = 35;

            // Add columns with dynamic sizing
            auditLogGrid.Columns.Add("id", "ID");
            auditLogGrid.Columns.Add("date", "Date");
            auditLogGrid.Columns.Add("time", "Time");
            auditLogGrid.Columns.Add("admin", "Admin");
            auditLogGrid.Columns.Add("action", "Action");
            auditLogGrid.Columns.Add("target", "Target");
            auditLogGrid.Columns.Add("reason", "Reason");
            auditLogGrid.Columns.Add("details", "Details");

            // Set minimum column widths
            auditLogGrid.Columns["id"].MinimumWidth = 60;
            auditLogGrid.Columns["date"].MinimumWidth = 80;
            auditLogGrid.Columns["time"].MinimumWidth = 80;
            auditLogGrid.Columns["admin"].MinimumWidth = 100;
            auditLogGrid.Columns["action"].MinimumWidth = 120;
            auditLogGrid.Columns["target"].MinimumWidth = 100;
            auditLogGrid.Columns["reason"].MinimumWidth = 150;
            auditLogGrid.Columns["details"].MinimumWidth = 200;

            // Row click event
            auditLogGrid.SelectionChanged += AuditLogGrid_SelectionChanged;

            borderPanel.Controls.Add(auditLogGrid);
            panelY += 210;

            // Details Panel - DYNAMIC WIDTH
            detailsPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 180),
                BackColor = Color.FromArgb(250, 245, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Draw purple border
            detailsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, detailsPanel.ClientRectangle,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(192, 132, 252), 2, ButtonBorderStyle.Solid);
            };

            int detailsY = 15;

            // Details header
            detailsLabel = new Label
            {
                Text = "SELECTED AUDIT LOG DETAILS:",
                Location = new Point(15, detailsY),
                Size = new Size(detailsPanel.Width - 30, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            detailsPanel.Controls.Add(detailsLabel);
            detailsY += 25;

            // Details content panel
            Panel detailsContent = new Panel
            {
                Location = new Point(15, detailsY),
                Size = new Size(detailsPanel.Width - 30, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Draw light purple border
            detailsContent.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, detailsContent.ClientRectangle,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid);
            };

            // Create labels for details with dynamic positioning
            int contentY = 10;
            int labelWidth = (detailsContent.Width / 2) - 20;

            selectedIdLabel = CreateDetailLabel("ID:", "", 15, contentY, labelWidth);
            detailsContent.Controls.Add(selectedIdLabel);
            contentY += 20;

            selectedDateLabel = CreateDetailLabel("Date:", "", 15, contentY, labelWidth);
            detailsContent.Controls.Add(selectedDateLabel);
            contentY += 20;

            selectedTimeLabel = CreateDetailLabel("Time:", "", 15, contentY, labelWidth);
            detailsContent.Controls.Add(selectedTimeLabel);
            contentY += 20;

            selectedAdminLabel = CreateDetailLabel("Admin:", "", 15, contentY, labelWidth);
            detailsContent.Controls.Add(selectedAdminLabel);

            // Second column
            contentY = 10;
            int secondColX = (detailsContent.Width / 2) + 5;

            selectedActionLabel = CreateDetailLabel("Action:", "", secondColX, contentY, labelWidth);
            detailsContent.Controls.Add(selectedActionLabel);
            contentY += 20;

            selectedTargetLabel = CreateDetailLabel("Target:", "", secondColX, contentY, labelWidth);
            detailsContent.Controls.Add(selectedTargetLabel);
            contentY += 20;

            selectedReasonLabel = CreateDetailLabel("Reason:", "", secondColX, contentY, labelWidth);
            detailsContent.Controls.Add(selectedReasonLabel);
            contentY += 20;

            selectedDetailsLabel = CreateDetailLabel("Details:", "", secondColX, contentY, labelWidth);
            detailsContent.Controls.Add(selectedDetailsLabel);

            detailsPanel.Controls.Add(detailsContent);
            borderPanel.Controls.Add(detailsPanel);
            panelY += 190;

            // Empty state panel - DYNAMIC WIDTH
            emptyStatePanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 80),
                BackColor = Color.FromArgb(250, 245, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Draw light purple border
            emptyStatePanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, emptyStatePanel.ClientRectangle,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(233, 213, 255), 1, ButtonBorderStyle.Solid);
            };

            emptyStateLabel = new Label
            {
                Text = "Select an audit log entry from the table above to view details",
                Location = new Point(20, 25),
                Size = new Size(emptyStatePanel.Width - 40, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(75, 85, 99),
                TextAlign = ContentAlignment.MiddleCenter
            };
            emptyStatePanel.Controls.Add(emptyStateLabel);
            borderPanel.Controls.Add(emptyStatePanel);
            panelY += 90;

            // Action Buttons - DYNAMIC POSITIONING
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, panelY),
                Size = new Size(borderPanel.Width - 40, 40),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            exportButton = new Button
            {
                Text = "Export to CSV",
                Location = new Point(0, 0),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exportButton.FlatAppearance.BorderSize = 0;
            exportButton.Click += ExportButton_Click;

            printButton = new Button
            {
                Text = "Print Report",
                Location = new Point(130, 0),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat
            };
            printButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            printButton.FlatAppearance.BorderSize = 1;
            printButton.Click += PrintButton_Click;

            viewDetailsButton = new Button
            {
                Text = "View Details",
                Location = new Point(240, 0),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat
            };
            viewDetailsButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            viewDetailsButton.FlatAppearance.BorderSize = 1;
            viewDetailsButton.Click += ViewDetailsButton_Click;

            buttonPanel.Controls.Add(exportButton);
            buttonPanel.Controls.Add(printButton);
            buttonPanel.Controls.Add(viewDetailsButton);
            borderPanel.Controls.Add(buttonPanel);

            // Handle resize of the main container
            mainContainer.Resize += (s, e) =>
            {
                UpdateLayout(mainContainer.Width);
            };

            mainContainer.Controls.Add(borderPanel);
            this.Controls.Add(mainContainer);

            this.ResumeLayout(false);
        }

        private void UpdateLayout(int containerWidth)
        {
            if (mainContainer == null) return;

            // Update border panel width
            foreach (Control control in mainContainer.Controls)
            {
                if (control is Panel borderPanel)
                {
                    // Update border panel width
                    borderPanel.Width = Math.Max(800, containerWidth - 40);

                    // Update all child controls that need dynamic width
                    foreach (Control child in borderPanel.Controls)
                    {
                        if (child is DataGridView grid)
                        {
                            grid.Width = borderPanel.Width - 40;
                            grid.PerformLayout(); // Force layout update
                        }
                        else if (child is Panel panel && (panel.Name == "detailsPanel" || panel.Name == "emptyStatePanel"))
                        {
                            panel.Width = borderPanel.Width - 40;

                            // Update details content if this is the details panel
                            if (panel == detailsPanel && panel.Controls.Count > 1)
                            {
                                // Update details content panel
                                Control detailsContent = panel.Controls[1]; // Should be the content panel
                                if (detailsContent != null)
                                {
                                    detailsContent.Width = panel.Width - 30;

                                    // Update label positions in the content panel
                                    UpdateDetailsPanelLayout(detailsContent);
                                }
                            }
                        }
                        else if (child is Panel buttonPanel && buttonPanel.Controls.Count == 3)
                        {
                            buttonPanel.Width = borderPanel.Width - 40;
                        }
                    }
                    break;
                }
            }
        }

        private void UpdateDetailsPanelLayout(Control detailsContent)
        {
            // Recalculate column widths
            int labelWidth = (detailsContent.Width / 2) - 20;
            int secondColX = (detailsContent.Width / 2) + 5;

            // Update all labels in the details content
            foreach (Control label in detailsContent.Controls)
            {
                if (label is Label detailLabel)
                {
                    // Determine which column it belongs to based on its text
                    if (detailLabel.Text.StartsWith("ID:") ||
                        detailLabel.Text.StartsWith("Date:") ||
                        detailLabel.Text.StartsWith("Time:") ||
                        detailLabel.Text.StartsWith("Admin:"))
                    {
                        // First column
                        detailLabel.Width = labelWidth;
                    }
                    else
                    {
                        // Second column
                        detailLabel.Left = secondColX;
                        detailLabel.Width = labelWidth;
                    }
                }
            }
        }

        private Label CreateDetailLabel(string labelText, string valueText, int x, int y, int width)
        {
            Label label = new Label
            {
                Location = new Point(x, y),
                Size = new Size(width, 20),
                Font = new Font("Segoe UI", 9),
                Text = $"{labelText} {valueText}"
            };

            return label;
        }

        private void InitializeSampleData()
        {
            if (auditLogGrid == null) return;

            // Clear existing rows
            auditLogGrid.Rows.Clear();

            // Add sample data
            string[][] sampleData = new string[][]
            {
                new string[] { "001", "2024-12-25", "10:30 AM", "Admin User", "System Override", "Loan Limits", "Emergency maintenance", "Bypassed credit check for customer" },
                new string[] { "002", "2024-12-24", "02:15 PM", "Super Admin", "User Access", "User Permissions", "New employee onboarding", "Granted full access to new staff" },
                new string[] { "003", "2024-12-23", "09:45 AM", "System Admin", "Data Export", "Customer Records", "Monthly backup", "Exported all customer records" },
                new string[] { "004", "2024-12-22", "04:20 PM", "Admin User", "System Update", "Interest Rates", "Rate adjustment", "Updated interest rates by 0.5%" },
                new string[] { "005", "2024-12-21", "11:10 AM", "Audit Manager", "Report Generation", "Audit Reports", "Quarterly audit", "Generated compliance report" }
            };

            foreach (var row in sampleData)
            {
                auditLogGrid.Rows.Add(row);
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            // Simple filter implementation
            MessageBox.Show($"Filtering from {dateFromPicker.Value.ToShortDateString()} to {dateToPicker.Value.ToShortDateString()}",
                "Filter Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AuditLogGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (auditLogGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = auditLogGrid.SelectedRows[0];

                // Update details labels
                selectedIdLabel.Text = $"ID: {selectedRow.Cells["id"].Value}";
                selectedDateLabel.Text = $"Date: {selectedRow.Cells["date"].Value}";
                selectedTimeLabel.Text = $"Time: {selectedRow.Cells["time"].Value}";
                selectedAdminLabel.Text = $"Admin: {selectedRow.Cells["admin"].Value}";
                selectedActionLabel.Text = $"Action: {selectedRow.Cells["action"].Value}";
                selectedTargetLabel.Text = $"Target: {selectedRow.Cells["target"].Value}";
                selectedReasonLabel.Text = $"Reason: \"{selectedRow.Cells["reason"].Value}\"";
                selectedDetailsLabel.Text = $"Details: \"{selectedRow.Cells["details"].Value}\"";

                // Show details panel, hide empty state
                detailsPanel.Visible = true;
                emptyStatePanel.Visible = false;

                // Highlight selected row
                foreach (DataGridViewRow row in auditLogGrid.Rows)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
                selectedRow.DefaultCellStyle.BackColor = Color.FromArgb(250, 245, 255);
            }
            else
            {
                // Show empty state, hide details
                detailsPanel.Visible = false;
                emptyStatePanel.Visible = true;
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exporting audit log to CSV...", "Export",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Preparing audit log report for printing...", "Print Report",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewDetailsButton_Click(object sender, EventArgs e)
        {
            if (auditLogGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = auditLogGrid.SelectedRows[0];
                string logId = selectedRow.Cells["id"].Value.ToString();
                MessageBox.Show($"Viewing details for audit log #{logId}", "View Details",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an audit log entry first", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Helper method to update details
        public void UpdateDetails(string id, string date, string time, string admin,
                                 string action, string target, string reason, string details)
        {
            selectedIdLabel.Text = $"ID: {id}";
            selectedDateLabel.Text = $"Date: {date}";
            selectedTimeLabel.Text = $"Time: {time}";
            selectedAdminLabel.Text = $"Admin: {admin}";
            selectedActionLabel.Text = $"Action: {action}";
            selectedTargetLabel.Text = $"Target: {target}";
            selectedReasonLabel.Text = $"Reason: \"{reason}\"";
            selectedDetailsLabel.Text = $"Details: \"{details}\"";
        }
    }
}