using LendingApp.Class.Models.CashierModels;
using LendingApp.LogicClass.Cashier;
using LendingApp.UI.CashierUI;
using PdfSharp.Fonts;
using System;
using System.Threading;
using System.Windows.Forms;
using System;

namespace LendingApp
{
    internal static class Program
    {
        private static Mutex _singleInstanceMutex;

        [STAThread]
        static void Main()
        {


            // Enable Windows font resolver for PDFsharp 6.2 when running on Windows.
            // Must be set before any XFont is constructed.
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            // Set a simple font resolver that delegates to the platform resolver.
            // This ensures requests for "Segoe UI" and other common Windows fonts are resolved.
            GlobalFontSettings.FontResolver = new PlatformDelegatingFontResolver();

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

        // Simple font resolver that calls the built-in PlatformFontResolver.
        // PdfSharp will call GetFont only for face names returned by ResolveTypeface
        // if those faces are provided by this resolver. We rely on PlatformFontResolver
        // to map the family name to a face; returning that result is supported.
        private sealed class PlatformDelegatingFontResolver : IFontResolver
        {
            public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
            {
                // Try platform resolver first (maps to installed Windows fonts)
                var info = PlatformFontResolver.ResolveTypeface(familyName, bold, italic);
                if (info != null)
                    return info;

                // Fallback to Arial (very likely present on Windows)
                return PlatformFontResolver.ResolveTypeface("Arial", bold, italic);
            }

            // Not used for PlatformFontResolver results; return null for other names.
            public byte[] GetFont(string faceName) => null;
        }
    }
}
