using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DriveHubModel
{
    /// <summary>
    /// A message from a customer
    /// </summary>
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-incrementing int
        public int ContactId {  get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(1024)]
        [Display(Prompt = "Your Message")]
        public string Message { get; set; }
    }
}
