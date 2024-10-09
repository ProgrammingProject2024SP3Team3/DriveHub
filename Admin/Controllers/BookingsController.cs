using Admin.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.EndPod)
                .ThenInclude(c => c.Site)
                .Include(b => b.StartPod)
                .ThenInclude(c => c.Site)
                .Include(b => b.Vehicle)
                .ThenInclude(c => c.VehicleRate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.EndPod)
                .Include(b => b.StartPod)
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["EndPodId"] = new SelectList(_context.Pods, "PodId", "PodName");
            ViewData["StartPodId"] = new SelectList(_context.Pods, "PodId", "PodName");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "Name");
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", booking.Id);
            ViewData["EndPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.EndPodId);
            ViewData["StartPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.StartPodId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "Name", booking.VehicleId);
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", booking.Id);
            ViewData["EndPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.EndPodId);
            ViewData["StartPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.StartPodId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "Name", booking.VehicleId);
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", booking.Id);
            ViewData["EndPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.EndPodId);
            ViewData["StartPodId"] = new SelectList(_context.Pods, "PodId", "PodName", booking.StartPodId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "Name", booking.VehicleId);
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
                .Include(b => b.EndPod)
                .Include(b => b.StartPod)
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
