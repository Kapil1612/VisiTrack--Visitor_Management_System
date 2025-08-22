using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VisiTrack.Data;
using VisiTrack.Models;
using VisiTrack.Models.ViewModel;



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
                .OrderByDescending(v => v.CheckInTime)
                .Select(v => new VisitsViewModel
                {
                    VisitID = v.VisitID,
                    VisitorName = (v.Visitor != null ? (v.Visitor.FirstName ?? "") + " " + (v.Visitor.LastName ?? "") : string.Empty),
                    Company = v.Visitor != null ? v.Visitor.Company : string.Empty,
                    HostName = (v.Host != null ? (v.Host.FirstName ?? "") + " " + (v.Host.LastName ?? "") : string.Empty),
                    Purpose = v.Purpose,
                    CheckInTime = v.CheckInTime,
                    CheckOutTime = v.CheckOutTime,
                    Status = v.Status
                })
                .ToList();

            return View(model);
        }



        private ReportViewModel BuildReport(DateTime? startDate, DateTime? endDate)
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
                }).ToList();

            // Custom report (between dates)
            var custom = new List<ReportRowViewModel>();
            if (startDate.HasValue && endDate.HasValue)
            {
                custom = context.Visits
                    .Include(v => v.Visitor)
                    .Include(v => v.Host)
                    .Where(v => v.CheckInTime.HasValue &&
                                v.CheckInTime.Value.Date >= startDate.Value.Date &&
                                v.CheckInTime.Value.Date <= endDate.Value.Date)
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
                    }).ToList();
            }

            return new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DailyReport = daily,
                CustomReport = custom
            };
        }

        // 🔹 Show report in view
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var model = BuildReport(startDate, endDate);
            return View(model);
        }

        // 🔹 Generate PDF report
        [HttpGet]
        public IActionResult GeneratePdf(DateTime? startDate, DateTime? endDate)
        {
            var model = BuildReport(startDate, endDate);

            var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                // Title
                document.Add(new Paragraph("Visitor`s Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                // Date range (if provided)
                if (model.StartDate.HasValue || model.EndDate.HasValue)
                {
                    document.Add(new Paragraph(
                        $"From: {model.StartDate:dd-MM-yyyy}  To: {model.EndDate:dd-MM-yyyy}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12));
                }

                // ✅ If custom report exists → show only that
                if (model.CustomReport != null && model.CustomReport.Any())
                {
                    document.Add(new Paragraph("Custom Report")
                        .SetFontSize(14).SetBold().SetMarginTop(15));
                    document.Add(CreateTable(model.CustomReport));
                }
                // ✅ Otherwise, show daily report
                else if (model.DailyReport != null && model.DailyReport.Any())
                {
                    document.Add(new Paragraph("Daily Report")
                        .SetFontSize(14).SetBold().SetMarginTop(15));
                    document.Add(CreateTable(model.DailyReport));
                }
                else
                {
                    document.Add(new Paragraph("No records found.")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12));
                }
            }

            return File(ms.ToArray(), "application/pdf", "VisitorReport.pdf");
        }


        private Table CreateTable(List<ReportRowViewModel> rows)
        {
            Table table = new Table(8, false);

            // Header
            table.AddHeaderCell("Visitor Name");
            table.AddHeaderCell("Company");
            table.AddHeaderCell("Host Name");
            table.AddHeaderCell("Purpose");
            table.AddHeaderCell("Check-In");
            table.AddHeaderCell("Check-Out");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Duration");

            // Rows
            foreach (var row in rows)
            {
                table.AddCell(row.VisitorName ?? "");
                table.AddCell(row.Company ?? "");
                table.AddCell(row.HostName ?? "");
                table.AddCell(row.Purpose ?? "");
                table.AddCell(row.CheckInTime?.ToString("dd-MM-yyyy HH:mm") ?? "");
                table.AddCell(row.CheckOutTime?.ToString("dd-MM-yyyy HH:mm") ?? "");
                table.AddCell(row.Status ?? "");
                table.AddCell(row.Duration ?? "");
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
