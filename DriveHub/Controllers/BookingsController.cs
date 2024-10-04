using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DriveHub.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public BookingsController(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Bookings
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search()
        {
            var vehicles = await _context.Vehicles.Where(c => c.Pod != null).Include(c => c.VehicleRate).ToListAsync();
            var seats = vehicles.Select(c => c.Seats).Distinct().ToList();
            var vehicleRates = await _context.VehicleRates.ToListAsync();
            var pods = await _context.Pods.Where(c => c.VehicleId != null).Include(c => c.Site).Include(c => c.Vehicle).OrderBy(c => c.SiteId).ToListAsync();
            var bookingSearchVM = new BookingSearchVM(
                vehicles,
                seats,
                vehicleRates,
                pods
                );
            return View(bookingSearchVM);
        }

        public async Task<IActionResult> CurrentBookings()
        {
            // TODO: this needs to be scoped to the currently logged in user's bookings

            // Here's the style of code we want to write if model relations allowed us
            // var booking = await _context.Bookings
            //     .Include(b => b.Vehicle)
            //     .Include(b => b.StartPod)
            //     .Include(b => b.EndPod)
            //     .FirstOrDefaultAsync(b => b.BookingId == id);
            // Unfortunately this is the code I can make work involving start and end pods
            var bookings = await (from b in _context.Bookings
                                  join sp in _context.Pods on b.StartPodId equals sp.PodId
                                  join ep in _context.Pods on b.EndPodId equals ep.PodId
                                  join ss in _context.Sites on sp.SiteId equals ss.SiteId
                                  join es in _context.Sites on ep.SiteId equals es.SiteId
                                  join vs in _context.Vehicles on b.VehicleId equals vs.VehicleId
                                  where b.BookingStatus != BookingStatus.Complete
                                  select new
                                  {
                                      Booking = b,
                                      StartPod = sp,
                                      EndPod = ep,
                                      StartSite = ss,
                                      EndSite = es,
                                      Vehicle = vs
                                  }).ToListAsync();

            return View(bookings);
        }

        // [Authorize] needed
        public async Task<IActionResult> PastBookings()
        {
            // TODO: this needs to be scoped to the currently logged in user's bookings

            // Here's the style of code we want to write if model relations allowed us
            // var booking = await _context.Bookings
            //     .Include(b => b.Vehicle)
            //     .Include(b => b.StartPod)
            //     .Include(b => b.EndPod)
            //     .FirstOrDefaultAsync(b => b.BookingId == id);
            // Unfortunately this is the code I can make work involving start and end pods
            var bookings = await (from b in _context.Bookings
                                  join sp in _context.Pods on b.StartPodId equals sp.PodId
                                  join ep in _context.Pods on b.EndPodId equals ep.PodId
                                  join ss in _context.Sites on sp.SiteId equals ss.SiteId
                                  join es in _context.Sites on ep.SiteId equals es.SiteId
                                  join vs in _context.Vehicles on b.VehicleId equals vs.VehicleId
                                  where b.BookingStatus == BookingStatus.Complete
                                  select new
                                  {
                                      Booking = b,
                                      StartPod = sp,
                                      EndPod = ep,
                                      StartSite = ss,
                                      EndSite = es,
                                      Vehicle = vs
                                  }).ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        // [Authorize] TODO
        public async Task<IActionResult> Details(string id)
        {

            var user = HttpContext.User;

            // This is code in the style I want to write but I cannot because StartPod and EndPod both being foreign keys of Pod confuses LINQ
            // var booking = await _context.Bookings
            //     .Include(b => b.Vehicle)
            //     .Include(b => b.StartPod)
            //     .Include(b => b.EndPod)
            //     .FirstOrDefaultAsync(b => b.BookingId == id);

            // So instead we get this monstrosity
            var booking = await (
                from b in _context.Bookings
                where b.BookingId == id
                join v in _context.Vehicles on b.VehicleId equals v.VehicleId
                join startPod in _context.Pods on b.StartPodId equals startPod.PodId
                join startSite in _context.Sites on startPod.SiteId equals startSite.SiteId
                join endPod in _context.Pods on b.EndPodId equals endPod.PodId
                join endSite in _context.Sites on endPod.SiteId equals endSite.SiteId
                select new
                {
                    b.BookingId,
                    Vehicle = v,
                    StartPod = new
                    {
                        Pod = startPod,
                        PodName = startPod.PodName,
                        Site = startSite
                    },
                    EndPod = new
                    {
                        Pod = endPod,
                        PodName = endPod.PodName,
                        Site = endSite
                    },
                    b.StartTime,
                    b.EndTime,
                    b.PricePerHour,
                    b.BookingStatus
                }
            ).FirstOrDefaultAsync();

            // TODO: needs to check booking belongs to User
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
        public async Task<IActionResult> Create(string id, string StartPodId, string EndPodId, DateTime StartTime, DateTime EndTime, decimal QuotedPricePerHour)
        {
            // DEBUG: this is just debug to show you what the form posted. Delete when you're happy with the function
            Console.WriteLine("--------------------\nCreate POST values\n--------------------");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
            }

            // Just get any user for now. This will be the current session's user in the real code
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null) return BadRequest("No first user in db");

            // Check the Vehicle exists, is in the given start pod (i.e. available for booking), and the QuotedPricePerHour matches the vehicle's current pricePerHour
            // These 3 things need to be done on top of the the simple model validations
            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .Include(v => v.Pod)
                .FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null || vehicle.Pod == null || vehicle.Pod.PodId != StartPodId || QuotedPricePerHour != vehicle.VehicleRate.PricePerHour)
            {
                // I don't think this should rerender the view for the user to try again, because something is really wrong here beyond just a validation error
                return BadRequest("Invalid booking");
            }

            var booking = new Booking
            {
                VehicleId = id,
                Id = user.Id,
                StartPodId = StartPodId,
                EndPodId = EndPodId,
                StartTime = StartTime,
                EndTime = EndTime,
                PricePerHour = QuotedPricePerHour,
                BookingStatus = BookingStatus.InProgress
            };

            // I can't make this validate, it might fix with binding or there might be changes to our model required
            TryValidateModel(booking);
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            // I'm just forcing creation for now. This is really bad.
            // I can't find a solution to these 3 validation errors:
            // The Vehicle field is required. (We set VehicleId)
            // The BookingId field is required. (We are creating a new Booking)
            // The ApplicationUser field is required. (I do set the Id field which represents a user)
            if (true || ModelState.IsValid)
            {
                vehicle.Pod.Vehicle = null;
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = booking.BookingId });
            }

            // Invalid model: Should rerender the view here with all the user correctable errors
            return Content("invalid");
        }

        // GET: Bookings/Edit/5
        // [Authorize] Todo need authorize to work
        public async Task<IActionResult> Edit(string id)
        {
            // TODO: need to get booking only if it belongs to logged in user
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize] TODO needs authorize
        public async Task<IActionResult> Edit(string id, DateTime StartTime, DateTime EndTime)
        {
            // All of this can be put into model validations
            if (
                StartTime.AddHours(1) > EndTime // minimum 1 hour booking
                || StartTime < DateTime.Now.AddMinutes(1) // booking start must be >= 1 minute from now
                || StartTime > DateTime.Now.AddDays(7) // booking can't start more than 1 week from now per Veronika's requirement
            )
            {
                // This should absolutely do the asp.net inline validation errors thing you guys know how to do
                return BadRequest("StartTime must be between 1 minute and 1 week from now.");
            }

            // TODO this needs to check the booking is associated with the logged in user
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            // This prevents changing of a completed journey but will need to be updated when we add more statuses
            if (booking.BookingStatus == BookingStatus.Complete) return BadRequest("Can't change a completed Booking");

            booking.StartTime = StartTime;
            booking.EndTime = EndTime;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = booking.BookingId });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(string id)
        {
            Console.WriteLine("=====");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            var pod = await _context.Pods.FindAsync(booking.EndPodId);

            // This null check should not be necessary as endPodId is a Foreign-Key but the compiler complains (I think there's an underlying model problem here)
            if (pod == null)
            {
                return NotFound();
            }
            // Prevent anyone returning an already returned car at a later date)
            if (booking.BookingStatus == BookingStatus.Complete)
            {
                return BadRequest("Can't return a completed Booking");
            }

            // TODO: need to check that the booking belongs to the logged in user before they can delete it

            // Return the car
            pod.VehicleId = booking.VehicleId;
            // Mark the booking complete
            booking.BookingStatus = BookingStatus.Complete;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = booking.BookingId });
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
