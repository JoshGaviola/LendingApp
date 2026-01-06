using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;

namespace LendingApp.UI.CashierUI
{
    public partial class CashierCollections : Form
    {
        private sealed class CollectionRow
        {
            public int CollectionId { get; set; }
            public int LoanId { get; set; }
            public string LoanNumber { get; set; }
            public string Customer { get; set; }
            public DateTime DueDate { get; set; }
            public decimal AmountDue { get; set; }
            public int DaysOverdue { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
        }

        private readonly bool _hosted;

        private Panel root;
        private Panel headerPanel;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;

        private Panel summaryPanel;
        private Panel cardDueToday;
        private Panel cardOverdue;
        private Panel cardThisWeek;
        private Panel cardCollectedToday;

        private Panel filtersPanel;
        private ComboBox cmbStatusFilter;
        private TextBox txtSearch;

        private Panel tableContainer;
        private Panel tableHeader;
        private Label lblTableTitle;
        private Label lblItemsCount;
        private Label lblShowingFilter;
        private DataGridView gridCollections;

        private Panel tableFooter;
        private Label lblTotalItems;
        private Label lblTotalAmount;

        private string selectedFilter = "All";
        private string searchTerm = "";
        private readonly string[] filterOptions = { "All", "Overdue", "Due Today", "Upcoming", "Collected" };

        // callback fired when user clicks Collect
        public event Action<string> CollectLoanRequested;

        public CashierCollections() : this(true) { }

        public CashierCollections(bool hosted)
        {
            _hosted = hosted;
            InitializeComponent();
            BuildUI();
            BindEvents();
            RefreshTable();
        }

        private void BuildUI()
        {
            Controls.Clear();

            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;

            root = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true,
                Padding = new Padding(16)
            };
            Controls.Add(root);

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblHeaderTitle = new Label
            {
                Text = "COLLECTIONS (DUE & OVERDUE)",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Location = new Point(16, 10)
            };

            lblHeaderSubtitle = new Label
            {
                Text = "Cashier collection queue (Due Today / Overdue)",
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                Location = new Point(16, 34)
            };

            headerPanel.Controls.Add(lblHeaderTitle);
            headerPanel.Controls.Add(lblHeaderSubtitle);

            // Summary cards
            summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 0)
            };

            cardDueToday = MakeSummaryCard("#FFEDD5", "#EA580C", "#F97316", "Due Today", "₱0.00", "");
            cardOverdue = MakeSummaryCard("#FEE2E2", "#DC2626", "#EF4444", "Overdue", "₱0.00", "");
            cardThisWeek = MakeSummaryCard("#DBEAFE", "#1D4ED8", "#2563EB", "This Week", "₱0.00", "");
            cardCollectedToday = MakeSummaryCard("#ECFDF5", "#065F46", "#10B981", "Collected Today", "₱0.00", "");

            summaryPanel.Controls.Add(cardDueToday);
            summaryPanel.Controls.Add(cardOverdue);
            summaryPanel.Controls.Add(cardThisWeek);
            summaryPanel.Controls.Add(cardCollectedToday);
            summaryPanel.Resize += (s, e) => LayoutSummaryCards();

            // Filters
            filtersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(16),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblStatus = new Label
            {
                Text = "Status Filter",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#4B5563"),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            cmbStatusFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(10, 30),
                Width = 220
            };
            cmbStatusFilter.Items.AddRange(filterOptions);

