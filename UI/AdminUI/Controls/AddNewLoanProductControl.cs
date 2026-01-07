using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LendingApp.Class;
using LendingApp.Class.Models.Loans;
using MySql.Data.MySqlClient;
using LendingApp.Class.Services.Admin;

namespace LendingApp.UI.AdminUI
{
    public partial class AddNewLoanProductControl : UserControl
    {
        // NEW: edit mode state
        private int? _editingProductId;

        // NEW: notify parent to refresh grid
        public event Action<int> ProductSaved;

        // Form fields
        private TextBox txtLoanTypeName;
        private TextBox txtDescription;
        private RadioButton rdoFixedInterest;
        private RadioButton rdoVariableInterest;
        private TextBox txtInterestRate;
        private ComboBox cmbInterestPeriod;
        private TextBox txtServiceFeePercentage;
        private TextBox txtServiceFeeFixed;
        private TextBox txtMinLoanAmount;
        private TextBox txtMaxLoanAmount;
        private CheckBox[] termCheckboxes;
        private CheckBox chkValidId;
        private CheckBox chkProofOfIncome;
        private CheckBox chkPayslip;
        private CheckBox chkBankStatement;
        private CheckBox chkComakerForm;
        private CheckBox chkOthers;
        private TextBox txtOthersDescription;
        private RadioButton rdoCollateralYes;
        private RadioButton rdoCollateralNo;
        private TextBox txtGracePeriod;
        private TextBox txtLatePenalty;
        private ComboBox cmbLatePenaltyPeriod;
        private RadioButton rdoStatusActive;
        private RadioButton rdoStatusInactive;
        private Button btnSaveProduct;
        private Button btnClearForm;

        private Label lblPlusIcon;

        public AddNewLoanProductControl()
        {
            InitializeControl();
        }

        // Build the dynamic UI (same structure as previous implementation).
        // Kept self-contained so the control works when instantiated by LoanProductsControl.
        private void InitializeControl()
        {
            // Control settings
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Segoe UI", 9);
            this.AutoScroll = true; // Enable scrolling on the UserControl

            // Main container - MUST have AutoScroll = true
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true, // This is CRITICAL for scrolling
                AutoSize = false // Don't auto-size, let it scroll
            };

            // Create a content panel that holds all the controls
            var contentPanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Width = Math.Max(820, mainPanel.Width - 40) // Account for padding and avoid zero width on startup
            };

            int yPos = 10;

