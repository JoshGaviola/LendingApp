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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
<<<<<<< HEAD
            Application.Run(new CustomerLogin());
=======
            Application.Run(new LendingApp.UI.CashierUI.CashierReleaseLoan());
>>>>>>> e9132d469b6846b74cc80e9f2252fa646b9211d8
        }
    }
}   
