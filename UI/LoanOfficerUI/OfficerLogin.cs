using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LendingApp.UI.CustomerUI; // For CustomerRegistration
using LendingApp.UI.LoanOfficerUI; // For OfficerDashboard

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerLogin : Form
    {
        private CustomerRegistration _openRegistrationForm;
        private OfficerDashboard _openOfficerDashboardForm;

        public OfficerLogin()
        {
            InitializeComponent();
            OfficerLoginLabel.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            EnterCredentialsLbl.ForeColor = ColorTranslator.FromHtml("#898484");
            SignInBtn.BackColor = ColorTranslator.FromHtml("#3498DB");

            lblRegister.Cursor = Cursors.Hand;
            lblRegister.ForeColor = Color.Blue;
            lblRegister.Font = new Font(lblRegister.Font, FontStyle.Underline);
            // If the Designer already wires Click, do not add it again to avoid double firing.
            // lblRegister.Click += lblRegister_Click;
        }

        bool movePosition;
        int xCoordinate;
        int yCoordinate;

        private void lblRegister_Click(object sender, EventArgs e)
        {
            if (_openRegistrationForm == null || _openRegistrationForm.IsDisposed)
            {
                _openRegistrationForm = new CustomerRegistration();
                _openRegistrationForm.FormClosed += (s, args) => _openRegistrationForm = null;
                _openRegistrationForm.Show(this);
            }
            else
            {
                _openRegistrationForm.Focus();
            }
        }

        private void OfficerLogin_MouseDown(object sender, MouseEventArgs e)
        {
            movePosition = true;
            xCoordinate = e.X;
            yCoordinate = e.Y;
        }

        private void OfficerLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if (movePosition)
            {
                SetDesktopLocation(MousePosition.X, MousePosition.Y - yCoordinate);
            }
        }

        private void OfficerLogin_MouseUp(object sender, MouseEventArgs e)
        {
            movePosition = false;
        }

        private void PanelLeftSideCustLogin_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = PanelLeftSideCustLogin.ClientRectangle;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                ColorTranslator.FromHtml("#2C3E50"),
                ColorTranslator.FromHtml("#3498DB"),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void BorderGray(object sender, PaintEventArgs e)
        {
            Panel p = sender as Panel;
            ControlPaint.DrawBorder(
                e.Graphics,
                p.ClientRectangle,
                ColorTranslator.FromHtml("#A7A7A7"),
                ButtonBorderStyle.Solid
            );
        }

        private void SignInBtn_Click_1(object sender, EventArgs e)
        {
            // TODO: Add real authentication logic here (validate username/password).
            // If credentials invalid, return early.

            if (_openOfficerDashboardForm == null || _openOfficerDashboardForm.IsDisposed)
            {
                _openOfficerDashboardForm = new OfficerDashboard();
                _openOfficerDashboardForm.FormClosed += (s, args) =>
                {
                    if (!this.IsDisposed)
                    {
                        this.Close();
                    }
                };

                this.Hide();
                _openOfficerDashboardForm.Show();
            }
            else
            {
                _openOfficerDashboardForm.Focus();
            }
        }

        private void txtboxPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtboxUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblRegister_Click_1(object sender, EventArgs e)
        {

        }

        private void EnterCredentialsLbl_Click(object sender, EventArgs e)
        {

        }

        private void OfficerLoginLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
