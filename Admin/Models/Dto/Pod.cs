using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Admin.Models.Dto
{
    public class Pod
    {
        public Pod() { }

        public string PodId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [DisplayName("Site name")]
        public int SiteId { get; set; }

        [MaybeNull]
        [DisplayName("Vehicle")]
        [DisplayFormat(NullDisplayText = "None")]
        public string? VehicleId { get; set; }

        [Required]
        [DisplayName("Pod name")]
        public string PodName { get; set; }
    }
}
