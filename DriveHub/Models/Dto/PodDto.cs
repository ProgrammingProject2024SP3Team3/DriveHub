/**
 * PodDto.cs 30/09/2024
 *
 * author: Ian McElwaine s3863018@rmit.student.edu.au
 * author: Sean Atherton s3893785@student.rmit.edu.au
 * 
 * This software is the author(s) original academic work.
 * It has been prepared for submission to RMIT University
 * as assessment work for COSC2650 Programming Project
 */

namespace DriveHub.Models.Dto
{
    public class PodDto
    {
        public string PodId { get; set; }

        public string PodName { get; set; }

        public string SiteName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string? VehicleName { get; set; }

        public string? VehicleCategory { get; set; }

        public decimal? PricePerHour { get; set; }
    }
}
