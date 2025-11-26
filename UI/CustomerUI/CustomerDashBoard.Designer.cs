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
            this.label1 = new System.Windows.Forms.Label();
            this.CustomerDashBHeaderPanel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.CustDashHeaderPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // CustDashHeaderPanel
            // 
            this.CustDashHeaderPanel.Controls.Add(this.label1);
            this.CustDashHeaderPanel.Controls.Add(this.CustomerDashBHeaderPanel);
            this.CustDashHeaderPanel.Location = new System.Drawing.Point(-2, 0);
            this.CustDashHeaderPanel.Margin = new System.Windows.Forms.Padding(4);
            this.CustDashHeaderPanel.Name = "CustDashHeaderPanel";
            this.CustDashHeaderPanel.Size = new System.Drawing.Size(1605, 92);
            this.CustDashHeaderPanel.TabIndex = 0;
            this.CustDashHeaderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CustDashHeaderPanel_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Poppins Medium", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dashboard";
            // 
            // CustomerDashBHeaderPanel
            // 
            this.CustomerDashBHeaderPanel.AutoSize = true;
            this.CustomerDashBHeaderPanel.BackColor = System.Drawing.Color.Transparent;
            this.CustomerDashBHeaderPanel.Font = new System.Drawing.Font("Poppins", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerDashBHeaderPanel.Location = new System.Drawing.Point(28, 18);
            this.CustomerDashBHeaderPanel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CustomerDashBHeaderPanel.Name = "CustomerDashBHeaderPanel";
            this.CustomerDashBHeaderPanel.Size = new System.Drawing.Size(243, 37);
            this.CustomerDashBHeaderPanel.TabIndex = 0;
            this.CustomerDashBHeaderPanel.Text = "Customer Dashboard";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Location = new System.Drawing.Point(321, 142);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(372, 144);
            this.panel1.TabIndex = 1;
            this.panel1.Tag = "DashBoardBox";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderGray);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Poppins", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(14, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 22);
            this.label2.TabIndex = 1;
            this.label2.Tag = "insidePanelGray";
            this.label2.Text = "Total Borrowed";
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(372, 10);
            this.panel4.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Location = new System.Drawing.Point(721, 142);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(363, 144);
            this.panel2.TabIndex = 2;
            this.panel2.Tag = "DashBoardBox";
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderGray);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Poppins Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(19, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 22);
            this.label3.TabIndex = 2;
            this.label3.Tag = "insidePanelGray";
            this.label3.Text = "Outstanding Balance";
            // 
            // panel5
            // 
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(363, 10);
            this.panel5.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Location = new System.Drawing.Point(1108, 142);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(369, 144);
            this.panel3.TabIndex = 3;
            this.panel3.Tag = "DashBoardBox";
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.BorderGray);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Poppins Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(19, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 22);
            this.label4.TabIndex = 3;
            this.label4.Tag = "insidePanelGray";
            this.label4.Text = "Monthly Payment";
            // 
            // panel6
            // 
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(369, 10);
            this.panel6.TabIndex = 2;
            // 
            // panel7
            // 
            this.panel7.Location = new System.Drawing.Point(321, 324);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(678, 206);
            this.panel7.TabIndex = 4;
            // 
            // panel8
            // 
            this.panel8.Location = new System.Drawing.Point(1036, 324);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(415, 206);
            this.panel8.TabIndex = 5;
            // 
            // panel9
            // 
            this.panel9.Location = new System.Drawing.Point(321, 563);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(975, 247);
            this.panel9.TabIndex = 6;
            // 
            // panel10
            // 
            this.panel10.Location = new System.Drawing.Point(-2, 91);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(258, 763);
            this.panel10.TabIndex = 7;
            // 
            // CustomerDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1502, 855);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.CustDashHeaderPanel);
            this.Font = new System.Drawing.Font("Poppins", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CustomerDashBoard";
            this.Text = "Customer dashboard";
            this.CustDashHeaderPanel.ResumeLayout(false);
            this.CustDashHeaderPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel CustDashHeaderPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label CustomerDashBHeaderPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
    }
}