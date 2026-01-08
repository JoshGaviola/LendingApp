using LendingApp.Class.Models.CashierModels;
using LendingApp.LogicClass.Cashier;
using LendingApp.UI.CashierUI;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
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


            // Allow PDFsharp 6.x to resolve common Windows fonts.
            // Must be set before any XFont is created.
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            bool createdNew;
            _singleInstanceMutex = new Mutex(true, @"Global\LendingApp_SingleInstance", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("LendingApp is already running.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }

            // e run ni sa console bai, e copy tapos sa sql ilisdi ang admin123 og ang hash nga ma generate diri
            string hash = BCrypt.Net.BCrypt.HashPassword("123456");
            Console.WriteLine(hash);
            Console.ReadLine();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
           // Application.Run(new LendingApp.UI.AdminUI.AdminLogin());
           // Application.Run(new LendingApp.UI.CashierUI.CashierLogin());
             Application.Run(new LendingApp.UI.LoanOfficerUI.OfficerLogin());

        }
    }
}
