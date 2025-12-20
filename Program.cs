using LendingApp.UI.CustomerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LendingApp
{
    internal static class Program
    {
 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new LendingApp.UI.CashierUI.CashierDashboard());

        }
    }
}   
