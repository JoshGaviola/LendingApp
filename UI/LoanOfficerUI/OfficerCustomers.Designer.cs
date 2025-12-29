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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.btnRegisterCustomer = new System.Windows.Forms.Button();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.cardTotal = new System.Windows.Forms.Panel();
            this.lblTotalTitle = new System.Windows.Forms.Label();
            this.lblTotalCustomers = new System.Windows.Forms.Label();
            this.cardNew = new System.Windows.Forms.Panel();
            this.lblNewTitle = new System.Windows.Forms.Label();
            this.lblNew = new System.Windows.Forms.Label();
            this.cardRegular = new System.Windows.Forms.Panel();
            this.lblRegularTitle = new System.Windows.Forms.Label();
            this.lblRegular = new System.Windows.Forms.Label();
            this.cardVIP = new System.Windows.Forms.Panel();
            this.lblVIPTitle = new System.Windows.Forms.Label();
            this.lblVIP = new System.Windows.Forms.Label();
            this.cardDelinquent = new System.Windows.Forms.Panel();
            this.lblDelinquentTitle = new System.Windows.Forms.Label();
            this.lblDelinquent = new System.Windows.Forms.Label();
            this.filtersPanel = new System.Windows.Forms.Panel();
            this.cmbCustomerType = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.gridCustomers = new System.Windows.Forms.DataGridView();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.lblInfoTitle = new System.Windows.Forms.Label();
            this.lblInfoText = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.statsPanel.SuspendLayout();
            this.cardTotal.SuspendLayout();
            this.cardNew.SuspendLayout();
            this.cardRegular.SuspendLayout();
            this.cardVIP.SuspendLayout();
            this.cardDelinquent.SuspendLayout();
            this.filtersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCustomers)).BeginInit();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Controls.Add(this.lblHeaderSubtitle);
            this.headerPanel.Controls.Add(this.btnRegisterCustomer);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1200, 80);
            this.headerPanel.TabIndex = 4;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Location = new System.Drawing.Point(16, 12);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(116, 13);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "Customer Management";
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(16, 40);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(242, 13);
            this.lblHeaderSubtitle.TabIndex = 1;
            this.lblHeaderSubtitle.Text = "View and manage customer profiles and accounts";
            // 
            // btnRegisterCustomer
            // 
            this.btnRegisterCustomer.Location = new System.Drawing.Point(1000, 20);
            this.btnRegisterCustomer.Name = "btnRegisterCustomer";
            this.btnRegisterCustomer.Size = new System.Drawing.Size(160, 35);
            this.btnRegisterCustomer.TabIndex = 2;
            this.btnRegisterCustomer.Text = "Register Customer";
            this.btnRegisterCustomer.Click += new System.EventHandler(this.btnRegisterCustomer_Click);
            // 
            // statsPanel
            // 
            this.statsPanel.BackColor = System.Drawing.Color.White;
            this.statsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statsPanel.Controls.Add(this.cardTotal);
            this.statsPanel.Controls.Add(this.cardNew);
            this.statsPanel.Controls.Add(this.cardRegular);
            this.statsPanel.Controls.Add(this.cardVIP);
            this.statsPanel.Controls.Add(this.cardDelinquent);
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statsPanel.Location = new System.Drawing.Point(0, 80);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1200, 100);
            this.statsPanel.TabIndex = 3;
            // 
            // cardTotal
            // 
            this.cardTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotal.Controls.Add(this.lblTotalTitle);
            this.cardTotal.Controls.Add(this.lblTotalCustomers);
            this.cardTotal.Location = new System.Drawing.Point(16, 16);
            this.cardTotal.Name = "cardTotal";
            this.cardTotal.Size = new System.Drawing.Size(200, 68);
            this.cardTotal.TabIndex = 0;
            // 
            // lblTotalTitle
            // 
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Location = new System.Drawing.Point(10, 8);
            this.lblTotalTitle.Name = "lblTotalTitle";
            this.lblTotalTitle.Size = new System.Drawing.Size(83, 13);
            this.lblTotalTitle.TabIndex = 0;
            this.lblTotalTitle.Text = "Total Customers";
            // 
            // lblTotalCustomers
            // 
            this.lblTotalCustomers.AutoSize = true;
            this.lblTotalCustomers.Location = new System.Drawing.Point(10, 34);
            this.lblTotalCustomers.Name = "lblTotalCustomers";
            this.lblTotalCustomers.Size = new System.Drawing.Size(13, 13);
            this.lblTotalCustomers.TabIndex = 1;
            this.lblTotalCustomers.Text = "0";
            // 
            // cardNew
            // 
            this.cardNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardNew.Controls.Add(this.lblNewTitle);
            this.cardNew.Controls.Add(this.lblNew);
            this.cardNew.Location = new System.Drawing.Point(226, 16);
            this.cardNew.Name = "cardNew";
            this.cardNew.Size = new System.Drawing.Size(200, 68);
            this.cardNew.TabIndex = 1;
            // 
            // lblNewTitle
            // 
            this.lblNewTitle.AutoSize = true;
            this.lblNewTitle.Location = new System.Drawing.Point(10, 8);
            this.lblNewTitle.Name = "lblNewTitle";
            this.lblNewTitle.Size = new System.Drawing.Size(29, 13);
            this.lblNewTitle.TabIndex = 0;
            this.lblNewTitle.Text = "New";
            // 
            // lblNew
            // 
            this.lblNew.AutoSize = true;
            this.lblNew.Location = new System.Drawing.Point(10, 34);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(13, 13);
            this.lblNew.TabIndex = 1;
            this.lblNew.Text = "0";
            // 
            // cardRegular
            // 
            this.cardRegular.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardRegular.Controls.Add(this.lblRegularTitle);
            this.cardRegular.Controls.Add(this.lblRegular);
            this.cardRegular.Location = new System.Drawing.Point(436, 16);
            this.cardRegular.Name = "cardRegular";
            this.cardRegular.Size = new System.Drawing.Size(200, 68);
            this.cardRegular.TabIndex = 2;
            // 
            // lblRegularTitle
            // 
            this.lblRegularTitle.AutoSize = true;
            this.lblRegularTitle.Location = new System.Drawing.Point(10, 8);
            this.lblRegularTitle.Name = "lblRegularTitle";
            this.lblRegularTitle.Size = new System.Drawing.Size(44, 13);
            this.lblRegularTitle.TabIndex = 0;
            this.lblRegularTitle.Text = "Regular";
            // 
            // lblRegular
            // 
            this.lblRegular.AutoSize = true;
            this.lblRegular.Location = new System.Drawing.Point(10, 34);
            this.lblRegular.Name = "lblRegular";
            this.lblRegular.Size = new System.Drawing.Size(13, 13);
            this.lblRegular.TabIndex = 1;
            this.lblRegular.Text = "0";
            // 
            // cardVIP
            // 
            this.cardVIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardVIP.Controls.Add(this.lblVIPTitle);
            this.cardVIP.Controls.Add(this.lblVIP);
            this.cardVIP.Location = new System.Drawing.Point(646, 16);
            this.cardVIP.Name = "cardVIP";
            this.cardVIP.Size = new System.Drawing.Size(200, 68);
            this.cardVIP.TabIndex = 3;
            // 
            // lblVIPTitle
            // 
            this.lblVIPTitle.AutoSize = true;
            this.lblVIPTitle.Location = new System.Drawing.Point(10, 8);
            this.lblVIPTitle.Name = "lblVIPTitle";
            this.lblVIPTitle.Size = new System.Drawing.Size(24, 13);
            this.lblVIPTitle.TabIndex = 0;
            this.lblVIPTitle.Text = "VIP";
            // 
            // lblVIP
            // 
            this.lblVIP.AutoSize = true;
            this.lblVIP.Location = new System.Drawing.Point(10, 34);
            this.lblVIP.Name = "lblVIP";
            this.lblVIP.Size = new System.Drawing.Size(13, 13);
            this.lblVIP.TabIndex = 1;
            this.lblVIP.Text = "0";
            // 
            // cardDelinquent
            // 
            this.cardDelinquent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardDelinquent.Controls.Add(this.lblDelinquentTitle);
            this.cardDelinquent.Controls.Add(this.lblDelinquent);
            this.cardDelinquent.Location = new System.Drawing.Point(856, 16);
            this.cardDelinquent.Name = "cardDelinquent";
            this.cardDelinquent.Size = new System.Drawing.Size(200, 68);
            this.cardDelinquent.TabIndex = 4;
            // 
            // lblDelinquentTitle
            // 
            this.lblDelinquentTitle.AutoSize = true;
            this.lblDelinquentTitle.Location = new System.Drawing.Point(10, 8);
            this.lblDelinquentTitle.Name = "lblDelinquentTitle";
            this.lblDelinquentTitle.Size = new System.Drawing.Size(58, 13);
            this.lblDelinquentTitle.TabIndex = 0;
            this.lblDelinquentTitle.Text = "Delinquent";
            // 
            // lblDelinquent
            // 
            this.lblDelinquent.AutoSize = true;
            this.lblDelinquent.Location = new System.Drawing.Point(10, 34);
            this.lblDelinquent.Name = "lblDelinquent";
            this.lblDelinquent.Size = new System.Drawing.Size(13, 13);
            this.lblDelinquent.TabIndex = 1;
            this.lblDelinquent.Text = "0";
            // 
            // filtersPanel
            // 
            this.filtersPanel.BackColor = System.Drawing.Color.White;
            this.filtersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filtersPanel.Controls.Add(this.cmbCustomerType);
            this.filtersPanel.Controls.Add(this.txtSearch);
            this.filtersPanel.Controls.Add(this.lblResults);
            this.filtersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filtersPanel.Location = new System.Drawing.Point(0, 180);
            this.filtersPanel.Name = "filtersPanel";
            this.filtersPanel.Size = new System.Drawing.Size(1200, 100);
            this.filtersPanel.TabIndex = 2;
            // 
            // cmbCustomerType
            // 
            this.cmbCustomerType.Location = new System.Drawing.Point(16, 16);
            this.cmbCustomerType.Name = "cmbCustomerType";
            this.cmbCustomerType.Size = new System.Drawing.Size(200, 21);
            this.cmbCustomerType.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(226, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(400, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(16, 56);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(129, 13);
            this.lblResults.TabIndex = 2;
            this.lblResults.Text = "Showing 0 of 0 customers";
            // 
            // gridCustomers
            // 
            this.gridCustomers.BackgroundColor = System.Drawing.Color.White;
            this.gridCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCustomers.Location = new System.Drawing.Point(0, 280);
            this.gridCustomers.Name = "gridCustomers";
            this.gridCustomers.Size = new System.Drawing.Size(1200, 420);
            this.gridCustomers.TabIndex = 0;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.infoPanel.Controls.Add(this.lblInfoTitle);
            this.infoPanel.Controls.Add(this.lblInfoText);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.infoPanel.Location = new System.Drawing.Point(0, 700);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(1200, 100);
            this.infoPanel.TabIndex = 1;
            // 
            // lblInfoTitle
            // 
            this.lblInfoTitle.AutoSize = true;
            this.lblInfoTitle.Location = new System.Drawing.Point(16, 12);
            this.lblInfoTitle.Name = "lblInfoTitle";
            this.lblInfoTitle.Size = new System.Drawing.Size(115, 13);
            this.lblInfoTitle.TabIndex = 0;
            this.lblInfoTitle.Text = "Customer Classification";
            // 
            // lblInfoText
            // 
            this.lblInfoText.AutoSize = true;
            this.lblInfoText.Location = new System.Drawing.Point(16, 36);
            this.lblInfoText.Name = "lblInfoText";
            this.lblInfoText.Size = new System.Drawing.Size(641, 13);
            this.lblInfoText.TabIndex = 1;
            this.lblInfoText.Text = "New: First-time borrowers • Regular: 1-4 successful loans • VIP: 5+ loans with ex" +
    "cellent history • Delinquent: Late payments or defaults";
            // 
            // OfficerCustomers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.gridCustomers);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.filtersPanel);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "OfficerCustomers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Management";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.statsPanel.ResumeLayout(false);
            this.cardTotal.ResumeLayout(false);
            this.cardTotal.PerformLayout();
            this.cardNew.ResumeLayout(false);
            this.cardNew.PerformLayout();
            this.cardRegular.ResumeLayout(false);
            this.cardRegular.PerformLayout();
            this.cardVIP.ResumeLayout(false);
            this.cardVIP.PerformLayout();
            this.cardDelinquent.ResumeLayout(false);
            this.cardDelinquent.PerformLayout();
            this.filtersPanel.ResumeLayout(false);
            this.filtersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCustomers)).EndInit();
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}