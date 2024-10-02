using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;

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

        public IActionResult Search()
        {
            return View();
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
        // [Authorize] TODO: this route should probably authorize even before the POST. I don't know how to make that work - Jack
        public async Task<IActionResult> Create(string id)
        {
            // Get a vehicle with its rate, pod and site
            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .Include(v => v.Pod)
                    .ThenInclude(p => p.Site)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || vehicle.Pod == null)
            {
                // Note: there may be a better error for the car not being bookable (i.e. not currently in a pod)
                return NotFound($"Vehicle not found or not in a pod.");
            }

            return View(vehicle);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize] TODO: also Authorize attribute
        // TODO: Left the async function signature as it will be what we need but causes warnings for now
        // public async Task<IActionResult> Create(string id, string StartPodId, string EndPodId, DateTime StartTime, DateTime EndTime, decimal QuotedPricePerHour)
        public IActionResult Create(string id, string StartPodId, string EndPodId, DateTime StartTime, DateTime EndTime, decimal QuotedPricePerHour)
        {
            // DEBUG: this is just debug to show you what the form posted
            // DELETEME: There is nothing real here and can all be deleted. it's just sending back json of what was posted for testing
            // TODO: replace this with a proper booking next patch
            Console.WriteLine("--------------------\nCreate POST values\n--------------------");
            Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject();
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
                json[key] = Request.Form[key].ToString();
            }
            return Content(json.ToString(), "application/json");
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
