using System;
using System.Drawing;
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
            // Set default values and events
            cmbLoanType.SelectedIndex = 0;
            cmbCoMakerRelationship.SelectedIndex = 0;
            cmbCollateralType.SelectedIndex = 0;

            // Setup grid columns if not already done
            if (customerGrid.Columns.Count == 0)
            {
                SetupCustomerGrid();
            }

            // Add sample data to grid
            AddSampleData();

            // Wire up events
            btnBack.Click += (s, e) => GoToStep(currentStep - 1);
            btnCancel.Click += (s, e) => this.Close();
            btnNext.Click += (s, e) => GoToStep(currentStep + 1);
            btnSubmit.Click += (s, e) => SubmitApplication();

            // Set initial step
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

        private void AddSampleData()
        {
            customerGrid.Rows.Clear();
            customerGrid.Rows.Add("CUST-001", "Juan Dela Cruz", "85", "Regular");
            customerGrid.Rows.Add("CUST-002", "Maria Santos", "72", "Regular");
            customerGrid.Rows.Add("CUST-003", "Pedro Reyes", "91", "VIP");
            customerGrid.Rows.Add("CUST-004", "Ana Lopez", "65", "Regular");
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
            // Update step label
            string[] stepLabels = {
                "Step 1: Select Customer",
                "Step 2: Loan Details",
                "Step 3: Co-maker & Documents"
            };

            if (stepLabel != null)
                stepLabel.Text = stepLabels[currentStep - 1];

            // Update buttons
            btnBack.Enabled = currentStep > 1;
            btnNext.Visible = currentStep < 3;
            btnSubmit.Visible = currentStep == 3;
        }

        private void SubmitApplication()
        {
            // Simple validation
            if (customerGrid.SelectedRows.Count == 0 && currentStep == 1)
            {
                MessageBox.Show("Please select a customer.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(1);
                return;
            }

            if (string.IsNullOrEmpty(cmbLoanType.Text) && currentStep == 2)
            {
                MessageBox.Show("Please select a loan type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GoToStep(2);
                return;
            }

            MessageBox.Show("Loan application submitted successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}