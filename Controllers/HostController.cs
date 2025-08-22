using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisiTrack.Data;
using VisiTrack.Models;
using VisiTrack.Models.ViewModel;
using Host = VisiTrack.Models.Host;

namespace VisiTrack.Controllers
{
    public class HostController(ApplicationContext context) : Controller
    {


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var hosts = context.Hosts
            .OrderByDescending(h => h.HostID) 
            .ToList();


            var model = new HostIndexViewModel
            {
                HostList = hosts,
                NewHost = new Host()
            };

            return View(model);

        }

      
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var host = await context.Hosts
                .FirstOrDefaultAsync(m => m.HostID == id);
            if (host == null)
            {
                return NotFound();
            }

            return View(host);
        }


       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Create(HostIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                context.Hosts.Add(model.NewHost);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.HostList = context.Hosts.ToList();
            return View("Index", model);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var host = await context.Hosts.FindAsync(id);
            if (host == null)
            {
                return NotFound();
            }
            return View(host);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HostID,FirstName,LastName,Department,ContactNumber")] Host host)
        {
            if (id != host.HostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(host);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HostExists(host.HostID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(host);
        }

      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var host = await context.Hosts
                .FirstOrDefaultAsync(m => m.HostID == id);
            if (host == null)
            {
                return NotFound();
            }

            return View(host);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var host = await context.Hosts.FindAsync(id);
            if (host != null)
            {
                context.Hosts.Remove(host);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HostExists(int id)
        {
            return context.Hosts.Any(e => e.HostID == id);
        }
    }
}
