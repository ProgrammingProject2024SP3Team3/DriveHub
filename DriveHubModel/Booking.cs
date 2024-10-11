/**
 * Booking.cs 17/09/2024
 *
 * author: Ian McElwaine s3863018@rmit.student.edu.au
 * author: Sean Atherton s3893785@student.rmit.edu.au
 * 
 * This software is the author(s) original academic work.
 * It has been prepared for submission to RMIT University
 * as assessment work for COSC2650 Programming Project
 */

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace DriveHubModel
{
    public class Booking
    {
        [Key]
        public string BookingId { get; set; }

        [ForeignKey("Vehicle")]
        [Required]
        public string VehicleId { get; set; }

        [ForeignKey("ApplicationUser")]
        [DisplayName("User Id")]
        [Required]
        public string Id { get; set; }

        [ForeignKey("Pod")]
        [DisplayName("Start Pod")]
        [Required]
        public string StartPodId { get; set; }

        [ForeignKey("Pod")]
        [DisplayName("End Pod")]
        [Required]
        public string EndPodId { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "A start time is required")]
        [DisplayName("Start Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayName("End Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PricePerHour { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [JsonIgnore]
        public virtual Pod StartPod { get; set; }

        [JsonIgnore]
        public virtual Pod EndPod { get; set; }

        [JsonIgnore]
        public virtual Vehicle Vehicle { get; set; }
    }

    public enum BookingStatus
    {
        InProgress, // An unpaid booking
        Booked,     // A paid booking
        Edited,     // A paid and edited booking
        Started,    // The car has been picked up
        Complete    // The car has been returned
    }
}
