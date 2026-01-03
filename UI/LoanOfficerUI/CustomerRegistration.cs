using LendingApp.Class.Interface;
using LendingApp.Class.Repo;
using LendingApp.Class.Service;
using LendingApp.Class.Models.LoanOfiicerModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LendingApp.Class;
using System.Linq;

namespace LendingApp.UI.CustomerUI
{
    public partial class CustomerRegistration : Form
    {
        private readonly ICustomerRegistrationService _registrationService;
        private CustomerRegistrationData formData;
        private int activeSection;

        private readonly List<Section> sections = new List<Section>
        {
            new Section { Id = 0, Name = "Personal Information", Icon = "👤" },
            new Section { Id = 1, Name = "Contact Information", Icon = "📞" },
            new Section { Id = 2, Name = "Government IDs", Icon = "🛡️" },
            new Section { Id = 3, Name = "Employment & Income", Icon = "🏢" },
            new Section { Id = 4, Name = "Financial Information", Icon = "💰" },
            new Section { Id = 5, Name = "Emergency Contact", Icon = "📞" },
            new Section { Id = 6, Name = "System & Classification", Icon = "📊" },
            new Section { Id = 7, Name = "Document Attachments", Icon = "📎" }
        };

        private Label progressLabel;
        private readonly List<Button> sectionButtons = new List<Button>();
        private readonly Dictionary<int, Panel> sectionPanels = new Dictionary<int, Panel>();

        private Font headerFont;
        private Font labelFont;
        private Font navFont;
        private Font navFontActive;

        // Add these fields inside CustomerRegistration class
        private bool _isEditMode = false;

        // Constructor with dependency injection
        public CustomerRegistration(ICustomerRegistrationService registrationService)
        {
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

        // Add this constructor inside CustomerRegistration class (below the existing constructors)
        public CustomerRegistration(CustomerRegistrationData existingCustomer)
            : this(new CustomerRegistrationService(new CustomerRepository()))
        {
            if (existingCustomer == null) throw new ArgumentNullException(nameof(existingCustomer));
            LoadExistingCustomer(existingCustomer);
        }

        private void InitializeFonts()
        {
            headerFont = new Font("Segoe UI", 14f, FontStyle.Bold);
            labelFont = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            navFont = new Font("Segoe UI", 9f, FontStyle.Regular);
            navFontActive = new Font("Segoe UI", 9.5f, FontStyle.Bold);
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
            // Header contents
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
            progressLabel = new Label
            {
                Text = "",
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(customerIdLbl);
            headerPanel.Controls.Add(progressLabel);
            headerPanel.Resize += (s, e) =>
            {
                progressLabel.Left = headerPanel.Width - progressLabel.Width - 20;
                progressLabel.Top = 25;
            };

            // Navigation buttons (inside a scroll host for design-time visibility)
            var navScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            navigationPanel.Controls.Clear();
            navigationPanel.Controls.Add(navScroll);
            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                var btn = new Button
                {
                    Text = "   " + section.Icon + "  " + section.Name,
                    Tag = section.Id,
                    Width = navigationPanel.Width - 20,
                    Height = 46,
                    Location = new Point(10, 10 + (i * 50)),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = navFont
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += SectionButton_Click;
                sectionButtons.Add(btn);
                navScroll.Controls.Add(btn);
            }

            // Footer buttons
            footerPanel.Controls.Clear();
            var btnPrevious = CreateFooterButton("← Previous", new Point(20, 17));
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Enabled = false;
            btnPrevious.Click += (s, e) => ShowSection(activeSection - 1);

            var btnNext = CreateFooterButton("Next →", new Point(140, 17));
            btnNext.Name = "btnNext";
            btnNext.Click += (s, e) => ShowSection(activeSection + 1);

            var btnSave = CreateFooterButton("💾 Submit", new Point(footerPanel.Width - 160, 17));
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackColor = ColorTranslator.FromHtml("#2E6DA4");
            btnSave.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;
            footerPanel.Resize += (s, e) =>
            {
                btnSave.Left = footerPanel.Width - btnSave.Width - 20;
            };

            footerPanel.Controls.Add(btnPrevious);
            footerPanel.Controls.Add(btnNext);
            footerPanel.Controls.Add(btnSave);

            // Content host enhancements
            contentHost.Padding = new Padding(10);
            EnableDoubleBuffer(contentHost);
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
                ForeColor = ColorTranslator.FromHtml("#2C3E50")
            };
            btn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D0D7DE");
            btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#E8F4FF");
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
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };
            EnableDoubleBuffer(panel);

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
                ForeColor = ColorTranslator.FromHtml("#34495E"),
                AutoSize = true,
                Margin = new Padding(20, 15, 0, 20)
            };
            panel.Controls.Add(header);

