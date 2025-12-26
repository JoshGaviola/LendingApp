using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem.Admin
{
    public partial class SystemConfigControl : UserControl
    {
        private Panel mainPanel;
        private int currentY = 20;
        private Color primaryColor = Color.FromArgb(59, 130, 246);  // Blue
        private Color secondaryColor = Color.FromArgb(239, 246, 255);  // Light blue
        private Color accentColor = Color.FromArgb(34, 197, 94);  // Green

        public SystemConfigControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateControls();
        }

        private void CreateControls()
        {
            mainPanel.Controls.Clear();
            currentY = 20;

            int panelWidth = mainPanel.ClientSize.Width - 40;
            if (panelWidth < 400) panelWidth = 400; // Minimum width

            // Header
            AddHeader("SYSTEM CONFIGURATION", panelWidth);

            // Sections
            AddSectionBox("Interest Calculation Method", new[] { "Diminishing Balance", "Flat Rate", "Add-On Rate", "Compound Interest" }, true, panelWidth);
            AddInputSection("Financial Parameters", new[] { "Penalty Rate", "Grace Period", "Service Charge" }, new[] { "2.0", "7", "3.0" }, new[] { "%", "days", "%" }, panelWidth);
            AddInputSection("Approval Workflow", new[] { "Level 1 Max", "Level 2 Max", "Level 3 Max" }, new[] { "50000", "200000", "500000" }, new[] { "₱", "₱", "₱" }, panelWidth);
            AddInputSection("Credit Scoring Weights", new[] { "Payment History", "Credit Utilization", "Credit History", "Income Stability" }, new[] { "35", "30", "15", "20" }, new[] { "%", "%", "%", "%" }, panelWidth);
            AddCheckboxSection("User Account Features", new[] { "Borrower Accounts", "Document Management", "SMS Reminders" }, panelWidth);
            AddInputSection("Loan Defaults", new[] { "Minimum Amount", "Maximum Amount", "Available Terms" }, new[] { "5000", "500000", "6,12,18,24" }, new[] { "₱", "₱", "months" }, panelWidth);
            AddCheckboxSection("Report Export Formats", new[] { "PDF", "Excel", "CSV" }, panelWidth);

            // Buttons
            AddActionButtons(panelWidth);

            // Info Panel
            AddInfoPanel(panelWidth);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CreateControls();
        }

        private void AddHeader(string title, int width)
        {
            Panel header = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 50),
                BackColor = primaryColor,
                Padding = new Padding(10)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            header.Controls.Add(titleLabel);
            mainPanel.Controls.Add(header);
            currentY += 60;
        }

        private void AddSectionBox(string title, string[] options, bool isRadio, int width)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = "  " + title + "  ",
                Location = new Point(10, currentY),
                Size = new Size(width, (options.Length * 28) + 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            int y = 25;
            foreach (string option in options)
            {
                Control control;
                if (isRadio)
                {
                    control = new RadioButton
                    {
                        Text = option,
                        Location = new Point(20, y),
                        Size = new Size(width - 40, 24),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black
                    };
                    if (option == "Diminishing Balance")
                        ((RadioButton)control).Checked = true;
                }
                else
                {
                    control = new CheckBox
                    {
                        Text = option,
                        Location = new Point(20, y),
                        Size = new Size(width - 40, 24),
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.Black
                    };
                    if (option == "PDF" || option == "Excel")
                        ((CheckBox)control).Checked = true;
                }
                groupBox.Controls.Add(control);
                y += 28;
            }

            mainPanel.Controls.Add(groupBox);
            currentY += groupBox.Height + 10;
        }

        private void AddInputSection(string title, string[] labels, string[] defaults, string[] suffixes, int width)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = "  " + title + "  ",
                Location = new Point(10, currentY),
                Size = new Size(width, (labels.Length * 35) + 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            int inputX = Math.Max(150, width / 2 - 50);

            int y = 25;
            for (int i = 0; i < labels.Length; i++)
            {
                // Label
                Label lbl = new Label
                {
                    Text = labels[i] + ":",
                    Location = new Point(20, y),
                    Size = new Size(inputX - 30, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                groupBox.Controls.Add(lbl);

                // TextBox with border
                TextBox txt = new TextBox
                {
                    Text = defaults[i],
                    Location = new Point(inputX, y),
                    Size = new Size(100, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    BorderStyle = BorderStyle.FixedSingle
                };
                groupBox.Controls.Add(txt);

                // Suffix
                Label suffix = new Label
                {
                    Text = suffixes[i],
                    Location = new Point(inputX + 105, y),
                    Size = new Size(60, 25),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                groupBox.Controls.Add(suffix);

                y += 35;
            }

            mainPanel.Controls.Add(groupBox);
            currentY += groupBox.Height + 10;
        }

        private void AddCheckboxSection(string title, string[] options, int width)
        {
            AddSectionBox(title, options, false, width);
        }

        private void AddActionButtons(int width)
        {
            Panel buttonPanel = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 50),
                BackColor = Color.Transparent
            };

            Button saveBtn = new Button
            {
                Text = "💾 SAVE CONFIGURATION",
                Location = new Point(0, 10),
                Size = new Size(Math.Min(180, width / 2 - 10), 35),
                BackColor = accentColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveBtn.FlatAppearance.BorderSize = 0;
            saveBtn.Click += (s, e) => MessageBox.Show("Configuration saved successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            Button restoreBtn = new Button
            {
                Text = "↻ RESTORE DEFAULTS",
                Location = new Point(Math.Min(190, width / 2 + 10), 10),
                Size = new Size(Math.Min(160, width / 2 - 10), 35),
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            restoreBtn.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            restoreBtn.FlatAppearance.BorderSize = 1;
            restoreBtn.Click += (s, e) => MessageBox.Show("Defaults restored!", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            buttonPanel.Controls.Add(saveBtn);
            buttonPanel.Controls.Add(restoreBtn);
            mainPanel.Controls.Add(buttonPanel);
            currentY += 60;
        }

        private void AddInfoPanel(int width)
        {
            Panel infoPanel = new Panel
            {
                Location = new Point(10, currentY),
                Size = new Size(width, 120),
                BackColor = secondaryColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label title = new Label
            {
                Text = "⚙️ VALIDATION RULES",
                Location = new Point(0, 0),
                Size = new Size(width - 30, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            Label rules = new Label
            {
                Text = "• Percentages must be between 0-100%\n" +
                       "• All amounts must be positive values\n" +
                       "• Credit score weights must total 100%\n" +
                       "• Grace period must be positive integer\n" +
                       "• Terms: Comma-separated months",
                Location = new Point(0, 25),
                Size = new Size(width - 30, 90),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };

            infoPanel.Controls.Add(title);
            infoPanel.Controls.Add(rules);
            mainPanel.Controls.Add(infoPanel);
        }
    }
}