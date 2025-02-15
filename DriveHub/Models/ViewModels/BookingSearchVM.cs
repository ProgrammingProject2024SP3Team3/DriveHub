﻿using DriveHubModel;

namespace DriveHub.Models.ViewModels
{
    public class BookingSearchVM
    {
        public IList<int> Seats { get; set; }

        public IList<VehicleRate> VehicleRates { get; set; }

        public IList<Pod> Pods { get; set; }

        public BookingSearchVM(
            IList<int> seats,
            List<VehicleRate> vehicleRates,
            List<Pod> pods
        )
        {
            Seats = seats;
            VehicleRates = vehicleRates;
            Pods = pods;
        }
    }
}
