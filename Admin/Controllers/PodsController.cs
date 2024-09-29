using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;

namespace Admin.Controllers
{
    public class PodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pods
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pods.Include(p => p.Site).Include(p => p.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pods/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pod = await _context.Pods
                .Include(p => p.Site)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(m => m.PodId == id);
            if (pod == null)
            {
                return NotFound();
            }

            return View(pod);
        }

        // GET: Pods/Create
        public IActionResult Create()
        {
            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "Address");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId");
            return View();
        }

        // POST: Pods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PodId,SiteId,VehicleId,PodName")] Pod pod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "Address", pod.SiteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", pod.VehicleId);
            return View(pod);
        }

        // GET: Pods/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pod = await _context.Pods.FindAsync(id);
            if (pod == null)
            {
                return NotFound();
            }
            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "Address", pod.SiteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", pod.VehicleId);
            return View(pod);
        }

        // POST: Pods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PodId,SiteId,VehicleId,PodName")] Pod pod)
        {
            if (id != pod.PodId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PodExists(pod.PodId))
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
            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "Address", pod.SiteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", pod.VehicleId);
            return View(pod);
        }

        // GET: Pods/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pod = await _context.Pods
                .Include(p => p.Site)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(m => m.PodId == id);
            if (pod == null)
            {
                return NotFound();
            }

            return View(pod);
        }

        // POST: Pods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var pod = await _context.Pods.FindAsync(id);
            if (pod != null)
            {
                _context.Pods.Remove(pod);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PodExists(string id)
        {
            return _context.Pods.Any(e => e.PodId == id);
        }
    }
}
