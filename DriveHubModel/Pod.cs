/**
 * Pod.cs 17/09/2024
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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class Pod
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PodId { get; set; }

        [ForeignKey("Site")]
        [Required]
        public int SiteId { get; set; }

        [ForeignKey("Vehicle")]
        [MaybeNull]
        public string? VehicleId { get; set; }

        [Required]
        public string PodName { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Site Site { get; set; }

        [JsonIgnore]
        [MaybeNull]
        public virtual Vehicle? Vehicle { get; set; } = null;
    }
}
