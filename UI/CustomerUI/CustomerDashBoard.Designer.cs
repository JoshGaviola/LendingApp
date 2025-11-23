namespace LendingApp.UI.CustomerUI
{
    partial class CustomerDashBoard
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
            this.CustDashHeaderPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // CustDashHeaderPanel
            // 
            this.CustDashHeaderPanel.Location = new System.Drawing.Point(1, 0);
            this.CustDashHeaderPanel.Name = "CustDashHeaderPanel";
            this.CustDashHeaderPanel.Size = new System.Drawing.Size(1029, 60);
            this.CustDashHeaderPanel.TabIndex = 0;
            this.CustDashHeaderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CustDashHeaderPanel_Paint);
            // 
            // CustomerDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 645);
            this.Controls.Add(this.CustDashHeaderPanel);
            this.Name = "CustomerDashBoard";
            this.Text = "f";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel CustDashHeaderPanel;
    }
}