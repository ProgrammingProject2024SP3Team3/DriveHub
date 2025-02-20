﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using DriveHubModel;
using Microsoft.AspNetCore.Authorization;
using Admin.Views.Vehicles;

namespace Admin.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger _logger;

        public VehiclesController(ApplicationDbContext context, ILogger<Controller> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.VehicleRate).Include(v => v.Pod);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .Include(v => v.Pod)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,VehicleRateId,Make,Model,RegistrationPlate,State,Year,Seats,Colour,Name,IsReserved")] Views.Vehicles.Create vehicleDto)
        {
            if (ModelState.IsValid)
            {
                var vehicle = new Vehicle();
                vehicle.VehicleId = vehicleDto.VehicleId;
                vehicle.VehicleRateId = vehicleDto.VehicleRateId;
                vehicle.Make = vehicleDto.Make;
                vehicle.Model = vehicleDto.Model;
                vehicle.RegistrationPlate = vehicleDto.RegistrationPlate;
                vehicle.State = vehicleDto.State;
                vehicle.Year = vehicleDto.Year;
                vehicle.Seats = vehicleDto.Seats;
                vehicle.IsReserved = vehicleDto.IsReserved;
                vehicle.Name = vehicleDto.Name;
                vehicle.Colour = vehicleDto.Colour;

                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = vehicle.VehicleId });
            }
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicleDto.VehicleRateId);
            return View(vehicleDto);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicle.VehicleRateId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Views.Vehicles.Edit vehicleDto)
        {
            _logger.LogInformation($"Editing {id}");
            _logger.LogInformation($"Editing {vehicleDto.ToString()}");

            if (id != vehicleDto.VehicleId)
            {
                _logger.LogWarning($"No match for {id}");
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning($"No match for {id}");
                return NotFound();
            }

            if (!_context.VehicleRates.Any(e => e.VehicleRateId == vehicleDto.VehicleRateId))
            {
                _logger.LogWarning($"No match for rate {vehicleDto.VehicleRateId}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogWarning($"Model is valid {id}");

                    vehicle.VehicleRateId = vehicleDto.VehicleRateId;
                    vehicle.Name = vehicleDto.Name;
                    vehicle.Make = vehicleDto.Make;
                    vehicle.Model = vehicleDto.Model;
                    vehicle.RegistrationPlate = vehicleDto.RegistrationPlate;
                    vehicle.State = vehicleDto.State;
                    vehicle.Year = vehicleDto.Year;
                    vehicle.Seats = vehicleDto.Seats;
                    vehicle.Colour = vehicleDto.Colour;
                    vehicle.IsReserved = vehicleDto.IsReserved;

                    _context.Update(vehicle);
                    _logger.LogWarning($"Updated {id}");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
                    {
                        _logger.LogWarning($"Database error {id}");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _logger.LogWarning($"Success editing {id}");
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning($"Failed editing {id}");
            ViewData["VehicleRateId"] = new SelectList(_context.VehicleRates, "VehicleRateId", "Description", vehicle.VehicleRateId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleRate)
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicle = await _context.Vehicles.Where(c => c.VehicleId == id).Include(c => c.Pod).FirstOrDefaultAsync();

            if (vehicle != null)
            {
                var podId = vehicle.Pod?.PodId;
                var pod = await _context.Pods.FindAsync(podId);
                if (pod != null)
                {
                    pod.VehicleId = null;
                    _context.Pods.Update(pod);
                }
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
