using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    partial class OfficerApplications
    {
        private IContainer components = null;

        private Panel headerPanel;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private Button btnNewApplication;

        private Panel filtersPanel;
        private ComboBox cmbStatus;
        private ComboBox cmbType;
        private TextBox txtSearch;
        private Label lblResults;

        private DataGridView gridApplications;

        private Panel statsPanel;
        private Panel cardTotal;
        private Label lblTotalTitle;
        private Label lblTotal;

        private Panel cardPending;
        private Label lblPendingTitle;
        private Label lblPending;

        private Panel cardReview;
        private Label lblReviewTitle;
        private Label lblReview;

        private Panel cardApproved;
        private Label lblApprovedTitle;
        private Label lblApproved;

        private Panel cardDisbursed;
        private Label lblDisbursedTitle;
        private Label lblDisbursed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.btnNewApplication = new System.Windows.Forms.Button();
            this.filtersPanel = new System.Windows.Forms.Panel();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.gridApplications = new System.Windows.Forms.DataGridView();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.cardTotal = new System.Windows.Forms.Panel();
            this.lblTotalTitle = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.cardPending = new System.Windows.Forms.Panel();
            this.lblPendingTitle = new System.Windows.Forms.Label();
            this.lblPending = new System.Windows.Forms.Label();
            this.cardReview = new System.Windows.Forms.Panel();
            this.lblReviewTitle = new System.Windows.Forms.Label();
            this.lblReview = new System.Windows.Forms.Label();
            this.cardApproved = new System.Windows.Forms.Panel();
            this.lblApprovedTitle = new System.Windows.Forms.Label();
            this.lblApproved = new System.Windows.Forms.Label();
            this.cardDisbursed = new System.Windows.Forms.Panel();
            this.lblDisbursedTitle = new System.Windows.Forms.Label();
            this.lblDisbursed = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.filtersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridApplications)).BeginInit();
            this.statsPanel.SuspendLayout();
            this.cardTotal.SuspendLayout();
            this.cardPending.SuspendLayout();
            this.cardReview.SuspendLayout();
            this.cardApproved.SuspendLayout();
            this.cardDisbursed.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Controls.Add(this.lblHeaderSubtitle);
            this.headerPanel.Controls.Add(this.btnNewApplication);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1200, 80);
            this.headerPanel.TabIndex = 3;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Location = new System.Drawing.Point(16, 12);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(91, 13);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "Loan Applications";
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(16, 40);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(229, 13);
            this.lblHeaderSubtitle.TabIndex = 1;
            this.lblHeaderSubtitle.Text = "Manage and review customer loan applications";
            // 
            // btnNewApplication
            // 
            this.btnNewApplication.Location = new System.Drawing.Point(1000, 20);
            this.btnNewApplication.Name = "btnNewApplication";
            this.btnNewApplication.Size = new System.Drawing.Size(160, 35);
            this.btnNewApplication.TabIndex = 2;
            this.btnNewApplication.Text = "New Application";
            this.btnNewApplication.Click += new System.EventHandler(this.btnNewApplication_Click);
            // 
            // filtersPanel
            // 
            this.filtersPanel.BackColor = System.Drawing.Color.White;
            this.filtersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filtersPanel.Controls.Add(this.cmbStatus);
            this.filtersPanel.Controls.Add(this.cmbType);
            this.filtersPanel.Controls.Add(this.txtSearch);
            this.filtersPanel.Controls.Add(this.lblResults);
            this.filtersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filtersPanel.Location = new System.Drawing.Point(0, 80);
            this.filtersPanel.Name = "filtersPanel";
            this.filtersPanel.Size = new System.Drawing.Size(1200, 100);
            this.filtersPanel.TabIndex = 2;
            // 
            // cmbStatus
            // 
            this.cmbStatus.Location = new System.Drawing.Point(16, 16);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(180, 21);
            this.cmbStatus.TabIndex = 0;
            // 
            // cmbType
            // 
            this.cmbType.Location = new System.Drawing.Point(210, 16);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(180, 21);
            this.cmbType.TabIndex = 1;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(404, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(400, 20);
            this.txtSearch.TabIndex = 2;
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(16, 56);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(137, 13);
            this.lblResults.TabIndex = 3;
            this.lblResults.Text = "Showing 0 of 0 applications";
            // 
            // gridApplications
            // 
            this.gridApplications.BackgroundColor = System.Drawing.Color.White;
            this.gridApplications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridApplications.Location = new System.Drawing.Point(0, 180);
            this.gridApplications.Name = "gridApplications";
            this.gridApplications.Size = new System.Drawing.Size(1200, 520);
            this.gridApplications.TabIndex = 0;
            // 
            // statsPanel
            // 
            this.statsPanel.BackColor = System.Drawing.Color.White;
            this.statsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statsPanel.Controls.Add(this.cardTotal);
            this.statsPanel.Controls.Add(this.cardPending);
            this.statsPanel.Controls.Add(this.cardReview);
            this.statsPanel.Controls.Add(this.cardApproved);
            this.statsPanel.Controls.Add(this.cardDisbursed);
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statsPanel.Location = new System.Drawing.Point(0, 700);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1200, 100);
            this.statsPanel.TabIndex = 1;
            // 
            // cardTotal
            // 
            this.cardTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotal.Controls.Add(this.lblTotalTitle);
            this.cardTotal.Controls.Add(this.lblTotal);
            this.cardTotal.Location = new System.Drawing.Point(16, 16);
            this.cardTotal.Name = "cardTotal";
            this.cardTotal.Size = new System.Drawing.Size(200, 68);
            this.cardTotal.TabIndex = 0;
            // 
            // lblTotalTitle
            // 
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Location = new System.Drawing.Point(10, 8);
            this.lblTotalTitle.Name = "lblTotalTitle";
            this.lblTotalTitle.Size = new System.Drawing.Size(31, 13);
            this.lblTotalTitle.TabIndex = 0;
            this.lblTotalTitle.Text = "Total";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(10, 34);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(13, 13);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "0";
            // 
            // cardPending
            // 
            this.cardPending.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardPending.Controls.Add(this.lblPendingTitle);
            this.cardPending.Controls.Add(this.lblPending);
            this.cardPending.Location = new System.Drawing.Point(226, 16);
            this.cardPending.Name = "cardPending";
            this.cardPending.Size = new System.Drawing.Size(200, 68);
            this.cardPending.TabIndex = 1;
            // 
            // lblPendingTitle
            // 
            this.lblPendingTitle.AutoSize = true;
            this.lblPendingTitle.Location = new System.Drawing.Point(10, 8);
            this.lblPendingTitle.Name = "lblPendingTitle";
            this.lblPendingTitle.Size = new System.Drawing.Size(46, 13);
            this.lblPendingTitle.TabIndex = 0;
            this.lblPendingTitle.Text = "Pending";
            // 
            // lblPending
            // 
            this.lblPending.AutoSize = true;
            this.lblPending.Location = new System.Drawing.Point(10, 34);
            this.lblPending.Name = "lblPending";
            this.lblPending.Size = new System.Drawing.Size(13, 13);
            this.lblPending.TabIndex = 1;
            this.lblPending.Text = "0";
            // 
            // cardReview
            // 
            this.cardReview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardReview.Controls.Add(this.lblReviewTitle);
            this.cardReview.Controls.Add(this.lblReview);
            this.cardReview.Location = new System.Drawing.Point(436, 16);
            this.cardReview.Name = "cardReview";
            this.cardReview.Size = new System.Drawing.Size(200, 68);
            this.cardReview.TabIndex = 2;
            // 
            // lblReviewTitle
            // 
            this.lblReviewTitle.AutoSize = true;
            this.lblReviewTitle.Location = new System.Drawing.Point(10, 8);
            this.lblReviewTitle.Name = "lblReviewTitle";
            this.lblReviewTitle.Size = new System.Drawing.Size(55, 13);
            this.lblReviewTitle.TabIndex = 0;
            this.lblReviewTitle.Text = "In Review";
            // 
            // lblReview
            // 
            this.lblReview.AutoSize = true;
            this.lblReview.Location = new System.Drawing.Point(10, 34);
            this.lblReview.Name = "lblReview";
            this.lblReview.Size = new System.Drawing.Size(13, 13);
            this.lblReview.TabIndex = 1;
            this.lblReview.Text = "0";
            // 
            // cardApproved
            // 
            this.cardApproved.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardApproved.Controls.Add(this.lblApprovedTitle);
            this.cardApproved.Controls.Add(this.lblApproved);
            this.cardApproved.Location = new System.Drawing.Point(646, 16);
            this.cardApproved.Name = "cardApproved";
            this.cardApproved.Size = new System.Drawing.Size(200, 68);
            this.cardApproved.TabIndex = 3;
            // 
            // lblApprovedTitle
            // 
            this.lblApprovedTitle.AutoSize = true;
            this.lblApprovedTitle.Location = new System.Drawing.Point(10, 8);
            this.lblApprovedTitle.Name = "lblApprovedTitle";
            this.lblApprovedTitle.Size = new System.Drawing.Size(53, 13);
            this.lblApprovedTitle.TabIndex = 0;
            this.lblApprovedTitle.Text = "Approved";
            // 
            // lblApproved
            // 
            this.lblApproved.AutoSize = true;
            this.lblApproved.Location = new System.Drawing.Point(10, 34);
            this.lblApproved.Name = "lblApproved";
            this.lblApproved.Size = new System.Drawing.Size(13, 13);
            this.lblApproved.TabIndex = 1;
            this.lblApproved.Text = "0";
            // 
            // cardDisbursed
            // 
            this.cardDisbursed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardDisbursed.Controls.Add(this.lblDisbursedTitle);
            this.cardDisbursed.Controls.Add(this.lblDisbursed);
            this.cardDisbursed.Location = new System.Drawing.Point(856, 16);
            this.cardDisbursed.Name = "cardDisbursed";
            this.cardDisbursed.Size = new System.Drawing.Size(200, 68);
            this.cardDisbursed.TabIndex = 4;
            // 
            // lblDisbursedTitle
            // 
            this.lblDisbursedTitle.AutoSize = true;
            this.lblDisbursedTitle.Location = new System.Drawing.Point(10, 8);
            this.lblDisbursedTitle.Name = "lblDisbursedTitle";
            this.lblDisbursedTitle.Size = new System.Drawing.Size(54, 13);
            this.lblDisbursedTitle.TabIndex = 0;
            this.lblDisbursedTitle.Text = "Disbursed";
            // 
            // lblDisbursed
            // 
            this.lblDisbursed.AutoSize = true;
            this.lblDisbursed.Location = new System.Drawing.Point(10, 34);
            this.lblDisbursed.Name = "lblDisbursed";
            this.lblDisbursed.Size = new System.Drawing.Size(13, 13);
            this.lblDisbursed.TabIndex = 1;
            this.lblDisbursed.Text = "0";
            // 
            // OfficerApplications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.gridApplications);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.filtersPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "OfficerApplications";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Officer Applications";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.filtersPanel.ResumeLayout(false);
            this.filtersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridApplications)).EndInit();
            this.statsPanel.ResumeLayout(false);
            this.cardTotal.ResumeLayout(false);
            this.cardTotal.PerformLayout();
            this.cardPending.ResumeLayout(false);
            this.cardPending.PerformLayout();
            this.cardReview.ResumeLayout(false);
            this.cardReview.PerformLayout();
            this.cardApproved.ResumeLayout(false);
            this.cardApproved.PerformLayout();
            this.cardDisbursed.ResumeLayout(false);
            this.cardDisbursed.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}