using DriveHubModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Admin.Views.Bookings
{
    public class Edit
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string StartPodId { get; set; }

        [Required]
        public string EndPodId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayName("Price per hour")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PricePerHour { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayName("Price per hour")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PricePerMinute { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; }
    }
}
