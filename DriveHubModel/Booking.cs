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

        [ForeignKey("Pod")]
        public Guid StartPodID { get; set; }

        [ForeignKey("Pod")]
        public Guid EndPodID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public AspNetUser User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Pod StartPod { get; set; }
        public Pod EndPod { get; set; }

        // One-to-One relationship: A Booking generates one Receipt
        public Receipt Receipt { get; set; }

        // One-to-One relationship: A Booking generates one Journey
        public Journey Journey { get; set; }
    }
}