            // ===== HEADER =====
            var headerPanel = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 20, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 255, 240)
            };

            var lblHeader = new Label
            {
                Text = "ADD NEW LOAN PRODUCT",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true,
                ForeColor = Color.DarkGreen
            };

            // Plus / edit icon (class-level so LoadProductForEdit can update it)
            lblPlusIcon = new Label
            {
                Text = "+",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(headerPanel.Width - 40, 10),
                Size = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Green,
                Name = "lblPlusIcon" // Give it a name for easier identification
            };

            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(lblPlusIcon);
            contentPanel.Controls.Add(headerPanel);

            yPos += 70;

            // ===== LOAN TYPE NAME & DESCRIPTION =====
            var lblSection1 = new Label
            {
                Text = "Loan Type Name & Description",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection1);

            yPos += 25;

            var panel1 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Loan Type Name
            var lblLoanTypeName = new Label
            {
                Text = "Loan Type Name:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            panel1.Controls.Add(lblLoanTypeName);

            txtLoanTypeName = new TextBox
            {
                Location = new Point(120, 12),
                Size = new Size(300, 25),
                Text = ""
            };
            panel1.Controls.Add(txtLoanTypeName);

            // Description
            var lblDescription = new Label
            {
                Text = "Description:",
                Location = new Point(450, 15),
                AutoSize = true
            };
            panel1.Controls.Add(lblDescription);

            txtDescription = new TextBox
            {
                Location = new Point(530, 12),
                Size = new Size(250, 25),
                Text = ""
            };
            panel1.Controls.Add(txtDescription);

            contentPanel.Controls.Add(panel1);
            yPos += 90;

            // ===== INTEREST TYPE & RATE =====
            var lblSection2 = new Label
            {
                Text = "Interest Type & Rate",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection2);

            yPos += 25;

            var panel2 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 120),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Interest Type
            var lblInterestType = new Label
            {
                Text = "Interest Type:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            panel2.Controls.Add(lblInterestType);

            rdoFixedInterest = new RadioButton
            {
                Text = "Fixed",
                Location = new Point(120, 15),
                AutoSize = true,
                Checked = true
            };
            panel2.Controls.Add(rdoFixedInterest);

            rdoVariableInterest = new RadioButton
            {
                Text = "Variable",
                Location = new Point(200, 15),
                AutoSize = true
            };
            panel2.Controls.Add(rdoVariableInterest);

            // Interest Rate
            var lblInterestRate = new Label
            {
                Text = "Interest Rate:",
                Location = new Point(10, 50),
                AutoSize = true
            };
            panel2.Controls.Add(lblInterestRate);

            txtInterestRate = new TextBox
            {
                Location = new Point(120, 47),
                Size = new Size(80, 25),
                Text = "0.00"
            };
            panel2.Controls.Add(txtInterestRate);

            var lblPercent = new Label
            {
                Text = "% per",
                Location = new Point(210, 50),
                AutoSize = true
            };
            panel2.Controls.Add(lblPercent);

            cmbInterestPeriod = new ComboBox
            {
                Location = new Point(260, 47),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbInterestPeriod.Items.AddRange(new object[] { "Month", "Year" });
            cmbInterestPeriod.SelectedIndex = 0;
            panel2.Controls.Add(cmbInterestPeriod);

            // Service Fee
            var lblServiceFee = new Label
            {
                Text = "Service Fee (%):",
                Location = new Point(10, 85),
                AutoSize = true
            };
            panel2.Controls.Add(lblServiceFee);

            txtServiceFeePercentage = new TextBox
            {
                Location = new Point(120, 82),
                Size = new Size(80, 25),
                Text = "0.00"
            };
            panel2.Controls.Add(txtServiceFeePercentage);

            var lblOr = new Label
            {
                Text = "% or ₱",
                Location = new Point(210, 85),
                AutoSize = true
            };
            panel2.Controls.Add(lblOr);

            txtServiceFeeFixed = new TextBox
            {
                Location = new Point(260, 82),
                Size = new Size(100, 25),
                Text = "0.00"
            };
            panel2.Controls.Add(txtServiceFeeFixed);

            contentPanel.Controls.Add(panel2);
            yPos += 130;

            // ===== LOAN AMOUNT =====
            var lblSection3 = new Label
            {
                Text = "Loan Amount",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection3);

            yPos += 25;

            var panel3 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 70),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Min Loan Amount
            var lblMinLoanAmount = new Label
            {
                Text = "Min Loan Amount: ₱",
                Location = new Point(10, 25),
                AutoSize = true
            };
            panel3.Controls.Add(lblMinLoanAmount);

            txtMinLoanAmount = new TextBox
            {
                Location = new Point(150, 22),
                Size = new Size(200, 25),
                Text = "0.00"
            };
            panel3.Controls.Add(txtMinLoanAmount);

            // Max Loan Amount
            var lblMaxLoanAmount = new Label
            {
                Text = "Max Loan Amount: ₱",
                Location = new Point(400, 25),
                AutoSize = true
            };
            panel3.Controls.Add(lblMaxLoanAmount);

            txtMaxLoanAmount = new TextBox
            {
                Location = new Point(540, 22),
                Size = new Size(200, 25),
                Text = "0.00"
            };
            panel3.Controls.Add(txtMaxLoanAmount);

            contentPanel.Controls.Add(panel3);
            yPos += 80;

            // ===== AVAILABLE TERMS =====
            var lblSection4 = new Label
            {
                Text = "AVAILABLE TERMS (months):",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection4);

            yPos += 25;

            var panel4 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            int[] terms = { 3, 6, 12, 18, 24, 36, 48, 60 };
            termCheckboxes = new CheckBox[terms.Length];

            int xPos = 10;
            for (int i = 0; i < terms.Length; i++)
            {
                termCheckboxes[i] = new CheckBox
                {
                    Text = terms[i].ToString(),
                    Location = new Point(xPos, 20),
                    AutoSize = true
                };
                panel4.Controls.Add(termCheckboxes[i]);
                xPos += 80;
            }

            contentPanel.Controls.Add(panel4);
            yPos += 70;

            // ===== REQUIRED DOCUMENTS =====
            var lblSection5 = new Label
            {
                Text = "REQUIRED DOCUMENTS:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection5);

            yPos += 25;

            var panel5 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Document checkboxes
            chkValidId = new CheckBox { Text = "Valid ID", Location = new Point(10, 20), AutoSize = true };
            chkProofOfIncome = new CheckBox { Text = "Proof of Income", Location = new Point(100, 20), AutoSize = true };
            chkPayslip = new CheckBox { Text = "Payslip", Location = new Point(220, 20), AutoSize = true };
            chkBankStatement = new CheckBox { Text = "Bank Statement", Location = new Point(300, 20), AutoSize = true };
            chkComakerForm = new CheckBox { Text = "Co-maker Form", Location = new Point(420, 20), AutoSize = true };
            chkOthers = new CheckBox { Text = "Others:", Location = new Point(540, 20), AutoSize = true };

            panel5.Controls.Add(chkValidId);
            panel5.Controls.Add(chkProofOfIncome);
            panel5.Controls.Add(chkPayslip);
            panel5.Controls.Add(chkBankStatement);
            panel5.Controls.Add(chkComakerForm);
            panel5.Controls.Add(chkOthers);

            txtOthersDescription = new TextBox
            {
                Location = new Point(610, 17),
                Size = new Size(170, 25),
                Text = "",
                Enabled = false
            };
            chkOthers.CheckedChanged += (s, e) => { txtOthersDescription.Enabled = chkOthers.Checked; };
            panel5.Controls.Add(txtOthersDescription);

            contentPanel.Controls.Add(panel5);
            yPos += 110;

            // ===== COLLATERAL REQUIRED =====
            var lblSection6 = new Label
            {
                Text = "Collateral Required?",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection6);

            yPos += 25;

            var panel6 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 50),
                BorderStyle = BorderStyle.FixedSingle
            };

            rdoCollateralYes = new RadioButton { Text = "Yes", Location = new Point(20, 15), AutoSize = true };
            rdoCollateralNo = new RadioButton { Text = "No", Location = new Point(100, 15), AutoSize = true, Checked = true };

            panel6.Controls.Add(rdoCollateralYes);
            panel6.Controls.Add(rdoCollateralNo);

            contentPanel.Controls.Add(panel6);
            yPos += 60;

            // ===== GRACE PERIOD & LATE PAYMENT =====
            var lblSection7 = new Label
            {
                Text = "Grace Period & Late Payment",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection7);

            yPos += 25;

            var panel7 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Grace Period
            var lblGracePeriod = new Label { Text = "Grace Period: (days)", Location = new Point(10, 20), AutoSize = true };
            txtGracePeriod = new TextBox { Location = new Point(150, 17), Size = new Size(100, 25), Text = "0" };

            // Late Payment Penalty
            var lblLatePenalty = new Label { Text = "Late Payment Penalty (%):", Location = new Point(10, 60), AutoSize = true };
            txtLatePenalty = new TextBox { Location = new Point(150, 57), Size = new Size(80, 25), Text = "0.00" };

            var lblPer = new Label { Text = "% per", Location = new Point(240, 60), AutoSize = true };
            cmbLatePenaltyPeriod = new ComboBox
            {
                Location = new Point(290, 57),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLatePenaltyPeriod.Items.AddRange(new object[] { "Day", "Week", "Month" });
            cmbLatePenaltyPeriod.SelectedIndex = 0;

            panel7.Controls.Add(lblGracePeriod);
            panel7.Controls.Add(txtGracePeriod);
            panel7.Controls.Add(lblLatePenalty);
            panel7.Controls.Add(txtLatePenalty);
            panel7.Controls.Add(lblPer);
            panel7.Controls.Add(cmbLatePenaltyPeriod);

            contentPanel.Controls.Add(panel7);
            yPos += 110;

            // ===== STATUS =====
            var lblSection8 = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lblSection8);

            yPos += 25;

            var panel8 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 50),
                BorderStyle = BorderStyle.FixedSingle
            };

            rdoStatusActive = new RadioButton { Text = "Active", Location = new Point(20, 15), AutoSize = true, Checked = true };
            rdoStatusInactive = new RadioButton { Text = "Inactive", Location = new Point(100, 15), AutoSize = true };

            panel8.Controls.Add(rdoStatusActive);
            panel8.Controls.Add(rdoStatusInactive);

            contentPanel.Controls.Add(panel8);
            yPos += 60;

            // ===== ACTION BUTTONS =====
            var panelButtons = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(contentPanel.Width - 40, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSaveProduct = new Button
            {
                Text = "Save Product",
                Location = new Point(20, 15),
                Size = new Size(120, 35),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveProduct.FlatAppearance.BorderSize = 0;
            btnSaveProduct.Click += (s, e) => SaveProduct();
            panelButtons.Controls.Add(btnSaveProduct);

            btnClearForm = new Button
            {
                Text = "Clear Form",
                Location = new Point(160, 15),
                Size = new Size(120, 35),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnClearForm.FlatAppearance.BorderColor = Color.Gray;
            btnClearForm.FlatAppearance.BorderSize = 1;
            btnClearForm.Click += (s, e) => ClearForm();
            panelButtons.Controls.Add(btnClearForm);

            contentPanel.Controls.Add(panelButtons);

            // Update content panel height based on final yPos
            contentPanel.Height = yPos + 80;

            // Handle resize to adjust widths and icon position
            this.Resize += (s, e) =>
            {
                int newWidth = Math.Max(820, mainPanel.Width - 40);
                contentPanel.Width = newWidth;

                // Update all panel widths if they still exist in the control tree
                foreach (Control c in contentPanel.Controls)
                {
                    if (c is Panel p)
                        p.Width = newWidth - 40;
                }

                // Update the plus icon position
                if (lblPlusIcon != null)
                {
                    lblPlusIcon.Left = (newWidth - 20) - 40; // headerPanel width minus 40
                }
            };

            // Add content panel to main panel
            mainPanel.Controls.Add(contentPanel);

            // Add main panel to user control
            this.Controls.Clear();
            this.Controls.Add(mainPanel);
        }

        // NEW: public API for LoanProductsControl
        public void LoadProductForEdit(int productId)
        {
            _editingProductId = productId;

            using (var db = new AppDbContext())
            {
                var p = db.LoanProducts.AsNoTracking().FirstOrDefault(x => x.ProductId == productId);
                if (p == null)
                    throw new InvalidOperationException("Loan product not found.");

                // load extended columns + terms + requirements (raw sql / MySqlConnection)
                var ext = LoadExtended(productId);
                var terms = LoadTerms(productId);
                var reqs = LoadRequirements(productId);

                // Fill base fields
                txtLoanTypeName.Text = p.ProductName ?? "";
                txtDescription.Text = p.Description ?? "";

                txtMinLoanAmount.Text = p.MinAmount.ToString("0.##", CultureInfo.InvariantCulture);
                txtMaxLoanAmount.Text = p.MaxAmount.ToString("0.##", CultureInfo.InvariantCulture);

                txtInterestRate.Text = p.InterestRate.ToString("0.##", CultureInfo.InvariantCulture);
                txtServiceFeePercentage.Text = p.ProcessingFeePct.ToString("0.##", CultureInfo.InvariantCulture);

                txtGracePeriod.Text = p.GracePeriodDays.ToString(CultureInfo.InvariantCulture);
                txtLatePenalty.Text = p.PenaltyRate.ToString("0.##", CultureInfo.InvariantCulture);

                rdoStatusActive.Checked = p.IsActive;
                rdoStatusInactive.Checked = !p.IsActive;

                // Extended fields
                rdoFixedInterest.Checked = string.Equals(ext.InterestType, "Fixed", StringComparison.OrdinalIgnoreCase);
                rdoVariableInterest.Checked = !rdoFixedInterest.Checked;

                SelectComboItemOrDefault(cmbInterestPeriod, ext.InterestPeriod, "Month");
                SelectComboItemOrDefault(cmbLatePenaltyPeriod, ext.PenaltyPeriod, "Month");

                txtServiceFeeFixed.Text = ext.ServiceFeeFixedAmount.HasValue
                    ? ext.ServiceFeeFixedAmount.Value.ToString("0.##", CultureInfo.InvariantCulture)
                    : "0.00";

                rdoCollateralYes.Checked = ext.RequiresCollateral;
                rdoCollateralNo.Checked = !ext.RequiresCollateral;

                // Terms
                if (termCheckboxes != null)
                {
                    foreach (var cb in termCheckboxes)
                    {
                        if (cb == null) continue;
                        int m;
                        cb.Checked = int.TryParse(cb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out m) && terms.Contains(m);
                    }
                }

                // Requirements
                chkValidId.Checked = reqs.ContainsKey("ValidId");
                chkProofOfIncome.Checked = reqs.ContainsKey("ProofOfIncome");
                chkPayslip.Checked = reqs.ContainsKey("Payslip");
                chkBankStatement.Checked = reqs.ContainsKey("BankStatement");
                chkComakerForm.Checked = reqs.ContainsKey("ComakerForm");

                if (reqs.ContainsKey("Others"))
                {
                    chkOthers.Checked = true;
                    txtOthersDescription.Enabled = true;
                    txtOthersDescription.Text = reqs["Others"] ?? "";
                }
                else
                {
                    chkOthers.Checked = false;
                    txtOthersDescription.Enabled = false;
                    txtOthersDescription.Text = "";
                }
            }

            // Update UI labels/buttons for edit mode
            btnSaveProduct.Text = "Save Changes";
            if (lblPlusIcon != null) lblPlusIcon.Text = "✎";
        }

        private sealed class LoanProductExtended
        {
            public string InterestType { get; set; }
            public string InterestPeriod { get; set; }
            public decimal? ServiceFeeFixedAmount { get; set; }
            public string PenaltyPeriod { get; set; }
            public bool RequiresCollateral { get; set; }
        }

        private static LoanProductExtended LoadExtended(int productId)
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["LendingAppDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ValidationException("Missing connection string: LendingAppDb");

            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT 
  interest_type,
  interest_period,
  service_fee_fixed_amount,
  penalty_period,
  requires_collateral
FROM loan_products
WHERE product_id = @productId;";
                    cmd.Parameters.AddWithValue("@productId", productId);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return new LoanProductExtended();

                        return new LoanProductExtended
                        {
                            InterestType = r["interest_type"] as string,
                            InterestPeriod = r["interest_period"] as string,
                            ServiceFeeFixedAmount = r["service_fee_fixed_amount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["service_fee_fixed_amount"], CultureInfo.InvariantCulture),
                            PenaltyPeriod = r["penalty_period"] as string,
                            RequiresCollateral = r["requires_collateral"] != DBNull.Value && Convert.ToInt32(r["requires_collateral"], CultureInfo.InvariantCulture) == 1
                        };
                    }
                }
            }
        }

        private static HashSet<int> LoadTerms(int productId)
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["LendingAppDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ValidationException("Missing connection string: LendingAppDb");

            var set = new HashSet<int>();
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT term_months FROM loan_product_terms WHERE product_id = @productId;";
                    cmd.Parameters.AddWithValue("@productId", productId);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var v = Convert.ToInt32(r["term_months"], CultureInfo.InvariantCulture);
                            set.Add(v);
                        }
                    }
                }
            }
            return set;
        }

        private static Dictionary<string, string> LoadRequirements(int productId)
        {
            var cs = System.Configuration.ConfigurationManager.ConnectionStrings["LendingAppDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ValidationException("Missing connection string: LendingAppDb");

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT requirement_key, requirement_text
FROM loan_product_requirements
WHERE product_id = @productId;";
                    cmd.Parameters.AddWithValue("@productId", productId);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var key = (r["requirement_key"] as string) ?? "";
                            var text = r["requirement_text"] == DBNull.Value ? null : (r["requirement_text"] as string);
                            if (!string.IsNullOrWhiteSpace(key))
                                dict[key] = text;
                        }
                    }
                }
            }

            return dict;
        }

        private static void SelectComboItemOrDefault(ComboBox cmb, string value, string fallback)
        {
            if (cmb == null) return;

            var v = (value ?? "").Trim();
            if (string.IsNullOrWhiteSpace(v)) v = fallback;

            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (string.Equals(cmb.Items[i].ToString(), v, StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }

            // fallback
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (string.Equals(cmb.Items[i].ToString(), fallback, StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }

            if (cmb.Items.Count > 0)
                cmb.SelectedIndex = 0;
        }

        private void SaveProduct()
        {
            try
            {
                var req = new LoanProductCreateRequest
                {
                    ProductName = (txtLoanTypeName.Text ?? "").Trim(),
                    Description = NormalizeDescription(txtDescription.Text),

                    InterestType = rdoFixedInterest.Checked ? "Fixed" : "Variable",
                    InterestPeriod = (cmbInterestPeriod.SelectedItem ?? "Year").ToString(),

                    InterestRate = LoanProductAdminService.ParseDecimal(txtInterestRate.Text),

                    ServiceFeePct = LoanProductAdminService.ParseDecimal(txtServiceFeePercentage.Text),
                    ServiceFeeFixedAmount = NormalizeNullableMoney(txtServiceFeeFixed.Text),

                    MinAmount = LoanProductAdminService.ParseMoney(txtMinLoanAmount.Text),
                    MaxAmount = LoanProductAdminService.ParseMoney(txtMaxLoanAmount.Text),

                    SelectedTerms = GetSelectedTerms(),

                    GracePeriodDays = LoanProductAdminService.ParseInt(txtGracePeriod.Text),
                    PenaltyRatePct = LoanProductAdminService.ParseDecimal(txtLatePenalty.Text),
                    PenaltyPeriod = (cmbLatePenaltyPeriod.SelectedItem ?? "Month").ToString(),

                    RequiresCollateral = rdoCollateralYes.Checked,
                    IsActive = rdoStatusActive.Checked,

                    Requirements = GetSelectedRequirements()
                        .Select(x => new LoanProductRequirement { Key = x.key, Text = x.text })
                        .ToList()
                };

                var svc = new LoanProductAdminService();

                if (_editingProductId.HasValue)
                {
                    svc.UpdateLoanProduct(_editingProductId.Value, req);

                    MessageBox.Show("Product updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ProductSaved?.Invoke(_editingProductId.Value);
                }
                else
                {
                    var newId = svc.CreateLoanProduct(req);

                    MessageBox.Show("Product saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ProductSaved?.Invoke(newId);
                    ClearForm();
                }
            }
            catch (ValidationException vex)
            {
                MessageBox.Show(vex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to save product", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string NormalizeDescription(string text)
        {
            var s = (text ?? "").Trim();
            if (string.Equals(s, "Enter description", StringComparison.OrdinalIgnoreCase)) return null;
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        private static decimal? NormalizeNullableMoney(string text)
        {
            var v = LoanProductAdminService.ParseMoney(text);
            return v <= 0m ? (decimal?)null : v;
        }

        private void ClearForm()
        {
            txtLoanTypeName.Clear();
            txtDescription.Clear();
            rdoFixedInterest.Checked = true;
            txtInterestRate.Clear();
            if (cmbInterestPeriod.Items.Count > 0) cmbInterestPeriod.SelectedIndex = 0;
            txtServiceFeePercentage.Clear();
            txtServiceFeeFixed.Clear();
            txtMinLoanAmount.Clear();
            txtMaxLoanAmount.Clear();

            if (termCheckboxes != null)
            {
                foreach (var checkbox in termCheckboxes)
                {
                    if (checkbox != null) checkbox.Checked = false;
                }
            }

            chkValidId.Checked = false;
            chkProofOfIncome.Checked = false;
            chkPayslip.Checked = false;
            chkBankStatement.Checked = false;
            chkComakerForm.Checked = false;
            chkOthers.Checked = false;
            txtOthersDescription.Clear();
            txtOthersDescription.Enabled = false;

            rdoCollateralNo.Checked = true;
            txtGracePeriod.Clear();
            txtLatePenalty.Clear();
            if (cmbLatePenaltyPeriod.Items.Count > 0) cmbLatePenaltyPeriod.SelectedIndex = 0;
            rdoStatusActive.Checked = true;
        }

        private List<int> GetSelectedTerms()
        {
            var list = new List<int>();
            if (termCheckboxes == null) return list;

            foreach (var cb in termCheckboxes)
            {
                if (cb == null || !cb.Checked) continue;

                if (int.TryParse(cb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var months))
                    list.Add(months);
            }

            return list;
        }

        private List<(string key, string text)> GetSelectedRequirements()
        {
            var req = new List<(string key, string text)>();

            if (chkValidId != null && chkValidId.Checked) req.Add(("ValidId", null));
            if (chkProofOfIncome != null && chkProofOfIncome.Checked) req.Add(("ProofOfIncome", null));
            if (chkPayslip != null && chkPayslip.Checked) req.Add(("Payslip", null));
            if (chkBankStatement != null && chkBankStatement.Checked) req.Add(("BankStatement", null));
            if (chkComakerForm != null && chkComakerForm.Checked) req.Add(("ComakerForm", null));

            if (chkOthers != null && chkOthers.Checked)
            {
                var otherText = (txtOthersDescription?.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(otherText) ||
                    otherText.Equals("Specify other documents", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationException("Please specify the 'Others' required document.");
                }

                req.Add(("Others", otherText));
            }

            return req;
        }
    }
}