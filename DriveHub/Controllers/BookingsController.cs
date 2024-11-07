using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DriveHub.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;
using Stripe;
using DriveHub.Models.DocumentModels;
using QuestPDF.Fluent;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace DriveHub.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public BookingsController(
            ApplicationDbContext context,
            ILogger<BookingsController> logger,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
        )
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
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
            var pods = await _context.Pods.Where(c => c.VehicleId != null).Where(c => c.Vehicle.IsReserved == false).Include(c => c.Site).Include(c => c.Vehicle).OrderBy(c => c.SiteId).ToListAsync();

            var bookingSearchVM = new BookingSearchVM(
                seats,
                vehicleRates,
                pods
                );
            return View("Search", bookingSearchVM);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create(string id)
        {
            _logger.LogInformation($"Received a request to book vehicle {id}");

            // Check if user already has an active reservation
            var hasReservation = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .AnyAsync(c => c.BookingStatus == BookingStatus.Reserved ||
                            c.BookingStatus == BookingStatus.Unpaid ||
                            c.BookingStatus == BookingStatus.Collected);

            if (hasReservation)
            {
                ViewBag.Message = "User has a current booking";
                return RedirectToAction(nameof(Current));
            }

            // Fetch vehicle data including related entities
            var vehicle = await _context.Vehicles
                .Include(v => v.Pod)
                .ThenInclude(p => p.Site)
                .Include(v => v.VehicleRate)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null || vehicle.Pod == null || vehicle.IsReserved)
            {
                _logger.LogWarning($"Vehicle unavailable or already reserved: {id}");
                return View(nameof(Error));
            }

            ViewBag.Vehicle = vehicle;
            return View(nameof(Create));
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VehicleId,StartPodId,QuotedPricePerHour")] DriveHub.Views.Bookings.Create create)
        {
            _logger.LogInformation($"Received POST to make a reservation for vehicle {create.VehicleId}");

            try
            {
                // Verify if the user has an active reservation
                bool hasReservation = await _context.Bookings
                    .AnyAsync(c => c.Id == _userManager.GetUserId(User) &&
                                (c.BookingStatus == BookingStatus.Reserved ||
                                    c.BookingStatus == BookingStatus.Unpaid ||
                                    c.BookingStatus == BookingStatus.Collected));

                if (hasReservation)
                {
                    ViewBag.Message = "You have a current booking.";
                    return RedirectToAction(nameof(Current));
                }

                // Start transaction to ensure atomic operations
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Fetch vehicle and ensure it exists
                var vehicle = await _context.Vehicles
                    .Include(v => v.VehicleRate)
                    .FirstOrDefaultAsync(v => v.VehicleId == create.VehicleId);


                if (vehicle == null || vehicle.IsReserved)
                {
                    _logger.LogWarning("Vehicle is unavailable or already reserved.");
                    ViewBag.Message = "This vehicle has already been reserved.";
                    return View(nameof(Error));
                }

                // Fetch the start pod and validate it
                var startPod = await _context.Pods
                    .Include(p => p.Site)
                    .Include(p => p.Vehicle)
                    .FirstOrDefaultAsync(p => p.PodId == create.StartPodId);

                if (startPod == null || startPod.Vehicle == null)
                {
                    _logger.LogError("Invalid start pod or vehicle not in pod.");
                    return View(nameof(Error));
                }

                // Prepare and validate the booking
                var booking = new Booking(
                    _userManager.GetUserId(User),
                    create.VehicleId,
                    create.StartPodId,
                    vehicle.VehicleRate.PricePerHour,
                    vehicle.VehicleRate.PricePerMinute
                )
                {
                    Vehicle = vehicle,
                    StartPod = startPod
                };

                // Reserve the vehicle
                vehicle.IsReserved = true;

                // Save booking and vehicle updates in a transaction
                _context.Add(booking);
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Reservation successfully created.");
                return View("Details", booking);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Concurrency issue: Vehicle was reserved by another user.");
                ViewBag.Message = "The vehicle was just reserved by someone else.";
                return View(nameof(Error));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation.");
                return View(nameof(Error));
            }
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
                    c =>
                    c.BookingStatus == BookingStatus.Reserved ||
                    c.BookingStatus == BookingStatus.Unpaid ||
                    c.BookingStatus == BookingStatus.Collected
                    )
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleRate)
                .Include(c => c.StartPod)
                .ThenInclude(d => d.Site)
                .Include(c => c.Invoice)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogInformation("Current: No active reservation found for the user.");
                return RedirectToAction(nameof(Search));
            }

            return View("Details", booking);
        }

        /// <summary>
        /// Displays current (in-progress) bookings for the logged-in user.
        /// </summary>
        /// <returns>A view showing in-progress bookings</returns>
        public async Task<IActionResult> Extend(string id)
        {
            var booking = await _context.Bookings
                .Where(c => c.BookingId == id)
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Reserved)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogWarning("Extend: No active reservation found for the user.");
                return View(nameof(Error));
            }

            if (booking.IsExtended)
            {
                _logger.LogWarning("Extend: Reservation already extended.");
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
                .Include(c => c.Invoice)
                .Include(c => c.Receipt)
                .OrderByDescending(c => c.Expires)
                .ToListAsync();

            return View("Past", bookings);
        }

        public async Task<IActionResult> Details(string id)
        {
            var booking = await _context.Bookings
                .Include(c => c.StartPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.Vehicle)
                    .ThenInclude(d => d.VehicleRate)
                .Include(c => c.Invoice)
                .Include(c => c.Receipt)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View("Details", booking);
        }

        public async Task<IActionResult> Cancel(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(m => m.BookingId == id)
                .Include(c => c.StartPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.Vehicle)
                    .ThenInclude(d => d.VehicleRate)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound();
            }

            return View("Cancel", booking);
        }

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

            return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
        }

        /// <summary>
        /// Pay an invoice
        /// </summary>
        /// <returns>StatusCodeResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(string BookingId)
        {
            _logger.LogInformation($"Pay: Starting payment process for booking {BookingId}.");

            if (string.IsNullOrWhiteSpace(BookingId))
            {
                _logger.LogWarning("Pay: BookingId is null or empty.");
                return View(nameof(Error));
            }

            var booking = await _context.Bookings
                .AsNoTracking()
                .Where(c => c.BookingId == BookingId)
                .Where(c => c.BookingStatus == BookingStatus.Unpaid)
                .Include(c => c.Vehicle)
                    .ThenInclude(c => c.VehicleRate)
                .Include(c => c.Invoice)
                .FirstOrDefaultAsync();

            if (booking == null || booking.Invoice == null)
            {
                _logger.LogWarning($"Pay: Booking not found for BookingId: {BookingId}");
                return View(nameof(Error));
            }

            var apiKey = _configuration.GetValue<string>("StripeKey");
            var client = new StripeClient(apiKey);
            var domain = _configuration.GetValue<string>("Domain");

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "aud", // Australian dollars
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"DriveHub - {booking.BookingId}",
                                Description = "Payment for your Drivehub ride",
                            },
                            UnitAmount = (long)(booking.Invoice.Amount * 100),
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{domain}/Payments/Success/{booking.PaymentId}",
                CancelUrl = $"{domain}/Payments/Cancel/{booking.PaymentId}",
            };
            try
            {
                var service = new SessionService(client);
                Session session = service.Create(options);
                Response.Headers.Append("Location", session.Url);
                _logger.LogInformation("Pay: Stripe session created successfully.");
                return new StatusCodeResult(303);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Pay: Error creating Stripe session.");
                return View(nameof(Error));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pay: Unexpected error.");
                return View(nameof(Error));
            }
        }

        public async Task<IActionResult> PrintInvoice(int id)
        {
            Settings.License = LicenseType.Community;

            var booking = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.Invoice.InvoiceNumber == id)
                .Where(c => c.BookingStatus == BookingStatus.Complete)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Vehicle)
                .Include(c => c.StartPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.Invoice)
                .Include(c => c.Receipt)
                .FirstOrDefaultAsync();

            if (booking == null || booking.Receipt == null)
            {
                _logger.LogWarning($"Receipt not found: {id}");
                return NotFound();
            }

            _logger.LogInformation($"Generated invoice: {id}");

            try
            {
                var doc = new InvoiceDocument(booking);
                var pdf = doc.GeneratePdf();
                return File(pdf, "application/pdf", $"DriveHub Inv{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> PrintReport()
        {
            Settings.License = LicenseType.Community;

            var bookings = await _context.Bookings
                .Where(c => c.Id == _userManager.GetUserId(User))
                .Where(c => c.BookingStatus == BookingStatus.Complete)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Vehicle)
                .Include(c => c.StartPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.EndPod)
                    .ThenInclude(d => d.Site)
                .Include(c => c.Invoice)
                .Include(c => c.Receipt)
                .ToListAsync();

            try
            {
                var doc = new BookingsDocument(bookings);
                var pdf = doc.GeneratePdf();
                return File(pdf, "application/pdf", $"DriveHub Booking Report.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating booking report");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool BookingExists(string id)
        {
            return _context.Bookings.Any(e => e.BookingId.ToString() == id);
        }
    }
}
