/**
 * Site.cs 17/09/2024
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
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace DriveHubModel
{
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-incrementing int
        public int SiteId { get; set; }

        [Required]
        public string SiteName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        // NetTopologySuite spatial data
        [Required]
        public Point Location { get; set; }  // Spatial data

        // One-to-Many relationship: A Site can have many Pods
        [JsonIgnore]
        public virtual IList<Pod> Pods { get; set; } = new List<Pod>();
    }
}
