using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EditUserForm = LendingSystem.UI.EditUserForm;
using EditFormUser = LendingSystem.UI.User;
using ResetPasswordForm = LendingApp.UI.AdminUI.ResetPasswordForm;

namespace LendingApp.UI.AdminUI.Views
{
    public partial class AdminUserManagementControl : UserControl
    {
        // User class definition
        public class User
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string EmployeeId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public string CreatedDate { get; set; }
            public string LastLogin { get; set; }
        }

        // Filter states
        private string searchQuery = "";
        private string roleFilter = "all";
        private string statusFilter = "all";

        // Pagination
        private int currentPage = 1;
        private const int itemsPerPage = 6;

        // Selected user
        private User selectedUser = null;

        // Mock users data
        private readonly List<User> allUsers = new List<User>
        {
            new User
            {
                Id = "1",
                Username = "admin",
                FullName = "Administrator",
                Role = "Admin",
                EmployeeId = "ADMIN-001",
                Email = "admin@company.com",
                Phone = "+639171111111",
                Status = "Active",
                CreatedDate = "2024-01-01",
                LastLogin = "2024-06-10 10:30 AM"
            },
            new User
            {
                Id = "2",
                Username = "maria_s",
                FullName = "Maria Santos",
                Role = "Cashier",
                EmployeeId = "CSH-001",
                Email = "maria.santos@company.com",
                Phone = "+639172222222",
                Status = "Active",
                CreatedDate = "2024-01-10",
                LastLogin = "2024-06-10 08:45 AM"
            },
            new User
            {
                Id = "3",
                Username = "juan_lo",
                FullName = "Juan Dela Cruz",
                Role = "LoanOfficer",
                EmployeeId = "LO-002",
                Email = "juan.delacruz@company.com",
                Phone = "+639171234567",
                Status = "Active",
                CreatedDate = "2024-01-15",
                LastLogin = "2024-06-10 09:15 AM"
            },
            new User
            {
                Id = "4",
                Username = "pedro_lo",
                FullName = "Pedro Rodriguez",
                Role = "LoanOfficer",
                EmployeeId = "LO-003",
                Email = "pedro.rodriguez@company.com",
                Phone = "+639173333333",
                Status = "Inactive",
                CreatedDate = "2024-02-01",
                LastLogin = "2024-05-20 02:30 PM"
            },
            new User
            {
                Id = "5",
                Username = "ana_c",
                FullName = "Ana Garcia",
                Role = "Cashier",
                EmployeeId = "CSH-002",
                Email = "ana.garcia@company.com",
                Phone = "+639174444444",
                Status = "Locked",
                CreatedDate = "2024-02-15",
                LastLogin = "2024-06-09 11:00 AM"
            },
            new User
            {
                Id = "6",
                Username = "luis_lo",
                FullName = "Luis Martinez",
                Role = "LoanOfficer",
                EmployeeId = "LO-004",
                Email = "luis.martinez@company.com",
                Phone = "+639175555555",
                Status = "Active",
                CreatedDate = "2024-03-01",
                LastLogin = "2024-06-10 07:20 AM"
            },
            new User
            {
                Id = "7",
                Username = "rosa_c",
                FullName = "Rosa Fernandez",
                Role = "Cashier",
                EmployeeId = "CSH-003",
                Email = "rosa.fernandez@company.com",
                Phone = "+639176666666",
                Status = "Active",
                CreatedDate = "2024-03-10",
                LastLogin = "2024-06-10 08:00 AM"
            },
            new User
            {
                Id = "8",
                Username = "carlos_lo",
                FullName = "Carlos Reyes",
                Role = "LoanOfficer",
                EmployeeId = "LO-005",
                Email = "carlos.reyes@company.com",
                Phone = "+639177777777",
                Status = "Active",
                CreatedDate = "2024-03-20",
                LastLogin = "2024-06-09 04:15 PM"
            },
            new User
            {
                Id = "9",
                Username = "elena_c",
                FullName = "Elena Torres",
                Role = "Cashier",
                EmployeeId = "CSH-004",
                Email = "elena.torres@company.com",
                Phone = "+639178888888",
                Status = "Active",
                CreatedDate = "2024-04-01",
                LastLogin = "2024-06-10 09:30 AM"
            },
            new User
            {
                Id = "10",
                Username = "miguel_lo",
                FullName = "Miguel Ramos",
                Role = "LoanOfficer",
                EmployeeId = "LO-006",
                Email = "miguel.ramos@company.com",
                Phone = "+639179999999",
                Status = "Active",
                CreatedDate = "2024-04-15",
                LastLogin = "2024-06-10 10:00 AM"
            },
            new User
            {
                Id = "11",
                Username = "sofia_c",
                FullName = "Sofia Morales",
                Role = "Cashier",
                EmployeeId = "CSH-005",
                Email = "sofia.morales@company.com",
                Phone = "+639170000000",
                Status = "Active",
                CreatedDate = "2024-05-01",
                LastLogin = "2024-06-10 08:30 AM"
            },
            new User
            {
                Id = "12",
                Username = "diego_lo",
                FullName = "Diego Castillo",
                Role = "LoanOfficer",
                EmployeeId = "LO-007",
                Email = "diego.castillo@company.com",
                Phone = "+639171111112",
                Status = "Active",
                CreatedDate = "2024-05-10",
                LastLogin = "2024-06-09 03:45 PM"
            },
            new User
            {
                Id = "13",
                Username = "carmen_c",
                FullName = "Carmen Flores",
                Role = "Cashier",
                EmployeeId = "CSH-006",
                Email = "carmen.flores@company.com",
                Phone = "+639172222223",
                Status = "Active",
                CreatedDate = "2024-05-15",
                LastLogin = "2024-06-10 07:50 AM"
            },
            new User
            {
                Id = "14",
                Username = "ricardo_lo",
                FullName = "Ricardo Navarro",
                Role = "LoanOfficer",
                EmployeeId = "LO-008",
                Email = "ricardo.navarro@company.com",
                Phone = "+639173333334",
                Status = "Active",
                CreatedDate = "2024-06-01",
                LastLogin = "2024-06-10 09:45 AM"
            },
            new User
            {
                Id = "15",
                Username = "isabel_lo",
                FullName = "Isabel Romero",
                Role = "LoanOfficer",
                EmployeeId = "LO-009",
                Email = "isabel.romero@company.com",
                Phone = "+639174444445",
                Status = "Active",
                CreatedDate = "2024-06-05",
                LastLogin = "2024-06-10 08:15 AM"
            }
        };

