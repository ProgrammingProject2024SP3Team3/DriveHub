using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using DriveHub.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace DriveHub.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(
            ApplicationDbContext context,
            ILogger<BookingsController> logger,
            UserManager<IdentityUser> userManager
        )
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
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

        public IActionResult CurrentBookings()
        {
            throw new NotImplementedException();
        }

        public IActionResult PastBookings()
        {
            throw new NotImplementedException();
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var booking = await _context.Bookings
                .Include(c => c.StartPod)
                .Include(c => c.EndPod)
                .Include(c => c.Vehicle)
                .FirstOrDefaultAsync(m => m.BookingId == id);

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
            var vehicle = await _context.Vehicles.Where(c => c.VehicleId == id).Include(c => c.Pod).ThenInclude(c => c.Site).Include(c => c.VehicleRate).FirstOrDefaultAsync();

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || vehicle.Pod == null)
            {
                _logger.LogWarning("Unable to find vehicle");
                return RedirectToAction(nameof(Search));
            }

            // Get start and empty pods
            var startPod = vehicle.Pod;
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

            // Check if booking is within one week from today
            if (bookingDto.StartTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("StartTime", "Bookings cannot be made more than one week in advance.");
            }

            // Check if booking is within one week from today
            if (bookingDto.StartTime > bookingDto.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start time is after end time");
            }

            // Custom logic to check for overlapping bookings in the database
            var conflictingBookings = _context.Bookings
                .Where(b => b.VehicleId == bookingDto.VehicleId &&
                            ((b.StartTime <= bookingDto.EndTime && b.StartTime >= bookingDto.StartTime) ||
                             (b.EndTime <= bookingDto.EndTime && b.EndTime >= bookingDto.StartTime)))
                .Any();

            if (conflictingBookings)
            {
                ModelState.AddModelError("VehicleId", "The selected vehicle is already booked during this time range.");
            }

            var vehicle = await _context.Vehicles.FindAsync(bookingDto.VehicleId);
            var currentPod = await _context.Pods.Where(c => c.PodId == bookingDto.StartPodId).Include(c => c.Site).FirstOrDefaultAsync();

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || currentPod == null || currentPod.VehicleId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                Booking booking = new Booking();
                var userId = _userManager.GetUserId(User);

                booking.Id = userId;
                booking.BookingId = Guid.NewGuid().ToString();
                booking.VehicleId = bookingDto.VehicleId;
                booking.StartPodId = bookingDto.StartPodId;
                booking.EndPodId = bookingDto.EndPodId;
                booking.StartTime = bookingDto.StartTime;
                booking.EndTime = bookingDto.EndTime;
                booking.PricePerHour = bookingDto.QuotedPricePerHour;
                booking.BookingStatus = BookingStatus.InProgress;
                _context.Add(booking);

                // Remove vehicle from pod
                currentPod.VehicleId = null;
                currentPod.Vehicle = null;
                _context.Update(currentPod);

                await _context.SaveChangesAsync();

                return View("Details", booking);
            }

            // Get start and empty pods
            var startPod = currentPod;
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
        public IActionResult Edit(BookingDto bookingDto)
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
                .FirstOrDefaultAsync(m => m.BookingId.ToString() == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(string id)
        {
            throw new NotImplementedException();
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId.ToString() == id);
        }
    }
}
