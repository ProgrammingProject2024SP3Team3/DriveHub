using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Controllers
{
    [Authorize]
    public class VehicleRatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleRatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VehicleRates
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleRates.ToListAsync());
        }

        // GET: VehicleRates/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleRate = await _context.VehicleRates
                .FirstOrDefaultAsync(m => m.VehicleRateId == id);
            if (vehicleRate == null)
            {
                return NotFound();
            }

            return View(vehicleRate);
        }

        // GET: VehicleRates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleRates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleRateId,Description,PricePerHour,EffectiveDate")] VehicleRate vehicleRate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleRate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleRate);
        }

        // GET: VehicleRates/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleRate = await _context.VehicleRates.FindAsync(id);
            if (vehicleRate == null)
            {
                return NotFound();
            }
            return View(vehicleRate);
        }

        // POST: VehicleRates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("VehicleRateId,Description,PricePerHour,EffectiveDate")] VehicleRate vehicleRate)
        {
            if (id != vehicleRate.VehicleRateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleRateExists(vehicleRate.VehicleRateId))
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
            return View(vehicleRate);
        }

        // GET: VehicleRates/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleRate = await _context.VehicleRates
                .FirstOrDefaultAsync(m => m.VehicleRateId == id);
            if (vehicleRate == null)
            {
                return NotFound();
            }

            return View(vehicleRate);
        }

        // POST: VehicleRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicleRate = await _context.VehicleRates.FindAsync(id);
            if (vehicleRate != null)
            {
                _context.VehicleRates.Remove(vehicleRate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleRateExists(string id)
        {
            return _context.VehicleRates.Any(e => e.VehicleRateId == id);
        }
    }
}
