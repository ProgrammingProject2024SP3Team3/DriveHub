/**
 * Create
 * 
 * Create.cs 17/09/2024
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

namespace Admin.Views.Vehicles
{
    public class Create
    {
        [Required]
        public string VehicleId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Category")]
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
        public string Year { get; set; }

        [Required]
        public int Seats { get; set; }

        [Required]
        public string Colour { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Reserved")]
        public bool IsReserved { get; set; }

        public override string ToString()
        {
            return VehicleId + " " + Name + " " + Make + " " + Model + " " + RegistrationPlate + " " + State + " " + Year + " " + Seats + " " + Colour + " " + VehicleRateId;
        }
    }
}
