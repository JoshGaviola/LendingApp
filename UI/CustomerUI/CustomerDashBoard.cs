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
    public partial class CustomerDashBoard : Form{
        public CustomerDashBoard(){

            InitializeComponent();
            SetLabelsGray();
            label1.ForeColor = ColorTranslator.FromHtml("#838383");
            panel4.BackColor = ColorTranslator.FromHtml("#3498DB");
            panel5.BackColor = ColorTranslator.FromHtml("#34DB3A");
            panel6.BackColor = ColorTranslator.FromHtml("#FFC300");

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
