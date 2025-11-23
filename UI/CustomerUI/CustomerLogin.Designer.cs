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
            this.SuspendLayout();
            // 
            // CustomerLoginLabel
            // 
            this.CustomerLoginLabel.AutoSize = true;
            this.CustomerLoginLabel.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerLoginLabel.ForeColor = System.Drawing.Color.OrangeRed;
            this.CustomerLoginLabel.Location = new System.Drawing.Point(379, 42);
            this.CustomerLoginLabel.Name = "CustomerLoginLabel";
            this.CustomerLoginLabel.Size = new System.Drawing.Size(222, 29);
            this.CustomerLoginLabel.TabIndex = 1;
            this.CustomerLoginLabel.Text = "Customer Login";
            // 
            // EnterCredentialsLbl
            // 
            this.EnterCredentialsLbl.AutoSize = true;
            this.EnterCredentialsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterCredentialsLbl.Location = new System.Drawing.Point(381, 71);
            this.EnterCredentialsLbl.Name = "EnterCredentialsLbl";
            this.EnterCredentialsLbl.Size = new System.Drawing.Size(322, 18);
            this.EnterCredentialsLbl.TabIndex = 2;
            this.EnterCredentialsLbl.Text = " Enter your credentials to access the dashboard";
            // 
            // PanelLeftSideCustLogin
            // 
            this.PanelLeftSideCustLogin.Location = new System.Drawing.Point(0, 0);
            this.PanelLeftSideCustLogin.Name = "PanelLeftSideCustLogin";
            this.PanelLeftSideCustLogin.Size = new System.Drawing.Size(357, 590);
            this.PanelLeftSideCustLogin.TabIndex = 3;
            this.PanelLeftSideCustLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelLeftSideCustLogin_Paint);
            // 
            // CustomerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 590);
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
    }
}

