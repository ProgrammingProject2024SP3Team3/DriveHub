/**
 * VehicleRate.cs 17/09/2024
 *
 * author: Ian McElwaine s3863018@rmit.student.edu.au
 * author: Sean Atherton s3893785@student.rmit.edu.au
 * 
 * This software is the author(s) original academic work.
 * It has been prepared for submission to RMIT University
 * as assessment work for COSC2650 Programming Project
 */

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    [DisplayName("Price Category")]
    public class VehicleRate
    {
        public VehicleRate() { }

        [SetsRequiredMembers]
        public VehicleRate(
            string vehicleRateId,
            string description,
            decimal pricePerHour,
            DateTime effectiveDate
            )
        {
            VehicleRateId = vehicleRateId;
            Description = description;
            PricePerHour = pricePerHour;
            EffectiveDate = effectiveDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleRateId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName("Price per hour")]
        public decimal PricePerHour { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Effective Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }

        [JsonIgnore]
        public virtual IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
