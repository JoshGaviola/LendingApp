using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
