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
            //Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerApplications());
            Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());

            //Application.Run(new LendingApp.UI.CashierUI.CashierDashboard());



            //Application.Run(new LendingApp.UI.CashierUI.CashierDashboard());
        }
    }
}
