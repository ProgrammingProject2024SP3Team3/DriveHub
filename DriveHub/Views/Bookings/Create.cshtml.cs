using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DriveHub.Views.Bookings
{
    public class Create
    {
        [Required]
        public string BookingId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [DisplayName("Car")]
        public string VehicleId { get; set; }

        [Required]
        public string StartPodId { get; set; }

        [Required]
        [Range(20, 50)]
        [DisplayName("Price per hour")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal QuotedPricePerHour { get; set; }
    }
}
