using HotelBookingSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Globalization;

namespace HotelBookingSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult HotelRevenueReport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HotelRevenueReport(DateTime startDate, DateTime endDate, string reportType)
        {
            var report = _reportService.GenerateHotelRevenueReport(startDate, endDate);

            if (reportType == "pdf")
            {
                // قم بإنشاء PDF
                return GeneratePdfReport(report);
            }
            else if (reportType == "excel")
            {
                // قم بإنشاء Excel
                return GenerateExcelReport(report);
            }

            return View(report);
        }

        private IActionResult GeneratePdfReport(List<HotelRevenueReport> report)
        {
            using (var memoryStream = new MemoryStream())
            {
                // إنشاء مستند PDF
                var document = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // إضافة عنوان
                document.Add(new Paragraph("Hotel Revenue Report"));
                document.Add(new Paragraph($"Date Generated: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(" ")); // سطر فارغ

                // إنشاء جدول لعرض البيانات
                var table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 2f, 2f, 2f });

                // إضافة رؤوس الأعمدة
                table.AddCell("Hotel Name");
                table.AddCell("Location");
                table.AddCell("Total Revenue");
                table.AddCell("Total Bookings");

                // إضافة البيانات إلى الجدول
                foreach (var item in report)
                {
                    table.AddCell(item.HotelName);
                    table.AddCell(item.Location);
                    table.AddCell(item.TotalRevenue.ToString("C", new CultureInfo("en-US")));
                    table.AddCell(item.TotalBookings.ToString());
                }

                document.Add(table);
                document.Close();

                // إرجاع الملف كـ IActionResult
                return File(memoryStream.ToArray(), "application/pdf", "HotelRevenueReport.pdf");
            }
        }



private IActionResult GenerateExcelReport(List<HotelRevenueReport> report)
    {
        // تعيين سياق الترخيص
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Hotel Revenue Report");

            // إضافة رؤوس الأعمدة
            worksheet.Cells[1, 1].Value = "Hotel Name";
            worksheet.Cells[1, 2].Value = "Location";
            worksheet.Cells[1, 3].Value = "Total Revenue";
            worksheet.Cells[1, 4].Value = "Total Bookings";

            // إضافة البيانات إلى الجدول
            int row = 2;
            foreach (var item in report)
            {
                worksheet.Cells[row, 1].Value = item.HotelName;
                worksheet.Cells[row, 2].Value = item.Location;
                worksheet.Cells[row, 3].Value = item.TotalRevenue.ToString("C", new CultureInfo("en-US"));
                worksheet.Cells[row, 4].Value = item.TotalBookings;
                row++;
            }

            // إعداد الذاكرة المؤقتة لملف Excel
            var stream = new MemoryStream();
            package.SaveAs(stream);

            // إرجاع الملف كـ IActionResult
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HotelRevenueReport.xlsx");
        }
    }

}
}
