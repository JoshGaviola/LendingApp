using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    partial class OfficerDashboard
    {
        private IContainer components = null;

        private Panel headerPanel;
        private Label lblTitle;
        private Label lblWelcome;
        private Button btnNotifications;
        private Button btnLogout;

        private Panel summaryPanel;
        private Panel cardPending;
        private Label lblPendingTitle;
        private Label lblPendingCount;
        private Label lblPendingSub;

        private Panel cardActive;
        private Label lblActiveTitle;
        private Label lblActiveValue;
        private Label lblActiveSub;

        private Panel cardOverdue;
        private Label lblOverdueTitle;
        private Label lblOverdueCount;
        private Label lblOverdueSub;

        private Panel cardCollections;
        private Label lblCollectionsTitle;
        private Label lblCollectionsValue;
        private Label lblCollectionsSub;

        private Panel sidebarPanel;
        private Panel contentPanel;

        private Panel sectionPending;
        private Panel sectionOverdue;
        private Panel sectionTasks;
        private Panel sectionActivity;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnNotifications = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.cardPending = new System.Windows.Forms.Panel();
            this.cardActive = new System.Windows.Forms.Panel();
            this.cardOverdue = new System.Windows.Forms.Panel();
            this.cardCollections = new System.Windows.Forms.Panel();
            this.lblPendingTitle = new System.Windows.Forms.Label();
            this.lblPendingCount = new System.Windows.Forms.Label();
            this.lblPendingSub = new System.Windows.Forms.Label();
            this.lblActiveTitle = new System.Windows.Forms.Label();
            this.lblActiveValue = new System.Windows.Forms.Label();
            this.lblActiveSub = new System.Windows.Forms.Label();
            this.lblOverdueTitle = new System.Windows.Forms.Label();
            this.lblOverdueCount = new System.Windows.Forms.Label();
            this.lblOverdueSub = new System.Windows.Forms.Label();
            this.lblCollectionsTitle = new System.Windows.Forms.Label();
            this.lblCollectionsValue = new System.Windows.Forms.Label();
            this.lblCollectionsSub = new System.Windows.Forms.Label();
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.sectionPending = new System.Windows.Forms.Panel();
            this.sectionOverdue = new System.Windows.Forms.Panel();
            this.sectionTasks = new System.Windows.Forms.Panel();
            this.sectionActivity = new System.Windows.Forms.Panel();
            this.headerPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblWelcome);
            this.headerPanel.Controls.Add(this.btnNotifications);
            this.headerPanel.Controls.Add(this.btnLogout);
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(200, 100);
            this.headerPanel.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(100, 23);
            this.lblTitle.TabIndex = 0;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Location = new System.Drawing.Point(0, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(100, 23);
            this.lblWelcome.TabIndex = 1;
            // 
            // btnNotifications
            // 
            this.btnNotifications.Location = new System.Drawing.Point(0, 0);
            this.btnNotifications.Name = "btnNotifications";
            this.btnNotifications.Size = new System.Drawing.Size(75, 23);
            this.btnNotifications.TabIndex = 2;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(0, 0);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 3;
            // 
            // summaryPanel
            // 
            this.summaryPanel.Controls.Add(this.cardPending);
            this.summaryPanel.Controls.Add(this.cardActive);
            this.summaryPanel.Controls.Add(this.cardOverdue);
            this.summaryPanel.Controls.Add(this.cardCollections);
            this.summaryPanel.Location = new System.Drawing.Point(0, 0);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(200, 100);
            this.summaryPanel.TabIndex = 2;
            // 
            // cardPending
            // 
            this.cardPending.Location = new System.Drawing.Point(0, 0);
            this.cardPending.Name = "cardPending";
            this.cardPending.Size = new System.Drawing.Size(200, 100);
            this.cardPending.TabIndex = 0;
            // 
            // cardActive
            // 
            this.cardActive.Location = new System.Drawing.Point(0, 0);
            this.cardActive.Name = "cardActive";
            this.cardActive.Size = new System.Drawing.Size(200, 100);
            this.cardActive.TabIndex = 1;
            // 
            // cardOverdue
            // 
            this.cardOverdue.Location = new System.Drawing.Point(0, 0);
            this.cardOverdue.Name = "cardOverdue";
            this.cardOverdue.Size = new System.Drawing.Size(200, 100);
            this.cardOverdue.TabIndex = 2;
            // 
            // cardCollections
            // 
            this.cardCollections.Location = new System.Drawing.Point(0, 0);
            this.cardCollections.Name = "cardCollections";
            this.cardCollections.Size = new System.Drawing.Size(200, 100);
            this.cardCollections.TabIndex = 3;
            // 
            // lblPendingTitle
            // 
            this.lblPendingTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPendingTitle.Name = "lblPendingTitle";
            this.lblPendingTitle.Size = new System.Drawing.Size(100, 23);
            this.lblPendingTitle.TabIndex = 0;
            // 
            // lblPendingCount
            // 
            this.lblPendingCount.Location = new System.Drawing.Point(0, 0);
            this.lblPendingCount.Name = "lblPendingCount";
            this.lblPendingCount.Size = new System.Drawing.Size(100, 23);
            this.lblPendingCount.TabIndex = 0;
            // 
            // lblPendingSub
            // 
            this.lblPendingSub.Location = new System.Drawing.Point(0, 0);
            this.lblPendingSub.Name = "lblPendingSub";
            this.lblPendingSub.Size = new System.Drawing.Size(100, 23);
            this.lblPendingSub.TabIndex = 0;
            // 
            // lblActiveTitle
            // 
            this.lblActiveTitle.Location = new System.Drawing.Point(0, 0);
            this.lblActiveTitle.Name = "lblActiveTitle";
            this.lblActiveTitle.Size = new System.Drawing.Size(100, 23);
            this.lblActiveTitle.TabIndex = 0;
            // 
            // lblActiveValue
            // 
            this.lblActiveValue.Location = new System.Drawing.Point(0, 0);
            this.lblActiveValue.Name = "lblActiveValue";
            this.lblActiveValue.Size = new System.Drawing.Size(100, 23);
            this.lblActiveValue.TabIndex = 0;
            // 
            // lblActiveSub
            // 
            this.lblActiveSub.Location = new System.Drawing.Point(0, 0);
            this.lblActiveSub.Name = "lblActiveSub";
            this.lblActiveSub.Size = new System.Drawing.Size(100, 23);
            this.lblActiveSub.TabIndex = 0;
            // 
            // lblOverdueTitle
            // 
            this.lblOverdueTitle.Location = new System.Drawing.Point(0, 0);
            this.lblOverdueTitle.Name = "lblOverdueTitle";
            this.lblOverdueTitle.Size = new System.Drawing.Size(100, 23);
            this.lblOverdueTitle.TabIndex = 0;
            // 
            // lblOverdueCount
            // 
            this.lblOverdueCount.Location = new System.Drawing.Point(0, 0);
            this.lblOverdueCount.Name = "lblOverdueCount";
            this.lblOverdueCount.Size = new System.Drawing.Size(100, 23);
            this.lblOverdueCount.TabIndex = 0;
            // 
            // lblOverdueSub
            // 
            this.lblOverdueSub.Location = new System.Drawing.Point(0, 0);
            this.lblOverdueSub.Name = "lblOverdueSub";
            this.lblOverdueSub.Size = new System.Drawing.Size(100, 23);
            this.lblOverdueSub.TabIndex = 0;
            // 
            // lblCollectionsTitle
            // 
            this.lblCollectionsTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCollectionsTitle.Name = "lblCollectionsTitle";
            this.lblCollectionsTitle.Size = new System.Drawing.Size(100, 23);
            this.lblCollectionsTitle.TabIndex = 0;
            // 
            // lblCollectionsValue
            // 
            this.lblCollectionsValue.Location = new System.Drawing.Point(0, 0);
            this.lblCollectionsValue.Name = "lblCollectionsValue";
            this.lblCollectionsValue.Size = new System.Drawing.Size(100, 23);
            this.lblCollectionsValue.TabIndex = 0;
            // 
            // lblCollectionsSub
            // 
            this.lblCollectionsSub.Location = new System.Drawing.Point(0, 0);
            this.lblCollectionsSub.Name = "lblCollectionsSub";
            this.lblCollectionsSub.Size = new System.Drawing.Size(100, 23);
            this.lblCollectionsSub.TabIndex = 0;
            // 
            // sidebarPanel
            // 
            this.sidebarPanel.Location = new System.Drawing.Point(0, 0);
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Size = new System.Drawing.Size(200, 100);
            this.sidebarPanel.TabIndex = 1;
            // 
            // contentPanel
            // 
            this.contentPanel.Location = new System.Drawing.Point(0, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(200, 100);
            this.contentPanel.TabIndex = 0;
            // 
            // sectionPending
            // 
            this.sectionPending.Location = new System.Drawing.Point(0, 0);
            this.sectionPending.Name = "sectionPending";
            this.sectionPending.Size = new System.Drawing.Size(200, 100);
            this.sectionPending.TabIndex = 0;
            // 
            // sectionOverdue
            // 
            this.sectionOverdue.Location = new System.Drawing.Point(0, 0);
            this.sectionOverdue.Name = "sectionOverdue";
            this.sectionOverdue.Size = new System.Drawing.Size(200, 100);
            this.sectionOverdue.TabIndex = 0;
            // 
            // sectionTasks
            // 
            this.sectionTasks.Location = new System.Drawing.Point(0, 0);
            this.sectionTasks.Name = "sectionTasks";
            this.sectionTasks.Size = new System.Drawing.Size(200, 100);
            this.sectionTasks.TabIndex = 0;
            // 
            // sectionActivity
            // 
            this.sectionActivity.Location = new System.Drawing.Point(0, 0);
            this.sectionActivity.Name = "sectionActivity";
            this.sectionActivity.Size = new System.Drawing.Size(200, 100);
            this.sectionActivity.TabIndex = 0;
            // 
            // OfficerDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "OfficerDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Officer Dashboard";
            this.headerPanel.ResumeLayout(false);
            this.summaryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}