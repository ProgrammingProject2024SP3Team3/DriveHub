using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Receipt
    {
        [Key]
        public Guid ReceiptID { get; set; }

        [ForeignKey("Booking")]
        public Guid BookingID { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string VehicleModel { get; set; }
        public string LicensePlate { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionID { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
    }
}
