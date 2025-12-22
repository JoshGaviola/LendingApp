using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class ReportSelectionControl : UserControl
    {
        public event EventHandler SelectedReportChanged;

        private string _selectedReport = "Daily Collection Report";
        public string SelectedReport
        {
            get => _selectedReport;
            set
            {
                _selectedReport = value ?? "";
                SetCheckedRadio(_selectedReport);
            }
        }

        public ReportSelectionControl()
        {
            BackColor = ColorTranslator.FromHtml("#F9FAFB");
            BorderStyle = BorderStyle.FixedSingle;
            Width = 850;
            Height = 270;

            var title = new Label
            {
                Text = "📄 REPORT SELECTION",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            Controls.Add(title);

            var reports = new[]
            {
                "Daily Collection Report",
                "Payment History",
                "Released Loans",
                "Active Loans",
                "Overdue Accounts",
                "Interest Income Report",
                "Service Charge Income Report",
                "Portfolio at Risk (PAR)",
                "Customer Payment Behavior"
            };

            int y = 40;
            foreach (var r in reports)
            {
                var rb = new RadioButton
                {
                    Text = r,
                    AutoSize = true,
                    Location = new Point(30, y),
                    Checked = r == _selectedReport
                };
                rb.CheckedChanged += (s, e) =>
                {
                    if (!rb.Checked) return;
                    _selectedReport = rb.Text;
                    SelectedReportChanged?.Invoke(this, EventArgs.Empty);
                };
                Controls.Add(rb);
                y += 24;
            }
        }

        private void SetCheckedRadio(string text)
        {
            foreach (Control c in Controls)
            {
                var rb = c as RadioButton;
                if (rb == null) continue;
                rb.Checked = string.Equals(rb.Text, text, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}