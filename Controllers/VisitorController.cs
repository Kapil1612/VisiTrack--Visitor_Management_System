using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisiTrack.Data;
using VisiTrack.Models;
using VisiTrack.Models.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VisiTrack.Controllers
{
    public class VisitorController(ApplicationContext context) : Controller
    {
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult CheckIn()
        {
            var model = new CheckInViewModel
            {
                Hosts = context.Hosts
                    .Select(h => new SelectListItem
                    {
                        Value = h.HostID.ToString(),
                        Text = h.FirstName + " " + h.LastName
                    }).ToList()
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(CheckInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Hosts = context.Hosts
                    .Select(h => new SelectListItem
                    {
                        Value = h.HostID.ToString(),
                        Text = h.FirstName + " " + h.LastName
                    }).ToList();
                return View(model);
            }

            context.Visitors.Add(model.Visitor);
            context.SaveChanges();

            var visit = new Visit
            {
                VisitorID = model.Visitor.VisitorID,
                HostID = model.HostID,
                Purpose = model.Purpose
            };
            context.Visits.Add(visit);
            context.SaveChanges();

            return RedirectToAction("CheckOut", "Visitor");
        }


        [Authorize(Roles = "Admin,Staff")]
        public IActionResult CheckOut(string name, DateTime? date)
        {
            var query = context.Visits
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Where(v => v.Status == "Checked In")
            .AsQueryable();


            if (!string.IsNullOrEmpty(name))
            {
               
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(v =>
                        (v.Visitor != null && v.Visitor.FirstName != null && v.Visitor.FirstName.Contains(name)) ||
                        (v.Visitor != null && v.Visitor.LastName != null && v.Visitor.LastName.Contains(name)) ||
                        (v.Host != null && v.Host.FirstName != null && v.Host.FirstName.Contains(name)) ||
                        (v.Host != null && v.Host.LastName != null && v.Host.LastName.Contains(name)));
                }
                       
            }

            if (date.HasValue)
            {
                var selectedDate = date.Value.Date;
                query = query.Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == selectedDate);
            }

            var model = new CheckInSearchViewmodel
            {
                Name = name,
                Date = date,
                Visits = query.Select(v => new ReportRowViewModel
                {
                    VisitID = v.VisitID,
                    VisitorName = (v.Visitor != null ? v.Visitor.FirstName + " " + v.Visitor.LastName : "Unknown"),
                    Company = v.Visitor != null ? v.Visitor.Company : "Unknown",
                    HostName = (v.Host != null ? v.Host.FirstName + " " + v.Host.LastName : "Unknown"),
                    Purpose = v.Purpose,
                    CheckInTime = v.CheckInTime,
                    CheckOutTime = v.CheckOutTime,
                    Status = v.Status
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut(List<int> visitId)
        {
            if (visitId == null || !visitId.Any())
            {
                TempData["Error"] = "No visitors selected.";
                return RedirectToAction("CheckOut");
            }

            var visits = context.Visits
            .Where(v => visitId.Contains(v.VisitID) && v.Status == "Checked In")
            .ToList();

            foreach (var visit in visits)
            {
                visit.CheckOutTime = DateTime.Now;
                visit.Status = "Checked Out";
            }

            context.SaveChanges();

            TempData["Success"] = "Checked out selected visitors successfully.";
            return RedirectToAction("CheckOut");
        }
    }

}

