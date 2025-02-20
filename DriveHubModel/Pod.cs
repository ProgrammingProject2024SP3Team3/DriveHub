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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class Pod
    {
        public Pod() { }

        [SetsRequiredMembers]
        public Pod(
            string podId,
            int siteId,
            string? vehicleId,
            string podName)
        {
            PodId = podId;
            SiteId = siteId;
            VehicleId = vehicleId;
            PodName = podName;
        }

        [SetsRequiredMembers]
        public Pod(
            int siteId,
            string? vehicleId,
            string podName)
        {
            SiteId = siteId;
            VehicleId = vehicleId;
            PodName = podName;
        }

        [Key]
        [Required]
        public string PodId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Site")]
        [Required]
        [DisplayName("Site")]
        public int SiteId { get; set; }

        [ForeignKey("Vehicle")]
        [MaybeNull]
        [DisplayName("Vehicle")]
        [DisplayFormat(NullDisplayText = "None")]
        public string? VehicleId { get; set; }

        [Required]
        [DisplayName("Pod name")]
        public string PodName { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual Site Site { get; set; }

        [JsonIgnore]
        [MaybeNull]
        [DisplayFormat(NullDisplayText = "None")]
        public virtual Vehicle? Vehicle { get; set; } = null;

        [JsonIgnore]
        public IList<Booking> StartPods { get; set; } = new List<Booking>();

        [JsonIgnore]
        public IList<Booking> EndPods { get; set; } = new List<Booking>();
    }
}
