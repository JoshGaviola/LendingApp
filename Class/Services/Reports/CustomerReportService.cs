using System;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using LendingApp.Class;
using System.Globalization;

namespace LendingApp.Class.Services.Reports
{
    /// <summary>
    /// Service responsible for generating customer profile PDF reports.
    /// Keeps UI free of PDFSharp and DB details.
    /// </summary>
    public class CustomerReportService
    {
        /// <summary>
        /// Generate a printable PDF customer profile including applications and released loans.
        /// Throws exceptions on failure so caller (UI) can show appropriate messages.
        /// </summary>
        public void GenerateCustomerProfilePdf(string customerId, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentException("customerId is required", nameof(customerId));
            if (string.IsNullOrWhiteSpace(outputFilePath)) throw new ArgumentException("outputFilePath is required", nameof(outputFilePath));

            // Ensure PDFsharp will resolve fonts (Program.cs should already configure GlobalFontSettings).
            // This method assumes GlobalFontSettings.UseWindowsFontsUnderWindows or a FontResolver is already set.

            using (var db = new AppDbContext())
            {
                var customer = db.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == customerId);
                if (customer == null) throw new InvalidOperationException("Customer not found: " + customerId);

                var apps = db.LoanApplications.AsNoTracking()
                    .Where(a => a.CustomerId == customerId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList();

                var loans = db.Loans.AsNoTracking()
                    .Where(l => l.CustomerId == customerId)
                    .OrderByDescending(l => l.ReleaseDate)
                    .ToList();

                var doc = new PdfDocument();
                doc.Info.Title = $"Customer Report - {customer.FirstName} {customer.LastName}";

                const double margin = 40;
                const double lineHeight = 14;

                // Use XFontStyleEx for PDFsharp 6.2+
                var fontTitle = new XFont("Segoe UI", 14, XFontStyleEx.Bold);
                var fontHeader = new XFont("Segoe UI", 10, XFontStyleEx.Bold);
                var fontNormal = new XFont("Segoe UI", 9, XFontStyleEx.Regular);

                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                double y = margin;
                double pageWidth = page.Width.Point;
                double usableWidth = pageWidth - margin * 2;

                void NewPage()
                {
                    gfx.Dispose();
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;
                }

                // Title
                gfx.DrawString("CUSTOMER PROFILE REPORT", fontTitle, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += 28;

                // Personal information
                gfx.DrawString("Personal Information", fontHeader, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += lineHeight;

                var dobText = customer.DateOfBirth.HasValue ? customer.DateOfBirth.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("en-US")) : "";
                var age = customer.DateOfBirth.HasValue ? (int)((DateTime.Today - customer.DateOfBirth.Value.Date).TotalDays / 365.2425) : 0;

                var personal = new[]
                {
                    $"Customer ID: {customer.CustomerId}",
                    $"Name: {(customer.FirstName ?? "")} {(customer.LastName ?? "")}".Trim(),
                    $"DOB: {dobText} (Age: {age})",
                    $"Gender: {customer.Gender} | Civil Status: {customer.CivilStatus}",
                    $"Nationality: {customer.Nationality}",
                    $"Email: {customer.EmailAddress}",
                    $"Mobile: {customer.MobileNumber} | Tel: {customer.TelephoneNumber}",
                    $"Present Address: {customer.PresentAddress}",
                    $"Permanent Address: {customer.PermanentAddress}"
                };

                foreach (var line in personal)
                {
                    gfx.DrawString(line, fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);
                    y += lineHeight;
                    if (y > page.Height.Point - margin) NewPage();
                }

                y += 6;
                gfx.DrawString("Employment & Bank", fontHeader, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += lineHeight;

                var emp = new[]
                {
                    $"Employment Status: {customer.EmploymentStatus}",
                    $"Company: {customer.CompanyName} | Position: {customer.Position}",
                    $"Department: {customer.Department}",
                    $"Company Address: {customer.CompanyAddress}",
                    $"Company Phone: {customer.CompanyPhone}",
                    $"Bank: {customer.BankName} | Account: {customer.BankAccountNumber}"
                };

                foreach (var line in emp)
                {
                    gfx.DrawString(line, fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);
                    y += lineHeight;
                    if (y > page.Height.Point - margin) NewPage();
                }

                y += 6;
                gfx.DrawString("Government IDs", fontHeader, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += lineHeight;

                var ids = new[]
                {
                    $"SSS: {customer.SSSNumber}",
                    $"TIN: {customer.TINNumber}",
                    $"Passport: {customer.PassportNumber}",
                    $"Driver's License: {customer.DriversLicenseNumber}",
                    $"UMID: {customer.UMIDNumber}",
                    $"PhilHealth: {customer.PhilhealthNumber}",
                    $"Pag-IBIG: {customer.PagibigNumber}"
                };

                foreach (var line in ids)
                {
                    gfx.DrawString(line, fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);
                    y += lineHeight;
                    if (y > page.Height.Point - margin) NewPage();
                }

                // Loan activity
                y += 6;
                gfx.DrawString("Loan Activity", fontHeader, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += lineHeight;

                if (apps.Any())
                {
                    gfx.DrawString("Applications:", fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);
                    y += lineHeight;
                    foreach (var a in apps)
                    {
                        var idText = !string.IsNullOrWhiteSpace(a.ApplicationNumber) ? a.ApplicationNumber : a.ApplicationId.ToString();
                        var amtText = a.RequestedAmount.ToString("C0", CultureInfo.GetCultureInfo("en-US"));
                        var line = $"{idText} | {amtText} | {a.Status} | {a.ApplicationDate:yyyy-MM-dd}";
                        gfx.DrawString(line, fontNormal, XBrushes.Black, new XRect(margin + 20, y, usableWidth - 20, 0), XStringFormats.TopLeft);
                        y += lineHeight;
                        if (y > page.Height.Point - margin) NewPage();
                    }
                }

                if (loans.Any())
                {
                    gfx.DrawString("Released Loans:", fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);
                    y += lineHeight;
                    foreach (var l in loans)
                    {
                        var idText = l.LoanNumber ?? l.LoanId.ToString();
                        var amtText = l.PrincipalAmount.ToString("C0", CultureInfo.GetCultureInfo("en-US"));
                        var line = $"{idText} | {amtText} | {l.Status} | {l.ReleaseDate:yyyy-MM-dd}";
                        gfx.DrawString(line, fontNormal, XBrushes.Black, new XRect(margin + 20, y, usableWidth - 20, 0), XStringFormats.TopLeft);
                        y += lineHeight;
                        if (y > page.Height.Point - margin) NewPage();
                    }
                }

                // Remarks
                y += 12;
                gfx.DrawString("Remarks:", fontHeader, XBrushes.Black, new XRect(margin, y, usableWidth, 0), XStringFormats.TopLeft);
                y += lineHeight;
                gfx.DrawString(customer.Remarks ?? "", fontNormal, XBrushes.Black, new XRect(margin + 10, y, usableWidth - 10, 0), XStringFormats.TopLeft);

                // Save
                doc.Save(outputFilePath);
                doc.Close();
            }
        }
    }
}