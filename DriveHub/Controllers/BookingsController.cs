using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.Dto;
using NetTopologySuite.Geometries;

namespace DriveHub.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public IActionResult Index()
        {
            return View();
        }

        // API: Get available pods and vehicles filtered by time and proximity
        // This method fetches available pods and their associated vehicles based on user input for time and proximity
        [HttpGet("Search")]
        public async Task<IActionResult> Search(DateTime startTime, DateTime endTime, double userLatitude, double userLongitude, double maxDistance = 5000)
        {
            // Define the user's location as a geographic point using latitude and longitude with SRID 4326 (WGS84 standard)
            var userLocation = new Point(userLongitude, userLatitude) { SRID = 4326 };

            // Fetch pods that are available during the given time period and within the specified proximity to the user's location
            var availablePods = await _context.Pods
                .Include(p => p.Vehicle)  // Include vehicle details for each pod
                .Include(p => p.Site)     // Include site details (location information) for each pod
                .Where(p => !_context.Bookings.Any(b => 
                    b.VehicleId == p.VehicleId && (
                        (startTime >= b.StartTime && startTime < b.EndTime) ||  // Check if the start time conflicts with any existing booking
                        (endTime > b.StartTime && endTime <= b.EndTime) ||      // Check if the end time conflicts with any existing booking
                        (startTime < b.StartTime && endTime > b.EndTime))))     // Check if the new booking envelops an existing booking
                .Where(p => p.Site.Location.Distance(userLocation) <= maxDistance) // Filter pods by proximity to the user's location using spatial distance
                .Select(p => new PodDto
                {
                    PodId = p.PodId,                      // Pod identifier
                    PodName = p.PodName,                  // Name of the pod
                    SiteName = p.Site.SiteName,           // Name of the site
                    Address = p.Site.Address,             // Address of the site
                    City = p.Site.City,                   // City where the site is located
                    PostCode = p.Site.PostCode,           // Post code of the site
                    Latitude = p.Site.Location.Y,         // Latitude from the geographic location (spatial data)
                    Longitude = p.Site.Location.X,        // Longitude from the geographic location (spatial data)
                    VehicleId = p.Vehicle.VehicleId,      // Vehicle identifier
                    VehicleName = p.Vehicle.Name,         // Vehicle name
                    Make = p.Vehicle.Make,                // Vehicle make
                    Model = p.Vehicle.Model,              // Vehicle model
                    RegistrationPlate = p.Vehicle.RegistrationPlate, // Vehicle registration plate
                    Seats = p.Vehicle.Seats,              // Number of seats in the vehicle
                    Colour = p.Vehicle.Colour,            // Vehicle colour
                    VehicleCategory = p.Vehicle.VehicleRate.Description, // Vehicle category
                    PricePerHour = p.Vehicle.VehicleRate.PricePerHour   // Hourly rate of the vehicle
                })
                .ToListAsync();  // Convert the query to a list asynchronously

            // Return the list of available pods and associated data as JSON for the frontend to consume
            return Json(availablePods);
        }

        public async Task<IActionResult> CurrentBookings()
        {
            var applicationDbContext = await _context.Bookings.Include(b => b.ApplicationUser).Include(b => b.Vehicle).ToListAsync();
            return View(applicationDbContext);
        }

        public async Task<IActionResult> PastBookings()
        {
            var applicationDbContext = await _context.Bookings.Include(b => b.ApplicationUser).Include(b => b.Vehicle).ToListAsync();
            return View(applicationDbContext);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.ApplicationUser)
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Bookings, "VehicleId", "VehicleId");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VehicleId,Id,StartPodId,EndPodId,StartTime,EndTime,PricePerHour,BookingStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", booking.Id);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", booking.VehicleId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", booking.Id);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", booking.VehicleId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BookingId,VehicleId,Id,StartPodId,EndPodId,StartTime,EndTime,PricePerHour,BookingStatus")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["Id"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", booking.Id);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", booking.VehicleId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.ApplicationUser)
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
