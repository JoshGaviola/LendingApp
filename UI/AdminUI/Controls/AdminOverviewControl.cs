using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.UI.AdminUI; // for AddNewLoanProductControl
using LendingApp.UI.AdminUI.Views; // for AddUserDialog

namespace LendingApp.UI.AdminUI.Views
{
    public partial class AdminOverviewControl : UserControl
    {
        // ===== UI Models (separation from EF entities) =====
        private sealed class UserOverviewRow
        {
            public string Username { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }
        }

        private sealed class LoanProductOverviewRow
        {
            public string Product { get; set; }
            public string Rate { get; set; }
            public string Status { get; set; }
        }

        // System Overview stats (static sample values)
        private readonly int _totalUsers = 15;
        private readonly int _activeLoans = 245;
        private readonly decimal _todaysCollections = 128592.44m;
        private readonly decimal _portfolioAtRisk = 125000m;
        private readonly decimal _portfolioAtRiskPercent = 5.1m;
        private readonly decimal _systemUptime = 99.7m;
        private readonly string _lastBackup = "Today 2:00 AM";

        // UI layout constants
        private const int PagePadding = 16;
        private const int Gap = 12;

        // Root layout
        private Panel root;
        private TableLayoutPanel layout;

        // System Configuration fields
        private TextBox txtPenaltyRate;
        private TextBox txtGracePeriod;
        private TextBox txtReceiptPrefix;
        private TextBox txtMaxLoginAttempts;
        private TextBox txtSessionTimeout;

        // ===== NEW: Overview grids + "View All" link references =====
        private DataGridView _usersGrid;
        private DataGridView _loanProductsGrid;
        private LinkLabel _viewAllUsersLink;

        // Toast
        private Panel _toastPanel;
        private Label _toastLabel;
        private Timer _toastTimer;

        public AdminOverviewControl()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;
            BackColor = ColorTranslator.FromHtml("#F9FAFB");

            BuildUI();
            BuildToast();

            // Load real data AFTER UI is built
            LoadOverviewData();
        }

        private void LoadOverviewData()
        {
            LoadUsersOverview();
            LoadLoanProductsOverview();
        }

        private void LoadUsersOverview()
        {
            if (_usersGrid == null) return;

            try
            {
                var rows = GetUsersOverviewRows(limit: 4);

                _usersGrid.Rows.Clear();
                foreach (var r in rows)
                    _usersGrid.Rows.Add(r.Username, r.Role, r.Status);

                if (_viewAllUsersLink != null)
                    _viewAllUsersLink.Text = "View All " + GetTotalUserCount().ToString(CultureInfo.InvariantCulture) + " Users →";
            }
            catch (Exception ex)
            {
                _usersGrid.Rows.Clear();
                _usersGrid.Rows.Add("Failed to load", "", "");
                Toast("Failed to load users overview: " + ex.Message, true);
            }
        }

        private void LoadLoanProductsOverview()
        {
            if (_loanProductsGrid == null) return;

            try
            {
                var rows = GetLoanProductsOverviewRows(limit: 3);

                _loanProductsGrid.Rows.Clear();
                foreach (var r in rows)
                    _loanProductsGrid.Rows.Add(r.Product, r.Rate, r.Status);
            }
            catch (Exception ex)
            {
                _loanProductsGrid.Rows.Clear();
                _loanProductsGrid.Rows.Add("Failed to load", "", "");
                Toast("Failed to load loan products overview: " + ex.Message, true);
            }
        }

        // ===== Data access (kept small and returns UI models only) =====
        private static List<UserOverviewRow> GetUsersOverviewRows(int limit)
        {
            using (var db = new AppDbContext())
            {
                // Show newest users first (CreatedDate desc), like an overview dashboard.
                var users = db.Users.AsNoTracking()
                    .OrderByDescending(u => u.CreatedDate)
                    .Take(limit)
                    .ToList();

                return users.Select(u => new UserOverviewRow
                {
                    Username = u.Username ?? "",
                    Role = u.Role ?? "",
                    Status = u.IsActive ? "Active" : "Inactive"
                }).ToList();
            }
        }

