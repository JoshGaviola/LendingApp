using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem
{
    public partial class OfficerApplicationReviewControl : UserControl
    {
        private Panel mainContainer;
        private Panel mainCard;
        public Button BackButton { get; private set; }
        private int totalContentHeight = 0;

        public OfficerApplicationReviewControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Main container for this tab
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            int yPos = 0;

            // Back button
            BackButton = new Button
            {
                Text = "← Back to Applications",
                Location = new Point(0, yPos),
                Size = new Size(180, 32),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            BackButton.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            BackButton.FlatAppearance.BorderSize = 1;
            mainContainer.Controls.Add(BackButton);
            yPos += 42;

            // Calculate initial content height
            totalContentHeight = CalculateTotalContentHeight(mainContainer.Width - 30);

            // Main card container - dynamic height
            mainCard = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, totalContentHeight + 100), // Header + padding
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Card Header
            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(mainCard.Width, 50),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLabel = new Label
            {
                Text = "LOAN APPLICATION EVALUATION",
                Location = new Point(15, 15),
                Size = new Size(mainCard.Width - 30, 20),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black
            };

            header.Controls.Add(titleLabel);
            mainCard.Controls.Add(header);

            // Content area
            Panel content = new Panel
            {
                Location = new Point(15, 60),
                Size = new Size(mainCard.Width - 30, totalContentHeight),
                BackColor = Color.White
            };

            BuildContent(content);

            mainCard.Controls.Add(content);
            mainContainer.Controls.Add(mainCard);

            // Handle resize - FIXED: Use proper event handler
            this.SizeChanged += OfficerApplicationReviewControl_SizeChanged;
            mainContainer.Resize += MainContainer_Resize;

            this.Controls.Add(mainContainer);
            this.ResumeLayout(false);
        }

        private void MainContainer_Resize(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        private void OfficerApplicationReviewControl_SizeChanged(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (mainContainer == null || mainCard == null) return;

            // Update main card width
            mainCard.Width = mainContainer.Width;

            // Update header width
            if (mainCard.Controls.Count > 0 && mainCard.Controls[0] is Panel headerPanel)
            {
                headerPanel.Width = mainCard.Width;
                if (headerPanel.Controls.Count > 0 && headerPanel.Controls[0] is Label headerLabel)
                {
                    headerLabel.Width = mainCard.Width - 30;
                }
            }

            // Update content width and rebuild if needed
            if (mainCard.Controls.Count > 1 && mainCard.Controls[1] is Panel contentPanel)
            {
                int newContentWidth = mainCard.Width - 30;
                if (contentPanel.Width != newContentWidth)
                {
                    contentPanel.Width = newContentWidth;

                    // Recalculate total height with new width
                    totalContentHeight = CalculateTotalContentHeight(newContentWidth);

                    // Update main card height
                    mainCard.Height = totalContentHeight + 100;
                    contentPanel.Height = totalContentHeight;

                    // Rebuild content
                    BuildContent(contentPanel);
                }
            }
        }

        private int CalculateTotalContentHeight(int contentWidth)
        {
            // Simulate the layout to calculate total height
            int height = 0;

            // Application Summary
            height += 30; // Section title
            height += 100; // Summary details

            // Credit Assessment
            height += 30;
            height += 140;

            // Loan Computation
            height += 30;
            height += 130;

            // Supporting Documents
            height += 30;
            height += 110;

            // Action Buttons (calculate based on width)
            int actionButtonsHeight = CalculateActionButtonsHeight(contentWidth);
            height += actionButtonsHeight;

            // Approval Limit
            height += 20; // Spacing
            height += 60; // Notice

            return height;
        }

        private int CalculateActionButtonsHeight(int panelWidth)
        {
            // Simulate button layout to calculate height
            int buttonY = 0;
            int currentX = 0;
            int buttonHeight = 32;
            int buttonSpacing = 5;
            int rowHeight = buttonHeight + 5;
            int totalHeight = 50; // Base height

            string[] buttons = {
                "APPROVE LOAN",
                "APPROVE WITH CONDITIONS",
                "REJECT",
                "REQUEST MORE INFO",
                "SAVE AS DRAFT",
                "CANCEL"
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                int buttonWidth = 140;
                if (i == 1 || i == 2) buttonWidth = 180;

                if (currentX + buttonWidth > panelWidth && i > 0)
                {
                    buttonY += rowHeight;
                    currentX = 0;
                    totalHeight += rowHeight;
                }

                currentX += buttonWidth + buttonSpacing;
            }

            return totalHeight + 10; // Add extra padding
        }

        private void BuildContent(Panel contentPanel)
        {
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            int contentY = 0;

            // Application Summary
            contentY = AddSection(contentPanel, "APPLICATION SUMMARY:", contentY);
            contentY = AddSummaryDetails(contentPanel, contentY);

            // Credit Assessment
            contentY = AddSection(contentPanel, "CREDIT ASSESSMENT:", contentY);
            contentY = AddCreditDetails(contentPanel, contentY);

            // Loan Computation
            contentY = AddSection(contentPanel, "LOAN COMPUTATION:", contentY);
            contentY = AddComputationDetails(contentPanel, contentY);

            // Supporting Documents
            contentY = AddSection(contentPanel, "SUPPORTING DOCUMENTS:", contentY);
            contentY = AddDocumentDetails(contentPanel, contentY);

            // Action Buttons
            contentY = AddActionButtons(contentPanel, contentY);

            // Approval Limit
            AddApprovalLimitNotice(contentPanel, contentY + 20);

            contentPanel.ResumeLayout();
        }

        private int AddSection(Panel parent, string title, int y)
        {
            Label sectionTitle = new Label
            {
                Text = title,
                Location = new Point(0, y),
                Size = new Size(parent.Width, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };
            parent.Controls.Add(sectionTitle);

            return y + 30;
        }

        private int AddSummaryDetails(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel details = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 90),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] summary = {
                "Application ID: APP-2024-00123 • Date: 2024-03-15",
                "Customer: Juan Dela Cruz (CUST-001)",
                "Loan Type: Personal Loan • Amount: ₱50,000",
                "Term: 12 months • Purpose: Home Renovation",
                "Desired Release: 2024-03-20"
            };

            for (int i = 0; i < summary.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = summary[i],
                    Location = new Point(10, 10 + (i * 16)),
                    Size = new Size(panelWidth - 20, 16),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                details.Controls.Add(lbl);
            }

            parent.Controls.Add(details);
            return y + 100;
        }

        private int AddCreditDetails(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Credit Score
            Label score = new Label
            {
                Text = "Credit Score: 720/1000 (Good)",
                Location = new Point(10, 10),
                Size = new Size(panelWidth - 20, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(score);

            // Breakdown
            Label breakdown = new Label
            {
                Text = "Calculation:",
                Location = new Point(10, 35),
                Size = new Size(panelWidth - 20, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(breakdown);

            string[] items = {
                "• Payment History (35%): 250/350",
                "• Credit Utilization (30%): 210/300",
                "• Credit History Length (15%): 120/150",
                "• Income Stability (20%): 140/200"
            };

            for (int i = 0; i < items.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = items[i],
                    Location = new Point(25, 60 + (i * 16)),
                    Size = new Size(panelWidth - 35, 16),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            parent.Controls.Add(panel);
            return y + 140;
        }

        private int AddComputationDetails(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] items = {
                "Principal Amount: ₱50,000",
                "Interest Rate: 12% per annum (1% monthly)",
                "Service Charge: 3% (₱1,500)",
                "Monthly Amortization: ₱4,442.44",
                "Total Payable: ₱53,309.28",
                "Total Interest: ₱3,309.28"
            };

            for (int i = 0; i < items.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = items[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(panelWidth - 20, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            // View Amortization button
            Button viewButton = new Button
            {
                Text = "View Amortization Schedule",
                Location = new Point(10, 105),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            viewButton.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            viewButton.FlatAppearance.BorderSize = 1;
            viewButton.Click += (s, e) => MessageBox.Show("Showing amortization schedule...");
            panel.Controls.Add(viewButton);

            parent.Controls.Add(panel);
            return y + 130;
        }

        private int AddDocumentDetails(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] docs = {
                "✓ Valid ID (scanned)",
                "✓ Proof of Income (3 months payslip)",
                "✓ Proof of Billing"
            };

            for (int i = 0; i < docs.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = docs[i],
                    Location = new Point(10, 10 + (i * 16)),
                    Size = new Size(panelWidth - 20, 16),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                };
                panel.Controls.Add(lbl);
            }

            Label coMaker = new Label
            {
                Text = "Co-maker: Maria Santos (CUST-002) • Score: 680",
                Location = new Point(10, 60),
                Size = new Size(panelWidth - 20, 16),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(coMaker);

            Label collateral = new Label
            {
                Text = "Collateral: None",
                Location = new Point(10, 78),
                Size = new Size(panelWidth - 20, 16),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black
            };
            panel.Controls.Add(collateral);

            parent.Controls.Add(panel);
            return y + 110;
        }

        private int AddActionButtons(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel buttonPanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 50)
            };

            string[] buttons = {
                "APPROVE LOAN",
                "APPROVE WITH CONDITIONS",
                "REJECT",
                "REQUEST MORE INFO",
                "SAVE AS DRAFT",
                "CANCEL"
            };

            Color[] colors = {
                Color.FromArgb(34, 197, 94),
                Color.Orange,
                Color.Red,
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(107, 114, 128),
                Color.FromArgb(107, 114, 128)
            };

            // Calculate button layout
            int buttonY = 0;
            int currentX = 0;
            int buttonHeight = 32;
            int buttonSpacing = 5;
            int rowHeight = buttonHeight + 5;

         
            for (int i = 0; i < buttons.Length; i++)
            {
                int buttonWidth = 140;
                if (i == 1 || i == 2) buttonWidth = 180;

                // Check if button fits in current row
                if (currentX + buttonWidth > panelWidth)
                {
                    // Move to next row
                    buttonY += rowHeight;
                    currentX = 0;
                    buttonPanel.Height += rowHeight;
                }

                Button btn = new Button
                {
                    Text = buttons[i],
                    Location = new Point(currentX, buttonY),
                    Size = new Size(buttonWidth, buttonHeight),
                    Font = new Font("Segoe UI", 9, i == 0 ? FontStyle.Bold : FontStyle.Regular),
                    BackColor = i < 3 ? colors[i] : Color.White,
                    ForeColor = i < 3 ? Color.White : Color.Black,
                    FlatStyle = FlatStyle.Flat
                };

                if (i >= 3)
                {
                    btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                    btn.FlatAppearance.BorderSize = 1;
                }
                else
                {
                    btn.FlatAppearance.BorderSize = 0;
                }
                int index = i; 
                btn.Click += (s, e) => ShowDialogForButton(buttons[index]);


                buttonPanel.Controls.Add(btn);
                currentX += buttonWidth + buttonSpacing;
            }

            parent.Controls.Add(buttonPanel);
            return y + buttonPanel.Height + 10;
        }

        private void AddApprovalLimitNotice(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel notice = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 60),
                BackColor = Color.FromArgb(240, 253, 244),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label title = new Label
            {
                Text = "✓ YOUR APPROVAL LIMIT: ₱50,000",
                Location = new Point(10, 10),
                Size = new Size(panelWidth - 20, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52)
            };
            notice.Controls.Add(title);

            Label message = new Label
            {
                Text = "This loan is WITHIN your authority limit.",
                Location = new Point(10, 30),
                Size = new Size(panelWidth - 20, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(22, 101, 52)
            };
            notice.Controls.Add(message);

            parent.Controls.Add(notice);
        }

        private void ShowDialogForButton(string buttonText)
        {
            string message = "";

            switch (buttonText)
            {
                case "APPROVE LOAN":
                    message = "Approve this loan application?";
                    break;
                case "APPROVE WITH CONDITIONS":
                    message = "Approve with conditions?";
                    break;
                case "REJECT":
                    message = "Reject this application?";
                    break;
                case "REQUEST MORE INFO":
                    message = "Request more information?";
                    break;
                case "SAVE AS DRAFT":
                    message = "Save as draft?";
                    break;
                case "CANCEL":
                    message = "Cancel review?";
                    break;
            }

            MessageBox.Show(message, buttonText, MessageBoxButtons.OKCancel);
        }
    }
}