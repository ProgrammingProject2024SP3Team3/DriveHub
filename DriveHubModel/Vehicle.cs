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

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class Vehicle
    {
        public Vehicle() { }

        [SetsRequiredMembers]
        public Vehicle(
            string vehicleId,
            string vehicleRateId,
            string make,
            string model,
            string registrationPlate,
            string state,
            string year,
            int seats,
            string colour,
            string name
            )
        {
            VehicleId = vehicleId;
            VehicleRateId = vehicleRateId;
            Make = make;
            Model = model;
            RegistrationPlate = registrationPlate;
            State = state;
            Year = year;
            Seats = seats;
            Colour = colour;
            Name = name;
        }
         
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VehicleId { get; set; }

        [ForeignKey("VehicleRate")]
        [DisplayName("Rate")]
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
        public string Year { get; set; }

        [Required]
        public int Seats { get; set; }

        [Required]
        public string Colour { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual VehicleRate VehicleRate { get; set; }

        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