        private static int GetTotalUserCount()
        {
            using (var db = new AppDbContext())
            {
                return db.Users.AsNoTracking().Count();
            }
        }

        private static List<LoanProductOverviewRow> GetLoanProductsOverviewRows(int limit)
        {
            using (var db = new AppDbContext())
            {
                // Show newest products first (CreatedDate desc), matching a dashboard summary.
                var products = db.LoanProducts.AsNoTracking()
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(limit)
                    .ToList();

                return products.Select(p => new LoanProductOverviewRow
                {
                    Product = p.ProductName ?? "",
                    Rate = p.InterestRate.ToString("0.##", CultureInfo.InvariantCulture) + "%", // consistent with LoanProductsControl formatting
                    Status = p.IsActive ? "Active" : "Inactive"
                }).ToList();
            }
        }

        private void BuildUI()
        {
            Controls.Clear();

            root = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(PagePadding),
                BackColor = Color.Transparent
            };
            Controls.Add(root);

            layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.Controls.Add(layout);

            // ===== Main Card (System Administration) =====
            var mainCard = MakeCard();
            mainCard.Padding = new Padding(0);

            // Blue border like your design
            mainCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(ColorTranslator.FromHtml("#93C5FD"), 2))
                {
                    var rect = mainCard.ClientRectangle;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            var mainHeader = CardHeader("SYSTEM ADMINISTRATION", "#DBEAFE", "⚙", "#2563EB");
            var mainBody = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(16)
            };

            // Sections inside main body
            mainBody.Controls.Add(CreateActionButtonsRow());
            mainBody.Controls.Add(CreateSystemConfigSection());
            mainBody.Controls.Add(CreateLoanProductsSection());
            mainBody.Controls.Add(CreateUserManagementSection());
            mainBody.Controls.Add(CreateSystemOverviewSection());

            // Add in dock order: body then header
            mainCard.Controls.Add(mainBody);
            mainCard.Controls.Add(mainHeader);

            // ===== Override Actions Card =====
            var overrideCard = CreateOverrideActionsCard();

            // Add to page (top-down)
            AddRow(mainCard);
            AddRow(overrideCard);

            // spacer for scroll comfort
            AddRow(new Panel { Height = 6, Dock = DockStyle.Top });

            // Force layout
            layout.PerformLayout();
        }

        private void AddRow(Control c)
        {
            c.Margin = new Padding(0, 0, 0, Gap);
            layout.RowCount += 1;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(c, 0, layout.RowCount - 1);
        }

        // ===== Card Helpers =====
        private static Panel MakeCard()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private static Panel CardHeader(string title, string backHex, string iconText, string iconHex)
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorTranslator.FromHtml(backHex),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 0, 16, 0)
            };

            var icon = new Label
            {
                Text = iconText,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml(iconHex),
                Location = new Point(16, 14)
            };

            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(50, 18)
            };

            header.Controls.Add(icon);
            header.Controls.Add(lblTitle);
            return header;
        }

        private Panel SectionCard(string title, string icon, string accentHex)
        {
            var card = MakeCard();
            card.Padding = new Padding(12);

            var titleRow = new Label
            {
                Text = icon + "  " + title,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Dock = DockStyle.Top
            };

            var accent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                Width = 160,
                BackColor = ColorTranslator.FromHtml(accentHex),
                Margin = new Padding(0, 6, 0, 12)
            };

            // content host
            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White
            };

            card.Controls.Add(body);
            card.Controls.Add(accent);
            card.Controls.Add(titleRow);

            card.Tag = body;
            return card;
        }

        // ===== Sections =====
        private Panel CreateSystemOverviewSection()
        {
            var section = SectionCard("SYSTEM OVERVIEW", "📊", "#2563EB");
            var body = (Panel)section.Tag;

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));

            AddOverviewRow(grid, 0,
                OverviewItem("Total Users:", _totalUsers.ToString(CultureInfo.InvariantCulture)),
                OverviewItem("Active Loans:", _activeLoans.ToString(CultureInfo.InvariantCulture)),
                OverviewItem("Today's Collections:", "₱" + _todaysCollections.ToString("N2", CultureInfo.InvariantCulture)));

            AddOverviewRow(grid, 1,
                OverviewItem("Portfolio at Risk:", "₱" + _portfolioAtRisk.ToString("N0", CultureInfo.InvariantCulture) + " (" + _portfolioAtRiskPercent.ToString("N1", CultureInfo.InvariantCulture) + "%)"),
                OverviewItem("System Uptime:", _systemUptime.ToString("N1", CultureInfo.InvariantCulture) + "%"),
                OverviewItem("Last Backup:", _lastBackup));

            body.Controls.Add(grid);
            return section;
        }

        private static void AddOverviewRow(TableLayoutPanel grid, int rowIndex, Control c1, Control c2, Control c3)
        {
            if (grid.RowCount <= rowIndex) grid.RowCount = rowIndex + 1;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            c1.Dock = DockStyle.Top;
            c2.Dock = DockStyle.Top;
            c3.Dock = DockStyle.Top;

            grid.Controls.Add(c1, 0, rowIndex);
            grid.Controls.Add(c2, 1, rowIndex);
            grid.Controls.Add(c3, 2, rowIndex);
        }

        private static Control OverviewItem(string label, string value)
        {
            var host = new Panel { Dock = DockStyle.Top, Height = 28, Margin = new Padding(0, 3, 12, 3) };

            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 6)
            };

            var val = new Label
            {
                Text = value,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            host.Controls.Add(lbl);
            host.Controls.Add(val);

            host.Resize += (s, e) =>
            {
                val.Left = Math.Max(lbl.Right + 10, host.Width - val.Width - 6);
                val.Top = 6;
            };

            return host;
        }

        private Panel CreateUserManagementSection()
        {
            var section = SectionCard("USER MANAGEMENT", "👥", "#2563EB");
            var body = (Panel)section.Tag;

            var btnRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 10)
            };

            var btnAdd = ButtonFilled("Add New User", "#16A34A", Color.White);
            // open the real AddUserDialog and refresh overview after successful creation
            btnAdd.Click += (s, e) => ShowAddUserDialog();

            var btnDeactivate = ButtonOutline("Deactivate");
            btnDeactivate.Click += (s, e) => Toast("User deactivation dialog...");

            btnRow.Controls.Add(btnAdd);
            btnRow.Controls.Add(btnDeactivate);
            body.Controls.Add(btnRow);

            _usersGrid = MakeGrid();
            _usersGrid.Height = 160;

            _usersGrid.Columns.Add("Username", "Username");
            _usersGrid.Columns.Add("Role", "Role");
            _usersGrid.Columns.Add("Status", "Status");

            body.Controls.Add(_usersGrid);

            var linkPanel = new Panel { Dock = DockStyle.Top, Height = 28 };
            _viewAllUsersLink = new LinkLabel
            {
                Text = "View All " + _totalUsers.ToString(CultureInfo.InvariantCulture) + " Users →",
                AutoSize = true,
                LinkColor = ColorTranslator.FromHtml("#2563EB"),
                Location = new Point(0, 6)
            };
            _viewAllUsersLink.Click += (s, e) => Toast("Navigating to User Management...");
            linkPanel.Controls.Add(_viewAllUsersLink);

            body.Controls.Add(linkPanel);

            return section;
        }

        private Panel CreateLoanProductsSection()
        {
            var section = SectionCard("LOAN PRODUCT CONFIGURATION", "₱", "#16A34A");
            var body = (Panel)section.Tag;

            _loanProductsGrid = MakeGrid();
            _loanProductsGrid.Height = 140;

            _loanProductsGrid.Columns.Add("Product", "Product");
            _loanProductsGrid.Columns.Add("Rate", "Int. Rate");
            _loanProductsGrid.Columns.Add("Status", "Status");

            body.Controls.Add(_loanProductsGrid);

            var btnRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 0)
            };

            var btnAdd = ButtonOutline("Add New Product");
            // open AddNewLoanProductControl in a modal host and refresh overview after save
            btnAdd.Click += (s, e) => ShowAddProductDialog();

            var btnEdit = ButtonOutline("Edit Product");
            btnEdit.Click += (s, e) => Toast("Opening product edit form...");

            btnRow.Controls.Add(btnAdd);
            btnRow.Controls.Add(btnEdit);

            body.Controls.Add(btnRow);
            return section;
        }

        private void ShowAddUserDialog()
        {
            try
            {
                string createdUsername = null;

                using (var dlg = new AddUserDialog())
                {
                    dlg.UserCreated += (userData) =>
                    {
                        if (userData != null)
                            createdUsername = userData.Username;
                    };

                    var result = dlg.ShowDialog(FindForm());
                    if (result == DialogResult.OK)
                    {
                        // refresh overview grid and statistics (small, quick)
                        LoadUsersOverview();

                        if (!string.IsNullOrWhiteSpace(createdUsername))
                        {
                            Toast($"User '{createdUsername}' created.", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast("Failed to add user: " + ex.Message, true);
            }
        }

        private void ShowAddProductDialog()
        {
            try
            {
                using (var host = new Form())
                {
                    host.Text = "Add New Product";
                    host.StartPosition = FormStartPosition.CenterParent;
                    host.FormBorderStyle = FormBorderStyle.FixedDialog;
                    host.MaximizeBox = false;
                    host.MinimizeBox = false;
                    host.ClientSize = new Size(920, 760);

                    var addCtrl = new AddNewLoanProductControl();
                    addCtrl.Dock = DockStyle.Fill;
                    host.Controls.Add(addCtrl);

                    // Try to locate the Save button inside the control and attach a handler that refreshes overview AFTER the control's own handler runs.
                    var saveBtn = FindButtonByText(addCtrl, "Save Product");
                    if (saveBtn != null)
                    {
                        // Attach after a short delay to ensure existing handlers are already wired (handler order matters).
                        saveBtn.Click += (s, e) =>
                        {
                            // Slight delay to let the control complete its MessageBox / save logic.
                            var t = new Timer { Interval = 300 };
                            t.Tick += (ts, te) =>
                            {
                                t.Stop();
                                t.Dispose();
                                try
                                {
                                    LoadLoanProductsOverview();
                                    Toast("Loan products refreshed.", false);
                                }
                                catch
                                {
                                    // swallow; toast already shows on errors
                                }
                            };
                            t.Start();
                        };
                    }

                    host.ShowDialog(FindForm());
                }
            }
            catch (Exception ex)
            {
                Toast("Failed to open Add Product: " + ex.Message, true);
            }
        }

        private static Button FindButtonByText(Control parent, string text)
        {
            if (parent == null) return null;
            foreach (Control c in parent.Controls)
            {
                if (c is Button b && string.Equals(b.Text, text, StringComparison.OrdinalIgnoreCase))
                    return b;

                var childSearch = FindButtonByText(c, text);
                if (childSearch != null) return childSearch;
            }
            return null;
        }

        private Panel CreateSystemConfigSection()
        {
            var section = SectionCard("SYSTEM CONFIGURATION", "🔧", "#6B7280");
            var body = (Panel)section.Tag;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            table.Controls.Add(ConfigField("Penalty Rate", "2.0", "% monthly", out txtPenaltyRate), 0, 0);
            table.Controls.Add(ConfigField("Grace Period", "3", "days", out txtGracePeriod), 1, 0);
            table.Controls.Add(ConfigField("Receipt Prefix", "OR-", null, out txtReceiptPrefix), 0, 1);
            table.Controls.Add(ConfigField("Max Login Attempts", "3", null, out txtMaxLoginAttempts), 1, 1);
            table.Controls.Add(ConfigField("Session Timeout", "30", "minutes", out txtSessionTimeout), 0, 2);

            body.Controls.Add(table);

            var btnSave = ButtonFilled("Save Configuration", "#2563EB", Color.White);
            btnSave.Margin = new Padding(0, 12, 0, 0);
            btnSave.Click += (s, e) => Toast("System configuration saved successfully!");
            body.Controls.Add(btnSave);

            return section;
        }

        private static Panel ConfigField(string label, string initial, string suffix, out TextBox tb)
        {
            var host = new Panel { Dock = DockStyle.Top, Height = 66, Margin = new Padding(0, 0, 12, 8) };

            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 0)
            };

            var textBox = new TextBox
            {
                Text = initial,
                Width = 180,
                Location = new Point(0, 22)
            };

            host.Controls.Add(lbl);
            host.Controls.Add(textBox);

            if (!string.IsNullOrEmpty(suffix))
            {
                var sfx = new Label
                {
                    Text = suffix,
                    AutoSize = true,
                    ForeColor = ColorTranslator.FromHtml("#6B7280"),
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(textBox.Right + 8, 26)
                };
                host.Controls.Add(sfx);

                host.Resize += (s, e) =>
                {
                    textBox.Width = Math.Max(120, host.Width - 90);
                    sfx.Left = textBox.Right + 8;
                };
            }
            else
            {
                host.Resize += (s, e) =>
                {
                    textBox.Width = Math.Max(160, host.Width - 6);
                };
            }

            tb = textBox;
            return host;
        }

        private Panel CreateActionButtonsRow()
        {
            var host = new Panel { Dock = DockStyle.Top, Height = 54 };

            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            var btnBackup = ButtonFilled("Run System Backup", "#7C3AED", Color.White);
            btnBackup.Click += (s, e) =>
            {
                Toast("Starting system backup...");
                var t = new Timer { Interval = 2000 };
                t.Tick += (s2, e2) =>
                {
                    t.Stop();
                    t.Dispose();
                    Toast("System backup completed successfully!");
                };
                t.Start();
            };

            var btnAudit = ButtonFilled("Generate Audit Report", "#EA580C", Color.White);
            btnAudit.Click += (s, e) =>
            {
                Toast("Generating audit report...");
                var t = new Timer { Interval = 1500 };
                t.Tick += (s2, e2) =>
                {
                    t.Stop();
                    t.Dispose();
                    Toast("Audit report generated successfully!");
                };
                t.Start();
            };

            row.Controls.Add(btnBackup);
            row.Controls.Add(btnAudit);
            host.Controls.Add(row);

            return host;
        }

        private Panel CreateOverrideActionsCard()
        {
            var card = MakeCard();
            card.Padding = new Padding(0);
            card.AutoSize = true; // Let it auto-size based on content

            var header = CardHeader("RECENT OVERRIDE ACTIONS", "#FEF3C7", "⚠", "#EA580C");

            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(16)
            };

            var item1 = OverrideItem("Loan Approval", "LN-APP-102 (₱120,000)", "Admin", "14:30");
            var item2 = OverrideItem("User Reactivation", "User: pedro_lo", "Admin", "11:15");
            var item3 = OverrideItem("Interest Rate Adjustment", "Product: Emergency Loan", "Admin", "09:45");

            body.Controls.Add(item1);
            body.Controls.Add(item2);
            body.Controls.Add(item3);

            // Layout items vertically
            void LayoutBody()
            {
                int y = body.Padding.Top;
                int width = Math.Max(200, body.ClientSize.Width - body.Padding.Left - body.Padding.Right);

                foreach (Control c in body.Controls)
                {
                    c.Width = width;
                    c.Left = body.Padding.Left;
                    c.Top = y;
                    y += c.Height + 10; // 10px margin between items
                }

                // Update body height based on content
                body.Height = y;
            }

            body.SizeChanged += (s, e) => LayoutBody();
            body.HandleCreated += (s, e) => LayoutBody();

            var footer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(16, 8, 16, 16)
            };

            var btnViewAll = ButtonOutline("View All Override Logs");
            btnViewAll.Click += (s, e) => Toast("Opening override logs...");
            btnViewAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnViewAll.Location = new Point(footer.Width - btnViewAll.Width - 16, 8);
            footer.Controls.Add(btnViewAll);

            // IMPORTANT: Add controls in REVERSE docking order
            // Footer first (bottom), then body, then header (top)
            card.Controls.Add(footer);
            card.Controls.Add(body);
            card.Controls.Add(header);

            return card;
        }

        private static Panel OverrideItem(string type, string details, string by, string time)
        {
            var item = new Panel
            {
                Height = 82,
                BackColor = ColorTranslator.FromHtml("#FFFBEB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 12, 12, 12)
            };

            var accent = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#F59E0B"),
                Width = 4,
                Height = item.Height - 2, // Slightly less than full height
                Location = new Point(0, 1)
            };

            var lblTitle = new Label
            {
                Text = "Override - " + type,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 8) // Leave space for accent
            };

            var lblDetails = new Label
            {
                Text = details,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 28)
            };

            var lblMeta = new Label
            {
                Text = "By: " + by + "   |   Time: " + time,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(16, 48)
            };

            item.Controls.Add(lblTitle);
            item.Controls.Add(lblDetails);
            item.Controls.Add(lblMeta);
            item.Controls.Add(accent); // Add accent last so it's in the background

            // Bring labels to front
            lblTitle.BringToFront();
            lblDetails.BringToFront();
            lblMeta.BringToFront();

            return item;
        }

        // ===== Grid + Button helpers =====
        private static DataGridView MakeGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Top,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = ColorTranslator.FromHtml("#E5E7EB"),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 32,
                RowTemplate = { Height = 28 }
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F9FAFB");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#374151");
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;

            return grid;
        }

        private static Button ButtonFilled(string text, string backHex, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Height = 34,
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml(backHex),
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Padding = new Padding(12, 0, 12, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static Button ButtonOutline(string text)
        {
            var btn = new Button
            {
                Text = text,
                Height = 34,
                AutoSize = true,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Padding = new Padding(12, 0, 12, 0)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }

        // ===== Toast =====
        private void BuildToast()
        {
            _toastPanel = new Panel
            {
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#111827"),
                Padding = new Padding(12, 8, 12, 8),
                Visible = false
            };

            _toastLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            _toastPanel.Controls.Add(_toastLabel);
            Controls.Add(_toastPanel);
            _toastPanel.BringToFront();

            _toastTimer = new Timer { Interval = 2200 };
            _toastTimer.Tick += (s, e) =>
            {
                _toastTimer.Stop();
                _toastPanel.Visible = false;
            };

            Resize += (s, e) => PositionToast();
            PositionToast();
        }

        private void PositionToast()
        {
            if (_toastPanel == null) return;
            _toastPanel.Left = ClientSize.Width - _toastPanel.Width - 12;
            _toastPanel.Top = 12;
        }

        private void Toast(string msg, bool isError = false)
        {
            if (_toastPanel == null || _toastLabel == null) return;

            _toastPanel.BackColor = isError ? ColorTranslator.FromHtml("#991B1B") : ColorTranslator.FromHtml("#111827");
            _toastLabel.Text = msg;

            _toastPanel.Visible = true;
            _toastPanel.BringToFront();
            _toastPanel.PerformLayout();
            PositionToast();

            _toastTimer.Stop();
            _toastTimer.Start();
        }
    }
}