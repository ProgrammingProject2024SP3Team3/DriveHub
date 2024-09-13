using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Journey
    {
        [Key]
        public Guid JourneyID { get; set; }

        [ForeignKey("Booking")]
        public Guid BookingID { get; set; }

        [ForeignKey("Pod")]
        public Guid StartPodID { get; set; }

        [ForeignKey("Pod")]
        public Guid EndPodID { get; set; }

        public decimal DistanceTravelled { get; set; }
        public TimeSpan Duration { get; set; }
        public string RouteDetails { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Pod StartPod { get; set; }
        public Pod EndPod { get; set; }
    }
}
