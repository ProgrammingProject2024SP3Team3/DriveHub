using Admin.Models;
using Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Admin.Models.Dto;

namespace Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Calculate revenue per day
            var revenuePerDay = await _context.Receipts
                .GroupBy(r => r.DateTime.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(r => r.Amount)
                })
                .ToListAsync();

            var model = new Admin.Views.Home.Index
            {
                NumberOfUsers = await _context.Users.CountAsync(),
                NumberOfTripsTaken = await _context.Invoices.CountAsync(),
                CarsUsed = await _context.Vehicles.CountAsync(v => v.IsReserved),
                CarsTotal = await _context.Vehicles.CountAsync(),
                TotalRevenue = revenuePerDay.Sum(r => r.Revenue),
            };

            ViewBag.RevenuePerDay = revenuePerDay;

            return View(model);
        }

        public IActionResult About()
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
