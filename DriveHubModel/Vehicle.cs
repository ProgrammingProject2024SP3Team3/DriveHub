using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Vehicle
    {
        [Key]
        public Guid VehicleID { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        public int Year { get; set; }
        public int Mileage { get; set; }

        [ForeignKey("Location")]
        public Guid LocationID { get; set; }
        public bool IsAvailable { get; set; }

        // Navigation properties
        public Location Location { get; set; }

        // One-to-Many relationship: A Vehicle can have many Bookings
        public ICollection<Booking> Bookings { get; set; }
    }
}
