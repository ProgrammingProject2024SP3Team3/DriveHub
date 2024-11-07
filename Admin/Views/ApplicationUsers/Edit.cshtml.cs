using System.ComponentModel.DataAnnotations;

namespace Admin.View.ApplicationUsers
{
    public class Edit
    {
        public Edit() { }

        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public bool PhoneNumberConfirmed { get; set; }
    }
}