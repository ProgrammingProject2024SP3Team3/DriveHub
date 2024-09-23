/**
 * Vehicle
 * 
 * Vehicle.cs 17/09/2024
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
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleId { get; set; }

        [ForeignKey("VehicleRate")]
        [Required]
        public string VehicleRateId { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [DisplayName("Registration Plate")]
        public string RegistrationPlate { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Seats { get; set; }

        [JsonIgnore]
        public virtual VehicleRate VehicleRate { get; set; }

        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
