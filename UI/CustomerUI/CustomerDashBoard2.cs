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

namespace LendingApp.UI.CustomerUI{
    public partial class CustomerDashBoard2 : Form{
        public CustomerDashBoard2(){

            InitializeComponent();
            SetLabelsGray();
            //formControls();
            this.BackColor = ColorTranslator.FromHtml("#F0F2F5");
            label1.ForeColor = ColorTranslator.FromHtml("#838383");
            panel4.BackColor = ColorTranslator.FromHtml("#3498DB");
            panel5.BackColor = ColorTranslator.FromHtml("#34DB3A");
            panel6.BackColor = ColorTranslator.FromHtml("#FFC300");
            RecentPayments.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            QuickActions.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            LoandID.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            LoanStatus.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            LoanType.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            Amount.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            Term.ForeColor = ColorTranslator.FromHtml("#2C3E50");
            Status.ForeColor = ColorTranslator.FromHtml("#2C3E50");

        }

        private void formControls()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }
        private void CustDashHeaderPanel_Paint(object sender, PaintEventArgs e){

            Rectangle rect = CustDashHeaderPanel.ClientRectangle;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                ColorTranslator.FromHtml("#2C3E50"),
                ColorTranslator.FromHtml("#3498DB"),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
        private void BorderGray(object sender, PaintEventArgs e){
            Panel p = sender as Panel;

            ControlPaint.DrawBorder(
                e.Graphics,
                p.ClientRectangle,
                ColorTranslator.FromHtml("#A3A3A3"),
                ButtonBorderStyle.Solid

            );
        }

        private void GrayLine(object sender, PaintEventArgs e)
        {

            using (Pen pen = new Pen(ColorTranslator.FromHtml("#B8B0B0"), 1))
            {
                e.Graphics.DrawLine(pen, 0, panel12.Height / 2, this.Width, panel12.Height / 2);
            }
        }

        private void SetLabelsGray(){
            foreach (Control control in this.Controls){
                if (control is Panel dashboardPanel && (string)dashboardPanel.Tag == "DashBoardBox"){

                    foreach (Control child in dashboardPanel.Controls){

                        if (child is Label dashboardLabel && (string)dashboardLabel.Tag == "insidePanelGray"){
                            dashboardLabel.ForeColor = ColorTranslator.FromHtml("#838383");
                        }

                    }
                }
            }


        }
    }
}
