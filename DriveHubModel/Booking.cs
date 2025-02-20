﻿/**
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
        public Booking() { }

        public Booking(
            string id,
            string vehicleId,
            string startPodId,
            decimal pricePerHour,
            decimal pricePerMinute
            )
        {
            Id = id;
            VehicleId = vehicleId;
            StartPodId = startPodId;
            PricePerHour = pricePerHour;
            PricePerMinute = pricePerMinute;
        }

        [Key]
        public string BookingId { get; set; } = Guid.NewGuid().ToString();

        [DisplayName("Reservation expiry")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}")]
        public DateTime Expires { get; set; } = DateTime.Now.AddHours(1);

        public bool IsExtended { get; set; } = false;

        public string PaymentId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Vehicle))]
        [Required]
        public string VehicleId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        [DisplayName("User Id")]
        [Required]
        public string Id { get; set; }

        [ForeignKey(nameof(Pod))]
        [DisplayName("Start Pod")]
        [Required]
        public string StartPodId { get; set; }

        [ForeignKey(nameof(Pod))]
        [DisplayName("End Pod")]
        public string? EndPodId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Start Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("End Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayName("Price p/h")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PricePerHour { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal PricePerMinute { get; set; }

        [Required]
        [DisplayName("Booking status")]
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Reserved;

        [JsonIgnore]
        [DisplayName("User")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [JsonIgnore]
        [DisplayName("Start pod")]
        public virtual Pod StartPod { get; set; }

        [JsonIgnore]
        [DisplayName("End pod")]
        public virtual Pod? EndPod { get; set; } = null;

        [JsonIgnore]
        public virtual Vehicle Vehicle { get; set; }

        [JsonIgnore]
        public virtual Invoice? Invoice { get; set; } = null;

        [JsonIgnore]
        public virtual Receipt? Receipt { get; set; } = null;

        public override string ToString()
        {
            return $"{BookingId} {VehicleId} {Id} {BookingStatus}";
        }
    }

    /// <summary>
    /// Represents the current point in the Booking lifecycle
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// A reservation for one hr
        /// </summary>
        Reserved,

        /// <summary>
        /// The vehicle has been picked up
        /// </summary>
        Collected,

        /// <summary>
        /// The trip is complete but unpaid
        /// </summary>
        Unpaid,

        /// <summary>
        /// The trip has been completed and paid
        /// </summary>
        Complete,

        /// <summary>
        /// An expired reservation
        /// </summary>
        Expired,

        /// <summary>
        /// An cancelled reservation
        /// </summary>
        Cancelled
    }
}
