using LendingApp.UI.AdminUI.Views;
using LendingSystem.Admin;
using LendingSystem.Reports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI
{
    public partial class AdminDashboard : Form
    {
        private string _username = "Admin";
        private Action _onLogout;

        private string activeNav = "Dashboard";

        private readonly List<string> navItems = new List<string>
        {
            "Dashboard",
            "User Management",
            "Loan Products",
            "Override Actions",
            "System Reports",
            "System Config",
            "Audit Log"
        };

        // Shell
        private Panel headerPanel;
        private Panel sidebarPanel;
        private Panel contentPanel;

        // Header controls
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnUserMenu;
        private Button btnLogout;

        // User dropdown (simple WinForms equivalent)
        private ContextMenuStrip userMenu;

        public AdminDashboard()
        {
            InitializeComponent();
            BuildUI();
            ShowActiveView();
        }

        public void SetUsername(string username)
        {
            _username = string.IsNullOrWhiteSpace(username) ? "Admin" : username;
            if (btnUserMenu != null) btnUserMenu.Text = _username + " ▼";
        }

        public void OnLogout(Action logout)
        {
            _onLogout = logout;
        }

        private void BuildUI()
        {
            Text = "Administrator Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = ColorTranslator.FromHtml("#F9FAFB");

            BuildHeader();
            BuildSidebar();
            BuildContent();

            Controls.Clear();
            Controls.Add(contentPanel);
            Controls.Add(sidebarPanel);
            Controls.Add(headerPanel);
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Icon box (mimic blue Settings icon background)
            var iconBox = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                Size = new Size(40, 40),
                Location = new Point(16, 12)
            };
            var iconLabel = new Label
            {
                Text = "⚙",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };
            iconBox.Controls.Add(iconLabel);

            lblTitle = new Label
            {
                Text = "ADMINISTRATOR DASHBOARD",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(64, 16)
            };

            lblSubtitle = new Label
            {
                Text = "System Administration",
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(64, 36)
            };

            btnUserMenu = new Button
            {
                Text = _username + " ▼",
                Height = 32,
                Width = 160,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUserMenu.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnUserMenu.Click += BtnUserMenu_Click;

            btnLogout = new Button
            {
                Text = "Logout",
                Height = 32,
                Width = 96,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
            btnLogout.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnLogout.Click += (s, e) => DoLogout();

            userMenu = new ContextMenuStrip();
            userMenu.Items.Add("My Profile", null, (s, e) => ShowToast("Opening profile..."));
            userMenu.Items.Add("Settings", null, (s, e) => ShowToast("Opening settings..."));
            userMenu.Items.Add(new ToolStripSeparator());
            userMenu.Items.Add("Logout", null, (s, e) => DoLogout());

            headerPanel.Controls.Add(iconBox);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(btnUserMenu);
            headerPanel.Controls.Add(btnLogout);

            headerPanel.Resize += (s, e) =>
            {
                btnLogout.Left = headerPanel.Width - btnLogout.Width - 16;
                btnLogout.Top = 16;

                btnUserMenu.Left = btnLogout.Left - btnUserMenu.Width - 10;
                btnUserMenu.Top = 16;
            };
        }

        private void BtnUserMenu_Click(object sender, EventArgs e)
        {
            if (userMenu == null) return;
            var btn = sender as Button;
            if (btn == null) return;

            userMenu.Show(btn, new Point(0, btn.Height));
        }

        private void BuildSidebar()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var navHost = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(12)
            };

            int y = 10;
            foreach (var item in navItems)
            {
                var btn = new Button
                {
                    Text = item,
                    Location = new Point(10, y),
                    Size = new Size(sidebarPanel.Width - 44, 42),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = activeNav == item ? ColorTranslator.FromHtml("#EFF6FF") : Color.White,
                    ForeColor = activeNav == item ? ColorTranslator.FromHtml("#1D4ED8") : ColorTranslator.FromHtml("#374151")
                };
                btn.FlatAppearance.BorderSize = activeNav == item ? 1 : 0;
                btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#BFDBFE");

                btn.Click += (s, e) =>
                {
                    activeNav = item;
                    BuildSidebar(); // refresh highlight
                    ShowActiveView();
                };

                navHost.Controls.Add(btn);
                y += 48;
            }

            // Bottom logout (like React sidebar logout)
            var btnLogoutSide = new Button
            {
                Text = "Logout",
                Dock = DockStyle.Bottom,
                Height = 44,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#B91C1C")
            };
            btnLogoutSide.FlatAppearance.BorderSize = 0;
            btnLogoutSide.Click += (s, e) => DoLogout();

            sidebarPanel.Controls.Clear();
            sidebarPanel.Controls.Add(btnLogoutSide);
            sidebarPanel.Controls.Add(navHost);
        }

        private void BuildContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                AutoScroll = true,
                Padding = new Padding(16)
            };
        }

        private void ShowActiveView()
        {
            contentPanel.SuspendLayout();
            contentPanel.Controls.Clear();

            if (activeNav == "Dashboard")
            {
                var adminOverview = new AdminOverviewControl();
                adminOverview.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(adminOverview);
            }
            else if (activeNav == "User Management")
            {
                var userManagement = new AdminUserManagementControl();
                userManagement.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(userManagement);
            }
            else if (activeNav == "Loan Products")
            {
                var loanProductsControl = new LoanProductsControl();
                loanProductsControl.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(loanProductsControl);
            }
            else if (activeNav == "Override Actions")
            {
                var overrideActions = new OverrideActionsControl();
                overrideActions.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(overrideActions);
            }
            else if (activeNav == "System Reports")
            {
                var systemReports = new SystemReportsControl();
                systemReports.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(systemReports);
            }
            else if (activeNav == "System Config")
            {
                var systemConfig = new SystemConfigControl();
                systemConfig.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(systemConfig);
            }
            else if (activeNav == "Audit Log")
            {
                var auditLog = new AuditLogControl();
                auditLog.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(auditLog);
            }
            else
            {
                contentPanel.Controls.Add(MakeCard(
                    title: activeNav,
                    message: "View coming soon.",
                    accentHex: "#374151",
                    iconText: "ℹ"));
            }

            // Only layout placeholder cards (Panels). UserControls manage their own layout.
            var first = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] : null;
            if (first is Panel card)
            {
                ApplyMainCardLayout(card);
                contentPanel.Resize -= ContentPanel_Resize;
                contentPanel.Resize += ContentPanel_Resize;
            }
            else
            {
                contentPanel.Resize -= ContentPanel_Resize;
            }

            contentPanel.ResumeLayout();
        }

        private void ContentPanel_Resize(object sender, EventArgs e)
        {
            var card = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] as Panel : null;
            if (card != null) ApplyMainCardLayout(card);
        }

        private void ApplyMainCardLayout(Panel card)
        {
            int pad = 16;
            int maxWidth = 1100;

            int availableWidth = Math.Max(200, contentPanel.ClientSize.Width - (pad * 2));
            card.Width = Math.Min(maxWidth, availableWidth);
            card.Left = pad + (availableWidth - card.Width) / 2;
            card.Top = pad;
        }

        private Panel MakeCard(string title, string message, string accentHex, string iconText)
        {
            var card = new Panel
            {
                Height = 280,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var accent = new Panel
            {
                BackColor = ColorTranslator.FromHtml(accentHex),
                Width = 6,
                Height = card.Height,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            var icon = new Label
            {
                Text = iconText,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 80,
                Height = 80,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#D1D5DB"),
                Location = new Point((card.Width / 2) - 40, 60),
                Anchor = AnchorStyles.Top
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(20, 20)
            };

            var lblMessage = new Label
            {
                Text = message,
                AutoSize = false,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(20, 54),
                Width = 900,
                Height = 60
            };

            card.Controls.Add(accent);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblMessage);
            card.Controls.Add(icon);

            card.Resize += (s, e) =>
            {
                accent.Height = card.Height;
                lblMessage.Width = card.Width - 40;
                icon.Left = (card.Width / 2) - (icon.Width / 2);
            };

            return card;
        }

        private void DoLogout()
        {
            if (_onLogout != null)
            {
                _onLogout.Invoke();
                Close();
                return;
            }

            // fallback behavior (consistent with other dashboards)
            Close();
        }

        private void ShowToast(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
