using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Admin.Models.Dto;

namespace Admin.Controllers
{
    [Authorize]
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUsersController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VehicleRates
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        // GET: VehicleRates/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleRate = await _context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vehicleRate == null)
            {
                return NotFound();
            }

            return View(vehicleRate);
        }

        // GET: VehicleRates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleRates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }

        // GET: VehicleRates/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: VehicleRates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed")] Admin.View.ApplicationUsers.Edit applicationUser)
        {
            ApplicationUser user;
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            user = await _context.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.FirstName = applicationUser.FirstName;
                user.LastName = applicationUser.LastName;
                user.Email = applicationUser.Email;
                user.EmailConfirmed = applicationUser.EmailConfirmed;
                user.PhoneNumber = applicationUser.PhoneNumber;
                user.PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed;

                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(applicationUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: VehicleRates/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: VehicleRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser != null)
            {
                _context.Users.Remove(applicationUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
