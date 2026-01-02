<<<<<<< HEAD
﻿using LendingApp.Data;
using LendingApp.Models.LoanOfiicerModels;
using LendingApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
=======
﻿using LendingApp.Class.Interface;
using LendingApp.Class.Repo;
using LendingApp.Class.Service;
using LendingApp.Models.LoanOfiicerModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
>>>>>>> 024ba6a64cd828dd254940d307284b2e15a30d32
using System.Windows.Forms;

namespace LendingApp.UI.CustomerUI
{
    public partial class CustomerRegistration : Form
    {
        private readonly ICustomerRegistrationService _registrationService;
        private CustomerRegistrationData formData;
        private int activeSection;
        private DataSample dataSample;

        private readonly Label progressLabel = new Label();
        private readonly List<Button> sectionButtons = new List<Button>();
        private readonly Dictionary<int, Panel> sectionPanels = new Dictionary<int, Panel>();

        private readonly List<Section> sections = new List<Section>
        {
         new Section { Id = 0, Name = "Personal Information", Icon = "👤" },
         new Section { Id = 1, Name = "System & Classification", Icon = "📊" }
        };


        private Font headerFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        private Font labelFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        private Font navFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        private Font navFontActive = new Font("Segoe UI", 9.5f, FontStyle.Bold);

        // Constructor with dependency injection
        public CustomerRegistration(ICustomerRegistrationService registrationService)
        {
<<<<<<< HEAD
            InitializeComponent();

            InitializeData();
            BuildLayout();
            BuildAllSections();
            ShowSection(0);
=======
            _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));

            InitializeComponent();
            InitializeFonts();
            InitializeData();
            BuildLayout();
            BuildAllSections();
            ShowSection(0);
        }

        // Parameterless constructor for designer + manual wiring
        public CustomerRegistration()
            : this(new CustomerRegistrationService(new CustomerRepository()))
        {
        }

        private void InitializeFonts()
        {
            headerFont = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            navFont = new Font("Segoe UI", 9f, FontStyle.Regular);
            navFontActive = new Font("Segoe UI", 9.5f, FontStyle.Bold);
>>>>>>> 024ba6a64cd828dd254940d307284b2e15a30d32
        }

        private void InitializeData()
        {
            formData = new CustomerRegistrationData
            {
                CustomerId = "CUST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                RegistrationDate = DateTime.Now
            };
        }

        private void BuildLayout()
        {
            // Simple header
            var title = new Label
            {
                Text = "Customer Registration",
                ForeColor = Color.White,
                Font = headerFont,
                AutoSize = true,
                Location = new Point(20, 18)
            };
            var customerIdLbl = new Label
            {
                Text = "Customer ID: " + formData.CustomerId,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(22, 45)
            };
            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(customerIdLbl);
            headerPanel.Controls.Add(progressLabel);

            // Footer buttons
            var btnPrevious = CreateFooterButton("← Previous", new Point(20, 17));
            btnPrevious.Enabled = false;
            btnPrevious.Click += (s, e) => ShowSection(activeSection - 1);

            var btnNext = CreateFooterButton("Next →", new Point(140, 17));
            btnNext.Click += (s, e) => ShowSection(activeSection + 1);

            var btnSave = CreateFooterButton("💾 Submit", new Point(footerPanel.Width - 160, 17));
            btnSave.BackColor = ColorTranslator.FromHtml("#2E6DA4");
            btnSave.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;

            footerPanel.Controls.Add(btnPrevious);
            footerPanel.Controls.Add(btnNext);
            footerPanel.Controls.Add(btnSave);
        }

        private Button CreateFooterButton(string text, Point location)
        {
            var btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(110, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            btn.FlatAppearance.BorderColor = Color.Gray;
            btn.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            return btn;
        }

        private void BuildAllSections()
        {
            contentHost.Controls.Clear();
            for (int i = 0; i < sections.Count; i++)
            {
                var panel = BuildSectionPanel(i);
                panel.Visible = false;
                sectionPanels[i] = panel;
                contentHost.Controls.Add(panel);
            }
        }

        private Panel BuildSectionPanel(int sectionIndex)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };

            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(15)
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));

            var header = new Label
            {
                Text = sections[sectionIndex].Name,
                Font = headerFont,
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(20, 15, 0, 20)
            };
            panel.Controls.Add(header);
            outer.Location = new Point(0, header.Bottom + 5);

            switch (sectionIndex)
            {
                case 0:
                    AddTextRow(outer, "First Name", formData.FirstName, v => formData.FirstName = v);
                    AddTextRow(outer, "Last Name", formData.LastName, v => formData.LastName = v);
                    break;

                case 1:
                    AddComboRow(outer, "Customer Type", new[] { "New", "Returning", "VIP" }, formData.CustomerType, v => formData.CustomerType = v);
                    break;
            }

            panel.Controls.Add(outer);
            return panel;
        }

        private void AddTextRow(TableLayoutPanel tlp, string caption, string value, Action<string> onChange)
        {
            var lbl = new Label { Text = caption, Font = labelFont, AutoSize = true };
            var txt = new TextBox { Text = value ?? "", Dock = DockStyle.Fill };
            txt.TextChanged += (s, e) => onChange(txt.Text);
            AddRow(tlp, lbl, txt);
        }

        private void AddComboRow(TableLayoutPanel tlp, string caption, string[] items, string selected, Action<string> onChange)
        {
            var lbl = new Label { Text = caption, Font = labelFont, AutoSize = true };
            var cmb = new ComboBox { Dock = DockStyle.Left, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmb.Items.AddRange(items);
            if (!string.IsNullOrEmpty(selected)) cmb.SelectedItem = selected;
            cmb.SelectedIndexChanged += (s, e) => onChange(cmb.SelectedItem?.ToString());
            AddRow(tlp, lbl, cmb);
        }

<<<<<<< HEAD
=======
        private void AddDateRow(TableLayoutPanel tlp, string caption, DateTime? value, Action<DateTime?> onChange)
        {
            var lbl = MakeLabel(caption);
            var dtp = new DateTimePicker
            {
                Dock = DockStyle.Left,
                Width = 220,
                Value = value.HasValue ? value.Value : DateTime.Now.AddYears(-25)
            };

            dtp.ValueChanged += (s, e) => onChange(dtp.Value);
            AddRow(tlp, lbl, dtp);
        }

        private void AddNumericRow(TableLayoutPanel tlp, string caption, decimal value, Action<decimal> onChange)
        {
            var lbl = MakeLabel(caption);
            var num = new NumericUpDown { Dock = DockStyle.Left, Width = 160, Maximum = 100000000, DecimalPlaces = 2, Value = value < 0 ? 0 : value };
            num.ValueChanged += (s, e) => onChange(num.Value);
            AddRow(tlp, lbl, num);
        }

        private void AddFileRow(TableLayoutPanel tlp, string caption, string current, Action<string> onPick)
        {
            var lbl = MakeLabel(caption);
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            var fileLabel = new Label { Text = string.IsNullOrEmpty(current) ? "No file selected" : Path.GetFileName(current), AutoSize = true, ForeColor = string.IsNullOrEmpty(current) ? Color.Gray : Color.Black, Margin = new Padding(0, 6, 10, 0) };
            var btn = new Button { Text = "Browse...", AutoSize = true, FlatStyle = FlatStyle.Standard };
            btn.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Supported Files|*.jpg;*.jpeg;*.png;*.pdf;*.bmp";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        onPick(ofd.FileName);
                        fileLabel.Text = Path.GetFileName(ofd.FileName);
                        fileLabel.ForeColor = Color.Black;
                    }
                }
            };
            panel.Controls.Add(fileLabel);
            panel.Controls.Add(btn);
            AddRow(tlp, lbl, panel);
        }

        private Label MakeLabel(string text)
        {
            return new Label { Text = text, Font = labelFont, ForeColor = ColorTranslator.FromHtml("#2C3E50"), AutoSize = true, Margin = new Padding(0, 6, 0, 0) };
        }

