using DriveHub.Models;
using DriveHubModel;
using Microsoft.AspNetCore.Mvc;
using DriveHub.Data;
using System.Diagnostics;

namespace DriveHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FourOFour()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        // GET: Pods/Create
        public IActionResult ContactUs()
        {
            return View();
        }

        // POST: Pods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs([Bind("Name,Email,Subject,Message")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return View(nameof(MessageReceived));
            }
            return View(contact);
        }

        public IActionResult Subscribe()
        {
            return View();
        }

        private IActionResult MessageReceived()
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
