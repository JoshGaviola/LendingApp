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
            this.gpCustomerList = new System.Windows.Forms.GroupBox();
            this.btnCustomer1 = new System.Windows.Forms.Button();
            this.btnCustomer2 = new System.Windows.Forms.Button();
            this.btnCustomer3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCustomerDetails = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gpCustomerList.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCustomerManagement
            // 
            this.lblCustomerManagement.AutoSize = true;
            this.lblCustomerManagement.Location = new System.Drawing.Point(325, 9);
            this.lblCustomerManagement.Name = "lblCustomerManagement";
            this.lblCustomerManagement.Size = new System.Drawing.Size(148, 13);
            this.lblCustomerManagement.TabIndex = 0;
            this.lblCustomerManagement.Text = "CUSTOMER MANAGEMENT";
            this.lblCustomerManagement.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblCustomerList
            // 
            this.lblCustomerList.AutoSize = true;
            this.lblCustomerList.Location = new System.Drawing.Point(30, 74);
            this.lblCustomerList.Name = "lblCustomerList";
            this.lblCustomerList.Size = new System.Drawing.Size(94, 13);
            this.lblCustomerList.TabIndex = 1;
            this.lblCustomerList.Text = "CUSTOMER LIST";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(30, 38);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(47, 13);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "Search: ";
            // 
            // txtSearchCustomers
            // 
            this.txtSearchCustomers.Location = new System.Drawing.Point(83, 35);
            this.txtSearchCustomers.Name = "txtSearchCustomers";
            this.txtSearchCustomers.Size = new System.Drawing.Size(211, 20);
            this.txtSearchCustomers.TabIndex = 3;
            this.txtSearchCustomers.Text = "SearchCustomers...";
            // 
            // gpCustomerList
            // 
            this.gpCustomerList.Controls.Add(this.btnCustomer3);
            this.gpCustomerList.Controls.Add(this.btnCustomer2);
            this.gpCustomerList.Controls.Add(this.btnCustomer1);
            this.gpCustomerList.Location = new System.Drawing.Point(33, 90);
            this.gpCustomerList.Name = "gpCustomerList";
            this.gpCustomerList.Size = new System.Drawing.Size(200, 225);
            this.gpCustomerList.TabIndex = 4;
            this.gpCustomerList.TabStop = false;
            this.gpCustomerList.Text = "Customer List";
            // 
            // btnCustomer1
            // 
            this.btnCustomer1.Location = new System.Drawing.Point(6, 29);
            this.btnCustomer1.Name = "btnCustomer1";
            this.btnCustomer1.Size = new System.Drawing.Size(188, 54);
            this.btnCustomer1.TabIndex = 0;
            this.btnCustomer1.Text = "            Juan Dela Cruz\r\n            +63 952 84254";
            this.btnCustomer1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer1.UseVisualStyleBackColor = true;
            // 
            // btnCustomer2
            // 
            this.btnCustomer2.Location = new System.Drawing.Point(6, 89);
            this.btnCustomer2.Name = "btnCustomer2";
            this.btnCustomer2.Size = new System.Drawing.Size(188, 54);
            this.btnCustomer2.TabIndex = 1;
            this.btnCustomer2.Text = "            Maria Santos\n            +63 952 84254";
            this.btnCustomer2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer2.UseVisualStyleBackColor = true;
            // 
            // btnCustomer3
            // 
            this.btnCustomer3.Location = new System.Drawing.Point(6, 149);
            this.btnCustomer3.Name = "btnCustomer3";
            this.btnCustomer3.Size = new System.Drawing.Size(188, 54);
            this.btnCustomer3.TabIndex = 2;
            this.btnCustomer3.Text = "            Alfred Rosenberg            \r\n            +63 952 84254";
            this.btnCustomer3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblCustomerDetails);
            this.panel1.Location = new System.Drawing.Point(401, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 241);
            this.panel1.TabIndex = 5;
            // 
            // lblCustomerDetails
            // 
            this.lblCustomerDetails.AutoSize = true;
            this.lblCustomerDetails.Location = new System.Drawing.Point(3, 0);
            this.lblCustomerDetails.Name = "lblCustomerDetails";
            this.lblCustomerDetails.Size = new System.Drawing.Size(86, 13);
            this.lblCustomerDetails.TabIndex = 6;
            this.lblCustomerDetails.Text = "Customer Details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // CustomerManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gpCustomerList);
            this.Controls.Add(this.txtSearchCustomers);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblCustomerList);
            this.Controls.Add(this.lblCustomerManagement);
            this.Name = "CustomerManagement";
            this.Text = "CustomerManagement";
            this.gpCustomerList.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomerManagement;
        private System.Windows.Forms.Label lblCustomerList;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchCustomers;
        private System.Windows.Forms.GroupBox gpCustomerList;
        private System.Windows.Forms.Button btnCustomer1;
        private System.Windows.Forms.Button btnCustomer3;
        private System.Windows.Forms.Button btnCustomer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCustomerDetails;
    }
}