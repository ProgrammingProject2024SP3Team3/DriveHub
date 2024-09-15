//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text.Json.Serialization;

//namespace DriveHubModel
//{
//    public class Rate
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public string RateID { get; set; }

//        [ForeignKey("Vehicle")]
//        [Required]
//        public string VehicleID { get; set; }

//        [Required]
//        [DataType(DataType.Currency)]
//        public decimal PricePerHour { get; set; }

//        [Required]
//        [DataType(DataType.Date)]
//        [DisplayName("Effective Date")]
//        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
//        public DateTime EffectiveDate { get; set; }

//        [JsonIgnore]
//        public Vehicle Vehicle { get; set; }
//    }
//}
