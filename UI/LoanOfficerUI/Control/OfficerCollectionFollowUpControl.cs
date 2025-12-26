using System;
using System.Drawing;
using System.Windows.Forms;

namespace LendingSystem
{
    public partial class OfficerCollectionFollowUpControl : UserControl
    {
        private OverdueLoanData loanData;
        private Panel mainContainer;
        private Panel mainCard;
        public Button BackButton { get; private set; }

        // Dialogs (same as before)
        private Form smsDialog;
        private Form callDialog;
        private Form emailDialog;
        private Form visitDialog;
        private Form partialPaymentDialog;
        private Form restructureDialog;
        private Form settlementDialog;
        private Form escalateDialog;
        private Form notesDialog;

        public delegate void BackHandler();
        public event BackHandler OnBack;

        public OfficerCollectionFollowUpControl(OverdueLoanData loan)
        {
            loanData = loan;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = Color.FromArgb(249, 250, 251);
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20, 15, 20, 15);

            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            int yPos = 0;

            // Back button with icon style
            BackButton = new Button
            {
                Text = "← Back to Dashboard",
                Location = new Point(0, yPos),
                Size = new Size(160, 36),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            BackButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            BackButton.FlatAppearance.BorderSize = 1;
            BackButton.Click += (s, e) => OnBack?.Invoke();
            mainContainer.Controls.Add(BackButton);
            yPos += 46;

            // Main card with shadow effect simulation
            mainCard = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(mainContainer.Width, 1170), // Increased from 1150 to 1170
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(229, 231, 235)
            };

            // Content area with consistent padding
            Panel content = new Panel
            {
                Location = new Point(25, 80),
                Size = new Size(mainCard.Width - 50, 1080), // Increased from 1060 to 1080
                BackColor = Color.White
            };

            // Header with gradient-like effect
            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(mainCard.Width, 60),
                BackColor = Color.FromArgb(239, 68, 68),
                BorderStyle = BorderStyle.None
            };

            Label titleLabel = new Label
            {
                Text = "OVERDUE LOAN COLLECTION",
                Location = new Point(25, 18),
                Size = new Size(mainCard.Width - 50, 24),
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                ForeColor = Color.White
            };

            // Status badge
            Panel statusBadge = new Panel
            {
                Location = new Point(mainCard.Width - 140, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(220, 38, 38),
                BorderStyle = BorderStyle.None
            };

            Label statusLabel = new Label
            {
                Text = $"OVERDUE {loanData.DaysOverdue}d",
                Location = new Point(10, 5),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            statusBadge.Controls.Add(statusLabel);
            header.Controls.Add(titleLabel);
            header.Controls.Add(statusBadge);
            mainCard.Controls.Add(header);

            int contentY = 0;

            // Two-column layout for better information density
            contentY = AddLoanOverviewSection(content, contentY);
            contentY = AddPaymentAndContactSection(content, contentY);
            contentY = AddActionGridSection(content, contentY);
            contentY = AddBottomActionSection(content, contentY);
            AddImportantNotice(content, contentY + 15);

            mainCard.Controls.Add(content);
            mainContainer.Controls.Add(mainCard);

            // Handle resize
            mainContainer.Resize += (s, e) =>
            {
                mainCard.Width = mainContainer.Width;
                header.Width = mainCard.Width;
                titleLabel.Width = mainCard.Width - 50;
                statusBadge.Left = mainCard.Width - 140;
                content.Width = mainCard.Width - 50;
                UpdateContentLayout(content);
            };

            this.Controls.Add(mainContainer);
            this.ResumeLayout(false);

            InitializeDialogs();
        }

        private void UpdateContentLayout(Panel contentPanel)
        {
            int currentY = 0;
            contentPanel.Controls.Clear();

            currentY = AddLoanOverviewSection(contentPanel, currentY); // Now returns y + 230 (was 210)
            currentY = AddPaymentAndContactSection(contentPanel, currentY); // Returns y + 330
            currentY = AddActionGridSection(contentPanel, currentY); // Returns y + 330
            currentY = AddBottomActionSection(contentPanel, currentY); // Returns y + 90
            AddImportantNotice(contentPanel, currentY + 15);
        }

        private int AddLoanOverviewSection(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            // Loan overview in two columns - INCREASED HEIGHT
            Panel overviewPanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 220), // Increased from 200 to 220
                BackColor = Color.White
            };

