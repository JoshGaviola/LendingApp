namespace LendingApp.UI.CustomerUI
{
    partial class CustomerManagement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCustomerManagement = new System.Windows.Forms.Label();
            this.lblCustomerList = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchCustomers = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblCustomerManagement
            // 
            this.lblCustomerManagement.AutoSize = true;
            this.lblCustomerManagement.Location = new System.Drawing.Point(324, 9);
            this.lblCustomerManagement.Name = "lblCustomerManagement";
            this.lblCustomerManagement.Size = new System.Drawing.Size(148, 13);
            this.lblCustomerManagement.TabIndex = 0;
            this.lblCustomerManagement.Text = "CUSTOMER MANAGEMENT";
            this.lblCustomerManagement.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblCustomerList
            // 
            this.lblCustomerList.AutoSize = true;
            this.lblCustomerList.Location = new System.Drawing.Point(29, 74);
            this.lblCustomerList.Name = "lblCustomerList";
            this.lblCustomerList.Size = new System.Drawing.Size(94, 13);
            this.lblCustomerList.TabIndex = 1;
            this.lblCustomerList.Text = "CUSTOMER LIST";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(29, 38);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(47, 13);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "Search: ";
            // 
            // txtSearchCustomers
            // 
            this.txtSearchCustomers.Location = new System.Drawing.Point(82, 35);
            this.txtSearchCustomers.Name = "txtSearchCustomers";
            this.txtSearchCustomers.Size = new System.Drawing.Size(211, 20);
            this.txtSearchCustomers.TabIndex = 3;
            this.txtSearchCustomers.Text = "SearchCustomers...";
            // 
            // CustomerManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtSearchCustomers);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblCustomerList);
            this.Controls.Add(this.lblCustomerManagement);
            this.Name = "CustomerManagement";
            this.Text = "CustomerManagement";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomerManagement;
        private System.Windows.Forms.Label lblCustomerList;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchCustomers;
    }
}