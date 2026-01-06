using LendingApp.Data;
using LendingApp.LogicClass.Cashier;
using LendingApp.Class.Models.CashierModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierDashboard : Form
    {
        private string _username = "Cashier";
        private Action _onLogout;

        private string activeNav = "Process Payment";
        private readonly List<string> navItems = new List<string>
        {
            "Process Payment", "Release Loan", "Daily Report", "Receipts", "Reports", "Settings"
        };

        // Daily summary stats (sample values like the React version)
        private int loansCount = 3;
        private string cashOnHand = "₱195,230";
        private string sessionStart = "8:30 AM";

        // Shell panels
        private Panel headerPanel;
        private Panel summaryPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Header controls
        private Label lblTitle;
        private Label lblCashier;
        private Label lblCashOnHandTitle;
        private Label lblCashOnHandValue;
        private Button btnLogout;

        // Summary cards
        private Panel cardPayments;
        private Panel cardLoans;
        private Panel cardCash;
        private Panel cardSession;

        private bool _summaryResizeHooked;
        private bool _contentResizeHooked;

        // Embedded views
        private CashierLoanRelease _loanReleaseForm;
        private CashierDailyReport _dailyReportForm;
        private CashierReciept _receiptsForm;
        private CashierReport _cashierReportForm;
        private CashierSettings _settingsForm;
        private CashierDashboardLogic _dashboardLogic;
        private CashierProcessPayment _cashierProcessPayment;

        private BindingList<TransactionModels> _transactions;
        private BindingList<LoanReleaseModels> _pendingLoans;

        public CashierDashboard(ApplicantsData data)
        {
            InitializeComponent();

            if (data == null)
            {
                _transactions = new BindingList<TransactionModels>();
                _pendingLoans = new BindingList<LoanReleaseModels>();
            }
            else
            {
                _transactions = data.recentTransactions;
                _pendingLoans = data.releaseLoan;
            }

            _dashboardLogic = new CashierDashboardLogic(data);

            BuildUI();
            PopulateData();
        }

        private void ShowProcessPaymentView()
        {
            if (_cashierProcessPayment == null || _cashierProcessPayment.IsDisposed)
            {
                _cashierProcessPayment = new CashierProcessPayment(_transactions)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill,
                };
            }
            contentPanel.Controls.Add(_cashierProcessPayment);
            _cashierProcessPayment.Show();
        }

        private void ShowLoanReleaseView()
        {
            if (_loanReleaseForm == null || _loanReleaseForm.IsDisposed)
            {
                _loanReleaseForm = new CashierLoanRelease(_pendingLoans)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_loanReleaseForm);
            _loanReleaseForm.Show();
        }

        public void SetUsername(string username)
        {
            _username = string.IsNullOrWhiteSpace(username) ? "Cashier" : username;
            if (lblCashier != null) lblCashier.Text = $"Cashier: {_username}";
        }

        public void OnLogout(Action logout)
        {
            _onLogout = logout;
        }

        private void BuildUI()
        {
            // Form settings
            Text = "Cashier Dashboard";
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            // Header
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTitle = new Label
            {
                Text = "CASHIER DASHBOARD",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 10)
            };

            lblCashier = new Label
            {
                Text = $"Cashier: {_username}",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(16, 34)
            };

            lblCashOnHandTitle = new Label
            {
                Text = "Cash on Hand",
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#6B7280")
            };

            lblCashOnHandValue = new Label
            {
                Text = cashOnHand,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#16A34A")
            };

            btnLogout = new Button
            {
                Text = "Logout",
                Width = 90,
                Height = 28,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnLogout.Click += (s, e) =>
            {
                _onLogout?.Invoke();
                Close();
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblCashier);
            headerPanel.Controls.Add(lblCashOnHandTitle);
            headerPanel.Controls.Add(lblCashOnHandValue);
            headerPanel.Controls.Add(btnLogout);

            headerPanel.Resize += (s, e) =>
            {
                // Right-align cash labels and logout
                btnLogout.Left = headerPanel.Width - btnLogout.Width - 16;
                btnLogout.Top = 16;

                lblCashOnHandValue.Left = btnLogout.Left - lblCashOnHandValue.Width - 24;
                lblCashOnHandValue.Top = 16;

                lblCashOnHandTitle.Left = lblCashOnHandValue.Left;
                lblCashOnHandTitle.Top = 34;
            };

            // Summary bar (daily cards)
            summaryPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 110,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10)
                };
                cardLoans = MakeSummaryCard(
                    backHex: "#FEF3C7",
                    titleHex: "#B45309",
                    valueHex: "#78350F",
                    title: "Loans Issued",
                    value: "₱" + _dashboardLogic?.CalculateTotalLoansPending().ToString("N2"),
                    sub: _dashboardLogic?.TotalLoansPending.ToString() + " Issued today"
                 );

                cardPayments = MakeSummaryCard(
                    backHex: "#ECFDF5",
                    titleHex: "#15803D",
                    valueHex: "#052E16",
                    title: "Payments Today",
                    value: "₱" + _dashboardLogic?.CalculateTotalRecentTransaction().ToString("N2"),
                    sub: _dashboardLogic?.TotalTransaction.ToString() + " transactions"
                );
           
                cardCash = MakeSummaryCard(
                    backHex: "#EDE9FE",
                    titleHex: "#6D28D9",
                    valueHex: "#3B0764",
                    title: "Cash on Hand",
                    value: cashOnHand,
                    sub: "Current balance"
                );

                cardSession = MakeSummaryCard(
                    backHex: "#FFEDD5",
                    titleHex: "#C2410C",
                    valueHex: "#7C2D12",
                    title: "Session",
                    value: sessionStart,
                    sub: "Started"
                );

            summaryPanel.Controls.Add(cardPayments);
            summaryPanel.Controls.Add(cardLoans);
            summaryPanel.Controls.Add(cardCash);
            summaryPanel.Controls.Add(cardSession);

            // Sidebar
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            BuildSidebar();

            // Content
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = false // hosted forms manage their own scrolling
            };

            Controls.Clear();
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(summaryPanel);
            Controls.Add(headerPanel);

            LayoutSummaryCards();
            ShowActiveView();

            if (!_summaryResizeHooked)
            {
                summaryPanel.Resize += (s, e) => LayoutSummaryCards();
                _summaryResizeHooked = true;
            }
        }

        private void BuildSidebar()
        {
            sidebarPanel.Controls.Clear();

            var navHost = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            int y = 10;
            foreach (var item in navItems)
            {
                var btn = new Button
                {
                    Text = item,
                    Location = new Point(10, y),
                    Size = new Size(sidebarPanel.Width - 40, 40),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = activeNav == item ? ColorTranslator.FromHtml("#ECFDF5") : Color.White,
                    ForeColor = activeNav == item ? ColorTranslator.FromHtml("#15803D") : ColorTranslator.FromHtml("#374151")
                };
                btn.FlatAppearance.BorderSize = activeNav == item ? 1 : 0;
                btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#BBF7D0");

                btn.Click += (s, e) =>
                {
                    activeNav = item;
                    BuildSidebar(); // refresh highlight
                    ShowActiveView();
                };

                navHost.Controls.Add(btn);
                y += 46;
            }

            // Logout button at bottom
            var btnLogoutSide = new Button
            {
                Text = "Logout",
                Dock = DockStyle.Bottom,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#B91C1C")
            };
            btnLogoutSide.FlatAppearance.BorderSize = 0;
            btnLogoutSide.Click += (s, e) =>
            {
                _onLogout?.Invoke();
                Close();
            };

            sidebarPanel.Controls.Add(btnLogoutSide);
            sidebarPanel.Controls.Add(navHost);
        }

        private void ShowActiveView()
        {
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            if (activeNav == "Process Payment")
            {
                ShowProcessPaymentView();
            }
            else if (activeNav == "Release Loan")
            {
                ShowLoanReleaseView();
            }
            else if (activeNav == "Daily Report")
            {
                ShowDailyReportView();
            }
            else if (activeNav == "Receipts")
            {
                ShowReceiptsView();
            }
            else if (activeNav == "Reports")
            {
                ShowReportsView();
            }
            else if (activeNav == "Settings")
            {
                ShowSettingsView();
            }
            else
            {
                contentPanel.Controls.Add(MakePlaceholderCard(
                    title: activeNav,
                    message: "View coming soon.",
                    accentHex: "#374151"
                ));
                ApplyPlaceholderLayout();
            }

            contentPanel.ResumeLayout();
        }
        private void ShowDailyReportView()
        {
            if (_dailyReportForm == null || _dailyReportForm.IsDisposed)
            {
                _dailyReportForm = new CashierDailyReport
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_dailyReportForm);
            _dailyReportForm.Show();
        }

        private void ShowReceiptsView()
        {
            if (_receiptsForm == null || _receiptsForm.IsDisposed)
            {
                _receiptsForm = new CashierReciept
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_receiptsForm);
            _receiptsForm.Show();
        }

        private void ShowReportsView()
        {
            if (_cashierReportForm == null || _cashierReportForm.IsDisposed)
            {
                _cashierReportForm = new CashierReport
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_cashierReportForm);
            _cashierReportForm.Show();
        }

        private void ShowSettingsView()
        {
            if (_settingsForm == null || _settingsForm.IsDisposed)
            {
                _settingsForm = new CashierSettings
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            contentPanel.Controls.Add(_settingsForm);
            _settingsForm.Show();
        }

        private void ApplyPlaceholderLayout()
        {
            // Center a single placeholder card and keep responsive width
            var card = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] as Panel : null;
            if (card != null)
            {
                ApplyMainCardLayout(card);
                if (!_contentResizeHooked)
                {
                    contentPanel.Resize += (s, e) => ApplyMainCardLayout(card);
                    _contentResizeHooked = true;
                }
            }
        }

        private void ApplyMainCardLayout(Panel card)
        {
            int pad = 24;
            int maxWidth = 1100;

            int availableWidth = Math.Max(200, contentPanel.Width - (pad * 2));
            card.Width = Math.Min(maxWidth, availableWidth);
            card.Left = pad + (availableWidth - card.Width) / 2;
            card.Top = pad;
        }

        private Panel MakeSummaryCard(string backHex, string titleHex, string valueHex, string title, string value, string sub)
        {
            var card = new Panel
            {
                Height = 84,
                BackColor = ColorTranslator.FromHtml(backHex),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml(titleHex),
                Location = new Point(12, 10)
            };

            var lblValue = new Label
            {
                Text = value,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml(valueHex),
                Location = new Point(12, 30)
            };

            var lblSub = new Label
            {
                Text = sub,
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml(titleHex),
                Location = new Point(12, 56)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblSub);

            return card;
        }

        private void LayoutSummaryCards()
        {
            int gap = 10;
            int padX = summaryPanel.Padding.Left;
            int y = summaryPanel.Padding.Top;

            int available = summaryPanel.Width - summaryPanel.Padding.Left - summaryPanel.Padding.Right;
            if (available <= 0) return;

            int cardW = (available - (gap * 3)) / 4;
            cardW = Math.Max(180, cardW);
            cardPayments.SetBounds(padX, y, cardW, 84);
            cardLoans.SetBounds(cardPayments.Right + gap, y, cardW, 84);
            cardCash.SetBounds(cardLoans.Right + gap, y, cardW, 84);
            cardSession.SetBounds(cardCash.Right + gap, y, cardW, 84);
        }

        private Panel MakePlaceholderCard(string title, string message, string accentHex)
        {
            var card = new Panel
            {
                Height = 240,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(20, 24)
            };

            var lblMessage = new Label
            {
                Text = message,
                AutoSize = false,
                Width = 900,
                Height = 60,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(20, 58)
            };

            var accent = new Panel
            {
                BackColor = ColorTranslator.FromHtml(accentHex),
                Width = 6,
                Height = card.Height,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            card.Controls.Add(accent);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblMessage);

            card.Resize += (s, e) =>
            {
                accent.Height = card.Height;
                lblMessage.Width = card.Width - 40;
            };

            return card;
        }

        private void PopulateData()
        {
            SetUsername(_username);
        }
    }
}
