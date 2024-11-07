using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Admin.Controllers
{
    [Authorize]
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationUsersController> _logger;

        public ApplicationUsersController(
            ApplicationDbContext context,
            ILogger<ApplicationUsersController> logger)
        {
            _context = context;
            _logger = logger;
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
        public async Task<IActionResult> Create(string id, [Bind("Id,UserName,FirstName,LastName,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed,Password,ConfirmPassword")] Admin.Views.ApplicationUsers.Create create)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Id = create.Id,
                    UserName = create.UserName,
                    NormalizedUserName = create.UserName.ToUpper(),
                    Email = create.Email,
                    NormalizedEmail = create.Email.ToUpper(),
                    EmailConfirmed = create.EmailConfirmed,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(), 
                    PhoneNumber = create.PhoneNumber,
                    PhoneNumberConfirmed = create.PhoneNumberConfirmed,
                    TwoFactorEnabled = false,
                    LockoutEnd = null, 
                    LockoutEnabled = false,
                    AccessFailedCount = 0, 
                    FirstName = create.FirstName,
                    LastName = create.LastName,
                };

                var paw = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = paw.HashPassword(user, create.Password);

                if (!_context.Users.Any(u => u.Id == user.Id))
                {
                    _context.Users.Add(user);
                }
                else
                {
                    // Optionally handle case where user already exists
                }

                var result = await _context.SaveChangesAsync(); 

                if (result > 0)
                {
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToAction(nameof(Details), new { id = user.Id });
                }
            }
            ViewBag.Error = "There was an error creating the user. Please review your data entry and try again."; 
            return View(create);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed")] Admin.Views.ApplicationUsers.Edit applicationUser)
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

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

    }
}
