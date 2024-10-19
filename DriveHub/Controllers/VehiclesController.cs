using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using Microsoft.AspNetCore.Identity;
using DriveHubModel;

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

        // GET: Vehicles/Pickup/5
        public async Task<IActionResult> Pickup(string id)
        {
            _logger.LogInformation($"Picking up {id}");

            if (id == null)
            {
                _logger.LogInformation($"Bad vehicle id {id}");
                return View(nameof(Error));
            }

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                _logger.LogInformation($"Vehicle not found {id}");
                return View(nameof(Error));
            }

            _logger.LogInformation($"Found vehicle {id}");

            var booking = await _context.Bookings
                .Where(c => c.VehicleId == id)
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Reserved)
                .Include(c => c.StartPod)
                .ThenInclude(c => c.Site)
                .FirstOrDefaultAsync();

            if (booking?.BookingId == null)
            {
                _logger.LogInformation($"Couldn't find booking for {id}");
                return RedirectToAction("Search", "Bookings");
            }

            _logger.LogInformation($"Pickup OK - {booking.BookingId}");

            booking.StartTime = DateTime.Now;
            booking.BookingStatus = BookingStatus.Collected;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return View(booking);
        }

        // GET: Vehicles/Dropoff/5
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

        /// <summary>
        /// Return the error page when a pickup/dropoff action is not sane. Not publicly accessible.
        /// </summary>
        /// <returns>The error page</returns>
        private IActionResult Error()
        {
            return View();
        }

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
