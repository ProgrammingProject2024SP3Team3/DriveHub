/**
 * Journey.cs 22/09/2024
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

namespace DriveHubModel
{
    public class Journey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string JourneyId { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public string BookingId { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
