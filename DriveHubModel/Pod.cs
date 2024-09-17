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

namespace DriveHubModels
{
    public class Pod
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PodID { get; set; }

        [ForeignKey(nameof(Location.LocationID))]
        [Required]
        public string LocationID { get; set; }

        [ForeignKey("Vehicle")]
        [MaybeNull]
        public string? VehicleID { get; set; }

        [Required]
        public string PodName { get; set; }

        // Navigation properties
        public virtual Location Location { get; set; }

        public virtual Vehicle? Vehicle { get; set; } = null;
    }
}
