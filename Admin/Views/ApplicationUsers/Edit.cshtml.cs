using System.ComponentModel.DataAnnotations;

namespace Admin.Views.ApplicationUsers
{
    public class Edit
    {
        public Edit() { }

        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Email confirmed")]
        public bool EmailConfirmed { get; set; }

        [RegularExpression(@"^04[0-9]{8}", ErrorMessage = "Must be in the format 04xxxxxxxx")]
        [Display(Name = "Mobile number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Mobile number confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [Display(Name = "Two factor enabled")]
        public bool TwoFactorEnabled { get; set; }
    }
}