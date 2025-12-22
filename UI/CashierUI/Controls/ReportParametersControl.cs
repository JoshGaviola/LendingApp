using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI.Controls
{
    public class ReportParametersControl : UserControl
    {
        public event EventHandler ApplyClicked;
        public event EventHandler ResetClicked;

        public DateTime DateFrom
        {
            get => dtpFrom.Value.Date;
            set => dtpFrom.Value = value;
        }

        public DateTime DateTo
        {
            get => dtpTo.Value.Date;
            set => dtpTo.Value = value;
        }

        public string SelectedCashier => cmbCashier.SelectedItem?.ToString() ?? "All";
        public string SelectedPaymentMode => cmbPaymentMode.SelectedItem?.ToString() ?? "All";
        public string SelectedLoanType => cmbLoanType.SelectedItem?.ToString() ?? "All";

        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private ComboBox cmbCashier;
        private ComboBox cmbPaymentMode;
        private ComboBox cmbLoanType;

        public ReportParametersControl()
        {
            BackColor = ColorTranslator.FromHtml("#F9FAFB");
            BorderStyle = BorderStyle.FixedSingle;
            Width = 850;
            Height = 140;

            var title = new Label
            {
                Text = "🔧 PARAMETERS",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 12)
            };
            Controls.Add(title);

            AddLabel("Date From", 20, 40);
            dtpFrom = AddDateTimePicker(20, 60);

            AddLabel("Date To", 160, 40);
            dtpTo = AddDateTimePicker(160, 60);

            AddLabel("Cashier", 300, 40);
            cmbCashier = AddComboBox(300, 60, new[] { "All", "Maria Santos", "Juan Dela Cruz" });

            AddLabel("Payment Mode", 460, 40);
            cmbPaymentMode = AddComboBox(460, 60, new[] { "All", "Cash", "GCash", "Bank" });

            AddLabel("Loan Type", 620, 40);
            cmbLoanType = AddComboBox(620, 60, new[] { "All", "Personal Loan", "Emergency Loan", "Salary Loan" });

            var btnApply = CreateButton("🔍 Apply", 80, ColorTranslator.FromHtml("#2563EB"), Color.White);
            btnApply.Location = new Point(620, 96);
            btnApply.Click += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);
            Controls.Add(btnApply);

            var btnReset = CreateButton("Reset", 80, Color.White, ColorTranslator.FromHtml("#374151"));
            btnReset.Location = new Point(710, 96);
            btnReset.Click += (s, e) => ResetClicked?.Invoke(this, EventArgs.Empty);
            Controls.Add(btnReset);
        }

        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(x, y)
            });
        }

        private DateTimePicker AddDateTimePicker(int x, int y)
        {
            var dtp = new DateTimePicker
            {
                Location = new Point(x, y),
                Width = 120,
                Format = DateTimePickerFormat.Short
            };
            Controls.Add(dtp);
            return dtp;
        }

        private ComboBox AddComboBox(int x, int y, string[] items)
        {
            var cmb = new ComboBox
            {
                Location = new Point(x, y),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;
            Controls.Add(cmb);
            return cmb;
        }

        private Button CreateButton(string text, int width, Color back, Color fore)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}