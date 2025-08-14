using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VisiTrack.Data;
using VisiTrack.Models;
using VisiTrack.Models.ViewModel;
using static VisiTrack.Models.ViewModel.ReportRowViewModel;


namespace VisiTrack.Controllers
{
    public class AdminController(ApplicationContext context) : Controller
    {
        private readonly ApplicationContext context = context;

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



        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;

            var daily = context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.Host)
                .Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == today)
                .Select(v => new ReportRowViewModel
                {
                    VisitID = v.VisitID,
                    VisitorName = (v.Visitor != null ? (v.Visitor.FirstName ?? "") + " " + (v.Visitor.LastName ?? "") : string.Empty),
                    Company = v.Visitor != null ? v.Visitor.Company : string.Empty,
                    HostName = (v.Host != null ? (v.Host.FirstName ?? "") + " " + (v.Host.LastName ?? "") : string.Empty),
                    Purpose = v.Purpose,
                    CheckInTime = v.CheckInTime ,
                    CheckOutTime = v.CheckOutTime,
                    Status = v.Status
                }).ToList();

            var custom = new List<ReportRowViewModel>();

            if (startDate.HasValue && endDate.HasValue)
            {
                custom = [.. context.Visits
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
                    })];
            }

            var model = new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DailyReport = daily,
                CustomReport = custom
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
