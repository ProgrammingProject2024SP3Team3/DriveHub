using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DriveHub.Data;
using DriveHub.Models.Dto;
using DriveHubModel;

namespace DriveHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmptyPods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PodApiDto>>> GetPods()
        {
            var dtos = new List<PodApiDto>();
            var pods = await _context.Pods.Include(c => c.Site).Include(c => c.Vehicle).Include(c => c.Vehicle.VehicleRate).ToListAsync();

            foreach (var pod in pods)
            {
                dtos.Add(GetPodDto(pod));
            }

            return dtos;
        }

        // GET: api/EmptyPods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PodApiDto>> GetPod(string id)
        {
            var pod = await _context.Pods.Where(c => c.PodId == id).Include(c => c.Site).Include(c => c.Vehicle).Include(c => c.Vehicle.VehicleRate).FirstOrDefaultAsync();

            if (pod == null)
            {
                return NotFound();
            }

            var dto = GetPodDto(pod);

            return dto;
        }

        private bool PodExists(string id)
        {
            return _context.Pods.Any(e => e.PodId == id);
        }

        private static PodApiDto GetPodDto(Pod pod)
        {
            var dto = new PodApiDto();
            dto.PodId = pod.PodId;
            dto.PodName = pod.PodName;
            dto.SiteName = pod.Site.SiteName;
            dto.Address = pod.Site.Address;
            dto.City = pod.Site.City;
            dto.PostCode = pod.Site.PostCode;
            dto.Latitude = pod.Site.Latitude;
            dto.Longitude = pod.Site.Longitude;
            dto.VehicleId = pod.VehicleId;
            dto.VehicleName = pod.Vehicle?.Name;
            dto.Make = pod.Vehicle?.Make;
            dto.Model = pod.Vehicle?.Model;
            dto.RegistrationPlate = pod.Vehicle?.RegistrationPlate;
            dto.Seats = pod.Vehicle?.Seats;
            dto.Colour = pod.Vehicle?.Colour;
            dto.VehicleCategory = pod.Vehicle?.VehicleRate.Description;
            dto.PricePerHour = pod.Vehicle?.VehicleRate.PricePerHour;

            return dto;
        }
    }
}
