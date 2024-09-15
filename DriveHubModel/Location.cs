using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LocationID { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public double GPSLatitude { get; set; }

        [Required]
        public double GPSLongitude { get; set; }

        // One-to-Many relationship: A Location can have many Pods
        //public virtual IList<Pod> Pods { get; set; } = new List<Pod>();



    }
}
