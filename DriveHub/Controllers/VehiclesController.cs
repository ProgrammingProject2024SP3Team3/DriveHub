using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using Microsoft.AspNetCore.Identity;

namespace DriveHub.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public VehiclesController(
            ApplicationDbContext context,
            ILogger<VehiclesController> logger,
            UserManager<IdentityUser> userManager
        )
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Pickup(string id)
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

            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .Where(c => c.VehicleId == id)
                .Where(c => c.Id == userId)
                .Where(c => c.ReservationExpires < DateTime.Now)
                .FirstOrDefaultAsync();

            return View(vehicle);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Dropoff(string id)
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

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
