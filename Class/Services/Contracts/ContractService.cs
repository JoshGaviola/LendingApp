using System;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using LendingApp.Class;
using System.Globalization;

namespace LendingApp.Class.Services.Contracts
{
    public class ContractService
    {
        // Generates a simple contract PDF for the given application id + evaluation id.
        // Throws on error; caller responsible for UI.
        public void GenerateContractPdf(int applicationId, long? evaluationId, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(outputFilePath)) throw new ArgumentException(nameof(outputFilePath));

            using (var db = new AppDbContext())
            {
                var app = db.LoanApplications.Find(applicationId);
                if (app == null) throw new InvalidOperationException("Application not found: " + applicationId);

                var customer = db.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == app.CustomerId);
                var eval = evaluationId.HasValue ? db.LoanApplicationEvaluations.Find(evaluationId.Value) : null;

                // compute terms to display (prefer evaluation values if present)
                var termMonths = eval?.TermMonths ?? app.PreferredTerm;
                var interestPct = eval?.InterestRatePct ?? 0m;
                var serviceFeePct = eval?.ServiceFeePct ?? 0m;
                var principal = app.RequestedAmount;

                // perform loan calc (call your existing LoanComputationService)
                var calc = Class.Services.Loans.LoanComputationService.Calculate(
                    principal: principal,
                    annualRatePct: interestPct,
                    termMonths: termMonths > 0 ? termMonths : 12,
                    serviceFeePct: serviceFeePct,
                    method: Class.Services.Loans.LoanComputationService.ParseInterestMethod(eval?.InterestMethod ?? "Diminishing Balance"));

                var doc = new PdfDocument();
                doc.Info.Title = $"Contract - {app.ApplicationNumber}";

                var fontTitle = new XFont("Arial", 14, XFontStyleEx.Bold);
                var fontHeader = new XFont("Arial", 10, XFontStyleEx.Bold);
                var fontNormal = new XFont("Arial", 9, XFontStyleEx.Regular);

                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                double y = 40;
                double margin = 40;
                double usable = page.Width.Point - margin * 2;

                gfx.DrawString("LOAN CONTRACT PREVIEW", fontTitle, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopCenter);
                y += 28;

                // borrower & app info
                gfx.DrawString($"Application: {app.ApplicationNumber}", fontHeader, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 16;
                gfx.DrawString($"Borrower: {(customer != null ? (customer.FirstName + " " + customer.LastName) : app.CustomerId)}", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Principal: {principal.ToString("C2", CultureInfo.GetCultureInfo("en-US"))}", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Term: {termMonths} months", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Interest: {interestPct:N2}% p.a.", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Service Fee: {serviceFeePct:N2}% ({calc.ServiceFeeAmount.ToString("C2", CultureInfo.GetCultureInfo("en-US"))})", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 20;

                gfx.DrawString("Monthly Payment Summary", fontHeader, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 16;
                gfx.DrawString($"Monthly Payment: {calc.MonthlyPayment.ToString("C2", CultureInfo.GetCultureInfo("en-US"))}", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Total Interest: {calc.TotalInterest.ToString("C2", CultureInfo.GetCultureInfo("en-US"))}", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 14;
                gfx.DrawString($"Total Payable: {calc.TotalPayable.ToString("C2", CultureInfo.GetCultureInfo("en-US"))}", fontNormal, XBrushes.Black, new XRect(margin, y, usable, 0), XStringFormats.TopLeft);
                y += 30;

                // standard T&C placeholder
                var terms = "Standard terms and conditions apply. This preview is not a signed contract. Final contract will be prepared upon release.";
                gfx.DrawString(terms, fontNormal, XBrushes.Black, new XRect(margin, y, usable, page.Height.Point - y - margin), XStringFormats.TopLeft);

                doc.Save(outputFilePath);
                doc.Close();
            }
        }
    }
}
