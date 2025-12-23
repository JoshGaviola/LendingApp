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
           Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());
            //Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerCollections());

=======
            Application.Run(new LendingApp.UI.AdminUI.AdminDashboard());
>>>>>>> 028db4dd3751d8eb9e508f4fd681719c46aea516
        }
    }
}
