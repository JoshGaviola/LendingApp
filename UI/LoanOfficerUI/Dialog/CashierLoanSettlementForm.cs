using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoanApplicationUI
{
    public partial class CashierLoanSettlementForm : Form
    {
        private SettlementData settlementData;

        public SettlementData SettlementData
        {
            get => settlementData;
            set
            {
                settlementData = value;
                UpdateSettlementInfo();
            }
        }

        // Labels for dynamic data
        private Label lblLoanNumber, lblCustomer, lblOriginalAmount, lblCurrentBalance, lblRemainingPayments;
        private Label lblOutstandingBalance, lblDiscountAmount, lblSavings, lblTotalAmount;
        private NumericUpDown numDiscount, numSettlementAmount;
        private TextBox txtRemarks;
        private RadioButton radCash, radGCash, radBank;

        public CashierLoanSettlementForm()
        {
            InitializeUI();
            Size = new Size(900, 700); // Increased width
            StartPosition = FormStartPosition.CenterParent;
        }

        private void InitializeUI()
        {
            Text = "Loan Settlement";
            BackColor = Color.White;
            Padding = new Padding(20);
            AutoScroll = true;

            // Main container - FIXED SCROLLING
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 0, 20, 0) // Right padding for scrollbar
            };

            // Content panel inside main panel
            var contentPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Width = 820
            };

            // 1. Banner
            contentPanel.Controls.Add(CreateBanner());

            // 2. Summary Section
            contentPanel.Controls.Add(CreateSectionPanel("Loan Summary", CreateSummaryTable(), 140));

            // 3. Calculation Section
            contentPanel.Controls.Add(CreateSectionPanel("Settlement Calculation", CreateCalculationTable(), 180));

            // 4. Payment Section (REMOVED PRINT OPTIONS)
            contentPanel.Controls.Add(CreateSectionPanel("Payment Details", CreatePaymentTable(), 200));

            // 5. Action Section
            contentPanel.Controls.Add(CreateSectionPanel("This action will:", CreateActionList(), 230));

            // 6. Buttons
            contentPanel.Controls.Add(CreateButtonPanel());

            mainPanel.Controls.Add(contentPanel);
            Controls.Add(mainPanel);
        }

        private Panel CreateBanner()
        {
            return new Panel
            {
                Height = 70,
                Width = 800,
                BackColor = Color.FromArgb(235, 255, 235),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 20)
            }.AddLabel("Early Loan Settlement",
                FontStyle.Bold, 14, Color.DarkGreen, ContentAlignment.MiddleCenter);
        }

        private Panel CreateSectionPanel(string title, Control content, int height)
        {
            var panel = new Panel
            {
                Width = 800,
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Title
            panel.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(300, 25)
            });

            // Content
            content.Location = new Point(15, 40);
            panel.Controls.Add(content);

            return panel;
        }

        private TableLayoutPanel CreateSummaryTable()
        {
            var table = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 5,
                Width = 770,
                Height = 100
            };

            // Configure columns
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Configure rows
            for (int i = 0; i < 5; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // Create labels
            lblLoanNumber = new Label { Text = "", Height = 20 };
            lblCustomer = new Label { Text = "", Height = 20 };
            lblOriginalAmount = new Label { Text = "", Height = 20 };
            lblCurrentBalance = new Label { Text = "", Height = 20, ForeColor = Color.OrangeRed };
            lblRemainingPayments = new Label { Text = "", Height = 20 };

            // Add rows
            AddTableRow(table, "Loan Number:", lblLoanNumber, 0);
            AddTableRow(table, "Customer:", lblCustomer, 1);
            AddTableRow(table, "Original Loan Amount:", lblOriginalAmount, 2);
            AddTableRow(table, "Current Balance:", lblCurrentBalance, 3);
            AddTableRow(table, "Remaining Payments:", lblRemainingPayments, 4);

            return table;
        }

        private TableLayoutPanel CreateCalculationTable()
        {
            var table = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 5,
                Width = 770,
                Height = 140
            };

            // Configure columns
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Configure rows
            for (int i = 0; i < 5; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // Create controls
            numDiscount = new NumericUpDown
            {
                Value = 2.5m,
                DecimalPlaces = 1,
                Minimum = 0,
                Maximum = 10,
                Width = 90,
                Height = 28
            };

            var btnRecalc = new Button
            {
                Text = "Recalculate",
                Width = 110,
                Height = 28,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRecalc.Click += (s, e) => Recalculate();

            lblOutstandingBalance = new Label { Text = "₱0.00", Height = 28 };
            lblDiscountAmount = new Label { Text = "₱0.00", Height = 28, ForeColor = Color.Green };
            lblSavings = new Label { Text = "₱0.00", Height = 28, ForeColor = Color.Green };
            lblTotalAmount = new Label { Text = "₱0.00", Height = 30, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.DarkGreen };

            // Add rows
            table.Controls.Add(new Label { Text = "Early Payment Discount (%):", Height = 28 }, 0, 0);
            table.Controls.Add(numDiscount, 1, 0);
            table.Controls.Add(btnRecalc, 2, 0);

            AddTableRow(table, "Outstanding Balance:", lblOutstandingBalance, 1, 2);
            AddTableRow(table, "Discount (2.5%):", lblDiscountAmount, 2, 2);
            AddTableRow(table, "Savings:", lblSavings, 3, 2);

            var totalLabel = new Label { Text = "Total Settlement Amount:", Height = 30, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            table.Controls.Add(totalLabel, 0, 4);
            table.SetColumnSpan(totalLabel, 2);
            table.Controls.Add(lblTotalAmount, 2, 4);

            numDiscount.ValueChanged += (s, e) => Recalculate();

            return table;
        }

        private TableLayoutPanel CreatePaymentTable()
        {
            var table = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3, // CHANGED from 4 to 3 (removed print options row)
                Width = 770,
                Height = 150 // Reduced height since we removed print options
            };

            // Configure columns
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Configure rows - REMOVED PRINT OPTIONS ROW
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Settlement Amount
            numSettlementAmount = new NumericUpDown
            {
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000,
                Width = 220,
                Height = 34,
                Font = new Font("Segoe UI", 10)
            };

            // Payment Method
            var methodPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Height = 35
            };
            radCash = new RadioButton { Text = "Cash", Checked = true, Width = 70, Height = 28, Font = new Font("Segoe UI", 9) };
            radGCash = new RadioButton { Text = "GCash", Width = 80, Height = 28, Font = new Font("Segoe UI", 9) };
            radBank = new RadioButton { Text = "Bank", Width = 70, Height = 28, Font = new Font("Segoe UI", 9) };
            methodPanel.Controls.AddRange(new Control[] { radCash, radGCash, radBank });

            // Remarks
            txtRemarks = new TextBox
            {
                Multiline = true,
                Width = 580,
                Height = 80, // Increased height for better visibility
                Font = new Font("Segoe UI", 9),
                Text = "Enter any additional notes about this settlement...",
                ScrollBars = ScrollBars.Vertical
            };

            // Add to table - NO PRINT OPTIONS
            AddTableRow(table, "Settlement Amount *", numSettlementAmount, 0);
            AddTableRow(table, "Payment Method", methodPanel, 1);
            AddTableRow(table, "Remarks / Notes", txtRemarks, 2);

            return table;
        }

        private FlowLayoutPanel CreateActionList()
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Width = 770,
                Height = 160
            };

            string[] actions =
            {
                "Record final settlement/closure transaction",
                "Update loan status to \"Fully Paid / Closed\"",
                "Print loan clearance certificate for customer",
                "Print loan clearance certificate for records",
                "Generate final settlement receipt",
                "Archive loan account and release collateral (if any)"
            };

            foreach (var action in actions)
            {
                var item = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    Height = 28,
                    Margin = new Padding(0, 3, 0, 3)
                };
                var checkLabel = new Label
                {
                    Text = "✓",
                    ForeColor = Color.Green,
                    Width = 25,
                    Height = 24,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                var textLabel = new Label
                {
                    Text = action,
                    AutoSize = true,
                    Height = 24,
                    Font = new Font("Segoe UI", 9),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 0, 0, 0)
                };
                item.Controls.Add(checkLabel);
                item.Controls.Add(textLabel);
                panel.Controls.Add(item);
            }

            return panel;
        }

        private FlowLayoutPanel CreateButtonPanel()
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Width = 800,
                Height = 50,
                Margin = new Padding(0, 20, 0, 10)
            };

            var btnPreviewClearance = new Button
            {
                Text = "Preview Clearance Certificate",
                Width = 220,
                Height = 38,
                BackColor = Color.FromArgb(66, 133, 244),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };

            var btnPreviewClosure = new Button
            {
                Text = "Preview Closure Certificate",
                Width = 220,
                Height = 38,
                BackColor = Color.FromArgb(66, 133, 244),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(15, 0, 0, 0)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Width = 110,
                Height = 38,
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Margin = new Padding(100, 0, 0, 0)
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            var btnConfirm = new Button
            {
                Text = "Confirm Settlement",
                Width = 160,
                Height = 38,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(15, 0, 0, 0)
            };
            btnConfirm.Click += (s, e) => ConfirmSettlement();

            panel.Controls.Add(btnPreviewClearance);
            panel.Controls.Add(btnPreviewClosure);
            panel.Controls.Add(btnCancel);
            panel.Controls.Add(btnConfirm);

            return panel;
        }

        private void AddTableRow(TableLayoutPanel table, string label, Control control, int row, int colSpan = 1)
        {
            table.Controls.Add(new Label
            {
                Text = label,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, row);

            if (control is Label labelControl)
                labelControl.TextAlign = ContentAlignment.MiddleLeft;

            table.Controls.Add(control, 1, row);
            if (colSpan > 1) table.SetColumnSpan(control, colSpan);
        }

        private void UpdateSettlementInfo()
        {
            if (settlementData == null) return;

            lblLoanNumber.Text = settlementData.LoanNumber;
            lblCustomer.Text = settlementData.Customer;
            lblOriginalAmount.Text = $"₱{settlementData.OriginalBalance:N2}";
            lblCurrentBalance.Text = $"₱{settlementData.CurrentBalance:N2}";
            lblRemainingPayments.Text = $"{settlementData.PaymentsRemaining} months";

            Recalculate();
        }

        private void Recalculate()
        {
            if (settlementData == null) return;

            decimal discountPercent = numDiscount.Value;
            decimal discountAmount = (settlementData.CurrentBalance * discountPercent) / 100;
            decimal totalAmount = settlementData.CurrentBalance - discountAmount;

            lblOutstandingBalance.Text = $"₱{settlementData.CurrentBalance:N2}";
            lblDiscountAmount.Text = $"- ₱{discountAmount:N2}";
            lblSavings.Text = $"₱{discountAmount:N2}";
            lblTotalAmount.Text = $"₱{totalAmount:N2}";

            numSettlementAmount.Value = totalAmount;
        }

        private void ConfirmSettlement()
        {
            if (numSettlementAmount.Value <= 0)
            {
                MessageBox.Show("Please enter a valid settlement amount", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // Add these public accessors to expose chosen settlement values for callers.
        public decimal SelectedSettlementAmount
        {
            get => numSettlementAmount?.Value ?? 0m;
        }

        public string SelectedPaymentMethod
        {
            get
            {
                if (radGCash != null && radGCash.Checked) return "GCash";
                if (radBank != null && radBank.Checked) return "Bank";
                return "Cash";
            }
        }

        public string RemarksText => txtRemarks?.Text ?? "";
    }

    public class SettlementData
    {
        public string LoanNumber { get; set; }
        public string Customer { get; set; }
        public decimal OriginalBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public int PaymentsRemaining { get; set; }
        public bool IsFinalPayment { get; set; }
    }

    // Extension method for cleaner code
    public static class ControlExtensions
    {
        public static Panel AddLabel(this Panel panel, string text, FontStyle style,
            int fontSize, Color color, ContentAlignment alignment)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", fontSize, style),
                ForeColor = color,
                Dock = DockStyle.Fill,
                TextAlign = alignment
            };
            panel.Controls.Add(label);
            return panel;
        }
    }
}