using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DriveHub.Models.Dto
{
    public class EditBookingDto
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        [DisplayName("Car")]
        public string VehicleId { get; set; }

        [Required]
        public string StartPodId { get; set; }

        [Required]
        [DisplayName("Ending pod")]
        public string EndPodId { get; set; }

        [Required]
        [DisplayName("start time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required]
        [DisplayName("end time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(20, 50)]
        [DisplayName("Price per hour")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal QuotedPricePerHour { get; set; }
    }
}
