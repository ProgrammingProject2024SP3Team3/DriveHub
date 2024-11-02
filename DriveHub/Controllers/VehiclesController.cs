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
                _logger.LogWarning($"Bad vehicle id {id}");
                return View(nameof(Error));
            }

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                _logger.LogWarning($"Vehicle not found {id}");
                return View(nameof(Error));
            }

            _logger.LogInformation($"Found vehicle {id}");

            Booking? booking = null;
            try
            {
                booking = await _context.Bookings
                .Where(c => c.VehicleId == id)
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Reserved)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleRate)
                .Include(c => c.StartPod)
                .ThenInclude(c => c.Site)
                .SingleAsync();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Booking not found for id: {id}");
                return RedirectToAction("Search", "Bookings");
            }

            if (booking == null)
            {
                _logger.LogWarning($"Vehicle not found {id}");
                return RedirectToAction("Search", "Bookings");
            }

            _logger.LogInformation($"Pickup OK - {booking.BookingId}");

            booking.StartTime = DateTime.Now;
            booking.BookingStatus = BookingStatus.Collected;
            var startPod = booking.StartPod;
            startPod.Vehicle = null;

            _context.Update(booking);
            _context.Update(startPod);
            _context.Update(vehicle);

            await _context.SaveChangesAsync();

            return View("Pickup", booking);
        }

        // GET: Vehicles/Dropoff/5
        public async Task<IActionResult> Dropoff(string id)
        {
            _logger.LogInformation($"Dropping off {id}");

            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Bad vehicle id provided.");
                return View(nameof(Error));
            }

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                _logger.LogWarning($"Vehicle not found for id: {id}");
                return View(nameof(Error));
            }

            _logger.LogInformation($"Found vehicle {id}");

            Booking? booking = null;
            try
            {
                booking = await _context.Bookings
                .Where(c => c.VehicleId == id)
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Collected)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleRate)
                .Include(c => c.StartPod)
                .ThenInclude(c => c.Site)
                .SingleAsync();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Booking not found for id: {id}");
                return View(nameof(Error));
            }

            if (booking == null || booking?.BookingId == null || booking.StartTime == null)
            {
                _logger.LogWarning($"Couldn't find collected booking for vehicle id: {id}");
                return View(nameof(Error));
            }

            var emptyPods = await _context.Pods.Where(c => c.VehicleId == null).Include(c => c.Site).ToListAsync();

            if (!emptyPods.Any())
            {
                _logger.LogWarning("No empty pods available for drop-off.");
                return View(nameof(Error));
            }

            Random rnd = new Random();
            var randPod = emptyPods[rnd.Next(emptyPods.Count)];

            booking.EndTime = DateTime.Now;
            booking.BookingStatus = BookingStatus.Unpaid;
            randPod.Vehicle = vehicle;
            booking.EndPod = randPod;
            vehicle.IsReserved = false;

            var totalMinutes = (int)Math.Round((((DateTime)booking.EndTime - (DateTime)booking.StartTime).TotalMinutes), 0);
            var totalAmount = (decimal)(Math.Max(totalMinutes, 2)) * booking.PricePerMinute;

            var invoice = new Invoice
            {
                Amount = totalAmount
            };

            booking.Invoice = invoice;

            _context.Add(invoice);
            _context.Update(booking);
            _context.Update(randPod);
            _context.Update(vehicle);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Drop-off completed for booking id: {booking.BookingId}");

            return View("Dropoff", booking);
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
