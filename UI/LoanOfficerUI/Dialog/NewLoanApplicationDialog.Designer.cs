using System.Windows.Forms;

namespace LendingApp.UI.LoanOfficerUI
{
    partial class NewLoanApplicationDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.customerGrid = new System.Windows.Forms.DataGridView();
            this.searchBtn = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dtpReleaseDate = new System.Windows.Forms.DateTimePicker();
            this.dateLabel = new System.Windows.Forms.Label();
            this.txtLoanPurpose = new System.Windows.Forms.TextBox();
            this.purposeLabel = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.amountLabel = new System.Windows.Forms.Label();
            this.cmbLoanType = new System.Windows.Forms.ComboBox();
            this.loanTypeLabel = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkCOE = new System.Windows.Forms.CheckBox();
            this.chkBankStatement = new System.Windows.Forms.CheckBox();
            this.chkProofIncome = new System.Windows.Forms.CheckBox();
            this.chkValidID = new System.Windows.Forms.CheckBox();
            this.docLabel = new System.Windows.Forms.Label();
            this.txtCollateralValue = new System.Windows.Forms.TextBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.txtCollateralDescription = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.cmbCollateralType = new System.Windows.Forms.ComboBox();
            this.collateralTypeLabel = new System.Windows.Forms.Label();
            this.collateralLabel = new System.Windows.Forms.Label();
            this.txtCoMakerContact = new System.Windows.Forms.TextBox();
            this.contactLabel = new System.Windows.Forms.Label();
            this.cmbCoMakerRelationship = new System.Windows.Forms.ComboBox();
            this.relationshipLabel = new System.Windows.Forms.Label();
            this.txtCoMakerName = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.coMakerLabel = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.stepLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customerGrid)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.tabControl.Location = new System.Drawing.Point(20, 85);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(640, 320);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.customerGrid);
            this.tabPage1.Controls.Add(this.searchBtn);
            this.tabPage1.Controls.Add(this.searchBox);
            this.tabPage1.Controls.Add(this.searchLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(632, 311);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Customer Selection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // customerGrid
            // 
            this.customerGrid.AllowUserToAddRows = false;
            this.customerGrid.AllowUserToDeleteRows = false;
            this.customerGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.customerGrid.Location = new System.Drawing.Point(13, 36);
            this.customerGrid.Name = "customerGrid";
            this.customerGrid.ReadOnly = true;
            this.customerGrid.RowHeadersVisible = false;
            this.customerGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.customerGrid.Size = new System.Drawing.Size(389, 200);
            this.customerGrid.TabIndex = 3;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(340, 10);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(80, 25);
            this.searchBtn.TabIndex = 2;
            this.searchBtn.Text = "Search";
            this.searchBtn.UseVisualStyleBackColor = true;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(130, 10);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(200, 20);
            this.searchBox.TabIndex = 1;
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(10, 13);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(91, 13);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search Customer:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dtpReleaseDate);
            this.tabPage2.Controls.Add(this.dateLabel);
            this.tabPage2.Controls.Add(this.txtLoanPurpose);
            this.tabPage2.Controls.Add(this.purposeLabel);
            this.tabPage2.Controls.Add(this.txtAmount);
            this.tabPage2.Controls.Add(this.amountLabel);
            this.tabPage2.Controls.Add(this.cmbLoanType);
            this.tabPage2.Controls.Add(this.loanTypeLabel);
            this.tabPage2.Location = new System.Drawing.Point(4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(632, 311);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Loan Details";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dtpReleaseDate
            // 
            this.dtpReleaseDate.Location = new System.Drawing.Point(120, 165);
            this.dtpReleaseDate.Name = "dtpReleaseDate";
            this.dtpReleaseDate.Size = new System.Drawing.Size(200, 20);
            this.dtpReleaseDate.TabIndex = 7;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Location = new System.Drawing.Point(10, 168);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(75, 13);
            this.dateLabel.TabIndex = 6;
            this.dateLabel.Text = "Release Date:";
            // 
            // txtLoanPurpose
            // 
            this.txtLoanPurpose.Location = new System.Drawing.Point(120, 105);
            this.txtLoanPurpose.Multiline = true;
            this.txtLoanPurpose.Name = "txtLoanPurpose";
            this.txtLoanPurpose.Size = new System.Drawing.Size(300, 50);
            this.txtLoanPurpose.TabIndex = 5;
            // 
            // purposeLabel
            // 
            this.purposeLabel.AutoSize = true;
            this.purposeLabel.Location = new System.Drawing.Point(10, 108);
            this.purposeLabel.Name = "purposeLabel";
            this.purposeLabel.Size = new System.Drawing.Size(49, 13);
            this.purposeLabel.TabIndex = 4;
            this.purposeLabel.Text = "Purpose:";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(120, 65);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(200, 20);
            this.txtAmount.TabIndex = 3;
            this.txtAmount.Text = "₱50,000";
            // 
            // amountLabel
            // 
            this.amountLabel.AutoSize = true;
            this.amountLabel.Location = new System.Drawing.Point(10, 68);
            this.amountLabel.Name = "amountLabel";
            this.amountLabel.Size = new System.Drawing.Size(46, 13);
            this.amountLabel.TabIndex = 2;
            this.amountLabel.Text = "Amount:";
            // 
            // cmbLoanType
            // 
            this.cmbLoanType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLoanType.FormattingEnabled = true;
            this.cmbLoanType.Items.AddRange(new object[] {
            "Personal Loan",
            "Salary Loan",
            "Business Loan"});
            this.cmbLoanType.Location = new System.Drawing.Point(120, 25);
            this.cmbLoanType.Name = "cmbLoanType";
            this.cmbLoanType.Size = new System.Drawing.Size(200, 21);
            this.cmbLoanType.TabIndex = 1;
            // 
            // loanTypeLabel
            // 
            this.loanTypeLabel.AutoSize = true;
            this.loanTypeLabel.Location = new System.Drawing.Point(10, 28);
            this.loanTypeLabel.Name = "loanTypeLabel";
            this.loanTypeLabel.Size = new System.Drawing.Size(61, 13);
            this.loanTypeLabel.TabIndex = 0;
            this.loanTypeLabel.Text = "Loan Type:";
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.chkCOE);
            this.tabPage3.Controls.Add(this.chkBankStatement);
            this.tabPage3.Controls.Add(this.chkProofIncome);
            this.tabPage3.Controls.Add(this.chkValidID);
            this.tabPage3.Controls.Add(this.docLabel);
            this.tabPage3.Controls.Add(this.txtCollateralValue);
            this.tabPage3.Controls.Add(this.valueLabel);
            this.tabPage3.Controls.Add(this.txtCollateralDescription);
            this.tabPage3.Controls.Add(this.descLabel);
            this.tabPage3.Controls.Add(this.cmbCollateralType);
            this.tabPage3.Controls.Add(this.collateralTypeLabel);
            this.tabPage3.Controls.Add(this.collateralLabel);
            this.tabPage3.Controls.Add(this.txtCoMakerContact);
            this.tabPage3.Controls.Add(this.contactLabel);
            this.tabPage3.Controls.Add(this.cmbCoMakerRelationship);
            this.tabPage3.Controls.Add(this.relationshipLabel);
            this.tabPage3.Controls.Add(this.txtCoMakerName);
            this.tabPage3.Controls.Add(this.nameLabel);
            this.tabPage3.Controls.Add(this.coMakerLabel);
            this.tabPage3.Location = new System.Drawing.Point(4, 5);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(632, 311);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Co-maker & Documents";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkCOE
            // 
            this.chkCOE.AutoSize = true;
            this.chkCOE.Location = new System.Drawing.Point(10, 400);
            this.chkCOE.Name = "chkCOE";
            this.chkCOE.Size = new System.Drawing.Size(48, 17);
            this.chkCOE.TabIndex = 18;
            this.chkCOE.Text = "COE";
            this.chkCOE.UseVisualStyleBackColor = true;
            // 
            // chkBankStatement
            // 
            this.chkBankStatement.AutoSize = true;
            this.chkBankStatement.Location = new System.Drawing.Point(10, 380);
            this.chkBankStatement.Name = "chkBankStatement";
            this.chkBankStatement.Size = new System.Drawing.Size(102, 17);
            this.chkBankStatement.TabIndex = 17;
            this.chkBankStatement.Text = "Bank Statement";
            this.chkBankStatement.UseVisualStyleBackColor = true;
            // 
            // chkProofIncome
            // 
            this.chkProofIncome.AutoSize = true;
            this.chkProofIncome.Location = new System.Drawing.Point(10, 360);
            this.chkProofIncome.Name = "chkProofIncome";
            this.chkProofIncome.Size = new System.Drawing.Size(101, 17);
            this.chkProofIncome.TabIndex = 16;
            this.chkProofIncome.Text = "Proof of Income";
            this.chkProofIncome.UseVisualStyleBackColor = true;
            // 
            // chkValidID
            // 
            this.chkValidID.AutoSize = true;
            this.chkValidID.Location = new System.Drawing.Point(10, 340);
            this.chkValidID.Name = "chkValidID";
            this.chkValidID.Size = new System.Drawing.Size(63, 17);
            this.chkValidID.TabIndex = 15;
            this.chkValidID.Text = "Valid ID";
            this.chkValidID.UseVisualStyleBackColor = true;
            // 
            // docLabel
            // 
            this.docLabel.AutoSize = true;
            this.docLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.docLabel.Location = new System.Drawing.Point(10, 320);
            this.docLabel.Name = "docLabel";
            this.docLabel.Size = new System.Drawing.Size(120, 13);
            this.docLabel.TabIndex = 14;
            this.docLabel.Text = "Document Checklist";
            // 
            // txtCollateralValue
            // 
            this.txtCollateralValue.Location = new System.Drawing.Point(120, 285);
            this.txtCollateralValue.Name = "txtCollateralValue";
            this.txtCollateralValue.Size = new System.Drawing.Size(200, 20);
            this.txtCollateralValue.TabIndex = 13;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(10, 288);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(37, 13);
            this.valueLabel.TabIndex = 12;
            this.valueLabel.Text = "Value:";
            // 
            // txtCollateralDescription
            // 
            this.txtCollateralDescription.Location = new System.Drawing.Point(120, 225);
            this.txtCollateralDescription.Multiline = true;
            this.txtCollateralDescription.Name = "txtCollateralDescription";
            this.txtCollateralDescription.Size = new System.Drawing.Size(300, 50);
            this.txtCollateralDescription.TabIndex = 11;
            // 
            // descLabel
            // 
            this.descLabel.AutoSize = true;
            this.descLabel.Location = new System.Drawing.Point(10, 228);
            this.descLabel.Name = "descLabel";
            this.descLabel.Size = new System.Drawing.Size(63, 13);
            this.descLabel.TabIndex = 10;
            this.descLabel.Text = "Description:";
            // 
            // cmbCollateralType
            // 
            this.cmbCollateralType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCollateralType.FormattingEnabled = true;
            this.cmbCollateralType.Items.AddRange(new object[] {
            "Real Estate",
            "Vehicle",
            "Jewelry",
            "Equipment",
            "Other"});
            this.cmbCollateralType.Location = new System.Drawing.Point(120, 185);
            this.cmbCollateralType.Name = "cmbCollateralType";
            this.cmbCollateralType.Size = new System.Drawing.Size(200, 21);
            this.cmbCollateralType.TabIndex = 9;
            // 
            // collateralTypeLabel
            // 
            this.collateralTypeLabel.AutoSize = true;
            this.collateralTypeLabel.Location = new System.Drawing.Point(10, 188);
            this.collateralTypeLabel.Name = "collateralTypeLabel";
            this.collateralTypeLabel.Size = new System.Drawing.Size(34, 13);
            this.collateralTypeLabel.TabIndex = 8;
            this.collateralTypeLabel.Text = "Type:";
            // 
            // collateralLabel
            // 
            this.collateralLabel.AutoSize = true;
            this.collateralLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.collateralLabel.Location = new System.Drawing.Point(10, 165);
            this.collateralLabel.Name = "collateralLabel";
            this.collateralLabel.Size = new System.Drawing.Size(103, 13);
            this.collateralLabel.TabIndex = 7;
            this.collateralLabel.Text = "Collateral (if any)";
            // 
            // txtCoMakerContact
            // 
            this.txtCoMakerContact.Location = new System.Drawing.Point(120, 125);
            this.txtCoMakerContact.Name = "txtCoMakerContact";
            this.txtCoMakerContact.Size = new System.Drawing.Size(200, 20);
            this.txtCoMakerContact.TabIndex = 6;
            // 
            // contactLabel
            // 
            this.contactLabel.AutoSize = true;
            this.contactLabel.Location = new System.Drawing.Point(10, 128);
            this.contactLabel.Name = "contactLabel";
            this.contactLabel.Size = new System.Drawing.Size(47, 13);
            this.contactLabel.TabIndex = 5;
            this.contactLabel.Text = "Contact:";
            // 
            // cmbCoMakerRelationship
            // 
            this.cmbCoMakerRelationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoMakerRelationship.FormattingEnabled = true;
            this.cmbCoMakerRelationship.Items.AddRange(new object[] {
            "Relative",
            "Spouse",
            "Friend",
            "Colleague",
            "Business Partner"});
            this.cmbCoMakerRelationship.Location = new System.Drawing.Point(120, 85);
            this.cmbCoMakerRelationship.Name = "cmbCoMakerRelationship";
            this.cmbCoMakerRelationship.Size = new System.Drawing.Size(200, 21);
            this.cmbCoMakerRelationship.TabIndex = 4;
            // 
            // relationshipLabel
            // 
            this.relationshipLabel.AutoSize = true;
            this.relationshipLabel.Location = new System.Drawing.Point(10, 88);
            this.relationshipLabel.Name = "relationshipLabel";
            this.relationshipLabel.Size = new System.Drawing.Size(68, 13);
            this.relationshipLabel.TabIndex = 3;
            this.relationshipLabel.Text = "Relationship:";
            // 
            // txtCoMakerName
            // 
            this.txtCoMakerName.Location = new System.Drawing.Point(120, 45);
            this.txtCoMakerName.Name = "txtCoMakerName";
            this.txtCoMakerName.Size = new System.Drawing.Size(200, 20);
            this.txtCoMakerName.TabIndex = 2;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(10, 48);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(38, 13);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Name:";
            // 
            // coMakerLabel
            // 
            this.coMakerLabel.AutoSize = true;
            this.coMakerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coMakerLabel.Location = new System.Drawing.Point(10, 25);
            this.coMakerLabel.Name = "coMakerLabel";
            this.coMakerLabel.Size = new System.Drawing.Size(103, 13);
            this.coMakerLabel.TabIndex = 0;
            this.coMakerLabel.Text = "Co-maker Details";
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(20, 415);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(80, 30);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "← Back";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(500, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.Location = new System.Drawing.Point(580, 415);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(80, 30);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next →";
            this.btnNext.UseVisualStyleBackColor = false;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(580, 415);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(80, 30);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Visible = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.stepLabel.Location = new System.Drawing.Point(20, 55);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(138, 15);
            this.stepLabel.TabIndex = 5;
            this.stepLabel.Text = "Step 1: Select Customer";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.titleLabel.Location = new System.Drawing.Point(20, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(297, 20);
            this.titleLabel.TabIndex = 6;
            this.titleLabel.Text = "NEW LOAN APPLICATION WIZARD";
            // 
            // NewLoanApplicationDialog
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(680, 480);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.stepLabel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewLoanApplicationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Loan Application Wizard";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.customerGrid)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Declare all controls that need to be accessed from code
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Label searchLabel;
        private TextBox searchBox;
        private Button searchBtn;
        private Label loanTypeLabel;
        private Label amountLabel;
        private Label purposeLabel;
        private Label dateLabel;
        private Label coMakerLabel;
        private Label nameLabel;
        private Label relationshipLabel;
        private Label contactLabel;
        private Label collateralLabel;
        private Label collateralTypeLabel;
        private Label descLabel;
        private Label valueLabel;
        private Label docLabel;
        private Label titleLabel;
        private DataGridView customerGrid;
    }
}