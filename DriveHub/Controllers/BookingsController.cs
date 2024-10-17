using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using DriveHub.Models.Dto;
using Microsoft.AspNetCore.Identity;
using System.Dynamic;

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

        /// <summary>
        /// Return the error page when a booking action is not sane. Not publicly accessible.
        /// </summary>
        /// <returns>The error page</returns>
        private IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Search for a vehicle to book
        /// </summary>
        /// <returns>The vehicle search page</returns>
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

        /// <summary>
        /// Displays current (in-progress) bookings for the logged-in user.
        /// </summary>
        /// <returns>A view showing in-progress bookings</returns>
        public async Task<IActionResult> CurrentBookings()
        {
            var bookings = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.EndTime > DateTime.Now)
                .Include(c => c.Vehicle)
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                .ThenInclude(d => d.Site)
                .ToListAsync();

            if (!bookings.Any())
            {
                _logger.LogInformation("CurrentBookings: No active bookings found for the user.");
                ViewBag.Message = "You have no current bookings.";
            }

            return View(bookings);
        }

        public async Task<IActionResult> PastBookings()
        {
            var bookings = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.EndTime < DateTime.Now)
                .Include(c => c.Vehicle)
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                .ThenInclude(d => d.Site)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var booking = await _context.Bookings
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.Vehicle)
                .ThenInclude(d => d.VehicleRate)
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
            _logger.LogInformation($"Received a request to book vehicle {id}");

            // Get a vehicle with its rate, pod and site
            var vehicle = await _context.Vehicles
                .Where(c => c.VehicleId == id)
                .Include(c => c.Pod)
                .ThenInclude(c => c.Site)
                .Include(c => c.VehicleRate)
                .FirstOrDefaultAsync();

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || vehicle.Pod == null)
            {
                _logger.LogWarning($"Unable to find vehicle or not in pod {id}");
                return RedirectToAction(nameof(Error));
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

            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}. {vehicle.RegistrationPlate}";
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
        public async Task<IActionResult> Create(ReservationDto reservationDto)
        {
            _logger.LogInformation($"Received POST to make a reservation");
            _logger.LogInformation(reservationDto.ToString());

            var vehicle = await _context.Vehicles.Include(c => c.VehicleRate).FirstOrDefaultAsync(c => c.VehicleId == reservationDto.VehicleId);
            var startPod = await _context.Pods.Include(c => c.Site).FirstOrDefaultAsync(c => c.PodId == reservationDto.StartPodId);

            // Prevent users from posting illegal data combinations
            if (vehicle == null ||
                startPod == null ||
                vehicle?.VehicleRate.PricePerHour != reservationDto.QuotedPricePerHour ||
                startPod.VehicleId != vehicle?.VehicleId
                )
            {
                _logger.LogError($"User has posted illegal data {reservationDto}");
                return RedirectToAction(nameof(Error));
            }

            string? userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                _logger.LogError($"User is not logged in");
                ModelState.AddModelError("", "Your session has expired. Please login again.");
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Booking is valid");
                Booking booking = new Booking();
                booking.BookingId = reservationDto.BookingId;
                booking.VehicleId = reservationDto.VehicleId;
                booking.Id = userId;
                booking.StartPodId = reservationDto.StartPodId;
                booking.PricePerHour = vehicle.VehicleRate.PricePerHour;
                booking.BookingStatus = BookingStatus.Reserved;
                _context.Add(booking);
                _logger.LogInformation($"Added Booking OK");

                // Remove vehicle from pod
                startPod.VehicleId = null;
                startPod.Vehicle = null;
                _context.Update(startPod);

                await _context.SaveChangesAsync();

                return View("Details", booking);
            }

            // -- Return a page with data and errors if the model is not valid --
            _logger.LogError($"There was an error with booking {reservationDto.ToString()}");

            // Get start pod and empty pods
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

            // Get the view data
            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}";
            ViewBag.VehicleId = vehicle.VehicleId;
            ViewBag.StartPod = $"{startPod.Site.SiteName} Pod #{startPod.PodName}";
            ViewBag.StartPodId = startPod.PodId;
            ViewBag.StartSite = $"{startPod.Site.Address}, {startPod.Site.City}";
            ViewBag.StartSiteLatitude = startPod.Site.Latitude;
            ViewBag.StartSiteLongitude = startPod.Site.Longitude;
            ViewBag.PricePerHour = vehicle.VehicleRate.PricePerHour;
            ViewData["Pods"] = new SelectList(emptyPods, "PodId", "PodName", startPod.PodId);

            return View(reservationDto);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Id != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.Include(c => c.VehicleRate).FirstOrDefaultAsync(c => c.VehicleId == booking.VehicleId);
            var startPod = await _context.Pods.Include(c => c.Site).FirstOrDefaultAsync(c => c.PodId == booking.StartPodId);

            if (vehicle == null || startPod == null)
            {
                return NotFound();
            }

            // Get start and empty pods
            var emptyPods = new List<PodVM>();

            var pods = await _context.Pods.Where(c => c.VehicleId == null).Include(c => c.Site).ToListAsync();
            foreach (var pod in pods)
            {
                var podVM = new PodVM();
                podVM.PodId = pod.PodId;
                podVM.PodName = $"{pod.Site.SiteName} Pod #{pod.PodName}";
                emptyPods.Add(podVM);
            }

            // Create data object
            var editBookingDto = new ReservationDto();
            editBookingDto.BookingId = booking.BookingId;
            editBookingDto.VehicleId = booking.VehicleId;
            editBookingDto.StartPodId = booking.StartPodId;
            editBookingDto.QuotedPricePerHour = booking.PricePerHour;

            ViewBag.BookingId = booking.BookingId;
            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}. {vehicle.RegistrationPlate}";
            ViewBag.VehicleId = vehicle.VehicleId;
            ViewBag.StartPod = $"{startPod.Site.SiteName} Pod #{startPod.PodName}";
            ViewBag.StartPodId = startPod.PodId;
            ViewBag.StartSite = $"{startPod.Site.Address}, {startPod.Site.City}";
            ViewBag.StartSiteLatitude = startPod.Site.Latitude;
            ViewBag.StartSiteLongitude = startPod.Site.Longitude;
            ViewBag.PricePerHour = vehicle.VehicleRate.PricePerHour;
            ViewData["Pods"] = new SelectList(emptyPods, "PodId", "PodName", startPod.PodId);

            return View(editBookingDto);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ReservationDto bookingDto)
        {
            if (id != bookingDto.BookingId)
            {
                return NotFound();
            }

            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.Include(c => c.VehicleRate).FirstOrDefaultAsync(c => c.VehicleId == booking.VehicleId);
            var startPod = await _context.Pods.Include(c => c.Site).FirstOrDefaultAsync(c => c.PodId == booking.StartPodId);

            if (ModelState.IsValid)
            {
                booking.BookingStatus = BookingStatus.Reserved;
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return View("Details", booking);
            }

            // Get start pod and empty pods
            var emptyPods = new List<PodVM>();
            var pods = await _context.Pods.Where(c => c.VehicleId == null).Include(c => c.Site).ToListAsync();
            foreach (var pod in pods)
            {
                var podVM = new PodVM();
                podVM.PodId = pod.PodId;
                podVM.PodName = $"{pod.Site.SiteName} Pod #{pod.PodName}";
                emptyPods.Add(podVM);
            }

            ViewBag.BookingId = booking.BookingId;
            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}. {vehicle.RegistrationPlate}";
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
            return View();
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId.ToString() == id);
        }

        private static decimal BookingPriceCalculator(Vehicle vehicle, DateTime startTime, DateTime endTime)
        {
            var diff = endTime - startTime;
            var price = (decimal)diff.TotalMinutes * vehicle.VehicleRate.PricePerHour / 60m;
            return price;
        }
    }
}
