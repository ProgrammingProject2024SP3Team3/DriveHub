using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveHubModel
{
    public class Pod
    {
        [Key]
        public Guid PodID { get; set; }

        [ForeignKey("Location")]
        public Guid LocationID { get; set; }

        [Required]
        public string PodName { get; set; }
        public int Capacity { get; set; }
        public int AvailableSpots { get; set; }

        // Navigation properties
        public Location Location { get; set; }

        // One-to-Many relationship: A Pod can have many Vehicles
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
