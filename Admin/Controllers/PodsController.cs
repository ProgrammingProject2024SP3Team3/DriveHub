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
        private readonly ILogger _logger;

        public PodsController(ApplicationDbContext context, ILogger<PodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Pods
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pods.Include(p => p.Site).Include(p => p.Vehicle).OrderBy(c => c.SiteId);
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
            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "SiteName");
            var vehicles = _context.Vehicles.Where(c => c.Pod == null).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "VehicleId", "Name");
            return View();
        }

        // POST: Pods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admin.Models.Dto.Pod pod)
        {
            if (ModelState.IsValid)
            {
                Pod podDb = new Pod(
                    pod.SiteId,
                    pod.VehicleId,
                    pod.PodName
                    );
                _context.Add(podDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "SiteName", pod.SiteId);
            var vehicles = _context.Vehicles.Where(c => c.Pod == null).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "VehicleId", "Name", pod.VehicleId);
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

            var vehicles = new List<Vehicle>();
            if (pod.VehicleId != null)
            {
                _logger.LogWarning($"Pod {id} has a vehicle");
                var vehicle = await _context.Vehicles.FindAsync(pod.VehicleId);
                vehicles.Add(vehicle);
            }
            vehicles.AddRange(await _context.Vehicles.Where(c => c.Pod == null).ToListAsync());

            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "SiteName", pod.SiteId);
            ViewData["VehicleId"] = new SelectList(vehicles, "VehicleId", "Name", pod.VehicleId);
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

            var vehicles = new List<Vehicle>();
            if (pod.VehicleId != null)
            {
                _logger.LogWarning($"Pod {id} has a vehicle");
                var vehicle = await _context.Vehicles.FindAsync(pod.VehicleId);
                vehicles.Add(vehicle);
            }
            vehicles.AddRange(await _context.Vehicles.Where(c => c.Pod == null).ToListAsync());

            ViewData["SiteId"] = new SelectList(_context.Sites, "SiteId", "SiteName", pod.SiteId);
            ViewData["VehicleId"] = new SelectList(vehicles, "VehicleId", "Name", pod.VehicleId);
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
