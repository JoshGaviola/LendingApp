using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CustomerUI
{
    partial class CustomerRegistration
    {
        private IContainer components = null;

        // Declare the core panels so the designer can render them
        private Panel headerPanel;
        private Panel navigationPanel;
        private Panel contentHost;
        private Panel footerPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.headerPanel = new Panel();
            this.navigationPanel = new Panel();
            this.contentHost = new Panel();
            this.footerPanel = new Panel();

            // Form
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.MinimumSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Customer Registration";

            // headerPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 70;
            this.headerPanel.BackColor = ColorTranslator.FromHtml("#2C3E50");

            // navigationPanel
            this.navigationPanel.Dock = DockStyle.Left;
            this.navigationPanel.Width = 260;
            this.navigationPanel.BackColor = Color.White;
            this.navigationPanel.BorderStyle = BorderStyle.FixedSingle;

            // contentHost
            this.contentHost.Dock = DockStyle.Fill;
            this.contentHost.BackColor = Color.White;
            this.contentHost.Padding = new Padding(10);

            // footerPanel
            this.footerPanel.Dock = DockStyle.Bottom;
            this.footerPanel.Height = 65;
            this.footerPanel.BackColor = Color.WhiteSmoke;
            this.footerPanel.BorderStyle = BorderStyle.FixedSingle;

            // Add to form (order matters for docking)
            this.Controls.Add(this.contentHost);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }
    }
}