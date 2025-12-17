namespace LendingApp
{
    partial class CashierLogin
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
            this.CustomerLoginLabel = new System.Windows.Forms.Label();
            this.EnterCredentialsLbl = new System.Windows.Forms.Label();
            this.PanelLeftSideCustLogin = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SignInBtn = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtboxUserName = new System.Windows.Forms.TextBox();
            this.txtboxPassword = new System.Windows.Forms.TextBox();
            this.lblRegister = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CustomerLoginLabel
            // 
            this.CustomerLoginLabel.AutoSize = true;
            this.CustomerLoginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerLoginLabel.ForeColor = System.Drawing.Color.OrangeRed;
            this.CustomerLoginLabel.Location = new System.Drawing.Point(379, 42);
            this.CustomerLoginLabel.Name = "CustomerLoginLabel";
            this.CustomerLoginLabel.Size = new System.Drawing.Size(197, 29);
            this.CustomerLoginLabel.TabIndex = 1;
            this.CustomerLoginLabel.Text = "Customer Login";
            // 
            // EnterCredentialsLbl
            // 
            this.EnterCredentialsLbl.AutoSize = true;
            this.EnterCredentialsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterCredentialsLbl.Location = new System.Drawing.Point(383, 75);
            this.EnterCredentialsLbl.Name = "EnterCredentialsLbl";
            this.EnterCredentialsLbl.Size = new System.Drawing.Size(322, 18);
            this.EnterCredentialsLbl.TabIndex = 2;
            this.EnterCredentialsLbl.Text = " Enter your credentials to access the dashboard";
            // 
            // PanelLeftSideCustLogin
            // 
            this.PanelLeftSideCustLogin.Location = new System.Drawing.Point(0, 0);
            this.PanelLeftSideCustLogin.Name = "PanelLeftSideCustLogin";
            this.PanelLeftSideCustLogin.Size = new System.Drawing.Size(357, 549);
            this.PanelLeftSideCustLogin.TabIndex = 3;
            this.PanelLeftSideCustLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelLeftSideCustLogin_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(380, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(380, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password";
            // 
            // SignInBtn
            // 
            this.SignInBtn.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SignInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SignInBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignInBtn.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SignInBtn.Location = new System.Drawing.Point(384, 440);
            this.SignInBtn.Name = "SignInBtn";
            this.SignInBtn.Size = new System.Drawing.Size(280, 49);
            this.SignInBtn.TabIndex = 7;
            this.SignInBtn.Text = "Sign in";
            this.SignInBtn.UseVisualStyleBackColor = false;
            this.SignInBtn.Click += new System.EventHandler(this.SignInBtn_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(386, 402);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Remember me";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtboxUserName
            // 
            this.txtboxUserName.Location = new System.Drawing.Point(383, 161);
            this.txtboxUserName.Multiline = true;
            this.txtboxUserName.Name = "txtboxUserName";
            this.txtboxUserName.Size = new System.Drawing.Size(281, 39);
            this.txtboxUserName.TabIndex = 9;
            // 
            // txtboxPassword
            // 
            this.txtboxPassword.Location = new System.Drawing.Point(383, 277);
            this.txtboxPassword.Multiline = true;
            this.txtboxPassword.Name = "txtboxPassword";
            this.txtboxPassword.Size = new System.Drawing.Size(281, 39);
            this.txtboxPassword.TabIndex = 10;
            // 
            // lblRegister
            // 
            this.lblRegister.AutoSize = true;
            this.lblRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegister.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblRegister.Location = new System.Drawing.Point(606, 403);
            this.lblRegister.Name = "lblRegister";
            this.lblRegister.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblRegister.Size = new System.Drawing.Size(58, 16);
            this.lblRegister.TabIndex = 11;
            this.lblRegister.Text = "Register";
            this.lblRegister.Click += new System.EventHandler(this.lblRegister_Click);
            // 
            // CustomerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 536);
            this.Controls.Add(this.lblRegister);
            this.Controls.Add(this.txtboxPassword);
            this.Controls.Add(this.txtboxUserName);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.SignInBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PanelLeftSideCustLogin);
            this.Controls.Add(this.EnterCredentialsLbl);
            this.Controls.Add(this.CustomerLoginLabel);
            this.Name = "CustomerLogin";
            this.Text = "CustomerLogin";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomerLogin_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CustomerLogin_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomerLogin_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label CustomerLoginLabel;
        private System.Windows.Forms.Label EnterCredentialsLbl;
        private System.Windows.Forms.Panel PanelLeftSideCustLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SignInBtn;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtboxUserName;
        private System.Windows.Forms.TextBox txtboxPassword;
        private System.Windows.Forms.Label lblRegister;
    }
}

