using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models.Dto
{
    public class ResetPasswordDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$",
         ErrorMessage = "Passwords must be 6 characters minimum and contain upper case, lower case, number, and special character")]
        [DisplayName("New password")]
        public string NewPassword { get; set; }
    }
}
