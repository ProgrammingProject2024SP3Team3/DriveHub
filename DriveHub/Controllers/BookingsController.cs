﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            // Redirect if user has a current reservation
            var hasReservation = _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(
                    c => c.BookingStatus == BookingStatus.Reserved ||
                    c.BookingStatus == BookingStatus.Unpaid ||
                    c.BookingStatus == BookingStatus.Collected
                    )
                .Any();

            if (hasReservation)
            {
                ViewBag.Message = "You have a current booking";
                return RedirectToAction(nameof(Current));
            }

            var vehicles = await _context.Vehicles.Where(c => c.IsReserved == false).Include(c => c.VehicleRate).ToListAsync();
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

        // GET: Bookings/Create
        public async Task<IActionResult> Create(string id)
        {
            _logger.LogInformation($"Received a request to book vehicle {id}");

            // Redirect if user has a current reserveration
            var hasReservation = _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(
                    c => c.BookingStatus == BookingStatus.Reserved ||
                    c.BookingStatus == BookingStatus.Unpaid ||
                    c.BookingStatus == BookingStatus.Collected
                    )
                .Any();

            if (hasReservation)
            {
                ViewBag.Message = "You have a current booking";
                return RedirectToAction(nameof(Current));
            }

            // Get a vehicle with its rate, pod and site
            var vehicle = await _context.Vehicles
                .Where(c => c.VehicleId == id)
                .Include(c => c.Pod)
                .ThenInclude(c => c.Site)
                .Include(c => c.VehicleRate)
                .FirstOrDefaultAsync();

            // If we couldn't find the vehicle or it's not currently in a pod then bail out
            if (vehicle == null || vehicle.Pod == null || vehicle.IsReserved == true)
            {
                _logger.LogWarning($"Unable to find vehicle or is reserved {id}");
                return View(nameof(Error));
            }

            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}. {vehicle.RegistrationPlate}";
            ViewBag.VehicleId = vehicle.VehicleId;
            ViewBag.StartPod = $"{vehicle.Pod.Site.SiteName} Pod #{vehicle.Pod.PodName}";
            ViewBag.StartPodId = vehicle.Pod.PodId;
            ViewBag.StartSite = $"{vehicle.Pod.Site.Address}, {vehicle.Pod.Site.City}";
            ViewBag.StartSiteLatitude = vehicle.Pod.Site.Latitude;
            ViewBag.StartSiteLongitude = vehicle.Pod.Site.Longitude;
            ViewBag.PricePerHour = vehicle.VehicleRate.PricePerHour;

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationDto reservationDto)
        {
            _logger.LogInformation($"Received POST to make a reservation");
            _logger.LogInformation(reservationDto.ToString());

            // Redirect if user has a current reserveration
            var hasReservation = _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(
                    c => c.BookingStatus == BookingStatus.Reserved ||
                    c.BookingStatus == BookingStatus.Unpaid ||
                    c.BookingStatus == BookingStatus.Collected
                    )
                .Any();

            if (hasReservation)
            {
                ViewBag.Message = "You have a current booking";
                return RedirectToAction(nameof(Current));
            }

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
                return View(nameof(Error));
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Booking is valid");
                Booking booking = new Booking(
                    _userManager.GetUserId(User),
                    reservationDto.VehicleId,
                    reservationDto.StartPodId,
                    vehicle.VehicleRate.PricePerHour
                    );

                booking.Vehicle = vehicle;
                booking.StartPod = startPod;
                _context.Add(booking);
                _logger.LogInformation($"Added Booking OK");

                // Complete the reservation
                vehicle.IsReserved = true;
                _context.Add(booking);
                _context.Update(vehicle);
                await _context.SaveChangesAsync();

                return View("Details", booking);
            }

            // -- Return a page with data and errors if the model is not valid --
            _logger.LogError($"There was an error with booking {reservationDto.ToString()}");

            ViewBag.Vehicle = $"{vehicle.Name} the {vehicle.Make} {vehicle.Model}. {vehicle.RegistrationPlate}";
            ViewBag.VehicleId = vehicle.VehicleId;
            ViewBag.StartPod = $"{vehicle.Pod.Site.SiteName} Pod #{vehicle.Pod.PodName}";
            ViewBag.StartPodId = vehicle.Pod.PodId;
            ViewBag.StartSite = $"{vehicle.Pod.Site.Address}, {vehicle.Pod.Site.City}";
            ViewBag.StartSiteLatitude = vehicle.Pod.Site.Latitude;
            ViewBag.StartSiteLongitude = vehicle.Pod.Site.Longitude;
            ViewBag.PricePerHour = vehicle.VehicleRate.PricePerHour;

            return View(reservationDto);
        }

        /// <summary>
        /// Displays current (in-progress) bookings for the logged-in user.
        /// </summary>
        /// <returns>A view showing in-progress bookings</returns>
        public async Task<IActionResult> Current()
        {
            var booking = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(
                    c => c.BookingStatus == BookingStatus.Reserved ||
                    c.BookingStatus == BookingStatus.Unpaid ||
                    c.BookingStatus == BookingStatus.Collected
                    )
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleRate)
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogInformation("Current: No active reservation found for the user.");
                return RedirectToAction(nameof(Search));
            }

            return View(booking);
        }

        /// <summary>
        /// Displays current (in-progress) bookings for the logged-in user.
        /// </summary>
        /// <returns>A view showing in-progress bookings</returns>
        public async Task<IActionResult> Extend(string id)
        {
            var booking = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Reserved)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogInformation("Current: No active reservation found for the user.");
                return View(nameof(Error));
            }

            booking.Expires = booking.Expires.AddMinutes(30);
            booking.IsExtended = true;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Current));
        }

        public async Task<IActionResult> Past()
        {
            var bookings = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus != BookingStatus.Reserved)
                .Where(c => c.BookingStatus != BookingStatus.Collected)
                .Include(c => c.Vehicle)
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.Receipt)
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
                .Include(c => c.Receipt)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Cancel(string id)
        {
            var booking = await _context.Bookings
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.Vehicle)
                .ThenInclude(d => d.VehicleRate)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            return View(booking);
        }

        // POST: Bookings/Create
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(string id)
        {
            _logger.LogInformation($"CancelConfirmed: Cancelling booking {id}.");
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.BookingStatus != BookingStatus.Reserved)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(c => c.VehicleId == booking.VehicleId);

            if (vehicle == null)
            {
                return View(nameof(Error));
            }

            booking.BookingStatus = BookingStatus.Cancelled;
            vehicle.IsReserved = false;

            _context.Update(booking);
            _context.Update(vehicle);
            await _context.SaveChangesAsync();

            return View(nameof(Index));
        }

        /// <summary>
        /// Return the error page when a booking action is not sane. Not publicly accessible.
        /// </summary>
        /// <returns>The error page</returns>
        private IActionResult Pay()
        {
            return View();
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId.ToString() == id);
        }
    }
}
