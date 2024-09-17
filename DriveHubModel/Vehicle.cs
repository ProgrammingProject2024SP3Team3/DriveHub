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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModels
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleID { get; set; }

        [ForeignKey(nameof(VehicleRate.VehicleRateID))]
        [Required]
        public string VehicleRateID { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Seats { get; set; }
    }
}
