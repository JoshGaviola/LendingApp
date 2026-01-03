using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI.Dialog
{
    public partial class ReviewApplicationDialog : Form
    {
        private readonly string _appId;
        private readonly string _customer;
        private readonly string _type;
        private readonly string _amount;
        private readonly string _status;

        public ReviewApplicationDialog(string appId, string customer, string type, string amount, string status)
        {
            _appId = appId;
            _customer = customer;
            _type = type;
            _amount = amount;
            _status = status;

            InitializeComponent(); // Creates basic form structure
            SetupUI(); // Builds the programmatic UI
        }

        private void SetupUI()
        {
            // Clear any existing controls
            this.Controls.Clear();

            // Main container with scroll
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            int currentY = 20;

            // Title
            Label title = new Label
            {
                Text = $"REVIEW APPLICATION - {_appId ?? "N/A"} (Pending)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, currentY),
                Size = new Size(600, 30)
            };
            mainPanel.Controls.Add(title);
            currentY += 40;

            // Application Details and Customer Info side by side
            int leftY = currentY;
            AddSectionLeft(mainPanel, "Application Details", ref leftY);
            AddApplicationDetails(mainPanel, ref leftY);

            int rightY = currentY;
            AddSectionRight(mainPanel, "Customer Quick Info", ref rightY);
            AddCustomerInfo(mainPanel, ref rightY);

            currentY = Math.Max(leftY, rightY) + 20;

            // Required Documents
            AddSection(mainPanel, "Required Documents Checklist", ref currentY);
            AddDocumentsChecklist(mainPanel, ref currentY);
            currentY += 20;

            // Preliminary Assessment
            AddSection(mainPanel, "Preliminary Assessment", ref currentY);
            AddAssessment(mainPanel, ref currentY);
            currentY += 20;

            // Action Buttons
            AddActionButtons(mainPanel, ref currentY);
            currentY += 50;

            // Close button
            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(520, currentY),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9)
            };
            btnClose.Click += (s, e) => this.Close();
            mainPanel.Controls.Add(btnClose);

            this.Controls.Add(mainPanel);
        }

        private void AddSection(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionLeft(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionRight(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(310, y),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(220, 252, 231),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddApplicationDetails(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(290, 130),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] details = {
                $"Customer: {_customer}",
                $"Loan Type: {_type}",
                $"Amount: {_amount}.00",
                $"Term: 24 months",
                $"Purpose: Business capital",
                $"Desired Release: Dec 20, 2024"
            };

            for (int i = 0; i < details.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = details[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(270, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            parent.Controls.Add(panel);
            y += 140;
        }

        private void AddCustomerInfo(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(310, y),
                Size = new Size(290, 130),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] info = {
                $"Credit Score: 75/100",
                $"Customer Type: Regular",
                $"Active Loans: 2",
                $"Total Balance: ₱85,000.00",
                $"Payment History: 90% on-time"
            };

            for (int i = 0; i < info.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = info[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(270, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            parent.Controls.Add(panel);
            y += 140;
        }

        private void AddDocumentsChecklist(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 180),
                BorderStyle = BorderStyle.FixedSingle
            };

            int rowY = 10;

            // Document 1
            AddDocumentRow(panel, "Valid ID 1 (Passport)", true, ref rowY);
            // Document 2
            AddDocumentRow(panel, "Valid ID 2 (Driver's License)", true, ref rowY);
            // Document 3
            AddDocumentRow(panel, "Proof of Income", true, ref rowY);
            // Document 4
            AddDocumentRow(panel, "Bank Statements (3 months)", false, ref rowY);
            // Document 5
            AddDocumentRow(panel, "COE/Employment Certificate", false, ref rowY);

            parent.Controls.Add(panel);
            y += 190;
        }

        private void AddDocumentRow(Panel panel, string docName, bool isUploaded, ref int y)
        {
            CheckBox checkBox = new CheckBox
            {
                Text = docName,
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9),
                Checked = isUploaded,
                Enabled = false
            };

            Button btnAction = new Button
            {
                Text = isUploaded ? "View" : "Upload",
                Location = new Point(450, y),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            btnAction.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnAction.Click += (s, e) => MessageBox.Show($"{btnAction.Text} {docName}", "Document");

            panel.Controls.Add(checkBox);
            panel.Controls.Add(btnAction);
            y += 30;
        }

        private void AddAssessment(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 150),
                BackColor = Color.FromArgb(255, 247, 237),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Debt-to-Income Ratio
            Label debtLabel = new Label
            {
                Text = "Debt-to-Income Ratio: 45% (Should be <50%)",
                Location = new Point(10, 15),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };

            Panel debtBar = new Panel
            {
                Location = new Point(10, 35),
                Size = new Size(400, 10),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            Panel debtFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(180, 10), // 45% of 400
                BackColor = Color.FromArgb(34, 197, 94)
            };
            debtBar.Controls.Add(debtFill);

            // Loan-to-Income Ratio
            Label loanLabel = new Label
            {
                Text = "Loan-to-Income Ratio: 60%",
                Location = new Point(10, 55),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };

            Panel loanBar = new Panel
            {
                Location = new Point(10, 75),
                Size = new Size(400, 10),
                BackColor = Color.FromArgb(200, 200, 200)
            };

            Panel loanFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(240, 10), // 60% of 400
                BackColor = Color.FromArgb(234, 179, 8)
            };
            loanBar.Controls.Add(loanFill);

            // Financial details
            string[] financial = {
                "Existing Monthly Payments: ₱4,500.00",
                "Proposed New Payment: ₱2,350.00 (Total: ₱6,850.00)",
                "Monthly Income: ₱40,000.00"
            };

            for (int i = 0; i < financial.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = financial[i],
                    Location = new Point(10, 95 + (i * 18)),
                    Size = new Size(400, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            panel.Controls.Add(debtLabel);
            panel.Controls.Add(debtBar);
            panel.Controls.Add(loanLabel);
            panel.Controls.Add(loanBar);
            parent.Controls.Add(panel);
            y += 160;
        }

        private void AddActionButtons(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(600, 40)
            };

            Button btnPending = new Button
            {
                Text = "Set Pending",
                Location = new Point(0, 5),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            btnPending.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnPending.Click += (s, e) =>
            {
                MessageBox.Show("Application set to pending status", "Success");
                this.Close();
            };

            Button btnEvaluate = new Button
            {
                Text = "Send for Evaluation",
                Location = new Point(130, 5),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEvaluate.Click += (s, e) =>
            {
                MessageBox.Show("Application sent for evaluation", "Success");
                this.Close();
            };

            Button btnReject = new Button
            {
                Text = "Reject",
                Location = new Point(280, 5),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat
            };
            btnReject.FlatAppearance.BorderColor = Color.Red;
            btnReject.Click += (s, e) =>
            {
                MessageBox.Show("Application rejected", "Warning");
                this.Close();
            };

            panel.Controls.Add(btnPending);
            panel.Controls.Add(btnEvaluate);
            panel.Controls.Add(btnReject);
            parent.Controls.Add(panel);
            y += 50;
        }
    }
}