>>>>>>> 024ba6a64cd828dd254940d307284b2e15a30d32
        private void AddRow(TableLayoutPanel tlp, Control label, Control field)
        {
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.Controls.Add(label, 0, tlp.RowCount);
            tlp.Controls.Add(field, 1, tlp.RowCount);
            tlp.RowCount += 1;
        }

        private void SectionButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int idx) ShowSection(idx);
        }

        private void ShowSection(int sectionIndex)
        {
            if (sectionIndex < 0 || sectionIndex >= sections.Count) return;
            foreach (var kvp in sectionPanels) kvp.Value.Visible = false;
            sectionPanels[sectionIndex].Visible = true;
            activeSection = sectionIndex;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
            // Add to AllLoans for testing
            var loan = new LoanModel
            {
                LoanNumber = "LN-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Borrower = $"{formData.FirstName} {formData.LastName}",
                Contact = "", // Add from formData if needed
                Type = formData.CustomerType ?? "New",
                Applied = "Pending",
                Amount = 1,
                Balance = 1,
                ApprovedDate = DateTime.Today
            };

            DataGetter.Data.AllLoans.Add(loan); 

            MessageBox.Show($"Loan added: {loan.Borrower}, Type: {loan.Type}");
            Close();
=======
            try
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;

                // Call the service (no EF code here!)
                var result = _registrationService.Register(formData);

                if (!result.Success)
                {
                    ShowSection(0);
                    MessageBox.Show(result.ErrorMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Registration submitted and saved to database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to submit registration.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;
            }
>>>>>>> 024ba6a64cd828dd254940d307284b2e15a30d32
        }
    }
}