            outer.Location = new Point(0, header.Bottom + 5);

            switch (sectionIndex)
            {
                case 0:
                    AddTextRow(outer, "First Name *", formData.FirstName, v => formData.FirstName = v);
                    AddTextRow(outer, "Last Name *", formData.LastName, v => formData.LastName = v);
                    AddTextRow(outer, "Middle Name", formData.MiddleName, v => formData.MiddleName = v);
                    AddDateRow(outer, "Date of Birth *", formData.DateOfBirth, v => formData.DateOfBirth = v);
                    AddComboRow(outer, "Gender *", new[] { "Male", "Female", "Other" }, formData.Gender, v => formData.Gender = v);
                    AddComboRow(outer, "Civil Status *", new[] { "Single", "Married", "Separated", "Widowed" }, formData.CivilStatus, v => formData.CivilStatus = v);
                    AddTextRow(outer, "Nationality", formData.Nationality, v => formData.Nationality = v);
                    break;
                case 1:
                    AddTextRow(outer, "Email Address", formData.EmailAddress, v => formData.EmailAddress = v);
                    AddTextRow(outer, "Mobile Number", formData.MobileNumber, v => formData.MobileNumber = v);
                    AddTextRow(outer, "Telephone Number", formData.TelephoneNumber, v => formData.TelephoneNumber = v);
                    AddMultilineRow(outer, "Present Address", formData.PresentAddress, v => formData.PresentAddress = v);
                    AddMultilineRow(outer, "Permanent Address", formData.PermanentAddress, v => formData.PermanentAddress = v);
                    AddTextRow(outer, "City", formData.City, v => formData.City = v);
                    AddTextRow(outer, "Province", formData.Province, v => formData.Province = v);
                    AddTextRow(outer, "Zip Code", formData.ZipCode, v => formData.ZipCode = v);
                    break;
                case 2:
                    AddTextRow(outer, "SSS Number", formData.SSSNumber, v => formData.SSSNumber = v);
                    AddTextRow(outer, "TIN Number", formData.TINNumber, v => formData.TINNumber = v);
                    AddTextRow(outer, "Passport Number", formData.PassportNumber, v => formData.PassportNumber = v);
                    AddTextRow(outer, "Driver's License No.", formData.DriversLicenseNumber, v => formData.DriversLicenseNumber = v);
                    AddTextRow(outer, "UMID Number", formData.UMIDNumber, v => formData.UMIDNumber = v);
                    AddTextRow(outer, "PhilHealth Number", formData.PhilhealthNumber, v => formData.PhilhealthNumber = v);
                    AddTextRow(outer, "Pag-IBIG Number", formData.PagibigNumber, v => formData.PagibigNumber = v);
                    break;
                case 3:
                    AddComboRow(outer, "Employment Status", new[] { "Employed", "Self-Employed", "Unemployed", "Retired" }, formData.EmploymentStatus, v => formData.EmploymentStatus = v);
                    AddTextRow(outer, "Company Name", formData.CompanyName, v => formData.CompanyName = v);
                    AddTextRow(outer, "Position", formData.Position, v => formData.Position = v);
                    AddTextRow(outer, "Department", formData.Department, v => formData.Department = v);
                    AddMultilineRow(outer, "Company Address", formData.CompanyAddress, v => formData.CompanyAddress = v);
                    AddTextRow(outer, "Company Phone", formData.CompanyPhone, v => formData.CompanyPhone = v);
                    break;
                case 4:
                    AddTextRow(outer, "Bank Name", formData.BankName, v => formData.BankName = v);
                    AddTextRow(outer, "Bank Account Number", formData.BankAccountNumber, v => formData.BankAccountNumber = v);
                    AddNumericRow(outer, "Credit Limit", formData.CreditLimit, v => formData.CreditLimit = v);
                    AddNumericRow(outer, "Initial Credit Score", formData.InitialCreditScore, v => formData.InitialCreditScore = (int)v);
                    break;
                case 5:
                    AddTextRow(outer, "Name", formData.EmergencyContactName, v => formData.EmergencyContactName = v);
                    AddTextRow(outer, "Relationship", formData.EmergencyContactRelationship, v => formData.EmergencyContactRelationship = v);
                    AddTextRow(outer, "Contact Number", formData.EmergencyContactNumber, v => formData.EmergencyContactNumber = v);
                    AddMultilineRow(outer, "Address", formData.EmergencyContactAddress, v => formData.EmergencyContactAddress = v);
                    break;
                case 6:
                    AddComboRow(outer, "Customer Type", new[] { "New", "Returning", "VIP" }, formData.CustomerType, v => formData.CustomerType = v);
                    AddComboRow(outer, "Status", new[] { "Active", "Inactive", "Suspended" }, formData.Status, v => formData.Status = v);
                    AddMultilineRow(outer, "Remarks", formData.Remarks, v => formData.Remarks = v);
                    break;
                case 7:
                    AddFileRow(outer, "Valid ID 1", formData.ValidId1Path, v => formData.ValidId1Path = v);
                    AddFileRow(outer, "Valid ID 2", formData.ValidId2Path, v => formData.ValidId2Path = v);
                    AddFileRow(outer, "Proof of Income", formData.ProofOfIncomePath, v => formData.ProofOfIncomePath = v);
                    AddFileRow(outer, "Proof of Address", formData.ProofOfAddressPath, v => formData.ProofOfAddressPath = v);
                    AddFileRow(outer, "Signature Image", formData.SignatureImagePath, v => formData.SignatureImagePath = v);
                    break;
            }

