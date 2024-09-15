

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DriveHubModel
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleID { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Seats { get; set; }

        // One-to-Many relationship: A Vehicle can have many Bookings
        //public ICollection<Booking> Bookings { get; set; }
    }
}
