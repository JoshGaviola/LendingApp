using LendingApp.LogicClass.Cashier;
using LendingApp.Class.Models.CashierModels;
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

            var data = new ApplicantsData();
            // Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());
            Application.Run(new LendingApp.UI.CashierUI.CashierLogin());
           // Application.Run(new LendingApp.UI.AdminUI.AdminDashboard());
          //  Application.Run(new LendingApp.UI.CashierUI.CashierDashboard(data));

          //  Application.Run(new LendingApp.UI.AdminUI.AdminDashboard());

           //  Application.Run(new LendingApp.UI.CashierUI.CashierDashboard(data));
            //  Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerDashboard());
            // Application.Run(new LendingApp.UI.AdminUI.AdminDashboard());
        }
    }
}
