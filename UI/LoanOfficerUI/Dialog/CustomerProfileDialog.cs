using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class.Interface;
using LendingApp.Class.Models.LoanOfiicerModels;
using LendingApp.Class.Repo;
using LendingApp.UI.CustomerUI;
using LendingApp.UI.LoanOfficerUI.Dialog; // <-- new dialog namespace
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using LendingApp.Class;
using LendingApp.Class.Services.Reports; // add at top of file's using section

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class CustomerProfileDialog : Form
    {
        private readonly ICustomerRepository _customerRepo;
        private CustomerData customer;

        public CustomerProfileDialog(CustomerData customerData)
            : this(customerData, new CustomerRepository())
        {
        }

        public CustomerProfileDialog(CustomerData customerData, ICustomerRepository customerRepo)
        {
            customer = customerData ?? throw new ArgumentNullException(nameof(customerData));
            _customerRepo = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));

            InitializeComponent();   // calls the designer one

            // Load from DB first (if possible), then build UI once.
            LoadCustomerData();
            SetupUI();
        }

        private void ReloadFromDb()
        {
            if (string.IsNullOrWhiteSpace(customer?.Id)) return;

            var c = _customerRepo.GetById(customer.Id);
            if (c == null) return;

            customer = MapToDialogCustomerData(c);

            // Rebuild UI to reflect updated values (labels are static text created once)
            SetupUI();
        }

        private void LoadCustomerData()
        {
            // If we have an ID, always prefer DB as the source-of-truth.
            if (string.IsNullOrWhiteSpace(customer?.Id))
                return;

            var dbCustomer = _customerRepo.GetById(customer.Id);
            if (dbCustomer == null)
                return;

            customer = MapToDialogCustomerData(dbCustomer);
        }

        private static CustomerData MapToDialogCustomerData(CustomerRegistrationData c)
        {
            return new CustomerData
            {
                Id = c.CustomerId,
                FullName = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                DOB = c.DateOfBirth.HasValue ? c.DateOfBirth.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")) : "",
                Age = c.DateOfBirth.HasValue ? (int)((DateTime.Today - c.DateOfBirth.Value.Date).TotalDays / 365.2425) : 0,
                Gender = c.Gender,
                CivilStatus = c.CivilStatus,
                Nationality = c.Nationality,
                Email = c.EmailAddress,
                Mobile = c.MobileNumber,
                Telephone = c.TelephoneNumber,
                PresentAddress = c.PresentAddress,
                PermanentAddress = c.PermanentAddress,
                RegistrationDate = c.RegistrationDate.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")),
                CustomerType = c.CustomerType,
                CreditScore = c.InitialCreditScore,
                CreditLimit = "₱" + c.CreditLimit.ToString("N2", CultureInfo.GetCultureInfo("en-US")),
                Status = c.Status,

                // Not implemented in DB yet (safe defaults)
                ActiveLoans = 0,
                TotalBalance = "₱0.00",
                PaymentHistory = "",

                // Government
                SSS = c.SSSNumber,
                TIN = c.TINNumber,
                Passport = c.PassportNumber,
                DriversLicense = c.DriversLicenseNumber,
                UMID = c.UMIDNumber,
                PhilHealth = c.PhilhealthNumber,
                Pagibig = c.PagibigNumber,

                // Employment
                EmploymentStatus = c.EmploymentStatus,
                CompanyName = c.CompanyName,
                Position = c.Position,
                Department = c.Department,
                CompanyAddress = c.CompanyAddress,
                CompanyPhone = c.CompanyPhone,

                // Bank
                BankName = c.BankName,
                BankAccountNumber = c.BankAccountNumber,

                // Emergency
                EmergencyContactName = c.EmergencyContactName,
                EmergencyContactRelationship = c.EmergencyContactRelationship,
                EmergencyContactNumber = c.EmergencyContactNumber,
                EmergencyContactAddress = c.EmergencyContactAddress,

                // Docs
                ValidId1Path = c.ValidId1Path,
                ValidId2Path = c.ValidId2Path,

                Remarks = c.Remarks,
                CreatedByText = c.CreatedBy.HasValue ? c.CreatedBy.Value.ToString(CultureInfo.InvariantCulture) : "",
                LastModifiedText = c.LastModifiedDate.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"))
            };
        }

        private void SetupUI()
        {
            // Main container with scroll
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            int currentY = 20;

            // Title
            Label title = new Label
            {
                Text = $"CUSTOMER PROFILE - {customer?.Id ?? "N/A"}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, currentY),
                Size = new Size(700, 30)
            };
            mainPanel.Controls.Add(title);
            currentY += 40;

            // Personal Information
            AddSection(mainPanel, "Personal Information", ref currentY);
            AddPersonalInfo(mainPanel, ref currentY);
            currentY += 20;

            // Government IDs and Financial Summary side by side
            int leftY = currentY;
            AddSectionLeft(mainPanel, "Government IDs", ref leftY);
            AddGovernmentIDs(mainPanel, ref leftY);

            int rightY = currentY;
            AddSectionRight(mainPanel, "Financial Summary", ref rightY);
            AddFinancialSummary(mainPanel, ref rightY);

            currentY = Math.Max(leftY, rightY) + 20;

            // Employment and Emergency Contact side by side
            leftY = currentY;
            AddSectionLeft(mainPanel, "Employment Information", ref leftY);
            AddEmploymentInfo(mainPanel, ref leftY);

            rightY = currentY;
            AddSectionRight(mainPanel, "Emergency Contact", ref rightY);
            AddEmergencyContact(mainPanel, ref rightY);

            currentY = Math.Max(leftY, rightY) + 20;

            // Bank Information and Documents side by side
            leftY = currentY;
            AddSectionLeft(mainPanel, "Bank Information", ref leftY);
            AddBankInfo(mainPanel, ref leftY);

            rightY = currentY;
            AddSectionRight(mainPanel, "Documents & Attachments", ref rightY);
            AddDocuments(mainPanel, ref rightY);

            currentY = Math.Max(leftY, rightY) + 20;

            // Remarks
            AddSection(mainPanel, "Remarks & Audit Trail", ref currentY);
            AddRemarks(mainPanel, ref currentY);
            currentY += 20;

            // Quick Actions
            AddSection(mainPanel, "Quick Actions", ref currentY);
            AddQuickActions(mainPanel, ref currentY);
            currentY += 60;

            // Buttons at bottom
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(700, 50)
            };

            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(500, 10),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9)
            };
            btnClose.Click += (s, e) => Close();

            Button btnEdit = new Button
            {
                Text = "Edit Customer",
                Location = new Point(590, 10),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White
            };
            btnEdit.Click += (s, e) => MessageBox.Show("Edit customer feature", "Edit");

            buttonPanel.Controls.Add(btnClose);
            buttonPanel.Controls.Add(btnEdit);
            mainPanel.Controls.Add(buttonPanel);

            Controls.Clear();
            Controls.Add(mainPanel);

            // Form properties (do it here, not in InitializeComponent)
            Text = $"Customer Profile - {customer?.Id ?? "N/A"}";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void AddQuickActions(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(700, 90),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Removed "Apply Loan", "Credit History", "Add Co-maker" per request.
            string[] buttons =
            {
                "Update Profile",
                "View Loans",
                "Generate Report",
                customer?.Status == "Active" ? "Blacklist" : "Activate"
            };

            int x = 10;
            int rowY = 10;

            for (int i = 0; i < buttons.Length; i++)
            {
                Button btn = new Button
                {
                    Text = buttons[i],
                    Location = new Point(x, rowY),
                    Size = new Size(120, 30),
                    Font = new Font("Segoe UI", 9),
                    FlatStyle = FlatStyle.Flat
                };

                if (buttons[i] == "Blacklist")
                {
                    btn.ForeColor = Color.Red;
                    btn.FlatAppearance.BorderColor = Color.Red;
                }
                else if (buttons[i] == "Activate")
                {
                    btn.ForeColor = Color.Green;
                    btn.FlatAppearance.BorderColor = Color.Green;
                }
                else
                {
                    btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                }

                btn.Click += (s, e) =>
                {
                    if (btn.Text == "Update Profile")
                    {
                        if (string.IsNullOrWhiteSpace(customer?.Id))
                        {
                            MessageBox.Show("Invalid customer id.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var dbCustomer = _customerRepo.GetById(customer.Id);
                        if (dbCustomer == null)
                        {
                            MessageBox.Show("Customer not found in database.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        using (var editForm = new CustomerRegistration(dbCustomer))
                        {
                            var result = editForm.ShowDialog(this);
                            if (result == DialogResult.OK)
                            {
                                // Refresh this dialog UI immediately
                                ReloadFromDb();

                                // Also let the caller know changes happened (optional behavior kept)
                                DialogResult = DialogResult.OK;
                            }
                        }

                        return;
                    }

                    if (btn.Text == "View Loans")
                    {
                        if (string.IsNullOrWhiteSpace(customer?.Id))
                        {
                            MessageBox.Show("Invalid customer id.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        using (var dlg = new CustomerLoansDialog(customer.Id, customer.FullName))
                        {
                            dlg.ShowDialog(this);
                        }
                        return;
                    }

                    if (btn.Text == "Generate Report")
                    {
                        try
                        {
                            GeneratePdfReport();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to generate report:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }

                    MessageBox.Show(btn.Text + " feature", "Action");
                };
                panel.Controls.Add(btn);
                x += 130;
            }

            parent.Controls.Add(panel);
            y += 100;
        }

        private void GeneratePdfReport()
        {
            if (string.IsNullOrWhiteSpace(customer?.Id))
            {
                MessageBox.Show("Invalid customer id.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // refresh latest customer from DB
            var dbCustomer = _customerRepo.GetById(customer.Id);
            if (dbCustomer != null) customer = MapToDialogCustomerData(dbCustomer);

            using (var sfd = new SaveFileDialog { Filter = "PDF files (*.pdf)|*.pdf", FileName = $"{customer.Id}_profile.pdf" })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    var svc = new CustomerReportService();
                    svc.GenerateCustomerProfilePdf(customer.Id, sfd.FileName);

                    MessageBox.Show("Report generated: " + sfd.FileName, "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to generate report:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Expanded customer data class to support what UI is showing
        public class CustomerData
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string DOB { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public string CivilStatus { get; set; }
            public string Nationality { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string Telephone { get; set; }
            public string PresentAddress { get; set; }
            public string PermanentAddress { get; set; }
            public string RegistrationDate { get; set; }
            public string CustomerType { get; set; }
            public int CreditScore { get; set; }
            public string CreditLimit { get; set; }
            public string Status { get; set; }

            public int ActiveLoans { get; set; }
            public string TotalBalance { get; set; }
            public string PaymentHistory { get; set; }

            public string SSS { get; set; }
            public string TIN { get; set; }
            public string Passport { get; set; }
            public string DriversLicense { get; set; }
            public string UMID { get; set; }
            public string PhilHealth { get; set; }
            public string Pagibig { get; set; }

            public string EmploymentStatus { get; set; }
            public string CompanyName { get; set; }
            public string Position { get; set; }
            public string Department { get; set; }
            public string CompanyAddress { get; set; }
            public string CompanyPhone { get; set; }

            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }

            public string EmergencyContactName { get; set; }
            public string EmergencyContactRelationship { get; set; }
            public string EmergencyContactNumber { get; set; }
            public string EmergencyContactAddress { get; set; }

            public string ValidId1Path { get; set; }
            public string ValidId2Path { get; set; }

            public string Remarks { get; set; }
            public string CreatedByText { get; set; }
            public string LastModifiedText { get; set; }
        }

        private void AddSection(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(700, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionLeft(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(340, 35),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddSectionRight(Panel parent, string title, ref int y)
        {
            Panel section = new Panel
            {
                Location = new Point(360, y),
                Size = new Size(340, 35),
                BackColor = Color.FromArgb(220, 252, 231),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label
            {
                Text = title,
                Location = new Point(10, 8),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black
            };

            section.Controls.Add(label);
            parent.Controls.Add(section);
            y += 40;
        }

        private void AddPersonalInfo(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(700, 200),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] info =
            {
                $"Customer ID: {customer?.Id}",
                $"Registration Date: {customer?.RegistrationDate}",
                $"Name: {customer?.FullName}",
                $"DOB: {customer?.DOB} ({customer?.Age})",
                $"Gender: {customer?.Gender} | Civil Status: {customer?.CivilStatus}",
                $"Nationality: {customer?.Nationality}",
                $"Email: {customer?.Email}",
                $"Mobile: {customer?.Mobile} | Telephone: {customer?.Telephone}",
                $"Present Address: {customer?.PresentAddress}",
                $"Permanent Address: {customer?.PermanentAddress}"
            };

            for (int i = 0; i < info.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = info[i],
                    Location = new Point(10, 10 + (i * 18)),
                    Size = new Size(680, 18),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 210;
        }

        private void AddGovernmentIDs(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(340, 180),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] ids =
            {
                "SSS: " + (customer?.SSS ?? ""),
                "TIN: " + (customer?.TIN ?? ""),
                "Passport: " + (customer?.Passport ?? ""),
                "Driver's: " + (customer?.DriversLicense ?? ""),
                "UMID: " + (customer?.UMID ?? ""),
                "PhilHealth: " + (customer?.PhilHealth ?? ""),
                "Pag-ibig: " + (customer?.Pagibig ?? "")
            };

            for (int i = 0; i < ids.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = ids[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 190;
        }

        private void AddFinancialSummary(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(360, y),
                Size = new Size(340, 180),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] financial =
            {
                $"Customer Type: {customer?.CustomerType}",
                $"Credit Score: {customer?.CreditScore}",
                $"Credit Limit: {customer?.CreditLimit}",
                $"Status: {customer?.Status}",
                "---",
                $"Active Loans: {customer?.ActiveLoans}",
                $"Total Balance: {customer?.TotalBalance}",
                $"Payment History: {customer?.PaymentHistory}"
            };

            for (int i = 0; i < financial.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = financial[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 190;
        }

        private void AddEmploymentInfo(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(340, 140),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] employment =
            {
                $"Status: {customer?.EmploymentStatus}",
                $"Company: {customer?.CompanyName}",
                $"Position: {customer?.Position}",
                $"Department: {customer?.Department}",
                $"Company Address: {customer?.CompanyAddress}",
                $"Company Phone: {customer?.CompanyPhone}"
            };

            for (int i = 0; i < employment.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = employment[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 150;
        }

        private void AddEmergencyContact(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(360, y),
                Size = new Size(340, 140),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] emergency =
            {
                $"Name: {customer?.EmergencyContactName} ({customer?.EmergencyContactRelationship})",
                $"Contact: {customer?.EmergencyContactNumber}",
                $"Address: {customer?.EmergencyContactAddress}"
            };

            for (int i = 0; i < emergency.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = emergency[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 150;
        }

        private void AddBankInfo(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(340, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] bank =
            {
                $"Bank: {customer?.BankName}",
                $"Account: {customer?.BankAccountNumber}"
            };

            for (int i = 0; i < bank.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = bank[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 90;
        }

        private void AddDocuments(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(360, y),
                Size = new Size(340, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] docs =
            {
                string.IsNullOrWhiteSpace(customer?.ValidId1Path) ? "✗ Valid ID 1" : "✓ Valid ID 1",
                string.IsNullOrWhiteSpace(customer?.ValidId2Path) ? "✗ Valid ID 2" : "✓ Valid ID 2"
            };

            for (int i = 0; i < docs.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = docs[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(320, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 90;
        }

        private void AddRemarks(Panel parent, ref int y)
        {
            Panel panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(700, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            string[] remarks =
            {
                "Remarks: " + (customer?.Remarks ?? ""),
                "Created By: " + (customer?.CreatedByText ?? ""),
                "Last Modified: " + (customer?.LastModifiedText ?? "")
            };

            for (int i = 0; i < remarks.Length; i++)
            {
                panel.Controls.Add(new Label
                {
                    Text = remarks[i],
                    Location = new Point(10, 10 + (i * 20)),
                    Size = new Size(680, 20),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Black
                });
            }

            parent.Controls.Add(panel);
            y += 110;
        }
    }
}