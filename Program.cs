using LendingApp.LogicClass.Cashier;
using LendingApp.Models.CashierModels;
using LendingApp.UI.CashierUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            var data = new DataSample();
<<<<<<< HEAD
            //Application.Run(new LendingApp.UI.CashierUI.CashierDashboard(data));
             Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());
=======
           Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());

            //Application.Run(new LendingApp.UI.AdminUI.AdminDashboard());
            //Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());

>>>>>>> 024ba6a64cd828dd254940d307284b2e15a30d32

        }
    }
}