            // Left column - Loan Details - INCREASED HEIGHT
            Panel leftPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(panelWidth / 2 - 10, 220), // Increased from 200 to 220
                BackColor = Color.FromArgb(254, 242, 242),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(254, 202, 202)
            };

            Label loanTitle = new Label
            {
                Text = "LOAN DETAILS",
                Location = new Point(15, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(153, 27, 27)
            };
            leftPanel.Controls.Add(loanTitle);

            string[] loanDetails = {
        $"ID: {loanData.Id}",
        $"Customer: {loanData.Customer} ({loanData.CustomerId})", // Added Customer ID
        $"Type: {loanData.LoanType}",
        $"Original: {loanData.OriginalAmount}",
        $"Term: {loanData.Term}",
        $"Due Date: {loanData.DueDate}"
    };

            for (int i = 0; i < loanDetails.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = loanDetails[i],
                    Location = new Point(15, 40 + (i * 26)), // Increased from 24 to 26 spacing
                    Size = new Size(300, 22), // Increased from 20 to 22
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = Color.FromArgb(75, 85, 99)
                };
                leftPanel.Controls.Add(lbl);
            }

            // Days overdue highlight - MOVED DOWN with more space
            Label daysOverdue = new Label
            {
                Text = $"⚠️ DAYS OVERDUE: {loanData.DaysOverdue}",
                Location = new Point(15, 195), // Moved from 150 to 185 (much lower)
                Size = new Size(300, 22),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };
            leftPanel.Controls.Add(daysOverdue);

            // Right column - Amounts Due - INCREASED HEIGHT
            Panel rightPanel = new Panel
            {
                Location = new Point(panelWidth / 2 + 10, 0),
                Size = new Size(panelWidth / 2 - 10, 220), // Increased from 200 to 220
                BackColor = Color.FromArgb(239, 246, 255),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(191, 219, 254)
            };

            Label amountTitle = new Label
            {
                Text = "AMOUNTS DUE",
                Location = new Point(15, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 78, 216)
            };
            rightPanel.Controls.Add(amountTitle);

            string[][] amounts = {
        new[] { "Amount Due:", loanData.AmountDue },
        new[] { "Penalty:", loanData.Penalty },
        new[] { "Total Due:", loanData.TotalDue },
        new[] { "Outstanding Balance:", loanData.OutstandingBalance }
    };

            for (int i = 0; i < amounts.Length; i++)
            {
                Label label = new Label
                {
                    Text = amounts[i][0],
                    Location = new Point(15, 40 + (i * 38)), // Increased from 34 to 38 spacing
                    Size = new Size(150, 24), // Increased from 20 to 24
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = Color.FromArgb(75, 85, 99)
                };

                Label value = new Label
                {
                    Text = amounts[i][1],
                    Location = new Point(165, 40 + (i * 38)),
                    Size = new Size(150, 24), // Increased from 20 to 24
                    Font = new Font("Segoe UI", 9.5F, i == 2 ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = i == 2 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(31, 41, 55)
                };

                rightPanel.Controls.Add(label);
                rightPanel.Controls.Add(value);
            }

            overviewPanel.Controls.Add(leftPanel);
            overviewPanel.Controls.Add(rightPanel);
            parent.Controls.Add(overviewPanel);

            return y + 230; // Increased from 210 to 230
        }

        private int AddPaymentAndContactSection(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            // Two-column layout for payment history and contact
            Panel sectionPanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 320), // Increased from 280 to 320
                BackColor = Color.White
            };

            // Payment History Column (unchanged, keep as is)
            Panel paymentPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(panelWidth / 2 - 10, 320), // Increased to match section height
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(229, 231, 235)
            };

            Label paymentTitle = new Label
            {
                Text = "PAYMENT HISTORY",
                Location = new Point(15, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            paymentPanel.Controls.Add(paymentTitle);

            string[][] payments = {
        new[] { "Nov 10, 2024", "₱3,850.00", "On time" },
        new[] { "Oct 10, 2024", "₱3,850.00", "On time" },
        new[] { "Sep 10, 2024", "₱3,850.00", "2 days late" },
        new[] { "Aug 10, 2024", "₱3,850.00", "On time" }
    };

            for (int i = 0; i < payments.Length; i++)
            {
                Panel paymentRow = new Panel
                {
                    Location = new Point(0, 45 + (i * 60)), // Increased from 48 to 60
                    Size = new Size(panelWidth / 2 - 40, 55), // Increased from 40 to 55
                    BackColor = i % 2 == 0 ? Color.FromArgb(249, 250, 251) : Color.White
                };

                Label date = new Label
                {
                    Text = payments[i][0],
                    Location = new Point(15, 15), // Increased from 10 to 15
                    Size = new Size(120, 22),
                    Font = new Font("Segoe UI", 9.5F), // Increased from 9 to 9.5
                    ForeColor = Color.FromArgb(75, 85, 99)
                };

                Label amount = new Label
                {
                    Text = payments[i][1],
                    Location = new Point(140, 15),
                    Size = new Size(100, 22),
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 41, 55)
                };

                Label status = new Label
                {
                    Text = payments[i][2],
                    Location = new Point(240, 15),
                    Size = new Size(90, 22), // Increased from 80 to 90
                    Font = new Font("Segoe UI", 9, FontStyle.Regular), // Changed from 8.5F to 9
                    ForeColor = payments[i][2] == "On time" ? Color.FromArgb(22, 163, 74) : Color.FromArgb(234, 88, 12),
                    TextAlign = ContentAlignment.MiddleRight
                };

                paymentRow.Controls.Add(date);
                paymentRow.Controls.Add(amount);
                paymentRow.Controls.Add(status);
                paymentPanel.Controls.Add(paymentRow);
            }

            // Next due - moved down
            Label nextDue = new Label
            {
                Text = "Next Due: Jan 10, 2025 • ₱3,850.00",
                Location = new Point(15, 285), // Moved from 235 to 285
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            paymentPanel.Controls.Add(nextDue);

            // Contact Column - FIXED to show all text
            Panel contactPanel = new Panel
            {
                Location = new Point(panelWidth / 2 + 10, 0),
                Size = new Size(panelWidth / 2 - 10, 320), // Increased to match section height
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(229, 231, 235)
            };

            Label contactTitle = new Label
            {
                Text = "CUSTOMER CONTACT",
                Location = new Point(15, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            contactPanel.Controls.Add(contactTitle);

            string[] contactInfo = {
        "Pedro Reyes",
        "+639456789012",
        "pedro.reyes@email.com",
        "123 Main St, Manila",
        "ABC Corp (3 years)"
    };

            string[] contactLabels = { "Name:", "Phone:", "Email:", "Address:", "Employer:" };

            for (int i = 0; i < contactInfo.Length; i++)
            {
                Label label = new Label
                {
                    Text = contactLabels[i],
                    Location = new Point(15, 45 + (i * 32)),
                    Size = new Size(80, 22),
                    Font = new Font("Segoe UI", 9.5F), // Increased from 9 to 9.5
                    ForeColor = Color.FromArgb(107, 114, 128)
                };

                Label value = new Label
                {
                    Text = contactInfo[i],
                    Location = new Point(100, 45 + (i * 32)),
                    Size = new Size(panelWidth / 2 - 120, 22), // Dynamic width
                    Font = new Font("Segoe UI", 9.5F), // Increased from 9 to 9.5
                    ForeColor = Color.FromArgb(31, 41, 55)
                };

                contactPanel.Controls.Add(label);
                contactPanel.Controls.Add(value);
            }

            // Contact Log - FIXED to show all text
            Label logTitle = new Label
            {
                Text = "CONTACT LOG",
                Location = new Point(15, 215), // Moved from 210 to 215
                Size = new Size(200, 22),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            contactPanel.Controls.Add(logTitle);

            string[] contactLog = {
        "• Dec 12: SMS sent - No reply",
        "• Dec 13: Call attempted - No answer",
        "• Dec 14: Email sent - Read receipt received" // Full text
    };

            for (int i = 0; i < contactLog.Length; i++)
            {
                Panel logRow = new Panel
                {
                    Location = new Point(10, 240 + (i * 24)), // More spacing
                    Size = new Size(panelWidth / 2 - 30, 22),
                    BackColor = Color.Transparent
                };

                Label log = new Label
                {
                    Text = contactLog[i],
                    Location = new Point(0, 0),
                    Size = new Size(panelWidth / 2 - 30, 22), // Full width
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Regular), // Increased from 8.5F to 9.5
                    ForeColor = Color.FromArgb(107, 114, 128)
                };

                logRow.Controls.Add(log);
                contactPanel.Controls.Add(logRow);
            }

            sectionPanel.Controls.Add(paymentPanel);
            sectionPanel.Controls.Add(contactPanel);
            parent.Controls.Add(sectionPanel);

            return y + 330; // Increased from 290 to 330
        }

        private int AddActionGridSection(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel actionPanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 320),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(229, 231, 235)
            };

            Label actionTitle = new Label
            {
                Text = "FOLLOW-UP ACTIONS",
                Location = new Point(15, 15),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55)
            };
            actionPanel.Controls.Add(actionTitle);

            // Action buttons in a 4x2 grid
            string[] buttonTexts = {
                "SEND SMS REMINDER",
                "RECORD CALL",
                "SEND EMAIL",
                "SCHEDULE VISIT",
                "RECORD PARTIAL PAYMENT",
                "RESTRUCTURE LOAN",
                "FULL SETTLEMENT",
                "ESCALATE TO COLLECTOR"
            };

            string[] buttonIcons = { "📱", "📞", "📧", "📅", "💰", "🔄", "✅", "⚠️" };
            Color[] buttonColors = {
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(16, 185, 129),
                Color.FromArgb(245, 158, 11),
                Color.FromArgb(34, 197, 94),
                Color.FromArgb(239, 68, 68)
            };

            int buttonWidth = (panelWidth - 50) / 4;
            int buttonHeight = 70;
            int buttonSpacing = 10;

            for (int i = 0; i < buttonTexts.Length; i++)
            {
                int col = i % 4;
                int row = i / 4;

                Panel buttonCard = new Panel
                {
                    Location = new Point(10 + (col * (buttonWidth + buttonSpacing)),
                                        50 + (row * (buttonHeight + buttonSpacing))),
                    Size = new Size(buttonWidth, buttonHeight),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    ForeColor = Color.FromArgb(229, 231, 235),
                    Cursor = Cursors.Hand
                };
                buttonCard.Click += (s, e) => HandleActionButtonClick(i);

                // Icon
                Label icon = new Label
                {
                    Text = buttonIcons[i],
                    Location = new Point(15, 10),
                    Size = new Size(30, 30),
                    Font = new Font("Segoe UI", 14),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Title
                Label title = new Label
                {
                    Text = buttonTexts[i],
                    Location = new Point(10, 40),
                    Size = new Size(buttonWidth - 20, 20),
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = buttonColors[i],
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Left border accent
                Panel accent = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(3, buttonHeight),
                    BackColor = buttonColors[i]
                };

                buttonCard.Controls.Add(accent);
                buttonCard.Controls.Add(icon);
                buttonCard.Controls.Add(title);
                actionPanel.Controls.Add(buttonCard);
            }

            parent.Controls.Add(actionPanel);
            return y + 330;
        }

        private int AddBottomActionSection(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel bottomPanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 80),
                BackColor = Color.White
            };

            // Notes button
            Button notesButton = new Button
            {
                Text = "📝 ADD FOLLOW-UP NOTES",
                Location = new Point(0, 15),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            notesButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            notesButton.FlatAppearance.BorderSize = 1;
            notesButton.Click += (s, e) => ShowDialog(notesDialog);

            // Save button
            Button saveButton = new Button
            {
                Text = "💾 SAVE PROGRESS",
                Location = new Point(210, 15),
                Size = new Size(160, 40),
                Font = new Font("Segoe UI", 9.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(107, 114, 128),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            saveButton.FlatAppearance.BorderSize = 1;
            saveButton.Click += (s, e) => MessageBox.Show("Progress saved successfully", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Resolve button
            Button resolveButton = new Button
            {
                Text = "✅ MARK AS RESOLVED",
                Location = new Point(380, 15),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resolveButton.FlatAppearance.BorderSize = 0;
            resolveButton.Click += (s, e) => {
                MessageBox.Show("Case marked as resolved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnBack?.Invoke();
            };

            bottomPanel.Controls.Add(notesButton);
            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Controls.Add(resolveButton);
            parent.Controls.Add(bottomPanel);

            return y + 90;
        }

        private void AddImportantNotice(Panel parent, int y)
        {
            int panelWidth = parent.Width;

            Panel noticePanel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(panelWidth, 70),
                BackColor = Color.FromArgb(255, 251, 235),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(253, 224, 71)
            };

            // Warning icon
            Label warningIcon = new Label
            {
                Text = "⚠️",
                Location = new Point(15, 20),
                Size = new Size(30, 30),
                Font = new Font("Segoe UI", 16),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Notice text
            Label noticeText = new Label
            {
                Text = "All actions are logged to audit trail. Penalties are calculated automatically per system configuration.",
                Location = new Point(55, 20),
                Size = new Size(panelWidth - 70, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(146, 64, 14)
            };

            noticePanel.Controls.Add(warningIcon);
            noticePanel.Controls.Add(noticeText);
            parent.Controls.Add(noticePanel);
        }

        private void HandleActionButtonClick(int index)
        {
            switch (index)
            {
                case 0: ShowDialog(smsDialog); break;
                case 1: ShowDialog(callDialog); break;
                case 2: ShowDialog(emailDialog); break;
                case 3: ShowDialog(visitDialog); break;
                case 4: ShowDialog(partialPaymentDialog); break;
                case 5: ShowDialog(restructureDialog); break;
                case 6: ShowDialog(settlementDialog); break;
                case 7: ShowDialog(escalateDialog); break;
            }
        }

        // Keep all the existing dialog initialization methods (same as before)
        private void InitializeDialogs() { /* Same as before */ }
        private void InitializeSMSDialog() { /* Same as before */ }
        private void InitializeCallDialog() { /* Same as before */ }
        private void InitializeEmailDialog() { /* Same as before */ }
        private void InitializeVisitDialog() { /* Same as before */ }
        private void InitializePartialPaymentDialog() { /* Same as before */ }
        private void InitializeRestructureDialog() { /* Same as before */ }
        private void InitializeSettlementDialog() { /* Same as before */ }
        private void InitializeEscalateDialog() { /* Same as before */ }
        private void InitializeNotesDialog() { /* Same as before */ }
        private void ShowDialog(Form dialog) { if (dialog != null) dialog.ShowDialog(); }
    }

    public class OverdueLoanData
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string CustomerId { get; set; }
        public string LoanType { get; set; }
        public string OriginalAmount { get; set; }
        public string Term { get; set; }
        public string DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public string AmountDue { get; set; }
        public string Penalty { get; set; }
        public string TotalDue { get; set; }
        public string OutstandingBalance { get; set; }
        public string Contact { get; set; }
    }
}