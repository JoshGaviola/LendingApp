using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.Class.Models.Admin;
using MySql.Data.MySqlClient;
using EditUserForm = LendingSystem.UI.EditUserForm;
using EditFormUser = LendingSystem.UI.User;
using ResetPasswordForm = LendingApp.UI.AdminUI.ResetPasswordForm;

namespace LendingApp.UI.AdminUI.Views
{
    public partial class AdminUserManagementControl : UserControl
    {
        // UI model (keeps UI independent from EF entity)
        public class UserRow
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
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
        private UserRow selectedUser = null;

        // Backing store (loaded from DB)
        private List<UserRow> allUsers = new List<UserRow>();

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

        // Toggle Active button (moved to field so other methods can update it)
        private Button btnToggleActive;

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
            LoadUsersFromDb();
        }

        private void LoadUsersFromDb()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var users = db.Users.AsNoTracking().ToList();

                    allUsers = users.Select(u => new UserRow
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                        Role = u.Role ?? "",
                        Email = u.Email ?? "",
                        Status = u.IsActive ? "Active" : "Inactive",
                        CreatedDate = u.CreatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        LastLogin = u.LastLogin.HasValue
                            ? u.LastLogin.Value.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture)
                            : "Never"
                    }).ToList();
                }

                selectedUser = null;
                UpdateStatisticsCard();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to load users", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allUsers = new List<UserRow>();
                selectedUser = null;
                UpdateStatisticsCard();
                ApplyFilters();
            }
        }

        private void BuildUI()
        {
            Controls.Clear();

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

            mainCard = CreateMainCard();
            mainContainer.Controls.Add(mainCard);

            statisticsCard = CreateStatisticsCard();
            statisticsCard.Margin = new Padding(0, 0, 0, 16);
            mainContainer.Controls.Add(statisticsCard);

            scrollPanel.Controls.Add(mainContainer);
            Controls.Add(scrollPanel);

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

            var header = CreateCardHeader("USER MANAGEMENT", "#DBEAFE", "👥", "#2563EB");

            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(24, 16, 24, 16)
            };

            body.Controls.Add(CreateSearchFilterSection());
            body.Controls.Add(CreateUserListSection());
            body.Controls.Add(CreateUserDetailsSection());
            body.Controls.Add(CreateActionButtons());

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

            var titlePanel = new Panel { Dock = DockStyle.Top, Height = 24, Margin = new Padding(0, 0, 0, 12) };
            titlePanel.Controls.Add(new Label { Text = "🔍", AutoSize = true, Font = new Font("Segoe UI", 9), Location = new Point(0, 4) });
            titlePanel.Controls.Add(new Label { Text = "SEARCH & FILTER", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(20, 4) });

            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 56, Margin = new Padding(0, 0, 0, 12) };
            searchPanel.Controls.Add(new Label { Text = "Search", AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#374151"), Location = new Point(0, 0) });

            var searchContainer = new Panel { Height = 32, Top = 20 };

            txtSearch = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 32,
                Font = new Font("Segoe UI", 9),
                Left = 0,
                ForeColor = ColorTranslator.FromHtml("#6B7280")
            };

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
                searchQuery = txtSearch.Text ?? "";
                ApplyFilters();

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    lblSearchPlaceholder.Visible = false;
                    txtSearch.ForeColor = ColorTranslator.FromHtml("#111827");
                }
            };

            lblSearchPlaceholder.Click += (s, e) => txtSearch.Focus();

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
            searchPanel.Controls.Add(searchContainer);

            var filtersPanel = new Panel { Dock = DockStyle.Top, Height = 80, Margin = new Padding(0, 0, 0, 12) };

            var rolePanel = new Panel { Width = 200, Height = 56, Location = new Point(0, 0) };
            rolePanel.Controls.Add(new Label { Text = "Role", AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#374151"), Location = new Point(0, 0) });

            cmbRoleFilter = new ComboBox { Width = 200, Height = 32, Top = 20, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbRoleFilter.Items.AddRange(new object[] { "All Roles", "Admin", "LoanOfficer", "Cashier" });
            cmbRoleFilter.SelectedIndex = 0;
            cmbRoleFilter.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbRoleFilter.SelectedItem?.ToString() ?? "All Roles";
                roleFilter = val == "All Roles" ? "all" : val;
                ApplyFilters();
            };
            rolePanel.Controls.Add(cmbRoleFilter);

            var statusPanel = new Panel { Width = 200, Height = 56, Location = new Point(220, 0) };
            statusPanel.Controls.Add(new Label { Text = "Status", AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#374151"), Location = new Point(0, 0) });

            cmbStatusFilter = new ComboBox { Width = 200, Height = 32, Top = 20, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbStatusFilter.Items.AddRange(new object[] { "All Statuses", "Active", "Inactive" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) =>
            {
                var val = cmbStatusFilter.SelectedItem?.ToString() ?? "All Statuses";
                statusFilter = val == "All Statuses" ? "all" : val;
                ApplyFilters();
            };
            statusPanel.Controls.Add(cmbStatusFilter);

            filtersPanel.Controls.Add(rolePanel);
            filtersPanel.Controls.Add(statusPanel);

            var buttonsPanel = new Panel { Dock = DockStyle.Top, Height = 40, Margin = new Padding(0, 0, 0, 8) };
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
            buttonsPanel.Layout += (s, e) =>
            {
                btnApplyFilters.Left = 0;
                btnClearFilters.Left = btnApplyFilters.Right + 8;
            };

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

            var titlePanel = new Panel { Dock = DockStyle.Top, Height = 24, Margin = new Padding(0, 0, 0, 12) };
            titlePanel.Controls.Add(new Label { Text = "👥", AutoSize = true, Font = new Font("Segoe UI", 9), Location = new Point(0, 4) });
            titlePanel.Controls.Add(new Label { Text = "USER LIST", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(20, 4) });

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

            dgvUsers.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EFF6FF");
            dgvUsers.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111827");

            dgvUsers.Columns.Add("Username", "Username");
            dgvUsers.Columns.Add("Role", "Role");
            dgvUsers.Columns.Add("Status", "Status");

            dgvUsers.SelectionChanged += (s, e) =>
            {
                if (dgvUsers.SelectedRows.Count <= 0) return;

                var selectedUsername = dgvUsers.SelectedRows[0].Cells["Username"].Value?.ToString();
                selectedUser = allUsers.FirstOrDefault(u => string.Equals(u.Username, selectedUsername, StringComparison.OrdinalIgnoreCase));
                UpdateUserDetails();
            };

            var paginationPanel = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 12, 0, 0) };
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
                Text = "0-0 of 0"
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

            paginationPanel.Layout += (s, e) =>
            {
                btnPrevious.Left = 0;
                lblPagination.Left = paginationPanel.Width / 2 - lblPagination.Width / 2;
                btnNext.Left = paginationPanel.Width - btnNext.Width;

                btnPrevious.Top = 8;
                lblPagination.Top = 12;
                btnNext.Top = 8;
            };

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

            var titlePanel = new Panel { Dock = DockStyle.Top, Height = 24, Margin = new Padding(0, 0, 0, 12) };
            titlePanel.Controls.Add(new Label { Text = "👤", AutoSize = true, Font = new Font("Segoe UI", 9), Location = new Point(0, 4) });
            titlePanel.Controls.Add(new Label { Text = "SELECTED USER DETAILS", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(20, 4) });

            pnlUserDetails = new Panel { Dock = DockStyle.Top, AutoSize = true };

            // Action buttons (left as UI only; needs real write ops later)
            var actionButtonsPanel = new Panel { Dock = DockStyle.Top, Height = 40, Margin = new Padding(0, 12, 0, 0) };

            var btnEdit = CreateOutlineButton("✏ Edit User");
            btnEdit.Click += (s, e) => CreateAndShowEditUserForm();

            var btnResetPassword = CreateOutlineButton("🔑 Reset Password");
            btnResetPassword.Click += (s, e) => ResetPassword();

            btnToggleActive = CreateOutlineButton("👤 Toggle Active");
            btnToggleActive.ForeColor = ColorTranslator.FromHtml("#EA580C");
            // Wire to toggle implementation
            btnToggleActive.Click += (s, e) => ToggleActiveSelectedUser();

            var btnDelete = CreateOutlineButton("🗑 Delete");
            btnDelete.ForeColor = ColorTranslator.FromHtml("#DC2626");
            // Implemented delete action (confirmation, DB delete, refresh UI)
            btnDelete.Click += (s, e) => DeleteSelectedUser();

            actionButtonsPanel.Controls.Add(btnEdit);
            actionButtonsPanel.Controls.Add(btnResetPassword);
            actionButtonsPanel.Controls.Add(btnToggleActive);
            actionButtonsPanel.Controls.Add(btnDelete);

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

        private void DeleteSelectedUser()
        {
            if (selectedUser == null)
            {
                ShowMessage("Please select a user to delete.", true);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete user '{selectedUser.Username}'?\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var entity = db.Users.SingleOrDefault(u => u.UserId == selectedUser.UserId);
                    if (entity == null)
                    {
                        ShowMessage("User not found in database. It may have been removed already.", true);
                        LoadUsersFromDb();
                        return;
                    }

                    db.Users.Remove(entity);
                    db.SaveChanges();
                }

                ShowMessage($"User '{selectedUser.Username}' has been deleted.");
                LoadUsersFromDb();
            }
            catch (Exception ex)
            {
                // Provide detailed error in a dialog (keeps behavior consistent with other DB error handling in the control)
                MessageBox.Show(ex.ToString(), "Failed to delete user", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleActiveSelectedUser()
        {
            if (selectedUser == null)
            {
                ShowMessage("Please select a user first.", true);
                return;
            }

            // Determine desired new status
            var currentlyActive = string.Equals(selectedUser.Status, "Active", StringComparison.OrdinalIgnoreCase);
            var action = currentlyActive ? "deactivate" : "activate";

            var confirm = MessageBox.Show(
                $"Are you sure you want to {action} user '{selectedUser.Username}'?",
                $"Confirm {action}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                bool newIsActive;
                using (var db = new AppDbContext())
                {
                    var entity = db.Users.SingleOrDefault(u => u.UserId == selectedUser.UserId);
                    if (entity == null)
                    {
                        ShowMessage("User not found in database. It may have been removed already.", true);
                        LoadUsersFromDb();
                        return;
                    }

                    // Toggle
                    entity.IsActive = !entity.IsActive;
                    newIsActive = entity.IsActive;

                    db.SaveChanges();
                }

                // Update backing store & UI
                var foundUser = allUsers.FirstOrDefault(x => x.UserId == selectedUser.UserId);
                if (foundUser != null)
                    foundUser.Status = newIsActive ? "Active" : "Inactive";

                selectedUser.Status = newIsActive ? "Active" : "Inactive";

                UpdateUserDetails();
                UpdateUserList();
                UpdateStatisticsCard();

                ShowMessage($"User '{selectedUser.Username}' has been {(newIsActive ? "activated" : "deactivated")}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to update user status", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateActionButtons()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 0) };

            var btnAddUser = CreateButton("➕ Add New User", "#16A34A", Color.White);
            btnAddUser.Click += (s, e) => ShowAddUserDialog();

            var btnRefresh = CreateOutlineButton("⟳ Refresh");
            btnRefresh.Click += (s, e) => LoadUsersFromDb();

            panel.Controls.Add(btnAddUser);
            panel.Controls.Add(btnRefresh);

            panel.Layout += (s, e) =>
            {
                btnAddUser.Left = 0;
                btnRefresh.Left = btnAddUser.Right + 8;
            };

            return panel;
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
                        LoadUsersFromDb();

                        if (!string.IsNullOrWhiteSpace(createdUsername))
                            SelectUserInGrid(createdUsername);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to add user", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectUserInGrid(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || dgvUsers == null) return;

            // Jump to correct page based on filtered list
            var filtered = GetFilteredUsers();
            var index = filtered.FindIndex(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                currentPage = index / itemsPerPage + 1;
                UpdateUserList();
            }

            for (int i = 0; i < dgvUsers.Rows.Count; i++)
            {
                var cellVal = dgvUsers.Rows[i].Cells["Username"].Value?.ToString();
                if (string.Equals(cellVal, username, StringComparison.OrdinalIgnoreCase))
                {
                    dgvUsers.ClearSelection();
                    dgvUsers.Rows[i].Selected = true;
                    dgvUsers.CurrentCell = dgvUsers.Rows[i].Cells[0];
                    UpdateUserDetails();
                    break;
                }
            }
        }

        private Panel CreateStatisticsCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = ColorTranslator.FromHtml("#EFF6FF"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 0, 16, 0)
            };

            header.Controls.Add(new Label
            {
                Text = "📊",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2563EB"),
                Location = new Point(16, 14)
            });

            header.Controls.Add(new Label
            {
                Text = "USER STATISTICS",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(50, 18)
            });

            var body = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(24, 16, 24, 16),
                Name = "statsBody"
            };

            card.Controls.Add(body);
            card.Controls.Add(header);
            return card;
        }

        private void UpdateStatisticsCard()
        {
            if (statisticsCard == null) return;

            var body = statisticsCard.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "statsBody");
            if (body == null) return;

            body.Controls.Clear();

            int totalUsers = allUsers.Count;
            int adminCount = allUsers.Count(u => string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase));
            int loanOfficerCount = allUsers.Count(u => string.Equals(u.Role, "LoanOfficer", StringComparison.OrdinalIgnoreCase));
            int cashierCount = allUsers.Count(u => string.Equals(u.Role, "Cashier", StringComparison.OrdinalIgnoreCase));
            int activeCount = allUsers.Count(u => string.Equals(u.Status, "Active", StringComparison.OrdinalIgnoreCase));
            int inactiveCount = allUsers.Count(u => string.Equals(u.Status, "Inactive", StringComparison.OrdinalIgnoreCase));

            var lblTotal = new Label
            {
                Text = $"Total Users: {totalUsers}",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(0, 0)
            };

            var lblRoles = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(16, 28),
                Text =
                    $"• Admin: {adminCount}\n" +
                    $"• LoanOfficer: {loanOfficerCount}\n" +
                    $"• Cashier: {cashierCount}"
            };

            var lblStatuses = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#374151"),
                Location = new Point(0, lblRoles.Bottom + 23),
                Text =
                    $"Active: {activeCount}\n" +
                    $"Inactive: {inactiveCount}"
            };

            body.Controls.Add(lblTotal);
            body.Controls.Add(lblRoles);
            body.Controls.Add(lblStatuses);
        }

        private List<UserRow> GetFilteredUsers()
        {
            return allUsers.Where(user =>
            {
                bool matchesSearch =
                    string.IsNullOrWhiteSpace(searchQuery) ||
                    (user.Username ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (user.FullName ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (user.Email ?? "").IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                bool matchesRole = roleFilter == "all" || string.Equals(user.Role, roleFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesStatus = statusFilter == "all" || string.Equals(user.Status, statusFilter, StringComparison.OrdinalIgnoreCase);

                return matchesSearch && matchesRole && matchesStatus;
            }).ToList();
        }

        private void ApplyFilters()
        {
            currentPage = 1;
            UpdateUserList();
            UpdateStatisticsCard();
        }

        private void UpdateUserList()
        {
            if (dgvUsers == null) return;

            dgvUsers.Rows.Clear();

            var filteredUsers = GetFilteredUsers();
            int totalPages = (int)Math.Ceiling(filteredUsers.Count / (double)itemsPerPage);

            int startIndex = (currentPage - 1) * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, filteredUsers.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var user = filteredUsers[i];
                dgvUsers.Rows.Add(user.Username, user.Role, user.Status);
            }

            if (lblPagination != null)
                lblPagination.Text = filteredUsers.Count == 0 ? "0-0 of 0" : $"{startIndex + 1}-{endIndex} of {filteredUsers.Count}";

            if (btnPrevious != null)
                btnPrevious.Enabled = currentPage > 1;

            if (btnNext != null)
                btnNext.Enabled = currentPage < totalPages;
        }

        private void UpdateUserDetails()
        {
            if (selectedUser == null || pnlUserDetails == null)
            {
                if (userDetailsCard != null) userDetailsCard.Visible = false;
                return;
            }

            userDetailsCard.Visible = true;
            pnlUserDetails.Controls.Clear();

            var detailsGrid = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0, 0, 0, 12)
            };
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

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

            AddDetailRow("User ID:", selectedUser.UserId.ToString(CultureInfo.InvariantCulture), 0);
            AddDetailRow("Username:", selectedUser.Username, 1);
            AddDetailRow("Full Name:", selectedUser.FullName, 2);
            AddDetailRow("Role:", selectedUser.Role, 3);
            AddDetailRow("Email:", selectedUser.Email, 4);
            AddDetailRow("Created:", selectedUser.CreatedDate, 5);
            AddDetailRow("Last Login:", selectedUser.LastLogin, 6);
            AddDetailRow("Status:", selectedUser.Status, 7);

            pnlUserDetails.Controls.Add(detailsGrid);

            // Update toggle button text/color to reflect current status
            if (btnToggleActive != null)
            {
                var isActive = string.Equals(selectedUser.Status, "Active", StringComparison.OrdinalIgnoreCase);
                btnToggleActive.Text = isActive ? "✗ Deactivate User" : "✓ Activate User";
                btnToggleActive.ForeColor = isActive ? ColorTranslator.FromHtml("#DC2626") : ColorTranslator.FromHtml("#16A34A");
            }
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
                    // ResetPasswordForm currently does NOT expose the password chosen/generated.
                    // So we implement the DB update here using a simple prompt AFTER DialogResult.OK.
                    // This keeps your existing ResetPasswordForm UI intact.
                    var result = form.ShowDialog(FindForm());
                    if (result != DialogResult.OK)
                        return;
                }

                // Minimal, safe follow-up prompt (does not change your existing UI forms).
                // If you want, we can later extend ResetPasswordForm to expose the new password directly.
                var newPassword = PromptForNewPassword(selectedUser.Username);
                if (string.IsNullOrWhiteSpace(newPassword))
                    return;

                UpdateUserPasswordInDb(selectedUser.Username, newPassword);

                ShowMessage("Password has been reset and saved to the database.");
            }
            catch (Exception ex)
            {
                ShowMessage("Error resetting password: " + ex.Message, true);
            }
        }

        private void CreateAndShowEditUserForm()
        {
            if (selectedUser == null)
            {
                ShowMessage("Please select a user to edit.", true);
                return;
            }

            // Map selected row -> EditUserForm.User (LendingSystem.UI.User)
            var editUser = new EditFormUser
            {
                Username = selectedUser.Username,
                FullName = selectedUser.FullName,
                Email = selectedUser.Email,
                Phone = "", // Not stored in users table currently
                Role = MapRoleToEditFormDisplay(selectedUser.Role),
                EmployeeId = selectedUser.UserId.ToString(CultureInfo.InvariantCulture),
                Status = selectedUser.Status,
                CreatedDate = selectedUser.CreatedDate,
                LastLogin = selectedUser.LastLogin
            };

            using (var form = new EditUserForm(editUser))
            {
                form.OnUserUpdated += updated =>
                {
                    try
                    {
                        SaveEditedUserToDb(updated);
                        LoadUsersFromDb();
                        SelectUserInGrid(updated.Username);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Failed to update user", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                form.ShowDialog(FindForm());
            }
        }

        private static string MapRoleToEditFormDisplay(string role)
        {
            if (string.Equals(role, "LoanOfficer", StringComparison.OrdinalIgnoreCase))
                return "Loan Officer";

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

        private void SaveEditedUserToDb(EditFormUser updated)
        {
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            if (string.IsNullOrWhiteSpace(updated.Username)) throw new InvalidOperationException("Username is required.");

            var newRoleDbValue = MapRoleToDbValue(updated.Role);
            var isActive = string.Equals(updated.Status, "Active", StringComparison.OrdinalIgnoreCase);

            // Split full name (best-effort)
            var full = (updated.FullName ?? "").Trim();
            var first = full;
            var last = "";
            var idx = full.LastIndexOf(' ');
            if (idx > 0)
            {
                first = full.Substring(0, idx).Trim();
                last = full.Substring(idx + 1).Trim();
            }

            using (var db = new AppDbContext())
            {
                var entity = db.Users.Single(u => u.Username == updated.Username);

                entity.Email = updated.Email ?? "";
                entity.FirstName = first;
                entity.LastName = last;
                entity.Role = newRoleDbValue;
                entity.IsActive = isActive;

                db.SaveChanges();
            }
        }

        private static string MapRoleToDbValue(string roleDisplayOrDb)
        {
            // EditUserForm uses "Loan Officer" but DB uses "LoanOfficer"
            if (string.Equals(roleDisplayOrDb, "Loan Officer", StringComparison.OrdinalIgnoreCase))
                return "LoanOfficer";

            // If already stored without space, keep it
            if (string.Equals(roleDisplayOrDb, "LoanOfficer", StringComparison.OrdinalIgnoreCase))
                return "LoanOfficer";

            return roleDisplayOrDb ?? "";
        }

        private static string PromptForNewPassword(string username)
        {
            using (var f = new Form())
            {
                f.Text = "Set New Password";
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;
                f.ClientSize = new Size(420, 170);

                var lbl = new Label
                {
                    Text = "Enter new password for: " + username,
                    AutoSize = true,
                    Location = new Point(12, 12)
                };

                var txt1 = new TextBox
                {
                    UseSystemPasswordChar = true,
                    Width = 380,
                    Location = new Point(12, 40)
                };

                var txt2 = new TextBox
                {
                    UseSystemPasswordChar = true,
                    Width = 380,
                    Location = new Point(12, 70)
                };

                var hint = new Label
                {
                    Text = "Confirm password (min 6 chars).",
                    AutoSize = true,
                    ForeColor = Color.Gray,
                    Location = new Point(12, 100)
                };

                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(236, 125), Width = 75 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(317, 125), Width = 75 };

                f.Controls.Add(lbl);
                f.Controls.Add(txt1);
                f.Controls.Add(txt2);
                f.Controls.Add(hint);
                f.Controls.Add(btnOk);
                f.Controls.Add(btnCancel);
                f.AcceptButton = btnOk;
                f.CancelButton = btnCancel;

                if (f.ShowDialog() != DialogResult.OK)
                    return null;

                var p1 = txt1.Text ?? "";
                var p2 = txt2.Text ?? "";

                if (p1.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                if (!string.Equals(p1, p2, StringComparison.Ordinal))
                {
                    MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                return p1;
            }
        }

        private void UpdateUserPasswordInDb(string username, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("username is required", nameof(username));
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("newPassword is required", nameof(newPassword));

            // NOTE: This project currently stores raw password in password_hash (see UserRegisterLogic).
            // This keeps behavior consistent with existing code. If you later hash, change here too.
            const string sql = @"UPDATE users SET password_hash = @password_hash WHERE username = @username;";

            using (var db = new AppDbContext())
            {
                var rows = db.Database.ExecuteSqlCommand(
                    sql,
                    new MySqlParameter("@password_hash", newPassword),
                    new MySqlParameter("@username", username));

                if (rows <= 0)
                    throw new InvalidOperationException("No user was updated. Username may not exist.");
            }
        }
    }
}