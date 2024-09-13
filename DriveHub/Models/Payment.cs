using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public Guid PaymentID { get; set; }

    [ForeignKey("Booking")]
    public Guid BookingID { get; set; }

    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public string TransactionID { get; set; }

    // Navigation properties
    public Booking Booking { get; set; }
}
