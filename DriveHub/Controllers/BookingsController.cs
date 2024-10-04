using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using DriveHub.Models.Dto;
using System.Security.Principal;

namespace DriveHub.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public BookingsController(ApplicationDbContext context, ILogger<BookingsController> logger)
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
            throw new NotImplementedException();
        }

        public async Task<IActionResult> PastBookings()
        {
            throw new NotImplementedException();
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(string id)
        {
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
        public async Task<IActionResult> Create(string id)
        {
            // Get a vehicle with its rate, pod and site
            var vehicle = await _context.Vehicles.Where(c => c.VehicleId == id).Include(c => c.Pod).Include(c => c.VehicleRate).FirstOrDefaultAsync();

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || vehicle.Pod == null)
            {
                return NotFound($"Vehicle not found or not in a pod.");
            }

            // Get start and empty pods
            var startPod = await _context.Pods.Where(c => c.VehicleId == id).Include(c => c.Site).FirstOrDefaultAsync();
            var emptyPods = new List<PodVM>();
            var startPodVM = new PodVM();
            startPodVM.PodId = startPod.PodId;
            startPodVM.PodName = $"{startPod.Site.SiteName} Pod #{startPod.PodName}";
            emptyPods.Add(startPodVM);

            var pods = await _context.Pods.Where(c => c.VehicleId == null).Include(c => c.Site).ToListAsync();
            foreach (var pod in pods)
            {
                var podVM = new PodVM();
                podVM.PodId = pod.PodId;
                podVM.PodName = $"{pod.Site.SiteName} Pod #{pod.PodName}";
                emptyPods.Add(podVM);
            }

            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}";
            ViewBag.VehicleId = vehicle.VehicleId;
            ViewBag.StartPod = $"{startPod.Site.SiteName} Pod #{startPod.PodName}";
            ViewBag.StartPodId = startPod.PodId;
            ViewBag.StartSite = $"{startPod.Site.Address}, {startPod.Site.City}";
            ViewBag.StartSiteLatitude = startPod.Site.Latitude;
            ViewBag.StartSiteLongitude = startPod.Site.Longitude;
            ViewBag.PricePerHour = vehicle.VehicleRate.PricePerHour;
            ViewData["Pods"] = new SelectList(emptyPods, "PodId", "PodName", startPod.PodId);

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingDto bookingDto)
        {
            // Check if booking duration is less than 30 mins
            var diff = bookingDto.EndTime - bookingDto.StartTime;
            if (diff.TotalMinutes < 30)
            {
                ModelState.AddModelError("EndTime", "The minimum booking duration is 30 mins");
            }

            // Custom validation to ensure booking is within one week from today
            if (bookingDto.StartTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("StartTime", "Bookings cannot be made more than one week in advance.");
            }

            // Custom logic to check for overlapping bookings in the database
            var conflictingBookings = _context.Bookings
                .Where(b => b.VehicleId == bookingDto.VehicleId &&
                            ((b.StartTime <= bookingDto.EndTime && b.StartTime >= bookingDto.StartTime) ||
                             (b.EndTime <= bookingDto.EndTime && b.EndTime >= bookingDto.StartTime)))
                .Any();

            if (conflictingBookings)
            {
                ModelState.AddModelError("", "The selected vehicle is already booked during this time range.");
            }

            if (ModelState.IsValid)
            {
                Booking booking = new Booking();
                booking.VehicleId = bookingDto.VehicleId;
                booking.StartPodId = bookingDto.StartPodId;
                booking.EndPodId = bookingDto.EndPodId;
                booking.StartTime = bookingDto.StartTime;
                booking.EndTime = bookingDto.EndTime;
                booking.PricePerHour = bookingDto.QuotedPricePerHour;
                booking.BookingStatus = BookingStatus.InProgress;

                _context.Add(booking);
                await _context.SaveChangesAsync();

                var model = await _context.Bookings
                 .Where(c => c.VehicleId == bookingDto.VehicleId)
                 .Where(c => c.StartTime == bookingDto.StartTime)
                 .FirstOrDefaultAsync();

                return View("Done");
            }

            return View(bookingDto);
        }

        public IActionResult Done()
        {
            return View();
        }

        // GET: Bookings/Edit/5
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
        public async Task<IActionResult> Edit(BookingDto bookingDto)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(string id)
        {
            throw new NotImplementedException();
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
