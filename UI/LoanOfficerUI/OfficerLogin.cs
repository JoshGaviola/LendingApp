using LendingApp.Class;
using LendingApp.Class.LogicClass;
using LendingApp.Class.Models.User;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LendingApp.UI.CustomerUI;

namespace LendingApp.UI.LoanOfficerUI
{
    public partial class OfficerLogin : Form
    {
        Panel pnlLeft, pnlRight;
        Label lblTitle, lblSubtitle, lblUsername, lblPassword, lblWelcome, lblAppName;
        TextBox txtUsername, txtPassword;
        Button btnLogin;
        PictureBox picLogo;

        private CustomerRegistration _openRegistrationForm;
        private OfficerDashboard _openOfficerDashboardForm;

        public OfficerLogin()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Officer Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Left Panel - Brand/Welcome Section
            pnlLeft = new Panel()
            {
                Width = 350,
                Height = 500,
                Location = new Point(0, 0),
                BackColor = ColorTranslator.FromHtml("#2C3E50")
            };
            pnlLeft.Paint += PnlLeft_Paint;

            // Logo placeholder
            picLogo = new PictureBox()
            {
                Size = new Size(80, 80),
                Location = new Point(135, 100),
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            picLogo.Paint += PicLogo_Paint;

            lblAppName = new Label()
            {
                Text = "LENDING APP",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(95, 200)
            };

            lblWelcome = new Label()
            {
                Text = "Officer Portal\nSecure Access Only",
                Font = new Font("Segoe UI", 11),
                ForeColor = ColorTranslator.FromHtml("#BDC3C7"),
                AutoSize = false,
                Size = new Size(300, 60),
                Location = new Point(70, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlLeft.Controls.Add(picLogo);
            pnlLeft.Controls.Add(lblAppName);
            pnlLeft.Controls.Add(lblWelcome);

            // Right Panel - Login Form
            pnlRight = new Panel()
            {
                Width = 450,
                Height = 500,
                Location = new Point(350, 0),
                BackColor = Color.White
            };

            // Close button
            Button btnClose = new Button()
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                Size = new Size(40, 40),
                Location = new Point(400, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.Red;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = ColorTranslator.FromHtml("#7F8C8D");

            lblTitle = new Label()
            {
                Text = "Officer Login",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                AutoSize = true,
                Location = new Point(60, 80)
            };

            lblSubtitle = new Label()
            {
                Text = "Enter your credentials to continue",
                Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                AutoSize = true,
                Location = new Point(60, 120)
            };

            lblUsername = new Label()
            {
                Text = "USERNAME",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                AutoSize = true,
                Location = new Point(60, 180)
            };

            txtUsername = new TextBox()
            {
                Location = new Point(60, 205),
                Width = 330,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = ColorTranslator.FromHtml("#ECF0F1")
            };

            Panel pnlUsername = new Panel()
            {
                Location = new Point(60, 205),
                Width = 330,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#ECF0F1")
            };
            pnlUsername.Controls.Add(txtUsername);
            txtUsername.Location = new Point(10, 10);
            txtUsername.Width = 310;

            lblPassword = new Label()
            {
                Text = "PASSWORD",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#7F8C8D"),
                AutoSize = true,
                Location = new Point(60, 260)
            };

            txtPassword = new TextBox()
            {
                Location = new Point(60, 285),
                Width = 330,
                Height = 40,
                Font = new Font("Segoe UI", 11),
                PasswordChar = '●',
                BorderStyle = BorderStyle.None,
                BackColor = ColorTranslator.FromHtml("#ECF0F1")
            };

            Panel pnlPassword = new Panel()
            {
                Location = new Point(60, 285),
                Width = 330,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#ECF0F1")
            };
            pnlPassword.Controls.Add(txtPassword);
            txtPassword.Location = new Point(10, 10);
            txtPassword.Width = 310;

            btnLogin = new Button()
            {
                Text = "LOGIN",
                Location = new Point(60, 360),
                Width = 330,
                Height = 45,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#3498DB"),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += BtnLogin_MouseEnter;
            btnLogin.MouseLeave += BtnLogin_MouseLeave;
            btnLogin.Click += SignInBtn_Click_1;

            pnlRight.Controls.Add(btnClose);
            pnlRight.Controls.Add(lblTitle);
            pnlRight.Controls.Add(lblSubtitle);
            pnlRight.Controls.Add(lblUsername);
            pnlRight.Controls.Add(pnlUsername);
            pnlRight.Controls.Add(lblPassword);
            pnlRight.Controls.Add(pnlPassword);
            pnlRight.Controls.Add(btnLogin);

            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlRight);

            // Allow Enter key to submit
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    SignInBtn_Click_1(btnLogin, EventArgs.Empty);
                }
            };
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            // Add gradient background
            LinearGradientBrush brush = new LinearGradientBrush(
                pnlLeft.ClientRectangle,
                ColorTranslator.FromHtml("#2C3E50"),
                ColorTranslator.FromHtml("#34495E"),
                90F);
            e.Graphics.FillRectangle(brush, pnlLeft.ClientRectangle);
        }

        private void PicLogo_Paint(object sender, PaintEventArgs e)
        {
            // Draw a simple shield icon
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(ColorTranslator.FromHtml("#3498DB"), 3))
            using (Brush brush = new SolidBrush(ColorTranslator.FromHtml("#3498DB")))
            {
                Point[] shield = new Point[]
                {
                    new Point(40, 15),
                    new Point(65, 15),
                    new Point(65, 35),
                    new Point(55, 50),
                    new Point(40, 35)
                };
                e.Graphics.DrawPolygon(pen, shield);
                e.Graphics.DrawLine(pen, 45, 30, 50, 35);
                e.Graphics.DrawLine(pen, 50, 35, 60, 25);
            }
        }

        private void BtnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = ColorTranslator.FromHtml("#2980B9");
        }

        private void BtnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = ColorTranslator.FromHtml("#3498DB");
        }

        private void PanelLeftSideCustLogin_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = ((Panel)sender).ClientRectangle;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                ColorTranslator.FromHtml("#2C3E50"),
                ColorTranslator.FromHtml("#3498DB"),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            if (_openRegistrationForm == null || _openRegistrationForm.IsDisposed)
            {
                _openRegistrationForm = new CustomerRegistration();
                _openRegistrationForm.FormClosed += (s, args) => _openRegistrationForm = null;
                _openRegistrationForm.Show(this);
            }
            else
            {
                _openRegistrationForm.Focus();
            }
        }

        private void SignInBtn_Click_1(object sender, EventArgs e)
        {

            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string role = "LoanOfficer";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LendingApp.Class.Services.DynamicLogin loginService = new LendingApp.Class.Services.DynamicLogin();

            User user = loginService.Authenticate(username, password, role);

           if(user != null)
            {
                this.Hide();
                OfficerDashboard officerDashboard = new OfficerDashboard();
                officerDashboard.Show();
            }
            else
            {
                MessageBox.Show("Invalid credentials or inactive account.", "Login Failed",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}