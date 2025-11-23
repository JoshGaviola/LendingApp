namespace LendingApp
{
    partial class CustomerLogin
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
            this.UsernamePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.PassPanel = new System.Windows.Forms.Panel();
            this.SignInBtn = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CustomerLoginLabel
            // 
            this.CustomerLoginLabel.AutoSize = true;
            this.CustomerLoginLabel.Font = new System.Drawing.Font("Poppins", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerLoginLabel.ForeColor = System.Drawing.Color.OrangeRed;
            this.CustomerLoginLabel.Location = new System.Drawing.Point(379, 42);
            this.CustomerLoginLabel.Name = "CustomerLoginLabel";
            this.CustomerLoginLabel.Size = new System.Drawing.Size(210, 42);
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
            this.label1.Font = new System.Drawing.Font("Poppins", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(380, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Username";
            // 
            // UsernamePanel
            // 
            this.UsernamePanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.UsernamePanel.Location = new System.Drawing.Point(384, 168);
            this.UsernamePanel.Name = "UsernamePanel";
            this.UsernamePanel.Size = new System.Drawing.Size(282, 44);
            this.UsernamePanel.TabIndex = 5;
            this.UsernamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderGray);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Poppins", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(380, 258);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password";
            // 
            // PassPanel
            // 
            this.PassPanel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.PassPanel.Location = new System.Drawing.Point(384, 284);
            this.PassPanel.Name = "PassPanel";
            this.PassPanel.Size = new System.Drawing.Size(282, 44);
            this.PassPanel.TabIndex = 6;
            this.PassPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderGray);
            // 
            // SignInBtn
            // 
            this.SignInBtn.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SignInBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SignInBtn.Font = new System.Drawing.Font("Poppins SemiBold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignInBtn.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SignInBtn.Location = new System.Drawing.Point(384, 440);
            this.SignInBtn.Name = "SignInBtn";
            this.SignInBtn.Size = new System.Drawing.Size(280, 49);
            this.SignInBtn.TabIndex = 7;
            this.SignInBtn.Text = "Sign in";
            this.SignInBtn.UseVisualStyleBackColor = false;
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
            // CustomerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 536);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.SignInBtn);
            this.Controls.Add(this.PassPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UsernamePanel);
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
        private System.Windows.Forms.Panel UsernamePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PassPanel;
        private System.Windows.Forms.Button SignInBtn;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

