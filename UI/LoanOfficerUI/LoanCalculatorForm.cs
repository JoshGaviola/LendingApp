using System;
using System.Drawing;
using System.Windows.Forms;
using LendingApp.Class.Services.Loans;
using System.Linq;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class LoanCalculatorForm : Form
    {
        private TextBox tbPrincipal;
        private TextBox tbRate;
        private ComboBox cbTerm;
        private TextBox tbProcessing;
        private ComboBox cbMethod;
        private Button btnCalculate;
        private Button btnReset;
        private Label lblMonthly;
        private Label lblTotalPayable;
        private Label lblTotalInterest;
        private Label lblServiceCharge;
        private DataGridView amortGrid;

        public LoanCalculatorForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Loan Calculator";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(700, 500);
            this.BackColor = Color.White;
            this.Padding = new Padding(20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void BuildUI()
        {
            var header = new Label
            {
                Text = "LOAN CALCULATOR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            Controls.Add(header);

            // Main container
            var mainPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(640, 380),
                BackColor = Color.White
            };
            Controls.Add(mainPanel);

            // Left panel - inputs
            var leftPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(300, 380),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };
            mainPanel.Controls.Add(leftPanel);

            int y = 0;

            // Input fields with labels
            AddInputField(leftPanel, "Loan Amount (₱)", "50000.00", ref y, out tbPrincipal);
            AddInputField(leftPanel, "Annual Interest Rate (%)", "12.0", ref y, out tbRate);

            // Term dropdown
            var termLabel = new Label
            {
                Text = "Loan Term (months)",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            leftPanel.Controls.Add(termLabel);
            y += 22;

            cbTerm = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(0, y),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 9)
            };
            cbTerm.Items.AddRange(new object[] { "6", "12", "18", "24" });
            cbTerm.SelectedItem = "12";
            leftPanel.Controls.Add(cbTerm);
            y += 35;

            AddInputField(leftPanel, "Processing Fee (%)", "3.0", ref y, out tbProcessing);

            // Method dropdown
            var methodLabel = new Label
            {
                Text = "Calculation Method",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            leftPanel.Controls.Add(methodLabel);
            y += 22;

            cbMethod = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(0, y),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 9)
            };
            cbMethod.Items.AddRange(new object[] { "Diminishing Balance", "Flat Rate" });
            cbMethod.SelectedItem = "Diminishing Balance";
            leftPanel.Controls.Add(cbMethod);
            y += 45;

            // Calculate button
            btnCalculate = new Button
            {
                Text = "CALCULATE",
                Location = new Point(0, y),
                Size = new Size(140, 35),
                BackColor = ColorTranslator.FromHtml("#10B981"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            leftPanel.Controls.Add(btnCalculate);

            // Reset button
            btnReset = new Button
            {
                Text = "RESET",
                Location = new Point(150, y),
                Size = new Size(140, 35),
                BackColor = ColorTranslator.FromHtml("#6B7280"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            leftPanel.Controls.Add(btnReset);

            // Right panel - results
            var rightPanel = new Panel
            {
                Location = new Point(320, 0),
                Size = new Size(320, 380),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };
            mainPanel.Controls.Add(rightPanel);

            // Results section
            var resultsLabel = new Label
            {
                Text = "CALCULATION RESULTS",
                Location = new Point(0, 0),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            rightPanel.Controls.Add(resultsLabel);

            int ry = 30;

            // Results grid
            var resultsTable = new TableLayoutPanel
            {
                Location = new Point(0, ry),
                Size = new Size(320, 140),
                ColumnCount = 2,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            resultsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            resultsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            AddResultRow(resultsTable, "Monthly Payment:", "₱0.00", 0, out lblMonthly);
            AddResultRow(resultsTable, "Total Payable:", "₱0.00", 1, out lblTotalPayable);
            AddResultRow(resultsTable, "Total Interest:", "₱0.00", 2, out lblTotalInterest);
            AddResultRow(resultsTable, "Service Charge:", "₱0.00", 3, out lblServiceCharge);

            rightPanel.Controls.Add(resultsTable);
            ry += 150;

            // Amortization section
            var amortLabel = new Label
            {
                Text = "AMORTIZATION SCHEDULE (First 3 months)",
                Location = new Point(0, ry),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            rightPanel.Controls.Add(amortLabel);
            ry += 25;

            amortGrid = new DataGridView
            {
                Location = new Point(0, ry),
                Size = new Size(320, 180),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 8)
            };
            amortGrid.Columns.Add("Month", "Month");
            amortGrid.Columns.Add("Payment", "Payment");
            amortGrid.Columns.Add("Principal", "Principal");
            amortGrid.Columns.Add("Interest", "Interest");
            amortGrid.Columns.Add("Balance", "Balance");
            rightPanel.Controls.Add(amortGrid);

            // Wire handlers
            btnCalculate.Click += BtnCalculate_Click;
            btnReset.Click += BtnReset_Click;
        }

        private void AddInputField(Panel panel, string labelText, string defaultValue, ref int y, out TextBox textBox)
        {
            var label = new Label
            {
                Text = labelText,
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            panel.Controls.Add(label);
            y += 22;

            textBox = new TextBox
            {
                Text = defaultValue,
                Location = new Point(0, y),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 9)
            };
            panel.Controls.Add(textBox);
            y += 35;
        }

        private void AddResultRow(TableLayoutPanel table, string labelText, string valueText, int row, out Label valueLabel)
        {
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151")
            };
            table.Controls.Add(label, 0, row);

            valueLabel = new Label
            {
                Text = valueText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            table.Controls.Add(valueLabel, 1, row);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            tbPrincipal.Text = "50000.00";
            tbRate.Text = "12.0";
            cbTerm.SelectedItem = "12";
            tbProcessing.Text = "3.0";
            cbMethod.SelectedItem = "Diminishing Balance";

            amortGrid.Rows.Clear();
            lblMonthly.Text = "₱0.00";
            lblTotalPayable.Text = "₱0.00";
            lblTotalInterest.Text = "₱0.00";
            lblServiceCharge.Text = "₱0.00";
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(tbPrincipal.Text, out decimal principal) || principal <= 0)
            {
                MessageBox.Show("Enter a valid principal amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPrincipal.Focus();
                return;
            }

            if (!double.TryParse(tbRate.Text, out double annualRate) || annualRate < 0)
            {
                MessageBox.Show("Enter a valid annual interest rate.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbRate.Focus();
                return;
            }

            if (!int.TryParse(cbTerm.SelectedItem?.ToString() ?? "12", out int termMonths) || termMonths <= 0)
            {
                MessageBox.Show("Select a valid term.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(tbProcessing.Text, out double procFeePct) || procFeePct < 0)
            {
                MessageBox.Show("Enter a valid processing fee percent.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbProcessing.Focus();
                return;
            }

            // Map UI method to enum
            LoanInterestMethod method = cbMethod.SelectedItem?.ToString() == "Flat Rate"
                ? LoanInterestMethod.FlatRate
                : LoanInterestMethod.DiminishingBalance;

            // Use the computation service for business logic
            var result = LoanComputationService.Calculate(
                principal: principal,
                annualRatePct: (decimal)annualRate,
                termMonths: termMonths,
                serviceFeePct: (decimal)procFeePct,
                method: method);

            lblMonthly.Text = $"₱{result.MonthlyPayment:N2}";
            lblTotalPayable.Text = $"₱{result.TotalPayable:N2}";
            lblTotalInterest.Text = $"₱{result.TotalInterest:N2}";
            lblServiceCharge.Text = $"₱{result.ServiceFeeAmount:N2}";

            // Get full amortization schedule from service and show first 3 rows in UI
            var schedule = LoanComputationService.GetAmortizationSchedule(
                principal: principal,
                annualRatePct: (decimal)annualRate,
                termMonths: termMonths,
                serviceFeePct: (decimal)procFeePct,
                method: method);

            amortGrid.Rows.Clear();

            foreach (var row in schedule.Take(3))
            {
                amortGrid.Rows.Add(
                    row.Month.ToString(),
                    $"₱{row.Payment:N2}",
                    $"₱{row.Principal:N2}",
                    $"₱{row.Interest:N2}",
                    $"₱{row.Balance:N2}"
                );
            }
        }
    }
}