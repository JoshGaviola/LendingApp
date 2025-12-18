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
            this.components = new Container();

            this.headerPanel = new Panel();
            this.lblHeaderTitle = new Label();
            this.lblHeaderSubtitle = new Label();
            this.btnNewApplication = new Button();

            this.filtersPanel = new Panel();
            this.cmbStatus = new ComboBox();
            this.cmbType = new ComboBox();
            this.txtSearch = new TextBox();
            this.lblResults = new Label();

            this.gridApplications = new DataGridView();

            this.statsPanel = new Panel();
            this.cardTotal = new Panel();
            this.lblTotalTitle = new Label();
            this.lblTotal = new Label();

            this.cardPending = new Panel();
            this.lblPendingTitle = new Label();
            this.lblPending = new Label();

            this.cardReview = new Panel();
            this.lblReviewTitle = new Label();
            this.lblReview = new Label();

            this.cardApproved = new Panel();
            this.lblApprovedTitle = new Label();
            this.lblApproved = new Label();

            this.cardDisbursed = new Panel();
            this.lblDisbursedTitle = new Label();
            this.lblDisbursed = new Label();

            // Form
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Officer Applications";
            this.BackColor = Color.White;

            // HeaderPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 80;
            this.headerPanel.BackColor = Color.White;
            this.headerPanel.BorderStyle = BorderStyle.FixedSingle;

            this.lblHeaderTitle.Location = new Point(16, 12);
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Text = "Loan Applications";

            this.lblHeaderSubtitle.Location = new Point(16, 40);
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Text = "Manage and review customer loan applications";

            this.btnNewApplication.Location = new Point(1000, 20);
            this.btnNewApplication.Size = new Size(160, 35);
            this.btnNewApplication.Text = "New Application";

            this.headerPanel.Controls.Add(this.lblHeaderTitle);
            this.headerPanel.Controls.Add(this.lblHeaderSubtitle);
            this.headerPanel.Controls.Add(this.btnNewApplication);

            // FiltersPanel
            this.filtersPanel.Dock = DockStyle.Top;
            this.filtersPanel.Height = 100;
            this.filtersPanel.BackColor = Color.White;
            this.filtersPanel.BorderStyle = BorderStyle.FixedSingle;

            this.cmbStatus.Location = new Point(16, 16);
            this.cmbStatus.Size = new Size(180, 24);

            this.cmbType.Location = new Point(210, 16);
            this.cmbType.Size = new Size(180, 24);

            this.txtSearch.Location = new Point(404, 16);
            this.txtSearch.Size = new Size(400, 24);

            this.lblResults.Location = new Point(16, 56);
            this.lblResults.AutoSize = true;
            this.lblResults.Text = "Showing 0 of 0 applications";

            this.filtersPanel.Controls.Add(this.cmbStatus);
            this.filtersPanel.Controls.Add(this.cmbType);
            this.filtersPanel.Controls.Add(this.txtSearch);
            this.filtersPanel.Controls.Add(this.lblResults);

            // GridApplications
            this.gridApplications.Dock = DockStyle.Fill;
            this.gridApplications.BackgroundColor = Color.White;

            // StatsPanel
            this.statsPanel.Dock = DockStyle.Bottom;
            this.statsPanel.Height = 100;
            this.statsPanel.BackColor = Color.White;
            this.statsPanel.BorderStyle = BorderStyle.FixedSingle;

            // Card Total
            this.cardTotal.Location = new Point(16, 16);
            this.cardTotal.Size = new Size(200, 68);
            this.cardTotal.BorderStyle = BorderStyle.FixedSingle;
            this.lblTotalTitle.Location = new Point(10, 8);
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Text = "Total";
            this.lblTotal.Location = new Point(10, 34);
            this.lblTotal.AutoSize = true;
            this.lblTotal.Text = "0";
            this.cardTotal.Controls.Add(this.lblTotalTitle);
            this.cardTotal.Controls.Add(this.lblTotal);

            // Card Pending
            this.cardPending.Location = new Point(226, 16);
            this.cardPending.Size = new Size(200, 68);
            this.cardPending.BorderStyle = BorderStyle.FixedSingle;
            this.lblPendingTitle.Location = new Point(10, 8);
            this.lblPendingTitle.AutoSize = true;
            this.lblPendingTitle.Text = "Pending";
            this.lblPending.Location = new Point(10, 34);
            this.lblPending.AutoSize = true;
            this.lblPending.Text = "0";
            this.cardPending.Controls.Add(this.lblPendingTitle);
            this.cardPending.Controls.Add(this.lblPending);

            // Card Review
            this.cardReview.Location = new Point(436, 16);
            this.cardReview.Size = new Size(200, 68);
            this.cardReview.BorderStyle = BorderStyle.FixedSingle;
            this.lblReviewTitle.Location = new Point(10, 8);
            this.lblReviewTitle.AutoSize = true;
            this.lblReviewTitle.Text = "In Review";
            this.lblReview.Location = new Point(10, 34);
            this.lblReview.AutoSize = true;
            this.lblReview.Text = "0";
            this.cardReview.Controls.Add(this.lblReviewTitle);
            this.cardReview.Controls.Add(this.lblReview);

            // Card Approved
            this.cardApproved.Location = new Point(646, 16);
            this.cardApproved.Size = new Size(200, 68);
            this.cardApproved.BorderStyle = BorderStyle.FixedSingle;
            this.lblApprovedTitle.Location = new Point(10, 8);
            this.lblApprovedTitle.AutoSize = true;
            this.lblApprovedTitle.Text = "Approved";
            this.lblApproved.Location = new Point(10, 34);
            this.lblApproved.AutoSize = true;
            this.lblApproved.Text = "0";
            this.cardApproved.Controls.Add(this.lblApprovedTitle);
            this.cardApproved.Controls.Add(this.lblApproved);

            // Card Disbursed
            this.cardDisbursed.Location = new Point(856, 16);
            this.cardDisbursed.Size = new Size(200, 68);
            this.cardDisbursed.BorderStyle = BorderStyle.FixedSingle;
            this.lblDisbursedTitle.Location = new Point(10, 8);
            this.lblDisbursedTitle.AutoSize = true;
            this.lblDisbursedTitle.Text = "Disbursed";
            this.lblDisbursed.Location = new Point(10, 34);
            this.lblDisbursed.AutoSize = true;
            this.lblDisbursed.Text = "0";
            this.cardDisbursed.Controls.Add(this.lblDisbursedTitle);
            this.cardDisbursed.Controls.Add(this.lblDisbursed);

            this.statsPanel.Controls.Add(this.cardTotal);
            this.statsPanel.Controls.Add(this.cardPending);
            this.statsPanel.Controls.Add(this.cardReview);
            this.statsPanel.Controls.Add(this.cardApproved);
            this.statsPanel.Controls.Add(this.cardDisbursed);

            // Add controls to form (order matters for docking)
            this.Controls.Add(this.gridApplications);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.filtersPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }
    }
}