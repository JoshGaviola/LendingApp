using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    partial class OfficerCustomers
    {
        private IContainer components = null;

        private Panel headerPanel;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private Button btnRegisterCustomer;

        private Panel statsPanel;
        private Panel cardTotal;
        private Label lblTotalTitle;
        private Label lblTotalCustomers;

        private Panel cardNew;
        private Label lblNewTitle;
        private Label lblNew;

        private Panel cardRegular;
        private Label lblRegularTitle;
        private Label lblRegular;

        private Panel cardVIP;
        private Label lblVIPTitle;
        private Label lblVIP;

        private Panel cardDelinquent;
        private Label lblDelinquentTitle;
        private Label lblDelinquent;

        private Panel filtersPanel;
        private ComboBox cmbCustomerType;
        private TextBox txtSearch;
        private Label lblResults;

        private DataGridView gridCustomers;

        private Panel infoPanel;
        private Label lblInfoTitle;
        private Label lblInfoText;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();

            this.headerPanel = new Panel();
            this.lblHeaderTitle = new Label();
            this.lblHeaderSubtitle = new Label();
            this.btnRegisterCustomer = new Button();

            this.statsPanel = new Panel();
            this.cardTotal = new Panel();
            this.lblTotalTitle = new Label();
            this.lblTotalCustomers = new Label();

            this.cardNew = new Panel();
            this.lblNewTitle = new Label();
            this.lblNew = new Label();

            this.cardRegular = new Panel();
            this.lblRegularTitle = new Label();
            this.lblRegular = new Label();

            this.cardVIP = new Panel();
            this.lblVIPTitle = new Label();
            this.lblVIP = new Label();

            this.cardDelinquent = new Panel();
            this.lblDelinquentTitle = new Label();
            this.lblDelinquent = new Label();

            this.filtersPanel = new Panel();
            this.cmbCustomerType = new ComboBox();
            this.txtSearch = new TextBox();
            this.lblResults = new Label();

            this.gridCustomers = new DataGridView();

            this.infoPanel = new Panel();
            this.lblInfoTitle = new Label();
            this.lblInfoText = new Label();

            // Form
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Customer Management";
            this.BackColor = Color.White;

            // HeaderPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 80;
            this.headerPanel.BackColor = Color.White;
            this.headerPanel.BorderStyle = BorderStyle.FixedSingle;

            this.lblHeaderTitle.Location = new Point(16, 12);
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Text = "Customer Management";

            this.lblHeaderSubtitle.Location = new Point(16, 40);
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Text = "View and manage customer profiles and accounts";

            this.btnRegisterCustomer.Location = new Point(1000, 20);
            this.btnRegisterCustomer.Size = new Size(160, 35);
            this.btnRegisterCustomer.Text = "Register Customer";

            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Controls.Add(this.lblHeaderSubtitle);
            this.headerPanel.Controls.Add(this.btnRegisterCustomer);

            // StatsPanel
            this.statsPanel.Dock = DockStyle.Top;
            this.statsPanel.Height = 100;
            this.statsPanel.BackColor = Color.White;
            this.statsPanel.BorderStyle = BorderStyle.FixedSingle;

            // Card Total
            this.cardTotal.Location = new Point(16, 16);
            this.cardTotal.Size = new Size(200, 68);
            this.cardTotal.BorderStyle = BorderStyle.FixedSingle;
            this.lblTotalTitle.Location = new Point(10, 8);
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Text = "Total Customers";
            this.lblTotalCustomers.Location = new Point(10, 34);
            this.lblTotalCustomers.AutoSize = true;
            this.lblTotalCustomers.Text = "0";
            this.cardTotal.Controls.Add(this.lblTotalTitle);
            this.cardTotal.Controls.Add(this.lblTotalCustomers);

            // Card New
            this.cardNew.Location = new Point(226, 16);
            this.cardNew.Size = new Size(200, 68);
            this.cardNew.BorderStyle = BorderStyle.FixedSingle;
            this.lblNewTitle.Location = new Point(10, 8);
            this.lblNewTitle.AutoSize = true;
            this.lblNewTitle.Text = "New";
            this.lblNew.Location = new Point(10, 34);
            this.lblNew.AutoSize = true;
            this.lblNew.Text = "0";
            this.cardNew.Controls.Add(this.lblNewTitle);
            this.cardNew.Controls.Add(this.lblNew);

            // Card Regular
            this.cardRegular.Location = new Point(436, 16);
            this.cardRegular.Size = new Size(200, 68);
            this.cardRegular.BorderStyle = BorderStyle.FixedSingle;
            this.lblRegularTitle.Location = new Point(10, 8);
            this.lblRegularTitle.AutoSize = true;
            this.lblRegularTitle.Text = "Regular";
            this.lblRegular.Location = new Point(10, 34);
            this.lblRegular.AutoSize = true;
            this.lblRegular.Text = "0";
            this.cardRegular.Controls.Add(this.lblRegularTitle);
            this.cardRegular.Controls.Add(this.lblRegular);

            // Card VIP
            this.cardVIP.Location = new Point(646, 16);
            this.cardVIP.Size = new Size(200, 68);
            this.cardVIP.BorderStyle = BorderStyle.FixedSingle;
            this.lblVIPTitle.Location = new Point(10, 8);
            this.lblVIPTitle.AutoSize = true;
            this.lblVIPTitle.Text = "VIP";
            this.lblVIP.Location = new Point(10, 34);
            this.lblVIP.AutoSize = true;
            this.lblVIP.Text = "0";
            this.cardVIP.Controls.Add(this.lblVIPTitle);
            this.cardVIP.Controls.Add(this.lblVIP);

            // Card Delinquent
            this.cardDelinquent.Location = new Point(856, 16);
            this.cardDelinquent.Size = new Size(200, 68);
            this.cardDelinquent.BorderStyle = BorderStyle.FixedSingle;
            this.lblDelinquentTitle.Location = new Point(10, 8);
            this.lblDelinquentTitle.AutoSize = true;
            this.lblDelinquentTitle.Text = "Delinquent";
            this.lblDelinquent.Location = new Point(10, 34);
            this.lblDelinquent.AutoSize = true;
            this.lblDelinquent.Text = "0";
            this.cardDelinquent.Controls.Add(this.lblDelinquentTitle);
            this.cardDelinquent.Controls.Add(this.lblDelinquent);

            this.statsPanel.Controls.Add(this.cardTotal);
            this.statsPanel.Controls.Add(this.cardNew);
            this.statsPanel.Controls.Add(this.cardRegular);
            this.statsPanel.Controls.Add(this.cardVIP);
            this.statsPanel.Controls.Add(this.cardDelinquent);

            // FiltersPanel
            this.filtersPanel.Dock = DockStyle.Top;
            this.filtersPanel.Height = 100;
            this.filtersPanel.BackColor = Color.White;
            this.filtersPanel.BorderStyle = BorderStyle.FixedSingle;

            this.cmbCustomerType.Location = new Point(16, 16);
            this.cmbCustomerType.Size = new Size(200, 24);

            this.txtSearch.Location = new Point(226, 16);
            this.txtSearch.Size = new Size(400, 24);

            this.lblResults.Location = new Point(16, 56);
            this.lblResults.AutoSize = true;
            this.lblResults.Text = "Showing 0 of 0 customers";

            this.filtersPanel.Controls.Add(this.cmbCustomerType);
            this.filtersPanel.Controls.Add(this.txtSearch);
            this.filtersPanel.Controls.Add(this.lblResults);

            // GridCustomers
            this.gridCustomers.Dock = DockStyle.Fill;
            this.gridCustomers.BackgroundColor = Color.White;

            // InfoPanel
            this.infoPanel.Dock = DockStyle.Bottom;
            this.infoPanel.Height = 100;
            this.infoPanel.BackColor = ColorTranslator.FromHtml("#EFF6FF");
            this.infoPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblInfoTitle.Location = new Point(16, 12);
            this.lblInfoTitle.AutoSize = true;
            this.lblInfoTitle.Text = "Customer Classification";
            this.lblInfoText.Location = new Point(16, 36);
            this.lblInfoText.AutoSize = true;
            this.lblInfoText.Text = "New: First-time borrowers • Regular: 1-4 successful loans • VIP: 5+ loans with excellent history • Delinquent: Late payments or defaults";
            this.infoPanel.Controls.Add(this.lblInfoTitle);
            this.infoPanel.Controls.Add(this.lblInfoText);

            // Add to form (order matters for docking)
            this.Controls.Add(this.gridCustomers);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.filtersPanel);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }
    }
}