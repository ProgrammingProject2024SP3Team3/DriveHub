 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Location
{
    [Key]
    public Guid LocationID { get; set; }

    [Required]
    public string Address { get; set; }

    public string City { get; set; }
    public string PostalCode { get; set; }
    public decimal GPSLatitude { get; set; }
    public decimal GPSLongitude { get; set; }

    // One-to-Many relationship: A Location can have many Vehicles
    public ICollection<Vehicle> Vehicles { get; set; }

    // One-to-Many relationship: A Location can have many Bookings (start and end locations)
    public ICollection<Booking> Bookings { get; set; }
}
