using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;

namespace Admin.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger _logger;

        public VehiclesController(ApplicationDbContext context, ILogger<Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.VehicleRate).Include(v => v.Pod);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .Include(v => v.Pod)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,VehicleRateId,Make,Model,RegistrationPlate,State,Year,Seats")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicle.VehicleRateId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicle.VehicleRateId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Admin.Models.Dto.Vehicle vehicleDto)
        {
            _logger.LogInformation($"Editing {id}");
            _logger.LogInformation($"Editing {vehicleDto.ToString()}");
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (id != vehicleDto.VehicleId)
            {
                _logger.LogWarning($"No match for {id}");
                return NotFound();
            }

            if (vehicle == null)
            {
                _logger.LogWarning($"No match for {id}");
                return NotFound();
            }

            if (!_context.VehicleRates.Any(e => e.VehicleRateId == vehicleDto.VehicleRateId))
            {
                _logger.LogWarning($"No match for rate {vehicleDto.VehicleRateId}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogWarning($"Model is valid {id}");                  

                    vehicle.VehicleRateId = vehicleDto.VehicleRateId;
                    vehicle.Name = vehicleDto.Name;
                    vehicle.Make = vehicleDto.Make;
                    vehicle.Model = vehicleDto.Model;
                    vehicle.RegistrationPlate = vehicleDto.RegistrationPlate;
                    vehicle.State = vehicleDto.State;
                    vehicle.Year = vehicleDto.Year;
                    vehicle.Seats = vehicleDto.Seats;
                    vehicle.Colour = vehicleDto.Colour;

                    _context.Update(vehicle);
                    _logger.LogWarning($"Updated {id}");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
                    {
                        _logger.LogWarning($"Database error {id}");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _logger.LogWarning($"Success editing {id}");
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning($"Failed editing {id}");
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicle.VehicleRateId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
