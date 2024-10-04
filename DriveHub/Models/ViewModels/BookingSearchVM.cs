using DriveHubModel;

namespace DriveHub.Models.ViewModels
{
    public class BookingSearchVM
    {
        public IList<Vehicle> Vehicles { get; set; }

        public IList<int> Seats { get; set; }

        public IList<VehicleRate> VehicleRates { get; set; }

        public IList<Pod> Pods { get; set; }

        public BookingSearchVM(
            List<Vehicle> vehicles,
            IList<int> seats,
            List<VehicleRate> vehicleRates,
            List<Pod> pods
        )
        {
            Vehicles = vehicles;
            Seats = seats;
            VehicleRates = vehicleRates;
            Pods = pods;
        }
    }
}
