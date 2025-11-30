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
            this.components = new Container();

            this.headerPanel = new Panel();
            this.lblTitle = new Label();
            this.lblWelcome = new Label();
            this.btnNotifications = new Button();
            this.btnLogout = new Button();

            this.summaryPanel = new Panel();
            this.cardPending = new Panel();
            this.lblPendingTitle = new Label();
            this.lblPendingCount = new Label();
            this.lblPendingSub = new Label();

            this.cardActive = new Panel();
            this.lblActiveTitle = new Label();
            this.lblActiveValue = new Label();
            this.lblActiveSub = new Label();

            this.cardOverdue = new Panel();
            this.lblOverdueTitle = new Label();
            this.lblOverdueCount = new Label();
            this.lblOverdueSub = new Label();

            this.cardCollections = new Panel();
            this.lblCollectionsTitle = new Label();
            this.lblCollectionsValue = new Label();
            this.lblCollectionsSub = new Label();

            this.sidebarPanel = new Panel();
            this.contentPanel = new Panel();

            this.sectionPending = new Panel();
            this.sectionOverdue = new Panel();
            this.sectionTasks = new Panel();
            this.sectionActivity = new Panel();

            // Form basics
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Officer Dashboard";

            // Header panel
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblWelcome);
            this.headerPanel.Controls.Add(this.btnNotifications);
            this.headerPanel.Controls.Add(this.btnLogout);

            // Summary cards
            this.summaryPanel.Controls.Add(this.cardPending);
            this.summaryPanel.Controls.Add(this.cardActive);
            this.summaryPanel.Controls.Add(this.cardOverdue);
            this.summaryPanel.Controls.Add(this.cardCollections);

            // Add content sections container
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }
    }
}