using DriveHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DriveHub.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ILogger<BookingsController> logger)
        {
            _logger = logger;
        }

        // Booking Landing/Search Page
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
