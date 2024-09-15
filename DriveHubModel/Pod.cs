using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DriveHubModel
{
    public class Pod
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PodID { get; set; }

        [ForeignKey("Location")]
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
