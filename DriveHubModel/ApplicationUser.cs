/**
 * ApplicationUser.cs 22/09/2024
 *
 * author: Ian McElwaine s3863018@rmit.student.edu.au
 * author: Sean Atherton s3893785@student.rmit.edu.au
 * 
 * This software is the author(s) original academic work.
 * It has been prepared for submission to RMIT University
 * as assessment work for COSC2650 Programming Project
 */

using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        [Required]
        [PersonalData]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
