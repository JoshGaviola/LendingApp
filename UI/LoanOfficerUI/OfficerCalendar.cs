using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerCalendar : Form
    {
        private DateTime _currentDate = DateTime.Today;
        private DateTime _selectedDate = DateTime.Today;

        private class CalendarTask
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; }         // "HH:mm" or "h:mm tt"
            public string Customer { get; set; }
            public string TaskType { get; set; }     // Payment Follow-up | Document Review | Credit Assessment | Site Visit | Internal | Customer Meeting
            public string LoanRef { get; set; }      // LN-xxx or APP-xxx or "-"
            public string Description { get; set; }
            public bool Completed { get; set; }
        }

        private readonly List<CalendarTask> _tasks = new List<CalendarTask>
        {
            new CalendarTask { Id="1", Date=DateTime.Today, Time="09:00 AM", Customer="Juan Cruz",   TaskType="Payment Follow-up", LoanRef="LN-001",  Description="Follow up on overdue payment", Completed=false },
            new CalendarTask { Id="2", Date=DateTime.Today, Time="02:00 PM", Customer="Maria Santos",TaskType="Document Review",   LoanRef="APP-002", Description="Verify submitted documents", Completed=false },
            new CalendarTask { Id="3", Date=DateTime.Today, Time="04:00 PM", Customer="Team Meeting",TaskType="Internal",          LoanRef="-",       Description="Weekly team sync meeting", Completed=false },
        };

        // Controls
        private Panel headerPanel;
        private Label lblTitle;
        private Button btnToday;
        private Button btnNewTask;

        private Panel calendarPanel;
        private Button btnPrevMonth;
        private Button btnNextMonth;
        private Label lblMonthYear;
        private TableLayoutPanel dayNamesHeader;
        private TableLayoutPanel calendarGrid;

        private Panel selectedInfoPanel;
        private Label lblSelectedDate;
        private Label lblSelectedCount;

        private Panel tasksPanel;
        private DataGridView gridTasks;

        // New task dialog controls
        private Form newTaskDialog;
        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private TextBox txtCustomer;
        private ComboBox cmbTaskType;
        private TextBox txtLoanRef;
        private TextBox txtDescription;
        private Button btnAddTask;
        private Button btnCancelTask;

        public OfficerCalendar()
        {
            InitializeComponent();
            BuildUI();
            RefreshCalendarHeader();
            BuildCalendarGrid();
            UpdateSelectedInfo();
            RefreshTasksTable();
        }

        private void BuildUI()
        {
            Text = "Officer Calendar";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F7F9FC");
            WindowState = FormWindowState.Maximized;

            // Header
            headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(16) };
            lblTitle = new Label { Text = "CALENDAR & TASKS", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#2C3E50"), AutoSize = true, Location = new Point(16, 18) };
            btnToday = new Button { Text = "Today", Width = 80, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNewTask = new Button { Text = "New Task", Width = 100, Height = 28, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnToday.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnNewTask.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#1D4ED8");
            btnToday.Click += (s, e) => { GoToToday(); };
            btnNewTask.Click += (s, e) => { OpenNewTaskDialog(); };
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnToday);
            headerPanel.Controls.Add(btnNewTask);
            headerPanel.Resize += (s, e) =>
            {
                btnNewTask.Location = new Point(headerPanel.Width - btnNewTask.Width - 16, 16);
                btnToday.Location = new Point(btnNewTask.Left - btnToday.Width - 8, 16);
            };

            // Calendar panel
            calendarPanel = new Panel { Dock = DockStyle.Top, Height = 360, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(16) };
            btnPrevMonth = new Button { Text = "<", Width = 32, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNextMonth = new Button { Text = ">", Width = 32, Height = 28, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            lblMonthYear = new Label { Text = "", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), AutoSize = true };
            btnPrevMonth.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnNextMonth.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D1D5DB");
            btnPrevMonth.Click += (s, e) => { PreviousMonth(); };
            btnNextMonth.Click += (s, e) => { NextMonth(); };

            calendarPanel.Controls.Add(btnPrevMonth);
            calendarPanel.Controls.Add(btnNextMonth);
            calendarPanel.Controls.Add(lblMonthYear);

            // Position header elements
            lblMonthYear.Location = new Point((calendarPanel.Width / 2) - (lblMonthYear.Width / 2), 16);
            calendarPanel.Resize += (s, e) =>
            {
                lblMonthYear.Location = new Point((calendarPanel.Width / 2) - (lblMonthYear.Width / 2), 16);
                btnPrevMonth.Location = new Point(16, 12);
                btnNextMonth.Location = new Point(calendarPanel.Width - btnNextMonth.Width - 16, 12);
            };

            // Day names header
            dayNamesHeader = new TableLayoutPanel { Dock = DockStyle.Top, Height = 30, ColumnCount = 7 };
            for (int i = 0; i < 7; i++) dayNamesHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.285f));
            string[] dayNames = { "S", "M", "T", "W", "T", "F", "S" };
            for (int i = 0; i < dayNames.Length; i++)
            {
                var lbl = new Label
                {
                    Text = dayNames[i],
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    ForeColor = ColorTranslator.FromHtml("#6B7280"),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular)
                };
                dayNamesHeader.Controls.Add(lbl, i, 0);
            }

            // Calendar grid (6 rows x 7 columns to cover all months)
            calendarGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 7, RowCount = 6, CellBorderStyle = TableLayoutPanelCellBorderStyle.Single };
            for (int i = 0; i < 7; i++) calendarGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.285f));
            for (int r = 0; r < 6; r++) calendarGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66f));

            var gridHost = new Panel { Dock = DockStyle.Fill };
            gridHost.Controls.Add(calendarGrid);

            calendarPanel.Controls.Add(gridHost);
            calendarPanel.Controls.Add(dayNamesHeader);

            // Selected info
            selectedInfoPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ColorTranslator.FromHtml("#EFF6FF"), BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(12) };
            lblSelectedDate = new Label { Text = "", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#1D4ED8"), Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblSelectedCount = new Label { Text = "", AutoSize = true, ForeColor = ColorTranslator.FromHtml("#2563EB"), Font = new Font("Segoe UI", 8, FontStyle.Bold) };
            selectedInfoPanel.Controls.Add(lblSelectedDate);
            selectedInfoPanel.Controls.Add(lblSelectedCount);
            selectedInfoPanel.Resize += (s, e) =>
            {
                lblSelectedDate.Location = new Point(12, 14);
                lblSelectedCount.Location = new Point(selectedInfoPanel.Width - lblSelectedCount.Width - 12, 14);
            };

            // Tasks table
            tasksPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(0) };
            var tasksHeader = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = ColorTranslator.FromHtml("#F9FAFB"), BorderStyle = BorderStyle.FixedSingle };
            var lblTasksTitle = new Label { Text = "TASKS FOR SELECTED DATE", AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#111827"), Location = new Point(12, 11) };
            var lblTasksSummary = new Label { AutoSize = true, Font = new Font("Segoe UI", 8), ForeColor = ColorTranslator.FromHtml("#6B7280") };
            tasksHeader.Controls.Add(lblTasksTitle);
            tasksHeader.Controls.Add(lblTasksSummary);
            tasksHeader.Resize += (s, e) =>
            {
                lblTasksSummary.Location = new Point(tasksHeader.Width - lblTasksSummary.Width - 12, 12);
            };

            gridTasks = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
            gridTasks.Columns.Clear();
            gridTasks.Columns.Add("Time", "Time");
            gridTasks.Columns.Add("Customer", "Customer");
            gridTasks.Columns.Add("TaskType", "Task Type");
            gridTasks.Columns.Add("LoanRef", "Loan Ref");
            gridTasks.Columns.Add("Description", "Description");
            var doneCol = new DataGridViewCheckBoxColumn { HeaderText = "Done", Name = "Done", ThreeState = false };
            gridTasks.Columns.Add(doneCol);
            gridTasks.CellValueChanged += GridTasks_CellValueChanged;
            gridTasks.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (gridTasks.IsCurrentCellDirty) gridTasks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            tasksPanel.Controls.Add(gridTasks);
            tasksPanel.Controls.Add(tasksHeader);

            // Compose
            Controls.Add(tasksPanel);
            Controls.Add(selectedInfoPanel);
            Controls.Add(calendarPanel);
            Controls.Add(headerPanel);

            // Helper to update task summary on refresh
            void UpdateTasksSummary()
            {
                var dayTasks = GetTasksForDate(_selectedDate);
                lblTasksSummary.Text = $"{dayTasks.Count(t => t.Completed)} of {dayTasks.Count} completed";
                tasksHeader.PerformLayout();
            }
            // Hook into refreshes
            gridTasks.DataSourceChanged += (s, e) => UpdateTasksSummary();
        }

        private void RefreshCalendarHeader()
        {
            lblMonthYear.Text = $"{_currentDate.ToString("MMMM").ToUpper()} {_currentDate.Year}";
            lblMonthYear.Location = new Point((calendarPanel.Width / 2) - (lblMonthYear.Width / 2), 16);
        }

        private void BuildCalendarGrid()
        {
            calendarGrid.SuspendLayout();
            calendarGrid.Controls.Clear();

            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            int firstDayOfWeek = (int)(new DateTime(_currentDate.Year, _currentDate.Month, 1).DayOfWeek); // 0=Sunday
            int cellIndex = 0;

            // Preceding blanks
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                var blank = MakeDayCell(null);
                calendarGrid.Controls.Add(blank, cellIndex % 7, cellIndex / 7);
                cellIndex++;
            }

            // Days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(_currentDate.Year, _currentDate.Month, day);
                var cell = MakeDayCell(date);
                calendarGrid.Controls.Add(cell, cellIndex % 7, cellIndex / 7);
                cellIndex++;
            }

            // Fill remaining blanks to complete 6 rows
            while (cellIndex < 42)
            {
                var blank = MakeDayCell(null);
                calendarGrid.Controls.Add(blank, cellIndex % 7, cellIndex / 7);
                cellIndex++;
            }

            calendarGrid.ResumeLayout();
        }

        private Panel MakeDayCell(DateTime? date)
        {
            var cell = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            cell.BorderStyle = BorderStyle.FixedSingle;

            if (!date.HasValue)
                return cell;

            var isToday = date.Value.Date == DateTime.Today;
            var isSelected = date.Value.Date == _selectedDate.Date;

            if (isToday) cell.BackColor = ColorTranslator.FromHtml("#DBEAFE");     // blue-100
            if (isSelected) cell.BackColor = ColorTranslator.FromHtml("#BFDBFE");  // blue-200

            var btn = new Button
            {
                Text = date.Value.Day.ToString(),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                _selectedDate = date.Value.Date;
                BuildCalendarGrid();
                UpdateSelectedInfo();
                RefreshTasksTable();
            };

            // dot indicator if tasks exist
            var hasTasks = GetTasksForDate(date.Value).Any();
            if (hasTasks)
            {
                var dot = new Panel
                {
                    Width = 6,
                    Height = 6,
                    BackColor = ColorTranslator.FromHtml("#2563EB"),
                    Location = new Point((cell.Width / 2) - 3, cell.Height - 10),
                    Anchor = AnchorStyles.Bottom
                };
                dot.BorderStyle = BorderStyle.None;
                cell.Controls.Add(dot);
                cell.Resize += (s, e) => { dot.Left = (cell.Width / 2) - 3; dot.Top = cell.Height - 12; };
            }

            cell.Controls.Add(btn);
            return cell;
        }

        private void UpdateSelectedInfo()
        {
            var dayTasks = GetTasksForDate(_selectedDate);
            lblSelectedDate.Text = $"Selected Date: {_selectedDate.ToString("D")}";
            lblSelectedCount.Text = dayTasks.Count > 0 ? $"{dayTasks.Count} task(s)" : "";
            lblSelectedDate.Location = new Point(12, 14);
            lblSelectedCount.Location = new Point(selectedInfoPanel.Width - lblSelectedCount.Width - 12, 14);
        }

        private List<CalendarTask> GetTasksForDate(DateTime date)
        {
            return _tasks.Where(t => t.Date.Date == date.Date).OrderBy(t => t.Time).ToList();
        }

        private void RefreshTasksTable()
        {
            var dayTasks = GetTasksForDate(_selectedDate);
            gridTasks.Rows.Clear();

            if (dayTasks.Count == 0)
            {
                // Show empty state row-like experience
                gridTasks.Rows.Add("—", "No tasks scheduled", "", "", "", false);
                var row = gridTasks.Rows[gridTasks.Rows.Count - 1];
                row.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#6B7280");
                return;
            }

            foreach (var t in dayTasks)
            {
                int idx = gridTasks.Rows.Add(t.Time, t.Customer, t.TaskType, t.LoanRef, t.Description, t.Completed);
                var row = gridTasks.Rows[idx];

                // Style for completed
                if (t.Completed)
                {
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ECFDF5"); // green-50
                    row.Cells["Customer"].Style.ForeColor = ColorTranslator.FromHtml("#6B7280");
                    row.Cells["Customer"].Style.Font = new Font("Segoe UI", 9, FontStyle.Strikeout);
                }

                // TaskType pill-like color
                var typeCell = row.Cells["TaskType"];
                var colors = GetTaskTypeColors(t.TaskType);
                typeCell.Style.BackColor = colors.back;
                typeCell.Style.ForeColor = colors.fore;
            }
        }

        private (Color back, Color fore) GetTaskTypeColors(string type)
        {
            switch (type)
            {
                case "Payment Follow-up": return (ColorTranslator.FromHtml("#FEE2E2"), ColorTranslator.FromHtml("#B91C1C")); // red
                case "Document Review": return (ColorTranslator.FromHtml("#DBEAFE"), ColorTranslator.FromHtml("#1D4ED8"));   // blue
                case "Internal": return (ColorTranslator.FromHtml("#F3F4F6"), ColorTranslator.FromHtml("#374151"));          // gray
                case "Credit Assessment":
                case "Site Visit":
                case "Customer Meeting": return (ColorTranslator.FromHtml("#EDE9FE"), ColorTranslator.FromHtml("#5B21B6"));   // purple
                default: return (ColorTranslator.FromHtml("#FEF3C7"), ColorTranslator.FromHtml("#92400E"));                   // yellow
            }
        }

        private void GridTasks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridTasks.Columns[e.ColumnIndex].Name == "Done")
            {
                // map back to model using date and time+customer
                var time = gridTasks.Rows[e.RowIndex].Cells["Time"].Value?.ToString() ?? "";
                var customer = gridTasks.Rows[e.RowIndex].Cells["Customer"].Value?.ToString() ?? "";
                var task = _tasks.FirstOrDefault(t => t.Date.Date == _selectedDate.Date && t.Time == time && t.Customer == customer);
                if (task != null)
                {
                    var valObj = gridTasks.Rows[e.RowIndex].Cells["Done"].Value;
                    bool done = false;
                    if (valObj is bool b) done = b;
                    task.Completed = done;
                    RefreshTasksTable();
                }
            }
        }

        private void PreviousMonth()
        {
            _currentDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(-1);
            RefreshCalendarHeader();
            BuildCalendarGrid();
        }

        private void NextMonth()
        {
            _currentDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(1);
            RefreshCalendarHeader();
            BuildCalendarGrid();
        }

        private void GoToToday()
        {
            _currentDate = DateTime.Today;
            _selectedDate = DateTime.Today;
            RefreshCalendarHeader();
            BuildCalendarGrid();
            UpdateSelectedInfo();
            RefreshTasksTable();
        }

        private void OpenNewTaskDialog()
        {
            if (newTaskDialog != null && !newTaskDialog.IsDisposed)
            {
                newTaskDialog.Focus();
                return;
            }

            newTaskDialog = new Form
            {
                Text = "Add New Task",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(420, 420),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            var y = 10;

            var lblDate = new Label { Text = "Date", Location = new Point(10, y), AutoSize = true };
            dtpDate = new DateTimePicker { Location = new Point(10, y + 18), Width = 180, Value = _selectedDate };
            y += 54;

            var lblTime = new Label { Text = "Time", Location = new Point(10, y), AutoSize = true };
            dtpTime = new DateTimePicker { Location = new Point(10, y + 18), Width = 180, Format = DateTimePickerFormat.Time, ShowUpDown = true };
            y += 54;

            var lblCustomer = new Label { Text = "Customer Name", Location = new Point(10, y), AutoSize = true };
            txtCustomer = new TextBox { Location = new Point(10, y + 18), Width = 360 };
            y += 54;

            var lblType = new Label { Text = "Task Type", Location = new Point(10, y), AutoSize = true };
            cmbTaskType = new ComboBox { Location = new Point(10, y + 18), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTaskType.Items.AddRange(new object[]
            {
                "Payment Follow-up",
                "Document Review",
                "Credit Assessment",
                "Site Visit",
                "Internal",
                "Customer Meeting"
            });
            y += 54;

            var lblLoan = new Label { Text = "Loan Reference", Location = new Point(10, y), AutoSize = true };
            txtLoanRef = new TextBox { Location = new Point(10, y + 18), Width = 220 };
            y += 54;

            var lblDesc = new Label { Text = "Description", Location = new Point(10, y), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(10, y + 18), Width = 360, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
            y += 88;

            btnCancelTask = new Button { Text = "Cancel", Location = new Point(10, y), Width = 80 };
            btnAddTask = new Button { Text = "Add Task", Location = new Point(100, y), Width = 100, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelTask.Click += (s, e) => newTaskDialog.Close();
            btnAddTask.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCustomer.Text) || cmbTaskType.SelectedItem == null)
                {
                    MessageBox.Show("Please provide required fields (Customer Name, Task Type).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var task = new CalendarTask
                {
                    Id = DateTime.Now.Ticks.ToString(),
                    Date = dtpDate.Value.Date,
                    Time = dtpTime.Value.ToString("hh:mm tt"),
                    Customer = txtCustomer.Text.Trim(),
                    TaskType = cmbTaskType.SelectedItem.ToString(),
                    LoanRef = string.IsNullOrWhiteSpace(txtLoanRef.Text) ? "-" : txtLoanRef.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Completed = false
                };
                _tasks.Add(task);
                _selectedDate = task.Date;
                _currentDate = new DateTime(task.Date.Year, task.Date.Month, 1);
                RefreshCalendarHeader();
                BuildCalendarGrid();
                UpdateSelectedInfo();
                RefreshTasksTable();
                newTaskDialog.Close();
            };

            panel.Controls.AddRange(new Control[]
            {
                lblDate, dtpDate, lblTime, dtpTime, lblCustomer, txtCustomer, lblType, cmbTaskType, lblLoan, txtLoanRef, lblDesc, txtDescription, btnCancelTask, btnAddTask
            });

            newTaskDialog.Controls.Add(panel);
            newTaskDialog.ShowDialog(this);
        }
    }
}