            panel.Controls.Add(outer);
            return panel;
        }

        private void AddTextRow(TableLayoutPanel tlp, string caption, string value, Action<string> onChange)
        {
            var lbl = MakeLabel(caption);
            var txt = new TextBox { Text = value ?? "", Dock = DockStyle.Fill };
            txt.TextChanged += (s, e) => onChange(txt.Text);
            AddRow(tlp, lbl, txt);
        }

        private void AddMultilineRow(TableLayoutPanel tlp, string caption, string value, Action<string> onChange)
        {
            var lbl = MakeLabel(caption);
            var txt = new TextBox { Text = value ?? "", Dock = DockStyle.Fill, Multiline = true, Height = 60, ScrollBars = ScrollBars.Vertical };
            txt.TextChanged += (s, e) => onChange(txt.Text);
            AddRow(tlp, lbl, txt);
        }

        private void AddComboRow(TableLayoutPanel tlp, string caption, string[] items, string selected, Action<string> onChange)
        {
            var lbl = MakeLabel(caption);
            var cmb = new ComboBox { Dock = DockStyle.Left, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmb.Items.AddRange(items);
            if (!string.IsNullOrEmpty(selected) && Array.IndexOf(items, selected) >= 0) cmb.SelectedItem = selected;
            cmb.SelectedIndexChanged += (s, e) => onChange(cmb.SelectedItem == null ? null : cmb.SelectedItem.ToString());
            AddRow(tlp, lbl, cmb);
        }

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

        private void AddRow(TableLayoutPanel tlp, Control label, Control field)
        {
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.Controls.Add(label, 0, tlp.RowCount);
            tlp.Controls.Add(field, 1, tlp.RowCount);
            tlp.RowCount += 1;
        }

        private void SectionButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is int idx) ShowSection(idx);
        }

        private void ShowSection(int sectionIndex)
        {
            if (sectionIndex < 0 || sectionIndex >= sections.Count) return;
            foreach (var kvp in sectionPanels) kvp.Value.Visible = false;
            sectionPanels[sectionIndex].Visible = true;
            activeSection = sectionIndex;
            UpdateNavigationState();
        }

        private void UpdateNavigationState()
        {
            var btnPrevious = footerPanel.Controls["btnPrevious"] as Button;
            var btnNext = footerPanel.Controls["btnNext"] as Button;
            if (btnPrevious != null) btnPrevious.Enabled = activeSection > 0;
            if (btnNext != null) btnNext.Enabled = activeSection < sections.Count - 1;

            for (int i = 0; i < sectionButtons.Count; i++)
            {
                var b = sectionButtons[i];
                bool active = i == activeSection;
                b.BackColor = active ? ColorTranslator.FromHtml("#E8F4FF") : Color.White;
                b.Font = active ? navFontActive : navFont;
            }

            progressLabel.Text = $"Section {activeSection + 1} of {sections.Count}";
            progressLabel.Left = headerPanel.Width - progressLabel.Width - 20;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;

                if (_isEditMode)
                {
                    // UPDATE existing record
                    using (var db = new AppDbContext())
                    {
                        // Ensure entity exists
                        var existing = db.Customers.SingleOrDefault(x => x.CustomerId == formData.CustomerId);
                        if (existing == null)
                        {
                            MessageBox.Show("Customer not found for update.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Copy values (explicit so EF tracks correctly)
                        existing.FirstName = formData.FirstName;
                        existing.LastName = formData.LastName;
                        existing.MiddleName = formData.MiddleName;
                        existing.DateOfBirth = formData.DateOfBirth;
                        existing.Gender = formData.Gender;
                        existing.CivilStatus = formData.CivilStatus;
                        existing.Nationality = formData.Nationality;

                        existing.EmailAddress = formData.EmailAddress;
                        existing.MobileNumber = formData.MobileNumber;
                        existing.TelephoneNumber = formData.TelephoneNumber;

                        existing.PresentAddress = formData.PresentAddress;
                        existing.PermanentAddress = formData.PermanentAddress;
                        existing.City = formData.City;
                        existing.Province = formData.Province;
                        existing.ZipCode = formData.ZipCode;

                        existing.SSSNumber = formData.SSSNumber;
                        existing.TINNumber = formData.TINNumber;
                        existing.PassportNumber = formData.PassportNumber;
                        existing.DriversLicenseNumber = formData.DriversLicenseNumber;
                        existing.UMIDNumber = formData.UMIDNumber;
                        existing.PhilhealthNumber = formData.PhilhealthNumber;
                        existing.PagibigNumber = formData.PagibigNumber;

                        existing.EmploymentStatus = formData.EmploymentStatus;
                        existing.CompanyName = formData.CompanyName;
                        existing.Position = formData.Position;
                        existing.Department = formData.Department;
                        existing.CompanyAddress = formData.CompanyAddress;
                        existing.CompanyPhone = formData.CompanyPhone;

                        existing.BankName = formData.BankName;
                        existing.BankAccountNumber = formData.BankAccountNumber;

                        existing.InitialCreditScore = formData.InitialCreditScore;
                        existing.CreditLimit = formData.CreditLimit;

                        existing.EmergencyContactName = formData.EmergencyContactName;
                        existing.EmergencyContactRelationship = formData.EmergencyContactRelationship;
                        existing.EmergencyContactNumber = formData.EmergencyContactNumber;
                        existing.EmergencyContactAddress = formData.EmergencyContactAddress;

                        existing.CustomerType = formData.CustomerType;
                        existing.Status = formData.Status;
                        existing.Remarks = formData.Remarks;

                        existing.ValidId1Path = formData.ValidId1Path;
                        existing.ValidId2Path = formData.ValidId2Path;
                        existing.ProofOfIncomePath = formData.ProofOfIncomePath;
                        existing.ProofOfAddressPath = formData.ProofOfAddressPath;
                        existing.SignatureImagePath = formData.SignatureImagePath;

                        existing.LastModifiedDate = DateTime.Now;

                        db.SaveChanges();
                    }

                    MessageBox.Show("Customer profile updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                // CREATE new record (existing behavior)
                var result = _registrationService.Register(formData);

                if (!result.Success)
                {
                    ShowSection(0);
                    MessageBox.Show(result.ErrorMessage, "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Registration submitted and saved to database.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to submit registration.\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        // Add this helper method inside CustomerRegistration class
        private void LoadExistingCustomer(CustomerRegistrationData existingCustomer)
        {
            _isEditMode = true;

            // Replace form data with DB entity
            formData = existingCustomer;

            // Rebuild UI so control initial values come from formData
            // IMPORTANT: clear header first so we don't duplicate controls
            headerPanel.Controls.Clear();
            navigationPanel.Controls.Clear();
            footerPanel.Controls.Clear();
            contentHost.Controls.Clear();

            BuildLayout();
            BuildAllSections();
            ShowSection(0);
        }

        private void EnableDoubleBuffer(Control c)
        {
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(c, true, null);
        }
    }

}