using LendingApp.Class;
using LendingApp.Class.Models.User;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LendingApp.UI.AdminUI
{
    public partial class AdminLogin : Form
    {
        Label lblTitle, lblUsername, lblPassword;
        TextBox txtUsername, txtPassword;
        Button btnLogin;

        public AdminLogin()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Admin Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(300, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label()
            {
                Text = "ADMIN LOGIN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(85, 20)
            };

            lblUsername = new Label()
            {
                Text = "Username",
                Location = new Point(30, 60)
            };

            txtUsername = new TextBox()
            {
                Location = new Point(30, 80),
                Width = 220
            };

            lblPassword = new Label()
            {
                Text = "Password",
                Location = new Point(30, 110)
            };

            txtPassword = new TextBox()
            {
                Location = new Point(30, 130),
                Width = 220,
                PasswordChar = '*'
            };

            btnLogin = new Button()
            {
                Text = "Login",
                Location = new Point(30, 170),
                Width = 220
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);


            btnLogin.Click += btnLogin_Click;
        }

        public void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Enter username and password!");
                return;
            }
            try
            {
                using (var db = new AppDbContext())
                {
                    string sql = @"SELECT * FROM users 
                           WHERE username = @username
                             AND password_hash = @password_hash 
                             AND role = 'Admin' 
                             AND is_active = 1";
                             

                    var admin = db.Database.SqlQuery<User>(
                        sql,
                        new MySql.Data.MySqlClient.MySqlParameter("@username", username),
                        new MySql.Data.MySqlClient.MySqlParameter("@password_hash", password))
                        .FirstOrDefault();

                    if (admin != null)
                    {
                        MessageBox.Show($"Login successful! Welcome {admin.FirstName}");

                        this.Hide();
                        AdminDashboard dashboard = new AdminDashboard();
                        dashboard.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials or not an admin user.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}