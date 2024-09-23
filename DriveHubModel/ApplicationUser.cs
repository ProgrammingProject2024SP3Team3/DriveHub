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
using System.Text.Json.Serialization;

namespace DriveHubModel
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public virtual IList<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
