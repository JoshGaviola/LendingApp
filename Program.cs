using System;
using System.Threading;
using System.Windows.Forms;

namespace LendingApp
{
    internal static class Program
    {
        private static Mutex _singleInstanceMutex;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            _singleInstanceMutex = new Mutex(true, @"Global\LendingApp_SingleInstance", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("LendingApp is already running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
<<<<<<< HEAD
            //Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerApplications());
            Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());

            //Application.Run(new LendingApp.UI.CashierUI.CashierDashboard());


            // Choose one:
=======

            Application.Run(new LendingApp.UI.CashierUI.CashierDashboard());
>>>>>>> 6f46d1fc4fcabfc5b685e2f83c02ba46b2e81cac
        }
    }
}
