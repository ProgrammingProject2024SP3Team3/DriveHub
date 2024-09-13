using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Booking
    {
        [Key]
        public Guid BookingID { get; set; }

        [ForeignKey("AspNetUser")]
        public string UserID { get; set; }

        [ForeignKey("Vehicle")]
        public Guid VehicleID { get; set; }

        [ForeignKey("Location")]
        public Guid StartLocationID { get; set; }

        [ForeignKey("Location")]
        public Guid EndLocationID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public AspNetUser User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }
        
        // One-to-One relationship: A Booking generates one Receipt
        public Receipt Receipt { get; set; }
    }
}
