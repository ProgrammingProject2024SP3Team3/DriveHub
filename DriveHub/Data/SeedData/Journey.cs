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

using System.ComponentModel.DataAnnotations;

namespace DriveHub.SeedData
{
    public class Journey
    {
        public string JourneyId { get; set; }

        public string BookingId { get; set; }

        public bool IsPaid { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
    }
}
