using System;
using System.IO;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public partial class ContractPreviewForm : Form
    {
        private readonly string _pdfPath;
        private WebBrowser _browser;
        private Button _btnSave;
        private Button _btnPrint;
        private Button _btnClose;

        public ContractPreviewForm(string pdfPath)
        {
            _pdfPath = pdfPath ?? throw new ArgumentNullException(nameof(pdfPath));

            InitializeComponent();
            BuildRuntimeUi();
            LoadPdf();
        }

        private void BuildRuntimeUi()
        {
            Text = "Contract Preview";
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;

            _browser = new WebBrowser { Dock = DockStyle.Fill };
            _btnSave = new Button { Text = "Save As...", Width = 100, Height = 30 };
            _btnPrint = new Button { Text = "Print", Width = 100, Height = 30 };
            _btnClose = new Button { Text = "Close", Width = 100, Height = 30 };

            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft
            };
            pnl.Controls.Add(_btnClose);
            pnl.Controls.Add(_btnPrint);
            pnl.Controls.Add(_btnSave);

            Controls.Clear();
            Controls.Add(_browser);
            Controls.Add(pnl);

            _btnSave.Click += BtnSave_Click;
            _btnPrint.Click += BtnPrint_Click;
            _btnClose.Click += (s, e) => Close();

            FormClosed += (s, e) =>
            {
                try { if (File.Exists(_pdfPath)) File.Delete(_pdfPath); } catch { }
            };
        }

        private void LoadPdf()
        {
            try
            {
                _browser.Navigate(_pdfPath);
            }
            catch
            {
                MessageBox.Show("Preview is not available. The file will be opened by the default PDF viewer.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start(_pdfPath);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = Path.GetFileName(_pdfPath)
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                File.Copy(_pdfPath, sfd.FileName, true);
                MessageBox.Show("Saved: " + sfd.FileName, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(_pdfPath, "/p");
            }
            catch
            {
                MessageBox.Show("Unable to print. Open the file and print from your PDF viewer.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
