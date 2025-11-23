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
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchCustomers = new System.Windows.Forms.TextBox();
            this.gpCustomerList = new System.Windows.Forms.GroupBox();
            this.btnCustomer3 = new System.Windows.Forms.Button();
            this.btnCustomer2 = new System.Windows.Forms.Button();
            this.btnCustomer1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblCustomerDetails = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.lblIncome = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.lblCreditScore = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.gpCustomerList.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
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
            this.gpCustomerList.Controls.Add(this.pictureBox3);
            this.gpCustomerList.Controls.Add(this.pictureBox2);
            this.gpCustomerList.Controls.Add(this.pictureBox1);
            this.gpCustomerList.Controls.Add(this.btnCustomer3);
            this.gpCustomerList.Controls.Add(this.btnCustomer2);
            this.gpCustomerList.Controls.Add(this.btnCustomer1);
            this.gpCustomerList.Location = new System.Drawing.Point(33, 74);
            this.gpCustomerList.Name = "gpCustomerList";
            this.gpCustomerList.Size = new System.Drawing.Size(231, 333);
            this.gpCustomerList.TabIndex = 4;
            this.gpCustomerList.TabStop = false;
            this.gpCustomerList.Text = "Customer List";
            // 
            // btnCustomer3
            // 
            this.btnCustomer3.BackColor = System.Drawing.Color.White;
            this.btnCustomer3.Location = new System.Drawing.Point(6, 32);
            this.btnCustomer3.Name = "btnCustomer3";
            this.btnCustomer3.Size = new System.Drawing.Size(188, 69);
            this.btnCustomer3.TabIndex = 2;
            this.btnCustomer3.Text = "                Alfred Rosenberg            \r\n                +63 952 84254";
            this.btnCustomer3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer3.UseVisualStyleBackColor = false;
            // 
            // btnCustomer2
            // 
            this.btnCustomer2.BackColor = System.Drawing.Color.White;
            this.btnCustomer2.Location = new System.Drawing.Point(6, 188);
            this.btnCustomer2.Name = "btnCustomer2";
            this.btnCustomer2.Size = new System.Drawing.Size(188, 69);
            this.btnCustomer2.TabIndex = 1;
            this.btnCustomer2.Text = "                Maria Santos\r\n                +63 952 84254";
            this.btnCustomer2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer2.UseVisualStyleBackColor = false;
            // 
            // btnCustomer1
            // 
            this.btnCustomer1.BackColor = System.Drawing.Color.White;
            this.btnCustomer1.Location = new System.Drawing.Point(6, 112);
            this.btnCustomer1.Name = "btnCustomer1";
            this.btnCustomer1.Size = new System.Drawing.Size(188, 68);
            this.btnCustomer1.TabIndex = 0;
            this.btnCustomer1.Text = "                Juan Dela Cruz\r\n                +63 952 84254";
            this.btnCustomer1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomer1.UseVisualStyleBackColor = false;
            this.btnCustomer1.Click += new System.EventHandler(this.btnCustomer1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.lblCategory);
            this.panel1.Controls.Add(this.textBox5);
            this.panel1.Controls.Add(this.lblCreditScore);
            this.panel1.Controls.Add(this.textBox4);
            this.panel1.Controls.Add(this.lblIncome);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.lblAddress);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.lblPhone);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.lblName);
            this.panel1.Controls.Add(this.lblCustomerDetails);
            this.panel1.Location = new System.Drawing.Point(390, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 333);
            this.panel1.TabIndex = 5;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(25, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            this.lblName.Click += new System.EventHandler(this.lblName_Click);
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(28, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(328, 20);
            this.textBox1.TabIndex = 8;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(28, 93);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(328, 20);
            this.textBox2.TabIndex = 10;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(25, 77);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(41, 13);
            this.lblPhone.TabIndex = 9;
            this.lblPhone.Text = "Phone:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(28, 139);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(328, 20);
            this.textBox3.TabIndex = 12;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(25, 123);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 11;
            this.lblAddress.Text = "Address:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(28, 183);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(328, 20);
            this.textBox4.TabIndex = 14;
            // 
            // lblIncome
            // 
            this.lblIncome.AutoSize = true;
            this.lblIncome.Location = new System.Drawing.Point(25, 167);
            this.lblIncome.Name = "lblIncome";
            this.lblIncome.Size = new System.Drawing.Size(45, 13);
            this.lblIncome.TabIndex = 13;
            this.lblIncome.Text = "Income:";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(28, 221);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(95, 20);
            this.textBox5.TabIndex = 16;
            // 
            // lblCreditScore
            // 
            this.lblCreditScore.AutoSize = true;
            this.lblCreditScore.Location = new System.Drawing.Point(25, 205);
            this.lblCreditScore.Name = "lblCreditScore";
            this.lblCreditScore.Size = new System.Drawing.Size(68, 13);
            this.lblCreditScore.TabIndex = 15;
            this.lblCreditScore.Text = "Credit Score:";
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblCategory.Location = new System.Drawing.Point(129, 224);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(50, 13);
            this.lblCategory.TabIndex = 17;
            this.lblCategory.Text = "Excellent";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(28, 259);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 18;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(25, 244);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 19;
            this.lblStatus.Text = "Status:";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(289, 293);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 58);
            this.button1.TabIndex = 20;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.ImageLocation = "C:\\Users\\user\\Documents\\projects\\LendingApp\\icons\\person.png";
            this.pictureBox1.Location = new System.Drawing.Point(16, 48);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 43);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.ImageLocation = "C:\\Users\\user\\Documents\\projects\\LendingApp\\icons\\person.png";
            this.pictureBox2.Location = new System.Drawing.Point(16, 123);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 43);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox3.ImageLocation = "C:\\Users\\user\\Documents\\projects\\LendingApp\\icons\\person.png";
            this.pictureBox3.Location = new System.Drawing.Point(16, 198);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(40, 43);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.ImageLocation = "C:\\Users\\user\\Documents\\projects\\LendingApp\\icons\\Save.png";
            this.pictureBox4.Location = new System.Drawing.Point(299, 242);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(52, 50);
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            // 
            // CustomerManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gpCustomerList);
            this.Controls.Add(this.txtSearchCustomers);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblCustomerManagement);
            this.Name = "CustomerManagement";
            this.Text = "CustomerManagement";
            this.gpCustomerList.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomerManagement;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchCustomers;
        private System.Windows.Forms.GroupBox gpCustomerList;
        private System.Windows.Forms.Button btnCustomer1;
        private System.Windows.Forms.Button btnCustomer3;
        private System.Windows.Forms.Button btnCustomer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblCustomerDetails;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label lblCreditScore;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label lblIncome;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}