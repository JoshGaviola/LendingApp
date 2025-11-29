using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LendingApp
{
    public partial class CustomerLogin : Form
    {
        public CustomerLogin()
        {
            InitializeComponent();
            CustomerLoginLabel.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            EnterCredentialsLbl.ForeColor = ColorTranslator.FromHtml("#898484");
            SignInBtn.BackColor = ColorTranslator.FromHtml("#3498DB");
        }

        bool movePosition;
        int xCoordinate;
        int yCoordinate;

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
                this.SetDesktopLocation(MousePosition.X, MousePosition.Y - yCoordinate);
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

        private void lblRegister_Click(object sender, EventArgs e)
        {

        }
    }
}
