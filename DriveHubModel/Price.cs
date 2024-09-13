using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Price
    {
        [Key]
        public Guid PriceID { get; set; }

        [ForeignKey("Vehicle")]
        public Guid VehicleID { get; set; }

        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal Discount { get; set; }
        public DateTime EffectiveDate { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; }
    }
}