        // UI Controls
        private TextBox txtSearch;
        private Label lblSearchPlaceholder;
        private ComboBox cmbRoleFilter;
        private ComboBox cmbStatusFilter;
        private DataGridView dgvUsers;
        private Button btnPrevious;
        private Button btnNext;
        private Label lblPagination;
        private Panel pnlUserDetails;

        // Main containers
        private Panel mainCard;
        private Panel searchFilterCard;
        private Panel userListCard;
        private Panel userDetailsCard;
        private Panel statisticsCard;

        public AdminUserManagementControl()
        {
            Dock = DockStyle.Fill;
            BackColor = ColorTranslator.FromHtml("#F9FAFB");

            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            // Main container with auto-scroll
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16),
                BackColor = ColorTranslator.FromHtml("#F9FAFB")
            };

            var mainContainer = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml("#F9FAFB")
            };

            // ===== Main Card =====
            mainCard = CreateMainCard();
            mainContainer.Controls.Add(mainCard);

            // ===== Statistics Card =====
            statisticsCard = CreateStatisticsCard();
            statisticsCard.Margin = new Padding(0, 0, 0, 16);
            mainContainer.Controls.Add(statisticsCard);

            scrollPanel.Controls.Add(mainContainer);
            Controls.Add(scrollPanel);

            // Initial load of user list (after all controls are created)
            UpdateUserList();
        }

        private Panel CreateMainCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 16)
            };

            // Add blue border
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(ColorTranslator.FromHtml("#93C5FD"), 2))
                {
                    var rect = card.ClientRectangle;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            // Header
            var header = CreateCardHeader("USER MANAGEMENT", "#DBEAFE", "👥", "#2563EB");

            // Body
            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(24, 16, 24, 16)
            };

            // Add sections to body
            body.Controls.Add(CreateSearchFilterSection());
            body.Controls.Add(CreateUserListSection());
            body.Controls.Add(CreateUserDetailsSection());
            body.Controls.Add(CreateActionButtons());

            // Add in correct docking order
            card.Controls.Add(body);
            card.Controls.Add(header);

            return card;
        }

        private Panel CreateCardHeader(string title, string backHex, string iconText, string iconHex)
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

        private Panel CreateSearchFilterSection()
        {
            searchFilterCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16),
                Margin = new Padding(0, 0, 0, 16)
            };

            // Title
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 24,
                Margin = new Padding(0, 0, 0, 12)
            };

            var titleIcon = new Label
            {
                Text = "🔍",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 4)
            };

            var titleLabel = new Label
            {
                Text = "SEARCH & FILTER",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(20, 4)
            };

            titlePanel.Controls.Add(titleIcon);
            titlePanel.Controls.Add(titleLabel);

            // Search
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Margin = new Padding(0, 0, 0, 12)
            };

            var lblSearch = new Label
            {
                Text = "Search",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 0)
            };

            var searchContainer = new Panel
            {
                Height = 32,
                Top = 20
            };

            txtSearch = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 32,
                Font = new Font("Segoe UI", 9),
                Left = 0,
                ForeColor = ColorTranslator.FromHtml("#6B7280")
            };

            // Create placeholder label
            lblSearchPlaceholder = new Label
            {
                Text = "Search by username, name, or email...",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#9CA3AF"),
                Location = new Point(4, 8),
                BackColor = Color.White,
                Cursor = Cursors.IBeam
            };

            // Handle placeholder visibility
            txtSearch.GotFocus += (s, e) =>
            {
                lblSearchPlaceholder.Visible = false;
                txtSearch.ForeColor = ColorTranslator.FromHtml("#111827");
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    lblSearchPlaceholder.Visible = true;
                    txtSearch.ForeColor = ColorTranslator.FromHtml("#6B7280");
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                searchQuery = txtSearch.Text;
                ApplyFilters();

                // Update placeholder visibility
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    lblSearchPlaceholder.Visible = false;
                    txtSearch.ForeColor = ColorTranslator.FromHtml("#111827");
                }
            };

            // Click placeholder to focus textbox
            lblSearchPlaceholder.Click += (s, e) =>
            {
                txtSearch.Focus();
            };

            var btnSearch = new Button
            {
                Text = "🔍",
                Width = 40,
                Height = 32,
                Left = txtSearch.Right + 8,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            btnSearch.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnSearch.FlatAppearance.BorderSize = 1;
            btnSearch.Click += (s, e) => ApplyFilters();

            searchContainer.Controls.Add(txtSearch);
            searchContainer.Controls.Add(lblSearchPlaceholder);
            searchContainer.Controls.Add(btnSearch);
            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(searchContainer);

            // Filters
            var filtersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Margin = new Padding(0, 0, 0, 12)
            };

            var rolePanel = new Panel
            {
                Width = 200,
                Height = 56,
                Location = new Point(0, 0)
            };

            var lblRole = new Label
            {
                Text = "Role",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 0)
            };

            cmbRoleFilter = new ComboBox
            {
                Width = 200,
                Height = 32,
                Top = 20,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbRoleFilter.Items.AddRange(new object[] { "All Roles", "Admin", "Loan Officer", "Cashier" });
            cmbRoleFilter.SelectedIndex = 0;
            cmbRoleFilter.SelectedIndexChanged += (s, e) =>
            {
                roleFilter = cmbRoleFilter.SelectedItem.ToString() == "All Roles" ? "all" :
                           cmbRoleFilter.SelectedItem.ToString() == "Loan Officer" ? "LoanOfficer" :
                           cmbRoleFilter.SelectedItem.ToString();
                ApplyFilters();
            };

            rolePanel.Controls.Add(lblRole);
            rolePanel.Controls.Add(cmbRoleFilter);

            var statusPanel = new Panel
            {
                Width = 200,
                Height = 56,
                Location = new Point(220, 0)
            };

            var lblStatus = new Label
            {
                Text = "Status",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, 0)
            };

            cmbStatusFilter = new ComboBox
            {
                Width = 200,
                Height = 32,
                Top = 20,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbStatusFilter.Items.AddRange(new object[] { "All Statuses", "Active", "Inactive", "Locked" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) =>
            {
                statusFilter = cmbStatusFilter.SelectedItem.ToString() == "All Statuses" ? "all" :
                               cmbStatusFilter.SelectedItem.ToString();
                ApplyFilters();
            };

            statusPanel.Controls.Add(lblStatus);
            statusPanel.Controls.Add(cmbStatusFilter);

            filtersPanel.Controls.Add(rolePanel);
            filtersPanel.Controls.Add(statusPanel);

            // Filter buttons
            var buttonsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 0, 0, 8)
            };

            var btnApplyFilters = CreateButton("Apply Filters", "#2563EB", Color.White);
            btnApplyFilters.Click += (s, e) => ApplyFilters();

            var btnClearFilters = CreateOutlineButton("Clear Filters");
            btnClearFilters.Click += (s, e) =>
            {
                txtSearch.Text = "";
                cmbRoleFilter.SelectedIndex = 0;
                cmbStatusFilter.SelectedIndex = 0;
                searchQuery = "";
                roleFilter = "all";
                statusFilter = "all";
                lblSearchPlaceholder.Visible = true;
                txtSearch.ForeColor = ColorTranslator.FromHtml("#6B7280");
                ApplyFilters();
            };

            buttonsPanel.Controls.Add(btnApplyFilters);
            buttonsPanel.Controls.Add(btnClearFilters);

            // Layout buttons
            buttonsPanel.Layout += (s, e) =>
            {
                btnApplyFilters.Left = 0;
                btnClearFilters.Left = btnApplyFilters.Right + 8;
            };

            // Add all to card
            searchFilterCard.Controls.Add(buttonsPanel);
            searchFilterCard.Controls.Add(filtersPanel);
            searchFilterCard.Controls.Add(searchPanel);
            searchFilterCard.Controls.Add(titlePanel);

            return searchFilterCard;
        }

        private Panel CreateUserListSection()
        {
            userListCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16),
                Margin = new Padding(0, 0, 0, 16)
            };

            // Title
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 24,
                Margin = new Padding(0, 0, 0, 12)
            };

            var titleIcon = new Label
            {
                Text = "👥",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 4)
            };

            var titleLabel = new Label
            {
                Text = "USER LIST",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(20, 4)
            };

            titlePanel.Controls.Add(titleIcon);
            titlePanel.Controls.Add(titleLabel);

            // DataGridView for users
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 200,
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
                RowTemplate = { Height = 36 },
                Font = new Font("Segoe UI", 9)
            };

            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F9FAFB");
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#374151");
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvUsers.EnableHeadersVisualStyles = false;

            // Style for selected row
            dgvUsers.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EFF6FF");
            dgvUsers.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111827");

            // Create columns
            dgvUsers.Columns.Add("Username", "Username");
            dgvUsers.Columns.Add("Role", "Role");
            dgvUsers.Columns.Add("Status", "Status");

            dgvUsers.SelectionChanged += (s, e) =>
            {
                if (dgvUsers.SelectedRows.Count > 0)
                {
                    var selectedUsername = dgvUsers.SelectedRows[0].Cells["Username"].Value?.ToString();
                    if (!string.IsNullOrEmpty(selectedUsername))
                    {
                        selectedUser = allUsers.FirstOrDefault(u => u.Username == selectedUsername);
                        UpdateUserDetails();
                    }
                }
            };

            // Pagination
            var paginationPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(0, 12, 0, 0)
            };

            btnPrevious = CreateOutlineButton("◀ Previous");
            btnPrevious.Click += (s, e) =>
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    UpdateUserList();
                }
            };

            lblPagination = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "1-6 of 15" // Default text
            };

            btnNext = CreateOutlineButton("Next ▶");
            btnNext.Click += (s, e) =>
            {
                var filteredUsers = GetFilteredUsers();
                var totalPages = (int)Math.Ceiling(filteredUsers.Count / (double)itemsPerPage);

                if (currentPage < totalPages)
                {
                    currentPage++;
                    UpdateUserList();
                }
            };

            paginationPanel.Controls.Add(btnPrevious);
            paginationPanel.Controls.Add(lblPagination);
            paginationPanel.Controls.Add(btnNext);

            // Layout pagination
            paginationPanel.Layout += (s, e) =>
            {
                btnPrevious.Left = 0;
                lblPagination.Left = paginationPanel.Width / 2 - lblPagination.Width / 2;
                btnNext.Left = paginationPanel.Width - btnNext.Width;

                btnPrevious.Top = 8;
                lblPagination.Top = 12;
                btnNext.Top = 8;
            };

            // Add all to card
            userListCard.Controls.Add(paginationPanel);
            userListCard.Controls.Add(dgvUsers);
            userListCard.Controls.Add(titlePanel);

            return userListCard;
        }

        private Panel CreateUserDetailsSection()
        {
            userDetailsCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16),
                Margin = new Padding(0, 0, 0, 16),
                Visible = false
            };

            // Title
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 24,
                Margin = new Padding(0, 0, 0, 12)
            };

            var titleIcon = new Label
            {
                Text = "👤",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 4)
            };

            var titleLabel = new Label
            {
                Text = "SELECTED USER DETAILS",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(20, 4)
            };

            titlePanel.Controls.Add(titleIcon);
            titlePanel.Controls.Add(titleLabel);

            // User details container
            pnlUserDetails = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true
            };

            // Action buttons
            var actionButtonsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 12, 0, 0)
            };

            var btnEdit = CreateOutlineButton("✏ Edit User");
            btnEdit.Click += (s, e) => CreateAndShowEditUserForm();

            var btnResetPassword = CreateOutlineButton("🔑 Reset Password");
            btnResetPassword.Click += (s, e) => ResetPassword();

            var btnDeactivate = CreateOutlineButton("👤 Deactivate");
            btnDeactivate.ForeColor = ColorTranslator.FromHtml("#EA580C");
            btnDeactivate.Click += (s, e) =>
            {
                if (selectedUser != null)
                    ShowMessage($"User {selectedUser.Username} deactivated", true);
            };

            var btnDelete = CreateOutlineButton("🗑 Delete");
            btnDelete.ForeColor = ColorTranslator.FromHtml("#DC2626");
            btnDelete.Click += (s, e) =>
            {
                if (selectedUser != null)
                    ShowMessage($"User {selectedUser.Username} deleted", true);
            };

            actionButtonsPanel.Controls.Add(btnEdit);
            actionButtonsPanel.Controls.Add(btnResetPassword);
            actionButtonsPanel.Controls.Add(btnDeactivate);
            actionButtonsPanel.Controls.Add(btnDelete);

            // Layout action buttons
            actionButtonsPanel.Layout += (s, e) =>
            {
                int x = 0;
                foreach (Control btn in actionButtonsPanel.Controls)
                {
                    btn.Left = x;
                    btn.Top = 0;
                    x += btn.Width + 8;
                }
            };

            userDetailsCard.Controls.Add(actionButtonsPanel);
            userDetailsCard.Controls.Add(pnlUserDetails);
            userDetailsCard.Controls.Add(titlePanel);

            return userDetailsCard;
        }

        private Panel CreateActionButtons()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(0, 8, 0, 0)
            };

            var btnAddUser = CreateButton("➕ Add New User", "#16A34A", Color.White);
            btnAddUser.Click += (s, e) => ShowAddUserDialog();

            var btnExportUsers = CreateOutlineButton("📥 Export Users");
            btnExportUsers.Click += (s, e) => ShowMessage("Exporting user list...");

            panel.Controls.Add(btnAddUser);
            panel.Controls.Add(btnExportUsers);

            panel.Layout += (s, e) =>
            {
                btnAddUser.Left = 0;
                btnExportUsers.Left = btnAddUser.Right + 8;
            };

            return panel;
        }

        private void ShowAddUserDialog()
        {
            var addUserDialog = new AddUserDialog();
            addUserDialog.UserCreated += (userData) =>
            {
                // Map role from dialog format to our format using traditional switch
                string mappedRole;
                switch (userData.Role)
                {
                    case "Loan Officer":
                        mappedRole = "LoanOfficer";
                        break;
                    default:
                        mappedRole = userData.Role; // Admin or Cashier stay the same
                        break;
                }

                // Add the new user to the list
                var newUser = new User
                {
                    Id = (allUsers.Count + 1).ToString(),
                    Username = userData.Username,
                    FullName = userData.FullName,
                    Role = mappedRole,
                    Email = userData.Email,
                    Phone = userData.Phone,
                    LastLogin = "Never"
                };

                allUsers.Add(newUser);

                // Refresh the user list
                UpdateUserList();

                // Show success message
                ShowMessage($"User '{userData.Username}' added successfully!");

                // Auto-select the new user in the grid
                SelectNewUserInGrid(newUser.Username);
            };

            addUserDialog.ShowDialog();
        }

        private void SelectNewUserInGrid(string username)
        {
            if (dgvUsers == null) return;

            // Find and select the new user in the DataGridView
            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.Cells["Username"].Value?.ToString() == username)
                {
                    row.Selected = true;
                    dgvUsers.CurrentCell = row.Cells[0];

                    // Ensure the row is visible
                    dgvUsers.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private Panel CreateStatisticsCard()
        {
            statisticsCard = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header with gradient-like background
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorTranslator.FromHtml("#EFF6FF"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 0, 16, 0)
            };

            var icon = new Label
            {
                Text = "📊",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2563EB"),
                Location = new Point(16, 14)
            };

            var lblTitle = new Label
            {
                Text = "USER STATISTICS",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(50, 18)
            };

            header.Controls.Add(icon);
            header.Controls.Add(lblTitle);

            // Body
            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(24, 16, 24, 16)
            };

            // Calculate statistics
            var totalUsers = allUsers.Count;
            var adminCount = allUsers.Count(u => u.Role == "Admin");
            var loanOfficerCount = allUsers.Count(u => u.Role == "LoanOfficer");
            var cashierCount = allUsers.Count(u => u.Role == "Cashier");
            var activeCount = allUsers.Count(u => u.Status == "Active");
            var inactiveCount = allUsers.Count(u => u.Status == "Inactive");
            var lockedCount = allUsers.Count(u => u.Status == "Locked");

            // Create statistics content
            var statsContainer = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true
            };

            var lblTotalUsers = new Label
            {
                Text = $"Total Users: {totalUsers}",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };

            var lblRoleStats = new Label
            {
                Text = $"• Admin: {adminCount} ({((double)adminCount / totalUsers * 100):F1}%)\n" +
                       $"• Loan Officers: {loanOfficerCount} ({((double)loanOfficerCount / totalUsers * 100):F1}%)\n" +
                       $"• Cashiers: {cashierCount} ({((double)cashierCount / totalUsers * 100):F1}%)",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 28)
            };

            var lblStatusStats = new Label
            {
                Text = $"\nActive: {activeCount} ({((double)activeCount / totalUsers * 100):F1}%)\n" +
                       $"Inactive: {inactiveCount} ({((double)inactiveCount / totalUsers * 100):F1}%)\n" +
                       $"Locked: {lockedCount} ({((double)lockedCount / totalUsers * 100):F1}%)",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, lblRoleStats.Bottom + 20)
            };

            var lblRecentStats = new Label
            {
                Text = "Last 7 Days:\n" +
                       "• New Users: 2\n" +
                       "• Password Resets: 3\n" +
                       "• Failed Logins: 12",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, lblStatusStats.Bottom + 20)
            };

            statsContainer.Controls.Add(lblTotalUsers);
            statsContainer.Controls.Add(lblRoleStats);
            statsContainer.Controls.Add(lblStatusStats);
            statsContainer.Controls.Add(lblRecentStats);

            body.Controls.Add(statsContainer);

            statisticsCard.Controls.Add(body);
            statisticsCard.Controls.Add(header);

            return statisticsCard;
        }

        private List<User> GetFilteredUsers()
        {
            return allUsers.Where(user =>
            {
                var matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                                   user.Username.ToLower().Contains(searchQuery.ToLower()) ||
                                   user.FullName.ToLower().Contains(searchQuery.ToLower()) ||
                                   user.Email.ToLower().Contains(searchQuery.ToLower());

                var matchesRole = roleFilter == "all" || user.Role == roleFilter;
                var matchesStatus = statusFilter == "all" || user.Status == statusFilter;

                return matchesSearch && matchesRole && matchesStatus;
            }).ToList();
        }

        private void ApplyFilters()
        {
            currentPage = 1;
            UpdateUserList();
            ShowMessage("Filters applied");
        }

        private void UpdateUserList()
        {
            if (dgvUsers == null) return;

            dgvUsers.Rows.Clear();

            var filteredUsers = GetFilteredUsers();
            var totalPages = (int)Math.Ceiling(filteredUsers.Count / (double)itemsPerPage);
            var startIndex = (currentPage - 1) * itemsPerPage;
            var endIndex = Math.Min(startIndex + itemsPerPage, filteredUsers.Count);

            // Add users for current page
            for (int i = startIndex; i < endIndex; i++)
            {
                var user = filteredUsers[i];
                dgvUsers.Rows.Add(user.Username, user.Role, user.Status);
            }

            // Update pagination label if it exists
            if (lblPagination != null)
            {
                lblPagination.Text = $"{startIndex + 1}-{endIndex} of {filteredUsers.Count}";
            }

            // Update button states if they exist
            if (btnPrevious != null)
                btnPrevious.Enabled = currentPage > 1;

            if (btnNext != null)
                btnNext.Enabled = currentPage < totalPages;
        }

        private void UpdateUserDetails()
        {
            if (selectedUser == null || pnlUserDetails == null)
            {
                if (userDetailsCard != null)
                    userDetailsCard.Visible = false;
                return;
            }

            userDetailsCard.Visible = true;
            pnlUserDetails.Controls.Clear();

            // Create details grid
            var detailsGrid = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0, 0, 0, 12)
            };
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            // Helper to add detail row
            void AddDetailRow(string label, string value, int row)
            {
                var lblField = new Label
                {
                    Text = label,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8),
                    ForeColor = ColorTranslator.FromHtml("#6B7280"),
                    Margin = new Padding(0, 4, 0, 4)
                };

                var lblValue = new Label
                {
                    Text = value,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = ColorTranslator.FromHtml("#111827"),
                    Margin = new Padding(0, 4, 0, 4)
                };

                detailsGrid.Controls.Add(lblField, 0, row);
                detailsGrid.Controls.Add(lblValue, 1, row);
            }

            // Add all user details
            AddDetailRow("Username:", selectedUser.Username, 0);
            AddDetailRow("Full Name:", selectedUser.FullName, 1);
            AddDetailRow("Role:", selectedUser.Role, 2);
            AddDetailRow("Employee ID:", selectedUser.EmployeeId, 3);
            AddDetailRow("Email:", selectedUser.Email, 4);
            AddDetailRow("Phone:", selectedUser.Phone, 5);
            AddDetailRow("Created:", selectedUser.CreatedDate, 6);
            AddDetailRow("Last Login:", selectedUser.LastLogin, 7);
            AddDetailRow("Status:", selectedUser.Status, 8);

            pnlUserDetails.Controls.Add(detailsGrid);
        }

        private void ResetPassword()
        {
            if (selectedUser == null)
            {
                ShowMessage("Please select a user first.", true);
                return;
            }

            try
            {
                using (var form = new ResetPasswordForm(
                    selectedUser.Username,
                    selectedUser.FullName,
                    selectedUser.Role,
                    selectedUser.Email))
                {
                    if (form.ShowDialog(FindForm()) == DialogResult.OK)
                    {
                        ShowMessage($"Password for {selectedUser.Username} has been reset successfully!");

                        // Update the user's last login
                        selectedUser.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        UpdateUserList();
                        UpdateUserDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error opening reset password dialog: {ex.Message}", true);
            }
        }

        private void CreateAndShowEditUserForm()
        {
            if (selectedUser == null)
            {
                ShowMessage("Please select a user to edit.", true);
                return;
            }

            // Map our internal user model to the form's user model
            var editUser = new EditFormUser
            {
                Username = selectedUser.Username,
                FullName = selectedUser.FullName,
                Email = selectedUser.Email,
                Phone = selectedUser.Phone,
                Role = MapRoleToEditFormDisplay(selectedUser.Role),
                EmployeeId = selectedUser.EmployeeId,
                Status = selectedUser.Status,
                CreatedDate = selectedUser.CreatedDate,
                LastLogin = selectedUser.LastLogin
            };

            using (var form = new EditUserForm(editUser))
            {
                form.OnUserUpdated += updated =>
                {
                    // Copy edits back into our internal user model
                    selectedUser.FullName = updated.FullName;
                    selectedUser.Email = updated.Email;
                    selectedUser.Phone = updated.Phone;
                    selectedUser.Role = MapRoleFromEditFormDisplay(updated.Role);
                    selectedUser.Status = updated.Status;

                    // Refresh UI
                    UpdateUserList();
                    SelectNewUserInGrid(selectedUser.Username);
                    UpdateUserDetails();
                };

                form.ShowDialog(FindForm());
            }
        }

        private static string MapRoleToEditFormDisplay(string role)
        {
            // Our list uses "LoanOfficer" while the edit form dropdown uses "Loan Officer".
            if (string.Equals(role, "LoanOfficer", StringComparison.OrdinalIgnoreCase))
                return "Loan Officer";

            return role ?? string.Empty;
        }

        private static string MapRoleFromEditFormDisplay(string role)
        {
            if (string.Equals(role, "Loan Officer", StringComparison.OrdinalIgnoreCase))
                return "LoanOfficer";

            return role ?? string.Empty;
        }

        private Button CreateButton(string text, string backHex, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Height = 34,
                AutoSize = true,
                BackColor = ColorTranslator.FromHtml(backHex),
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Padding = new Padding(12, 0, 12, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Button CreateOutlineButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Height = 34,
                AutoSize = true,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Padding = new Padding(12, 0, 12, 0)
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btn.FlatAppearance.BorderSize = 1;
            return btn;
        }

        private void ShowMessage(string message, bool isWarning = false)
        {
            MessageBox.Show(message, "User Management",
                MessageBoxButtons.OK,
                isWarning ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }
    }
}