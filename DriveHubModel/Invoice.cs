﻿/**
 * Receipt
 * 
 * Receipt.cs 18/10/2024
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
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    /// <summary>
    /// An invoice for a completed booking.
    /// </summary>
    public class Invoice
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceNumber { get; set; }

        [Required]
        [ForeignKey(nameof(Booking))]
        public string BookingId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Receipt date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Amount { get; set; }

        [JsonIgnore]
        public virtual Booking Booking { get; set; }
    }
}
