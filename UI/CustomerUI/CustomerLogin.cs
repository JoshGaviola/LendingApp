using LendingApp.UI.CustomerUI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LendingApp
{
    public partial class CustomerLogin : Form
    {
        private CustomerRegistration _openRegistrationForm;
        private CustomerDashBoard _openCustomerDashBoardForm;

        public CustomerLogin()
        {
            InitializeComponent();
            CustomerLoginLabel.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            EnterCredentialsLbl.ForeColor = ColorTranslator.FromHtml("#898484");
            SignInBtn.BackColor = ColorTranslator.FromHtml("#3498DB");

            lblRegister.Cursor = Cursors.Hand;
            lblRegister.ForeColor = Color.Blue;
            lblRegister.Font = new Font(lblRegister.Font, FontStyle.Underline);

            // REMOVE this line if Designer already wires Click:
            // lblRegister.Click += lblRegister_Click;   // <— deleted to avoid double subscription
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

        private void CustomerLogin_MouseDown(object sender, MouseEventArgs e)
        {
            movePosition = true;
            xCoordinate = e.X;
            yCoordinate = e.Y;
        }

        private void CustomerLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if (movePosition)
            {
                SetDesktopLocation(MousePosition.X, MousePosition.Y - yCoordinate);
            }
        }

        private void CustomerLogin_MouseUp(object sender, MouseEventArgs e)
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

        private void SignInBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
