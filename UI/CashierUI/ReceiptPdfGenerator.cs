using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
                
namespace LendingApp.UI.CashierUI
{
    public static class ReceiptPdfGenerator
    {
        // Generate a simple receipt PDF and return saved file path.
        public static string GeneratePdf(
            string receiptNo,
            DateTime date,
            string time,
            string customer,
            string loanAccount,
            decimal principal,
            decimal interest,
            decimal penalty,
            decimal total,
            string paymentMode,
            string cashier)
        {
            if (string.IsNullOrWhiteSpace(receiptNo)) receiptNo = "N/A";
            var pdf = new PdfDocument();
            pdf.Info.Title = "Official Receipt " + receiptNo;
            var page = pdf.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            page.Orientation = PdfSharp.PageOrientation.Portrait;

            var gfx = XGraphics.FromPdfPage(page);
            // Use common Windows font families so the PlatformFontResolver can find them
            // (we enabled GlobalFontSettings.UseWindowsFontsUnderWindows in Program.Main).
            var headerFont = new XFont("Arial", XUnit.FromPoint(14));
            var normalFont = new XFont("Arial", XUnit.FromPoint(10));
            var boldFont = new XFont("Arial", XUnit.FromPoint(10));   // Platform resolver will map bold if requested
            var mono = new XFont("Courier New", XUnit.FromPoint(9));

            double left = 40;
            double y = 40;
            double pageWidth = page.Width.Point;

            // Header
            gfx.DrawString("OFFICIAL RECEIPT", headerFont, XBrushes.Black, new XRect(left, y, pageWidth - 80, 24), XStringFormats.TopCenter);
            y += 36;

            // Company / meta
            gfx.DrawString("Lending Company Name", normalFont, XBrushes.Gray, new XRect(left, y, pageWidth - 80, 18), XStringFormats.TopCenter);
            y += 24;

            // Receipt meta left column
            gfx.DrawString("Receipt No:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(receiptNo, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 18;

            gfx.DrawString("Date:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " " + time, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 22;

            gfx.DrawLine(XPens.LightGray, left, y, pageWidth - left, y);
            y += 10;

            // Customer / loan
            gfx.DrawString("Received from:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(customer, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 18;

            gfx.DrawString("Loan Account:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(loanAccount, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 20;

            // Payment details block
            gfx.DrawString("Payment Details:", boldFont, XBrushes.Black, new XPoint(left, y));
            y += 18;

            gfx.DrawString("Principal:", mono, XBrushes.Black, new XPoint(left + 10, y));
            gfx.DrawString($"₱{principal:N2}", mono, XBrushes.Black, new XRect(left + 260, y, 200, 16), XStringFormats.TopRight);
            y += 16;

            gfx.DrawString("Interest:", mono, XBrushes.Black, new XPoint(left + 10, y));
            gfx.DrawString($"₱{interest:N2}", mono, XBrushes.Black, new XRect(left + 260, y, 200, 16), XStringFormats.TopRight);
            y += 16;

            gfx.DrawString("Penalty:", mono, XBrushes.Black, new XPoint(left + 10, y));
            gfx.DrawString($"₱{penalty:N2}", mono, XBrushes.Black, new XRect(left + 260, y, 200, 16), XStringFormats.TopRight);
            y += 20;

            gfx.DrawLine(XPens.LightGray, left, y, pageWidth - left, y);
            y += 8;

            gfx.DrawString("Total Amount:", boldFont, XBrushes.Black, new XPoint(left + 10, y));
            gfx.DrawString($"₱{total:N2}", boldFont, XBrushes.Black, new XRect(left + 260, y, 200, 18), XStringFormats.TopRight);
            y += 30;

            gfx.DrawString("Payment Mode:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(paymentMode, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 18;

            gfx.DrawString("Cashier:", boldFont, XBrushes.Black, new XPoint(left, y));
            gfx.DrawString(cashier, normalFont, XBrushes.Black, new XPoint(left + 110, y));
            y += 30;

            // Footer - signature/notes
            gfx.DrawString("Thank you for your payment.", normalFont, XBrushes.Gray, new XPoint(left, y));
            y += 24;

            // Save to temp file
            var fileName = $"receipt_{receiptNo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            pdf.Save(tempPath);

            // Try to open it with default PDF viewer
            try
            {
                var p = new ProcessStartInfo(tempPath)
                {
                    UseShellExecute = true
                };
                Process.Start(p);
            }
            catch
            {
                // swallow - caller can handle if necessary
            }

            return tempPath;
        }
    }
}