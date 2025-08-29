using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VisiTrack.Data;
using VisiTrack.Models;
using VisiTrack.Models.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace VisiTrack.Controllers
{
    public class AdminController(ApplicationContext context) : Controller
    {
        private readonly ApplicationContext context = context;

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            var today = DateTime.Today;
            var totalToday = context.Visits
                .Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == today)
                .Count();

            var currentlyCheckedIn = context.Visits
                .Count(v => v.Status == "Checked In");

            var model = new AdminDashboardViewModel
            {
                TotalVisitorsToday = totalToday,
                CurrentlyCheckedIn = currentlyCheckedIn
            };
            return View(model);
        }

        public IActionResult Visits()
        {
            var model = context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.Host)
                .Select(v => new VisitsViewModel
                {
                    VisitID = v.VisitID,
                    VisitorName = (v.Visitor != null ? (v.Visitor.FirstName ?? "") + " " + (v.Visitor.LastName ?? "") : string.Empty),
                    VistorContactNumber = v.Visitor != null ? v.Visitor.ContactNumber ?? "N/A" : "N/A",
                    Company = v.Visitor != null ? v.Visitor.Company : string.Empty,
                    HostName = (v.Host != null ? (v.Host.FirstName ?? "") + " " + (v.Host.LastName ?? "") : string.Empty),
                    Purpose = v.Purpose,
                    CheckInTime = v.CheckInTime,
                    CheckOutTime = v.CheckOutTime,
                    Status = v.Status
                }).OrderBy(v => v.VisitorName)
                  .ThenByDescending(v => v.VisitID)
                  .ToList();

            return View(model);
        }



        private ReportViewModel BuildReport(DateTime? startDate, DateTime? endDate, string visitorSearch, string hostSearch, string companySearch, string purposeSearch, string statusSearch)
        {
           

            // Daily report (today only)
            var daily = context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.Host)
                .Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == DateTime.Today && (!v.CheckOutTime.HasValue || v.CheckOutTime.Value.Date >= DateTime.Today))
                .Select(v => new ReportRowViewModel
                {
                    VisitID = v.VisitID,
                    VisitorName = (v.Visitor != null ? (v.Visitor.FirstName ?? "") + " " + (v.Visitor.LastName ?? "") : string.Empty),
                    Company = v.Visitor != null ? v.Visitor.Company : string.Empty,
                    HostName = (v.Host != null ? (v.Host.FirstName ?? "") + " " + (v.Host.LastName ?? "") : string.Empty),
                    Purpose = v.Purpose,
                    CheckInTime = v.CheckInTime,
                    CheckOutTime = v.CheckOutTime,
                    Status = v.Status
                }).OrderBy(v => v.VisitorName)
                  .ThenByDescending(v => v.VisitID)
                  .ToList();
           
            var customQuery = context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.Host)
                .AsQueryable();

           
            if (startDate.HasValue && endDate.HasValue)
            {
                customQuery = customQuery.Where(v =>
                    v.CheckInTime.HasValue &&
                    v.CheckInTime.Value.Date >= startDate.Value.Date &&
                    v.CheckInTime.Value.Date <= endDate.Value.Date);
            }

           
            if (!string.IsNullOrEmpty(visitorSearch))
            {
                customQuery = customQuery.Where(v =>
                    v.Visitor != null &&
                    (v.Visitor.FirstName + " " + v.Visitor.LastName).Contains(visitorSearch));
            }

            if (!string.IsNullOrEmpty(companySearch))
            {
                customQuery = customQuery.Where(v =>
                    v.Visitor != null && (v.Visitor.Company ?? "").Contains(companySearch));
            }

           
            if (!string.IsNullOrEmpty(hostSearch))
            {
                customQuery = customQuery.Where(v =>
                    v.Host != null && (v.Host.FirstName + " " + v.Host.LastName).Contains(hostSearch));
            }

        
            if (!string.IsNullOrEmpty(purposeSearch))
            {
                customQuery = customQuery.Where(v =>
                    (v.Purpose ?? "").Contains(purposeSearch));
            }

            // ✅ Apply status filter
            if (!string.IsNullOrEmpty(statusSearch))
            {
                customQuery = customQuery.Where(v =>
                    (v.Status ?? "").Contains(statusSearch));
            }


            var custom = customQuery
                    .Select(v => new ReportRowViewModel
                    {
                        VisitID = v.VisitID,
                        VisitorName = (v.Visitor != null ? (v.Visitor.FirstName ?? "") + " " + (v.Visitor.LastName ?? "") : string.Empty),
                        Company = v.Visitor != null ? v.Visitor.Company : string.Empty,
                        HostName = (v.Host != null ? (v.Host.FirstName ?? "") + " " + (v.Host.LastName ?? "") : string.Empty),
                        Purpose = v.Purpose,
                        CheckInTime = v.CheckInTime ?? DateTime.MinValue,
                        CheckOutTime = v.CheckOutTime,
                        Status = v.Status
                    }).OrderBy(v => v.VisitorName)
                      .ThenByDescending(v => v.VisitID)
                      .ToList();

            return new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                VisitorSearch = visitorSearch,
                HostSearch = hostSearch,
                CompanySearch = companySearch,
                PurposeSearch = purposeSearch,
                StatusSearch = statusSearch,
                DailyReport = daily,
                CustomReport = custom,
                DailyCount = daily.Count,
                CustomCount = custom.Count
            };
        }

      
        public IActionResult Report(DateTime? startDate, DateTime? endDate, string visitorSearch,string hostSearch,string companySearch , string purposeSearch , string statusSearch)
        {
            var model = BuildReport(startDate, endDate,visitorSearch, hostSearch, companySearch, purposeSearch, statusSearch);
            return View(model);
        }

        public IActionResult TodayReport()
        {
            var model = BuildReport(null, null,"","","","","");
            return View(model);
        }


   
        [HttpGet]
        public IActionResult GenerateDailyPdf()
        {
            var model = BuildReport(null, null,"","","","",""); // always today's data


            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
              
                document.Add(new Paragraph("REPORT")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                
                document.Add(new Paragraph($"Date: {DateTime.Today:dd-MM-yyyy}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12));

                if (model.DailyReport != null && model.DailyReport.Any())
                {
                    document.Add(new Paragraph("Visitor Log")
                        .SetFontSize(14).SetBold().SetMarginTop(15));
                    document.Add(CreateTable(model.DailyReport));
                }
                else
                {
                    document.Add(new Paragraph("No records found for today.")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12));
                }
            }
            
            return File(ms.ToArray(), "application/pdf", "DailyVisitorReport.pdf");
        }


        [HttpGet]
        public IActionResult GenerateCustomPdf(DateTime? startDate, DateTime? endDate, string visitorSearch, string hostSearch, string companySearch, string purposeSearch, string statusSearch)
        {
           
            if ((!startDate.HasValue || !endDate.HasValue) && string.IsNullOrEmpty(visitorSearch)
        && string.IsNullOrEmpty(hostSearch)
        && string.IsNullOrEmpty(companySearch)
        && string.IsNullOrEmpty(purposeSearch)
        && string.IsNullOrEmpty(statusSearch))

            {
                TempData["Error"] = "Please select a date range before downloading the custom report.";
                return RedirectToAction("Report");
            }

            var model = BuildReport(startDate, endDate, visitorSearch, hostSearch, companySearch, purposeSearch, statusSearch);

            byte[] fileBytes;
            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
               
                document.Add(new Paragraph("REPORT")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

              
                if (startDate.HasValue && endDate.HasValue)
                {
                    document.Add(new Paragraph($"From: {startDate:dd-MM-yyyy}  To: {endDate:dd-MM-yyyy}")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12));
                }
                if (!string.IsNullOrEmpty(visitorSearch))
                    document.Add(new Paragraph($"Visitor Filter: {visitorSearch}").SetFontSize(12));
                if (!string.IsNullOrEmpty(hostSearch))
                    document.Add(new Paragraph($"Host Filter: {hostSearch}").SetFontSize(12));
                if (!string.IsNullOrEmpty(companySearch))
                    document.Add(new Paragraph($"Company Filter: {companySearch}").SetFontSize(12));
                if (!string.IsNullOrEmpty(purposeSearch))
                    document.Add(new Paragraph($"Purpose Filter: {purposeSearch}").SetFontSize(12));
                if (!string.IsNullOrEmpty(statusSearch))
                    document.Add(new Paragraph($"Status Filter: {statusSearch}").SetFontSize(12));



              
                if (model.CustomReport != null && model.CustomReport.Any())
                {
                    document.Add(new Paragraph("Visitor Logs")
                        .SetFontSize(14).SetBold().SetMarginTop(15));
                    document.Add(CreateTable(model.CustomReport));
                }
                else
                {
                    document.Add(new Paragraph("No records found for the selected date range.")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)); 
                }
                document.Close();

              
                fileBytes = ms.ToArray();
            }
            return File(fileBytes, "application/pdf", "CustomVisitorReport.pdf");
        }



        private Table CreateTable(List<ReportRowViewModel> rows)
        {
            Table table = new Table(9, false);

           
            var bold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

          
            table.AddHeaderCell(new Cell().Add(new Paragraph("Id").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Visitor Name").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Company").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Host Name").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Purpose").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Check-In").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Check-Out").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Duration").SetFont(bold).SetFontSize(12)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Status").SetFont(bold).SetFontSize(12)));

          
            foreach (var row in rows)
            {
                table.AddCell(new Cell().Add(new Paragraph(row.VisitID.ToString()).SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.VisitorName ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Company ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.HostName ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Purpose ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.CheckInTime?.ToString("dd-MM-yyyy HH:mm") ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.CheckOutTime?.ToString("dd-MM-yyyy HH:mm") ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Duration ?? "").SetFontSize(10)));
                table.AddCell(new Cell().Add(new Paragraph(row.Status ?? "").SetFontSize(10)));
            }

            return table;
        }


      


        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreatedAt = DateTime.Now
                };

                context.Contacts.Add(contact);
                context.SaveChanges();

                TempData["Success"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
