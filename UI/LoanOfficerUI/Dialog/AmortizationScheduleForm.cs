using LendingApp.Class.Services.Loans;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace LoanApplicationUI
{
    public class AmortizationScheduleForm : Form
    {
        private DataGridView dgv;
        private Button btnClose;
        private Button btnExport;
        private readonly List<AmortizationRow> _schedule;

        public AmortizationScheduleForm(List<AmortizationRow> schedule, string title = "Amortization Schedule")
        {
            _schedule = schedule ?? new List<AmortizationRow>();
            InitializeComponent();
            Text = title;
            PopulateGrid();
        }

        private void InitializeComponent()
        {
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(700, 520);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 420,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            dgv.Columns.Add("Month", "Month");
            dgv.Columns.Add("Payment", "Payment");
            dgv.Columns.Add("Principal", "Principal");
            dgv.Columns.Add("Interest", "Interest");
            dgv.Columns.Add("Balance", "Balance");

            btnExport = new Button
            {
                Text = "Export CSV",
                Width = 110,
                Height = 30,
                Location = new Point(12, 430),
                BackColor = ColorTranslator.FromHtml("#F59E0B"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += BtnExport_Click;

            btnClose = new Button
            {
                Text = "Close",
                Width = 110,
                Height = 30,
                Location = new Point(132, 430),
                BackColor = ColorTranslator.FromHtml("#6B7280"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.Click += (s, e) => Close();

            Controls.Add(dgv);
            Controls.Add(btnExport);
            Controls.Add(btnClose);
        }

        private void PopulateGrid()
        {
            dgv.Rows.Clear();
            foreach (var r in _schedule)
            {
                dgv.Rows.Add(
                    r.Month,
                    $"₱{r.Payment:N2}",
                    $"₱{r.Principal:N2}",
                    $"₱{r.Interest:N2}",
                    $"₱{r.Balance:N2}"
                );
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv", FileName = "amortization.csv" })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("Month,Payment,Principal,Interest,Balance");
                        foreach (var r in _schedule)
                        {
                            sw.WriteLine($"{r.Month},{r.Payment},{r.Principal},{r.Interest},{r.Balance}");
                        }
                    }
                    MessageBox.Show("Exported amortization schedule.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to export file:\n" + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}