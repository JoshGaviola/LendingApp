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
            this.SuspendLayout();
            // 
            // CustomerLoginLabel
            // 
            this.CustomerLoginLabel.AutoSize = true;
            this.CustomerLoginLabel.ForeColor = System.Drawing.Color.BurlyWood;
            this.CustomerLoginLabel.Location = new System.Drawing.Point(380, 86);
            this.CustomerLoginLabel.Name = "CustomerLoginLabel";
            this.CustomerLoginLabel.Size = new System.Drawing.Size(77, 13);
            this.CustomerLoginLabel.TabIndex = 1;
            this.CustomerLoginLabel.Text = "CustomerLogin";
            // 
            // CustomerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 536);
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
    }
}

