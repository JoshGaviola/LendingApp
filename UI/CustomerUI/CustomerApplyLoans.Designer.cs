namespace LendingApp.UI.CustomerUI
{
    partial class CustomerApplyLoans
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
            this.lblApplyForLoan = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblApplyForLoan
            // 
            this.lblApplyForLoan.AutoSize = true;
            this.lblApplyForLoan.Location = new System.Drawing.Point(332, 9);
            this.lblApplyForLoan.Name = "lblApplyForLoan";
            this.lblApplyForLoan.Size = new System.Drawing.Size(78, 13);
            this.lblApplyForLoan.TabIndex = 0;
            this.lblApplyForLoan.Text = "Apply For Loan";
            this.lblApplyForLoan.Click += new System.EventHandler(this.lblApplyForLoan_Click);
            // 
            // CustomerApplyLoans
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblApplyForLoan);
            this.Name = "CustomerApplyLoans";
            this.Text = "CustomerApplyLoans";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblApplyForLoan;
    }
}