            var lblSearch = new Label
            {
                Text = "Search",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#4B5563"),
                AutoSize = true,
                Location = new Point(250, 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(250, 30),
                Width = 300
            };

            filtersPanel.Controls.Add(lblStatus);
            filtersPanel.Controls.Add(cmbStatusFilter);
            filtersPanel.Controls.Add(lblSearch);
            filtersPanel.Controls.Add(txtSearch);

            // Table
            tableContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 480,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 0)
            };

            tableHeader = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTableTitle = new Label
            {
                Text = "COLLECTIONS LIST",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                AutoSize = true,
                Location = new Point(16, 16)
            };

            lblItemsCount = new Label
            {
                Text = "0 items",
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#1D4ED8"),
                AutoSize = true,
                Location = new Point(170, 18),
                BackColor = ColorTranslator.FromHtml("#DBEAFE"),
                Padding = new Padding(6, 2, 6, 2)
            };

            lblShowingFilter = new Label
            {
                Text = "Showing: All",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#6B7280"),
                AutoSize = true
            };
            tableHeader.Resize += (s, e) => lblShowingFilter.Location = new Point(tableHeader.Width - lblShowingFilter.Width - 16, 16);

            gridCollections = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 }
            };

            gridCollections.Columns.Add("LoanNumber", "Loan #");
            gridCollections.Columns.Add("Customer", "Customer");
            gridCollections.Columns.Add("DueDate", "Due Date");
            gridCollections.Columns.Add("Amount", "Amount Due");
            gridCollections.Columns.Add("Days", "Days");
            gridCollections.Columns.Add("Priority", "Priority");
            gridCollections.Columns.Add("Status", "Status");

            var actionsCol = new DataGridViewButtonColumn
            {
                Name = "Actions",
                HeaderText = "Actions",
                UseColumnTextForButtonValue = false
            };
            gridCollections.Columns.Add(actionsCol);

            gridCollections.CellContentClick += GridCollections_CellContentClick;

            var gridHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            gridHost.Controls.Add(gridCollections);

            tableFooter = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                BackColor = ColorTranslator.FromHtml("#F9FAFB"),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 8, 12, 8)
            };

            lblTotalItems = new Label
            {
                Text = "Total: 0 collections",
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#4B5563"),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            lblTotalAmount = new Label
            {
                Text = "Total Amount: ₱0.00",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                AutoSize = true
            };
            tableFooter.Resize += (s, e) => lblTotalAmount.Location = new Point(tableFooter.Width - lblTotalAmount.Width - 12, 10);

            tableHeader.Controls.Add(lblTableTitle);
            tableHeader.Controls.Add(lblItemsCount);
            tableHeader.Controls.Add(lblShowingFilter);

            tableContainer.Controls.Add(gridHost);
            tableContainer.Controls.Add(tableFooter);
            tableContainer.Controls.Add(tableHeader);

            // Compose root
            root.Controls.Add(tableContainer);
            root.Controls.Add(filtersPanel);
            root.Controls.Add(summaryPanel);
            root.Controls.Add(headerPanel);

            // Initial filter
            cmbStatusFilter.SelectedIndex = 0;
            LayoutSummaryCards();
        }

        private void BindEvents()
        {
            cmbStatusFilter.SelectedIndexChanged += (s, e) =>
            {
                selectedFilter = cmbStatusFilter.SelectedItem != null ? cmbStatusFilter.SelectedItem.ToString() : "All";
                lblShowingFilter.Text = "Showing: " + selectedFilter;
                RefreshTable();
            };

            txtSearch.TextChanged += (s, e) =>
            {
                searchTerm = txtSearch.Text ?? "";
                RefreshTable();
            };
        }

        private void RefreshTable()
        {
            List<CollectionRow> rows;
            try
            {
                rows = LoadFromDb();
            }
            catch (Exception ex)
            {
                rows = new List<CollectionRow>();
                MessageBox.Show("Failed to load collections: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Filter
            var filtered = rows.Where(r =>
            {
                bool matchesFilter = selectedFilter == "All" || string.Equals(r.Status, selectedFilter, StringComparison.OrdinalIgnoreCase);
                bool matchesSearch = string.IsNullOrWhiteSpace(searchTerm)
                    || (r.Customer ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                    || (r.LoanNumber ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
                return matchesFilter && matchesSearch;
            }).ToList();

            gridCollections.Rows.Clear();

            foreach (var r in filtered)
            {
                int idx = gridCollections.Rows.Add(
                    r.LoanNumber,
                    r.Customer,
                    r.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    "₱" + r.AmountDue.ToString("N2", CultureInfo.InvariantCulture),
                    DaysCellText(r.DaysOverdue),
                    r.Priority,
                    r.Status,
                    ""
                );

                var row = gridCollections.Rows[idx];
                row.Tag = r;

                // Days overdue styling
                var daysCell = row.Cells["Days"];
                if (daysCell != null)
                {
                    bool overdue = r.DaysOverdue > 0;
                    daysCell.Style.BackColor = overdue ? ColorTranslator.FromHtml("#FEE2E2") : Color.White;
                    daysCell.Style.ForeColor = overdue ? ColorTranslator.FromHtml("#B91C1C") : ColorTranslator.FromHtml("#6B7280");
                }

                // Priority styling
                var prCell = row.Cells["Priority"];
                if (prCell != null)
                {
                    var colors = GetPriorityColors(r.Priority);
                    prCell.Style.BackColor = colors.back;
                    prCell.Style.ForeColor = colors.fore;
                }

                // Status styling (light)
                var stCell = row.Cells["Status"];
                if (stCell != null)
                {
                    if (r.Status == "Overdue")
                    {
                        stCell.Style.BackColor = ColorTranslator.FromHtml("#FEE2E2");
                        stCell.Style.ForeColor = ColorTranslator.FromHtml("#991B1B");
                    }
                    else if (r.Status == "Due Today")
                    {
                        stCell.Style.BackColor = ColorTranslator.FromHtml("#FFEDD5");
                        stCell.Style.ForeColor = ColorTranslator.FromHtml("#9A3412");
                    }
                    else if (r.Status == "Collected")
                    {
                        stCell.Style.BackColor = ColorTranslator.FromHtml("#DCFCE7");
                        stCell.Style.ForeColor = ColorTranslator.FromHtml("#166534");
                    }
                }

                // Action button
                var actionCell = row.Cells["Actions"] as DataGridViewButtonCell;
                if (actionCell != null)
                {
                    actionCell.Value = GetActionText(r.Status);
                }
            }

            lblItemsCount.Text = filtered.Count.ToString(CultureInfo.InvariantCulture) + " items";
            lblTotalItems.Text = "Total: " + filtered.Count.ToString(CultureInfo.InvariantCulture) + " collections";
            lblTotalAmount.Text = "Total Amount: ₱" + filtered.Sum(x => x.AmountDue).ToString("N2", CultureInfo.InvariantCulture);

            // Update summary cards
            UpdateSummary(rows);
        }

        private void GridCollections_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (gridCollections.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var data = gridCollections.Rows[e.RowIndex].Tag as CollectionRow;
                if (data == null) return;

                var action = GetActionText(data.Status);
                if (action == "Collect")
                {
                    if (!string.IsNullOrWhiteSpace(data.LoanNumber))
                        CollectLoanRequested?.Invoke(data.LoanNumber);
                }
                else
                {
                    // If status is Collected, show the receipt for the latest payment for that loan.
                    try
                    {
                        using (var db = new AppDbContext())
                        {
                            var payment = db.Payments.AsNoTracking()
                                            .Where(p => p.LoanId == data.LoanId)
                                            .OrderByDescending(p => p.PaymentDate)
                                            .FirstOrDefault();

                            if (payment == null || string.IsNullOrWhiteSpace(payment.ReceiptNo))
                            {
                                MessageBox.Show("No receipt found for that loan.", "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // Create receipt viewer as a true top-level modal dialog.
                            using (var receiptForm = new CashierReciept())
                            {
                                // The CashierReciept class is normally used as an embedded control (TopLevel = false).
                                // For modal viewing we must make it a top-level form before calling ShowDialog.
                                receiptForm.TopLevel = true;
                                receiptForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                                receiptForm.StartPosition = FormStartPosition.CenterParent;
                                receiptForm.LoadAndSelectReceipt(payment.ReceiptNo);

                                // Show modal with this form as owner
                                var owner = FindForm();
                                if (owner != null) receiptForm.ShowDialog(owner);
                                else receiptForm.ShowDialog();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to open receipt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private List<CollectionRow> LoadFromDb()
        {
            var today = DateTime.Today;

            using (var db = new AppDbContext())
            {
                // Collections + loan # + customer name
                var q = from c in db.Collections.AsNoTracking()
                        join l in db.Loans.AsNoTracking() on c.LoanId equals l.LoanId
                        join cust in db.Customers.AsNoTracking() on c.CustomerId equals cust.CustomerId into custJoin
                        from cust in custJoin.DefaultIfEmpty()
                        select new
                        {
                            c.CollectionId,
                            c.LoanId,
                            l.LoanNumber,
                            CustomerName = ((cust.FirstName ?? "") + " " + (cust.LastName ?? "")).Trim(),
                            c.DueDate,
                            c.AmountDue,
                            c.DaysOverdue,
                            c.Priority,
                            c.Status,
                            c.UpdatedDate
                        };

                var raw = q.ToList();

                // Normalize cashier statuses for UI:
                // - Paid => Collected
                // - Otherwise use DueDate to infer Due Today/Overdue/Upcoming
                var rows = raw.Select(x =>
                {
                    string uiStatus;
                    if (string.Equals(x.Status, "Paid", StringComparison.OrdinalIgnoreCase))
                    {
                        uiStatus = "Collected";
                    }
                    else
                    {
                        if (x.DueDate.Date < today) uiStatus = "Overdue";
                        else if (x.DueDate.Date == today) uiStatus = "Due Today";
                        else uiStatus = "Upcoming";
                    }

                    // fallback name
                    var name = string.IsNullOrWhiteSpace(x.CustomerName) ? x.LoanNumber : x.CustomerName;

                    return new CollectionRow
                    {
                        CollectionId = x.CollectionId,
                        LoanId = x.LoanId,
                        LoanNumber = x.LoanNumber,
                        Customer = name,
                        DueDate = x.DueDate.Date,
                        AmountDue = x.AmountDue,
                        DaysOverdue = x.DaysOverdue,
                        Priority = string.IsNullOrWhiteSpace(x.Priority) ? "Medium" : x.Priority,
                        Status = uiStatus
                    };
                }).ToList();

                return rows;
            }
        }

        private void UpdateSummary(List<CollectionRow> allRows)
        {
            var today = DateTime.Today;
            var dueToday = allRows.Where(r => r.DueDate.Date == today && r.Status != "Collected").ToList();
            var overdue = allRows.Where(r => r.DueDate.Date < today && r.Status != "Collected").ToList();
            var weekEnd = today.AddDays(7);

            var thisWeek = allRows.Where(r => r.DueDate.Date >= today && r.DueDate.Date <= weekEnd && r.Status != "Collected").ToList();

            // “Collected Today”: use collected status and DueDate == today is not correct.
            // Use the collections table `UpdatedDate` ideally, but CollectionRow doesn't have it.
            // Keep it conservative: collected with DueDate == today.
            var collectedToday = allRows.Where(r => r.Status == "Collected" && r.DueDate.Date == today).ToList();

            SetCardValue(cardDueToday, "₱" + dueToday.Sum(x => x.AmountDue).ToString("N2", CultureInfo.InvariantCulture), dueToday.Count + " payments");
            SetCardValue(cardOverdue, "₱" + overdue.Sum(x => x.AmountDue).ToString("N2", CultureInfo.InvariantCulture), overdue.Count + " accounts");
            SetCardValue(cardThisWeek, "₱" + thisWeek.Sum(x => x.AmountDue).ToString("N2", CultureInfo.InvariantCulture), "Expected");
            SetCardValue(cardCollectedToday, "₱" + collectedToday.Sum(x => x.AmountDue).ToString("N2", CultureInfo.InvariantCulture), collectedToday.Count + " payments");
        }

        private static void SetCardValue(Panel card, string value, string sub)
        {
            var valueLabel = card.Tag as Tuple<Label, Label>;
            if (valueLabel == null) return;
            valueLabel.Item1.Text = value;
            valueLabel.Item2.Text = sub;
        }

        private string GetActionText(string status)
        {
            if (status == "Overdue" || status == "Due Today") return "Collect";
            return "View";
        }

        private static string DaysCellText(int days)
        {
            return days > 0 ? "+" + days.ToString(CultureInfo.InvariantCulture) : "0";
        }

        private static (Color back, Color fore) GetPriorityColors(string priority)
        {
            switch (priority)
            {
                case "Critical": return (ColorTranslator.FromHtml("#FEE2E2"), ColorTranslator.FromHtml("#B91C1C"));
                case "High": return (ColorTranslator.FromHtml("#FFEDD5"), ColorTranslator.FromHtml("#9A3412"));
                case "Medium": return (ColorTranslator.FromHtml("#FEF3C7"), ColorTranslator.FromHtml("#92400E"));
                case "Low": return (ColorTranslator.FromHtml("#D1FAE5"), ColorTranslator.FromHtml("#065F46"));
                default: return (ColorTranslator.FromHtml("#F3F4F6"), ColorTranslator.FromHtml("#374151"));
            }
        }

        private Panel MakeSummaryCard(string backHex, string titleHex, string accentHex, string title, string value, string sub)
        {
            var card = new Panel { Width = 250, Height = 80, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var inner = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            card.Controls.Add(inner);

            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 9), ForeColor = ColorTranslator.FromHtml("#6B7280"), AutoSize = true, Location = new Point(10, 10) };
            var lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml(titleHex), AutoSize = true, Location = new Point(10, 30) };
            var lblSub = new Label { Text = sub, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml(accentHex), AutoSize = true, Location = new Point(10, 52) };

            var iconBox = new Panel { Width = 40, Height = 40, BackColor = ColorTranslator.FromHtml(backHex), Location = new Point(card.Width - 60, 20) };
            iconBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            inner.Controls.Add(lblTitle);
            inner.Controls.Add(lblValue);
            inner.Controls.Add(lblSub);
            inner.Controls.Add(iconBox);

            // Store value/sub labels for updating later
            card.Tag = Tuple.Create(lblValue, lblSub);

            return card;
        }

        private void LayoutSummaryCards()
        {
            int gap = 10;
            int startX = 0;
            int y = 12;

            cardDueToday.Location = new Point(startX, y);
            cardOverdue.Location = new Point(cardDueToday.Right + gap, y);
            cardThisWeek.Location = new Point(cardOverdue.Right + gap, y);
            cardCollectedToday.Location = new Point(cardThisWeek.Right + gap, y);
        }
    }
}