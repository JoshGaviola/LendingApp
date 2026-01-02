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
        private CashierDashboard cashierDashboard;
        private BindingList<Class.Models.CashierModels.TransactionModels> transactions;
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
    

        /*
        private void SignInBtn_Click(object sender, EventArgs e)
        {
            cashierDashboard = new CashierDashboard();
            cashierDashboard.Show();
        
        */
    }
}
