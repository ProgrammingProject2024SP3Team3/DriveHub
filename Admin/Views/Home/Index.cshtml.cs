using DriveHubModel;

namespace Admin.Views.Home
{
    public class Index
    {
        public int NumberOfUsers { get; set; }
        public int NumberOfTripsTaken { get; set; }
        public int CarsUsed { get; set; }
        public int CarsTotal { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Receipt> Receipts { get; set; }
    }
}
