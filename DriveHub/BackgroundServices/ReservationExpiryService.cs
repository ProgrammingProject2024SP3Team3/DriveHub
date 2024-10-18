using DriveHub.Data;
using DriveHubModel;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.BackgroundServices
{
    public class ReservationExpiryService : BackgroundService
    {
        private readonly IServiceProvider _services;

        private readonly ILogger<ReservationExpiryService> _logger;

        public ReservationExpiryService(IServiceProvider services, ILogger<ReservationExpiryService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReservationExpiry service is running.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);

                _logger.LogInformation("ReservationExpiry service is waiting a minute.");

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReservationExpiry service is working.");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var reservations = await context.Bookings
                .Where(c => c.BookingStatus == BookingStatus.Reserved)
                .Where(c => c.Expires < DateTime.Now)
                .Include(c => c.Vehicle)
                .ToListAsync();

            foreach (var reservation in reservations)
            {
                reservation.BookingStatus = BookingStatus.Expired;
                reservation.Vehicle.IsReserved = false;
                context.Update(reservation);
                _logger.LogInformation($"Updating booking {reservation.BookingId} for {reservation.VehicleId}");
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ReservationExpiry service work complete.");
        }
    }
}
