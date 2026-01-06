using LendingApp.Class.LogicClass.Cashier;
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

namespace LendingApp.UI.CashierUI
{
    public partial class CashierLogin : Form
    {
        private ApplicantsData data;
        private CashierDashboard cashierDashboard;
        
        public CashierLogin()
        {
            InitializeComponent();
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

        private void SignInBtn_Click(object sender, EventArgs e)
        {
           string username = txtboxUserName.Text.Trim();
           string password = txtboxPassword.Text.Trim();

           CashierLoginLogic cashierLoginLogic = new CashierLoginLogic();
            
           bool success = cashierLoginLogic.LoginSuccessfully(username, password);

            if (success)
            {
                cashierDashboard = new CashierDashboard(data);
                cashierDashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

       
    }
}
