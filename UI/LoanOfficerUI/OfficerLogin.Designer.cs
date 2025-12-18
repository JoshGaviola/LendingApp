namespace LendingApp.UI.LoanOfficerUI
{
    partial class OfficerLogin
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
            this.lblRegister = new System.Windows.Forms.Label();
            this.txtboxPassword = new System.Windows.Forms.TextBox();
            this.txtboxUserName = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SignInBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelLeftSideCustLogin = new System.Windows.Forms.Panel();
            this.EnterCredentialsLbl = new System.Windows.Forms.Label();
            this.OfficerLoginLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRegister
            // 
            this.lblRegister.AutoSize = true;
            this.lblRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegister.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblRegister.Location = new System.Drawing.Point(606, 402);
            this.lblRegister.Name = "lblRegister";
            this.lblRegister.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblRegister.Size = new System.Drawing.Size(58, 16);
            this.lblRegister.TabIndex = 21;
            this.lblRegister.Text = "Register";
            // 
            // txtboxPassword
            // 
            this.txtboxPassword.Location = new System.Drawing.Point(383, 276);
            this.txtboxPassword.Multiline = true;
            this.txtboxPassword.Name = "txtboxPassword";
            this.txtboxPassword.Size = new System.Drawing.Size(281, 39);
            this.txtboxPassword.TabIndex = 20;
            // 
            // txtboxUserName
            // 
            this.txtboxUserName.Location = new System.Drawing.Point(383, 160);
            this.txtboxUserName.Multiline = true;
            this.txtboxUserName.Name = "txtboxUserName";
            this.txtboxUserName.Size = new System.Drawing.Size(281, 39);
            this.txtboxUserName.TabIndex = 19;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(386, 401);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 18;
            this.checkBox1.Text = "Remember me";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // SignInBtn
            // 
            this.SignInBtn.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SignInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SignInBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignInBtn.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SignInBtn.Location = new System.Drawing.Point(384, 439);
            this.SignInBtn.Name = "SignInBtn";
            this.SignInBtn.Size = new System.Drawing.Size(280, 49);
            this.SignInBtn.TabIndex = 17;
            this.SignInBtn.Text = "Sign in";
            this.SignInBtn.UseVisualStyleBackColor = false;
            this.SignInBtn.Click += new System.EventHandler(this.SignInBtn_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(380, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(380, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 15;
            this.label1.Text = "Username";
            // 
            // PanelLeftSideCustLogin
            // 
            this.PanelLeftSideCustLogin.Location = new System.Drawing.Point(0, 0);
            this.PanelLeftSideCustLogin.Name = "PanelLeftSideCustLogin";
            this.PanelLeftSideCustLogin.Size = new System.Drawing.Size(357, 549);
            this.PanelLeftSideCustLogin.TabIndex = 3;
            this.PanelLeftSideCustLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelLeftSideCustLogin_Paint);
            // 
            // EnterCredentialsLbl
            // 
            this.EnterCredentialsLbl.AutoSize = true;
            this.EnterCredentialsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterCredentialsLbl.Location = new System.Drawing.Point(383, 74);
            this.EnterCredentialsLbl.Name = "EnterCredentialsLbl";
            this.EnterCredentialsLbl.Size = new System.Drawing.Size(322, 18);
            this.EnterCredentialsLbl.TabIndex = 13;
            this.EnterCredentialsLbl.Text = " Enter your credentials to access the dashboard";
            // 
            // OfficerLoginLabel
            // 
            this.OfficerLoginLabel.AutoSize = true;
            this.OfficerLoginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfficerLoginLabel.ForeColor = System.Drawing.Color.OrangeRed;
            this.OfficerLoginLabel.Location = new System.Drawing.Point(379, 41);
            this.OfficerLoginLabel.Name = "OfficerLoginLabel";
            this.OfficerLoginLabel.Size = new System.Drawing.Size(163, 29);
            this.OfficerLoginLabel.TabIndex = 12;
            this.OfficerLoginLabel.Text = "Officer Login";
            // 
            // OfficerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 506);
            this.Controls.Add(this.lblRegister);
            this.Controls.Add(this.txtboxPassword);
            this.Controls.Add(this.txtboxUserName);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.SignInBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PanelLeftSideCustLogin);
            this.Controls.Add(this.EnterCredentialsLbl);
            this.Controls.Add(this.OfficerLoginLabel);
            this.Name = "OfficerLogin";
            this.Text = "OfficerLogin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRegister;
        private System.Windows.Forms.TextBox txtboxPassword;
        private System.Windows.Forms.TextBox txtboxUserName;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button SignInBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PanelLeftSideCustLogin;
        private System.Windows.Forms.Label EnterCredentialsLbl;
        private System.Windows.Forms.Label OfficerLoginLabel;
    }
}