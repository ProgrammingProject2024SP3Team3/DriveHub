using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Admin.Views.ApplicationUsers
{
    public class Create
    {
        public Create() { }

        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Email confirmed")]
        public bool EmailConfirmed { get; set; }

        [MaybeNull]
        [Display(Name = "Mobile number")]
        [RegularExpression(@"^04[0-9]{8}", ErrorMessage = "Must be in the format 04xxxxxxxx")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Mobile number confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}