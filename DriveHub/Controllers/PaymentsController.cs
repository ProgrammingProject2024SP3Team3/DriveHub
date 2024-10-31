using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DriveHub.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public PaymentsController(
            ApplicationDbContext context,
            ILogger<BookingsController> logger,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration
        )
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// The payment was successful
        /// </summary>
        /// <param name="id">PaymentId</param>
        /// <returns>RedirectToAction</returns>
        public async Task<IActionResult> Success(string id)
        {
            _logger.LogInformation($"Payment successful for {id}");

            var booking = await _context.Bookings
                  .Where(c => c.PaymentId == id)
                  .Include(c => c.Invoice)
                  .FirstOrDefaultAsync();

            if (booking == null || booking.Invoice == null)
            {
                _logger.LogError($"Bad booking id {id}");
                return RedirectToAction("Error", "Bookings");
            }

            Receipt receipt = new Receipt();
            receipt.Amount = booking.Invoice.Amount;
            booking.Receipt = receipt;
            booking.BookingStatus = BookingStatus.Complete;

            _context.Add(receipt);
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
        }

        /// <summary>
        /// The payment was not successful
        /// </summary>
        /// <param name="id">PaymentId</param>
        /// <returns>RedirectToAction</returns>
        public async Task<IActionResult> Cancel(string id)
        {
            _logger.LogInformation($"Payment failed for {id}");

            if (id == null)
            {
                _logger.LogError($"Bad payment id {id}");
                return RedirectToAction("Error", "Bookings");
            }

            var booking = await _context.Bookings
              .Where(c => c.PaymentId == id)
              .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogError($"Bad booking id {id}");
                return RedirectToAction("Error", "Bookings");
            }

            return RedirectToAction("Details", "Bookings", new { id = booking.BookingId });
        }
    }
}