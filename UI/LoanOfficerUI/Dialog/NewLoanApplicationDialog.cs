using LendingApp.Class;
using LendingApp.Class.Models.Loans;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class NewLoanApplicationDialog : Form
    {
        private int currentStep = 1;
        private TabControl tabControl;
        private Button btnBack;
        private Button btnNext;
        private Button btnCancel;
        private Button btnSubmit;
        private ComboBox cmbLoanType;
        private TextBox txtAmount;
        private TextBox txtLoanPurpose;
        private DateTimePicker dtpReleaseDate;
        private ComboBox cmbCoMakerRelationship;
        private TextBox txtCoMakerName;
        private TextBox txtCoMakerContact;
        private ComboBox cmbCollateralType;
        private TextBox txtCollateralDescription;
        private TextBox txtCollateralValue;
        private CheckBox chkValidID;
        private CheckBox chkProofIncome;
        private CheckBox chkBankStatement;
        private CheckBox chkCOE;
        private Label stepLabel;

        public NewLoanApplicationDialog()
        {
            // Call InitializeComponent FIRST - this creates all controls
            InitializeComponent();

            // Then configure/initialize them
            ConfigureUI();
        }

        private void ConfigureUI()
        {
            // Ensure grid columns exist (designer doesn't define them)
            if (customerGrid.Columns.Count == 0)
                SetupCustomerGrid();

            // Defaults (only if items exist)
            if (cmbLoanType.Items.Count > 0) cmbLoanType.SelectedIndex = 0;
            if (cmbCoMakerRelationship.Items.Count > 0) cmbCoMakerRelationship.SelectedIndex = 0;
            if (cmbCollateralType.Items.Count > 0) cmbCollateralType.SelectedIndex = 0;

            // Load real customers from DB instead of sample rows
            LoadCustomersFromDb();

            // Search behavior
            searchBtn.Click += (s, e) => LoadCustomersFromDb(searchBox.Text);

            btnBack.Click += (s, e) => GoToStep(currentStep - 1);
            btnCancel.Click += (s, e) => Close();
            btnNext.Click += (s, e) => GoToStep(currentStep + 1);

            // Keep both routes safe (designer already wires btnSubmit_Click)
            btnSubmit.Click += (s, e) => SubmitApplication();

            UpdateStepUI();
        }

        private void SetupCustomerGrid()
        {
            customerGrid.Columns.Clear();
            customerGrid.Columns.Add("ID", "ID");
            customerGrid.Columns.Add("Name", "Name");
            customerGrid.Columns.Add("Score", "Credit Score");
            customerGrid.Columns.Add("Class", "Classification");
        }

        private void LoadCustomersFromDb(string search = null)
        {
            customerGrid.Rows.Clear();

            using (var db = new AppDbContext())
            {
                IQueryable<LendingApp.Class.Models.LoanOfiicerModels.CustomerRegistrationData> query =
                    db.Customers.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim();
                    query = query.Where(c =>
                        c.CustomerId.Contains(s) ||
                        c.FirstName.Contains(s) ||
                        c.LastName.Contains(s));
                }

                var customers = query
                    .OrderByDescending(c => c.RegistrationDate)
                    .Select(c => new
                    {
                        c.CustomerId,
                        Name = (c.FirstName ?? "") + " " + (c.LastName ?? ""),
                        c.InitialCreditScore,
                        c.CustomerType
                    })
                    .ToList();

                foreach (var c in customers)
                {
                    customerGrid.Rows.Add(
                        c.CustomerId,
                        c.Name.Trim(),
                        c.InitialCreditScore.ToString(CultureInfo.InvariantCulture),
                        c.CustomerType
                    );
                }
            }
        }

        private void GoToStep(int step)
        {
            if (step < 1 || step > 3) return;

            currentStep = step;
            tabControl.SelectedIndex = step - 1;
            UpdateStepUI();
        }

        private void UpdateStepUI()
        {
            string[] stepLabels =
            {
                "Step 1: Select Customer",
                "Step 2: Loan Details",
                "Step 3: Co-maker & Documents"
            };

            stepLabel.Text = stepLabels[currentStep - 1];

            btnBack.Enabled = currentStep > 1;
            btnNext.Visible = currentStep < 3;
            btnSubmit.Visible = currentStep == 3;
        }

        private void SubmitApplication()
        {
            if (customerGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(1);
                return;
            }

            decimal requestedAmount;
            if (!TryParseCurrency(txtAmount.Text, out requestedAmount) || requestedAmount <= 0)
            {
                MessageBox.Show("Please enter a valid requested amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(2);
                return;
            }

            var customerId = customerGrid.SelectedRows[0].Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show("Invalid customer selected.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(1);
                return;
            }

            int productId = MapLoanTypeToProductId(cmbLoanType.Text);
            if (productId <= 0)
            {
                MessageBox.Show(
                    "Loan type is not mapped to a valid product_id.\nUpdate MapLoanTypeToProductId() to match your loan_products table.",
                    "Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Required by your schema, but not currently in the UI:
            // add a real UI field later (NumericUpDown/ComboBox).
            int preferredTerm = 12;

            var now = DateTime.Now;
            var entity = new LoanApplicationEntity
            {
                ApplicationNumber = GenerateApplicationNumber(now),
                CustomerId = customerId,
                ProductId = productId,
                RequestedAmount = requestedAmount,
                PreferredTerm = preferredTerm,
                Purpose = string.IsNullOrWhiteSpace(txtLoanPurpose.Text) ? null : txtLoanPurpose.Text.Trim(),
                DesiredReleaseDate = dtpReleaseDate.Value.Date,

                Status = "Pending",
                Priority = "Medium",

                ApplicationDate = now,
                StatusDate = now
            };

            try
            {
                using (var db = new AppDbContext())
                {
                    db.LoanApplications.Add(entity);
                    db.SaveChanges();
                }

                MessageBox.Show("Loan application saved successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save loan application.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static bool TryParseCurrency(string text, out decimal value)
        {
            value = 0m;
            var cleaned = (text ?? "").Trim().Replace("₱", "").Replace(",", "");
            return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
        }

        private static string GenerateApplicationNumber(DateTime now)
        {
            // varchar(20) safe: "APP-20260103-153045"
            return "APP-" + now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        }

        private static int MapLoanTypeToProductId(string loanTypeText)
        {
            // IMPORTANT: replace with your real loan_products.product_id values
            switch ((loanTypeText ?? "").Trim())
            {
                case "Personal Loan": return 1;
                case "Salary Loan": return 2;
                case "Business Loan": return 3;
                default: return 0;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Designer-wired handler; keep it calling the same logic
            SubmitApplication();
        }
    }
}